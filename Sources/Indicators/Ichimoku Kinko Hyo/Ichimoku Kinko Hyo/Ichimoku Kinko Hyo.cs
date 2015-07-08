using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class IchimokuKinkoHyo : Indicator
    {
        [Parameter(DefaultValue = 9)]
        public int periodFast { get; set; }

        [Parameter(DefaultValue = 26)]
        public int periodMedium { get; set; }

        [Parameter(DefaultValue = 52)]
        public int periodSlow { get; set; }

        [Parameter(DefaultValue = 26)]
        public int DisplacementChikou { get; set; }
        
        [Parameter(DefaultValue = 26)]
        public int DisplacementCloud { get; set; }

        [Output("TenkanSen", Color = Colors.Red)]
        public IndicatorDataSeries TenkanSen { get; set; }
        [Output("Kijunsen", Color = Colors.Blue)]
        public IndicatorDataSeries KijunSen { get; set; }
        [Output("ChikouSpan", Color = Colors.DarkViolet)]
        public IndicatorDataSeries ChikouSpan { get; set; }

        [Output("SenkouSpanB", Color = Colors.Red, LineStyle=LineStyle.Lines)]
        public IndicatorDataSeries SenkouSpanB { get; set; }
        [Output("SenkouSpanA", Color = Colors.Green,LineStyle=LineStyle.Lines)]
        public IndicatorDataSeries SenkouSpanA { get; set; }

    	double maxfast,minfast,maxmedium,minmedium,maxslow,minslow;

        public override void Calculate(int index)
        {
			if((index<periodFast)||(index<periodSlow)){return;}
			
			maxfast = MarketSeries.High[index];
			minfast = MarketSeries.Low[index];
			maxmedium = MarketSeries.High[index];
			minmedium = MarketSeries.Low[index];
			maxslow = MarketSeries.High[index];
			minslow = MarketSeries.Low[index];
			
			for(int i=0;i<periodFast;i++)
			{
				if(maxfast< MarketSeries.High[index-i]){maxfast = MarketSeries.High[index-i];}
				if(minfast> MarketSeries.Low[index-i]){minfast = MarketSeries.Low[index-i];}
			}
			for(int i=0;i<periodMedium;i++)
			{
				if(maxmedium< MarketSeries.High[index-i]){maxmedium = MarketSeries.High[index-i];}
				if(minmedium> MarketSeries.Low[index-i]){minmedium = MarketSeries.Low[index-i];}
			}
			for(int i=0;i<periodSlow;i++)
			{
				if(maxslow< MarketSeries.High[index-i]){maxslow = MarketSeries.High[index-i];}
				if(minslow> MarketSeries.Low[index-i]){minslow = MarketSeries.Low[index-i];}
			}
			TenkanSen[index] = (maxfast + minfast) /2;
			KijunSen[index] = (maxmedium + minmedium) /2;
			ChikouSpan[index-DisplacementChikou] = MarketSeries.Close[index];
			
			SenkouSpanA[index+DisplacementCloud] = (TenkanSen[index] + KijunSen[index]) / 2;
			SenkouSpanB[index+DisplacementCloud] = (maxslow + minslow) / 2; 
        }
    }
}