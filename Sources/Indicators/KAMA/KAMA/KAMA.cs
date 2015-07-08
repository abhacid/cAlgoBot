using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class KAMA : Indicator
    {
        [Parameter]
        public DataSeries Source { get; set; }
        
        [Output("KAMA")]
        public IndicatorDataSeries kama { get; set; }

        [Parameter(DefaultValue = 2)]
        public int Fast { get; set; }
		
        [Parameter(DefaultValue = 30)]
        public int Slow { get; set; }
		
        [Parameter(DefaultValue = 10)]
        public int Period { get; set; }
		
		private IndicatorDataSeries diff;
		double sum;
		protected override void Initialize()
        {
            diff = CreateDataSeries();
        }
		
        public override void Calculate(int index)
        {
			if(index>0)
			{
				diff[index] = Math.Abs(Source[index] - Source [index-1]);
			}
			if(index<Period)
			{
				kama[index] = Source[index];
				return;
			}
			double fastd = 2.0 / (double)(Fast + 1);
			double slowd = 2.0 / (double)(Slow + 1);
			
			double signal = Math.Abs(Source[index] - Source[index-Period]);
			sum=0;
			for(int i = 0; i<Period;i++)
			{
				sum+= diff[index-i];
			}
			double noise  = sum;
			if (noise == 0) 
			{
				kama[index] = kama[index-1];
				return;
			}
				double smooth = Math.Pow((signal / noise) * (fastd - slowd) + slowd, 2);
			
			kama[index] = kama[index-1] + smooth * (Source[index]-kama[index-1]);
        }
    }
}