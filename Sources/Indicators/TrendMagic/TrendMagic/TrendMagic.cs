// Disclaimer ---------------------------------------------------------------------------------------------------
//	"TrendMagic", translated from MT5 indicator, http://www.mql5.com/en/code/284
// 	"ATR" calculation code adopted from "Average True Range" cAlgo indicator, http://ctdn.com/algos/show/10
//  thanks to qualitiedx2 
// --------------------------------------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class TrendMagic : Indicator
    {
        [Parameter(DefaultValue = 50)]
        public int CCI_Period { get; set; }
        
        [Parameter(DefaultValue = 5)]
        public int ATR_Period { get; set; }
        
        [Output("Main Line", Color = Colors.DarkSlateGray, PlotType = PlotType.Line, Thickness = 1)]
        public IndicatorDataSeries MTrend { get; set; }

        [Output("Down Trend", Color = Colors.Red, PlotType = PlotType.Points,  Thickness = 4)]
        public IndicatorDataSeries DownTrend { get; set; }
			
        [Output("Up Trend", Color = Colors.Blue, PlotType = PlotType.Points, Thickness = 4)]
        public IndicatorDataSeries UpTrend { get; set; }
        
        private IndicatorDataSeries atr;
       	private IndicatorDataSeries tr;
        private CommodityChannelIndex CCI;
        private ExponentialMovingAverage ema;
		private TrueRange tri;
		
        protected override void Initialize()
        {
            // Initialize and create nested indicators
            CCI = Indicators.CommodityChannelIndex(CCI_Period);
            atr = CreateDataSeries();
			tr =  CreateDataSeries();
			tri = Indicators.TrueRange();           
        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index
        	if(index<ATR_Period+1)
        	{atr[index] = tri.Result[index];}
        	if(index>=ATR_Period){
        	atr[index] = (atr[index-1]*(ATR_Period-1)+tri.Result[index]) / ATR_Period;}
        	
        	if(CCI.Result[index]>=0.0)
        	{
        		MTrend[index] = MarketSeries.Low[index]-atr[index];
        		if(MTrend[index]<MTrend[index-1])
        			MTrend[index]=MTrend[index-1];
        		UpTrend[index]=MTrend[index];
        		DownTrend[index]=double.NaN;
				if(double.IsNaN(UpTrend[index-1])) DownTrend[index]=MTrend[index]; 
        	} 
        	else if(CCI.Result[index]<0.0)
        	{
         		MTrend[index]=MarketSeries.High[index]+atr[index];
         		if(MTrend[index]>MTrend[index-1])
         			MTrend[index]=MTrend[index-1];  
        		UpTrend[index]=double.NaN;       
        		DownTrend[index]=MTrend[index];   
				if(double.IsNaN(DownTrend[index-1])) UpTrend[index]=MTrend[index];  
        	}
        }
    }
}