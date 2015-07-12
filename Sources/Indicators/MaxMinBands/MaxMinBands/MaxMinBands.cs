using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MaxMinBands : Indicator
    {
        [Parameter(DefaultValue = 5)]
        public int period { get; set; }

        [Output("Maxband", Color = Colors.Blue)]
        public IndicatorDataSeries maxband { get; set; }

        [Output("Minband", Color = Colors.Red)]
        public IndicatorDataSeries minband { get; set; }

        double lower;
        double higher;

        public override void Calculate(int index)
        {
            lower = MarketSeries.Low[index];
            higher = MarketSeries.High[index];

            for (int i = 0; i < period; i++)
            {
                if (MarketSeries.Low[index - i] < lower)
                {
                    lower = MarketSeries.Low[index - i];
                }
                if (MarketSeries.High[index - i] > higher)
                {
                    higher = MarketSeries.High[index - i];
                }
                minband[index] = lower;
                maxband[index] = higher;
            }
        }
    }
}
