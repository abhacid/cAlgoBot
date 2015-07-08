using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Collections;
using System.Linq;

namespace cAlgo.Indicators
{
    [Indicator(ScalePrecision=5, IsOverlay = false, AccessRights = AccessRights.None)]
    public class TickChart : Indicator
    {        
        [Output("Ask", Color=Colors.Blue)]
        public IndicatorDataSeries Ask { get; set; }
		
        [Output("Bid", Color=Colors.Red)]
        public IndicatorDataSeries Bid { get; set; }
        
        private MarketDepth _marketDepth;
               
        private static void ShiftDataSeries(IndicatorDataSeries dataSeries)
        {
        	for (var i = 0; i < dataSeries.Count - 1; i++)
        	{
        		dataSeries[i] = dataSeries[i + 1];
        	}
        }
        
        private static void FillDataSeries(IndicatorDataSeries dataSeries, double value, int startIndex, int count)
        {
        	for (var i = startIndex; i < startIndex + count; i++)
        		dataSeries[i] = value;
        }
        
        public override void Calculate(int index)
        {
        	if (!IsRealTime)
        		return;
        		
        	if (!double.IsNaN(Ask[index]))
        	{
        		ShiftDataSeries(Ask);
        		ShiftDataSeries(Bid);
        	}
        	
        	FillDataSeries(Ask, Symbol.Ask, index, 50);
        	FillDataSeries(Bid, Symbol.Bid, index, 50);
        	
        	var spread = Math.Round((Symbol.Ask - Symbol.Bid) / Symbol.PipSize, 1);
        	ChartObjects.DrawText("Spread label", "Spread:\t" + spread + " pips", StaticPosition.BottomRight);
        }
    }
}