using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class HullMovingAverage:Indicator
    {
        private WeightedMovingAverage _wma;
        private WeightedMovingAverage _wma2;
        private WeightedMovingAverage _wma3;
        private IndicatorDataSeries _iSeries;

        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 20, MinValue = 1)]
        public int Period { get; set; }


        [Output("Main", Color = Colors.Violet)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            //wma(2*wma(close,period/2)-wma(close,period), sqrt(Period))
            _iSeries = CreateDataSeries();

            _wma = Indicators.WeightedMovingAverage(Source, Period / 2);
            _wma2 = Indicators.WeightedMovingAverage(Source, Period);
            _wma3 = Indicators.WeightedMovingAverage(_iSeries, (int) Math.Sqrt(Period));
        }
        public override void Calculate(int index)
        {
            double price = Source[index];

            if (index < Period)
            {
                Result[index] = price;
                return;
            }

            _iSeries[index] = 2 * _wma.Result[index] - _wma2.Result[index];
            Result[index] = _wma3.Result[index];

        }


    }
}
