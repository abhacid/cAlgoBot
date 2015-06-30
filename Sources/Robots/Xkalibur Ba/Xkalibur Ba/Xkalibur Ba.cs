//# reference: ..\Indicators\EHMA.algo

//Needs to install: http://ctdn.com/algos/indicators/show/17, Manage references.
//
//Date: 17/12/2014
//Country: Chile
//Copyright: Felipe Sepulveda Maldonado 
//LinkedIn: https://cl.linkedin.com/in/felipesepulvedamaldonado
//Facebook: https://www.facebook.com/mymagicflight1
//Whats Up: +56 9 58786321
// felipe.sepulveda@gmail.com
//
//Recomended Timeframe: Minute, for more frequency and accuracy.
//Cheers!

using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class XkaliburBa : Robot
    {
        #region cBot Parameters
        [Parameter("cBot Label", DefaultValue = "XkaliburBa")]
        public string cBotLabel { get; set; }

        [Parameter("Volume", DefaultValue = 1000, MinValue = 1000)]
        public int InitialVolume { get; set; }

        [Parameter("Take Profit", DefaultValue = 2, MinValue = 0, MaxValue = 102)]
        public double TakeProfit { get; set; }

        [Parameter("WilliamsR High", DefaultValue = 70, MinValue = 60, MaxValue = 95)]
        public int wHigh { get; set; }

        [Parameter("WilliamsR Low", DefaultValue = 30, MinValue = 5, MaxValue = 40)]
        public int wLow { get; set; }

        [Parameter("WilliamsR Period", DefaultValue = 120, MinValue = 10, MaxValue = 150)]
        public int wrPeriod { get; set; }

        [Parameter("Zero Loss Inverse (Pips)", DefaultValue = 6, MinValue = 4, MaxValue = 15)]
        public int zlInv { get; set; }

        [Parameter("EHMA Period", DefaultValue = 12, MinValue = 7)]
        public int HullPeriod { get; set; }

        [Parameter("Max. Time Open (Minutes)", DefaultValue = 30, MinValue = 5, MaxValue = 120)]
        public int maxTime { get; set; }
        double TrailingStop;
        int nVolume;
        string comId;
        private EHMA hullMA1;
        private EHMA hullMA2;
        #endregion

        #region cBot Events
        protected override void OnStart()
        {
            hullMA1 = Indicators.GetIndicator<EHMA>(HullPeriod);
            hullMA2 = Indicators.GetIndicator<EHMA>(HullPeriod * 6);
            Timer.Start(1);
        }

        protected override void OnTimer()
        {
        }

        protected override void OnTick()
        {
            double mercado = 100000 * Indicators.AverageTrueRange(MarketSeries, 5, MovingAverageType.VIDYA).Result.Last(0);
            double will = 100 + Indicators.WilliamsPctR(wrPeriod).Result.Last(0);

            if ((will < wLow) && hullMA1.ehma.IsRising() && hullMA2.ehma.IsRising())
            {
                System.Threading.Thread.Sleep(15000);
                ExecuteMarketOrder(TradeType.Buy, Symbol, InitialVolume, cBotLabel, 100, 100, 2);
            }

            if ((will > wHigh) && hullMA1.ehma.IsFalling() && hullMA2.ehma.IsFalling())
            {
                System.Threading.Thread.Sleep(15000);
                ExecuteMarketOrder(TradeType.Sell, Symbol, InitialVolume, cBotLabel, 100, 100, 2);
            }

            SetTrailingStop(mercado);
            ZeroLoss();
        }
        #endregion

        private void SetTrailingStop(double mercado)
        {
            var sellPositions = Positions.FindAll(cBotLabel, Symbol, TradeType.Sell);
            TrailingStop = mercado / 30;

            foreach (Position position in sellPositions)
            {
                int mPosition = (position.EntryTime.Hour * 60) + position.EntryTime.Minute + maxTime;
                int mActual = (Time.Hour * 60) + Time.Minute;

                double distance = position.EntryPrice - Symbol.Ask;
                if ((distance < TakeProfit * Symbol.PipSize) || (mPosition < mActual))
                    continue;

                double newStopLossPrice = Symbol.Ask + TrailingStop * Symbol.PipSize;
                if (position.StopLoss == null || newStopLossPrice < position.StopLoss)
                    ModifyPosition(position, newStopLossPrice, position.TakeProfit);
            }

            var buyPositions = Positions.FindAll(cBotLabel, Symbol, TradeType.Buy);

            foreach (Position position in buyPositions)
            {

                int mPosition = (position.EntryTime.Hour * 60) + position.EntryTime.Minute + maxTime;
                int mActual = (Time.Hour * 60) + Time.Minute;

                double distance = Symbol.Bid - position.EntryPrice;
                if ((distance < TakeProfit * Symbol.PipSize) || (mPosition < mActual))
                    continue;

                double newStopLossPrice = Symbol.Bid - TrailingStop * Symbol.PipSize;
                if (position.StopLoss == null || newStopLossPrice > position.StopLoss)
                    ModifyPosition(position, newStopLossPrice, position.TakeProfit);
            }
        }

        private void ZeroLoss()
        {
            var sellPositions = Positions.FindAll(cBotLabel, Symbol, TradeType.Sell);
            foreach (Position position in sellPositions)
            {
                double distance = position.EntryPrice - Symbol.Ask;
                double xtp = (double)((position.EntryPrice - position.TakeProfit) / Symbol.PipSize);

                if ((distance < -zlInv * Symbol.PipSize) && (xtp > 90))
                {
                    double zlTP = Symbol.Ask - 90 * Symbol.PipSize;
                    ModifyPosition(position, position.StopLoss, zlTP);

                    switch (position.Volume)
                    {
                        case 1000:
                            nVolume = 2000;
                            break;
                        case 2000:
                            nVolume = 3000;
                            break;
                        default:
                            nVolume = (int)(position.Volume * 2);
                            break;
                    }

                    if (position.Comment != "")
                        comId = position.Comment;
                    else
                        comId = position.Id.ToString();

                    ExecuteMarketOrder(TradeType.Buy, Symbol, nVolume, cBotLabel, 100, 100, 2, comId);
                }
            }

            var buyPositions = Positions.FindAll(cBotLabel, Symbol, TradeType.Buy);

            foreach (Position position in buyPositions)
            {
                double distance = Symbol.Bid - position.EntryPrice;
                double xtp = (double)((position.TakeProfit - position.EntryPrice) / Symbol.PipSize);

                if ((distance < -zlInv * Symbol.PipSize) && (xtp > 90))
                {
                    double zlTP = Symbol.Bid + 90 * Symbol.PipSize;
                    ModifyPosition(position, position.StopLoss, zlTP);

                    switch (position.Volume)
                    {
                        case 1000:
                            nVolume = 2000;
                            break;
                        case 2000:
                            nVolume = 3000;
                            break;
                        default:
                            nVolume = (int)(position.Volume * 2);
                            break;
                    }

                    if (position.Comment != "")
                        comId = position.Comment;
                    else
                        comId = position.Id.ToString();

                    ExecuteMarketOrder(TradeType.Sell, Symbol, nVolume, cBotLabel, 100, 100, 2, comId);
                }
            }
        }
    }
}

