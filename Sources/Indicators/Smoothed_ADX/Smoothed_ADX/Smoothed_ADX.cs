// -------------------------------------------------------------------------------------------------
//
//    Smoothed ADX Indicator
//
// -------------------------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class Smoothed_ADX : Indicator
    {
        private DirectionalMovementSystem _dms;
        private SimpleMovingAverage _smaHigh, _smaLow;
        private double _DIplus;
        private double _DIminus;

        private IndicatorDataSeries _highSeries, _lowSeries;

        [Parameter("ADX Period", DefaultValue = 14)]
        public int AdxPeriod { get; set; }

        [Parameter("SMA Period", DefaultValue = 5)]
        public int SmaPeriod { get; set; }

        [Output("High", PlotType = PlotType.Line, Color = Colors.Blue)]
        public IndicatorDataSeries High { get; set; }

        [Output("Low", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries Low { get; set; }

        protected override void Initialize()
        {
            _highSeries = CreateDataSeries();
            _lowSeries = CreateDataSeries();

            _smaHigh = Indicators.SimpleMovingAverage(_highSeries, SmaPeriod);
            _smaLow = Indicators.SimpleMovingAverage(_lowSeries, SmaPeriod);

            _dms = Indicators.DirectionalMovementSystem(AdxPeriod);
        }

        public override void Calculate(int index)
        {
            _DIplus = _dms.DIPlus[index];
            _DIminus = _dms.DIMinus[index];

            if (_DIminus > _DIplus)
            {
                _lowSeries[index] = MarketSeries.Low[index];
                _highSeries[index] = MarketSeries.High[index];
            }
            else if(_DIminus < _DIplus)
            {
                _lowSeries[index] = MarketSeries.High[index];
                _highSeries[index] = MarketSeries.Low[index];
            }

            High[index] = _smaHigh.Result[index];
            Low[index] = _smaLow.Result[index];

        }
    }
}