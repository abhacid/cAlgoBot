// ---------------------------------------------------------------------------------------
//
//    CSV reader to open and close market orders  
//    mt4 writes to file 
//    version1 
//    lotsize calculation fixed _july1_siamfx
//
// ---------------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{

    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]
    //AccessRights.FullAccess

    public class MT2cTrader : Robot
    {

        [Parameter("Orders Input File Path", DefaultValue = "C:\\Users\\trader\\AppData\\Roaming\\MetaQuotes\\Terminal\\69420FB8433504FEA0FA029C390238DB\\MQL4\\Files\\TradeCopy.csv")]
        // C:\\Users\\trader\\CSV\\TradeCopy.csv

        public string orders_input_file { get; set; }

        [Parameter("Slippage", DefaultValue = 3.5)]
        public double slippage { get; set; }


        [Parameter("Delimiter", DefaultValue = ";")]
        public string delimiter { get; set; }

        protected override void OnStart()
        {

        }

        private bool debug = true;

        protected override void OnTick()
        {
            //todo, check M.D.
            //price = marketDepth.AskEntries[0].Price; 
            //volume = marketDepth.AskEntries[0].Volume;

            string[] lines = new String[0];

            try
            {
                lines = File.ReadAllLines(orders_input_file);

            } catch (Exception e)
            {
                Print("Exception: " + e.Message);
            }

            List<string> existing_positions = new List<string>();

            foreach (string line in lines)
            {

                OrderData order = new OrderData(line.Split(delimiter.Length > 0 ? delimiter[0] : ','), MarketData);
                existing_positions.Add(order.label);

                if (debug)
                    Print(line);

                if (order.isCorrect() && (Positions.Find(order.label) == null))
                    ExecuteMarketOrder(order.type, order.symbol, order.lot, order.label, order.sl, order.tp, slippage);
            }

            for (int pos = 0; pos < Positions.Count; pos++)
                if (!existing_positions.Contains(Positions[pos].Label))
                    ClosePosition(Positions[pos]);
        }

    }

    public class OrderData
    {

        private const long mt_lot_coefficient = 100000;
        //corrected_100000_july1
        public Symbol symbol;
        public TradeType type;
        public long lot;
        public int sl;
        public int tp;
        public string label;
        private bool initialized_properly = true;

        public OrderData(string[] raw_pieces, MarketData market_data)
        {
            try
            {
                this.label = raw_pieces[0].Trim();
                this.symbol = market_data.GetSymbol(raw_pieces[1].Trim());
                this.setType(Convert.ToInt32(raw_pieces[2].Trim()));
                this.lot = Convert.ToInt64(this.parseDouble(raw_pieces[3]) * mt_lot_coefficient);
                double price = this.parseDouble(raw_pieces[4]);
                this.sl = this.getPipDistance(price, this.parseDouble(raw_pieces[5]));
                this.tp = this.getPipDistance(price, this.parseDouble(raw_pieces[6]));
            } catch (Exception e)
            {
                this.initialized_properly = false;
            }
        }

        public bool isCorrect()
        {
            return this.initialized_properly;
        }

        private double parseDouble(string value)
        {
            return double.Parse(value.Trim().Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
        }





        private void setType(int mt_type)
        {
            this.type = mt_type == 0 ? TradeType.Buy : TradeType.Sell;
        }

        private int getPipDistance(double basic_price, double close_price)
        {
            return Convert.ToInt32(Math.Round(Math.Abs(basic_price - close_price) / this.symbol.PipSize));
        }

    }

}
