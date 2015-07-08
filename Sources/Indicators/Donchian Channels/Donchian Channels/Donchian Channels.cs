// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//    Please use the “Feedback” tab to provide us with your suggestions about cAlgo’s API.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class DonchianChannels : Indicator
    {
        [Parameter(DefaultValue = 12)]
        public int Periods { get; set; }
        
        [Parameter(DefaultValue = 3)]
        public int Extremes { get; set; }
        
        [Parameter(DefaultValue = -2, MinValue = -2)]
        public int Margins { get; set; }

        [Output("Upper Line", Color = Colors.Blue, PlotType = PlotType.Line, LineStyle = LineStyle.Solid, Thickness = 2)]
        public IndicatorDataSeries UpperSeries { get; set; }
        
        [Output("Lower Line", Color = Colors.Red, PlotType = PlotType.Line, LineStyle = LineStyle.Solid, Thickness = 2)]
        public IndicatorDataSeries LowerSeries { get; set; }
        
        [Output("Middle Line", Color = Colors.DarkOrange, PlotType = PlotType.Line, LineStyle = LineStyle.DotsRare, Thickness = 2)]
        public IndicatorDataSeries MiddSeries { get; set; }
				
		
        protected override void Initialize()
        {
            // Initialize and create nested indicators
         
        }

		private double Highest(DataSeries arr,int range,int fromIndex)
		{
			double res;
			int i;
			res=arr[fromIndex];
			for(i=fromIndex;i>fromIndex-range && i>=0;i--)
			{
				if(res<arr[i]) res=arr[i];
			}
			return(res);
		}

		private double Lowest(DataSeries arr,int range,int fromIndex)
		{
			double res;
			int i;
			res=arr[fromIndex];
			for(i=fromIndex;i>fromIndex-range && i>=0;i--)
			{
				if(res>arr[i]) res=arr[i];
			}
			return(res);
		}

        public override void Calculate(int index)
        {
            // Calculate value at specified index
            // Result[index] = ...
            
			double smin=0,smax=0,SsMax=0,SsMin=0;
						
            for (int i = index - Periods + 1; i <= index; i++)
            {
				if(Extremes==1)
				{
					SsMax = Highest(MarketSeries.High,Periods,i);
					SsMin = Lowest(MarketSeries.Low,Periods,i);
				}
				else if(Extremes==3)
				{
					SsMax = (Highest(MarketSeries.Open,Periods,i)+Highest(MarketSeries.High,Periods,i))/2;
					SsMin = (Lowest(MarketSeries.Open,Periods,i)+Lowest(MarketSeries.Low,Periods,i))/2;
				}
				else
				{
					SsMax = Highest(MarketSeries.Open,Periods,i);
					SsMin = Lowest(MarketSeries.Open,Periods,i);
				}  
			      smin = SsMin+(SsMax-SsMin)*Margins/100;
			      smax = SsMax-(SsMax-SsMin)*Margins/100;
			      UpperSeries[i]=smax;
			      LowerSeries[i]=smin;
			      MiddSeries[i]=(UpperSeries[i]+LowerSeries[i])/2.0;
                
            }
            
        }
        
    }
}