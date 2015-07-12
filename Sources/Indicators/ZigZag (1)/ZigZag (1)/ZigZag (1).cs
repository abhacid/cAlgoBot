using System;
using cAlgo.API;
using cAlgo.API.Internals;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class ZigZag : Indicator
    {

        [Parameter(DefaultValue = 12)]
        public int Depth { get; set; }

        [Parameter(DefaultValue = 5)]
        public int Deviation { get; set; }

        [Parameter(DefaultValue = 3)]
        public int BackStep { get; set; }

        [Output("ZigZag", Color = Colors.OrangeRed)]
        public IndicatorDataSeries Result { get; set; }

        #region Private fields

        private double _lastLow;
        private double _lastHigh;
        private double _low;
        private double _high;
        private int _lastHighIndex;
        private int _lastLowIndex;
        private int _type;
        private double _point;
        private double _currentLow;
        private double _currentHigh;

        private IndicatorDataSeries _highZigZags;
        private IndicatorDataSeries _lowZigZags;

        #endregion

        protected override void Initialize()
        {
            _highZigZags = CreateDataSeries();
            _lowZigZags = CreateDataSeries();
            _point = Symbol.PointSize;
        }

        public override void Calculate(int index)
        {

            if (index < Depth)
            {
                Result[index] = 0;
                _highZigZags[index] = 0;
                _lowZigZags[index] = 0;
                return;
            }

            _currentLow = Functions.Minimum(MarketSeries.Low, Depth);
            if (Math.Abs(_currentLow - _lastLow) < double.Epsilon)
                _currentLow = 0.0;
            else
            {
                _lastLow = _currentLow;

                if ((MarketSeries.Low[index] - _currentLow) > (Deviation * _point))
                    _currentLow = 0.0;
                else
                {
                    for (int i = 1; i <= BackStep; i++)
                    {
                        if (Math.Abs(_lowZigZags[index - i]) > double.Epsilon && _lowZigZags[index - i] > _currentLow)
                            _lowZigZags[index - i] = 0.0;
                    }
                }
            }
            if (Math.Abs(MarketSeries.Low[index] - _currentLow) < double.Epsilon)
                _lowZigZags[index] = _currentLow;
            else
                _lowZigZags[index] = 0.0;

            _currentHigh = MarketSeries.High.Maximum(Depth);

            if (Math.Abs(_currentHigh - _lastHigh) < double.Epsilon)
                _currentHigh = 0.0;
            else
            {
                _lastHigh = _currentHigh;

                if ((_currentHigh - MarketSeries.High[index]) > (Deviation * _point))
                    _currentHigh = 0.0;
                else
                {
                    for (int i = 1; i <= BackStep; i++)
                    {
                        if (Math.Abs(_highZigZags[index - i]) > double.Epsilon && _highZigZags[index - i] < _currentHigh)
                            _highZigZags[index - i] = 0.0;
                    }
                }
            }

            if (Math.Abs(MarketSeries.High[index] - _currentHigh) < double.Epsilon)
                _highZigZags[index] = _currentHigh;
            else
                _highZigZags[index] = 0.0;


            switch (_type)
            {
                case 0:
                    if (Math.Abs(_low - 0) < double.Epsilon && Math.Abs(_high - 0) < double.Epsilon)
                    {
                        if (Math.Abs(_highZigZags[index]) > double.Epsilon)
                        {
                            _high = MarketSeries.High[index];
                            _lastHighIndex = index;
                            _type = -1;
                            Result[index] = _high;
                        }
                        if (Math.Abs(_lowZigZags[index]) > double.Epsilon)
                        {
                            _low = MarketSeries.Low[index];
                            _lastLowIndex = index;
                            _type = 1;
                            Result[index] = _low;
                        }
                    }
                    break;
                case 1:
                    if (Math.Abs(_lowZigZags[index]) > double.Epsilon && _lowZigZags[index] < _low && Math.Abs(_highZigZags[index] - 0.0) < double.Epsilon)
                    {
                        Result[_lastLowIndex] = double.NaN;
                        _lastLowIndex = index;
                        _low = _lowZigZags[index];
                        Result[index] = _low;
                    }
                    if (Math.Abs(_highZigZags[index] - 0.0) > double.Epsilon && Math.Abs(_lowZigZags[index] - 0.0) < double.Epsilon)
                    {
                        _high = _highZigZags[index];
                        _lastHighIndex = index;
                        Result[index] = _high;
                        _type = -1;
                    }
                    break;
                case -1:
                    if (Math.Abs(_highZigZags[index]) > double.Epsilon && _highZigZags[index] > _high && Math.Abs(_lowZigZags[index] - 0.0) < double.Epsilon)
                    {
                        Result[_lastHighIndex] = double.NaN;
                        _lastHighIndex = index;
                        _high = _highZigZags[index];
                        Result[index] = _high;
                    }
                    if (Math.Abs(_lowZigZags[index]) > double.Epsilon && Math.Abs(_highZigZags[index]) <= double.Epsilon)
                    {
                        _low = _lowZigZags[index];
                        _lastLowIndex = index;
                        Result[index] = _low;
                        _type = 1;
                    }
                    break;
                default:
                    return;
            }

        }
    }
}
