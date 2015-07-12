
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class FisherRSI : Indicator
    {

        [Parameter()]
        public DataSeries DataSource { get; set; }

        [Output("FisherRSI", Color = Colors.Red)]
        public IndicatorDataSeries fisherRSI { get; set; }

        [Output("Trigger", Color = Colors.Blue)]
        public IndicatorDataSeries trigger { get; set; }

        [Parameter(DefaultValue = 14, MinValue = 2)]
        public int Period { get; set; }

        private RelativeStrengthIndex rsi;
        private IndicatorDataSeries value1;

        protected override void Initialize()
        {
            rsi = Indicators.RelativeStrengthIndex(DataSource, Period);
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
            fisherRSI[index] = (4 * value1[index] + 3 * value1[index - 1] + 2 * value1[index - 2] + value1[index - 3]) / 10;
            fisherRSI[index] = (0.5 * Math.Log((1 + 1.98 * (fisherRSI[index] - 0.5)) / (1 - 1.98 * (fisherRSI[index] - 0.5))));

            trigger[index] = fisherRSI[index - 1];
        }
    }
}
