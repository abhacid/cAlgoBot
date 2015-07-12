
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class StochasticRSI2 : Indicator
    {
        [Output("StochRSI", Color = Colors.Red)]
        public IndicatorDataSeries StochRSI { get; set; }

        [Output("Trigger", Color = Colors.Blue)]
        public IndicatorDataSeries trigger { get; set; }

        [Parameter(DefaultValue = 14, MinValue = 2)]
        public int Period { get; set; }

        private RelativeStrengthIndex rsi;
        private IndicatorDataSeries value1;

        protected override void Initialize()
        {
            rsi = Indicators.RelativeStrengthIndex(MarketSeries.Close, Period);
            value1 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            double rsiL = rsi.Result[index];
            double rsiH = rsi.Result[index];
            for (int i = index - Period + 1; i <= index; i++)
            {
                if (rsiH < rsi.Result[i])
                {
                    rsiH = rsi.Result[i];
                }
                if (rsiL > rsi.Result[i])
                {
                    rsiL = rsi.Result[i];
                }
            }
            if (rsiH != rsiL)
            {
                value1[index] = ((rsi.Result[index] - rsiL) / (rsiH - rsiL));
            }
            StochRSI[index] = (4 * value1[index] + 3 * value1[index - 1] + 2 * value1[index - 2] + value1[index - 3]) / 10;
            StochRSI[index] = 2 * (StochRSI[index] - 0.5);

            trigger[index] = 0.96 * (StochRSI[index - 1] + 0.02);
        }
    }
}
