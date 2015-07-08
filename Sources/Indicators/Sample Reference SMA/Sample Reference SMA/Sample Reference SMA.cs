using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AutoRescale = false, AccessRights = AccessRights.None)]
    public class SampleReferenceSMA : Indicator
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 14)]
        public int SmaPeriod { get; set; }

        [Output("Referenced SMA Output")]
        public IndicatorDataSeries refSMA { get; set; }

        private SampleSMA sma;

        protected override void Initialize()
        {
            sma = Indicators.GetIndicator<SampleSMA>(Source, SmaPeriod);
        }

        public override void Calculate(int index)
        {
            refSMA[index] = sma.Result[index];
        }
    }
}
