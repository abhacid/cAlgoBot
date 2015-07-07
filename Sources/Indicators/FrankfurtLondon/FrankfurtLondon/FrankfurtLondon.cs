using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class FrankfurtLondon : Indicator
    {
        // fix for tick charts
        int seen_frankfurt = 0;
        int seen_london = 0;
        
        protected override void Initialize()
        {
        }

        public override void Calculate(int index)
        {
            if (MarketSeries.OpenTime[index].Minute == 0) {
                if (MarketSeries.OpenTime[index].Hour == 6 && seen_frankfurt != MarketSeries.OpenTime[index].Day) {
                    seen_frankfurt = MarketSeries.OpenTime[index].Day;
                    ChartObjects.DrawVerticalLine("Frankfurt Open" + index, index, Colors.OrangeRed, 1);
                }
                if (MarketSeries.OpenTime[index].Hour == 7 && seen_london != MarketSeries.OpenTime[index].Day) {
                    seen_london = MarketSeries.OpenTime[index].Day;
                    ChartObjects.DrawVerticalLine("London Open" + index, index, Colors.Red, 1);
                }
            }
        }
    }
}
