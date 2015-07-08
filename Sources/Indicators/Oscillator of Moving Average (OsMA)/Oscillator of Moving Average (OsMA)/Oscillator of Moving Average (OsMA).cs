using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Levels(0.0)]
    [Indicator(IsOverlay = false, ScalePrecision = 5, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class OSMA : Indicator
    {
        [Parameter(DefaultValue = 12)]
        public int shortCycle { get; set; }

        [Parameter(DefaultValue = 26)]
        public int longCycle { get; set; }

        [Parameter(DefaultValue = 9)]
        public int signalPeriod { get; set; }

        [Output("Main", PlotType = PlotType.Histogram)]
        public IndicatorDataSeries Result { get; set; }

        MacdHistogram macd;

        protected override void Initialize()
        {
            macd = Indicators.MacdHistogram(shortCycle, longCycle, signalPeriod);
        }

        public override void Calculate(int index)
        {
            Result[index] = macd.Signal[index] - macd.Histogram[index];
        }
    }
}