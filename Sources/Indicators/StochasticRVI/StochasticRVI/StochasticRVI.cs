using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    class StochasticRVI : Indicator
    {
        [Parameter(DefaultValue = 7)]
        public int Length { get; set; }

        [Parameter(DefaultValue = 80)]
        public int Delimiter { get; set; }

        [Output("RVI", Color = Colors.Red)]
        public IndicatorDataSeries RVI { get; set; }

        [Output("Trigger", Color = Colors.Blue)]
        public IndicatorDataSeries Trigger { get; set; }

        [Output("Delimiter1", Color = Colors.Green)]
        public IndicatorDataSeries Delimiter1 { get; set; }

        [Output("Delimiter2", Color = Colors.Green)]
        public IndicatorDataSeries Delimiter2 { get; set; }

        private IndicatorDataSeries _buffer1;
        private IndicatorDataSeries _buffer2;
        private IndicatorDataSeries _buffer3;
        private IndicatorDataSeries _buffer4;

        protected override void Initialize()
        {
            _buffer1 = CreateDataSeries();
            _buffer2 = CreateDataSeries();
            _buffer3 = CreateDataSeries();
            _buffer4 = CreateDataSeries();
        }
        public override void Calculate(int index)
        {
            if (index < Length - 1)
            {
                _buffer1[index] = 0;
                _buffer2[index] = 0;
                _buffer3[index] = 0;
                _buffer4[index] = 0;
                RVI[index] = 0;
                return;
            }

            _buffer1[index] = ((MarketSeries.Close[index] - MarketSeries.Open[index]) + 2 * (MarketSeries.Close[index - 1] - MarketSeries.Open[index - 1]) + 2 * (MarketSeries.Close[index - 2] - MarketSeries.Open[index - 2]) + (MarketSeries.Close[index - 3] - MarketSeries.Open[index - 3])) / 6;

            _buffer2[index] = ((MarketSeries.High[index] - MarketSeries.Low[index]) + 2 * (MarketSeries.High[index - 1] - MarketSeries.Low[index - 1]) + 2 * (MarketSeries.High[index - 2] - MarketSeries.Low[index - 2]) + (MarketSeries.High[index - 3] - MarketSeries.Low[index - 3])) / 6;


            double numerator = 0;
            double denominator = 0;

            for (int i = 0; i < Length; i++)
            {
                numerator += _buffer1[index - i];
                denominator += _buffer2[index - i];
            }
            double epsilon = Math.Pow(10, -10);
            if (Math.Abs(denominator) > epsilon)
                _buffer3[index] = numerator / denominator;


            double maxRVI = Functions.Maximum(_buffer3, Length);
            double minRVI = Functions.Minimum(_buffer3, Length);

            if (Math.Abs(maxRVI - minRVI) > epsilon)
                _buffer4[index] = (_buffer3[index] - minRVI) / (maxRVI - minRVI);

            RVI[index] = (4 * _buffer4[index] + 3 * _buffer4[index - 1] + 2 * _buffer4[index - 2] + _buffer4[index - 3]) / 10;
            RVI[index] = 2 * (RVI[index] - 0.5);

            Trigger[index] = 0.96 * (RVI[index - 1] + 0.02);

            Delimiter1[index] = 0.01 * Delimiter;
            Delimiter2[index] = -0.01 * Delimiter;

        }
    }
}
