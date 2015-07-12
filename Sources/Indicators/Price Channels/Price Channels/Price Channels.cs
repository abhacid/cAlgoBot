using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class PriceChannels : Indicator
    {
        [Output("HiChannel")]
        public IndicatorDataSeries HiChannel { get; set; }

        [Output("LowChannel")]
        public IndicatorDataSeries LowChannel { get; set; }

        [Output("Center Line", LineStyle = LineStyle.Dots)]
        public IndicatorDataSeries CenterLine { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        public override void Calculate(int index)
        {
            if (index < Period)
                return;

            double upper = double.MinValue;
            double lower = double.MaxValue;

            for (int i = index - Period; i <= index - 1; i++)
            {
                upper = Math.Max(MarketSeries.High[i], upper);
                lower = Math.Min(MarketSeries.Low[i], lower);
            }

            HiChannel[index] = upper;
            LowChannel[index] = lower;
            CenterLine[index] = (upper + lower) / 2;


        }
    }
}
