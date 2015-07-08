using cAlgo.API;
using cAlgo.API.Indicators;

namespace MoneyBiz.cAlgo.Indicators
{
    [Indicator("Zero Lag Moving Average (ZLMA)", ScalePrecision = 5, IsOverlay = true)]
    public class ZeroLagMovingAverage : Indicator
    {
        private MovingAverage _ma1;
        private MovingAverage _ma2;

        [Parameter("Data Source")]
        public DataSeries DataSource { get; set; }

        [Parameter("Moving Average Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MovingAverageType { get; set; }

        [Parameter("Periods", DefaultValue = 7, MinValue = 1)]
        public int Periods { get; set; }

        [Parameter("Correction Factor", DefaultValue = 0.7, MinValue = 0.0, MaxValue = 1.0)]
        public double CorrectionFactor { get; set; }

        [Output("Result", Color = Colors.Red, LineStyle = LineStyle.Solid)]
        public IndicatorDataSeries Result { get; protected set; }

        protected override void Initialize()
        {
            _ma1 = Indicators.MovingAverage(DataSource, Periods, MovingAverageType);
            _ma2 = Indicators.MovingAverage(_ma1.Result, Periods, MovingAverageType);

            base.Initialize();
        }

        public override void Calculate(int index)
        {
            _ma1.Calculate(index);
            _ma2.Calculate(index);

            var diff = _ma1.Result[index] - _ma2.Result[index];

            Result[index] = _ma1.Result[index] + CorrectionFactor * diff;
        }


    }
}
