using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class LaguerreFilter : Indicator
    {
        [Parameter(DefaultValue = 0.5)]
        public double gamma { get; set; }

        [Output("Laguerre Filter" , Color = Colors.Yellow)]
        public IndicatorDataSeries Filt { get; set; }
        
        [Output("FIR", Color = Colors.Blue)]
        public IndicatorDataSeries Fir { get; set; }

		private IndicatorDataSeries price;
		private IndicatorDataSeries L0;
		private IndicatorDataSeries L1;
		private IndicatorDataSeries L2;
		private IndicatorDataSeries L3;

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
            price[index] = (MarketSeries.High[index]+MarketSeries.Low[index])/2;
            if(index<=3)
            {
            	L0[index] = (1-gamma)*price[index];
            	L1[index] = -gamma*L0[index] + L0[index-1];
            	L2[index] = -gamma*L1[index] + L1[index-1];
            	L3[index] = -gamma*L2[index] + L2[index-1];
            }
            if(index>3)
            {
            	L0[index] = (1-gamma)*price[index] + gamma*L0[index-1];
            	L1[index] = -gamma*L0[index] + L0[index-1] + gamma*L1[index-1];
            	L2[index] = -gamma*L1[index] + L1[index-1] + gamma*L2[index-1];
            	L3[index] = -gamma*L2[index] + L2[index-1] + gamma*L3[index-1];
            }
            Filt[index] = (L0[index] + 2*L1[index] + 2*L2[index] + L3[index] ) /6;
            Fir[index] = (price[index] + 2*price[index-1] + 2*price[index-2] + price[index-3])/6;
        }
    }
}