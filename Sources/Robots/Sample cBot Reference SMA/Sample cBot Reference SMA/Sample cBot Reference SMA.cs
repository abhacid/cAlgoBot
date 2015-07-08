using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SamplecBotReferenceSMA : Robot
    {
        [Parameter("Source")]
        public DataSeries Source { get; set; }

        [Parameter("SMA Period", DefaultValue = 14)]
        public int SmaPeriod { get; set; }

        private SampleSMA sma;

        protected override void OnStart()
        {
            sma = Indicators.GetIndicator<SampleSMA>(Source, SmaPeriod);
        }

        protected override void OnTick()
        {
            Print("{0}", sma.Result.LastValue);
        }
    }
}
