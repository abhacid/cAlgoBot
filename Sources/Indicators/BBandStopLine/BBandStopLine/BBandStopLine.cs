using cAlgo.API;
using cAlgo.API.Indicators;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class BBandStopLine : Indicator
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 1.5)]
        public double StDeviation { get; set; }

        [Parameter(DefaultValue = 15)]
        public int Period { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }

        [Output("Top", PlotType = PlotType.Points, Color = Colors.Red, Thickness = 4)]
        public IndicatorDataSeries Top { get; set; }

        [Output("Bottom", PlotType = PlotType.Points, Color = Colors.Green, Thickness = 4)]
        public IndicatorDataSeries Bottom { get; set; }

        private BollingerBands _bband;
        private int _flag;

        protected override void Initialize()
        {
            _bband = Indicators.BollingerBands(Source, Period, StDeviation, MAType);
        }

        public override void Calculate(int index)
        {
            Top[index] = _bband.Top[index];
            Bottom[index] = _bband.Bottom[index];


            if (MarketSeries.Close[index] > _bband.Top[index])
                _flag = 1;
            else if (MarketSeries.Close[index] < _bband.Bottom[index])
                _flag = -1;

            if (_flag == 1)
            {
                if (_bband.Bottom[index] < Bottom[index - 1])
                    Bottom[index] = Bottom[index - 1];
                Top[index] = double.NaN;
            }
            else if (_flag == -1)
            {
                if (_bband.Top[index] > Top[index - 1])
                    Top[index] = Top[index - 1];
                Bottom[index] = double.NaN;
            }
        }
    }
}
