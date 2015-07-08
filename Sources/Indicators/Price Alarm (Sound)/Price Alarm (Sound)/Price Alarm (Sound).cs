using System;
using System.IO;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AutoRescale = false, AccessRights = AccessRights.FullAccess)]
    public class PriceAlarmSound : Indicator
    {
        const Colors AskColor = Colors.DeepSkyBlue;
        const Colors BidColor = Colors.Red;

        [Parameter()]
        public double Price { get; set; }

        [Parameter("Spot price (Bid: 1, Ask: 2)", DefaultValue = 1, MinValue = 1, MaxValue = 2)]
        public double BidOrAsk { get; set; }

        [Output("Bid Target", Color = BidColor, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries BidTarget { get; set; }

        [Output("Ask Target", Color = AskColor, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries AskTarget { get; set; }

        [Output("Played Notification", Color = Colors.Gray, LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries PlayedNotificationLine { get; set; }

        bool spotPriceWasAbove;
        int tickCount;
        bool triggered;

        static HashSet<Notification> PlayedNotifications = new HashSet<Notification>();

        protected override void Initialize()
        {
            if (Price == 0)
                MarketData.GetMarketDepth(Symbol).Updated += AnimateWarning;

            if (BidOrAsk == 1)
                spotPriceWasAbove = Symbol.Bid > Price;
            else
                spotPriceWasAbove = Symbol.Ask > Price;

            if (NotificationWasPlayed())
            {
                triggered = true;
            }
        }

        private void AnimateWarning()
        {
            if (tickCount++ % 2 == 0)
                ChartObjects.DrawText("warning", "Please specify the Price", StaticPosition.Center, Colors.Red);
            else
                ChartObjects.DrawText("warning", "Please specify the Price", StaticPosition.Center, Colors.White);
        }

        public override void Calculate(int index)
        {
            if (triggered)
            {
                PlayedNotificationLine[index] = Price;
                return;
            }

            if (BidOrAsk == 1)
                BidTarget[index] = Price;
            else
                AskTarget[index] = Price;

            if (IsRealTime)
            {
                var color = BidOrAsk == 1 ? BidColor : AskColor;
                var spotPrice = BidOrAsk == 1 ? Symbol.Bid : Symbol.Ask;
                var distance = Math.Round(Math.Abs(Price - spotPrice) / Symbol.PipSize, 1);

                ChartObjects.DrawText("distance", " " + distance + " pips left", index, Price, VerticalAlignment.Center, HorizontalAlignment.Right, color);
                if (spotPriceWasAbove && spotPrice <= Price || !spotPriceWasAbove && spotPrice >= Price)
                {
                    var windowsFolder = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
                    Notifications.PlaySound(Path.Combine(windowsFolder, "Media", "tada.wav"));
                    triggered = true;
                    FillLine(BidTarget, double.NaN);
                    FillLine(AskTarget, double.NaN);
                    FillLine(PlayedNotificationLine, Price);
                    ChartObjects.RemoveObject("distance");
                    PlayedNotifications.Add(CreateNotification());
                }
            }
        }

        private void FillLine(IndicatorDataSeries dataSeries, double price)
        {
            for (var i = 0; i < MarketSeries.Close.Count; i++)
                dataSeries[i] = price;
        }

        private bool NotificationWasPlayed()
        {
            return PlayedNotifications.Contains(CreateNotification());
        }

        private Notification CreateNotification()
        {
            return new Notification 
            {
                Symbol = Symbol.Code,
                Price = Price,
                BidIsSpotPrice = BidOrAsk == 1
            };
        }
    }

    struct Notification
    {
        public string Symbol;
        public double Price;
        public bool BidIsSpotPrice;
    }
}
