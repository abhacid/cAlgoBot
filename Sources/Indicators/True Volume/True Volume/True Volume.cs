using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TrueVolume : Indicator
    {

        [Output("Greater", PlotType = PlotType.Histogram)]
        public IndicatorDataSeries GreaterVolume { get; set; }

        [Output("Lesser", PlotType = PlotType.Histogram)]
        public IndicatorDataSeries LesserVolume { get; set; }

        protected override void Initialize()
        {
            // Initialize and create nested indicators
        }

        public override void Calculate(int index)
        {
            double pVolume = MarketSeries.TickVolume[index - 1] - ((MarketSeries.High[index - 1] - MarketSeries.Low[index - 1]) / Symbol.TickSize);
            double cVolume = MarketSeries.TickVolume[index] - ((MarketSeries.High[index] - MarketSeries.Low[index]) / Symbol.TickSize);


            if (cVolume > pVolume)
            {
                GreaterVolume[index] = cVolume;
                LesserVolume[index] = 0;
            }
            if (cVolume < pVolume)
            {
                LesserVolume[index] = cVolume;
                GreaterVolume[index] = 0;
            }
            if (cVolume == pVolume)
            {
                if (LesserVolume[index - 1] == 0)
                    GreaterVolume[index] = cVolume;
                if (GreaterVolume[index - 1] == 0)
                    LesserVolume[index] = cVolume;
            }



        }
    }
}
