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

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/cAlgoBot
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
using cAlgo.Lib;

namespace cAlgo
{

    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
	[Levels (-0.67, 0, 0.67)]
    public class CandlestickTendencyII : Indicator
    {
        #region Indicator Parameters
        [Parameter()]
        public TimeFrame Global { get; set; }

		[Parameter("Minimum Global Candle Ceil", DefaultValue = 0, MinValue = 0)]
		public int MinimumGlobalCandleCeil { get; set; }

        [Output("GlobalTrendSignal", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries GlobalTrendSignal { get; set; }

        [Output("LocalTrendSignal", PlotType =PlotType.Line, Color = Colors.Green)]
        public IndicatorDataSeries LocalTrendSignal { get; set; }

        [Output("LocalMA", PlotType =PlotType.Line, Color = Colors.Blue, Thickness=2)]
        public IndicatorDataSeries LocalMA { get; set; }

        #endregion

        #region Indicator Variables

        MarketSeries marketSerieGlobal;
        double localTrendValue = 0;
        double globalTrendValue = 0;
		MovingAverage _localMA;
        #endregion

        protected override void Initialize()
        {
            marketSerieGlobal = MarketData.GetSeries(Global);
			
			int period = 2 * (int) (Global.ToTimeSpan().Ticks / MarketSeries.TimeFrame.ToTimeSpan().Ticks);

			_localMA = Indicators.MovingAverage(LocalTrendSignal, period, MovingAverageType.Exponential);

        }

		private int GetIndexByDate(MarketSeries series, DateTime time)
		{
			for(int i = series.Open.Count - 1; i > 0; i--)
			{
				if(time >= series.OpenTime[i])
					return i;
			}
			return -1;
		}

		/// <summary>
		///  GetIndexByExactTime don't allway work by example if attached timeframe is 1T (One Tick)!		
		///  
		///	 The Close price looks only at the final price of a candle, while the Median 
		///	 calculates in the wicks too.One can say that the wicks are only noise, so the CLOSE must be the better 
		///	 one, but not always, sometimes the wicks can hint of the future direction, so it may be a mistake to 
		///	 use the Close.It is a big dilemma nontheless...
		/// </summary>
		/// <param name="index"></param>
        public override void Calculate(int index)
        {
			int globalIndex = GetIndexByDate(marketSerieGlobal, MarketSeries.OpenTime[index]);

            bool isGlobalTrendRising = marketSerieGlobal.Close[globalIndex] > marketSerieGlobal.Open[globalIndex];
			bool isLocalTrendRising = MarketSeries.Close[index] > MarketSeries.Open[index - 1];

			bool isGlobalTrendFalling = marketSerieGlobal.Close[globalIndex] < marketSerieGlobal.Open[globalIndex];
			bool isLocalTrendFalling = MarketSeries.Close[index] < MarketSeries.Open[index - 1];

            bool IsLongSignal = isGlobalTrendRising && isLocalTrendRising;
            bool IsShortSignal = isGlobalTrendFalling && isLocalTrendFalling;

			bool isGlobalCandleAboveCeil = Math.Abs(marketSerieGlobal.Close[globalIndex] - marketSerieGlobal.Open[globalIndex]) > MinimumGlobalCandleCeil*Symbol.PipSize;


			if(isLocalTrendFalling)
				localTrendValue = -1;
			else
			{
				if(isLocalTrendRising)
					localTrendValue = 1;
				else
					localTrendValue = 0;
			}

			if (isGlobalCandleAboveCeil)
			{
				if(isGlobalTrendFalling)
					globalTrendValue = -2;
				else
				{
					if(isGlobalTrendRising)
						globalTrendValue = 2;
					else
						globalTrendValue = 0;
				}
			}
			else
				globalTrendValue = 0;

			LocalTrendSignal[index] = localTrendValue;
			GlobalTrendSignal[index] = globalTrendValue;
			LocalMA[index] = 5 * _localMA.Result[index];
        }
    }
}
