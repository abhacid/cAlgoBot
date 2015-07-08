using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class ZeroLagEMA:Indicator
    {
        private double _alpha;
        private int _lag;

        [Parameter(DefaultValue = 14, MinValue = 5)]
        public int Period {get; set; }

        [Parameter]
        public DataSeries Source { get; set; }

        [Output("Main", Color = Colors.Orange)]
        public IndicatorDataSeries Result { get; set; }


        public override void Calculate(int index)
        {

            if (index == 0)
            {
                _alpha = 2.0/(Period + 1);
                _lag = (int) Math.Ceiling((Period - 1)/2.0);                
            }
            if(index < _lag)
            {
                Result[index] = Source[index];
                return;
            }

            Result[index] = _alpha * (2 * Source[index] - Source[index -_lag]) + (1 - _alpha) * Result[index - 1];

        }
    }
}
