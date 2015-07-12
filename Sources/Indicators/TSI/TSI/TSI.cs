using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Levels(-25, 0, 25)]
    [Indicator(AccessRights = AccessRights.None)]
    public class TSI : Indicator
    {
        private MovingAverage _divisor;
        private MovingAverage _longDivisor;
        private MovingAverage _dividend;
        private MovingAverage _longDividend;

        private IndicatorDataSeries _dataSeries;
        private IndicatorDataSeries _dataSeriesAbs;
        private MovingAverage _signal;
        private IndicatorDataSeries _tsiSeries;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter("ShortPeriod", DefaultValue = 13, MinValue = 1)]
        public int ShortPeriod { get; set; }

        [Parameter("LongPeriod", DefaultValue = 25, MinValue = 1)]
        public int LongPeriod { get; set; }

        [Parameter("SignalPeriod", DefaultValue = 7, MinValue = 1)]
        public int SignalPeriod { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Exponential, MinValue = MovingAverageType.Simple, MaxValue = MovingAverageType.Exponential)]
        public MovingAverageType MaType { get; set; }


        [Output("Tsi", Color = Colors.DarkBlue)]
        public IndicatorDataSeries Tsi { get; set; }
        [Output("Signal", Color = Colors.LightBlue)]
        public IndicatorDataSeries Signal { get; set; }
        [Output("Diff", Color = Colors.Gray, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries Diff { get; set; }

        protected override void Initialize()
        {
            //_ma = Indicators.MovingAverage(MarketSeries.Close, SignalPeriod, MaType);
            _dataSeries = CreateDataSeries();
            _dataSeriesAbs = CreateDataSeries();

            _longDividend = Indicators.MovingAverage(_dataSeries, LongPeriod, MaType);
            _dividend = Indicators.MovingAverage(_longDividend.Result, ShortPeriod, MaType);

            _longDivisor = Indicators.MovingAverage(_dataSeriesAbs, LongPeriod, MaType);
            _divisor = Indicators.MovingAverage(_longDivisor.Result, ShortPeriod, MaType);

            _tsiSeries = CreateDataSeries();
            _signal = Indicators.MovingAverage(_tsiSeries, SignalPeriod, MaType);

        }

        public override void Calculate(int index)
        {
            if (index < 1)
            {
                Tsi[index] = 0;
                return;
            }

            _dataSeries[index] = MarketSeries.Close[index] - MarketSeries.Close[index - 1];
            _dataSeriesAbs[index] = Math.Abs(MarketSeries.Close[index] - MarketSeries.Close[index - 1]);

            double tsiDivisor = _divisor.Result[index];
            double tsiDividend = _dividend.Result[index];

            if (Math.Abs(tsiDivisor) < double.Epsilon)
                _tsiSeries[index] = 0;
            else
                _tsiSeries[index] = 100.0 * tsiDividend / tsiDivisor;

            Tsi[index] = _tsiSeries[index];
            Signal[index] = _signal.Result[index];

            Diff[index] = _tsiSeries[index] - Signal[index];
        }



    }
}
