using System;
using cAlgo.API;

namespace cAlgo.Robots
{
    [Robot(AccessRights = AccessRights.None)]
    public class TradingNewsRobotwithTrailingStopLoss : Robot
    {
        private PendingOrder _buyOrder;
        private bool _ordersCreated;
        private PendingOrder _sellOrder;
        private Position position;
                
        [Parameter("News Day (1-5)", DefaultValue = 1, MinValue = 1, MaxValue = 5)]
        public int NewsDay { get; set; }

        [Parameter("News Hour", DefaultValue = 14, MinValue = 0, MaxValue = 23)]
        public int NewsHour { get; set; }

        [Parameter("News Minute", DefaultValue = 30, MinValue = 0, MaxValue = 59)]
        public int NewsMinute { get; set; }

        [Parameter("Pips away", DefaultValue = 10)]
        public int PipsAway { get; set; }

        [Parameter("Take Profit", DefaultValue = 50)]
        public int TakeProfit { get; set; }

        [Parameter("Stop Loss", DefaultValue = 10)]
        public int StopLoss { get; set; }

        [Parameter("Volume", DefaultValue = 100000, MinValue = 10000)]
        public int Volume { get; set; }

        [Parameter("Seconds Before", DefaultValue = 5, MinValue = 1)]
        public int SecondsBefore { get; set; }

        [Parameter("Seconds Timeout", DefaultValue = 10, MinValue = 1)]
        public int SecondsTimeout { get; set; }

        [Parameter("One Cancels Other", DefaultValue = 1, MinValue = 0, MaxValue = 1)]
        public int Oco { get; set; }

        [Parameter("Trigger (pips)", DefaultValue = 20)]
        public int Trigger { get; set; }

        [Parameter("Trailing Stop (pips)", DefaultValue = 10)]
        public int TrailingStop { get; set; }


        protected override void OnStart()
        {
            MarketData.GetMarketDepth(Symbol).Updated += MarketDepth_Updated;
        }

        protected override void OnTick()
        {
            if (position == null) return;
            
            // Trailing
            if(position.TradeType == TradeType.Sell)
            {
                double distance = position.EntryPrice - Symbol.Ask;

                if (distance >= Trigger * Symbol.PipSize)
                {
                    double newStopLossPrice = Symbol.Ask + TrailingStop * Symbol.PipSize;
                    if (position.StopLoss == null || newStopLossPrice < position.StopLoss)
                    {
                        Trade.ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                    }
                }
                
            }
            else
            {
                double distance = Symbol.Bid - position.EntryPrice;

                if (distance >= Trigger * Symbol.PipSize)
                {
                    double newStopLossPrice = Symbol.Bid - TrailingStop * Symbol.PipSize;
                    if (position.StopLoss == null || newStopLossPrice > position.StopLoss)
                    {
                        Trade.ModifyPosition(position, newStopLossPrice, position.TakeProfit);
                    }
                }
            }
        }


        private void MarketDepth_Updated()
        {
            if ((int) Server.Time.DayOfWeek == NewsDay && !_ordersCreated)
            {
                var triggerTime = new DateTime(Server.Time.Year, Server.Time.Month, Server.Time.Day, NewsHour,
                                               NewsMinute, 0);

                if (Server.Time <= triggerTime && (triggerTime - Server.Time).TotalSeconds <= SecondsBefore)
                {
                    _ordersCreated = true;
                    DateTime expirationTime = triggerTime.AddSeconds(SecondsTimeout);

                    double sellOrderTargetPrice = Symbol.Bid - PipsAway*Symbol.PipSize;
                    Trade.CreateSellStopOrder(Symbol, Volume, sellOrderTargetPrice,
                                              sellOrderTargetPrice + StopLoss*Symbol.PipSize,
                                              sellOrderTargetPrice - TakeProfit*Symbol.PipSize, expirationTime);

                    double buyOrderTargetPrice = Symbol.Ask + PipsAway*Symbol.PipSize;
                    Trade.CreateBuyStopOrder(Symbol, Volume, buyOrderTargetPrice,
                                             buyOrderTargetPrice - StopLoss*Symbol.PipSize,
                                             buyOrderTargetPrice + TakeProfit*Symbol.PipSize, expirationTime);
                }
            }
        }

        protected override void OnPendingOrderCreated(PendingOrder newOrder)
        {
            if (newOrder.TradeType == TradeType.Buy)
                _buyOrder = newOrder;
            else
                _sellOrder = newOrder;
        }

        protected override void OnPositionOpened(Position openedPosition)
        {
            position = openedPosition;
            if (Oco == 1)
            {
                Trade.DeletePendingOrder(_buyOrder);
                Trade.DeletePendingOrder(_sellOrder);
                _ordersCreated = false;
            }
        }

        protected override void OnPositionClosed(Position closedPosition)
        {
            position = null;
        }
    }
}