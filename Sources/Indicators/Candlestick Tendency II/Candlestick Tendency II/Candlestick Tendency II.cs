#region Licence
//The MIT License (MIT)
//Copyright (c) 2014 abdallah HACID, https://www.facebook.com/ab.hacid

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//and associated documentation files (the "Software"), to deal in the Software without restriction,
//including without limitation the rights to use, copy, modify, merge, publish, distribute,
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
//is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or
//substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
//BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/Robot_Forex
#endregion

#region Description
// Author		: gorkroitor 
// link			: http://ctdn.com/algos/indicators/show/648
// Modified		: by Abdallah HACID

//This is a simple indicator to show if global and local trend directions are the same. It uses two timeframes, 
//a "local" timeframe and a higher order "global" timeframe.The indicator shows price action only and has no lag. 
//It generates a square wave (green line), the output signal is above zero if the price tendency is bullish upward, 
//and below zero for bearish downward tendency. The red line shows the tendency of a higher order timeframe 
//(the global tendency). The idea is to trade long when price tends to grow on both timeframes 
//(indicator showing both lines cross zero and go above), and vice versa, trade short when the price 
//tends down (both lines cross zero and go below). Close or exit positions on the opposite signal. 
//The global timeframe setting of at least 4x local timeframe is recommended, for example, m15/h1, or h1/ h4, etc...

#endregion

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{

    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class CandlestickTendencyII : Indicator
	{
		#region Indicator Parameters
		[Parameter()]
        public TimeFrame Global { get; set; }

        [Output("GlobalTrendSignal", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries GlobalTrendSignal { get; set; }

        [Output("LocalTrendSignal", PlotType = PlotType.Line, Color = Colors.Green)]
        public IndicatorDataSeries LocalTrendSignal { get; set; }

        [Output("Signal", PlotType = PlotType.Line, Color = Colors.Blue)]
		public IndicatorDataSeries Signal { get; set; }
		#endregion

		#region Indicator Variables

		MarketSeries marketSerieGlobal;

		#endregion


		protected override void Initialize()
        {
            marketSerieGlobal = MarketData.GetSeries(Global);
        }

		#region Indicator predicates
		public bool isLocalTrendRising(int index)
        {
			return (MarketSeries.Close[index] > MarketSeries.Open[index - 1]); 
        }
        public bool isLocalTrendFalling(int index)
        {
			return (MarketSeries.Close[index] < MarketSeries.Open[index - 1]); 
        }
        public bool isGlobalTrendRising(int index)
        {
			return (marketSerieGlobal.Close[index] > marketSerieGlobal.Open[index - 1]); 
        }
        public bool isGlobalTrendFalling(int index)
        {
			return (marketSerieGlobal.Close[index] < marketSerieGlobal.Open[index - 1]); 
        }

		public bool IsLongSignal(int index)
		{
			int globalIndex = marketSerieGlobal.OpenTime.GetIndexByTime(MarketSeries.OpenTime[index]);

			return isGlobalTrendRising(globalIndex) && isLocalTrendRising(index); 
		}

		public bool IsShortSignal(int index)
		{
			int globalIndex = marketSerieGlobal.OpenTime.GetIndexByTime(MarketSeries.OpenTime[index]);

			return isGlobalTrendFalling(globalIndex) && isLocalTrendFalling(index); 
		}


		#endregion

		double signal = 0;
		double localTrendValue = 0;
		double globalTrendValue = 0;

		public override void Calculate(int index)
        {
			int globalIndex = marketSerieGlobal.OpenTime.GetIndexByTime(MarketSeries.OpenTime[index]); // GetIndexByExactTime don't allway work by example if attached timeframe is 1T (One Tick)!


			if(isLocalTrendFalling(index))
				localTrendValue = -1;
			else if(isLocalTrendRising(index))
				localTrendValue = 1;
			else
				localTrendValue = 0;

			if(isGlobalTrendFalling(globalIndex))
				globalTrendValue = -2;
			else if(isGlobalTrendRising(globalIndex))
				globalTrendValue = 2;
			else
				globalTrendValue = 0;

			if(IsLongSignal(index))
				signal = 3;
			else if(IsShortSignal(index))
				signal = -3;
			else
				signal = 0;

            LocalTrendSignal[index] = localTrendValue;
            GlobalTrendSignal[index] = globalTrendValue;
			Signal[index] = signal;
        }
    }
}
