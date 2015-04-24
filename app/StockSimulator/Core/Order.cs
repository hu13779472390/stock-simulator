﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StockSimulator.Core.JsonConverters;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading;

namespace StockSimulator.Core
{
	[JsonObject(MemberSerialization.OptIn)]
	public class Order
	{
		/// <summary>
		/// Different types of orders that can be placed.
		/// </summary>
		public enum OrderType
		{
			Long,
			Short
		}

		/// <summary>
		/// The status of the order from it's start to end.
		/// </summary>
		public enum OrderStatus
		{
			Open,
			Filled,
			ProfitTarget,
			StopTarget,
			LengthExceeded,
			Cancelled
		}

		[JsonProperty("buyPrice")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double BuyPrice { get; set; }
		
		[JsonProperty("sellPrice")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double SellPrice { get; set; }

		[JsonProperty("buyDate")]
		[JsonConverter(typeof(ShortDateTimeConverter))]
		public DateTime BuyDate { get; set; }

		[JsonProperty("sellDate")]
		[JsonConverter(typeof(ShortDateTimeConverter))]
		public DateTime SellDate { get; set; }

		[JsonProperty("numShares")]
		public int NumberOfShares { get; set; }

		[JsonProperty("id")]
		public long OrderId { get; set; }

		[JsonProperty("gain")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double Gain { get; set; }

		[JsonProperty("accountValue")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double AccountValue { get; set; }

		[JsonProperty("ticker")]
		[JsonConverter(typeof(TickerDataStringConverter))]
		public TickerData Ticker { get; set; }

		[JsonProperty("orderStatus")]
		public OrderStatus Status { get; set; }

		public int BuyBar { get; set; }
		public int SellBar { get; set; }
		public OrderType Type { get; set; }
		public double ProfitTargetPrice { get; set; }
		public double StopPrice { get; set; }
		public string StrategyName { get; set; }
		public double Value { get; set; }
		public StrategyStatistics StartStatistics { get; set; }
		public StrategyStatistics EndStatistics { get; set; }
		public List<string> DependentIndicatorNames { get; set; }

		private double LimitBuyPrice { get; set; }
		private int LimitOpenedBar { get; set; }

		/// <summary>
		/// Contructor for the order.
		/// </summary>
		/// <param name="type">Type of order we're placing, long or short</param>
		/// <param name="tickerData">Ticker data</param>
		/// <param name="currentBar">Current bar of the simulation</param>
		/// <param name="fromStrategyName">Name of the strategy this order is for. Can't use the actual strategy reference because it could come from a strategy combo (ie. MacdCrossover-SmaCrossover)</paramparam>
		/// <param name="dependentIndicatorNames">Names of the dependent indicators so they can be shown on the web with the order</param>
		public Order(OrderType type, TickerData tickerData, string fromStrategyName, int currentBar, List<string> dependentIndicatorNames)
		{
			StrategyName = fromStrategyName;
			OrderId = GetUniqueId();
			Type = type;
			Ticker = tickerData;
			DependentIndicatorNames = dependentIndicatorNames;
			AccountValue = 0;
			LimitBuyPrice = tickerData.Close[currentBar];
			LimitOpenedBar = currentBar;

			// Get things like win/loss percent up to the point this order was finished.
			StartStatistics = Simulator.Orders.GetStrategyStatistics(StrategyName,
				type,
				Ticker.TickerAndExchange,
				currentBar,
				Simulator.Config.MaxLookBackBars);
		}

		/// <summary>
		/// Updates the order for this bar
		/// </summary>
		/// <param name="curBar">Current bar of the simulation</param>
		public void Update(int curBar)
		{
			// If the order is open and not filled we need to fill it.
			if (Status == OrderStatus.Open)
			{
				// If we are using limit orders make sure the price is higher than that 
				// limit before buying.
				if (Simulator.Config.UseLimitOrders)
				{
					if (curBar - LimitOpenedBar >= Simulator.Config.MaxBarsLimitOrderFill)
					{
						Status = OrderStatus.Cancelled;
					}
					else if (Ticker.Open[curBar] >= LimitBuyPrice)
					{
						BuyPrice = Ticker.Open[curBar];
					}
					else if (Ticker.Close[curBar] > LimitBuyPrice || Ticker.High[curBar] > LimitBuyPrice)
					{
						BuyPrice = LimitBuyPrice;
					}
				}
				else
				{
					BuyPrice = Ticker.Open[curBar];
				}

				if (BuyPrice > 0)
				{
					BuyBar = curBar;
					BuyDate = Ticker.Dates[curBar];
					Status = OrderStatus.Filled;

					double sizeOfOrder = Simulator.Config.SizeOfOrder;
					// TODO: not sure I like this since it's not a fixed order. Its hard to know how much of
					// and account value we'll need to make a consistent profit. Seems easier to just always 
					// place the same amount so we can tinker with our initial value and find out how much
					// an investment we'll need to make enough money for a living. Which is the goal =p.
					//if (GetType() == typeof(MainStrategyOrder))
					//{
					//	double accountValue = (double)Simulator.Broker.AccountValue[curBar > 0 ? curBar - 1 : curBar][1];
						
					//	// We only ever want to gain (or lose) a percent of our account value.
					//	sizeOfOrder = (accountValue * Simulator.Config.PercentGainPerTrade) / Simulator.Config.ProfitTarget;

					//	// Make sure we don't have an order of an unrealistic amount.
					//	if (sizeOfOrder > Simulator.Config.MaxOrderSize)
					//	{
					//		sizeOfOrder = Simulator.Config.MaxOrderSize;
					//	}
					//}

					NumberOfShares = BuyPrice > 0.0 ? Convert.ToInt32(Math.Floor(sizeOfOrder / BuyPrice)) : 0;
					Value = NumberOfShares * BuyPrice;

					double direction = Type == OrderType.Long ? 1.0 : -1.0;

					// Set prices to exit.
					ProfitTargetPrice = BuyPrice + ((BuyPrice * Simulator.Config.ProfitTarget) * direction);
					StopPrice = BuyPrice - ((BuyPrice * Simulator.Config.StopTarget) * direction);
				}
			}

			// Close any orders that need to be closed
			if (Status == OrderStatus.Filled)
			{
				Value = NumberOfShares * Ticker.Close[curBar];

				if (Type == OrderType.Long)
				{
					FinishLongOrder(curBar);
				}
				else
				{
					FinishShortOrder(curBar);
				}

				// Limit the order since we won't want to be in the market forever.
				if (curBar - BuyBar >= Simulator.Config.MaxBarsOrderOpen)
				{
					FinishOrder(Ticker.Close[curBar], curBar, OrderStatus.LengthExceeded);
				}
			}
		}

		/// <summary>
		/// Returns whether the order is closed.
		/// </summary>
		/// <returns>True if the order is any of the closed statuses</returns>
		public bool IsFinished()
		{
			return Status == OrderStatus.ProfitTarget ||
				Status == OrderStatus.StopTarget ||
				Status == OrderStatus.LengthExceeded;
		}

		/// <summary>
		/// Checks all the conditions that could close a long order and if any
		/// are true then close the order.
		/// </summary>
		/// <param name="curBar">Current bar of the simulation</param>
		private void FinishLongOrder(int curBar)
		{
			// Gapped open above our profit target, then close at the open price.
			if (Ticker.Open[curBar] >= ProfitTargetPrice)
			{
				FinishOrder(Ticker.Open[curBar], curBar, OrderStatus.ProfitTarget);
			}
			// Either the high or close during this bar was above our profit target,
			// then close at the profit target.
			else if (Math.Max(Ticker.Close[curBar], Ticker.High[curBar]) >= ProfitTargetPrice)
			{
				FinishOrder(ProfitTargetPrice, curBar, OrderStatus.ProfitTarget);
			}
			// Gapped open below our stop target, so close at the open price.
			else if (Ticker.Open[curBar] <= StopPrice)
			{
				FinishOrder(Ticker.Open[curBar], curBar, OrderStatus.StopTarget);
			}
			// Either the low or close during this bar was below our stop target,
			// then close at the stop target.
			else if (Math.Min(Ticker.Close[curBar], Ticker.Low[curBar]) <= StopPrice)
			{
				FinishOrder(StopPrice, curBar, OrderStatus.StopTarget);
			}
		}

		/// <summary>
		/// Checks all the conditions that could close a short order and if any
		/// are true then close the order.
		/// </summary>
		/// <param name="curBar">Current bar of the simulation</param>
		private void FinishShortOrder(int curBar)
		{
			// Gapped open below our profit target, then close at the open price.
			if (Ticker.Open[curBar] <= ProfitTargetPrice)
			{
				FinishOrder(Ticker.Open[curBar], curBar, OrderStatus.ProfitTarget);
			}
			// Either the low or close during this bar was below our profit target,
			// then close at the profit target.
			else if (Math.Max(Ticker.Close[curBar], Ticker.Low[curBar]) <= ProfitTargetPrice)
			{
				FinishOrder(ProfitTargetPrice, curBar, OrderStatus.ProfitTarget);
			}
			// Gapped open above our stop target, so close at the open price.
			else if (Ticker.Open[curBar] >= StopPrice)
			{
				FinishOrder(Ticker.Open[curBar], curBar, OrderStatus.StopTarget);
			}
			// Either the high or close during this bar was above our stop target,
			// then close at the stop target.
			else if (Math.Min(Ticker.Close[curBar], Ticker.High[curBar]) >= StopPrice)
			{
				FinishOrder(StopPrice, curBar, OrderStatus.StopTarget);
			}
		}

		/// <summary>
		/// Closes the order and records current stats for orders for this strategy.
		/// </summary>
		/// <param name="sellPrice">Price the stock was sold at</param>
		/// <param name="currentBar">Current bar of the simulation</param>
		private void FinishOrder(double sellPrice, int currentBar, OrderStatus sellStatus)
		{
			double direction = Type == OrderType.Long ? 1.0 : -1.0;

			// If the sell price is 0 then it's a bug that no more data for this stock exists 
			// and we had an order open. This is not really realistic so we'll just have the order
			// gain $0.
			SellPrice = sellPrice > 0.0 ? sellPrice : BuyPrice;
			SellBar = currentBar;
			SellDate = Ticker.Dates[currentBar];
			Status = sellStatus;
			Value = NumberOfShares * SellPrice;
			Gain = (Value - (NumberOfShares * BuyPrice)) * direction;

			// Get things like win/loss percent up to the point this order was finished.
			// TODO: not sure if this is needed.
			//EndStatistics = Simulator.Orders.GetStrategyStatistics(StrategyName,
			//	Ticker.TickerAndExchange.ToString(),
			//	currentBar,
			//	Simulator.Config.MaxLookBackBars);
		}

		/// <summary>
		/// Returns a unique order id to make sure we can lookup orders easily.
		/// </summary>
		private static long _uniqueId = 0;
		private static long GetUniqueId()
		{
			return Interlocked.Increment(ref _uniqueId);
		}
	}
}
