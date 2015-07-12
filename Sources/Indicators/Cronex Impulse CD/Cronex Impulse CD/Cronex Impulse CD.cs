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
    public class CronexImpulseCD : Indicator
    {
        [Parameter("FastMA", DefaultValue = 14)]
        public int FastMA { get; set; }

        [Parameter("SlowMA", DefaultValue = 34)]
        public int SlowMA { get; set; }

        [Parameter("SignalMA", DefaultValue = 9)]
        public int SignalMA { get; set; }

        [Output("MacdDivrBuffer", Color = Colors.Red, IsHistogram = true)]
        public IndicatorDataSeries MacdDivrBuffer { get; set; }

        [Output("CDDivrBuffer", Color = Colors.Blue, IsHistogram = true)]
        public IndicatorDataSeries CDDivrBuffer { get; set; }

        private MovingAverage SignalBuffer;
        private MovingAverage HiInd, LoInd, MasterInd;
        private MedianPrice Median;

        protected override void Initialize()
        {
            Median = Indicators.MedianPrice();
            HiInd = Indicators.MovingAverage(MarketSeries.High, SlowMA, MovingAverageType.Simple);
            LoInd = Indicators.MovingAverage(MarketSeries.Low, SlowMA, MovingAverageType.Simple);
            MasterInd = Indicators.MovingAverage(Median.Result, FastMA, MovingAverageType.Exponential);
            SignalBuffer = Indicators.MovingAverage(MacdDivrBuffer, SignalMA, MovingAverageType.Simple);
        }

        public override void Calculate(int index)
        {
            if (MasterInd.Result[index] > HiInd.Result[index])
                MacdDivrBuffer[index] = MasterInd.Result[index] - HiInd.Result[index];

            if (MasterInd.Result[index] < LoInd.Result[index])
                MacdDivrBuffer[index] = MasterInd.Result[index] - LoInd.Result[index];

            CDDivrBuffer[index] = MacdDivrBuffer[index] - SignalBuffer.Result[index];
        }
    }
}
