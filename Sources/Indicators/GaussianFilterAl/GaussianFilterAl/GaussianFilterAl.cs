using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class GaussianFilterAl:Indicator
    {
        private double _beta;
        private double _alpha;
        private double _coeff;
        private double _alphaPow;

        [Parameter(DefaultValue = 12, MinValue = 1)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 3, MinValue = 1, MaxValue = 4)]
        public int Poles { get; set; }

        [Parameter]
        public DataSeries Source { get; set; }

        [Output("Main", Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _beta = (1 - Math.Cos(2*Math.PI/Period))/(Math.Pow(Math.Sqrt(2.0), 2.0/Poles) - 1);
            _alpha = -_beta + Math.Sqrt(_beta*(_beta + 2));
            _coeff = 1.0 - _alpha;
            _alphaPow = Math.Pow(_alpha, Poles);
        }

        public override void Calculate(int index)
        {
            if(index < Poles)
            {
                Result[index] = Source[index];
                return;
            }

            Result[index] = _alphaPow * Source[index] + Poles * _coeff * Result[index - 1];
            
            switch(Poles)
            {
                case 1:
                    break;
                case 2:
                    Result[index] -= Math.Pow(_coeff, 2.0) * Result[index - 2];
                    break;
                case 3:
                    Result[index] -= 3 * Math.Pow(_coeff, 2.0) * Result[index - 2]
                        - Math.Pow(_coeff,3.0)*Result[index - 3];
                    break;
                case 4:
                    Result[index] -= 6 * Math.Pow(_coeff, 2.0) * Result[index - 2]
                        - 4*Math.Pow(_coeff,3.0)*Result[index - 3]
                        + Math.Pow(_coeff, 4.0)*Result[index - 4];
                    break;
            }            
        }
    }
}