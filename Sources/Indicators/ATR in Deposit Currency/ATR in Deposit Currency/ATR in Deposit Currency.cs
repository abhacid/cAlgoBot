using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ATRinDepositCurrency : Indicator
    {
        private AverageTrueRange _atr;

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MAType { get; set; }

        [Parameter(DefaultValue = 10000)]
        public int Volume { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _atr = Indicators.AverageTrueRange(Period, MAType);
        }

        public override void Calculate(int index)
        {
            Result[index] = _atr.Result[index] * Volume * (Symbol.PipValue / Symbol.PipSize);
        }
    }
}
