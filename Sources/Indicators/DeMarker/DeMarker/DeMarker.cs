using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    [Levels(0.3, 0.7)]
    public class DeMarker : Indicator
    {
        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        private IndicatorDataSeries _deMax;
        private IndicatorDataSeries _deMin;
        private SimpleMovingAverage _deMinSma;
        private SimpleMovingAverage _deMaxSma;


        protected override void Initialize()
        {
            _deMax = CreateDataSeries();
            _deMin = CreateDataSeries();
            _deMaxSma = Indicators.SimpleMovingAverage(_deMax, Period);
            _deMinSma = Indicators.SimpleMovingAverage(_deMin, Period);
        }

        public override void Calculate(int index)
        {
            if (MarketSeries.High[index] > MarketSeries.High[index - 1])
                _deMax[index] = MarketSeries.High[index] - MarketSeries.High[index - 1];
            else
                _deMax[index] = 0;

            if (MarketSeries.Low[index] < MarketSeries.Low[index - 1])
                _deMin[index] = MarketSeries.Low[index - 1] - MarketSeries.Low[index];
            else
                _deMin[index] = 0;

            Result[index] = _deMaxSma.Result[index] / (_deMaxSma.Result[index] + _deMinSma.Result[index]);
        }
    }
}
