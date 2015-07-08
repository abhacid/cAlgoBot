using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class CenterOfGravityOscillator : Indicator	//John Ehler's
    {
        [Output("CoG",Color=Colors.Red)]
        public IndicatorDataSeries cg { get; set; }

		[Output("Lag",Color=Colors.Blue)]
        public IndicatorDataSeries lag { get; set; }
		
        [Parameter(DefaultValue = 10)]
        public int Length { get; set; }

		private IndicatorDataSeries input;
	
		protected override void Initialize()
        {
        	input = CreateDataSeries();
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
            lag[index]=cg[index-1];
        }
    }
}