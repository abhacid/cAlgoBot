
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


// PipsATR is nothing other than the ATR indicator, but its values ​​are expressed in pips rather than value. 
// This gives for each timeframe chosen volatility in pips, which in my opinion is more explicit.
// On the other hand you can choose the timeframe for which to calculate the volatility; if for example you are 
// working on h1, you can see the ATR on h4 or on daily.

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None, ScalePrecision = 0)]
    [Levels(0, 10, 15, 20, 25, 30, 35, 40, 45, 50,
    55, 60, 70, 80, 90, 100, 200, 300)]
    public class PipsATRIndicator : Indicator
    {
        #region Indicator parameters
        [Parameter("Atr TimeFrame")]
        public TimeFrame AtrTimeFrame { get; set; }

        [Parameter("ATR Period", DefaultValue = 20)]
        public int AtrPeriod { get; set; }

        [Parameter("ATR MAType")]
        public MovingAverageType AtrMaType { get; set; }

        [Output("Pips ATR", Color = Colors.SteelBlue)]
        public IndicatorDataSeries Result { get; set; }
        #endregion

        private AverageTrueRange atr;

        protected override void Initialize()
        {
            atr = Indicators.AverageTrueRange(MarketData.GetSeries(AtrTimeFrame), AtrPeriod, AtrMaType);
        }

        public override void Calculate(int index)
        {
            Result[index] = atr.Result[index] / Symbol.PipSize;
        }
    }
}
