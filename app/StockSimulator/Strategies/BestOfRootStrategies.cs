﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using StockSimulator.Core;

namespace StockSimulator.Strategies
{
	/// <summary>
	/// Strategy that takes stores the best sub strategies for each day along with each ones statistcis
	/// to the day it was found.
	/// </summary>
	public class BestOfRootStrategies : RootSubStrategy
	{
		/// <summary>
		/// Construct the class and initialize the bar data to default values.
		/// </summary>
		/// <param name="tickerData">Ticker for the strategy</param>
		/// <param name="factory">Factory for creating dependents</param>
		public BestOfRootStrategies(TickerData tickerData, RunnableFactory factory) 
			: base(tickerData, factory)
		{
		}

		/// <summary>
		/// Returns an array of dependent names.
		/// </summary>
		public override string[] DependentNames
		{
			get
			{
				string[] deps = {
					"ElliotWavesStrategy"
					//"FibonacciRsi3m3",
					//"FibonacciDtOscillator",
					//"BressertComboStrategy",
					//"ComboStrategy",
					//"BressertApproach"
				};

				return deps;
			}
		}

		/// <summary>
		/// Returns the name of this strategy.
		/// </summary>
		/// <returns>The name of this strategy</returns>
		public override string ToString()
		{
			return "BestOfRootStrategies";
		}

		/// <summary>
		/// Sees which strategies were found on this far and places orders for all 
		/// the combos of those strategies. The value of this strategy is the best 
		/// strategy that was found on this bar based on the success of the history
		/// of that strategy.
		/// </summary>
		/// <param name="currentBar">Current bar of the simulation</param>
		protected override void OnBarUpdate(int currentBar)
		{
			base.OnBarUpdate(currentBar);

			// For each dependent strategy, see if they are found on this bar.
			// If they are then we'll see which is the best and treat that
			// as the only one found on todays bar for the main strategy.
			for (int i = 0; i < Dependents.Count; i++)
			{
				if (Dependents[i] is RootSubStrategy)
				{
					RootSubStrategy dependentStrategy = (RootSubStrategy)Dependents[i];

					// If there is a percent then there was something found. Save the
					// highest one as the one found today.
					if (dependentStrategy.Bars[currentBar].HighestPercent > Bars[currentBar].HighestPercent)
					{
						Bars[currentBar] = dependentStrategy.Bars[currentBar];
					}
				}
			}
		}
	}
}
