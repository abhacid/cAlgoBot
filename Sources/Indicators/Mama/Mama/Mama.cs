using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Mama : Indicator
    {
        #region Input Parameters

        [Parameter("FastLimit", DefaultValue = 0.5)]
        public double FastLimit { get; set; }

        [Parameter("SlowLimit", DefaultValue = 0.05)]
        public double SlowLimit { get; set; }

        #endregion

        #region Output

        [Output("MAMA", Color = Colors.Blue)]
        public IndicatorDataSeries MamaResult { get; set; }

        [Output("FAMA", Color = Colors.Red)]
        public IndicatorDataSeries FamaResult { get; set; }

        #endregion

        #region private fields

        private IndicatorDataSeries _alpha;
        private IndicatorDataSeries _deltaPhase;
        private IndicatorDataSeries _detrender;
        private IndicatorDataSeries _i1;
        private IndicatorDataSeries _i2;
        private IndicatorDataSeries _im;
        private IndicatorDataSeries _ji;
        private IndicatorDataSeries _jq;
        private IndicatorDataSeries _period;
        private IndicatorDataSeries _period1;
        private IndicatorDataSeries _period2;
        private IndicatorDataSeries _phase;
        private IndicatorDataSeries _price;
        private IndicatorDataSeries _q1;
        private IndicatorDataSeries _q2;
        private IndicatorDataSeries _re;
        private IndicatorDataSeries _smooth;
        private IndicatorDataSeries _smoothPeriod;

        #endregion

        protected override void Initialize()
        {
            _price = CreateDataSeries();
            _smooth = CreateDataSeries();
            _detrender = CreateDataSeries();
            _period = CreateDataSeries();
            _period1 = CreateDataSeries();
            _period2 = CreateDataSeries();
            _smoothPeriod = CreateDataSeries();
            _phase = CreateDataSeries();
            _deltaPhase = CreateDataSeries();
            _alpha = CreateDataSeries();
            _q1 = CreateDataSeries();
            _i1 = CreateDataSeries();
            _ji = CreateDataSeries();
            _jq = CreateDataSeries();
            _i2 = CreateDataSeries();
            _q2 = CreateDataSeries();
            _re = CreateDataSeries();
            _im = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            if (index <= 5)
            {
                MamaResult[index] = 0;
                FamaResult[index] = 0;

                _price[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;

                _smooth[index] = 0;
                _detrender[index] = 0;
                _period[index] = 0;
                _smoothPeriod[index] = 0;
                _phase[index] = 0;
                _deltaPhase[index] = 0;
                _alpha[index] = 0;
                _q1[index] = 0;
                _i1[index] = 0;
                _ji[index] = 0;
                _jq[index] = 0;
                _i2[index] = 0;
                _q2[index] = 0;
                _re[index] = 0;
                _im[index] = 0;

                return;
            }


            _price[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;
            _smooth[index] = (4 * _price[index] + 3 * _price[index - 1] + 2 * _price[index - 2] + _price[index - 3]) / 10;
            _detrender[index] = (0.0962 * _smooth[index] + 0.5769 * _smooth[index - 2] - 0.5769 * _smooth[index - 4] - 0.0962 * _smooth[index - 6]) * (0.075 * _period[index - 1] + 0.54);

            //  Compute InPhase and Quadrature components

            _q1[index] = (0.0962 * _detrender[index] + 0.5769 * _detrender[index - 2] - 0.5769 * _detrender[index - 4] - 0.0962 * _detrender[index - 6]) * (0.075 * _period[index - 1] + 0.54);
            _i1[index] = _detrender[index - 3];

            //  Advance the phase of I1 and Q1 by 90 degrees

            _ji[index] = (0.0962 * _i1[index] + 0.5769 * _i1[index - 2] - 0.5769 * _i1[index - 4] - 0.0962 * _i1[index - 6]) * (0.075 * _period[index - 1] + 0.54);
            _jq[index] = (0.0962 * _q1[index] + 0.5769 * _q1[index - 2] - 0.5769 * _q1[index - 4] - 0.0962 * _q1[index - 6]) * (0.075 * _period[index - 1] + 0.54);

            //  Phasor addition for 3 bar averaging

            _i2[index] = _i1[index] - _jq[index];
            _q2[index] = _q1[index] + _ji[index];

            //  Smooth the I and Q components before applying the discriminator

            _i2[index] = 0.2 * _i2[index] + 0.8 * _i2[index - 1];
            _q2[index] = 0.2 * _q2[index] + 0.8 * _q2[index - 1];

            //  Homodyne Discriminator

            _re[index] = _i2[index] * _i2[index - 1] + _q2[index] * _q2[index - 1];
            _im[index] = _i2[index] * _q2[index - 1] - _q2[index] * _i2[index - 1];
            _re[index] = 0.2 * _re[index] + 0.8 * _re[index - 1];
            _im[index] = 0.2 * _im[index] + 0.8 * _im[index - 1];

            double epsilon = Math.Pow(10, -10);
            if (Math.Abs(_im[index] - 0.0) > epsilon && Math.Abs(_re[index] - 0.0) > epsilon)
                if (Math.Abs(Math.Atan(_im[index] / _re[index]) - 0.0) > epsilon)
                    _period[index] = 360 / Math.Atan(_im[index] / _re[index]);
                else
                    _period[index] = 0;

            if (_period[index] > 1.5 * _period[index - 1])
                _period[index] = 1.5 * _period[index - 1];

            if (_period[index] < 0.67 * _period[index - 1])
                _period[index] = 0.67 * _period[index - 1];

            if (_period[index] < 6)
                _period[index] = 6;

            if (_period[index] > 50)
                _period[index] = 50;

            _period[index] = 0.2 * _period[index] + 0.8 * _period[index - 1];

            _smoothPeriod[index] = 0.33 * _period[index] + 0.67 * _smoothPeriod[index - 1];

            if (Math.Abs(_i1[index] - 0) > epsilon)
                _phase[index] = Math.Atan(_q1[index] / _i1[index]);

            if (Math.Abs(Math.Atan(_q1[index] / _i1[index]) - 0.0) > epsilon)
                _phase[index] = Math.Atan(_q1[index] / _i1[index]);
            else
                _phase[index] = 0;

            _deltaPhase[index] = _phase[index - 1] - _phase[index];
            if (_deltaPhase[index] < 1)
                _deltaPhase[index] = 1;

            _alpha[index] = FastLimit / _deltaPhase[index];
            if (_alpha[index] < SlowLimit)
                _alpha[index] = SlowLimit;

            MamaResult[index] = _alpha[index] * _price[index] + ((1 - _alpha[index]) * MamaResult[index - 1]);

            FamaResult[index] = 0.5 * _alpha[index] * MamaResult[index] + (1 - 0.5 * _alpha[index]) * FamaResult[index - 1];


        }
    }
}
