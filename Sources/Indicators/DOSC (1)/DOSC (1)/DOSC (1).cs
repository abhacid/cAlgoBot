using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class DOSC : Indicator
    {
        [Parameter(DefaultValue = 7)]
        public int SlowSMA { get; set; }
        
        [Parameter(DefaultValue = 1)]
        public int FastSMA { get; set; }
        
		[Parameter("Data Source")]
        public DataSeries DataSource { get; set; }
        
        [Output("ZeroLine", Color=Colors.White)]
        public IndicatorDataSeries zeroline { get; set; }
        
        [Output("DOSC", Color=Colors.SkyBlue)]
        public IndicatorDataSeries dosc { get; set; }
        
		private SimpleMovingAverage imafast;
		private SimpleMovingAverage imaslow;
		
        protected override void Initialize()
        {
          	imafast = Indicators.SimpleMovingAverage(DataSource,FastSMA);
          	imaslow = Indicators.SimpleMovingAverage(DataSource,SlowSMA);
        }

        public override void Calculate(int index)
        {
			dosc[index] = 10000* (imafast.Result[index] - imaslow.Result[index]);
			zeroline[index] = 0;
        }
    }
}