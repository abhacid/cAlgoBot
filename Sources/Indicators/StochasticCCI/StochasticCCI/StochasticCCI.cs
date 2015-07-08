
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class StochasticCCI : Indicator
    {
        [Output("StochCCI",Color = Colors.Blue)]
        public IndicatorDataSeries StochCCI { get; set; }
        
		[Output("Trigger",Color = Colors.Red)]
        public IndicatorDataSeries trigger { get; set; }
		
		[Parameter(DefaultValue = 14, MinValue = 2)]
        public int Period { get; set; }
		
		private CommodityChannelIndex cci;
		private IndicatorDataSeries value1;
		
		protected override void Initialize()
        {
            cci = Indicators.CommodityChannelIndex(Period);
            value1 = CreateDataSeries();
        }
		
        public override void Calculate(int index)
        {
        	double cciL = cci.Result[index];
        	double cciH = cci.Result[index];
        	for (int i = index - Period + 1; i <= index; i++)
        	{
        		if(cciH<cci.Result[i]){cciH=cci.Result[i];}
        		if(cciL>cci.Result[i]){cciL=cci.Result[i];}
        	}
        	if (cciH != cciL)
        	{
        		value1[index] = ((cci.Result[index] - cciL) / (cciH - cciL));
        	}
        	StochCCI[index] = (4*value1[index] + 3*value1[index-1]+2*value1[index-2] + value1[index-3])/10;
        	StochCCI[index] = 2*(StochCCI[index]-0.5);
        	
        	trigger[index] = 0.96*(StochCCI[index-1]+0.02);
        }
    }
}