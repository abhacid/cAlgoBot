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
using cAlgo.API.Indicators;

using cAlgo.Lib;

namespace cAlgo.Indicators
{
    /// <summary>
    /// https://github.com/abhacid/cAlgoBotdiscussions/554324
    /// </summary>
    [Indicator(ScalePrecision = 0, IsOverlay = false, AccessRights = AccessRights.None)]
    [Levels(30, 70)]
    public class RSIWellesIndicator : Indicator
    {
        [Parameter("Period", DefaultValue = 14, MinValue = 2)]
        public int Period { get; set; }

        [Output("RSI-Welles", Color = Colors.Yellow)]
        public IndicatorDataSeries wellesRSI { get; set; }

        public IndicatorDataSeries AvgRise { get; set; }

        public IndicatorDataSeries AvgFall { get; set; }

        [Output("Overbought", Color = Colors.Turquoise)]
        public IndicatorDataSeries overbought { get; set; }

        [Output("Oversold", Color = Colors.Red)]
        public IndicatorDataSeries oversold { get; set; }

        protected override void Initialize()
        {
            base.Initialize();
            AvgRise = CreateDataSeries();
            AvgFall = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            if (IsLastBar)
                return;

            double increasing = 0;
            double fall = 0;
            double rise = 0;

            if (index < Period)
                wellesRSI[index] = 0;
            else if (index == Period)
            {
                for (int i = 0; i < Period; i++)
                {
                    increasing = MarketSeries.Close[i + 1] - MarketSeries.Close[i];
                    if (increasing > 0)
                        rise += increasing;
                    else
                        fall -= increasing;
                }

                AvgRise[index] = rise / Period;
                AvgFall[index] = fall / Period;
            }
            else
            {
                // Rest of averages are smoothed
                increasing = MarketSeries.Close[index] - MarketSeries.Close[index - 1];
                if (increasing > 0)
                    rise = increasing;
                else
                    fall = -increasing;

                AvgRise[index] = (AvgRise[index - 1] * (Period - 1) + rise) / Period;
                AvgFall[index] = (AvgFall[index - 1] * (Period - 1) + fall) / Period;
            }

            wellesRSI[index] = AvgFall[index] == 0 ? 100 : 100 * (1 - 1 / (1 + AvgRise[index] / AvgFall[index]));
        }
    }
}
