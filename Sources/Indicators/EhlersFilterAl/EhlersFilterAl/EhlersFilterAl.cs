using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class EhlersFilterAl:Indicator
    {

        private TriangularMovingAverage _triangularMovingAverage;
        private double[] _coef;

        [Parameter(DefaultValue = 14, MinValue = 5)]
        public int Period { get; set; }

        [Parameter]
        public DataSeries Source { get; set; }

        [Output("Main", Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries Result { get; set; }

        protected override void  Initialize()
        {            
            _triangularMovingAverage = Indicators.TriangularMovingAverage(Source, 4);
        }

        public override void Calculate(int index)
        {
            int length;

            if (index < 2*Period - 1)
                length = index/2 + 1;
            else
                length = Period;
            
            _coef = new double[length];

            for (int i = 0; i < length; i++)
            {
                double distance = 0;
                for (int j = 1; j < length; j++)
                    distance +=
                        Math.Pow(
                            (_triangularMovingAverage.Result[index - i] - _triangularMovingAverage.Result[index - i - j]),
                            2.0);
                _coef[i] = distance;
            }

            double num = 0.0;
            double sumCoef = 0.0;

            for (int i = 0; i < length; i++)
            {
                num += _coef[i] * _triangularMovingAverage.Result[index - i];
                sumCoef += _coef[i];
            }
            if ( Math.Abs(sumCoef - 0) > double.Epsilon)
                Result[index] = num / sumCoef;
            }
        }
}
