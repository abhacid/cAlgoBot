using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using System.Collections.Specialized;
using System.Net;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class BelkhayateTiming : Indicator
    {

        private int LastAlertBar;

        [Output("High", Color = Colors.Gray, PlotType = PlotType.Points)]
        public IndicatorDataSeries High { get; set; }
        [Output("Low", Color = Colors.Gray, PlotType = PlotType.Points)]
        public IndicatorDataSeries Low { get; set; }
        [Output("Open", Color = Colors.Gray, PlotType = PlotType.Points)]
        public IndicatorDataSeries Open { get; set; }
        [Output("Close", Color = Colors.Gray, PlotType = PlotType.Points)]
        public IndicatorDataSeries Close { get; set; }

        [Output("SellLine1", Color = Colors.DimGray, PlotType = PlotType.Line, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries SellLine1 { get; set; }
        [Output("SellLine2", Color = Colors.DimGray, PlotType = PlotType.Line, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries SellLine2 { get; set; }
        [Output("BuyLine1", Color = Colors.DimGray, PlotType = PlotType.Line, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries BuyLine1 { get; set; }
        [Output("BuyLine2", Color = Colors.DimGray, PlotType = PlotType.Line, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries BuyLine2 { get; set; }

        protected override void Initialize()
        {

        }

        public override void Calculate(int index)
        {
            double middle = (((MarketSeries.High[index] + MarketSeries.Low[index]) / 2) + ((MarketSeries.High[index - 1] + MarketSeries.Low[index - 1]) / 2) + ((MarketSeries.High[index - 2] + MarketSeries.Low[index - 2]) / 2) + ((MarketSeries.High[index - 3] + MarketSeries.Low[index - 3]) / 2) + ((MarketSeries.High[index - 4] + MarketSeries.Low[index - 4]) / 2)) / 5;
            double scale = (((MarketSeries.High[index] - MarketSeries.Low[index]) + (MarketSeries.High[index - 1] - MarketSeries.Low[index - 1]) + (MarketSeries.High[index - 2] - MarketSeries.Low[index - 2]) + (MarketSeries.High[index - 3] - MarketSeries.Low[index - 3]) + (MarketSeries.High[index - 4] - MarketSeries.Low[index - 4])) / 5) * 0.2;

            High[index] = (MarketSeries.High[index] - middle) / scale;
            Low[index] = (MarketSeries.Low[index] - middle) / scale;
            Open[index] = (MarketSeries.Open[index] - middle) / scale;
            Close[index] = (MarketSeries.Close[index] - middle) / scale;


            if (Open[index] > Close[index])
            {
                ChartObjects.DrawLine(string.Format("CRS_High_{0}", index), index, High[index], index, Low[index], Colors.Gray, 1, LineStyle.Solid);
                ChartObjects.DrawLine(string.Format("CRS_Open_{0}", index), index, Open[index], index, Close[index], Colors.DimGray, 11, LineStyle.Solid);
            }
            else
            {
                ChartObjects.DrawLine(string.Format("CRS_Low_{0}", index), index, High[index], index, Low[index], Colors.Gray, 1, LineStyle.Solid);
                ChartObjects.DrawLine(string.Format("CRS_Close_{0}", index), index, Open[index], index, Close[index], Colors.LightGray, 11, LineStyle.Solid);
            }

            SellLine1[index] = 6;
            SellLine2[index] = 8;
            BuyLine1[index] = -6;
            BuyLine2[index] = -8;

        }

    }
}

