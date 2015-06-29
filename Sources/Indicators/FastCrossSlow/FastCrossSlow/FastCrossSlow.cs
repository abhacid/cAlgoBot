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

// Project Hosting for Open Source Software on Codeplex : https://calgobots.codeplex.com/
#endregion

#region Indicator Infos
// This indicator print buy/sell signal on the price graph windows when the fast line cross the slow line
#endregion

#region Indicator Parameters Comments
// -------------------------------------------------------------------------------
//	
//			Symbol				=	All
//			TimeFrame			=	prefere h4
//
//
// -------------------------------------------------------------------------------

#endregion


using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Lib;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class FastCrossSlow : Indicator
    {
        [Parameter("MA Type", DefaultValue = 6)]
        public MovingAverageType MaType { get; set; }

        [Parameter("Slow Period", DefaultValue = 11, MinValue = 1)]
        public int SlowPeriod { get; set; }

        [Parameter("Fast Period", DefaultValue = 5, MinValue = 1)]
        public int fastPeriod { get; set; }

        [Output("slowMa", Thickness = 2, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries SlowMAResult { get; set; }

        [Output("fastMA", Thickness = 2, Color = Colors.Green)]
        public IndicatorDataSeries FastMAResult { get; set; }

        private string upArrow = "▲";
        private string downArrow = "▼";
        private double arrowOffset;

        MovingAverage slowMA;
        MovingAverage fastMA;
        protected override void Initialize()
        {
            fastMA = Indicators.MovingAverage(MarketSeries.Close, fastPeriod, MaType);
            slowMA = Indicators.MovingAverage(MarketSeries.Close, SlowPeriod, MaType);

            arrowOffset = Symbol.PipSize * 5;

        }
        public override void Calculate(int index)
        {
            FastMAResult[index] = fastMA.Result[index];
            SlowMAResult[index] = slowMA.Result[index];

            double high = MarketSeries.High[index-1];
            double low = MarketSeries.Low[index-1];

            if (isCrossAbove())
                ChartObjects.DrawText(string.Format("Buy {0}", index), upArrow, index - 1, low - arrowOffset, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Green);

            if (isCrossBelow())
                ChartObjects.DrawText(string.Format("Sell {0}", index), downArrow, index - 1, high + arrowOffset, VerticalAlignment.Top, HorizontalAlignment.Center, Colors.Red);
        }

        #region Predicate
        public bool isCrossAbove()
        {
            return FastMAResult.HasCrossedAbove(SlowMAResult, 0);
        }
        public bool isCrossBelow()
        {
            return FastMAResult.HasCrossedBelow(SlowMAResult, 0);
        }
        #endregion

    }
}
