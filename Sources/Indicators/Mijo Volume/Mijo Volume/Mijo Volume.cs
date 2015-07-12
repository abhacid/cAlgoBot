
using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Volume : Indicator
    {
        [Output("Ask Volume", Color = Colors.Blue, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries ask_volume { get; set; }

        [Output("Bid Volume", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries bid_volume { get; set; }

        [Output("Buy Volume Strength", Color = Colors.Green, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries Buy_Volume_Strength { get; set; }

        [Output("Sell Volume Strength", Color = Colors.Magenta, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries Sell_Volume_Strength { get; set; }

        [Output("No Volume Strength", Color = Colors.Yellow, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries No_Volume_Strength { get; set; }

        MarketDepth _MarketDepth;

        private int old_index = 0;
        private double sum_ask_volume = 0;
        private double sum_bid_volume = 0;
        private double high_ask_price = 0;
        private double low_bid_price = 999999;
        private double lot_size = 100000;
        private double highlow_range = 0;
        private double sum_total_volume = 0;
        private double old_sum_total_volume = 0;
        private double old_highlow_range = 0;
        private double Symbol_PointSize = 0;

        private const VerticalAlignment vAlign = VerticalAlignment.Center;
        private const HorizontalAlignment hAlign = HorizontalAlignment.Right;


        protected override void Initialize()
        {
            old_index = MarketSeries.Close.Count - 1;
            Symbol_PointSize = Symbol.PointSize;
            _MarketDepth = MarketData.GetMarketDepth(Symbol);

            _MarketDepth.Updated += OnUpdated;
        }

        void OnUpdated()
        {
            int index = MarketSeries.Close.Count - 1;
            double sum_ask_bid_volume = 0;

            //Insert values to global variables 
            if (index != old_index)
            {
                ask_volume[index] = 0;
                bid_volume[index] = 0;
                Buy_Volume_Strength[index] = 0;
                Sell_Volume_Strength[index] = 0;
                old_index = index;
                sum_ask_volume = 0;
                sum_bid_volume = 0;
                high_ask_price = 0;
                low_bid_price = 999999;
                old_sum_total_volume = sum_total_volume;
                old_highlow_range = highlow_range;
            }

            //Insert values of Volume
            foreach (var entry in _MarketDepth.AskEntries)
            {
                sum_ask_volume = sum_ask_volume + (entry.Volume / lot_size);
                if (high_ask_price < entry.Price)
                {
                    high_ask_price = entry.Price;
                }
            }

            foreach (var entry in _MarketDepth.BidEntries)
            {
                sum_bid_volume = sum_bid_volume + (entry.Volume / lot_size);
                if (low_bid_price > entry.Price)
                {
                    low_bid_price = entry.Price;
                }
            }

            //Calc Volume
            sum_ask_bid_volume = (sum_ask_volume - sum_bid_volume);
            sum_total_volume = (sum_ask_volume + sum_bid_volume);
            highlow_range = (high_ask_price - low_bid_price) / Symbol_PointSize;

            //Show Volume Indicator

            if (sum_ask_bid_volume >= 0)
            {
                ask_volume[index] = sum_total_volume;
                bid_volume[index] = 0;
            }
            else
            {
                ask_volume[index] = 0;
                bid_volume[index] = sum_total_volume;
            }
            if (old_sum_total_volume > 0 && sum_total_volume > old_sum_total_volume && highlow_range > old_highlow_range)
            {
                if (high_ask_price - Symbol.Ask > Symbol.Bid - low_bid_price)
                {
                    Buy_Volume_Strength[index] = 0;
                    Sell_Volume_Strength[index] = (sum_total_volume * (-1));
                    No_Volume_Strength[index] = 0;
                }
                if (high_ask_price - Symbol.Ask < Symbol.Bid - low_bid_price)
                {
                    Buy_Volume_Strength[index] = (sum_total_volume * (-1));
                    Sell_Volume_Strength[index] = 0;
                    No_Volume_Strength[index] = 0;
                }
            }
            else
            {
                Buy_Volume_Strength[index] = 0;
                Sell_Volume_Strength[index] = 0;
                No_Volume_Strength[index] = (sum_total_volume * (-1));
            }
        }

        public override void Calculate(int index)
        {
        }
    }
}
