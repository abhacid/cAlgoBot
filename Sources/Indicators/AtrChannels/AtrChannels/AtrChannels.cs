// -------------------------------------------------------------------------------
//
//    Average True Range Channels
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class AtrChannels : Indicator
    {
        private AverageTrueRange atr;
        private MovingAverage ma;

        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 14)]
        public int ATRPeriods { get; set; }

        [Parameter(DefaultValue = 14)]
        public int MAPeriods { get; set; }

        [Parameter(DefaultValue = MovingAverageType.Weighted)]
        public MovingAverageType MAType { get; set; }

        [Parameter(DefaultValue = 1)]
        public double WeightCoef1 { get; set; }

        [Parameter(DefaultValue = 3)]
        public double WeightCoef2 { get; set; }

        [Parameter(DefaultValue = 4.8)]
        public double WeightCoef3 { get; set; }

        [Output("MA", PlotType = PlotType.Line, Color = Colors.Green)]
        public IndicatorDataSeries MA { get; set; }

        [Output("UpChannel1", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries UpChannel1 { get; set; }

        [Output("DownChannel1", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries DownChannel1 { get; set; }

        [Output("UpChannel2", PlotType = PlotType.Line, Color = Colors.Blue)]
        public IndicatorDataSeries UpChannel2 { get; set; }

        [Output("DownChannel2", PlotType = PlotType.Line, Color = Colors.Blue)]
        public IndicatorDataSeries DownChannel2 { get; set; }

        [Output("UpChannel3", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries UpChannel3 { get; set; }

        [Output("DownChannel3", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries DownChannel3 { get; set; }


        protected override void Initialize()
        {
            ma = Indicators.MovingAverage(Source, MAPeriods, MAType);
            atr = Indicators.AverageTrueRange(ATRPeriods, MovingAverageType.Simple);
        }

        public override void Calculate(int index)
        {
            MA[index] = ma.Result[index];

            UpChannel1[index] = ma.Result[index] + atr.Result[index] * WeightCoef1;
            DownChannel1[index] = ma.Result[index] - atr.Result[index] * WeightCoef1;

            UpChannel2[index] = ma.Result[index] + atr.Result[index] * WeightCoef2;
            DownChannel2[index] = ma.Result[index] - atr.Result[index] * WeightCoef2;

            UpChannel3[index] = ma.Result[index] + atr.Result[index] * WeightCoef3;
            DownChannel3[index] = ma.Result[index] - atr.Result[index] * WeightCoef3;
        }
    }
}
