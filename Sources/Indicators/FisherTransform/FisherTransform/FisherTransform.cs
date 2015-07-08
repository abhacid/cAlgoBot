
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class FisherTransform : Indicator
    {       
        [Output("FisherTransform", Color = Colors.Orange)]
        public IndicatorDataSeries Fish { get; set; }
    	
    	[Output("Trigger", Color = Colors.Green)]
        public IndicatorDataSeries trigger { get; set; }
        
        [Parameter(DefaultValue = 13, MinValue = 2)]
        public int Len { get; set; }
		
		double MaxH;
		double MinL;
		int tr = 0;
		private IndicatorDataSeries price;
		private IndicatorDataSeries Value1;
		
		protected override void Initialize()
        {
        	price = CreateDataSeries();
        	Value1 = CreateDataSeries();
		}
        public override void Calculate(int index)
        {
        	if(index <= Len+1)
        	{
        	Value1[index-1]=1;
        	Fish[index-1]=0;
        	}

			price[index] = (MarketSeries.High[index]+MarketSeries.Low[index])/2;
			
			MaxH=price[index];
			MinL=price[index];
			for(int i=index; i > index - Len; i--)
			{
				MinL = Math.Min(MinL,price[i]);
				MaxH = Math.Max(MaxH,price[i]);
			}
			if(Value1[index] > 0.9999){Value1[index] = 0.9999;}
			if(Value1[index] < -0.9999) {Value1[index] = -0.9999;}
			
			Value1[index] = 0.5*2*((price[index]-MinL)/(MaxH-MinL)-0.5)+0.5*Value1[index-1];
			
			Fish[index] = 0.25*Math.Log((1+Value1[index])/(1-Value1[index])) +0.5*Fish[index-1];
			trigger[index] = Fish[index-1];
        }
    }
}