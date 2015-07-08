using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator("Fisher")]
    public class Fisher : Indicator
    {
        private IndicatorDataSeries _value1;
        private IndicatorDataSeries _buffer0;
        private IndicatorDataSeries _fisher1;

        [Parameter("Period", DefaultValue = 10)]
        public int Period { get; set; }

        [Output("Buffer1", PlotType = PlotType.Histogram, Color = Colors.Green)]
        public IndicatorDataSeries Buffer1 { get; set; }

        [Output("Buffer2", PlotType = PlotType.Histogram, Color = Colors.Red)]
        public IndicatorDataSeries Buffer2 { get; set; }


        protected override void Initialize()
        {
            _fisher1 = CreateDataSeries();
            _value1 = CreateDataSeries();
            _buffer0 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            if (index < Period)
            {
                _value1[index] = 0;
                _fisher1[index] = 0;

                return;
            }

            double maxH = MarketSeries.High.Maximum(Period);
            double minL = MarketSeries.Low.Minimum(Period);

            double price = (MarketSeries.High[index] + MarketSeries.Low[index])/2;

            double value = 0.33*2*((price - minL)/(maxH - minL) - 0.5) + 0.67*_value1[index - 1];
            value = Math.Min(Math.Max(value, -0.999), 0.999);

            _buffer0[index] = 0.5*Math.Log((1 + value)/(1 - value)) + 0.5*_fisher1[index - 1];

            _value1[index] = value;
            _fisher1[index] = _buffer0[index];

            bool up = _buffer0[index] > 0;

            if (!up)
            {
                Buffer2[index] = _buffer0[index];
                Buffer1[index] = 0.0;
            }
            else
            {
                Buffer1[index] = _buffer0[index];
                Buffer2[index] = 0.0;
            }
        }
    }
}