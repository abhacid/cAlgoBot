using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class GMMA : Indicator
    {
        [Output("Short EMA1", Color = Colors.Blue)]
        public IndicatorDataSeries ShortEma1 { get; set; }

        [Output("Short EMA2", Color = Colors.Blue)]
        public IndicatorDataSeries ShortEma2 { get; set; }

        [Output("Short EMA3", Color = Colors.Blue)]
        public IndicatorDataSeries ShortEma3 { get; set; }

        [Output("Short EMA4", Color = Colors.Blue)]
        public IndicatorDataSeries ShortEma4 { get; set; }

        [Output("Short EMA5", Color = Colors.Blue)]
        public IndicatorDataSeries ShortEma5 { get; set; }

        [Output("Short EMA6", Color = Colors.Blue)]
        public IndicatorDataSeries ShortEma6 { get; set; }



        [Output("Long EMA1", Color = Colors.Red)]
        public IndicatorDataSeries LongEma1 { get; set; }

        [Output("Long EMA2", Color = Colors.Red)]
        public IndicatorDataSeries LongEma2 { get; set; }

        [Output("Long EMA3", Color = Colors.Red)]
        public IndicatorDataSeries LongEma3 { get; set; }

        [Output("Long EMA4", Color = Colors.Red)]
        public IndicatorDataSeries LongEma4 { get; set; }

        [Output("Long EMA5", Color = Colors.Red)]
        public IndicatorDataSeries LongEma5 { get; set; }

        [Output("Long EMA6", Color = Colors.Red)]
        public IndicatorDataSeries LongEma6 { get; set; }





        private ExponentialMovingAverage m_shortEma1;
        private ExponentialMovingAverage m_shortEma2;
        private ExponentialMovingAverage m_shortEma3;
        private ExponentialMovingAverage m_shortEma4;
        private ExponentialMovingAverage m_shortEma5;
        private ExponentialMovingAverage m_shortEma6;

        private ExponentialMovingAverage m_longEma1;
        private ExponentialMovingAverage m_longEma2;
        private ExponentialMovingAverage m_longEma3;
        private ExponentialMovingAverage m_longEma4;
        private ExponentialMovingAverage m_longEma5;
        private ExponentialMovingAverage m_longEma6;



        protected override void Initialize()
        {
            m_shortEma1 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 3);
            m_shortEma2 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 5);
            m_shortEma3 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 8);
            m_shortEma4 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 10);
            m_shortEma5 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 12);
            m_shortEma6 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 15);

            m_longEma1 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 30);
            m_longEma2 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 35);
            m_longEma3 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 40);
            m_longEma4 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 45);
            m_longEma5 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 50);
            m_longEma6 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 60);
        }

        public override void Calculate(int index)
        {
            ShortEma1[index] = m_shortEma1.Result[index];
            ShortEma2[index] = m_shortEma2.Result[index];
            ShortEma3[index] = m_shortEma3.Result[index];
            ShortEma4[index] = m_shortEma4.Result[index];
            ShortEma5[index] = m_shortEma5.Result[index];
            ShortEma6[index] = m_shortEma6.Result[index];

            LongEma1[index] = m_longEma1.Result[index];
            LongEma2[index] = m_longEma2.Result[index];
            LongEma3[index] = m_longEma3.Result[index];
            LongEma4[index] = m_longEma4.Result[index];
            LongEma5[index] = m_longEma5.Result[index];
            LongEma6[index] = m_longEma6.Result[index];
        }
    }
}
