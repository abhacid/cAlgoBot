//Date: 11/12/2014
//Country: Chile
//Copyright: Felipe Sepulveda Maldonado
//LinkedIn: https://cl.linkedin.com/in/felipesepulvedamaldonado
//Facebook: https://www.facebook.com/mymagicflight1
//Whats Up: +56 9 58786321
//Donations Wallet: wallet.google.com felipe.sepulveda@gmail.com
//Description: WillR% & Moving Average
//Recomended Timeframe: Minute, for more frequency and accuracy.
//Cheers!

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Maithai : Robot
    {
        #region cBot Parameters
        [Parameter("cBot Label", DefaultValue = "Maithai")]
        public string cBotLabel { get; set; }

        [Parameter("Volume", DefaultValue = 1000, MinValue = 1000)]
        public int InitialVolume { get; set; }

        [Parameter("Take Profit", DefaultValue = 2, MinValue = 1, MaxValue = 100)]
        public int TakeProfit { get; set; }

        [Parameter("Trailing Stop", DefaultValue = 1, MinValue = 0, MaxValue = 30)]
        public int TrailingStop { get; set; }

        [Parameter("Stop Loss", DefaultValue = 14, MinValue = 5, MaxValue = 150)]
        public int StopLoss { get; set; }

        [Parameter("WilliamsR High", DefaultValue = 90, MinValue = 85, MaxValue = 95)]
        public int wHigh { get; set; }

        [Parameter("WilliamsR Low", DefaultValue = 15, MinValue = 5, MaxValue = 20)]
        public int wLow { get; set; }

        [Parameter("WilliamsR Period", DefaultValue = 100, MinValue = 50, MaxValue = 200)]
        public int wrPeriod { get; set; }

        [Parameter("ATR Market Volatility Min.", DefaultValue = 35, MinValue = 0, MaxValue = 110)]
        public int atrMarketMin { get; set; }

        [Parameter("ATR Market Volatility Max.", DefaultValue = 100, MinValue = 100)]
        public int atrMarketMax { get; set; }

        [Parameter("ATR Period", DefaultValue = 10, MinValue = 1, MaxValue = 100)]
        public int atrPeriod { get; set; }

        [Parameter("Moving Average Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MovingAverageType { get; set; }

        [Parameter("MA Period", DefaultValue = 14, MinValue = 3, MaxValue = 100)]
        public int Period { get; set; }

        [Parameter("MA Source")]
        public DataSeries Source { get; set; }

        [Parameter("Max. Time Open (Minutes)", DefaultValue = 60, MinValue = 5, MaxValue = 360)]
        public int maxTime { get; set; }
        #endregion

        private MovingAverage _movingAverage;

        #region cBot Events
        protected override void OnStart()
        {
            _movingAverage = Indicators.MovingAverage(Source, Period, MovingAverageType);
            Timer.Start(1);
        }

        protected override void OnTimer()
        {
        }

        protected override void OnTick()
        {
            //Put your core logic here
            double mercado = 100000 * Indicators.AverageTrueRange(MarketSeries, atrPeriod, MovingAverageType.VIDYA).Result.Last(0);
            double will = 100 + Indicators.WilliamsPctR(wrPeriod).Result.Last(0);

            ChartObjects.DrawText("", "Market:" + mercado.ToString() + "WillR:" + will.ToString(), StaticPosition.BottomRight, Colors.White);

            if ((will < wLow) && (mercado > atrMarketMin) && (mercado < atrMarketMax) && _movingAverage.Result.IsRising())
            {
                System.Threading.Thread.Sleep(1000);
                var result = ExecuteMarketOrder(TradeType.Buy, Symbol, InitialVolume, cBotLabel, StopLoss, 100, 2, mercado.ToString() + "-" + will.ToString());
            }

            if ((will > wHigh) && (mercado > atrMarketMax) && (mercado < atrMarketMax) && _movingAverage.Result.IsFalling())
            {
                System.Threading.Thread.Sleep(1000);
                var result = ExecuteMarketOrder(TradeType.Sell, Symbol, InitialVolume, cBotLabel, StopLoss, 100, 2, mercado.ToString() + "-" + will.ToString());
            }
            // Trailing Stop for all positions
            SetTrailingStop();
            TimerClose();
        }
        #endregion

        private void SetTrailingStop()
        {
            var sellPositions = Positions.FindAll(cBotLabel, Symbol, TradeType.Sell);

            foreach (Position position in sellPositions)
            {
                double distance = position.EntryPrice - Symbol.Ask;
                if (distance < TakeProfit * Symbol.PipSize)
                    continue;

                double newStopLossPrice = Symbol.Ask + TrailingStop * Symbol.PipSize;
                if (position.StopLoss == null || newStopLossPrice < position.StopLoss)
                    ModifyPosition(position, newStopLossPrice, position.TakeProfit);
            }

            var buyPositions = Positions.FindAll(cBotLabel, Symbol, TradeType.Buy);

            foreach (Position position in buyPositions)
            {
                double distance = Symbol.Bid - position.EntryPrice;
                if (distance < TakeProfit * Symbol.PipSize)
                    continue;

                double newStopLossPrice = Symbol.Bid - TrailingStop * Symbol.PipSize;
                if (position.StopLoss == null || newStopLossPrice > position.StopLoss)
                    ModifyPosition(position, newStopLossPrice, position.TakeProfit);
            }
        }

        private void TimerClose()
        {
            foreach (Position position in Positions)
            {
                int mPosition = (position.EntryTime.Hour * 60) + position.EntryTime.Minute + maxTime;
                int mActual = (Time.Hour * 60) + Time.Minute;
                //Si es que pasaron X min, entonces reevalua Buy (Abajo). Deja de insistir?
                if (mPosition < mActual)
                    ClosePosition(position);

            }
        }
    }
}
