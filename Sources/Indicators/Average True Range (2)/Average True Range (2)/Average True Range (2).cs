// -------------------------------------------------------------------------------
//
//    Average True Range
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class AverageTrueRange : Indicator
    {
        [Parameter("Period", DefaultValue = 14)]
        public int Period { get; set; }


        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        private ExponentialMovingAverage _ema;
        private IndicatorDataSeries _tempBuffer;

        protected override void Initialize()
        {
            _tempBuffer = CreateDataSeries();
            _ema = Indicators.ExponentialMovingAverage(_tempBuffer, Period);
        }

        public override void Calculate(int index)
        {
            double high = MarketSeries.High[index];
            double low = MarketSeries.Low[index];

            if (index == 0)
            {
                _tempBuffer[index] = high - low;
            }
            else
            {
                double prevClose = MarketSeries.Close[index - 1];
                _tempBuffer[index] = Math.Max(high, prevClose) - Math.Min(low, prevClose);
            }

            Result[index] = _ema.Result[index];
        }
    }
}
