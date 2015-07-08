using cAlgo.API;
using System;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AutoRescale = false, AccessRights = AccessRights.None, ScalePrecision = 1)]
    public class Level2 : Indicator
    {
        MarketDepth GBPUSD;
        private int _askNo;
        private int _bidNo;
        [Parameter("Line thickness", DefaultValue = 1.5)]
        public double lineThickness { get; set; }
        [Parameter("Line len(bars)", DefaultValue = 35)]
        public int lineLen { get; set; }

        protected override void Initialize()
        {
            GBPUSD = MarketData.GetMarketDepth(Symbol);
            GBPUSD.Updated += OnGbpUsdUpdated;
        }
        void OnGbpUsdUpdated()
        {
            float sumBid = 0;
            float sumAsk = 0;
            _askNo = 0;
            _bidNo = 0;
            var index = MarketSeries.Close.Count - 1;
            for (int i = 0; i < 100; i++)
            {
                ChartObjects.RemoveObject(i.ToString() + "ask");
                ChartObjects.RemoveObject(i.ToString() + "bid");
            }
            foreach (var entry in GBPUSD.AskEntries)
            {
                sumAsk += entry.Volume;
            }
            foreach (var entry in GBPUSD.BidEntries)
            {
                sumBid += entry.Volume;
            }
            string s = "";
            foreach (var entry in GBPUSD.AskEntries)
            {
                double val = entry.Volume / sumAsk;
                int len = Math.Max(1, (int)(lineLen * val));
                ChartObjects.DrawLine(_askNo.ToString() + "ask", index + 1, entry.Price, index + len, entry.Price, Colors.Red, lineThickness);
                s += "\n ask " + len.ToString();
                _askNo++;
            }
            foreach (var entry in GBPUSD.BidEntries)
            {
                double val = entry.Volume / sumBid;
                int len = Math.Max(1, (int)(lineLen * val));
                ChartObjects.DrawLine(_bidNo.ToString() + "bid", index + 1, entry.Price, index + len, entry.Price, Colors.Blue, lineThickness);
                s += "\n bid " + len.ToString();
                _bidNo++;
            }
        }

        public override void Calculate(int index)
        {
            OnGbpUsdUpdated();
        }
    }
}
