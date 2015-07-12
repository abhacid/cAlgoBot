// -------------------------------------------------------------------------------
//							Average True Range (ATR)
// based on indicator 'True Range' and overlayed by simple Moving Average (SMA)
// -------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator("Average True Range", IsOverlay = false, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class AverageTrueRange : Indicator
    {

        [Parameter(DefaultValue = 14)]
        public int Periods { get; set; }

        [Output("Average True Range", Color = Colors.Blue)]
        public IndicatorDataSeries Result { get; set; }

        private IndicatorDataSeries tr;
        private TrueRange tri;
        private MovingAverage TRMA;

        protected override void Initialize()
        {
            // Initialize and create nested indicators
            tr = CreateDataSeries();
            tri = Indicators.TrueRange();
            TRMA = Indicators.MovingAverage(tr, Periods, MovingAverageType.Simple);

        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index            
            tr[index] = tri.Result[index];
            Result[index] = TRMA.Result[index];

        }
    }
}
