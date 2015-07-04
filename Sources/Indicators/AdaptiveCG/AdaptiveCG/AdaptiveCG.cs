//# reference:..\Indicators\CyclePeriod.algo
using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator()]
    [Levels(0.0)]
    public class AdaptiveCG : Indicator
    {
        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        [Output("Adaptive CG", Color = Colors.Blue)]
        public IndicatorDataSeries Result { get; set; }

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

                Result[index] = (_price[index] - 2 * _price[index - 1] + _price[index - 2]) / 4;
                Trigger[index] = Result[index - 1];

                return;
            }

            _period[index] = _cyclePeriod.Result[index];
            var period = (int)Math.Floor(_period[index] / 2.0);

            double numerator = 0;
            double denominator = 0;

            for (int i = 0; i < period; i++)
            {
                numerator += (1 + i) * _price[index - i];
                denominator += _price[index - i];
            }


            if (Math.Abs(denominator) > double.Epsilon)
                Result[index] = -numerator / denominator + (period + 1.0) / 2.0;
            else
                Result[index] = 0.0;

            Trigger[index] = Result[index - 1];


        }
    }
}
