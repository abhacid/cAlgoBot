using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class AwesomeOscillator : Indicator
    {
        [Parameter(DefaultValue = 5)]
        public int periodFast { get; set; }

        [Parameter(DefaultValue = 34)]
        public int periodSlow { get; set; }

        [Output("Awesome OscillatorGreen", IsHistogram = true, Color = Colors.Green)]
        public IndicatorDataSeries AwesomeGreen { get; set; }

        [Output("Awesome OscillatorRed", IsHistogram = true, Color = Colors.Red)]
        public IndicatorDataSeries AwesomeRed { get; set; }

        private SimpleMovingAverage smaSlow;
        private SimpleMovingAverage smaFast;
        private IndicatorDataSeries medianprice;

        protected override void Initialize()
        {
            medianprice = CreateDataSeries();
            smaSlow = Indicators.SimpleMovingAverage(medianprice, periodSlow);
            smaFast = Indicators.SimpleMovingAverage(medianprice, periodFast);
        }

        public override void Calculate(int index)
        {
            medianprice[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;
            double previousAO = smaFast.Result[index - 1] - smaSlow.Result[index - 1];
            double currentAO = smaFast.Result[index] - smaSlow.Result[index];

            if (previousAO >= currentAO)
            {
                AwesomeRed[index] = smaFast.Result[index] - smaSlow.Result[index];
            }
            else
            {
                AwesomeGreen[index] = smaFast.Result[index] - smaSlow.Result[index];
            }
        }
    }
}
