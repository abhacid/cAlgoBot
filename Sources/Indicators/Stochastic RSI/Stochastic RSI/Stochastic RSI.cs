
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class StochasticRSI : Indicator
    {
        [Output("StochRSI",Color = Colors.Yellow)]
        public IndicatorDataSeries StochRSI { get; set; }
		
		[Output("Overbought",Color = Colors.Turquoise)]
        public IndicatorDataSeries overbought { get; set; }
        
		[Output("Oversold",Color = Colors.Red)]
        public IndicatorDataSeries oversold { get; set; }
		
		[Parameter(DefaultValue = 14, MinValue = 2)]
        public int Period { get; set; }
		
		private RelativeStrengthIndex rsi;
		
		protected override void Initialize()
        {
            rsi = Indicators.RelativeStrengthIndex(MarketSeries.Close,Period);
        }
		
        public override void Calculate(int index)
        {
        	overbought[index] = 0.8;
        	oversold[index] = 0.2;
        	double rsiL = rsi.Result[index];
        	double rsiH = rsi.Result[index];
        	for (int i = index - Period + 1; i <= index; i++)
        	{
        		if(rsiH<rsi.Result[i]){rsiH=rsi.Result[i];}
        		if(rsiL>rsi.Result[i]){rsiL=rsi.Result[i];}
        	}
        	if (rsi.Result[index] != rsiL && rsiH != rsiL)
        	{
        		StochRSI[index] = ((rsi.Result[index] - rsiL) / (rsiH - rsiL));
        	}
        	else
        	{
        		StochRSI[index] = 0;
        	}
        }
    }
}