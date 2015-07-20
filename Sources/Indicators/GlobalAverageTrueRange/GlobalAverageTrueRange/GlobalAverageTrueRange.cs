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


using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.Lib;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator("Average Daily Range", ScalePrecision = 5, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class GlobalAverageTrueRange : Indicator
    {
        [Parameter("Global Timeframe")]
        public TimeFrame GlobalTimeFrame { get; set; }

        [Parameter("ATR Period", DefaultValue = 14)]
        public int AtrPeriod { get; set; }

        [Parameter("ATR Moving Average Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType AtrMovingAverageType { get; set; }

        [Output("Global ATR", PlotType = PlotType.Line, Thickness = 2, Color = Colors.Blue)]
        public IndicatorDataSeries GlobalAtr { get; set; }

        [Output("Global TR", PlotType = PlotType.Line, Thickness = 1, Color = Colors.Azure)]
        public IndicatorDataSeries GlobalTr { get; set; }

        private MarketSeries _globalSeries;
        private TrueRange _globalTr;
        private AverageTrueRange _globalAtr;

        protected override void Initialize()
        {
            _globalSeries = MarketData.GetSeries(Symbol, TimeFrame.Daily);
            _globalTr = Indicators.TrueRange(_globalSeries);
            _globalAtr = Indicators.AverageTrueRange(_globalSeries, AtrPeriod, AtrMovingAverageType);
        }


        public override void Calculate(int index)
        {

            int globalIndex = _globalSeries.GetIndexByDate(MarketSeries.OpenTime[index]);

            GlobalAtr[index] = _globalAtr.Result[globalIndex];
            GlobalTr[index] = _globalTr.Result[globalIndex];
        }



    }
}
