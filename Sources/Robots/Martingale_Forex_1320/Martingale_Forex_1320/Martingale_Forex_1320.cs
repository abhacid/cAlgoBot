#region Licence
//The MIT License (MIT)
//Copyright (c) 2014 abdallah HACID, https://www.facebook.com/ab.hacid

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//and associated documentation files (the "Software"), to deal in the Software without restriction,
//including without limitation the rights to use, copy, modify, merge, publish, distribute,
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
//is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or
//substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
//BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/Robot_Forex
#endregion


#region Description
//
// Le projet et sa description se trouvent sur Github à l'adresse https://github.com/abhacid/Martingale_Forex
//
// Ce projet permet d'écrire un robot de trading basé sur un exemple Robot_Forex initial écrit par 
// imWald sur le dépôt de code source CTDN.
//
// Pour résumer c'est une martingale avec stop loss et money management.

#endregion

using System;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.Lib;

namespace cAlgo.Robots
{
    [Robot("Martingale Forex", AccessRights = AccessRights.None)]
    public class Martingale_Forex : Robot
    {
        #region Parameters
        [Parameter("Money Management (%)", DefaultValue = 1.6, MinValue = 0)]
        public double MoneyManagement { get; set; }

        [Parameter("Take Profit", DefaultValue = 5, MinValue = 5)]
        public double TakeProfit { get; set; }

        [Parameter("Stop Loss Factor", DefaultValue = 5.5, MinValue = 0.1)]
        public double StopLossFactor { get; set; }

        [Parameter("Martingale", DefaultValue = 0.5, MinValue = 0)]
        public double MartingaleCoeff { get; set; }

        [Parameter("Max Orders", DefaultValue = 2, MinValue = 2)]
        public int MaxOrders { get; set; }

        #endregion

        private bool isRobotStopped;
        private string botName;
        // le label permet de s'y retrouver parmis toutes les instances possibles.
        private string instanceLabel;

        private double stopLoss;
        private double firstLot;
        private StaticPosition corner_position;
        private string BotVersion = "1.3.2.0";
        private bool DEBUG;


        protected override void OnStart()
        {
            DEBUG = true;
            botName = ToString();
            instanceLabel = botName + "-" + BotVersion + "-" + Symbol.Code + "-" + TimeFrame.ToString();

            stopLoss = TakeProfit * StopLossFactor;
            Positions.Opened += OnPositionOpened;

            int corner = 1;

            switch (corner)
            {
                case 1:
                    corner_position = StaticPosition.TopLeft;
                    break;
                case 2:
                    corner_position = StaticPosition.TopRight;
                    break;
                case 3:
                    corner_position = StaticPosition.BottomLeft;
                    break;
                case 4:
                    corner_position = StaticPosition.BottomRight;
                    break;
            }

            if (!DEBUG)
                ChartObjects.DrawText("BotVersion", botName + " Version : " + BotVersion, corner_position);


            Print("The current symbol has PipSize of: {0}", Symbol.PipSize);
            Print("The current symbol has PipValue of: {0}", Symbol.PipValue);
            Print("The current symbol has TickSize: {0}", Symbol.TickSize);
            Print("The current symbol has TickSValue: {0}", Symbol.TickValue);
        }

        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;

            Position[] positions = GetPositions();

            if (positions.Length > 0 && isRobotStopped)
                return;
            else
                isRobotStopped = false;

            if (positions.Length == 0)
            {
                // Calcule le volume en fonction du money management pour un risque maximum et un stop loss donné.
                // Ne tient pas compte des risques sur d'autres positions ouvertes du compte de trading utilisé
                double maxVolume = this.moneyManagement(MoneyManagement, stopLoss);
                firstLot = maxVolume / (MaxOrders + (MartingaleCoeff * MaxOrders * (MaxOrders - 1)) / 2.0);

                if (firstLot <= 0)
                    throw new System.ArgumentException(String.Format("the 'first lot' : {0} parameter must be positive and not null", firstLot));
                else
                    SendFirstOrder(firstLot);
            }
            else

