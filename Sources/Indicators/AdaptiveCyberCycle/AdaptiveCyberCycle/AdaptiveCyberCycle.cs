//#reference:..\Indicators\CyclePeriod.algo
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator()]
    [Levels(0.0)]
    public class AdaptiveCyberCycle : Indicator
    {
        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        [Output("Adaptive Cyber Cycle", Color = Colors.Blue)]
        public IndicatorDataSeries AdaptiveCycle { get; set; }

        [Output("Trigger", Color = Colors.Green)]
        public IndicatorDataSeries Trigger { get; set; }

        private IndicatorDataSeries _price;
        private IndicatorDataSeries _smooth;
        private IndicatorDataSeries _period;
        private CyclePeriod _cyclePeriod;

        protected override void Initialize()
        {
            _price = CreateDataSeries();
            _smooth = CreateDataSeries();
            _period = CreateDataSeries();
            _cyclePeriod = Indicators.GetIndicator<CyclePeriod>(Alpha);
        }


        public override void Calculate(int index)
        {
            _price[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;
            _smooth[index] = (_price[index] + 2 * _price[index - 1] + 2 * _price[index - 2] + _price[index - 3]) / 6;

            if (index < 7)
            {
                _period[index] = 0;

                AdaptiveCycle[index] = (_price[index] - 2 * _price[index - 1] + _price[index - 2]) / 4;
                Trigger[index] = AdaptiveCycle[index - 1];

                return;
            }

            _period[index] = _cyclePeriod.Result[index];

            double alpha = 2 / (_period[index] + 1);

            AdaptiveCycle[index] = (1 - 0.5 * alpha) * (1 - 0.5 * alpha) * (_smooth[index] - 2 * _smooth[index - 1] + _smooth[index - 2]) + 2 * (1 - alpha) * AdaptiveCycle[index - 1] - (1 - alpha) * (1 - alpha) * AdaptiveCycle[index - 2];

            Trigger[index] = AdaptiveCycle[index - 1];


        }
    }
}
