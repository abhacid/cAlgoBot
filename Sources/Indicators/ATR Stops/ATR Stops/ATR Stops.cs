using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator("ATR Trailing Stop", AutoRescale = false, IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ATRStops : Indicator
    {

        [Parameter("MA Method", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Parameter("Period", DefaultValue = 15, MinValue = 2, MaxValue = 50)]
        public int Period { get; set; }

        [Parameter("Weight", DefaultValue = 3.0, MinValue = 0.1, MaxValue = 4.0)]
        public double Weight { get; set; }

        [Parameter("True:High_Low False:Close", DefaultValue = true)]
        public bool UseHighAndLow { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        private AverageTrueRange _atr;
        private bool _isLong;

        protected override void Initialize()
        {
            _atr = Indicators.AverageTrueRange(Period, MaType);
        }

        public override void Calculate(int index)
        {
            var currentAtr = Weight * _atr.Result[index];

            if (double.IsNaN(currentAtr))
                return;

            if (double.IsNaN(Result[index - 1]) && !double.IsNaN(_atr.Result[index - 1]))
            {
                var previousATR = Weight * _atr.Result[index - 1];

                _isLong = MarketSeries.Close.IsRising();

                var previous = UseHighAndLow ? (_isLong ? MarketSeries.High[index - 1] : MarketSeries.Low[index - 1]) : MarketSeries.Close[index - 1];

                Result[index] = _isLong ? previous - previousATR : previous + previousATR;
            }
            else
            {
                var current = MarketSeries.Close[index];

                if (_isLong)
                {
                    if (current >= Result[index - 1])
                    {
                        if (UseHighAndLow)
                            current = MarketSeries.High[index];
                        Result[index] = Math.Max(Result[index - 1], current - currentAtr);
                    }
                    else
                    {
                        _isLong = false;
                        if (UseHighAndLow)
                            current = MarketSeries.Low[index];
                        Result[index] = current + currentAtr;
                    }
                }
                else
                {
                    if (current <= Result[index - 1])
                    {
                        if (UseHighAndLow)
                            current = MarketSeries.Low[index];
                        Result[index] = Math.Min(Result[index - 1], current + currentAtr);
                    }
                    else
                    {
                        _isLong = true;
                        if (UseHighAndLow)
                            current = MarketSeries.High[index];
                        Result[index] = current - currentAtr;
                    }
                }
            }
        }
    }
}
