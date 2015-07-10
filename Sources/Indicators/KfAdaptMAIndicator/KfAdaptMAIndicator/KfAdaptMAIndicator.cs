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
using cAlgo.API.Indicators;
using cAlgo.Lib;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    [Levels(50, 0, -50)]
    public class KfAdaptMAIndicator : Indicator
    {
        [Parameter("Period", DefaultValue = 9)]
        public int Period { get; set; }

        [Parameter("FastPeriod", DefaultValue = 2)]
        public int FastPeriod { get; set; }

        [Parameter("SlowPeriod", DefaultValue = 30)]
        public int SlowPeriod { get; set; }

        [Output("Kaufman Adaptative Moving Average", Color = Colors.SteelBlue)]
        public IndicatorDataSeries Result { get; set; }


        protected override void Initialize()
        {
            // Initialize and create nested indicators
        }

        public override void Calculate(int index)
        {
            if (index != 0)
            {
                int period = Math.Min(index, Period);
                double lastClose = MarketSeries.Close.LastValue;
                DataSeries Close = MarketSeries.Close;

                double Fastest = 2.0 / (FastPeriod + 1);
                double Slowest = 2.0 / (SlowPeriod + 1);
                double denominator = Close.fold((acc, previewClose, close) => acc + Math.Abs(close - previewClose), (double)0, +1, index - period, index);
                ;
                double numerator = Math.Abs(lastClose - MarketSeries.Close[index - period]);
                ;
                double alpha;

                if (denominator != 0)
                    alpha = Math.Pow(((numerator / denominator) * (Fastest - Slowest) + Slowest), 2);
                else
                    alpha = 0;

                Result[index] = (alpha * lastClose) + ((1 - alpha) * Result.Last(1));
            }
            else
                Result[index] = 1;
        }
    }
}
