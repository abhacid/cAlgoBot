using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class RelativeVigorIndex : Indicator
    {
        private IndicatorDataSeries _value1;
        private IndicatorDataSeries _value2;

        [Parameter(DefaultValue = 10)]
        public int Period { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }

        [Output("RVI", Color = Colors.Green)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Signal", Color = Colors.Red)]
        public IndicatorDataSeries Signal { get; set; }

        protected override void Initialize()
        {
            _value1 = CreateDataSeries();
            _value2 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            _value1[index] = ((MarketSeries.Close[index] - MarketSeries.Open[index]) + 2 * (MarketSeries.Close[index - 1] - MarketSeries.Open[index - 1]) + 2 * (MarketSeries.Close[index - 2] - MarketSeries.Open[index - 2]) + (MarketSeries.Close[index - 3] - MarketSeries.Open[index - 3])) / 6;

            _value2[index] = ((MarketSeries.High[index] - MarketSeries.Low[index]) + 2 * (MarketSeries.High[index - 1] - MarketSeries.Low[index - 1]) + 2 * (MarketSeries.High[index - 2] - MarketSeries.Low[index - 2]) + (MarketSeries.High[index - 3] - MarketSeries.Low[index - 3])) / 6;

            double num = 0;
            double denum = 0;
            for (int i = 0; i < Period; i++)
            {
                num += _value1[index - i];
                denum += _value2[index - i];
            }

            Result[index] = num / denum;

            Signal[index] = (Result[index] + 2 * Result[index - 1] + 2 * Result[index - 2] + Result[index - 3]) / 6;
        }
    }
}
