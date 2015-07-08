using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    [Levels(0.0)]
    internal class FisherCyberCycle : Indicator
    {
        private IndicatorDataSeries _price;
        private IndicatorDataSeries _smooth;
        private IndicatorDataSeries _value1;
        private double _value2;
        private IndicatorDataSeries _cycle;

        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        [Parameter(DefaultValue = 8)]
        public int Length { get; set; }

        [Output("CyberCycle", Color = Colors.Red)]
        public IndicatorDataSeries Cycle { get; set; }

        [Output("Trigger", Color = Colors.Blue)]
        public IndicatorDataSeries Trigger { get; set; }


        protected override void Initialize()
        {
            _price = CreateDataSeries();
            _smooth = CreateDataSeries();
            _value1 = CreateDataSeries();
            _cycle = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            _price[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;
            _smooth[index] = (_price[index] + 2 * _price[index - 1] + 2 * _price[index - 2] + _price[index - 3]) / 6;

            if (index < 7)
            {
                _cycle[index] = (_price[index] - 2 * _price[index - 1] + _price[index - 2]) / 4;
                return;
            }

            _cycle[index] = (1 - 0.5 * Alpha) * (1 - 0.5 * Alpha) * (_smooth[index] - 2 * _smooth[index - 1] + _smooth[index - 2])
                           + 2 * (1 - Alpha) * _cycle[index - 1] - (1 - Alpha) * (1 - Alpha) * (_cycle[index - 2]);

            double maxCycle = Functions.Maximum(_cycle, Length);
            double minCycle = Functions.Minimum(_cycle, Length);

            double epsilon = Math.Pow(10,-10);
            if (Math.Abs(maxCycle - minCycle) > epsilon)
                _value1[index] = (_cycle[index] - minCycle) / (maxCycle - minCycle);
            else
                _value1[index] = 0;
            _value2 = (4 * _value1[index] + 3 * _value1[index - 1] + 2 * _value1[index - 2] + _value1[index - 3]) / 10;
            Cycle[index] = .5 * Math.Log((1 + 1.98 * (_value2 - .5)) / (1 - 1.98 * (_value2 - .5)));

            Trigger[index] = Cycle[index - 1];
        }
    }
}