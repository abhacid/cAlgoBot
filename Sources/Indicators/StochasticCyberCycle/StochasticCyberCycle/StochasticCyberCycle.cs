using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class StochasticCyberCycle : Indicator
    {
        [Parameter(DefaultValue = 0.07)]
        public double alpha { get; set; }

        [Parameter(DefaultValue = 8)]
        public int length { get; set; }

        [Output("StochCyberCycle", Color = Colors.Red)]
        public IndicatorDataSeries stochcc { get; set; }

        [Output("Trigger", Color = Colors.Blue)]
        public IndicatorDataSeries trigger { get; set; }

        private IndicatorDataSeries price;
        private IndicatorDataSeries smooth;
        private IndicatorDataSeries value1;
        private IndicatorDataSeries cycle;

        protected override void Initialize()
        {
            price = CreateDataSeries();
            smooth = CreateDataSeries();
            cycle = CreateDataSeries();
            value1 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            price[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;

            smooth[index] = (price[index] + 2 * price[index - 1] + 2 * price[index - 2] + price[index - 3]) / 6;

            cycle[index] = (1 - 0.5 * alpha) * (1 - 0.5 * alpha) * (smooth[index] - 2 * smooth[index - 1] + smooth[index - 2]) + 2 * (1 - alpha) * cycle[index - 1] - (1 - alpha) * (1 - alpha) * (cycle[index - 2]);
            if (index < 7)
            {
                cycle[index] = (price[index] - 2 * price[index - 1] + price[index - 2]) / 4;
            }
            double ccgH = cycle[index];
            double ccgL = cycle[index];
            for (int i = index - length + 1; i <= index; i++)
            {
                if (ccgH < cycle[i])
                {
                    ccgH = cycle[i];
                }
                if (ccgL > cycle[i])
                {
                    ccgL = cycle[i];
                }
            }
            if (ccgH != ccgL)
            {
                value1[index] = ((cycle[index] - ccgL) / (ccgH - ccgL));
            }
            stochcc[index] = (4 * value1[index] + 3 * value1[index - 1] + 2 * value1[index - 2] + value1[index - 3]) / 10;
            stochcc[index] = 2 * (stochcc[index] - 0.5);

            trigger[index] = 0.96 * (stochcc[index - 1] + 0.02);
        }
    }
}
