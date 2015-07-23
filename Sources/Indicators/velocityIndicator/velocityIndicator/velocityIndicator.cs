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

// -------------------------------------------------------------------------------------------------
//
//    Leonardo Hermoso, modifié par https://www.facebook.com/ab.hacid
//    
//    leonardo.hermoso arroba hotmail.com
//    If you are going to modify this file please make a copy using the "Duplicate" command.
//
// -------------------------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Lib;


namespace cAlgo.Indicators
{
    [Indicator(TimeZone = TimeZones.UTC, IsOverlay = false, AccessRights = AccessRights.None)]
    [Levels(-5, -3, -2, -1, -0.75, -0.5, -0.25, 0, 0.25, 0.5,
    0.75, 1, 2, 3, 5)]
    public class VelocityIndicator : Indicator
    {
        #region cIndicators Parammeters

        [Parameter("Number Of Candle", DefaultValue = 5, MinValue = 1)]
        public int NumberOfCandle { get; set; }

        [Parameter("Moving Average", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MovingAverageType { get; set; }

        [Parameter("MA Period", DefaultValue = 14, MinValue = 2)]
        public int Period { get; set; }

        [Output("High Acceleration", Color = Colors.Green, Thickness = 1)]
        public IndicatorDataSeries HighAccelerationSeries { get; set; }

        [Output("Low Acceleration", Color = Colors.Red, Thickness = 1)]
        public IndicatorDataSeries LowAccelerationSeries { get; set; }

        [Output("Moving Average", Color = Colors.Blue, Thickness = 2)]
        public IndicatorDataSeries MovingAverageSeries { get; set; }

        #endregion

        MovingAverage _movingAverage;
        double _elapsedTime;

        protected override void Initialize()
        {
            _movingAverage = Indicators.MovingAverage(HighAccelerationSeries, Period, MovingAverageType);

            long elapsedTicks = (long)Math.Ceiling((NumberOfCandle + 0.5) * MarketSeries.TimeFrame.ToTimeSpan().Ticks);
            TimeSpan elapsedSpan = new TimeSpan(elapsedTicks);
            _elapsedTime = elapsedSpan.TotalSeconds;
        }

        /// <summary>
        /// S = S0 + V0t +(at^2)/2   a = (2*(S-S0))/t^2.
        /// v = v0 +at
        /// </summary>
        /// <param name="index"></param>
        public override void Calculate(int index)
        {
            double high = MarketSeries.High[index];
            double previewHigh = MarketSeries.High[index - NumberOfCandle];
            double actualPriceOrHigh = (MarketSeries.Bars() - 1) == index ? Symbol.Mid() : high;

            double low = MarketSeries.Low[index];
            double previewLow = MarketSeries.Low[index - NumberOfCandle];
            double actualPriceOrLow = (MarketSeries.Bars() - 1) == index ? Symbol.Mid() : low;

            double highAcceleration = (2 * (high - previewHigh) / Math.Pow(_elapsedTime, 2)) * Math.Pow(10, 2 * Symbol.Digits);
            double lowAcceleration = (2 * (low - previewLow) / Math.Pow(_elapsedTime, 2)) * Math.Pow(10, 2 * Symbol.Digits);

            HighAccelerationSeries[index] = highAcceleration;
            LowAccelerationSeries[index] = lowAcceleration;
            MovingAverageSeries[index] = _movingAverage.Result[index];


            // Print("{0} {1} {2} {3} {4}",highAcceleration, lowAcceleration, elapsedTime, MarketSeries.Bars()-1, index);

        }
    }
}



