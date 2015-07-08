using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class Laguerre_RSI : Indicator
    {
        [Parameter(DefaultValue = 0.2)]
        public double gamma { get; set; }

        [Output("Laguerre RSI",Color = Colors.Yellow)]
        public IndicatorDataSeries laguerrersi { get; set; }
        
        [Output("Overbought",Color = Colors.Turquoise)]
        public IndicatorDataSeries overbought { get; set; }
        
        [Output("oversold",Color = Colors.Red)]
        public IndicatorDataSeries oversold { get; set; }
        
    	private IndicatorDataSeries price;
		private IndicatorDataSeries L0;
		private IndicatorDataSeries L1;
		private IndicatorDataSeries L2;
		private IndicatorDataSeries L3;
		
		double cu;
		double cd;
		
        protected override void Initialize()
        {
        	price = CreateDataSeries();
            L0 = CreateDataSeries();
            L1 = CreateDataSeries();
            L2 = CreateDataSeries();
            L3 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
        	overbought[index] = 0.8;
        	oversold[index] = 0.2;
        	price[index] = (MarketSeries.High[index]+MarketSeries.Low[index])/2;
            if(index<=6)
            {
            	L0[index] = (1-gamma)*price[index];
            	L1[index] = -gamma*L0[index] + L0[index-1];
            	L2[index] = -gamma*L1[index] + L1[index-1];
            	L3[index] = -gamma*L2[index] + L2[index-1];
            }
            if(index>6)
            {
            	L0[index] = (1-gamma)*price[index] + gamma*L0[index-1];
            	L1[index] = -gamma*L0[index] + L0[index-1] + gamma*L1[index-1];
            	L2[index] = -gamma*L1[index] + L1[index-1] + gamma*L2[index-1];
            	L3[index] = -gamma*L2[index] + L2[index-1] + gamma*L3[index-1];
            }
           // laguerrersi[index] = L0[index];
            cu=0;
            cd=0;
            if(L0[index]>=L1[index])
            {
            	cu = L0[index]-L1[index];
            }
            else
            {
            	cd = L1[index] - L0[index];
            }
            if(L1[index]>=L2[index])
            {
            	cu = cu+ L1[index]-L2[index];
            }
            else
            {
            	cd = cd + L2[index] - L1[index];
            }
            if(L2[index]>=L3[index])
            {
            	cu = cu + L2[index] - L3[index];
            }
            else
            {
            	cd = cd + L3[index] - L2[index];
            }
            if(cu+cd!=0)
            {
            	laguerrersi[index] = cu / (cu+cd);
            }
        }
    }
}