// -------------------------------------------------------------------------------------------------
//
//    This code is a cAlgo API example.
//    
//    All changes to this file will be lost on next application start.
//    If you are going to modify this file please make a copy using the "Duplicate" command.
//
// -------------------------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SampleDeMarker : Indicator
    {
        [Parameter(DefaultValue = 14)]
        public int Periods { get; set; }

        [Output("DMark", Color = Colors.Turquoise)]
        public IndicatorDataSeries DMark { get; set; }

        private IndicatorDataSeries deMin;
        private IndicatorDataSeries deMax;
        private MovingAverage deMinMA;
        private MovingAverage deMaxMA;

        protected override void Initialize()
        {
            deMin = CreateDataSeries();
            deMax = CreateDataSeries();
            deMinMA = Indicators.MovingAverage(deMin, Periods, MovingAverageType.Simple);
            deMaxMA = Indicators.MovingAverage(deMax, Periods, MovingAverageType.Simple);
        }

        public override void Calculate(int index)
        {
            deMin[index] = Math.Max(MarketSeries.Low[index - 1] - MarketSeries.Low[index], 0);
            deMax[index] = Math.Max(MarketSeries.High[index] - MarketSeries.High[index - 1], 0);

            var min = deMinMA.Result[index];
            var max = deMaxMA.Result[index];

            DMark[index] = max / (min + max);
        }
    }
}
