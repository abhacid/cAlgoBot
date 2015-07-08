using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class Stpmt : Indicator
    {

        private StochasticOscillator _stochastic5;
        private StochasticOscillator _stochastic14;
        private StochasticOscillator _stochastic45;
        private StochasticOscillator _stochastic75;

        private SimpleMovingAverage _stpmtMa;
        private IndicatorDataSeries _stpmt;

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Output("STPMT", Color = Colors.Red)]
        public IndicatorDataSeries Result { get; set; }
        
        [Output("STPMT_MA", Color = Colors.Blue)]
        public IndicatorDataSeries StpmtMa { get; set; }
        
        [Output("DataSeries1", Color = Colors.LightGray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries DataSeries1 { get; set; }

        [Output("DataSeries2", Color = Colors.LightGray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries DataSeries2 { get; set; }

        [Output("DataSeries3", Color = Colors.LightGray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries DataSeries3 { get; set; }

        [Output("DataSeries4", Color = Colors.LightGray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries DataSeries4 { get; set; }


        protected override void Initialize()
        {
            _stochastic5 = Indicators.StochasticOscillator(5, 3, 3, MaType);
            _stochastic14 = Indicators.StochasticOscillator(14, 3, 3, MaType);
            _stochastic45 = Indicators.StochasticOscillator(45, 3, 14, MaType);
            _stochastic75 = Indicators.StochasticOscillator(75, 3, 20, MaType);
            
            _stpmt = CreateDataSeries();
            _stpmtMa = Indicators.SimpleMovingAverage(_stpmt, 9);

        }
        public override void Calculate(int index)
        {

            DataSeries1[index] = _stochastic5.PercentK[index];
            DataSeries2[index] = _stochastic14.PercentK[index];
            DataSeries3[index] = _stochastic45.PercentK[index];
            DataSeries4[index] = _stochastic75.PercentK[index];

            _stpmt[index] = (4.1*DataSeries1[index] + 2.5*DataSeries2[index] + DataSeries3[index] + 4*DataSeries4[index])/
                            11.6;
            Result[index] = _stpmt[index];
            StpmtMa[index] = _stpmtMa.Result[index];


        }
    }
}
