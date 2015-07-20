// -------------------------------------------------------------------------------
//							Average True Range (ATR)
// based on indicator 'True Range' and overlayed by simple Moving Average (SMA)
// -------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator("Average True Range", IsOverlay = false, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class ATRandTR : Indicator
    {

        [Parameter(DefaultValue = 14)]
        public int Periods { get; set; }

        [Output("Average True Range", Color = Colors.Blue)]
        public IndicatorDataSeries Result { get; set; }

        [Output("True Range", Color = Colors.White)]
        public IndicatorDataSeries TrResult { get; set; }

        //private IndicatorDataSeries tr;
        private TrueRange tri;
        private MovingAverage TRMA;

        protected override void Initialize()
        {
            // Initialize and create nested indicators
            // tr = CreateDataSeries();
            tri = Indicators.TrueRange();
            TRMA = Indicators.MovingAverage(TrResult, Periods, MovingAverageType.Simple);

        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index            
            TrResult[index] = tri.Result[index];
            Result[index] = TRMA.Result[index];

        }
    }
}
