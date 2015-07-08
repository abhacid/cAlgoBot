
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class RateOfChange : Indicator
    {
    	[Parameter]
        public DataSeries Source { get; set; }
        
        [Output("Rate Of Change", IsHistogram = true)]
        public IndicatorDataSeries roc { get; set; }
        
        [Output("ROCline")]
        public IndicatorDataSeries rocline { get; set; }
		
		[Output("0", Color= Colors.Gray)]
        public IndicatorDataSeries zero { get; set; }
        
        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        public override void Calculate(int index)
        {
        	zero[index] = 0;
            int barsAgo = Math.Min(index, Period);
            roc[index] = (((Source[index] - Source[index-barsAgo])/ Source[index-barsAgo])*100);
            rocline[index]=roc[index];
        }
    }
}