using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class QualitativeQuantitativeE : Indicator
    {
        private int _wildersPeriod;
        private int _startBar;
        private const int SF = 5;
        private ExponentialMovingAverage _ema;
        private ExponentialMovingAverage _emaAtr;
        private ExponentialMovingAverage _emaRsi;
        private RelativeStrengthIndex _rsi;


        private IndicatorDataSeries _atrRsi;

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Output("Main", Color = Colors.Green)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Signal", Color = Colors.Red, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries ResultS { get; set; }

        [Output("Upper", Color = Colors.Gray, LineStyle = LineStyle.DotsRare)]
        public IndicatorDataSeries Upper { get; set; }

        [Output("Lower", Color = Colors.Gray, LineStyle = LineStyle.DotsRare)]
        public IndicatorDataSeries Lower { get; set; }

        [Output("Middle", Color = Colors.Gray, LineStyle = LineStyle.DotsRare)]
        public IndicatorDataSeries Middle { get; set; }

        protected override void Initialize()
        {

            _atrRsi = CreateDataSeries();
            CreateDataSeries();

            _wildersPeriod = Period * 2 - 1;
            _startBar = _wildersPeriod < SF ? SF : _wildersPeriod;

            _rsi = Indicators.RelativeStrengthIndex(MarketSeries.Close, Period);
            _emaRsi = Indicators.ExponentialMovingAverage(_rsi.Result, SF);
            _emaAtr = Indicators.ExponentialMovingAverage(_atrRsi, _wildersPeriod);
            _ema = Indicators.ExponentialMovingAverage(_emaAtr.Result, _wildersPeriod);

        }

        public override void Calculate(int index)
        {
            Result[index] = _emaRsi.Result[index];

            if (index <= _startBar)
            {
                ResultS[index] = 0;
                return;
            }

            _atrRsi[index] = Math.Abs(Result[index - 1] - Result[index]);

            double tr = ResultS[index - 1];

            if (Result[index] < ResultS[index - 1])
            {
                tr = Result[index] + _ema.Result[index] * 4.236;

                if (Result[index - 1] < ResultS[index - 1] && tr > ResultS[index - 1])
                    tr = ResultS[index - 1];
            }
            else if (Result[index] > ResultS[index - 1])
            {
                tr = Result[index] - _ema.Result[index] * 4.236;

                if (Result[index - 1] > ResultS[index - 1] && tr < ResultS[index - 1])
                    tr = ResultS[index - 1];
            }

            ResultS[index] = tr;

            Upper[index] = 70;
            Lower[index] = 30;
            Middle[index] = 50;
        }
    }
}
