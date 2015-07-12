
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class RibbonStudyEMA : Indicator
    {
        [Output("RibEMA1", Color = Colors.White)]
        public IndicatorDataSeries EMA1 { get; set; }
        [Output("RibEMA2", Color = Colors.White)]
        public IndicatorDataSeries EMA2 { get; set; }
        [Output("RibEMA3", Color = Colors.LightYellow)]
        public IndicatorDataSeries EMA3 { get; set; }
        [Output("RibEMA4", Color = Colors.LightYellow)]
        public IndicatorDataSeries EMA4 { get; set; }
        [Output("RibEMA5", Color = Colors.Yellow)]
        public IndicatorDataSeries EMA5 { get; set; }
        [Output("RibEMA6", Color = Colors.Yellow)]
        public IndicatorDataSeries EMA6 { get; set; }
        [Output("RibEMA7", Color = Colors.Gold)]
        public IndicatorDataSeries EMA7 { get; set; }
        [Output("RibEMA8", Color = Colors.Gold)]
        public IndicatorDataSeries EMA8 { get; set; }
        [Output("RibEMA9", Color = Colors.Red)]
        public IndicatorDataSeries EMA9 { get; set; }
        [Output("RibEMA10", Color = Colors.Red)]
        public IndicatorDataSeries EMA10 { get; set; }

        [Parameter(DefaultValue = 5)]
        public int Period1 { get; set; }
        [Parameter(DefaultValue = 10)]
        public int Period2 { get; set; }
        [Parameter(DefaultValue = 15)]
        public int Period3 { get; set; }
        [Parameter(DefaultValue = 20)]
        public int Period4 { get; set; }
        [Parameter(DefaultValue = 25)]
        public int Period5 { get; set; }
        [Parameter(DefaultValue = 30)]
        public int Period6 { get; set; }
        [Parameter(DefaultValue = 35)]
        public int Period7 { get; set; }
        [Parameter(DefaultValue = 40)]
        public int Period8 { get; set; }
        [Parameter(DefaultValue = 45)]
        public int Period9 { get; set; }
        [Parameter(DefaultValue = 50)]
        public int Period10 { get; set; }

        private ExponentialMovingAverage ema1;
        private ExponentialMovingAverage ema2;
        private ExponentialMovingAverage ema3;
        private ExponentialMovingAverage ema4;
        private ExponentialMovingAverage ema5;
        private ExponentialMovingAverage ema6;
        private ExponentialMovingAverage ema7;
        private ExponentialMovingAverage ema8;
        private ExponentialMovingAverage ema9;
        private ExponentialMovingAverage ema10;
        protected override void Initialize()
        {
            ema1 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period1);
            ema2 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period2);
            ema3 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period3);
            ema4 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period4);
            ema5 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period5);
            ema6 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period6);
            ema7 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period7);
            ema8 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period8);
            ema9 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period9);
            ema10 = Indicators.ExponentialMovingAverage(MarketSeries.Close, Period10);
        }

        public override void Calculate(int index)
        {
            EMA1[index] = ema1.Result[index];
            EMA2[index] = ema2.Result[index];
            EMA3[index] = ema3.Result[index];
            EMA4[index] = ema4.Result[index];
            EMA5[index] = ema5.Result[index];
            EMA6[index] = ema6.Result[index];
            EMA7[index] = ema7.Result[index];
            EMA8[index] = ema8.Result[index];
            EMA9[index] = ema9.Result[index];
            EMA10[index] = ema10.Result[index];
        }
    }
}
