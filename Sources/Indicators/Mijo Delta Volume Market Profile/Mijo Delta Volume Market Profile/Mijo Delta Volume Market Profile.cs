// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Indicator. 
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MijoDeltaVolumeMarketProfile : Indicator
    {
        [Parameter(DefaultValue = true)]
        public bool Show_Delta_Volume { get; set; }

        [Output("BidEntries", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries BidResult { get; set; }

        [Output("AskEntries", Color = Colors.Blue, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries AskResult { get; set; }

        MarketDepth _marketDepth;

        private int _askNo;
        private int _bidNo;
        private int index = 0;
        private int old_day = 0;
        private int old_time = 0;
        private int current_time = 0;
        private int start_time = 0;
        private double today_high = 0;
        private double today_low = 999999;
        private double today_bid = 0;
        private double total_pv = 0;
        private double total_pt = 0;
        private int lot_size = 100000;
        private double ask_volume = 0;
        private double bid_volume = 0;
        private double total_volume = 0;
        private double old_askbid_middle = 0;
        private int array_value_index = 1000000;
        private double[] aVolume = new double[1000000];
        private const VerticalAlignment vAlignAskBid = VerticalAlignment.Top;
        private const HorizontalAlignment hAlignAskBid = HorizontalAlignment.Center;
        private const VerticalAlignment vAlignText = VerticalAlignment.Top;
        private const HorizontalAlignment hAlignText = HorizontalAlignment.Right;

        protected override void Initialize()
        {
            //Global Init
            for (var s = 0; s < array_value_index; s++)
            {
                aVolume[s] = 0;
            }
            today_bid = Symbol.Bid;
            old_askbid_middle = ((Symbol.Ask + Symbol.Bid) / 2);
            _marketDepth = MarketData.GetMarketDepth(Symbol);
            index = MarketSeries.Close.Count - 1;
            old_day = MarketSeries.OpenTime[index].DayOfYear;
            start_time = MarketSeries.OpenTime[index].Year * MarketSeries.OpenTime[index].Day * MarketSeries.OpenTime[index].Hour * MarketSeries.OpenTime[index].Millisecond;

            //Cycle
            _marketDepth.Updated += Calc_And_Show_Chart;

        }

        void Calc_And_Show_Chart()
        {
            //Variable
            _askNo = 0;
            _bidNo = 0;
            int set_position = 0;
            double fair_value = 0;
            double total_time = 0;
            double vwap_price = 0;
            double twap_price = 0;
            int fair_value_index = 0;
            double vwap_price_low = 0;
            double vwap_price_high = 0;
            double fairvalue_price = 0;
            double fairvalue_volume = 0;
            double cumulative_volume = 0;
            double Check_Bid_Volume = 0;
            double Check_Ask_Volume = 0;
            double high_range_volume = 0;
            double low_range_volume = 0;
            double highlow_range_volume = 0;
            double symbol_point = Symbol.PointSize;
            int new_index = MarketSeries.Close.Count - 1;
            int CurrentDay = MarketSeries.OpenTime[new_index].DayOfYear;
            /*            
            var marketSeries1440 = MarketData.GetSeries(TimeFrame.Daily);
            int DailyTimeFrameCount = marketSeries1440.Close.Count - 1;
            double today_high = marketSeries1440.High[DailyTimeFrameCount];
            double today_low = marketSeries1440.Low[DailyTimeFrameCount];
            double today_bid = marketSeries1440.Open[DailyTimeFrameCount];
            */

            foreach (var entry in _marketDepth.AskEntries)
            {
                if (entry.Price > today_high)
                {
                    today_high = entry.Price;
                }
            }

            foreach (var entry in _marketDepth.BidEntries)
            {
                if (entry.Price < today_low)
                {
                    today_low = entry.Price;
                }
            }

            double range = ((today_high - today_low) / symbol_point);
            int middle_of_range = (int)Math.Round(range / 2);
            int middle_of_array = array_value_index / 2;
            int start_set_ask = middle_of_array + (int)Math.Round(((Symbol.Ask - today_bid) / symbol_point));
            int start_set_bid = middle_of_array + (int)Math.Round(((Symbol.Bid - today_bid) / symbol_point));
            int start_volume_index = middle_of_array + (int)Math.Round(((today_low - today_bid) / symbol_point));
            int start_index = (new_index - (int)range);
            int ask_value_index = 0;
            int bid_value_index = 0;
            double middle_price = ((today_high + today_low) / 2);
            current_time = MarketSeries.OpenTime[new_index].Year * MarketSeries.OpenTime[new_index].Day * MarketSeries.OpenTime[new_index].Hour * MarketSeries.OpenTime[new_index].Millisecond;

            //Clear array & indicator chart
            for (var i = new_index; i > 0; i--)
            {
                AskResult[i] = 0;
                BidResult[i] = 0;
            }
            /*
            if (CurrentDay != old_day)
            {
                for (var i = 0; i < array_value_index; i++)
                {
                    aVolume[i] = 0;
                }
                today_high =0;
                today_low = 999999;
                today_bid = Symbol.Bid;
                CurrentDay = old_day;
            }*/

            //Calc Array  
            for (var bs = 0; bs < array_value_index; bs++)
            {
            }

            foreach (var entry in _marketDepth.AskEntries)
            {
                double entry_ask_volume = (entry.Volume / lot_size);
                if (Show_Delta_Volume == true)
                {
                    aVolume[start_set_ask + _askNo] = aVolume[start_set_ask + _askNo] + entry_ask_volume;
                }
                else
                {
                    aVolume[start_set_ask + _askNo] = entry_ask_volume;
                }
                vwap_price_high = entry.Price;
                Check_Ask_Volume = Check_Ask_Volume + entry_ask_volume;
                _askNo++;
            }

            foreach (var entry in _marketDepth.BidEntries)
            {
                double entry_bid_volume = (entry.Volume / lot_size);
                if (Show_Delta_Volume == true)
                {
                    aVolume[start_set_bid - _bidNo] = aVolume[start_set_bid - _bidNo] - entry_bid_volume;
                }
                else
                {
                    aVolume[start_set_bid - _bidNo] = (entry_bid_volume * (-1));

                }
                Check_Bid_Volume = Check_Bid_Volume + entry_bid_volume;
                vwap_price_low = entry.Price;
                _bidNo++;
            }

            //Indicator Chart
            if (Check_Ask_Volume <= 0)
            {
                ChartObjects.DrawText("No_Ask_Volume", "No Ask Pending Volume", ((((start_index + new_index) / 2) + new_index) / 2), 0, vAlignAskBid, hAlignAskBid, Colors.Magenta);
            }
            else
            {
                ChartObjects.RemoveObject("No_Ask_Volume");
            }
            if (Check_Bid_Volume <= 0)
            {
                ChartObjects.DrawText("No_Bid_Volume", "No Bid Pending Volume", ((((start_index + new_index) / 2) + start_index) / 2), 0, vAlignAskBid, hAlignAskBid, Colors.Magenta);
            }
            else
            {
                ChartObjects.RemoveObject("No_Bid_Volume");
            }

            if (Check_Ask_Volume > 0 && Check_Bid_Volume > 0)
            {
                for (int c = start_index; c < new_index; c++)
                {
                    double volume = aVolume[start_volume_index];

                    if (start_volume_index > middle_of_array)
                    {
                        highlow_range_volume = highlow_range_volume + Math.Abs(volume);
                    }
                    else
                    {
                        highlow_range_volume = highlow_range_volume - Math.Abs(volume);
                    }

                    if (volume > 0)
                    {
                        AskResult[c] = volume;
                        BidResult[c] = 0;
                    }
                    else
                    {
                        if (volume < 0)
                        {
                            AskResult[c] = 0;
                            BidResult[c] = (volume * (-1));
                        }
                        else
                        {
                            AskResult[c] = 0;
                            BidResult[c] = 0;
                        }
                    }
                    if (Math.Abs(volume) > fair_value)
                    {
                        fair_value_index = c;
                        fair_value = Math.Abs(volume);
                        fairvalue_volume = volume;
                    }
                    start_volume_index++;
                }
            }

            //Calc for Object Chart Text
            ask_value_index = start_index + (int)((vwap_price_high - today_low) / symbol_point);
            bid_value_index = start_index + (int)((vwap_price_low - today_low) / symbol_point);

            ask_volume = ask_volume + Check_Ask_Volume;
            bid_volume = bid_volume + Check_Bid_Volume;
            total_volume = ask_volume + bid_volume;
            set_position = (int)Math.Round(fair_value / 10);
            cumulative_volume = bid_volume - ask_volume;
            fairvalue_price = today_low + ((fair_value_index - start_index) * symbol_point);
            //VWAP
            total_pv += (Check_Ask_Volume + Check_Bid_Volume) * ((vwap_price_high + vwap_price_low + ((Symbol.Bid + Symbol.Ask) / 2)) / 3);
            vwap_price = total_pv / total_volume;
            //TWAP i dont create to work calc in miliseconds to calc diference betwen update time
                        /*if(old_time > 0 && old_askbid_middle > 0)
            {
                total_pt += (current_time - old_time) * ((vwap_price_high + vwap_price_low + old_askbid_middle + ((Symbol.Bid + Symbol.Ask)/2)) /4);
                total_time = current_time - start_time;
                twap_price = total_pt / total_time;
            }
            old_askbid_middle = ((Symbol.Bid + Symbol.Ask)/2);
            old_time = current_time;*/
            /*if(current_time > old_time)
            {*/
total_pt += (current_time - old_time) * ((MarketSeries.Open[new_index - 1] + MarketSeries.High[new_index - 1] + MarketSeries.Low[new_index - 1] + MarketSeries.Close[new_index - 1]) / 4);
            total_time = current_time - start_time;
            if (total_time > 0)
            {
                twap_price = total_pt / total_time;
            }
            else
            {
                twap_price = 0;
            }
            //}
            old_time = current_time;

            //Indicator Object Chart Line
            ChartObjects.DrawVerticalLine("High", new_index, Colors.Magenta, 4, LineStyle.Solid);
            ChartObjects.DrawVerticalLine("Low", start_index, Colors.Green, 4, LineStyle.Solid);
            ChartObjects.DrawVerticalLine("Middle", ((start_index + new_index) / 2), Colors.PapayaWhip, 4, LineStyle.Solid);
            //ChartObjects.DrawVerticalLine("FairValue", fair_value_index, Colors.Yellow, 3, LineStyle.Solid);
            ChartObjects.DrawVerticalLine("Ask", ask_value_index, Colors.Yellow, 2, LineStyle.Solid);
            ChartObjects.DrawVerticalLine("Bid", bid_value_index, Colors.Yellow, 2, LineStyle.Solid);

            //Indicator Object Chart Text
            ChartObjects.DrawText("Sum_Volume", "Sum Volume " + total_volume, new_index + 10, fair_value, vAlignText, hAlignText, Colors.DimGray);
            ChartObjects.DrawText("Ask_Volume", "Ask Volume " + ask_volume, new_index + 10, (fair_value - set_position), vAlignText, hAlignText, Colors.DimGray);
            ChartObjects.DrawText("Bid_Volume", "Bid Volume " + bid_volume, new_index + 10, (fair_value - (set_position * 2)), vAlignText, hAlignText, Colors.DimGray);
            if (highlow_range_volume < 0)
            {
                ChartObjects.DrawText("MiddleRangeVolume", "Buyers & Sellers " + highlow_range_volume, new_index + 10, (fair_value - (set_position * 3)), vAlignText, hAlignText, Colors.RoyalBlue);
            }
            else
            {
                if (highlow_range_volume > 0)
                {
                    ChartObjects.DrawText("MiddleRangeVolume", "Buyers & Sellers " + highlow_range_volume, new_index + 10, (fair_value - (set_position * 3)), vAlignText, hAlignText, Colors.Red);
                }
                else
                {
                    ChartObjects.DrawText("MiddleRangeVolume", "Buyers & Sellers " + highlow_range_volume, new_index + 10, (fair_value - (set_position * 3)), vAlignText, hAlignText, Colors.DimGray);
                }
            }

            if (cumulative_volume < 0)
            {
                ChartObjects.DrawText("Cumulative_Volume", "Cumulative Volume " + cumulative_volume, new_index + 10, (fair_value - (set_position * 4)), vAlignText, hAlignText, Colors.RoyalBlue);
            }
            else
            {
                if (cumulative_volume > 0)
                {
                    ChartObjects.DrawText("Cumulative_Volume", "Cumulative Volume " + cumulative_volume, new_index + 10, (fair_value - (set_position * 4)), vAlignText, hAlignText, Colors.Red);
                }
                else
                {
                    ChartObjects.DrawText("Cumulative_Volume", "Cumulative Volume " + cumulative_volume, new_index + 10, (fair_value - (set_position * 4)), vAlignText, hAlignText, Colors.DimGray);
                }
            }
            if (fairvalue_volume > 0)
            {
                ChartObjects.DrawText("FairValue_Volume", "Fair Value Volume " + fairvalue_volume, new_index + 10, (fair_value - (set_position * 5)), vAlignText, hAlignText, Colors.RoyalBlue);
            }
            else
            {
                if (fairvalue_volume < 0)
                {
                    ChartObjects.DrawText("FairValue_Volume", "Fair Value Volume " + fairvalue_volume, new_index + 10, (fair_value - (set_position * 5)), vAlignText, hAlignText, Colors.Red);
                }
                else
                {
                    ChartObjects.DrawText("FairValue_Volume", "Fair Value Volume " + fairvalue_volume, new_index + 10, (fair_value - (set_position * 5)), vAlignText, hAlignText, Colors.DimGray);
                }
            }
            if (fairvalue_price < vwap_price)
            {
                ChartObjects.DrawText("FairValue_Price", "Fair Value Price @ " + fairvalue_price, new_index + 10, (fair_value - (set_position * 6)), vAlignText, hAlignText, Colors.RoyalBlue);
            }
            else
            {
                if (fairvalue_price > vwap_price)
                {
                    ChartObjects.DrawText("FairValue_Price", "Fair Value Price @ " + fairvalue_price, new_index + 10, (fair_value - (set_position * 6)), vAlignText, hAlignText, Colors.Red);
                }
                else
                {
                    ChartObjects.DrawText("FairValue_Price", "Fair Value Price @ " + fairvalue_price, new_index + 10, (fair_value - (set_position * 6)), vAlignText, hAlignText, Colors.DimGray);
                }
            }

            if (vwap_price > middle_price)
            {
                ChartObjects.DrawText("VWAP_Price", "VWAP Price @ " + vwap_price.ToString("0.#####"), new_index + 10, (fair_value - (set_position * 7)), vAlignText, hAlignText, Colors.RoyalBlue);
            }
            else
            {
                if (vwap_price < middle_price)
                {
                    ChartObjects.DrawText("VWAP_Price", "VWAP Price @ " + vwap_price.ToString("0.#####"), new_index + 10, (fair_value - (set_position * 7)), vAlignText, hAlignText, Colors.Red);
                }
                else
                {
                    ChartObjects.DrawText("VWAP_Price", "VWAP Price @ " + vwap_price.ToString("0.#####"), new_index + 10, (fair_value - (set_position * 7)), vAlignText, hAlignText, Colors.DimGray);
                }
            }

            //ChartObjects.DrawText("TWAP_Price", "TWAP Price @ "+twap_price.ToString("0.#####"), new_index+10, (fair_value-(set_position*7)), vAlignText, hAlignText, Colors.DimGray);            
            ChartObjects.DrawText("High_Price", "High Price @ " + today_high, new_index + 10, (fair_value - (set_position * 8)), vAlignText, hAlignText, Colors.DimGray);
            ChartObjects.DrawText("Middle_Price", "Middle Price @ " + middle_price, new_index + 10, (fair_value - (set_position * 9)), vAlignText, hAlignText, Colors.DimGray);
            ChartObjects.DrawText("Low_Price", "Low Price @ " + today_low, new_index + 10, (fair_value - (set_position * 10)), vAlignText, hAlignText, Colors.DimGray);
        }
        public override void Calculate(int index)
        {
        }
    }
}
