using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class FDI : Indicator
    {
        private double _diff;
        private double _highnow;
        private double _length;
        private double _lownow;
        private double _priorDiff;

        [Parameter(DefaultValue = 30)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 1.5)]
        public double MidLine { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        [Output("Midline", Color = Colors.Red)]
        public IndicatorDataSeries Midline { get; set; }

        public override void Calculate(int index)
        {
            _highnow = Functions.Maximum(MarketSeries.Close, Period);
            _lownow = Functions.Minimum(MarketSeries.Close, Period);
            _length = 0;
            _priorDiff = 0;
            
            for (int idx = 1; idx < Period; idx++)
                if (_highnow - _lownow > 0)
                {
                    _diff = (MarketSeries.Close[index - idx] - _lownow)/(_highnow - _lownow);
                    if (idx > 1)
                        _length = (_length + Math.Sqrt(Math.Pow((_diff - _priorDiff) + (1/Math.Pow(Period, 2)), 2)));
                    _priorDiff = _diff;
                }

            if (_length > 0)
                Result[index] = 1 + (Math.Log(_length) + Math.Log(2))/Math.Log(2*(Period));
            else
                Result[index] = 0;
                
            Midline[index] = MidLine;
        }
    }
}