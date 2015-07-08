using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator()]
    public class KSToscillator : Indicator
    {
        private PriceROC _priceRoc1;
        private PriceROC _priceRoc2;
        private PriceROC _priceRoc3;
        private PriceROC _priceRoc4;

        private MovingAverage _movingAverage1;
        private MovingAverage _movingAverage2;
        private MovingAverage _movingAverage3;
        private MovingAverage _movingAverage4;

        private ExponentialMovingAverage ema;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 10)]
        public int X1 { get; set; }
        [Parameter(DefaultValue = 15)]
        public int X2 { get; set; }
        [Parameter(DefaultValue = 20)]
        public int X3 { get; set; }
        [Parameter(DefaultValue = 30)]
        public int X4 { get; set; }

        [Parameter(DefaultValue = 1)]
        public int W1 { get; set; }
        [Parameter(DefaultValue = 2)]
        public int W2 { get; set; }
        [Parameter(DefaultValue = 3)]
        public int W3 { get; set; }
        [Parameter(DefaultValue = 4)]
        public int W4 { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }

        [Parameter(DefaultValue = 10)]
        public int AVG1 { get; set; }
        [Parameter(DefaultValue = 10)]
        public int AVG2 { get; set; }
        [Parameter(DefaultValue = 10)]
        public int AVG3 { get; set; }
        [Parameter(DefaultValue = 15)]
        public int AVG4 { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        [Output("Ema", Color = Colors.Blue)]
        public IndicatorDataSeries Ema { get; set; }

        protected override void Initialize()
        {
            _priceRoc1 = Indicators.PriceROC(Source, X1);
            _priceRoc2 = Indicators.PriceROC(Source, X2);
            _priceRoc3 = Indicators.PriceROC(Source, X3);
            _priceRoc4 = Indicators.PriceROC(Source, X4);

            _movingAverage1 = Indicators.MovingAverage(_priceRoc1.Result, AVG1, MAType);
            _movingAverage2 = Indicators.MovingAverage(_priceRoc2.Result, AVG2, MAType);
            _movingAverage3 = Indicators.MovingAverage(_priceRoc3.Result, AVG3, MAType);
            _movingAverage4 = Indicators.MovingAverage(_priceRoc4.Result, AVG4, MAType);

            ema = Indicators.ExponentialMovingAverage(Result, 9);

        }


        public override void Calculate(int index)
        {
            Result[index] = W1 * _movingAverage1.Result[index] + W2 * _movingAverage2.Result[index] + W3 * _movingAverage3.Result[index] + W4 * _movingAverage4.Result[index];

            Ema[index] = ema.Result[index];


        }
    }
}
