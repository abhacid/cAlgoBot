using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class MarketHours : Indicator
    {
// London
        [Parameter(DefaultValue = 10)]
        public double LondonOpen { get; set; }

        [Parameter(DefaultValue = 18)]
        public double LondonClose { get; set; }
// New York        
        [Parameter(DefaultValue = 15)]
        public double NewYorkOpen { get; set; }

        [Parameter(DefaultValue = 24)]
        public double NewYorkClose { get; set; }
// Sydney        
        [Parameter(DefaultValue = 0)]
        public double SydneyOpen { get; set; }

        [Parameter(DefaultValue = 8)]
        public double SydneyClose { get; set; }
// Tokyo        
        [Parameter(DefaultValue = 2)]
        public double TokyoOpen { get; set; }

        [Parameter(DefaultValue = 10)]
        public double TokyoClose { get; set; }

        [Output("London", Color = Colors.White, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries London { get; set; }

        [Output("New York", Color = Colors.Red, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries NewYork { get; set; }

        [Output("Sydney", Color = Colors.Green, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries Sydney { get; set; }

        [Output("Tokyo", Color = Colors.LightBlue, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries Tokyo { get; set; }

        public override void Calculate(int index)
        {
// London        
            if (MarketSeries.OpenTime[index].Hour >= LondonOpen & MarketSeries.OpenTime[index].Hour < LondonClose)
            {
                London[index] = 3;
            }
// New York        
            if (MarketSeries.OpenTime[index].Hour >= NewYorkOpen & MarketSeries.OpenTime[index].Hour < NewYorkClose)
            {
                NewYork[index] = 2;
            }
// Sydney        
            if (MarketSeries.OpenTime[index].Hour >= SydneyOpen & MarketSeries.OpenTime[index].Hour < SydneyClose)
            {
                Sydney[index] = 1;
            }
// Tokyo        
            if (MarketSeries.OpenTime[index].Hour >= TokyoOpen & MarketSeries.OpenTime[index].Hour < TokyoClose)
            {
                Tokyo[index] = 0;
            }

        }

    }
}
