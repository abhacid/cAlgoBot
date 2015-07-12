
using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class RibbonStudySMA : Indicator
    {
        [Output("RibSMA1", Color = Colors.White)]
        public IndicatorDataSeries SMA1 { get; set; }
        [Output("RibSMA2", Color = Colors.White)]
        public IndicatorDataSeries SMA2 { get; set; }
        [Output("RibSMA3", Color = Colors.LightYellow)]
        public IndicatorDataSeries SMA3 { get; set; }
        [Output("RibSMA4", Color = Colors.LightYellow)]
        public IndicatorDataSeries SMA4 { get; set; }
        [Output("RibSMA5", Color = Colors.Yellow)]
        public IndicatorDataSeries SMA5 { get; set; }
        [Output("RibSMA6", Color = Colors.Yellow)]
        public IndicatorDataSeries SMA6 { get; set; }
        [Output("RibSMA7", Color = Colors.Gold)]
        public IndicatorDataSeries SMA7 { get; set; }
        [Output("RibSMA8", Color = Colors.Gold)]
        public IndicatorDataSeries SMA8 { get; set; }
        [Output("RibSMA9", Color = Colors.Red)]
        public IndicatorDataSeries SMA9 { get; set; }
        [Output("RibSMA10", Color = Colors.Red)]
        public IndicatorDataSeries SMA10 { get; set; }

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

        private SimpleMovingAverage sma1;
        private SimpleMovingAverage sma2;
        private SimpleMovingAverage sma3;
        private SimpleMovingAverage sma4;
        private SimpleMovingAverage sma5;
        private SimpleMovingAverage sma6;
        private SimpleMovingAverage sma7;
        private SimpleMovingAverage sma8;
        private SimpleMovingAverage sma9;
        private SimpleMovingAverage sma10;
        protected override void Initialize()
        {
            sma1 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period1);
            sma2 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period2);
            sma3 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period3);
            sma4 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period4);
            sma5 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period5);
            sma6 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period6);
            sma7 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period7);
            sma8 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period8);
            sma9 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period9);
            sma10 = Indicators.SimpleMovingAverage(MarketSeries.Close, Period10);
        }

        public override void Calculate(int index)
        {
            SMA1[index] = sma1.Result[index];
            SMA2[index] = sma2.Result[index];
            SMA3[index] = sma3.Result[index];
            SMA4[index] = sma4.Result[index];
            SMA5[index] = sma5.Result[index];
            SMA6[index] = sma6.Result[index];
            SMA7[index] = sma7.Result[index];
            SMA8[index] = sma8.Result[index];
            SMA9[index] = sma9.Result[index];
            SMA10[index] = sma10.Result[index];
        }
    }
}
