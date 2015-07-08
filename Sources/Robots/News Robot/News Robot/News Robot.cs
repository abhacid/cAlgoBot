using System;
using System.Linq;
using System.Text;
using cAlgo.API;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class NewsRobot : Robot
    {
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

        [Parameter("Seconds Before", DefaultValue = 10, MinValue = 1)]
        public int SecondsBefore { get; set; }

        [Parameter("Seconds Timeout", DefaultValue = 10, MinValue = 1)]
        public int SecondsTimeout { get; set; }

        [Parameter("One Cancels Other")]
        public bool Oco { get; set; }

        [Parameter("ShowTimeLeftNews", DefaultValue = false)]
        public bool ShowTimeLeftToNews { get; set; }

        [Parameter("ShowTimeLeftPlaceOrders", DefaultValue = true)]
        public bool ShowTimeLeftToPlaceOrders { get; set; }

        private bool _ordersCreated;

        private DateTime _triggerTimeInServerTimeZone;

        private const string Label = "News Robot";

        protected override void OnStart()
        {
            Positions.Opened += OnPositionOpened;
            Timer.Start(1);

            var triggerTimeInLocalTimeZone = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, NewsHour, NewsMinute, 0);
            if (triggerTimeInLocalTimeZone < DateTime.Now)
                triggerTimeInLocalTimeZone = triggerTimeInLocalTimeZone.AddDays(1);
            _triggerTimeInServerTimeZone = TimeZoneInfo.ConvertTime(triggerTimeInLocalTimeZone, TimeZoneInfo.Local, TimeZone);
        }

        protected override void OnTimer()
        {
            var remainingTime = _triggerTimeInServerTimeZone - Server.Time;
            DrawRemainingTime(remainingTime);

            if (!_ordersCreated)
            {
                var sellOrderTargetPrice = Symbol.Bid - PipsAway * Symbol.PipSize;
                ChartObjects.DrawHorizontalLine("sell target", sellOrderTargetPrice, Colors.Red, 1, LineStyle.DotsVeryRare);
                var buyOrderTargetPrice = Symbol.Ask + PipsAway * Symbol.PipSize;
                ChartObjects.DrawHorizontalLine("buy target", buyOrderTargetPrice, Colors.Blue, 1, LineStyle.DotsVeryRare);

                if (Server.Time <= _triggerTimeInServerTimeZone && (_triggerTimeInServerTimeZone - Server.Time).TotalSeconds <= SecondsBefore)
                {
                    _ordersCreated = true;
                    var expirationTime = _triggerTimeInServerTimeZone.AddSeconds(SecondsTimeout);

                    PlaceStopOrder(TradeType.Sell, Symbol, Volume, sellOrderTargetPrice, Label, StopLoss, TakeProfit, expirationTime);
                    PlaceStopOrder(TradeType.Buy, Symbol, Volume, buyOrderTargetPrice, Label, StopLoss, TakeProfit, expirationTime);

                    ChartObjects.RemoveObject("sell target");
                    ChartObjects.RemoveObject("buy target");
                }
            }

            if (_ordersCreated && !PendingOrders.Any(o => o.Label == Label))
            {
                Print("Orders expired");
                Stop();
            }
        }

        private void DrawRemainingTime(TimeSpan remainingTimeToNews)
        {
            if (ShowTimeLeftToNews)
            {
                if (remainingTimeToNews > TimeSpan.Zero)
                {
                    ChartObjects.DrawText("countdown1", "Time left to news: " + FormatTime(remainingTimeToNews), StaticPosition.TopLeft);
                }
                else
                {
                    ChartObjects.RemoveObject("countdown1");
                }
            }
            if (ShowTimeLeftToPlaceOrders)
            {
                var remainingTimeToOrders = remainingTimeToNews - TimeSpan.FromSeconds(SecondsBefore);
                if (remainingTimeToOrders > TimeSpan.Zero)
                {
                    ChartObjects.DrawText("countdown2", "Time left to place orders: " + FormatTime(remainingTimeToOrders), StaticPosition.TopRight);
                }
                else
                {
                    ChartObjects.RemoveObject("countdown2");
                }
            }
        }

        private static StringBuilder FormatTime(TimeSpan remainingTime)
        {
            var remainingTimeStr = new StringBuilder();
            if (remainingTime.TotalHours >= 1)
                remainingTimeStr.Append((int)remainingTime.TotalHours + "h ");
            if (remainingTime.TotalMinutes >= 1)
                remainingTimeStr.Append(remainingTime.Minutes + "m ");
            if (remainingTime.TotalSeconds > 0)
                remainingTimeStr.Append(remainingTime.Seconds + "s");
            return remainingTimeStr;
        }

        private void OnPositionOpened(PositionOpenedEventArgs args)
        {
            var position = args.Position;
            if (position.Label == Label && position.SymbolCode == Symbol.Code)
            {
                if (Oco)
                {
                    foreach (var order in PendingOrders)
                    {
                        if (order.Label == Label && order.SymbolCode == Symbol.Code)
                        {
                            CancelPendingOrderAsync(order);
                        }
                    }
                }
                Stop();
            }
        }
    }
}
