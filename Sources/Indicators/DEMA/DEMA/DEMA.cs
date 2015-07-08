
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class DEMA : Indicator
    {
    	[Parameter]
        public DataSeries DataSource { get; set; }
    	
        [Output("DEMA",Color = Colors.Yellow)]
        public IndicatorDataSeries dema { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

		private ExponentialMovingAverage ema;
		private ExponentialMovingAverage ema2;
		
		protected override void Initialize()
        {
            ema = Indicators.ExponentialMovingAverage(DataSource,Period);//i need the correct DataSeries
            ema2 = Indicators.ExponentialMovingAverage(ema.Result,Period);
        }
		
        public override void Calculate(int index)
        {
	       dema[index] = (2* ema.Result[index] - ema2.Result[index]);
        }
    }
}