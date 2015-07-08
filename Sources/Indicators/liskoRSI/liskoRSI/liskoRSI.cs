using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;
using System;

namespace cAlgo.Indicators
{

    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AutoRescale = false, AccessRights = AccessRights.None)]
    public class liskoRSI : Indicator
    {
        [Parameter("Typical(OHC/3)", DefaultValue = true)]
        public bool is_typic { get; set; }
        [Parameter("WClose (OHCC/4)", DefaultValue = false)]
        public bool is_wclose { get; set; }
        [Parameter("Median (OH/2)", DefaultValue = false)]
        public bool is_median { get; set; }
        [Parameter("Period:", DefaultValue = 10)]
        public int Period { get; set; }

        [Output("level80", Color = Colors.Red, LineStyle = LineStyle.Dots, PlotType = PlotType.Line, Thickness = 1)]
        public IndicatorDataSeries level80 { get; set; }
        [Output("level20", Color = Colors.Green, LineStyle = LineStyle.Dots, PlotType = PlotType.Line, Thickness = 1)]
        public IndicatorDataSeries level20 { get; set; }

        [Output("liskorsi", Color = Colors.Blue, PlotType = PlotType.Line, Thickness = 1)]
        public IndicatorDataSeries liskoRSIResult { get; set; }

        public RelativeStrengthIndex rsi;

        protected override void Initialize()
        {
            DataSeries ds;
            ds = MarketSeries.Typical;
            if (is_typic)
            {
                ds = MarketSeries.Typical;
            }
            if (is_wclose)
            {
                ds = MarketSeries.WeightedClose;
            }
            if (is_median)
            {
                ds = MarketSeries.Median;
            }
            rsi = Indicators.RelativeStrengthIndex(ds, Period);
        }

        public override void Calculate(int index)
        {
            liskoRSIResult[index] = rsi.Result[index];
            level80[index] = 80;
            level20[index] = 20;
        }
    }
}
