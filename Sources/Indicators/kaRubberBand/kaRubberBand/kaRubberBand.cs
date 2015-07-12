using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class kaRubberBand : Indicator
    {
        #region Variables

        double _syncShort, _syncLong, _syncMulti;
        double _shortWeight, _longWeight, _multiWeight;

        #endregion

        #region parameters

        [Parameter(DefaultValue = 35)]
        public int LongLeg { get; set; }

        [Parameter(DefaultValue = 1)]
        public int ShortLeg { get; set; }

        [Parameter(DefaultValue = 1)]
        public int MultiW { get; set; }

        [Parameter(DefaultValue = 10)]
        public int MultiLeg { get; set; }

        #endregion

        #region output

        [Output("Zero", Color = Colors.Gray)]
        public IndicatorDataSeries ZeroLine { get; set; }

        [Output("High", Color = Colors.Violet)]
        public IndicatorDataSeries High { get; set; }

        [Output("Low", Color = Colors.Violet)]
        public IndicatorDataSeries Low { get; set; }

        [Output("Oscillator")]
        public IndicatorDataSeries Oscillator { get; set; }

        #endregion

        protected override void Initialize()
        {
            _shortWeight = 2.0 / (ShortLeg + 1);
            _longWeight = 2.0 / (LongLeg + 1);
            _multiWeight = 2.0 / (MultiLeg + 1);

        }
        public override void Calculate(int index)
        {
            // Init
            if (index == 0)
            {
                _syncShort = MarketSeries.Close[index];
                _syncLong = MarketSeries.Close[index];
            }
            else
            {
                _syncShort = _syncShort * (1 - _shortWeight) + _shortWeight * MarketSeries.Close[index];
                _syncLong = _syncLong * (1 - _longWeight) + _longWeight * MarketSeries.Close[index];
            }

            double multiOsc = 100 * ((_syncShort / _syncLong) - 1);

            if (Math.Abs(_syncMulti - 0) < double.Epsilon)
                _syncMulti = multiOsc;
            else
                _syncMulti = Math.Abs(_syncMulti) * (1 - _multiWeight) + (_multiWeight * multiOsc);

            High[index] = _syncMulti * MultiW;
            Low[index] = -_syncMulti * MultiW;
            Oscillator[index] = multiOsc;

            ZeroLine[index] = 0.0;
        }
    }
}
