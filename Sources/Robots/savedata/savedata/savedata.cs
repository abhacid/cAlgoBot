using System;
using System.Linq;
using System.IO;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

/* code modified from http://ctdn.com/algos/cbots/show/588 
 * 
 * when using tick data make sure you change Data in backtesting options to tick data from server
 * 
 * Original License:
 * Want to collaborate:     www.linkedin.com/pub/joe-ellsworth/0/22/682/ or     http://bayesanalytic.com
 * No Promises,  No Warranty.  Terms of use MIT http://opensource.org/licenses/MIT
*/

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FileSystem)]
    public class savedata : Robot
    {
        [Parameter("Data Dir", DefaultValue = "C:\\Users\\username\\")]
        public string DataDir { get; set; }

        private string fiName;
        private System.IO.FileStream fstream;
        private System.IO.StreamWriter fwriter;
        private bool BarChart = true; // if false is tick chart

        protected override void OnStart()
        {
            var ticktype = MarketSeries.TimeFrame.ToString();
            if (ticktype.Contains("Tick"))
                BarChart = false;
            fiName = DataDir + "\\" + Symbol.Code + "-" + ticktype + ".csv";

            string csvhead = BarChart ? "date,open,high,low,close" : "date,ask,bid";
            if (System.IO.File.Exists(fiName) == false)
                System.IO.File.WriteAllText(fiName, csvhead);

            // had to open file this way to prevent .net from locking it and preventing
            // access by other processes when using to download live ticks.
            fstream = File.Open(fiName, FileMode.Open, FileAccess.Write, FileShare.ReadWrite);
            // setup to append to end of file
            fstream.Seek(0, SeekOrigin.End);
            // write stream has to be created after seek due to .net wierdness
            // creating with 0 prevents buffering since we want tick data
            // to be available to consumers right away.
            fwriter = new System.IO.StreamWriter(fstream, System.Text.Encoding.UTF8, 1);
            fwriter.WriteLine();
            // QUESTION:  How to tell when in Backtest mode so we
            //  can create the stream with a large buffer and turn off 
            // auto flush to improve IO performance. 
            fwriter.AutoFlush = true;
            // with autoflush true will autocleanup 
            // since we can not close since we may run forever
        }

        protected override void OnTick()
        {
            if (!BarChart)
            {
                var sa = new System.Collections.Generic.List<string>();
                var barTime = MarketSeries.OpenTime.LastValue;
                var timestr = barTime.ToString("yyyy-MM-dd HH:mm:ss.fff");

                sa.Add(timestr);
                sa.Add(Symbol.Ask.ToString("F6"));
                sa.Add(Symbol.Bid.ToString("F6"));

                var sout = string.Join(",", sa);
                fwriter.WriteLine(sout);
                fwriter.Flush();
            }
        }

        protected override void OnBar()
        {
            if (BarChart)
            {
                var sa = new System.Collections.Generic.List<string>();
                var barTime = MarketSeries.OpenTime.LastValue;
                var timestr = barTime.ToString("yyyy-MM-dd HH:mm:ss.fff");
                sa.Add(timestr);
                sa.Add(MarketSeries.Open[MarketSeries.Close.Count - 2].ToString("F6"));
                sa.Add(MarketSeries.High[MarketSeries.Close.Count - 2].ToString("F6"));
                sa.Add(MarketSeries.Low[MarketSeries.Close.Count - 2].ToString("F6"));
                sa.Add(MarketSeries.Close[MarketSeries.Close.Count - 2].ToString("F6"));

                var sout = string.Join(",", sa);
                fwriter.WriteLine(sout);
                fwriter.Flush();
            }
        }

        protected override void OnStop()
        {
            fwriter.Close();
            fstream.Close();
        }
    }
}
