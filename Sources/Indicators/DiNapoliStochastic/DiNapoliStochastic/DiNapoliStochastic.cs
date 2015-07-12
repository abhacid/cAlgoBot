using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Levels(30, 70)]
    [Indicator(AccessRights = AccessRights.None)]
    public class DiNapoliStochastic : Indicator
    {
        [Parameter(DefaultValue = 8)]
        public int FastK { get; set; }

        [Parameter(DefaultValue = 3)]
        public int SlowK { get; set; }

        [Parameter(DefaultValue = 3)]
        public int SlowD { get; set; }

        [Output("Main", Color = Colors.Blue)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Signal", Color = Colors.Red)]
        public IndicatorDataSeries Signal { get; set; }

        public override void Calculate(int index)
        {
            if (index < FastK)
            {
                Result[index] = 0;
                Signal[index] = 0;
                return;
            }

            double min = MarketSeries.Low.Minimum(FastK);
            double max = MarketSeries.High.Maximum(FastK);
            double fast = 0.0;

            if (Math.Abs(max - min) > double.Epsilon)
                fast = (MarketSeries.Close[index] - min) / (max - min) * 100;

            Result[index] = Result[index - 1] + (fast - Result[index - 1]) / SlowK;
            Signal[index] = Signal[index - 1] + (Result[index] - Signal[index - 1]) / SlowD;

        }
    }
}
