using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    class HighMinusLow : Indicator
    {
        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        public override void Calculate(int index)
        {
            Result[index] = MarketSeries.High[index] - MarketSeries.Low[index];
        }
    }
}
