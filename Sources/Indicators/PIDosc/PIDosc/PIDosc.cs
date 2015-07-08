using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class PIDosc : Indicator
    {
        [Parameter]
        public DataSeries Source { get; set; }
    
        [Parameter("RSI Periods", DefaultValue = 14)]
        public int rsiperiods { get; set; }

        [Parameter("SMA Periods", DefaultValue = 200)]
        public int smaperiods { get; set; }

        [Output("Main", Color = Colors.Blue)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Bottom", Color = Colors.Green)]
        public IndicatorDataSeries bottom { get; set; }

        [Output("Top", Color = Colors.Red)]
        public IndicatorDataSeries top { get; set; }

		double value1;
		
		RelativeStrengthIndex rsi;
		SimpleMovingAverage sma;
		
        protected override void Initialize()
        {
			rsi = Indicators.RelativeStrengthIndex(Source, rsiperiods);
			sma = Indicators.SimpleMovingAverage(Source, smaperiods);
        }

        public override void Calculate(int index)
        {
        	if(MarketSeries.Close[index] > sma.Result[index]){
				Result[index] = (rsi.Result[index] - 35) / (85 - 35) * 100;
			}else{
				Result[index] = (rsi.Result[index] - 20) / (70 - 20) * 100;
			}
			top[index] = 100;
			bottom[index] = 0;
        }
    }
}