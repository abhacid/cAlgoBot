// -------------------------------------------------------------------------------------------------
//
//    This code is a cAlgo API example.
//    
//    All changes to this file will be lost on next application start.
//    If you are going to modify this file please make a copy using the "Duplicate" command.
//
// -------------------------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AutoRescale = false, AccessRights = AccessRights.None)]
    public class SampleAlligator : Indicator
    {
        [Parameter(DefaultValue = 13)]
        public int JawsPeriods { get; set; }

        [Parameter(DefaultValue = 8)]
        public int JawsShift { get; set; }

        [Parameter(DefaultValue = 8)]
        public int TeethPeriods { get; set; }

        [Parameter(DefaultValue = 5)]
        public int TeethShift { get; set; }

        [Parameter(DefaultValue = 5)]
        public int LipsPeriods { get; set; }

        [Parameter(DefaultValue = 3)]
        public int LipsShift { get; set; }

        [Output("Jaws", Color = Colors.Blue)]
        public IndicatorDataSeries Jaws { get; set; }

        [Output("Teeth", Color = Colors.Red)]
        public IndicatorDataSeries Teeth { get; set; }

        [Output("Lips", Color = Colors.Lime)]
        public IndicatorDataSeries Lips { get; set; }

        private WellesWilderSmoothing jawsMa;
        private WellesWilderSmoothing teethMa;
        private WellesWilderSmoothing lipsMa;

        protected override void Initialize()
        {

            jawsMa = Indicators.WellesWilderSmoothing(MarketSeries.Median, JawsPeriods);
            teethMa = Indicators.WellesWilderSmoothing(MarketSeries.Median, TeethPeriods);
            lipsMa = Indicators.WellesWilderSmoothing(MarketSeries.Median, LipsPeriods);
        }

        public override void Calculate(int index)
        {
            Jaws[index + JawsShift] = jawsMa.Result[index];
            Teeth[index + TeethShift] = teethMa.Result[index];
            Lips[index + LipsShift] = lipsMa.Result[index];
        }
    }
}
