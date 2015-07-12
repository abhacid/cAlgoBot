using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    [Levels(0.0)]
    public class CyberCycle : Indicator
    {
        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        [Output("CyberCycle", Color = Colors.Red)]
        public IndicatorDataSeries Cycle { get; set; }

        [Output("Trigger", Color = Colors.Blue)]
        public IndicatorDataSeries Trigger { get; set; }

        private IndicatorDataSeries _price;
        private IndicatorDataSeries _smooth;
        protected override void Initialize()
        {
            _price = CreateDataSeries();
            _smooth = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            _price[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;

            _smooth[index] = (_price[index] + 2 * _price[index - 1] + 2 * _price[index - 2] + _price[index - 3]) / 6;

            Cycle[index] = (1 - 0.5 * Alpha) * (1 - 0.5 * Alpha) * (_smooth[index] - 2 * _smooth[index - 1] + _smooth[index - 2]) + 2 * (1 - Alpha) * Cycle[index - 1] - (1 - Alpha) * (1 - Alpha) * (Cycle[index - 2]);

            if (index < 7)
            {
                Cycle[index] = (_price[index] - 2 * _price[index - 1] + _price[index - 2]) / 4;
            }

            Trigger[index] = Cycle[index - 1];
        }
    }
}
