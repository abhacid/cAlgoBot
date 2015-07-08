// -------------------------------------------------------------------------------------------------
//
//    ADR - Average Daily Range
//
// -------------------------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class myADR : Indicator
    {
        [Parameter("ADR Period", DefaultValue = 5, MinValue = 1, MaxValue = 999)]
        public int adr_period { get; set; }

        [Parameter("Color Label", DefaultValue = "Lime")]
        public string color_label_str { get; set; }

        [Parameter("Color Text", DefaultValue = "White")]
        public string color_value_str { get; set; }

        [Parameter("Timeframe Daily", DefaultValue = true)]
        public bool timeframe_daily { get; set; }

        private MarketSeries mseries;
        private Colors color_label;
        private Colors color_value;

        protected override void Initialize()
        {
            if (!Enum.TryParse(color_label_str, out color_label))
                color_label = Colors.Lime;

            if (!Enum.TryParse(color_value_str, out color_value))
                color_value = Colors.White;

            if (timeframe_daily)
                mseries = MarketData.GetSeries(TimeFrame.Daily);
            else
                mseries = MarketSeries;
        }

        public override void Calculate(int index)
        {
            if (!IsLastBar)
                return;

            int index_last = 0;
            double range_today = 0;
            double range_adr = 0;
            double range_adr_150 = 0;
            double range_adr_200 = 0;
            string tf_str = "";

            index_last = mseries.Close.Count - 1;

            range_today = (mseries.High[index_last] - mseries.Low[index_last]) * Math.Pow(10, Symbol.Digits - 1);
            range_today = Math.Round(range_today, 0);

            for (int i = index_last; i > index_last - adr_period; i--)
                range_adr += (mseries.High[i] - mseries.Low[i]) * Math.Pow(10, Symbol.Digits - 1);

            range_adr /= adr_period;
            range_adr = Math.Round(range_adr, 0);

            range_adr_150 = range_adr * 1.5;
            range_adr_150 = Math.Round(range_adr_150, 0);

            range_adr_200 = range_adr * 2.0;
            range_adr_200 = Math.Round(range_adr_200, 0);

            tf_str = mseries.TimeFrame.ToString();

            ChartObjects.DrawText("RLabels", "R" + tf_str + "\n" + "RAvg" + adr_period + "\n" + "R150" + "\n" + "R200", StaticPosition.TopLeft, color_label);
            ChartObjects.DrawText("RValues", "\t" + range_today + "\n\t" + range_adr + "\n\t" + range_adr_150 + "\n\t" + range_adr_200, StaticPosition.TopLeft, color_value);
        }
    }
}
