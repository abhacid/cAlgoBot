// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//    Please use the “Feedback” tab to provide us with your suggestions about cAlgo’s API.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class TickVolume : Indicator
    {
    
    	[Output("UpVolume", Color = Colors.Green, PlotType = PlotType.Histogram, Thickness = 1)]
        public IndicatorDataSeries UpVolume { get; set; }              

    	[Output("DownVolume", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 1)]
        public IndicatorDataSeries DownVolume { get; set; }  
        
        protected override void Initialize()
        {
            // Initialize and create nested indicators
        }

        public override void Calculate(int index)
        {
        	 UpVolume[index] = MarketSeries.TickVolume[index];
        	 if(UpVolume[index] < UpVolume[index-1]) DownVolume[index]= UpVolume[index];
        	 else DownVolume[index]= 0;
        	
       }
    }
}