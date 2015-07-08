
using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ActualMarketMakerPendingOrders : Indicator
    {
        [Parameter(DefaultValue = false)]
        public bool Reverz_Buyers_And_Sellers { get; set; }
        
        [Parameter(DefaultValue = false)]
        public bool Reverz_Fair_Value { get; set; }
        
        [Parameter(DefaultValue = false)]
        public bool Reverz_Strength { get; set; }
        
        [Parameter(DefaultValue = 1)]
        public int ShiftRightText { get; set; }
        
        MarketDepth _MarketDepth;
        private double old_ask_volume = 0;
        private double old_bid_volume = 0;
        private double old_sum_volume = 0;
        private double old_askbid_range = 0;
        private double old_fair_value_volume = 0;
        private const VerticalAlignment vAlign = VerticalAlignment.Center;
        private const HorizontalAlignment hAlign = HorizontalAlignment.Right;
        
        
        protected override void Initialize()
        {
            _MarketDepth = MarketData.GetMarketDepth(Symbol);
            _MarketDepth.Updated += OnUpdated;
        }

        void OnUpdated()
        {
            int index = MarketSeries.Close.Count - 1;
            double close = MarketSeries.Close[index];
            double ask_volume = 0;
            double bid_volume = 0;
            double ask_price = 0;
            double bid_price = 0;
            double fair_value_price = 0;
            double fair_value_volume = 0;
            double askbid_middle = ((Symbol.Ask + Symbol.Bid)/2);
            double current_ask_bid_volume = 0;
            double sum_volume = 0;
            double askbid_range = 0;
            bool show_volume_spread = false;
            string buy_name_strength = "Buy Strength";
            string sell_name_strength = "Sell Strength";
                
            //Insert values of Volume
            foreach (var entry in _MarketDepth.AskEntries)
            {
                ask_volume = ask_volume + entry.Volume;
                if(entry.Volume > fair_value_volume )
                {
                    fair_value_volume = entry.Volume;
                    fair_value_price = entry.Price;
                }
                ask_price = entry.Price;
            }

            foreach (var entry in _MarketDepth.BidEntries)
            {
                bid_volume = bid_volume + entry.Volume;
                if(entry.Volume > fair_value_volume )
                {
                    fair_value_volume = entry.Volume;
                    fair_value_price = entry.Price;
                }
                bid_price = entry.Price;
            }
            
            //Calc Volume
            fair_value_volume = fair_value_volume / 10000;
            current_ask_bid_volume = ((ask_volume - bid_volume) / 10000);
            sum_volume = ((ask_volume + bid_volume) / 10000);
            askbid_range = (ask_price - bid_price)/Symbol.PointSize;
            
            if(askbid_range > old_askbid_range && sum_volume > old_sum_volume && old_sum_volume > 0)
            {
                show_volume_spread = true;
            }
            
            //Reverz Volume
            if (Reverz_Buyers_And_Sellers == true)
            {
                current_ask_bid_volume = (current_ask_bid_volume * (-1));
            }
            if (Reverz_Fair_Value == true)
            {
                fair_value_volume = (fair_value_volume * (-1));
            }
            if(Reverz_Strength)
            {
                buy_name_strength = "Sell Strength";
                sell_name_strength = "Buy Strength";
            }
            
            //Show Text on Chart 
            if (current_ask_bid_volume > 0)
            {
                ChartObjects.DrawText("BuyOrSell", Math.Abs(current_ask_bid_volume) + " Buyers", index + ShiftRightText, close, vAlign, hAlign, Colors.Green);
            }
            else
            {
                if (current_ask_bid_volume < 0)
                {
                    ChartObjects.DrawText("BuyOrSell", Math.Abs(current_ask_bid_volume) + " Sellers", index + ShiftRightText, close, vAlign, hAlign, Colors.Magenta);
                }
                else
                {
                    ChartObjects.DrawText("BuyOrSell", current_ask_bid_volume + " Middle", index + ShiftRightText, close, vAlign, hAlign, Colors.Yellow);
                }
            }
            
            if(fair_value_price > askbid_middle)
            {
                ChartObjects.DrawText("FairValue",/*(fair_value_volume - old_fair_value_volume) +*/ "Sell Fair Value", index + ShiftRightText, ask_price, VerticalAlignment.Top, hAlign, Colors.Magenta);
            }
            if(fair_value_price < askbid_middle)
            {
                ChartObjects.DrawText("FairValue",/*(fair_value_volume - old_fair_value_volume) +*/ "Buy Fair Value", index + ShiftRightText, ask_price, VerticalAlignment.Top, hAlign, Colors.Green);
            }
                
            if(show_volume_spread == true)
            {
                if(ask_price - Symbol.Ask < Symbol.Bid - bid_price)
                {
                    ChartObjects.DrawText("VolumeStrength", buy_name_strength, index + ShiftRightText, bid_price, VerticalAlignment.Bottom, hAlign, Colors.Green);
                }
                if(ask_price - Symbol.Ask > Symbol.Bid - bid_price)
                {
                    ChartObjects.DrawText("VolumeStrength", sell_name_strength, index + ShiftRightText, bid_price, VerticalAlignment.Bottom, hAlign, Colors.Magenta);
                }
            }
            
            //Insert values to global variables 
            old_sum_volume = sum_volume;
            old_askbid_range = askbid_range;
            old_fair_value_volume = fair_value_volume;
        }

        public override void Calculate(int index)
        {
        }
    }
}
