using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class FixedOffsetBands : Indicator
    {
        private MovingAverage _ma;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 12)]
        public int MaPeriod { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }

        [Parameter(DefaultValue = 23)]
        public double PipDistance { get; set; }


        [Output("Center", PlotType = PlotType.Line, Color = Colors.Yellow)]
        public IndicatorDataSeries Center { get; set; }

        [Output("Upper", PlotType = PlotType.Line, Color = Colors.Yellow)]
        public IndicatorDataSeries Upper { get; set; }

        [Output("Lower", PlotType = PlotType.Line, Color = Colors.Yellow)]
        public IndicatorDataSeries Lower { get; set; }


        protected override void Initialize()
        {
            _ma = Indicators.MovingAverage(Source, MaPeriod, MAType);
        }

        public override void Calculate(int index)
        {
            if (index > MaPeriod)
            {
                Center[index] = _ma.Result[index];
                Upper[index] = _ma.Result[index] + (PipDistance * Symbol.PipSize);
                Lower[index] = _ma.Result[index] - (PipDistance * Symbol.PipSize);
            }
        }
    }
}
