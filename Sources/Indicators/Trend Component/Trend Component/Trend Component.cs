using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class TrendComponent : Indicator
    {
        [Parameter(DefaultValue = 11)]
        public int Period { get; set; }
        
        [Parameter(DefaultValue = 0.05,MaxValue = 0.1)]
        public double Delta { get; set; }
        
        [Output("Zeroline", Color = Colors.White)]
        public IndicatorDataSeries zeroline { get; set; }
        [Output("Trend", Color = Colors.Blue)]
        public IndicatorDataSeries Trend { get; set; }
		
		private double avgbp;

		public IndicatorDataSeries Price;
		public IndicatorDataSeries bp;
        protected override void Initialize()
        {
            Price = CreateDataSeries();
            bp = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
			double beta = Math.Cos(360/Period);
			double gamma = (1 / Math.Cos((720*Delta)/Period));
			double alpha = gamma * Math.Sqrt((gamma*gamma)-1);   		
        	bp[1]=0;
       		bp[2]=0;
			Price[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) /2;
			bp[index] = 0.5*(1-alpha)*(Price[index]-Price[index-2]) + 
			beta*(1+alpha)*bp[index-1]-alpha*bp[index-2];
			zeroline[index] = 0;
			if(index>=2*Period)
			{
			avgbp=0;
			for(int i=0;i<2*Period;i++)
			{
				avgbp+=bp[index-i];
			}
			avgbp=avgbp/(2*Period);
			Trend[index] = avgbp;
			}
        }
    }
}