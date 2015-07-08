using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Levels(0)]
    [Indicator(AccessRights = AccessRights.None)]
    public class ElliotOscillator : Indicator
    {
        private SimpleMovingAverage _fastSma;
        private SimpleMovingAverage _slowSma;
        private SimpleMovingAverage _sma100;
        private SimpleMovingAverage _sma200;
        private SimpleMovingAverage _sma20;

        private double _d;
        private bool _upTrend;
        private bool _neutral;
        private IndicatorDataSeries _elliot;


        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter("FastPeriod", DefaultValue = 5)]
        public int FastPeriod { get; set; }
        
        [Parameter("SlowPeriod", DefaultValue = 34)]
        public int SlowPeriod { get; set; }

        [Output("UpTrend", Color = Colors.Green, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries UpTrend { get; set; }
        [Output("DownTrend", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries DownTrend { get; set; }
        [Output("Neutral", Color = Colors.Gray, PlotType = PlotType.Histogram, Thickness = 2)]
        public IndicatorDataSeries Neutral { get; set; }
        
        [Output("Line", Color = Colors.Red)]
        public IndicatorDataSeries Line { get; set; }

        protected override void Initialize()
        {
            _fastSma = Indicators.SimpleMovingAverage(Source, FastPeriod);
            _slowSma = Indicators.SimpleMovingAverage(Source, SlowPeriod);
            _sma100 = Indicators.SimpleMovingAverage(Source, 100);
            _sma200 = Indicators.SimpleMovingAverage(Source, 200);
            _sma20 = Indicators.SimpleMovingAverage(Source, 20);
            _elliot = CreateDataSeries();

        }
        public override void Calculate(int index)
        {
            if (index < 3)
                return;

            _elliot[index] = _fastSma.Result[index] - _slowSma.Result[index];
            Line[index] = _fastSma.Result[index - 3] - _slowSma.Result[index - 3];

            if (_sma100.Result.LastValue > _sma200.Result.LastValue
                && _sma20.Result.LastValue >_sma100.Result.LastValue)
            {
                UpTrend[index] = _elliot[index];
                DownTrend[index] = double.NaN;
                Neutral[index] = double.NaN;
            }
            else if (_sma100.Result.LastValue < _sma200.Result.LastValue
                && _sma20.Result.LastValue < _sma100.Result.LastValue)
            {
                DownTrend[index] = _elliot[index];
                UpTrend[index] = double.NaN;
                Neutral[index] = double.NaN;
            }
            else
            {
                Neutral[index] = _elliot[index];
                UpTrend[index] = double.NaN;
                DownTrend[index] = double.NaN;
            }
        }
    }
}
