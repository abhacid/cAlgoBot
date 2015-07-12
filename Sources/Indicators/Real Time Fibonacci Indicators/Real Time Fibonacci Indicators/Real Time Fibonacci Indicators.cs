using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class RealTimeFibonacciIndicators : Indicator
    {
        [Parameter(DefaultValue = 14, MinValue = 2)]
        public int Period { get; set; }

        [Output("AverageTrueRange", Color = Colors.Orange)]
        public IndicatorDataSeries atr { get; set; }

        private ExponentialMovingAverage ema;
        public IndicatorDataSeries tr;
        private TrueRange tri;
        protected override void Initialize()
        {
            tr = CreateDataSeries();
            tri = Indicators.TrueRange();
            ema = Indicators.ExponentialMovingAverage(tr, Period);
        }

        public override void Calculate(int index)
        {
            if (index < Period + 1)
            {
                atr[index] = tri.Result[index];
            }
            if (index >= Period)
            {
                atr[index] = (atr[index - 1] * (Period - 1) + tri.Result[index]) / Period;
            }
        }
    }
}
