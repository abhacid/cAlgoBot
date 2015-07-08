using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC)]
    public class EMAMTF : Indicator
    {
        [Parameter(DefaultValue = 14)]
        public int Periods { get; set; }

        [Parameter("EMA Timeframe1", DefaultValue = "Minute15")]
        public TimeFrame EMATimeframe1 { get; set; }

        [Parameter("EMA Timeframe2", DefaultValue = "Hour")]
        public TimeFrame EMATimeframe2 { get; set; }

        [Parameter("EMA Timeframe3", DefaultValue = "Hour4")]
        public TimeFrame EMATimeframe3 { get; set; }

        [Output("EMA1", Color = Colors.Blue)]
        public IndicatorDataSeries EMA1 { get; set; }

        [Output("EMA2", Color = Colors.Red)]
        public IndicatorDataSeries EMA2 { get; set; }

        [Output("EMA3", Color = Colors.Yellow)]
        public IndicatorDataSeries EMA3 { get; set; }

        private MarketSeries series1;
        private MarketSeries series2;
        private MarketSeries series3;

        private ExponentialMovingAverage Ema1;
        private ExponentialMovingAverage Ema2;
        private ExponentialMovingAverage Ema3;

        protected override void Initialize()
        {
            series1 = MarketData.GetSeries(EMATimeframe1);
            series2 = MarketData.GetSeries(EMATimeframe2);
            series3 = MarketData.GetSeries(EMATimeframe3);

            Ema1 = Indicators.ExponentialMovingAverage(series1.Close, Periods);
            Ema2 = Indicators.ExponentialMovingAverage(series2.Close, Periods);
            Ema3 = Indicators.ExponentialMovingAverage(series3.Close, Periods);

        }

        public override void Calculate(int index)
        {

            var index1 = GetIndexByDate(series1, MarketSeries.OpenTime[index]);
            if (index1 != -1)
            {
                EMA1[index] = Ema1.Result[index1];
            }

            var index2 = GetIndexByDate(series2, MarketSeries.OpenTime[index]);
            if (index2 != -1)
            {
                EMA2[index] = Ema2.Result[index2];
            }

            var index3 = GetIndexByDate(series3, MarketSeries.OpenTime[index]);
            if (index3 != -1)
            {
                EMA3[index] = Ema3.Result[index3];
            }

        }


        private int GetIndexByDate(MarketSeries series, DateTime time)
        {
            for (int i = series.Close.Count - 1; i > 0; i--)
            {
                if (time == series.OpenTime[i])
                    return i;
            }
            return -1;
        }
    }
}