                ControlSeries();
        }

        protected override void OnError(Error CodeOfError)
        {
            if (CodeOfError.Code == ErrorCode.NoMoney)
            {
                isRobotStopped = true;
                Print("ERROR!!! No money for order open, robot is stopped!");
            }
            else if (CodeOfError.Code == ErrorCode.BadVolume)
            {
                isRobotStopped = true;
                Print("ERROR!!! Bad volume for order open, robot is stopped!");
            }
        }

        private void OnPositionOpened(PositionOpenedEventArgs args)
        {
            double? stopLossPrice = null;
            double? takeProfitPrice = null;

            switch (GetPositionsSide())
            {
                case 0:
                    double averageBuyPrice = GetAveragePrice(TradeType.Buy);
                    takeProfitPrice = averageBuyPrice + TakeProfit * Symbol.PipSize;
                    stopLossPrice = averageBuyPrice - stopLoss * Symbol.PipSize;
                    break;
                case 1:
                    double averageSellPrice = GetAveragePrice(TradeType.Sell);
                    takeProfitPrice = averageSellPrice - TakeProfit * Symbol.PipSize;
                    stopLossPrice = averageSellPrice + stopLoss * Symbol.PipSize;
                    break;
            }

            if (stopLossPrice.HasValue || takeProfitPrice.HasValue)
            {
                Position[] positions = GetPositions();

                foreach (Position position in positions)
                {
                    if (stopLossPrice != position.StopLoss || takeProfitPrice != position.TakeProfit)
                        ModifyPosition(position, stopLossPrice, takeProfitPrice);
                }
            }
        }

        private void SendFirstOrder(double OrderVolume)
        {
            switch (GetSignal())
            {
                case 0:
                    executeOrder(TradeType.Buy, OrderVolume);
                    break;
                case 1:
                    executeOrder(TradeType.Sell, OrderVolume);
                    break;
            }
        }

        private void ControlSeries()
        {
            Position[] positions = GetPositions();

            if (positions.Length < MaxOrders)
            {
                long volume = Symbol.NormalizeVolume(firstLot * (1 + MartingaleCoeff * positions.Length), RoundingMode.ToNearest);
                int countOfBars = (int)(25.0 / positions.Length);

                int pipstep = GetDynamicPipstep(countOfBars, MaxOrders - 1);
                int positionSide = GetPositionsSide();

                switch (positionSide)
                {
                    case 0:
                        double lastBuyPrice = GetLastPrice(TradeType.Buy);

                        if (!DEBUG)
                            ChartObjects.DrawHorizontalLine("gridBuyLine", lastBuyPrice - pipstep * Symbol.PipSize, Colors.Green, 2);

                        if (Symbol.Ask < lastBuyPrice - pipstep * Symbol.PipSize)
                            executeOrder(TradeType.Buy, volume);
                        break;

                    case 1:
                        double lastSellPrice = GetLastPrice(TradeType.Sell);

                        if (!DEBUG)
                            ChartObjects.DrawHorizontalLine("gridSellLine", lastSellPrice + pipstep * Symbol.PipSize, Colors.Red, 2);

                        if (Symbol.Bid > lastSellPrice + pipstep * Symbol.PipSize)
                            executeOrder(TradeType.Sell, volume);
                        break;
                }
            }

            if (!DEBUG)
                ChartObjects.DrawText("MaxDrawdown", "MaxDrawdown: " + Math.Round(GetMaxDrawdown(), 2) + " Percent", corner_position);
        }

        // You can modify the condition of entry here.
        private int GetSignal()
        {
            int Result = -1;
            int LastBarIndex = MarketSeries.Close.Count - 2;
            int PrevBarIndex = LastBarIndex - 1;

            // two up candles for a buy signal.
            if (MarketSeries.Close[LastBarIndex] > MarketSeries.Open[LastBarIndex])
                if (MarketSeries.Close[PrevBarIndex] > MarketSeries.Open[PrevBarIndex])
                    Result = 0;

            // two down candles for a sell signal.
            if (MarketSeries.Close[LastBarIndex] < MarketSeries.Open[LastBarIndex])
                if (MarketSeries.Close[PrevBarIndex] < MarketSeries.Open[PrevBarIndex])
                    Result = 1;

            return Result;
        }

