using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator("Adaptive Laguerre Filter", IsOverlay = true, AccessRights = AccessRights.None)]
    public class AdaptiveLaguerreFilter : Indicator
    {
        private const int MedianArraySize = 5;
        private IndicatorDataSeries _diff;
        private IndicatorDataSeries _laguerre0;
        private IndicatorDataSeries _laguerre1;
        private IndicatorDataSeries _laguerre2;
        private IndicatorDataSeries _laguerre3;
        private IndicatorDataSeries _price;


        [Parameter(DefaultValue = 14, MinValue = 5)]
        public int Period { get; set; }

        [Output("Adaptive Laguerre Filter", Color = Colors.MediumOrchid, Thickness = 2)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _laguerre0 = CreateDataSeries();
            _laguerre1 = CreateDataSeries();
            _laguerre2 = CreateDataSeries();
            _laguerre3 = CreateDataSeries();
            _diff = CreateDataSeries();

            _price = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            _price[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;
            if(index == 0)
            {
                _laguerre0[index] = 0.0;
                _laguerre1[index] = 0.0;
                _laguerre2[index] = 0.0;
                _laguerre3[index] = 0.0;

                Result[index] = 0.0;
                return;
            }

            _diff[index] = Math.Abs(_price[index] - Result[index - 1]);
            
            double alpha = 0.0;

            if (index >= Period)
            {
                double max = _diff.Maximum(Period);
                double min = _diff.Minimum(Period);
                if (Math.Abs(max - min) > double.Epsilon)
                {
                    var array = new double[MedianArraySize];
                    for (int i = 0; i < MedianArraySize; i++)
                        array[i] = _diff[index - i];

                    //Array.Sort(array);
                    const int medianIndex = MedianArraySize/2;
                    alpha = (array[medianIndex] - min)/(max - min);
                }
            }

            _laguerre0[index] = alpha*_price[index] + (1 - alpha)*_laguerre0[index - 1];

            _laguerre1[index] = -(1 - alpha)*_laguerre0[index] + _laguerre0[index - 1] + (1 - alpha)*_laguerre1[index - 1];
            _laguerre2[index] =-(1 - alpha)*_laguerre1[index] + _laguerre1[index - 1] + (1 - alpha)*_laguerre2[index - 1];
            _laguerre3[index] =-(1 - alpha)*_laguerre2[index] + _laguerre2[index - 1] + (1 - alpha)*_laguerre3[index - 1];

            Result[index] = (_laguerre0[index] + 2*_laguerre1[index] + 2*_laguerre2[index] + _laguerre3[index])/6;
            
        }
    }
}