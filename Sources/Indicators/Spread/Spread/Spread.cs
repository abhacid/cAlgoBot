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

// Efficiency Ratio Indicator (ERIndicator) measure noise signal, see the article: https://calgobots.codeplex.com/discussions/557184 
// 
// If ERIndicator is near to 1.0 the noise is low, it is near to 0 the noise is important. It is better to take a position 
// when ERIndicator is greater than 0.6.



using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Lib;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    [Levels(-1, -0.3, -0.2, -0.1, 0, 0.1, 0.2, 0.3, 0.4, 0.5,
    0.6, 0.7, 0.8, 0.9, 1, 1.1, 1.2, 1.3, 1.4, 1.5,
    2, 3, 4)]
    public class Spread : Indicator
    {
        [Parameter("Spread MA Source")]
        public DataSeries SpreadMASource { get; set; }

        [Parameter("Spread MA Period", DefaultValue = 11, MinValue = 1)]
        public int SpreadMAPeriod { get; set; }

        [Parameter("Spread MA Type", DefaultValue = 1)]
        public MovingAverageType SpreadMaType { get; set; }

        [Output("Spread", Color = Colors.SteelBlue)]
        public IndicatorDataSeries SpreadResult { get; set; }

        [Output("MA Spread", Color = Colors.Green)]
        public IndicatorDataSeries SpreadMAResult { get; set; }

        MovingAverage ma;
        protected override void Initialize()
        {
            ma = Indicators.MovingAverage(SpreadResult, SpreadMAPeriod, SpreadMaType);
        }

        public override void Calculate(int index)
        {
            double ask = Math.Round(Symbol.Ask, 5);
            double bid = Math.Round(Symbol.Bid, 5);
            double spread = Math.Round(Symbol.Spread / Symbol.PipSize, 5);

            SpreadResult[index] = spread;
            SpreadMAResult[index] = ma.Result[index];

            string text = string.Format("Spread : {0}, Ask : {1}, Bid : {2}", spread, ask, bid);
            ChartObjects.DrawText("spread", "\t" + text, StaticPosition.TopLeft, Colors.White);
        }



    }
}

