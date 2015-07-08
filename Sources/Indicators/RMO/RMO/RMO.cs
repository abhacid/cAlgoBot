using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Levels(-7,7)]
    [Indicator(AccessRights = AccessRights.None)]
    public class RMO:Indicator
    {
        
        private SimpleMovingAverage _sma;
        private SimpleMovingAverage _sma2;
        private SimpleMovingAverage _sma3;
        private SimpleMovingAverage _sma4;
        private SimpleMovingAverage _sma5;
        private SimpleMovingAverage _sma6;
        private SimpleMovingAverage _sma7;
        private SimpleMovingAverage _sma8;
        private SimpleMovingAverage _sma9;
        private SimpleMovingAverage _sma10;
        private ExponentialMovingAverage _ema1;
        private ExponentialMovingAverage _ema2;
        private ExponentialMovingAverage _ema3;
        private ExponentialMovingAverage _ema4;
        private ExponentialMovingAverage _ema5;
        private ExponentialMovingAverage _ema6;


        private IndicatorDataSeries iSeries1;
        private IndicatorDataSeries iSeries4;

        [Output("ST2", Color = Colors.Black)]
        public IndicatorDataSeries ST2 { get; set; }

        [Output("ST3", Color = Colors.Black)]
        public IndicatorDataSeries ST3 { get; set; }
        
        

        [Parameter(DefaultValue = 2)]
        public int Len1 { get; set; }

        [Parameter(DefaultValue = 10)]
        public int Len2 { get; set; }
        
        [Parameter(DefaultValue = 30)]
        public int Len3 { get; set; }
        
        [Parameter(DefaultValue = 81)]
        public int Len4 { get; set; }

        [Output("Bearish", Color = Colors.Red, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries BearBuffer { get; set; }
        [Output("Bullish", Color = Colors.Green, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries BullBuffer { get; set; }
        [Output("Neutral", Color = Colors.Gray, PlotType = PlotType.Histogram)]
        public IndicatorDataSeries NeutralBuffer { get; set; }


        protected override void Initialize()
        {
            iSeries1 = CreateDataSeries();            
            iSeries4 = CreateDataSeries();

            _sma = Indicators.SimpleMovingAverage(MarketSeries.Close, Len1);
            _sma2 = Indicators.SimpleMovingAverage(_sma.Result, Len1);
            _sma3 = Indicators.SimpleMovingAverage(_sma2.Result, Len1);
            _sma4 = Indicators.SimpleMovingAverage(_sma3.Result, Len1);
            _sma5 = Indicators.SimpleMovingAverage(_sma4.Result, Len1);
            _sma6 = Indicators.SimpleMovingAverage(_sma5.Result, Len1);
            _sma7 = Indicators.SimpleMovingAverage(_sma6.Result, Len1);
            _sma8 = Indicators.SimpleMovingAverage(_sma7.Result, Len1);
            _sma9 = Indicators.SimpleMovingAverage(_sma8.Result, Len1);
            _sma10 = Indicators.SimpleMovingAverage(_sma9.Result, Len1);

            _ema1 = Indicators.ExponentialMovingAverage(iSeries1, Len3);
            _ema2 = Indicators.ExponentialMovingAverage(_ema1.Result, Len3);

            _ema3 = Indicators.ExponentialMovingAverage(ST2, Len3);
            _ema4 = Indicators.ExponentialMovingAverage(_ema3.Result, Len3);
            
            _ema5 = Indicators.ExponentialMovingAverage(iSeries1, Len4);
            _ema6 = Indicators.ExponentialMovingAverage(_ema3.Result, Len4);


        }

        public override void Calculate(int index)
        {
            if (index < 4)
                return;

            double fix = MarketSeries.High.Maximum(Len2 - 1) - MarketSeries.Low.Minimum(Len2 - 1);

            if (Math.Abs(fix - 0) < double.Epsilon)
                fix = 1;

            iSeries1[index] = 100*(MarketSeries.Close[index] -
                                   (_sma.Result[index] + _sma2.Result[index] + _sma3.Result[index]
                                    + _sma4.Result[index] + _sma5.Result[index] + _sma6.Result[index]
                                    + _sma7.Result[index] + _sma8.Result[index] + _sma9.Result[index]
                                    + _sma10.Result[index])/10)/fix;
            
            ST2[index] = 2*_ema1.Result[index] - _ema2.Result[index];

            ST3[index] = 2*_ema3.Result[index] - _ema4.Result[index];

            iSeries4[index] = 2*_ema5.Result[index] - _ema6.Result[index];

            if (iSeries4[index] > 0 && ST2[index] > 0 && ST3[index] > 0)
                BullBuffer[index] = iSeries4[index];
            else if (iSeries4[index] < 0 && ST2[index] < 0 && ST3[index] < 0)
                BearBuffer[index] = iSeries4[index];
            else
                NeutralBuffer[index] = iSeries4[index];

        }

    }
}
