using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, ScalePrecision = 5, TimeZone = TimeZones.EasternStandardTime, AccessRights = AccessRights.None)]
    public class PredictedIntradayVolatility : Indicator
    {

        [Parameter(DefaultValue = 10, MinValue = 1, MaxValue = 50)]
        public int RateOfChange { get; set; }

        [Output("Result", Color = Colors.Orange)]
        public IndicatorDataSeries Result { get; set; }


        private TrueRange tr;

        protected override void Initialize()
        {
            tr = Indicators.TrueRange();
        }

        public override void Calculate(int index)
        {
            double period = Math.Min((MarketSeries.OpenTime[index] - MarketSeries.OpenTime[index - 1]).TotalHours, (MarketSeries.OpenTime[index - 1] - MarketSeries.OpenTime[index - 2]).TotalHours);
            int indexesPerDay = (int)Math.Round(24 / period);

            if (double.IsNaN(Result[index]))
                Result[index] = tr.Result[index];

            Result[index + indexesPerDay] = Result[index] * (1 - RateOfChange / 100.0) + tr.Result[index] * (RateOfChange / 100.0);
        }
    }
}
