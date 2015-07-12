using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class RSIBBands : Indicator
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 14)]
        public int Period { get; set; }

        [Parameter(DefaultValue = 2)]
        public double stdDev { get; set; }

        [Output("RSI")]
        public IndicatorDataSeries RSI { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Main { get; set; }

        [Output("Top")]
        public IndicatorDataSeries Top { get; set; }

        [Output("Bottom")]
        public IndicatorDataSeries Bottom { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Exponential)]
        public MovingAverageType MAType { get; set; }

        BollingerBands bbands;
        RelativeStrengthIndex rsi;


        protected override void Initialize()
        {
            rsi = Indicators.RelativeStrengthIndex(Source, Period);
            bbands = Indicators.BollingerBands(rsi.Result, Period, stdDev, MAType);
        }

        public override void Calculate(int index)
        {
            RSI[index] = rsi.Result[index];
            Bottom[index] = bbands.Bottom[index];
            Top[index] = bbands.Top[index];
            Main[index] = bbands.Main[index];
        }
    }
}
