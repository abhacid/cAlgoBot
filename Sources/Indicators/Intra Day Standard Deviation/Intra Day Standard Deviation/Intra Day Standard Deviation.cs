using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    //TimeZone = TimeZones.UTC
    public class IntraDayStandardDeviation : Indicator
    {
        [Parameter(DefaultValue = 2)]
        public int TimeZonesUTC { get; set; }

        [Output("Upper Bands", Color = Colors.Magenta)]
        public IndicatorDataSeries SD3Pos { get; set; }

        [Output("Lower Bands", Color = Colors.Green)]
        public IndicatorDataSeries SD3Neg { get; set; }

        [Output("VWAP", Color = Colors.Red)]
        public IndicatorDataSeries VWAP { get; set; }

        [Output("Midle", Color = Colors.Blue)]
        public IndicatorDataSeries Midle { get; set; }

        private int end_bar = 0;
        private int start_bar = 0;
        private int oldCurrentDay = 0;

        public override void Calculate(int index)
        {
            int end_bar = index;
            int CurrentDay = MarketSeries.OpenTime[end_bar].DayOfYear;
            double TotalPV = 0;
            double TotalVolume = 0;
            double highest = 0;
            double lowest = 999999;

            if (CurrentDay == oldCurrentDay)
            {
                for (int i = start_bar; i <= end_bar; i++)
                {
                    TotalPV += MarketSeries.TickVolume[i] * ((MarketSeries.Low[i] + MarketSeries.High[i] + MarketSeries.Close[i]) / 3);
                    TotalVolume += MarketSeries.TickVolume[i];
                    VWAP[i] = TotalPV / TotalVolume;

                    if (MarketSeries.High[i] > highest)
                    {
                        highest = MarketSeries.High[i];
                    }
                    if (MarketSeries.Low[i] < lowest)
                    {
                        lowest = MarketSeries.Low[i];
                    }

                    Midle[i] = (highest + lowest) / 2;

                    double SD = 0;

                    for (int k = start_bar; k <= i; k++)
                    {
                        double avg = (MarketSeries.High[k] + MarketSeries.Low[k] + MarketSeries.Close[k]) / 3;
                        double diff = avg - VWAP[i];
                        SD += (MarketSeries.TickVolume[k] / TotalVolume) * (diff * diff);
                    }

                    SD = Math.Sqrt(SD);

                    double SD_Pos = VWAP[i] + SD;
                    double SD_Neg = VWAP[i] - SD;
                    double SD2Pos = SD_Pos + SD;
                    double SD2Neg = SD_Neg - SD;

                    SD3Pos[i] = SD2Pos + SD;
                    SD3Neg[i] = SD2Neg - SD;
                }
            }
            else
            {
                oldCurrentDay = MarketSeries.OpenTime[end_bar].DayOfYear;
                start_bar = end_bar - TimeZonesUTC;
            }

            return;
        }
    }
}
