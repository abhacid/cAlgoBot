// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//    Please use the “Feedback” tab to provide us with your suggestions about cAlgo’s API.
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class Centered_Detrend_Price : Indicator
    {
        
        /*Permet de choisir la série sur laquelle sera calculé l'indicateur*/
        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 20)]
        public int Periode { get; set; }
        
        [Parameter(DefaultValue = 20)]
        public int Bands_Periode { get; set; }
        
        [Parameter(DefaultValue = 2.0)]
        public double Bands_Deviation { get; set; }

        [Output("Cycle")]
        public IndicatorDataSeries Cycle { get; set; }
        
        [Output("Cycle Ma", Color = Colors.Purple)]
        public IndicatorDataSeries Cycle_Ma { get; set; }
        
        [Output("Cycle Ma Band Up", Color = Colors.Purple)]
        public IndicatorDataSeries Band_Up { get; set; }
        
        [Output("Cycle Ma Band Down", Color = Colors.Purple)]
        public IndicatorDataSeries Band_Down { get; set; }

        
        
        /////////////////////////////////////
        // Custom Variables                //
        /////////////////////////////////////
        
        private IndicatorDataSeries ABS;
        private MovingAverage Avg_Abs;
        private MovingAverage Avg_Cycle;
        private MovingAverage Avg;
        private int back;
        
        protected override void Initialize()
        {
            // Initialize and create nested indicators
            ABS       = CreateDataSeries();
            Avg       = Indicators.MovingAverage(Source, Periode, MovingAverageType.Simple);
            Avg_Abs   = Indicators.MovingAverage(ABS, Bands_Periode, MovingAverageType.Simple);
            Avg_Cycle = Indicators.MovingAverage(Cycle, Bands_Periode, MovingAverageType.Simple);
            back      = Periode/2;
        }

        public override void Calculate(int index)
        {
            /*Calculate close - centered moving average*/
            
            Cycle[index] = MarketSeries.Close[index] - Avg.Result[index+back];
            
            /*Calculate a moving average of cycle index*/
            
    		Avg_Cycle.Calculate(index);
			Cycle_Ma[index] = Avg_Cycle.Result[index];
			
			/*Calculate the difference of Cycle with her moving average*/
			ABS[index] = Math.Abs(Cycle[index]-Cycle_Ma[index]);
			Avg_Abs.Calculate(index);
			
			
			Band_Up[index]   = Cycle_Ma[index] +(Bands_Deviation*Avg_Abs.Result[index]);
			Band_Down[index] = Cycle_Ma[index] -(Bands_Deviation*Avg_Abs.Result[index]);
            
        }
    }
        
}