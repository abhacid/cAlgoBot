using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class ADXR : Indicator
    {
        [Parameter]
        public DataSeries Source { get; set; }
        
        [Parameter(DefaultValue = 14)]
        public int interval { get; set; }

        [Output("ADXR", Color= Colors.Turquoise)]
        public IndicatorDataSeries adxr { get; set; }
		
        [Output("DiMinus", Color= Colors.Red)]
        public IndicatorDataSeries diminus { get; set; }
		
		[Output("DiPlus", Color= Colors.Blue)]
        public IndicatorDataSeries diplus { get; set; }

		private DirectionalMovementSystem adx;
        protected override void Initialize()
        {
            adx = Indicators.DirectionalMovementSystem(14);
        }

        public override void Calculate(int index)
        {
			adxr[index] = (adx.ADX[index] + adx.ADX[index-interval])/2;
			diminus[index] = adx.DIMinus[index];
			diplus[index] = adx.DIPlus[index];
        }
    }
}