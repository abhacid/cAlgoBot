

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{

    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class CandlestickTendency : Indicator
    {

        [Parameter()]
        public TimeFrame HighOrderTimeFrame { get; set; }

        [Output("Line", PlotType = PlotType.Line, Color = Colors.Green)]
        public IndicatorDataSeries Line { get; set; }

        [Output("Histogram", PlotType = PlotType.Histogram, Color = Colors.DarkGreen)]
        public IndicatorDataSeries Histogram { get; set; }

        [Output("High Order Line", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries HighOrderLine { get; set; }

        int index1, index2;
        MarketSeries series2;
        double value1, value2;

        protected override void Initialize()
        {

            value1 = value2 = 0;
            series2 = MarketData.GetSeries(HighOrderTimeFrame);
        }

        public bool trend1IsRising
        {
            get { return (MarketSeries.Close[index1] > MarketSeries.Open[index1 - 1]); }
        }
        public bool trend1IsFalling
        {
            get { return (MarketSeries.Close[index1] < MarketSeries.Open[index1 - 1]); }
        }
        public bool trend2IsRising
        {
            get { return (series2.Close[index2] > series2.Open[index2 - 1]); }
        }
        public bool trend2IsFalling
        {
            get { return (series2.Close[index2] < series2.Open[index2 - 1]); }
        }

        public override void Calculate(int index)
        {

            index1 = index;
            index2 = series2.OpenTime.GetIndexByExactTime(MarketSeries.OpenTime[index1]);

            if (trend1IsFalling)
                value1 = -1;

            if (trend1IsRising)
                value1 = 1;

            if (trend2IsFalling)
                value2 = -2;

            if (trend2IsRising)
                value2 = 2;

            Line[index] = value1;
            Histogram[index] = value1;
            HighOrderLine[index] = value2;
        }
    }
}
