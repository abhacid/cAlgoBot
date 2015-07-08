using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class ITrend : Indicator
    {
        [Output("ITrend",Color=Colors.Red)]
        public IndicatorDataSeries itrend { get; set; }
        
        [Output("Lag",Color=Colors.DodgerBlue)]
        public IndicatorDataSeries lag { get; set; }

        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }
		
		private IndicatorDataSeries price;
		
		protected override void Initialize()
        {
        	price = CreateDataSeries();
		}

        public override void Calculate(int index)
        {
        	price[index] = (MarketSeries.High[index]+MarketSeries.Low[index])/2;
            if(index<3)
            {
            	itrend[index]=price[index];
           	}
           	else
           	{
           		if(index<7)
           		{
           			itrend[index]=((price[index]+
           			2*price[index-1]+price[index-2])/4);
           		}
           		else
           		{
           			itrend[index]=(Alpha - Alpha *Alpha/4)*price[index]
           			+0.5*Alpha*Alpha*price[index-1]-
           			(Alpha-0.75*Alpha*Alpha)*price[index-2]+2
           			*(1-Alpha )*itrend[index-1]-(1-Alpha )
           			*(1-Alpha )*itrend[index-2];
           		}
           		lag[index]=2*itrend[index]-itrend[index-2];
           	}
        }
    }
}