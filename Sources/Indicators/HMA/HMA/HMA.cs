using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class HMA : Indicator
    {
        [Output("HMA", Color = Colors.Orange)]
        public IndicatorDataSeries hma { get; set; }

        [Parameter(DefaultValue = 21)]
        public int Period { get; set; }

        private IndicatorDataSeries diff;
        private WeightedMovingAverage wma1;
        private WeightedMovingAverage wma2;
        private WeightedMovingAverage wma3;

        protected override void Initialize()
        {
            diff = CreateDataSeries();
            wma1 = Indicators.WeightedMovingAverage(MarketSeries.Close, (int)Period / 2);
            wma2 = Indicators.WeightedMovingAverage(MarketSeries.Close, Period);
            wma3 = Indicators.WeightedMovingAverage(diff, (int)Math.Sqrt(Period));
        }

        public override void Calculate(int index)
        {
            double var1 = 2 * wma1.Result[index];
            double var2 = wma2.Result[index];

            diff[index] = var1 - var2;

            hma[index] = wma3.Result[index];
        }
    }
}
