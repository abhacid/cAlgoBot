using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class VHF : Indicator
    {


        [Parameter("Period", DefaultValue = 14)]
        public int Period { get; set; }

        [Parameter("Threshold", DefaultValue = 0.35)]
        public double Threshold { get; set; }

        [Output("VH Filter", Color = Colors.Purple)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Threshold", Color = Colors.Red)]
        public IndicatorDataSeries ThresholdLine { get; set; }


        public override void Calculate(int index)
        {
            if (index < Period + 1)
                return;

            double max = MarketSeries.Close.Maximum(Period);
            double min = MarketSeries.Close.Minimum(Period);

            double numerator = max - min;
            double denominator = 0;

            for (int i = 0; i < Period; i++)
            {
                denominator += Math.Abs(MarketSeries.Close[index - i] - MarketSeries.Close[index - i - 1]);
            }

            Result[index] = (numerator / denominator);
            ThresholdLine[index] = Threshold;


        }
    }
}
