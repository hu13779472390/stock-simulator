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
	public class OrderHistory
	{
		public ConcurrentDictionary<long, Order> IdDictionary { get; set; }
		public ConcurrentDictionary<int, List<Order>> TickerDictionary { get; set; }
		public ConcurrentDictionary<int, ConcurrentBag<Order>> StrategyDictionary { get; set; }

		/// <summary>
		/// Initialize the object.
		/// </summary>
		public OrderHistory()
		{
			IdDictionary = new ConcurrentDictionary<long, Order>();
			TickerDictionary = new ConcurrentDictionary<int, List<Order>>();
			StrategyDictionary = new ConcurrentDictionary<int, ConcurrentBag<Order>>();
		}

		/// <summary>
		/// Adds an order to all the dictionaries for searching by multiple key types.
		/// </summary>
		/// <param name="order">The order to add</param>
		public void AddOrder(Order order)
		{
			if (IdDictionary.ContainsKey(order.OrderId))
			{
				throw new Exception("Duplicate order ids found");	
			}
			
			IdDictionary[order.OrderId] = order;
			AddToListTable(TickerDictionary, order, order.Ticker.TickerAndExchange.GetHashCode());
			AddToListTableConcurrent(StrategyDictionary, order, order.StrategyName.GetHashCode());
		}

		/// <summary>
		/// Calculates things like win/loss percent, gain, etc. for the strategy used on the ticker.
		/// </summary>
		/// <param name="strategyName">Name of the strategy the statistics are for</param>
		/// <param name="tickerAndExchange">Ticker the strategy used</param>
		/// <param name="currentBar">Current bar of the simulation</param>
		/// <param name="maxBarsAgo">Maximum number of bars in the past to consider for calculating</param>
		/// <returns>Class holding the statistics calculated</returns>
		public StrategyStatistics GetStrategyStatistics(string strategyName, TickerExchangePair tickerAndExchange, int currentBar, int maxBarsAgo)
		{
			// Orders that started less than this bar will not be considered.
			int cutoffBar = currentBar - maxBarsAgo;
			if (cutoffBar < 0)
			{
				cutoffBar = 0;
			}

			StrategyStatistics stats = new StrategyStatistics(strategyName);

			int tickerHash = tickerAndExchange.GetHashCode();
			if (TickerDictionary.ContainsKey(tickerHash))
			{
				List<Order> tickerOrders = TickerDictionary[tickerHash];

				for (int i = tickerOrders.Count - 1; i >= 0; i--)
				{
					Order order = tickerOrders[i];
					bool shouldAddOrder = false;

					// For the date
					if (Simulator.Config.UseLookbackBars)
					{
						shouldAddOrder = order.BuyBar >= cutoffBar && order.IsFinished() && order.StrategyName == strategyName;
					}
					// For the number of orders as the cutoff
					else
					{
						shouldAddOrder = stats.NumberOfOrders < Simulator.Config.MaxLookBackOrders && order.IsFinished() && order.StrategyName == strategyName;
					}

					if (shouldAddOrder == true)
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
				stats = new StrategyStatistics(strategyName);
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

			StrategyStatistics stats = new StrategyStatistics(tickerAndExchange.ToString());

			int tickerHash = tickerAndExchange.GetHashCode();
			if (TickerDictionary.ContainsKey(tickerHash))
			{
				List<Order> tickerOrders = TickerDictionary[tickerHash];

				for (int i = tickerOrders.Count - 1; i >= 0; i--)
				{
					Order order = tickerOrders[i];
					if (order.BuyBar >= cutoffBar && order.IsFinished())
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
				stats = new StrategyStatistics(tickerAndExchange.ToString());
			}

			return stats;
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
