// -------------------------------------------------------------------------------
//	  T3MA Indicator
//    weighted sum of: single EMA, double EMA, triple EMA etc
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class T3MA : Indicator
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        // Volume Factor
        [Parameter(DefaultValue = 0.7)]
        public double Volume_Factor { get; set; }


        [Output("T3MA", Color = Colors.Red)]
        public IndicatorDataSeries Result { get; set; }

        private ExponentialMovingAverage ema1;
        private ExponentialMovingAverage ema2;
        private ExponentialMovingAverage ema3;
        private ExponentialMovingAverage ema4;
        private ExponentialMovingAverage ema5;
        private ExponentialMovingAverage ema6;


        protected override void Initialize()
        {
            ema1 = Indicators.ExponentialMovingAverage(Source, Period);
            ema2 = Indicators.ExponentialMovingAverage(ema1.Result, Period);
            ema3 = Indicators.ExponentialMovingAverage(ema2.Result, Period);
            ema4 = Indicators.ExponentialMovingAverage(ema3.Result, Period);
            ema5 = Indicators.ExponentialMovingAverage(ema4.Result, Period);
            ema6 = Indicators.ExponentialMovingAverage(ema5.Result, Period);

        }

        public override void Calculate(int index)
        {

            double b, b2, b3, c1, c2, c3, c4;

            b = Volume_Factor;
            b2 = Math.Pow(b, 2);
            // Volume Factor Squared
            b3 = Math.Pow(b, 3);
            // Volume Factor Cubed
            c1 = -b3;
            c2 = 3 * b2 + 3 * b3;
            c3 = -6 * b2 - 3 * b - 3 * b3;
            c4 = 1 + 3 * b + b3 + 3 * b2;

            Result[index] = c1 * ema6.Result[index] + c2 * ema5.Result[index] + c3 * ema4.Result[index] + c4 * ema3.Result[index];
        }
    }
}
