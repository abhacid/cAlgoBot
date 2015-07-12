using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class XPoleSuperSmother : Indicator
    {
        private double _coef1;
        private double _coef2;
        private double _coef3;
        private double _coef4;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 20, MinValue = 5)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 2, MinValue = 2, MaxValue = 3)]
        public int Poles { get; set; }

        [Output("Main", Color = Colors.Violet)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            SetCoefficients();
        }
        public override void Calculate(int index)
        {
            double price = Source[index];

            if (index < Poles)
            {
                Result[index] = price;
                return;
            }

            Result[index] = _coef1 * price + _coef2 * Result[index - 1] + _coef3 * Result[index - 2];

            if (Poles == 3)
                Result[index] += _coef4 * Result[index - 3];

        }

        private void SetCoefficients()
        {
            if (Poles == 2)
            {
                double a = Math.Exp(-Math.Sqrt(2.0) * Math.PI / Period);

                _coef2 = 2 * a * Math.Cos(Math.Sqrt(2.0) * Math.PI / Period);
                _coef3 = -a * a;

                _coef1 = 1 - _coef2 - _coef3;
            }
            else
            {
                double a = Math.Exp(-Math.PI / Period);
                double b = 2 * a * Math.Cos(Math.Sqrt(3.0) * Math.PI / Period);

                _coef2 = a * a + b;
                _coef3 = -a * a * (1 + b);
                _coef4 = Math.Pow(a, 4.0);

                _coef1 = 1 - _coef2 - _coef3 - _coef4;

            }
        }


    }
}
