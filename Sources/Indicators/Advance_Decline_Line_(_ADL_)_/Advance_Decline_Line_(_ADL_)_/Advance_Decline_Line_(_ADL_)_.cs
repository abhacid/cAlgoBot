using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class ADL : Indicator
    {
        [Parameter(DefaultValue = 10)]
        public int Period { get; set; }

        [Output("ADL")]
        public IndicatorDataSeries adl { get; set; }

    	int rs;
		int fs;

        public override void Calculate(int index)
        {
        rs=0;
        fs=0;
        if(index<=Period){adl[index]=0;}
        for(int i=0;i<Period;i++)
        {
        	if(MarketSeries.Close[index-i]>MarketSeries.Open[index-i]){rs+=1;}
        	if(MarketSeries.Close[index-i]<MarketSeries.Open[index-i]){fs+=1;}
        }
        if(index>Period){adl[index] = rs - fs + adl[index-1];}
        }
    }
}