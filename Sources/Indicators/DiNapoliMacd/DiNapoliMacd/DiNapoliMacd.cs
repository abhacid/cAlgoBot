using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class DiNapoliMacd : Indicator
    {
        [Parameter(DefaultValue = 17.5185)]
        public double LongCycle { get; set; }
        
        [Parameter(DefaultValue = 8.3896)]
        public double ShortCycle { get; set; }
        
        [Parameter(DefaultValue = 9.0503)]
        public double SignalPeriod { get; set; }

        [Output("Main", PlotType = PlotType.Histogram, Color = Colors.SkyBlue)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Signal", Color = Colors.Red, LineStyle = LineStyle.Dots)]
        public IndicatorDataSeries Signal { get; set; }

        private IndicatorDataSeries _fastSeries;
        private IndicatorDataSeries _slowSeries;

        protected override void Initialize()
        {
            _fastSeries = CreateDataSeries();
            _slowSeries= CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            if(index == 0)
            {
                Result[index] = 0.0;
                Signal[index] = 0.0;
                _fastSeries[index] = 0.0;
                _slowSeries[index] = 0.0;
                return;
            }

            _fastSeries[index] = _fastSeries[index - 1] + 2.0 / (1.0 + ShortCycle) * (MarketSeries.Close[index] - _fastSeries[index - 1]);
            _slowSeries[index] = _slowSeries[index - 1] + 2.0 / (1.0 + LongCycle) * (MarketSeries.Close[index] - _slowSeries[index - 1]);
            Result[index] = _fastSeries[index] - _slowSeries[index];
            Signal[index] = Signal[index - 1] + 2.0/(1 + SignalPeriod)*(Result[index] - Signal[index - 1]);
        }
    }
}
