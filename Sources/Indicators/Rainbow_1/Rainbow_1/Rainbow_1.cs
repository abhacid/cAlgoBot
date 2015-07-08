// -------------------------------------------------------------------------------
//
//    Rainbow
//
// -------------------------------------------------------------------------------

using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Rainbow_1 : Indicator
    {
        #region private fields

        private ExponentialMovingAverage _ema1;
        private ExponentialMovingAverage _ema2;
        private ExponentialMovingAverage _ema3;
        private ExponentialMovingAverage _ema4;
        private ExponentialMovingAverage _ema5;
        private ExponentialMovingAverage _ema6;
        private ExponentialMovingAverage _ema7;
        private ExponentialMovingAverage _ema8;
        private ExponentialMovingAverage _ema9;
        private ExponentialMovingAverage _ema10;
        private ExponentialMovingAverage _ema11;
        private ExponentialMovingAverage _ema12;
        private ExponentialMovingAverage _ema13;
        private ExponentialMovingAverage _ema14;
        private ExponentialMovingAverage _ema15;
        private ExponentialMovingAverage _ema16;
        private ExponentialMovingAverage _ema17;
        private ExponentialMovingAverage _ema18;
        private ExponentialMovingAverage _ema19;
        private ExponentialMovingAverage _ema20;
        private ExponentialMovingAverage _ema21;
        private ExponentialMovingAverage _ema22;
        private ExponentialMovingAverage _ema23;
        private ExponentialMovingAverage _ema24;
        private ExponentialMovingAverage _ema25;
        private ExponentialMovingAverage _ema26;
        private ExponentialMovingAverage _ema27;
        private ExponentialMovingAverage _ema28;
        private ExponentialMovingAverage _ema29;
        private ExponentialMovingAverage _ema30;


        #endregion

        #region Output

        [Output("1", PlotType = PlotType.Line, Color = Colors.MediumOrchid)]
        public IndicatorDataSeries ExtMapBuffer1 { get; set; }

        [Output("2", PlotType = PlotType.Line, Color = Colors.MediumOrchid)]
        public IndicatorDataSeries ExtMapBuffer2 { get; set; }

        [Output("3", PlotType = PlotType.Line, Color = Colors.MediumOrchid)]
        public IndicatorDataSeries ExtMapBuffer3 { get; set; }

        [Output("4", PlotType = PlotType.Line, Color = Colors.MediumOrchid)]
        public IndicatorDataSeries ExtMapBuffer4 { get; set; }

        [Output("5", PlotType = PlotType.Line, Color = Colors.MediumOrchid)]
        public IndicatorDataSeries ExtMapBuffer5 { get; set; }

        [Output("6", PlotType = PlotType.Line, Color = Colors.MediumOrchid)]
        public IndicatorDataSeries ExtMapBuffer6 { get; set; }

        [Output("7", PlotType = PlotType.Line, Color = Colors.MediumOrchid)]
        public IndicatorDataSeries ExtMapBuffer7 { get; set; }

        [Output("8", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries ExtMapBuffer8 { get; set; }

        [Output("9", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries ExtMapBuffer9 { get; set; }

        [Output("10", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries ExtMapBuffer10 { get; set; }

        [Output("11", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries ExtMapBuffer11 { get; set; }

        [Output("12", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries ExtMapBuffer12 { get; set; }

        [Output("13", PlotType = PlotType.Line, Color = Colors.Red)]
        public IndicatorDataSeries ExtMapBuffer13 { get; set; }

        [Output("14", PlotType = PlotType.Line, Color = Colors.Lime)]
        public IndicatorDataSeries ExtMapBuffer14 { get; set; }

        [Output("15", PlotType = PlotType.Line, Color = Colors.Lime)]
        public IndicatorDataSeries ExtMapBuffer15 { get; set; }

        [Output("16", PlotType = PlotType.Line, Color = Colors.Lime)]
        public IndicatorDataSeries ExtMapBuffer16 { get; set; }

        [Output("17", PlotType = PlotType.Line, Color = Colors.Lime)]
        public IndicatorDataSeries ExtMapBuffer17 { get; set; }

        [Output("18", PlotType = PlotType.Line, Color = Colors.Lime)]
        public IndicatorDataSeries ExtMapBuffer18 { get; set; }

        [Output("19", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries ExtMapBuffer19 { get; set; }

        [Output("20", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries ExtMapBuffer20 { get; set; }

        [Output("21", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries ExtMapBuffer21 { get; set; }

        [Output("22", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries ExtMapBuffer22 { get; set; }

        [Output("23", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries ExtMapBuffer23 { get; set; }

        [Output("24", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries ExtMapBuffer24 { get; set; }

        [Output("25", PlotType = PlotType.Line, Color = Colors.DeepSkyBlue)]
        public IndicatorDataSeries ExtMapBuffer25 { get; set; }

        [Output("26", PlotType = PlotType.Line, Color = Colors.Gold)]
        public IndicatorDataSeries ExtMapBuffer26 { get; set; }

        [Output("27", PlotType = PlotType.Line, Color = Colors.Gold)]
        public IndicatorDataSeries ExtMapBuffer27 { get; set; }

        [Output("28", PlotType = PlotType.Line, Color = Colors.Gold)]
        public IndicatorDataSeries ExtMapBuffer28 { get; set; }

        [Output("29", PlotType = PlotType.Line, Color = Colors.Gold)]
        public IndicatorDataSeries ExtMapBuffer29 { get; set; }

        [Output("30", PlotType = PlotType.Line, Color = Colors.Gold)]
        public IndicatorDataSeries ExtMapBuffer30 { get; set; }




        #endregion

        protected override void Initialize()
        {


            _ema1 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 170);
            _ema2 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 180);
            _ema3 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 190);
            _ema4 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 195);
            _ema5 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 200);

            _ema6 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 160);
            _ema7 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 150);
            _ema8 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 140);
            _ema9 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 130);
            _ema10 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 120);

            _ema11 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 110);
            _ema12 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 97);
            _ema13 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 88);
            _ema14 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 79);
            _ema15 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 70);

            _ema16 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 62);
            _ema17 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 56);
            _ema18 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 48);
            _ema19 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 42);
            _ema20 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 36);

            _ema21 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 28);
            _ema22 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 24);
            _ema23 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 22);
            _ema24 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 18);
            _ema25 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 14);

            _ema26 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 12);
            _ema27 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 10);
            _ema28 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 8);
            _ema29 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 6);
            _ema30 = Indicators.ExponentialMovingAverage(MarketSeries.Close, 4);

        }

        public override void Calculate(int index)
        {

            #region

            ExtMapBuffer1[index] = _ema1.Result[index];
            ExtMapBuffer2[index] = _ema2.Result[index];
            ExtMapBuffer3[index] = _ema3.Result[index];
            ExtMapBuffer4[index] = _ema4.Result[index];
            ExtMapBuffer5[index] = _ema5.Result[index];
            ExtMapBuffer6[index] = _ema6.Result[index];

            ExtMapBuffer7[index] = _ema7.Result[index];
            ExtMapBuffer8[index] = _ema8.Result[index];
            ExtMapBuffer9[index] = _ema9.Result[index];
            ExtMapBuffer10[index] = _ema10.Result[index];
            ExtMapBuffer11[index] = _ema11.Result[index];
            ExtMapBuffer12[index] = _ema12.Result[index];

            ExtMapBuffer13[index] = _ema13.Result[index];
            ExtMapBuffer14[index] = _ema14.Result[index];
            ExtMapBuffer15[index] = _ema15.Result[index];
            ExtMapBuffer16[index] = _ema16.Result[index];
            ExtMapBuffer17[index] = _ema17.Result[index];
            ExtMapBuffer18[index] = _ema18.Result[index];

            ExtMapBuffer19[index] = _ema19.Result[index];
            ExtMapBuffer20[index] = _ema20.Result[index];
            ExtMapBuffer21[index] = _ema21.Result[index];
            ExtMapBuffer22[index] = _ema22.Result[index];
            ExtMapBuffer23[index] = _ema23.Result[index];
            ExtMapBuffer24[index] = _ema24.Result[index];

            ExtMapBuffer25[index] = _ema25.Result[index];
            ExtMapBuffer26[index] = _ema26.Result[index];
            ExtMapBuffer27[index] = _ema27.Result[index];
            ExtMapBuffer28[index] = _ema28.Result[index];
            ExtMapBuffer29[index] = _ema29.Result[index];
            ExtMapBuffer30[index] = _ema30.Result[index];




            #endregion

        }
    }
}