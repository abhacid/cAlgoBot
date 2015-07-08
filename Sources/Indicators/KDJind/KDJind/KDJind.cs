using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    [Levels(20, 80)]
    public class KDJind : Indicator
    {
        private IndicatorDataSeries _denominator;
        private IndicatorDataSeries _nominator;
        private IndicatorDataSeries _fastK;
        private MovingAverage _mak;
        private MovingAverage _mad;

        [Parameter(DefaultValue = 7)]
        public int DPeriod { get; set; }
        
        [Parameter(DefaultValue = 14)]
        public int KPeriod { get; set; }
        
        [Parameter(DefaultValue = 3)]
        public int Smooth { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Output("%K")]
        public IndicatorDataSeries KSeries { get; set; }
        
        [Output("%D", Color = Colors.Red, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries DSeries { get; set; }
        
        [Output("%J", Color = Colors.Blue)]
        public IndicatorDataSeries JSeries { get; set; }

        // Levels
        [Output("100", Color = Colors.Gray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries Line100 { get; set; }

        [Output("0", Color = Colors.Gray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries Line0 { get; set; }

        protected override void Initialize()
        {
            _denominator = CreateDataSeries();
            _nominator = CreateDataSeries();
            _fastK = CreateDataSeries();
            CreateDataSeries();
            _mak = Indicators.MovingAverage(_fastK, Smooth, MaType);
            _mad = Indicators.MovingAverage(_mak.Result, DPeriod, MaType);

        }
        public override void Calculate(int index)
        {
            double min = MarketSeries.Low.Minimum(KPeriod);
            double max = MarketSeries.High.Maximum(KPeriod);

            _nominator[index] = MarketSeries.Close[index] - min;
            _denominator[index] = max - min;

            if(Math.Abs(_denominator[index] - 0) < double.Epsilon)
            {
                _fastK[index] = index == 0 ? 50 : _fastK[index - 1];
            }
            else
            {
                double percent = 100*_nominator[index]/_denominator[index];
                _fastK[index] = Math.Min(100, Math.Max(0, percent));
            }

            KSeries[index] = _mak.Result[index];
            DSeries[index] = _mad.Result[index];
            JSeries[index] = 3*_mak.Result[index] - 2*_mad.Result[index];

            Line0[index] = 0.0;
            Line100[index] = 0.0;
        }
    }
}
