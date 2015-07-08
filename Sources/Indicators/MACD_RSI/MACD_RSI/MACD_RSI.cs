// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Indicator. 
//
// -------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class MACD_RSI : Indicator
    {
        private MacdCrossOver macdCrossOver;
        private RelativeStrengthIndex rsi;


        [Parameter()]
        public DataSeries source { get; set; }

        [Parameter(DefaultValue = 9)]
        public int Periods { get; set; }

        [Parameter(DefaultValue = 12)]
        public int shortCycle { get; set; }

        [Parameter(DefaultValue = 26)]
        public int longCycle { get; set; }


        [Output("MACD", Color = Colors.Blue)]
        public IndicatorDataSeries MACD { get; set; }

        [Output("MACD_Signal", Color = Colors.Red)]
        public IndicatorDataSeries MACD_Signal { get; set; }

        [Output("MACD_Histogram", PlotType = PlotType.Histogram, Color = Colors.LightSkyBlue)]
        public IndicatorDataSeries MACD_Histogram { get; set; }

        [Output("RSI")]
        public IndicatorDataSeries RSI { get; set; }


        protected override void Initialize()
        {
            // Initialize and create nested indicators
            macdCrossOver = Indicators.MacdCrossOver(source, longCycle, shortCycle, Periods);
            rsi = Indicators.RelativeStrengthIndex(source, Periods);
        }


        public override void Calculate(int index)
        {
            const int factor = 100000;
            MACD[index] = macdCrossOver.MACD[index] * factor;
            MACD_Histogram[index] = macdCrossOver.Histogram[index] * factor;
            MACD_Signal[index] = macdCrossOver.Signal[index] * factor;
            RSI[index] = rsi.Result[index];
        }
    }
}
