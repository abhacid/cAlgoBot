using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class EmpiricalModeDecomposition : Indicator
    {
        #region Variables
        
        private double _alpha;
        private double _beta;
        private double _gamma;
        private double _mean;

        private IndicatorDataSeries _bp;
        private IndicatorDataSeries _peak;
        private IndicatorDataSeries _valley;
        private SimpleMovingAverage _smaBp;
        private SimpleMovingAverage _smaPeak;
        private SimpleMovingAverage _smaValley;

        #endregion

        #region  Parameters
        
        [Parameter("Period", DefaultValue = 20)]
        public int Period { get; set; }
        
        [Parameter("Delta", DefaultValue = 0.5)]
        public double Delta { get; set; }
        
        [Parameter("Fraction", DefaultValue = 0.1)]
        public double Fraction { get; set; }

        #endregion

        #region Output

        [Output("Avg Peaks", Color = Colors.Green)]
        public IndicatorDataSeries UpperBand { get; set; }

        [Output("Avg Valleys", Color = Colors.Blue)]
        public IndicatorDataSeries LowerBand { get; set; }

        [Output("Trend", Color = Colors.Red)]
        public IndicatorDataSeries Result { get; set; }

        #endregion

        protected override void Initialize()
        {
            _bp = CreateDataSeries();
            _peak = CreateDataSeries();
            _valley = CreateDataSeries();

            _smaBp = Indicators.SimpleMovingAverage(_bp, 2*Period);
            _smaPeak = Indicators.SimpleMovingAverage(_peak, 50);
            _smaValley = Indicators.SimpleMovingAverage(_valley, 50);

            _beta = Math.Cos(2 * Math.PI / Period);
            _gamma = 1 / Math.Cos(4 * Math.PI * Delta / Period);
            _alpha = _gamma - Math.Sqrt(Math.Pow(_gamma, 2) - 1);

        }

        public override void Calculate(int index)
        {

            if (index < 51)
            {
                _bp[index] = 0;
                _peak[index] = 0;
                _valley[index] = 0;

                return;
            }


            _bp[index] = (0.5*(1 - _alpha)*
                          (((MarketSeries.High[index] + MarketSeries.Low[index])/2) -
                           ((MarketSeries.High[index - 2] + MarketSeries.Low[index - 2])/2))
                           + _beta * (1 + _alpha) * _bp[index - 1] - _alpha * _bp[index - 2]);

            _mean = _smaBp.Result[index];

            _peak[index] = _peak[index - 1];
            _valley[index] = _valley[index - 1];


            if (_bp[index - 1] > _bp[index] && _bp[index - 1] > _bp[index - 2])
                _peak[index] = _bp[index - 1]; 

            else if (_bp[index - 1] < _bp[index] && _bp[index - 1] < _bp[index - 2])
                _valley[index] = _bp[index - 1];

            UpperBand[index] = _smaPeak.Result[index] * Fraction;
            LowerBand[index] = _smaValley.Result[index] * Fraction;

            Result[index] = _mean;


        }


    }
}

