// -------------------------------------------------------------------------------
//
//    Accelerator Decelerator Oscilator Indicator
//    
//    AO - SMA(AO, 5)
//    AO = SMA(Source, 5) - SMA(Source, 34)
//	  AO: Awesome Oscilator
//    SMA: Simple Moving Average
// -------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator("AC", IsOverlay = false, AccessRights = AccessRights.None)]
    public class AccelerationDecelerationOscillator : Indicator
    {
        [Parameter]
        public DataSeries Source { get; set; }

        [Output("Buy", Color = Colors.Green, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries ExtBuffer1 { get; set; }

        [Output("Sell", Color = Colors.Red, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries ExtBuffer2 { get; set; }


        private SimpleMovingAverage _movingAverage5;
        private SimpleMovingAverage _movingAverage34;
        private IndicatorDataSeries _awesomeOsc;			
        private SimpleMovingAverage _movingAverage;
        private IndicatorDataSeries _extBuffer0;

        protected override void Initialize()
        {
            _movingAverage5 = Indicators.SimpleMovingAverage(Source, 5);
            _movingAverage34 = Indicators.SimpleMovingAverage(Source, 34);
            _awesomeOsc = CreateDataSeries();
            _movingAverage = Indicators.SimpleMovingAverage(_awesomeOsc, 5); 
            _extBuffer0 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            _awesomeOsc[index] = _movingAverage5.Result[index] - _movingAverage34.Result[index];

            if (index < 1)
                return;

            bool up = true;
            
            double prev = _awesomeOsc[index] - _movingAverage.Result[index];
            double current = _awesomeOsc[index - 1] - _movingAverage.Result[index - 1];

            if (current < prev) 
            {
                up = false;
            }
            if (!up) 
            {
                ExtBuffer2[index] = current; 	
                ExtBuffer1[index] = 0.0;	 	
            }
            else
            {
                ExtBuffer1[index] = current;	
                ExtBuffer2[index] = 0.0;		
            }

            _extBuffer0[index] = current;
        }
    }
}