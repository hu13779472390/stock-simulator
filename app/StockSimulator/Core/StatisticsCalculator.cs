﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StockSimulator.Core.JsonConverters;

namespace StockSimulator.Core
{
	/// <summary>
	/// Calculates statistics from orders that are added.
	/// </summary>
	[JsonObject(MemberSerialization.OptIn)]
	public class StatisticsCalculator
	{
		[JsonProperty("numberOfOrders")]
		public int NumberOfOrders { get; set; }

		[JsonProperty("winPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double WinPercent { get; set; }

		[JsonProperty("lossPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LossPercent { get; set; }

		[JsonProperty("profitTargetPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ProfitTargetPercent { get; set; }

		[JsonProperty("stopLossPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double StopLossPercent { get; set; }

		[JsonProperty("lengthExceededPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LengthExceededPercent { get; set; }

		[JsonProperty("largestWinner")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LargestWinner { get; set; }

		[JsonProperty("largestWinnerBuyDate")]
		[JsonConverter(typeof(ShortDateTimeConverter))]
		public DateTime LargestWinnerBuyDate { get; set; }

		[JsonProperty("largestLoser")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LargestLoser { get; set; }

		[JsonProperty("largestLoserBuyDate")]
		[JsonConverter(typeof(ShortDateTimeConverter))]
		public DateTime LargestLoserBuyDate { get; set; }

		[JsonProperty("mostConsecutiveLosers")]
		public int MostConsecutiveLosers { get; set; }

		[JsonProperty("mostConsecutiveLosersDate")]
		[JsonConverter(typeof(ShortDateTimeConverter))]
		public DateTime MostConsecutiveLosersDate { get; set; }

		[JsonProperty("maxDrawdown")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double MaxDrawdown { get; set; }

		[JsonProperty("maxDrawdownDate")]
		[JsonConverter(typeof(ShortDateTimeConverter))]
		public DateTime MaxDrawdownDate { get; set; }

		[JsonProperty("profitFactor")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ProfitFactor { get; set; }

		[JsonProperty("profitFactorLargest")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ProfitFactorLargest { get; set; }

		////////////////////// LONG //////////////////////

		[JsonProperty("longWinPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LongWinPercent { get; set; }

		[JsonProperty("longLossPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LongLossPercent { get; set; }

		[JsonProperty("longProfitTargetPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LongProfitTargetPercent { get; set; }

		[JsonProperty("longStopLossPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LongStopLossPercent { get; set; }

		[JsonProperty("longLengthExceededPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LongLengthExceededPercent { get; set; }

		[JsonProperty("longWinAvg")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LongWinAvg { get; set; }

		[JsonProperty("longWinAvgPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LongWinAvgPercent { get; set; }

		[JsonProperty("longLossAvg")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LongLossAvg { get; set; }

		[JsonProperty("longLossAvgPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double LongLossAvgPercent { get; set; }

		[JsonProperty("longNumberOfOrders")]
		public long LongNumberOfOrders { get; set; }

		////////////////////// SHORT /////////////////////

		[JsonProperty("shortWinPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ShortWinPercent { get; set; }

		[JsonProperty("shortLossPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ShortLossPercent { get; set; }

		[JsonProperty("shortProfitTargetPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ShortProfitTargetPercent { get; set; }

		[JsonProperty("shortStopLossPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ShortStopLossPercent { get; set; }

		[JsonProperty("shortLengthExceededPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ShortLengthExceededPercent { get; set; }

		[JsonProperty("shortWinAvg")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ShortWinAvg { get; set; }

		[JsonProperty("shortWinAvgPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ShortWinAvgPercent { get; set; }

		[JsonProperty("shortLossAvg")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ShortLossAvg { get; set; }

		[JsonProperty("shortLossAvgPercent")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double ShortLossAvgPercent { get; set; }

		[JsonProperty("shortNumberOfOrders")]
		public long ShortNumberOfOrders { get; set; }

	
		[JsonProperty("gain")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double Gain { get; set; }

		[JsonProperty("averageOrderLength")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double AverageOrderLength { get; set; }

		[JsonProperty("averageProfitOrderLength")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double AverageProfitOrderLength { get; set; }

		[JsonProperty("averageStopOrderLength")]
		[JsonConverter(typeof(RoundedDoubleConverter))]
		public double AverageStopOrderLength { get; set; }

		// Not serialized!
		public virtual List<Order> Orders { get; set; }

		private long _numberOfWins = 0;
		private long _numberOfLosses = 0;
		private long _numberOfProfitTargets = 0;
		private long _numberOfStopLosses = 0;
		private long _numberOfLengthExceeded = 0;

		private long _longNumberOfWins = 0;
		private long _longNumberOfLosses = 0;
		private long _longNumberOfProfitTargets = 0;
		private long _longNumberOfStopLosses = 0;
		private long _longNumberOfLengthExceeded = 0;

		private long _shortNumberOfWins = 0;
		private long _shortNumberOfLosses = 0;
		private long _shortNumberOfProfitTargets = 0;
		private long _shortNumberOfStopLosses = 0;
		private long _shortNumberOfLengthExceeded = 0;

		private long _totalLengthOfAllOrders = 0;
		private long _totalLengthOfProfitOrders = 0;
		private long _totalLengthOfStopOrders = 0;

		private double _totalGain = 0;
		private double _highestGain = 0;

		private double _totalMoneyWins = 0;
		private double _totalMoneyLosses = 0;

		private int _currentConsecutiveLosers = 0;

		/// <summary>
		/// Constructor that doesn't calculate the stats to be used with add order.
		/// </summary>
		public StatisticsCalculator()
		{
			Orders = null;

			LargestWinner = 0;
			LargestWinnerBuyDate = DateTime.MinValue;
			LargestLoser = 0;
			LargestLoserBuyDate = DateTime.MinValue;

			MostConsecutiveLosers = 0;
			MostConsecutiveLosersDate = DateTime.MinValue;
			MaxDrawdown = 0;
			MaxDrawdownDate = DateTime.MinValue;

			LongWinAvg = 0;
			LongWinAvgPercent = 0;
			LongLossAvg = 0;
			LongLossAvgPercent = 0;

			ShortWinAvg = 0;
			ShortWinAvgPercent = 0;
			ShortLossAvg = 0;
			ShortLossAvgPercent = 0;
		}

		/// <summary>
		/// Adds an order to the list so the percents can be calculated later.
		/// </summary>
		/// <param name="order"></param>
		public void AddOrder(Order order)
		{
			if (order.IsFinished())
			{
				// The only time this is used is if the StrategyTickerPairStatisctics uses it
				// for outputting all the order data.
				if (Orders != null)
				{
					Orders.Add(order);
				}

				++NumberOfOrders;
				if (order.Gain > 0)
				{
					++_numberOfWins;
					_currentConsecutiveLosers = 0;
					_totalMoneyWins += order.Gain;

					if (order.Gain > LargestWinner)
					{
						LargestWinner = order.Gain;
						LargestWinnerBuyDate = order.BuyDate;
					}
				}
				else
				{
					++_numberOfLosses;
					++_currentConsecutiveLosers;
					_totalMoneyLosses += Math.Abs(order.Gain);

					if (order.Gain < LargestLoser)
					{
						LargestLoser = order.Gain;
						LargestLoserBuyDate = order.BuyDate;
					}

					if (_currentConsecutiveLosers > MostConsecutiveLosers)
					{
						MostConsecutiveLosers = _currentConsecutiveLosers;
						MostConsecutiveLosersDate = order.BuyDate;
					}
				}

				if (order.SellReason == Order.SellReasonType.ProfitTarget)
				{
					++_numberOfProfitTargets;
					_totalLengthOfProfitOrders += order.SellBar - order.BuyBar;
				}
				else if (order.SellReason == Order.SellReasonType.StopLoss)
				{
					++_numberOfStopLosses;
					_totalLengthOfStopOrders += order.SellBar - order.BuyBar;
				}
				else if (order.SellReason == Order.SellReasonType.LengthExceeded)
				{
					++_numberOfLengthExceeded;
				}

				AddOrderLong(order);
				AddOrderShort(order);

				_totalLengthOfAllOrders += order.SellBar - order.BuyBar;
				_totalGain += order.Gain;

				if (_totalGain >= _highestGain)
				{
					_highestGain = _totalGain;
				}
				else
				{
					double currentDrawdown = _highestGain - _totalGain;
					if (currentDrawdown > MaxDrawdown)
					{
						MaxDrawdown = currentDrawdown;
						MaxDrawdownDate = order.BuyDate;
					}
				}
			}
		}

		/// <summary>
		/// Calculate and save all the statistics.
		/// </summary>
		public void CalculateStatistics()
		{
			Gain = _totalGain;
			
			WinPercent = 0;
			LossPercent = 0;
			ProfitTargetPercent = 0;
			StopLossPercent = 0;
			LengthExceededPercent = 0;

			ProfitFactor = 0;
			ProfitFactorLargest = 0;

			LongWinPercent = 0;
			LongLossPercent = 0;
			LongProfitTargetPercent = 0;
			LongStopLossPercent = 0;
			LongLengthExceededPercent = 0;

			ShortWinPercent = 0;
			ShortLossPercent = 0;
			ShortProfitTargetPercent = 0;
			ShortStopLossPercent = 0;
			ShortLengthExceededPercent = 0;

			AverageOrderLength = 0;
			AverageProfitOrderLength = 0;
			AverageStopOrderLength = 0;

			if (NumberOfOrders > 0)
			{
				WinPercent = Math.Round(((double)_numberOfWins / NumberOfOrders) * 100.0);
				LossPercent = Math.Round(((double)_numberOfLosses / NumberOfOrders) * 100.0);
				ProfitTargetPercent = Math.Round(((double)_numberOfProfitTargets / NumberOfOrders) * 100.0);
				StopLossPercent = Math.Round(((double)_numberOfStopLosses / NumberOfOrders) * 100.0);
				LengthExceededPercent = Math.Round(((double)_numberOfLengthExceeded / NumberOfOrders) * 100.0);
				ProfitFactor = _totalMoneyLosses > 0 ? _totalMoneyWins / _totalMoneyLosses : _totalMoneyWins;
				ProfitFactorLargest = LargestLoser < 0 ? LargestWinner / Math.Abs(LargestLoser) : LargestWinner;

				if (LongNumberOfOrders > 0)
				{
					LongWinPercent = Math.Round(((double)_longNumberOfWins / LongNumberOfOrders) * 100.0);
					LongLossPercent = Math.Round(((double)_longNumberOfLosses / LongNumberOfOrders) * 100.0);
					LongProfitTargetPercent = Math.Round(((double)_longNumberOfProfitTargets / LongNumberOfOrders) * 100.0);
					LongStopLossPercent = Math.Round(((double)_longNumberOfStopLosses / LongNumberOfOrders) * 100.0);
					LongLengthExceededPercent = Math.Round(((double)_longNumberOfLengthExceeded / LongNumberOfOrders) * 100.0);
					LongWinAvg = _longNumberOfWins > 0 ? LongWinAvg / _longNumberOfWins : 0;
					LongWinAvgPercent = _longNumberOfWins > 0 ? LongWinAvgPercent / _longNumberOfWins : 0;
					LongLossAvg = _longNumberOfLosses > 0 ? LongLossAvg / _longNumberOfLosses : 0;
					LongLossAvgPercent = _longNumberOfLosses > 0 ? LongLossAvgPercent / _longNumberOfLosses : 0;
				}

				if (ShortNumberOfOrders > 0)
				{
					ShortWinPercent = Math.Round(((double)_shortNumberOfWins / ShortNumberOfOrders) * 100.0);
					ShortLossPercent = Math.Round(((double)_shortNumberOfLosses / ShortNumberOfOrders) * 100.0);
					ShortProfitTargetPercent = Math.Round(((double)_shortNumberOfProfitTargets / ShortNumberOfOrders) * 100.0);
					ShortStopLossPercent = Math.Round(((double)_shortNumberOfStopLosses / ShortNumberOfOrders) * 100.0);
					ShortLengthExceededPercent = Math.Round(((double)_shortNumberOfLengthExceeded / ShortNumberOfOrders) * 100.0);
					ShortWinAvg = _shortNumberOfWins > 0 ? ShortWinAvg / _shortNumberOfWins : 0;
					ShortWinAvgPercent = (_shortNumberOfWins > 0 ? ShortWinAvgPercent / _shortNumberOfWins : 0) * -1;
					ShortLossAvg = _shortNumberOfLosses > 0 ? ShortLossAvg / _shortNumberOfLosses : 0;
					ShortLossAvgPercent = (_shortNumberOfLosses > 0 ? ShortLossAvgPercent / _shortNumberOfLosses : 0) * -1;
				}

				AverageOrderLength = Math.Round((double)_totalLengthOfAllOrders / NumberOfOrders);
				AverageProfitOrderLength = _numberOfProfitTargets > 0 ? (double)_totalLengthOfProfitOrders / _numberOfProfitTargets : 0;
				AverageStopOrderLength = _numberOfStopLosses > 0 ? (double)_totalLengthOfStopOrders / _numberOfStopLosses : 0;
			}
		}

		/// <summary>
		/// Inits the values from already calculated statistics.
		/// </summary>
		/// <param name="stats">Other stattistics object.</param>
		public void InitFromStrategyTickerPairStatistics(StrategyTickerPairStatistics stats)
		{
			WinPercent = stats.WinPercent;
			LossPercent = stats.LossPercent;
			ProfitTargetPercent = stats.ProfitTargetPercent;
			StopLossPercent = stats.StopLossPercent;
			LengthExceededPercent = stats.LengthExceededPercent;
			Gain = stats.Gain;
			NumberOfOrders = stats.NumberOfOrders;

			LargestWinner = stats.LargestWinner;
			LargestWinnerBuyDate = stats.LargestWinnerBuyDate;
			LargestLoser = stats.LargestLoser;
			LargestLoserBuyDate = stats.LargestLoserBuyDate;

			MostConsecutiveLosers = stats.MostConsecutiveLosers;
			MostConsecutiveLosersDate = stats.MostConsecutiveLosersDate;
			MaxDrawdown = stats.MaxDrawdown;
			MaxDrawdownDate = stats.MaxDrawdownDate;
			ProfitFactor = stats.ProfitFactor;
			ProfitFactorLargest = stats.ProfitFactorLargest;

			LongWinPercent = stats.LongWinPercent;
			LongLossPercent = stats.LongLossPercent;
			LongProfitTargetPercent = stats.LongProfitTargetPercent;
			LongStopLossPercent = stats.LongStopLossPercent;
			LongLengthExceededPercent = stats.LongLengthExceededPercent;
			LongNumberOfOrders = stats.LongNumberOfOrders;
			LongWinAvg = stats.LongWinAvg;
			LongWinAvgPercent = stats.LongWinAvgPercent;
			LongLossAvg = stats.LongLossAvg;
			LongLossAvgPercent = stats.LongLossAvgPercent;

			ShortWinPercent = stats.ShortWinPercent;
			ShortLossPercent = stats.ShortLossPercent;
			ShortProfitTargetPercent = stats.ShortProfitTargetPercent;
			ShortStopLossPercent = stats.ShortStopLossPercent;
			ShortLengthExceededPercent = stats.ShortLengthExceededPercent;
			ShortNumberOfOrders = stats.ShortNumberOfOrders;
			ShortWinAvg = stats.ShortWinAvg;
			ShortWinAvgPercent = stats.ShortWinAvgPercent;
			ShortLossAvg = stats.ShortLossAvg;
			ShortLossAvgPercent = stats.ShortLossAvgPercent;

			AverageOrderLength = stats.AverageOrderLength;
			AverageProfitOrderLength = stats.AverageProfitOrderLength;
			AverageStopOrderLength = stats.AverageStopOrderLength;
		}

		/// <summary>
		/// Increments values for long orders.
		/// </summary>
		/// <param name="order">Order being added from AddOrder</param>
		private void AddOrderLong(Order order)
		{
			if (order.Type == Order.OrderType.Long)
			{
				++LongNumberOfOrders;

				if (order.Gain > 0)
				{
					++_longNumberOfWins;
					LongWinAvg += order.Gain;
					LongWinAvgPercent += UtilityMethods.PercentChange(order.BuyPrice, order.SellPrice);
				}
				else
				{
					++_longNumberOfLosses;
					LongLossAvg += order.Gain;
					LongLossAvgPercent += UtilityMethods.PercentChange(order.BuyPrice, order.SellPrice);
				}

				if (order.SellReason == Order.SellReasonType.ProfitTarget)
				{
					++_longNumberOfProfitTargets;
				}
				else if (order.SellReason == Order.SellReasonType.StopLoss)
				{
					++_longNumberOfStopLosses;
				}
				else if (order.SellReason == Order.SellReasonType.LengthExceeded)
				{
					++_longNumberOfLengthExceeded;
				}
			}
		}

		/// <summary>
		/// Increments values for short orders.
		/// </summary>
		/// <param name="order">Order being added from AddOrder</param>
		private void AddOrderShort(Order order)
		{
			if (order.Type == Order.OrderType.Short)
			{
				++ShortNumberOfOrders;

				if (order.Gain > 0)
				{
					++_shortNumberOfWins;
					ShortWinAvg += order.Gain;
					ShortWinAvgPercent += UtilityMethods.PercentChange(order.BuyPrice, order.SellPrice);
				}
				else
				{
					++_shortNumberOfLosses;
					ShortLossAvg += order.Gain;
					ShortLossAvgPercent += UtilityMethods.PercentChange(order.BuyPrice, order.SellPrice);
				}

				if (order.SellReason == Order.SellReasonType.ProfitTarget)
				{
					++_shortNumberOfProfitTargets;
				}
				else if (order.SellReason == Order.SellReasonType.StopLoss)
				{
					++_shortNumberOfStopLosses;
				}
				else if (order.SellReason == Order.SellReasonType.LengthExceeded)
				{
					++_shortNumberOfLengthExceeded;
				}
			}
		}
	
	}
}
