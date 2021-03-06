﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace StockSimulator.Core
{
	/// <summary>
	/// Holds all the orders that are placed from every ticker and strategy. 
	/// Allows lookup for that order by ticker, strategy, or order id
	/// </summary>
	public class OrderHistory : IOrderHistory
	{
		public ConcurrentDictionary<int, List<Order>> TickerDictionary { get; set; }
		public ConcurrentDictionary<int, ConcurrentBag<Order>> StrategyDictionary { get; set; }

		/// <summary>
		/// Initialize the object.
		/// </summary>
		public OrderHistory()
		{
			TickerDictionary = new ConcurrentDictionary<int, List<Order>>();
			StrategyDictionary = new ConcurrentDictionary<int, ConcurrentBag<Order>>();
		}

		/// <summary>
		/// Adds an order to all the dictionaries for searching by multiple key types.
		/// </summary>
		/// <param name="order">The order to add</param>
		/// <param name="dependentIndicators">Indicators used when making a decision to place this order</param>
		/// <param name="currentBar">Current bar the order is being added in</param>
		public void AddOrder(Order order, List<Indicator> dependentIndicators, int currentBar)
		{
			AddToListTable(TickerDictionary, order, order.Ticker.TickerAndExchange.GetHashCode());
			AddToListTableConcurrent(StrategyDictionary, order, order.StrategyName.GetHashCode());

			SaveSnapshot(order, dependentIndicators, currentBar);
		}

		/// <summary>
		/// Frees the orders for a ticker when it finished.
		/// </summary>
		/// <param name="tickerAndExchange">Ticker to free</param>
		public void PurgeTickerOrders(TickerExchangePair tickerAndExchange)
		{
		}

		/// <summary>
		/// Calculates things like win/loss percent, gain, etc. for the strategy used on the ticker.
		/// </summary>
		/// <param name="strategyName">Name of the strategy the statistics are for</param>
		/// <param name="orderType">Type of orders placed with this strategy (long or short)</param>
		/// <param name="tickerAndExchange">Ticker the strategy used</param>
		/// <param name="currentBar">Current bar of the simulation</param>
		/// <param name="maxBarsAgo">Maximum number of bars in the past to consider for calculating</param>
		/// <returns>Class holding the statistics calculated</returns>
		public StrategyStatistics GetStrategyStatistics(string strategyName, double orderType, TickerExchangePair tickerAndExchange, int currentBar, int maxBarsAgo)
		{
			// Orders that started less than this bar will not be considered.
			int cutoffBar = currentBar - maxBarsAgo;
			if (cutoffBar < 0)
			{
				cutoffBar = 0;
			}

			// Get the list of orders to search.
			StrategyStatistics stats = new StrategyStatistics(strategyName, orderType);
			List<Order> orderList = null;
			if (strategyName.Length > 0 && tickerAndExchange == null)
			{
				int strategyKey = strategyName.GetHashCode();
				if (StrategyDictionary.ContainsKey(strategyKey))
				{
					orderList = StrategyDictionary[strategyKey].ToList();
				}
			}
			else if (tickerAndExchange != null)
			{
				int tickerHash = tickerAndExchange.GetHashCode();
				if (TickerDictionary.ContainsKey(tickerHash))
				{
					orderList = TickerDictionary[tickerHash];
				}
			}

			if (orderList != null)
			{
				for (int i = orderList.Count - 1; i >= 0; i--)
				{
					Order order = orderList[i];

					if (order.IsFinished() && 
						order.StrategyName == strategyName &&
						order.BuyBar >= cutoffBar && 
						order.Type == orderType &&
						stats.NumberOfOrders < Simulator.Config.MaxLookBackOrders)
					{
						stats.AddOrder(order);
					}
				}
			}

			if (stats.NumberOfOrders > Simulator.Config.MinRequiredOrders)
			{
				stats.CalculateStatistics();
			}
			else
			{
				stats = new StrategyStatistics(strategyName, orderType);
			}

			return stats;
		}

		/// <summary>
		/// Calculates things like win/loss percent, gain, etc. for the ticker.
		/// </summary>
		/// <param name="tickerAndExchange">Ticker to calculate for</param>
		/// <param name="currentBar">Current bar of the simulation</param>
		/// <param name="maxBarsAgo">Maximum number of bars in the past to consider for calculating</param>
		/// <returns>Class holding the statistics calculated</returns>
		public StrategyStatistics GetTickerStatistics(TickerExchangePair tickerAndExchange, int currentBar, int maxBarsAgo)
		{
			// Orders that started less than this bar will not be considered.
			int cutoffBar = currentBar - maxBarsAgo;
			if (cutoffBar < 0)
			{
				cutoffBar = 0;
			}

			// Order type doesn't matter here since we are just using this class to 
			// output overall ticker info which could be from any order type. It will
			// get ignored on the web output display.
			StrategyStatistics stats = new StrategyStatistics(tickerAndExchange.ToString(), Order.OrderType.Long);

			int tickerHash = tickerAndExchange.GetHashCode();
			if (TickerDictionary.ContainsKey(tickerHash))
			{
				List<Order> tickerOrders = TickerDictionary[tickerHash];

				for (int i = tickerOrders.Count - 1; i >= 0; i--)
				{
					Order order = tickerOrders[i];
					if (order.BuyBar >= cutoffBar)
					{
						stats.AddOrder(order);
					}
				}
			}

			// Only count the statistics if we have a bit more data to deal with.
			// We want to avoid having a strategy say it's 100% correct when it 
			// only has 1 winning trade.
			if (stats.NumberOfOrders > Simulator.Config.MinRequiredOrders)
			{
				stats.CalculateStatistics();
			}
			else
			{
				// For the same reasons as earlier in this function, order type doesn't matter here.
				stats = new StrategyStatistics(tickerAndExchange.ToString(), Order.OrderType.Long);
			}

			return stats;
		}

		/// <summary>
		/// Saves the indicator series for this order so that during analysis we can see
		/// exactly what the indicators looked like at the time the order was placed.
		/// </summary>
		/// <param name="order">The order to add</param>
		/// <param name="dependentIndicators">Indicators used when making a decision to place this order</param>
		/// <param name="currentBar">Current bar the order was placed.</param>
		private void SaveSnapshot(Order order, List<Indicator> dependentIndicators, int currentBar)
		{
			if (Simulator.Config.OnlyMainStrategySnapshots && !(order is MainStrategyOrder))
			{
				return;
			}

			Simulator.DataOutput.OutputIndicatorSnapshots(order, dependentIndicators, currentBar);
		}

		/// <summary>
		/// Adds an order to a dictionary that has a list of orders indexed by a string hash.
		/// </summary>
		/// <param name="table">Table to add to</param>
		/// <param name="order">Order to add</param>
		/// <param name="hash">Hash index for the order list</param>
		private void AddToListTable(ConcurrentDictionary<int, List<Order>> table, Order order, int hash)
		{
			if (table.ContainsKey(hash) == false)
			{
				table[hash] = new List<Order>();
			}
			table[hash].Add(order);
		}

		/// <summary>
		/// Adds an order to a dictionary that has a list of orders indexed by a string hash.
		/// </summary>
		/// <param name="table">Table to add to</param>
		/// <param name="order">Order to add</param>
		/// <param name="hash">Hash index for the order list</param>
		private void AddToListTableConcurrent(ConcurrentDictionary<int, ConcurrentBag<Order>> table, Order order, int hash)
		{
			if (table.ContainsKey(hash) == false)
			{
				table[hash] = new ConcurrentBag<Order>();
			}
			table[hash].Add(order);
		}
	}
}
