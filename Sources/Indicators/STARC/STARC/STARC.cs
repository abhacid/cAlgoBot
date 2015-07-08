//#reference: AverageTrueRange.algo
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class STARC : Indicator
    {
        private AverageTrueRange _atr;
        private MovingAverage _ma;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 14)]
        public int AtrPeriod { get; set; }

        [Parameter(DefaultValue = 14)]
        public int MaPeriod { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Weighted)]
        public MovingAverageType MAType { get; set; }

        [Parameter(DefaultValue = 1.6)]
        public double ATRMultiplier { get; set; }


        [Output("Center", PlotType = PlotType.Line, Color = Colors.Green)]
        public IndicatorDataSeries Center { get; set; }

        [Output("Upper", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries Upper { get; set; }

        [Output("Lower", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries Lower { get; set; }


        protected override void Initialize()
        {
            _ma = Indicators.MovingAverage(Source, MaPeriod, MAType);
            _atr = Indicators.GetIndicator<AverageTrueRange>(AtrPeriod);
        }

        public override void Calculate(int index)
        {
            if (index > Math.Max(AtrPeriod, MaPeriod))
            {
                Center[index] = _ma.Result[index];
                Upper[index] = _ma.Result[index] + _atr.Result[index] * ATRMultiplier;
                Lower[index] = _ma.Result[index] - _atr.Result[index] * ATRMultiplier;

            }
        }
    }
}
