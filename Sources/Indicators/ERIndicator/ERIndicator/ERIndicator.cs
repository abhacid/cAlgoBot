
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

// Efficiency Ratio Indicator (ERIndicator) measure noise signal, see the article: https://github.com/abhacid/cAlgoBot/wiki/Mesurer-le-bruit-d'un-signal-boursier 
// 
// If ERIndicator is near to 1.0 the noise is low, it is near to 0 the noise is important. It is better to take a position 
// when ERIndicator is greater than 0.6.



using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Lib;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    [Levels(0, 0.3, 0.6, 1)]
    public class ERIndicator : Indicator
    {
        [Parameter("Period", DefaultValue = 11)]
        public int Period { get; set; }

        [Output("Efficiency Ratio", Color = Colors.SteelBlue)]
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

                double numerator = Math.Abs(MarketSeries.Close.LastValue - MarketSeries.Close[index - period]);
                ;

                double denominator = MarketSeries.Close.fold((acc, previewClose, close) => acc + Math.Abs(close - previewClose), (double)0, +1, index - period, index);

                if (denominator != 0)
                    Result[index] = numerator / denominator;
                else
                    Result[index] = 1.0;
            }
            else
                Result[index] = 1.0;

        }
    }
}