        private TradeResult executeOrder(TradeType tradeType, double volume)
        {
            //Print("normalized volume : {0}", Symbol.NormalizeVolume(volume, RoundingMode.ToNearest));
            return ExecuteMarketOrder(tradeType, Symbol, Symbol.NormalizeVolume(volume, RoundingMode.ToNearest), instanceLabel);
        }

        private Position[] GetPositions()
        {
            return Positions.FindAll(instanceLabel, Symbol);
        }

        private double GetAveragePrice(TradeType TypeOfTrade)
        {
            double Result = Symbol.Bid;
            double AveragePrice = 0;
            long count = 0;

            foreach (Position position in GetPositions())
            {
                if (position.TradeType == TypeOfTrade)
                {
                    AveragePrice += position.EntryPrice * position.Volume;
                    count += position.Volume;
                }
            }

            if (AveragePrice > 0 && count > 0)
                Result = AveragePrice / count;

            return Result;
        }

        private int GetPositionsSide()
        {
            int Result = -1;
            int BuySide = 0, SellSide = 0;
            Position[] positions = GetPositions();

            foreach (Position position in positions)
            {
                if (position.TradeType == TradeType.Buy)
                    BuySide++;

                if (position.TradeType == TradeType.Sell)
                    SellSide++;
            }

            if (BuySide == positions.Length)
                Result = 0;

            if (SellSide == positions.Length)
                Result = 1;

            return Result;
        }

        private int GetDynamicPipstep(int CountOfBars, int division)
        {
            int Result;
            double HighestPrice = 0, LowestPrice = 0;
            int StartBar = MarketSeries.Close.Count - 2 - CountOfBars;
            int EndBar = MarketSeries.Close.Count - 2;

            for (int i = StartBar; i < EndBar; i++)
            {
                if (HighestPrice == 0 && LowestPrice == 0)
                {
                    HighestPrice = MarketSeries.High[i];
                    LowestPrice = MarketSeries.Low[i];
                    continue;
                }

                if (MarketSeries.High[i] > HighestPrice)
                    HighestPrice = MarketSeries.High[i];

                if (MarketSeries.Low[i] < LowestPrice)
                    LowestPrice = MarketSeries.Low[i];
            }

            Result = (int)((HighestPrice - LowestPrice) / Symbol.PipSize / division);

            return Result;
        }

        private double savedMaxBalance;
        private List<double> drawdown = new List<double>();
        private double GetMaxDrawdown()
        {
            savedMaxBalance = Math.Max(savedMaxBalance, Account.Balance);

            drawdown.Add((savedMaxBalance - Account.Balance) / savedMaxBalance * 100);
            drawdown.Sort();

            double maxDrawdown = drawdown[drawdown.Count - 1];

            return maxDrawdown;
        }

        private double GetLastPrice(TradeType tradeType)
        {
            double LastPrice = 0;

            foreach (Position position in GetPositions())
            {
                if (tradeType == TradeType.Buy)
                    if (position.TradeType == tradeType)
                    {
                        if (LastPrice == 0)
                        {
                            LastPrice = position.EntryPrice;
                            continue;
                        }
                        if (position.EntryPrice < LastPrice)
                            LastPrice = position.EntryPrice;
                    }

                if (tradeType == TradeType.Sell)
                    if (position.TradeType == tradeType)
                    {
                        if (LastPrice == 0)
                        {
                            LastPrice = position.EntryPrice;
                            continue;
                        }
                        if (position.EntryPrice > LastPrice)
                            LastPrice = position.EntryPrice;
                    }
            }

            return LastPrice;
        }

    }
}
