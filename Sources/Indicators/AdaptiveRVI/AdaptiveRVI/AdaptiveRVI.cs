//#reference:..\Indicators\CyclePeriod.algo
using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator()]
    [Levels(0.0)]
    public class AdaptiveRVI : Indicator
    {
        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        [Output("Adaptive RVI", Color = Colors.Blue)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Trigger", Color = Colors.Green)]
        public IndicatorDataSeries Trigger { get; set; }

        private IndicatorDataSeries _buffer1;
        private IndicatorDataSeries _buffer2;
        private IndicatorDataSeries _period;
        private CyclePeriod _cyclePeriod;

        protected override void Initialize()
        {
            _buffer1 = CreateDataSeries();
            _buffer2 = CreateDataSeries();
            _period = CreateDataSeries();
            _cyclePeriod = Indicators.GetIndicator<CyclePeriod>(Alpha);

        }

        public override void Calculate(int index)
        {
            _buffer1[index] = ((MarketSeries.Close[index] - MarketSeries.Open[index]) + 2 * (MarketSeries.Close[index - 1] - MarketSeries.Open[index - 1]) + 2 * (MarketSeries.Close[index - 2] - MarketSeries.Open[index - 2]) + (MarketSeries.Close[index - 3] - MarketSeries.Open[index - 3])) / 6;

            _buffer2[index] = ((MarketSeries.High[index] - MarketSeries.Low[index]) + 2 * (MarketSeries.High[index - 1] - MarketSeries.Low[index - 1]) + 2 * (MarketSeries.High[index - 2] - MarketSeries.Low[index - 2]) + (MarketSeries.High[index - 3] - MarketSeries.Low[index - 3])) / 6;

            _period[index] = _cyclePeriod.Result[index];
            var period = (int)Math.Floor((4 * _period[index] + 3.0 * _period[index - 1] + 2.0 * _period[index - 3] + _period[index - 4]) / 20.0);

            double numerator = 0;
            double denominator = 0;

            for (int i = 0; i < period; i++)
            {
                numerator += _buffer1[index - i];
                denominator += _buffer2[index - i];
            }

            if (Math.Abs(denominator) > double.Epsilon)
                Result[index] = numerator / denominator;
            else
                Result[index] = 0.0;

            Trigger[index] = Result[index - 1];


        }
    }
}
