// -------------------------------------------------------------------------------------------------
//
//    DataExportTicks
//
//    This cBot is intended to download tick level history to an external CSV File. 
//    Target file can not be open in excel or will fail to open export file.
//
//    Export Historical
//      To use set the Backtest Begin date and End Test Date to reflect desired date range.
//      Set the time frame to 1 tick. 
//      Set the backtest Data to "1 tick Data from Server"
//      Then start the backtest. 
//      Some fields are not populated for backtesting
//
//      Note: Calgo load all the backtest data into memory.  It used over 2.88
//        Gig of memory when I instructed back test to go back to Jan-1 2014.
//        didn't crash but did grind to a halt.  You may want to limit
//        size of request.   It created a size 1.082 Gig. 
//
//
//    Note: Could not test with live data due to weekend.    
//    Export Live Ticks
//      Can be used to download current ticks by runing as an active strategy. 
//      If you open the file in shared mode with a readline() pending it will
//      deliver new ticks as they become available. 
//
//      When using to read data in other processes suggest using fast memory
//      RamDrive or fast SSD. Otherwise may need to use NamedPipe. Our other
//      process did not support windows named pipes so just using a fast SSD.
//
//     Want to collaborate:   www.linkedin.com/pub/joe-ellsworth/0/22/682/ or
//         http://bayesanalytic.com
//
//     No Promises,  No Warranty.  Terms of use MIT http://opensource.org/licenses/MIT
// -------------------------------------------------------------------------------------------------

using System;
using System.Linq;
using System.IO;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FileSystem)]
    public class DataExportTicks : Robot
    {
        // QUESTION:  How do I set default Source Timeframe to T1 (1 tick)
        //[Parameter("TimeFrame ")]
        //public TimeFrame TFrame { get; set; }

        // QUESTION: How do I get Backteset to default to 1 tick from server
        //  so don't have to manually reset?


        [Parameter("MA Type")]
        public MovingAverageType MAType { get; set; }

        [Parameter("Data Dir", DefaultValue = "c:\\download\\calgo")]
        public string DataDir { get; set; }


        private string fiName;
        private System.IO.FileStream fstream;
        private System.IO.StreamWriter fwriter;
        private string csvhead = "date,ask,bid,spread,num_ask,num_bid,vol_adj_ask,vol_adj_bid,vol_adj_spread,vac_ask,vac_bid,vac_bear_vs_bull\n";


        protected override void OnStart()
        {
            var ticktype = MarketSeries.TimeFrame.ToString();
            fiName = DataDir + "\\" + "exp-" + Symbol.Code + "-ticks.csv";
            Print("fiName=" + fiName);

            if (System.IO.File.Exists(fiName) == false)
            {
                // generate new file with CSV header only if
                // one does not already exist. 
                System.IO.File.WriteAllText(fiName, csvhead);
            }

            // had to open file this way to prevent .net from locking it and preventing
            // access by other processes when using to download live ticks.
            fstream = File.Open(fiName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            // setup to append to end of file
            Print("File is Open");
            fstream.Seek(0, SeekOrigin.End);
            // write stream has to be created after seek due to .net wierdness
            // creating with 0 prevents buffering since we want tick data
            // to be available to consumers right away.
            fwriter = new System.IO.StreamWriter(fstream, System.Text.Encoding.UTF8, 1);
            // QUESTION:  How to tell when in Backtest mode so we
            //  can create the stream with a large buffer and turn off 
            // auto flush to improve IO performance. 
            Print("Fwriter is created");
            fwriter.AutoFlush = true;
            // with autoflush true will autocleanup 
            // since we can not close since we may run forever
            Print("done onStart()");
        }

        protected double vol_weighted_price(cAlgo.API.Collections.IReadonlyList<cAlgo.API.MarketDepthEntry> mkentries)
        {
            double weightdiv = 0.0;
            double tsum = 0.0;
            for (int i = 0; i < mkentries.Count; i++)
            {
                var aent = mkentries[i];
                tsum += (double)aent.Price * (double)aent.Volume;
                weightdiv += (double)aent.Volume;
            }
            if (weightdiv == 0)
            {
                return 0;
            }
            else
            {
                return tsum / weightdiv;
            }
        }


        protected double vol_weighted_cnt(cAlgo.API.Collections.IReadonlyList<cAlgo.API.MarketDepthEntry> mkentries)
        {
            double weightdiv = 0.0;
            double tsum = 0.0;
            for (int i = 0; i < mkentries.Count; i++)
            {
                var aent = mkentries[i];
                tsum += (double)aent.Volume * (double)aent.Price;
                weightdiv += (double)aent.Price;
            }
            if (weightdiv == 0)
            {
                return 0;
            }
            else
            {
                return tsum / weightdiv;
            }
        }

        protected override void OnTick()
        {
            var sa = new System.Collections.Generic.List<string>();
            var barTime = MarketSeries.OpenTime.LastValue;
            var timestr = barTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

            MarketDepth mkdepth = MarketData.GetMarketDepth(Symbol.Code);
            var volAdjAsk = vol_weighted_price(mkdepth.AskEntries);
            var volAdjBid = vol_weighted_price(mkdepth.BidEntries);
            var volAdjSpread = volAdjAsk - volAdjBid;
            var volCntAsk = vol_weighted_cnt(mkdepth.AskEntries) / 100000.0;
            var volCntBid = vol_weighted_cnt(mkdepth.BidEntries) / 100000.0;
            var vonCntBuyVsSell = volCntAsk - volCntBid;

            if ((volAdjAsk == 0.0) && (volAdjBid == 0.0))
            {
                return;
            }

            sa.Add(timestr);
            sa.Add(Symbol.Ask.ToString("F6"));
            sa.Add(Symbol.Bid.ToString("F6"));
            sa.Add(Symbol.Spread.ToString("F6"));
            sa.Add(mkdepth.AskEntries.Count.ToString("F2"));
            sa.Add(mkdepth.BidEntries.Count.ToString("F2"));
            sa.Add(volAdjAsk.ToString("F6"));
            sa.Add(volAdjBid.ToString("F6"));
            sa.Add(volAdjSpread.ToString("F6"));
            sa.Add(volCntAsk.ToString("F2"));
            sa.Add(volCntBid.ToString("F2"));
            sa.Add(vonCntBuyVsSell.ToString("F2"));

            var sout = string.Join(",", sa);
            //System.IO.File.AppendAllText(fiName, sout);
            fwriter.WriteLine(sout);
            fwriter.Flush();

        }


        protected override void OnStop()
        {
            Print("OnStop()");
            fwriter.Close();
            fstream.Close();
            // Put your deinitialization logic here
        }
    }
}
