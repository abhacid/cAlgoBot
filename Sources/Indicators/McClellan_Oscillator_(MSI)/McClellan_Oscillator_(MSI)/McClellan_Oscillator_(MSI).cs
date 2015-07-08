using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class MSI : Indicator
    {
        [Output("MSIupper", Color = Colors.Blue, IsHistogram = true)]
        public IndicatorDataSeries MSIup { get; set; }
        
        [Output("MSIupperLine", Color = Colors.Blue)]
        public IndicatorDataSeries MSIupline { get; set; }
        
        [Output("MSILower", Color = Colors.Red, IsHistogram = true)]
        public IndicatorDataSeries MSIlow { get; set; }
        
        [Output("MSILowerLine", Color = Colors.Red)]
        public IndicatorDataSeries MSIlowline { get; set; }

        [Parameter(DefaultValue = 39)]
        public int SlowPeriod { get; set; }

        [Parameter(DefaultValue = 19)]
        public int FastPeriod { get; set; }
        
        private ExponentialMovingAverage ema1;
        private ExponentialMovingAverage ema2;
        
		protected override void Initialize()
        {
            ema1 = Indicators.ExponentialMovingAverage(MarketSeries.Close,SlowPeriod);
            ema2 = Indicators.ExponentialMovingAverage(MarketSeries.Close,FastPeriod);
        }
        
        public override void Calculate(int index)
        {
			double McClellanUpper = Math.Max(((ema2.Result[index]) - (ema1.Result[index])), 0);
     		double McClellanLower = Math.Min(((ema2.Result[index]) - (ema1.Result[index])), 0);
     		
     		MSIup[index] = McClellanUpper;
     		MSIlow[index] = McClellanLower;
     		MSIupline[index]=McClellanUpper;
     		MSIlowline[index] = McClellanLower;
        }
    }
}