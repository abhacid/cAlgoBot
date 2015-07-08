using System;
using cAlgo.API;
using cAlgo.API.Internals;
using System.Runtime.InteropServices;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Fxa0RSICrossing50PlusATR : Indicator
    {
        // Alert
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int MessageBox(IntPtr hWnd, String text, String caption, uint type);

        [Parameter()]
        public DataSeries SourceSeries { get; set; }

        [Parameter(DefaultValue = 0.15)]
        public double ATR_percent { get; set; }

        [Parameter(DefaultValue = 21)]
        public int RSI_period { get; set; }

        [Parameter(DefaultValue = 21)]
        public int ATR_period { get; set; }

        [Output("Up", PlotType = PlotType.Points, Thickness = 4)]
        public IndicatorDataSeries UpSeries { get; set; }

        [Output("Down", PlotType = PlotType.Points, Color = Colors.Red, Thickness = 4)]
        public IndicatorDataSeries DownSeries { get; set; }

        [Output("Sell", PlotType = PlotType.Points, Color = Colors.Yellow, Thickness = 4)]
        public IndicatorDataSeries SellSeries { get; set; }

        private DateTime _openTime;

        private RelativeStrengthIndex RSINow;
        private Position position;
        private AverageTrueRange ATR;
        private int trend = 0;
        private MarketSeries seriesH1;
        private double price = 0;
        private double stopLoss = 0;
        private double lastRSI60arrow = 0;
        private double lastCloseLong;
        private double lastCloseShort = 0;
        private double lastRSI40arrow = 0;
        //set out of area

        protected override void Initialize()
        {
            RSINow = Indicators.RelativeStrengthIndex(SourceSeries, RSI_period);
            seriesH1 = MarketData.GetSeries(TimeFrame.Daily);
            ATR = Indicators.AverageTrueRange(seriesH1, ATR_period, MovingAverageType.Simple);

        }

        public override void Calculate(int index)
        {
            if (index < 1)
                return;


            double RSI_now = RSINow.Result[index];
            double RSI2 = RSINow.Result[index - 1];

            double myATR1 = ATR.Result[index];

            // up
            if (RSI_now >= 50)
            {
                //did it cross from below 50
                if (RSI_now > 50 && RSI2 < 50)
                {
                    deletealllines();
                    price = Symbol.Bid + (myATR1 * ATR_percent);
                    stopLoss = Symbol.Bid + (myATR1 * ATR_percent) - (0.3 * myATR1);
                    UpSeries[index] = MarketSeries.Low[index] - 2 * Symbol.PipValue;
                    ChartObjects.DrawHorizontalLine("entry", price, Colors.Blue);
                    ChartObjects.DrawHorizontalLine("stop", stopLoss, Colors.Blue);
                    lastRSI60arrow = 0;
                    lastCloseLong = 0;
                }
                //add to position at cross of 60, sometimes this can occur twice
                if (RSI_now >= 60 && RSI2 < 60)
                {
                    //don't draw another arrow
                    if (lastRSI60arrow >= 60)
                    {
                    }
                    //draw another arrow for adding to position
                    if (lastRSI60arrow <= 60)
                    {
                        UpSeries[index] = MarketSeries.Low[index] - 2 * Symbol.PipValue;
                        lastRSI60arrow = RSI_now;
                    }
                }

                //sell first lot
                if (RSI_now < 70 && RSI2 >= 70)
                {
                    if (lastCloseLong == 0)
                    {
                        SellSeries[index] = MarketSeries.High[index] + 2 * Symbol.PipValue;
                        lastCloseLong = 1;
                    }
                }

            }


            // down
            //is going short
            if (RSI_now < 50)
            {
                //did it cross from above 50
                if (RSI_now < 50 && RSI2 > 50)
                {
                    deletealllines();
                    DownSeries[index] = MarketSeries.High[index] + 2 * Symbol.PipValue;
                    price = Symbol.Ask - (myATR1 * ATR_percent);
                    stopLoss = Symbol.Ask - (myATR1 * ATR_percent) + (0.3 * myATR1);
                    ChartObjects.DrawHorizontalLine("entry", price, Colors.Blue);
                    ChartObjects.DrawHorizontalLine("stop", stopLoss, Colors.Blue);
                    lastCloseShort = 0;
                    lastRSI40arrow = RSI_now;
                    //set out of area
                }
                if (RSI_now < 40 && RSI2 > 40)
                {
                    //don't draw another arrow
                    if (lastRSI40arrow <= 40)
                    {

                    }
                    //draw another arrow to add to position
                    if (lastRSI40arrow >= 40)
                    {
                        DownSeries[index] = MarketSeries.High[index] + 2 * Symbol.PipValue;
                        lastRSI40arrow = RSI_now;
                    }
                }
                if (RSI_now > 30 && RSI2 < 30)
                {
                    if (lastCloseShort == 0)
                    {
                        SellSeries[index] = MarketSeries.Low[index] - 4 * Symbol.PipValue;
                        lastCloseShort = 1;
                    }
                }
            }


        }

        protected void DisplayAlert(string tradyTypeSignal, double takeProfit, double stopLoss, double entryPrice)
        {
            string entryPricetext = entryPrice != 0.0 ? string.Format(" at price {0}", Math.Round(entryPrice, 4)) : "";
            string takeProfitText = takeProfit != 0.0 ? string.Format(", TP on  {0}", Math.Round(takeProfit, 4)) : "";
            string stopLossText = stopLoss != 0.0 ? string.Format(", SL on {0}", Math.Round(stopLoss, 4)) : "";

            var alertMessage = string.Format("{0} {1} {2} {3} {4}", tradyTypeSignal, entryPricetext, takeProfitText, stopLossText, Symbol.Code);

            MessageBox(new IntPtr(0), alertMessage, "Trade Signal", 0);

        }

        protected void deletealllines()
        {
            ChartObjects.RemoveObject("entry");
            ChartObjects.RemoveObject("stop");

        }

    }
}
