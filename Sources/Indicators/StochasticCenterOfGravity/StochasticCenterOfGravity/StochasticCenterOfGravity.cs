using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class StochasticCenterOfGravity : Indicator
    {
        [Output("StochCoG",Color=Colors.Red)]
        public IndicatorDataSeries stochcog { get; set; }
        
		[Output("Trigger",Color = Colors.Blue)]
        public IndicatorDataSeries trigger { get; set; }
 
        [Parameter(DefaultValue = 10)]
        public int Length { get; set; }

		private IndicatorDataSeries input;
		private IndicatorDataSeries cg;
		private IndicatorDataSeries value1;
		
		protected override void Initialize()
        {
        	input = CreateDataSeries();
        	cg= CreateDataSeries();
        	value1 = CreateDataSeries();
		}
		
        public override void Calculate(int index)
        {
            if(index<Length+1)
            {
            	return;
            }
            input[index] = (MarketSeries.High[index]+MarketSeries.Low[index])/2;
           	double Num=0;
            double Denom=0;
            for(int i=0;i<Length;i++)
            {
            	Num += (1+i)*input[index-i];
            	Denom += input[index-i];
            }
            if(Denom!=0)
            {
            	cg[index]= -Num/Denom + (Length+1)/2;
            }
        	double scogH = cg[index];
        	double scogL = cg[index];
        	for (int i = index - Length + 1; i <= index; i++)
        	{
        		if(scogH<cg[i]){scogH=cg[i];}
        		if(scogL>cg[i]){scogL=cg[i];}
        	}
        	if (scogH != scogL)
        	{
        		value1[index] = ((cg[index] - scogL) / (scogH - scogL));
        	}
        	stochcog[index] = (4*value1[index] + 3*value1[index-1]+2*value1[index-2] + value1[index-3])/10;
        	stochcog[index] = 2*(stochcog[index]-0.5);
        	
        	trigger[index] = 0.96*(stochcog[index-1]+0.02);
        }
    }
}