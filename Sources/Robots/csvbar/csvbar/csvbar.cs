// -------------------------------------------------------------------------------------------------
//    DataExportBars
//
//    This cBot is intended to download Bar level history to an external CSV File. 
//    Target file can not be open in excel or will fail to open export file.  It can
//    also be used to export live data when made Active during a trading session. 
//    Intended to allow shared read by other processes while this session is live which
//    will allow external systems to analyze the data in near realtime. 
//
//    Export Historical
//      To use set the Backtest Begin date and End Test Date to reflect desired date range.
//      Set the time frame to Desired Bar Size
//      Set the backtest Data to "Bars from Server"
//      SElect the time frame desired for the extract.
//      Then start the backtest. 
//
//      Important:
//         To get accurate volume you must choose "Tick Data from Server (Accurate)"
//         in the back test window.  Then you can choose the desired bar resolution
//         in the the Bar resolution in the instance settings.  
//        
//
//    Tested with live data due the week of 12/10/2014 and it seems to work just fine.
//    
//    Can be used to download current Bars by runing as an active strategy. 
//      If you open the file in shared mode with a readline() pending it will
//      deliver new bars as they become available but will always be 1 bar behind
//      because it has to wait until the bar is complete.   If you need data
//      faster use a shorter bar or consider DataExportTicks  
//
//      When using to read data in other processes suggest using fast memory
//      RamDrive or fast SSD. Otherwise may need to use NamedPipe. Our other
//      technology running in a separate  process did not support windows 
//      named pipes so just using a fast SSD and seems to work.  Will eventually
//      need to add function which detects when there is a new hour and open
//      new file so we do grow the files too large. 
//
//     Note: If you load the CSV files in excell and the save again it will mess 
//      up the dateTime unless you set the Excel dat format for the column. 
//      to:  Date -> 3/14/12 13:30
//
//     Want to collaborate:   www.linkedin.com/pub/joe-ellsworth/0/22/682/ or
//         http://bayesanalytic.com
//
//     No Promises,  No Warranty.  Terms of use MIT http://opensource.org/licenses/MIT
//
// -------------------------------------------------------------------------------------------------

using System;
using System.IO;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FileSystem)]
    public class DataExportBars : Robot
    {
        [Parameter("MA Source")]
        public DataSeries Source { get; set; }

        [Parameter("RSI Period", DefaultValue = 14)]
        public int RSIPeriod { get; set; }

        [Parameter("StdDev Period", DefaultValue = 100)]
        public int StdDevPeriod { get; set; }


        [Parameter("Data Dir", DefaultValue = "c:\\download\\calgo")]
        public string DataDir { get; set; }

        const string dateFormat = "yyyy-MM-dd HH:mm:ss.fff";
        private RelativeStrengthIndex rsi;
        private StandardDeviation stdDev;
        private string fiName;
        private System.IO.FileStream fstream;
        private System.IO.StreamWriter fwriter;
        private string csvhead = "datetime,open,close,high,low,volume,weighted_val,rsi,stddev,spread\n";
        private string maxDate = "";
        private bool isFirstBar = true;
        // max date processed so far.
        // We do not want to re-write dates that are already in the file so 
        //  we scan to the last line in the file and record that date times.
        //  when running in history mode we simply skip any bars that occur
        //  before last date.
        protected string get_most_recent_date(string fiName)
        {
            Print("start get_most_recent_date");
            string maxDateRead = "";
            if (System.IO.File.Exists(fiName) == false)
            {
                return "";
            }
            using (var readStream = File.Open(fiName, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                // setup to append to end of file
                Print("grd File is Open");
                readStream.Seek(readStream.Length - 5000, SeekOrigin.Begin);
                // We know that our average bar line is under 120 bytes so skipping back
                // by 2K bytes gives us plenty of margin to read the last line without 
                // readin the entire file.                 
                var freader = new System.IO.StreamReader(readStream, System.Text.Encoding.UTF8);
                Print("grd Got freader for " + fiName);
                while (true)
                {
                    string fline = freader.ReadLine();
                    if (fline == null)
                    {
                        break;
                    }
                    fline = fline.Trim();
                    string[] tflds = fline.Split(',');
                    string currDate = tflds[0].Trim();
                    if (currDate.Length > 20)
                    {
                        // minimum DateTime is 20 bytes so must not be
                        // a datetime if it is less.  Should probably have
                        // a more sophisticated check. 
                        if (currDate.CompareTo(maxDateRead) > 0)
                        {
                            maxDateRead = currDate;
                        }
                    }
                }
                // while                    
                readStream.Close();
                // using statement above will close stream anyway
                Print("grd returning " + maxDateRead);
                return maxDateRead;
            }
            // using
        }
        // func()

        protected override void OnStart()
        {
            // demonstrate using indicators easier to compute in pre-build cAlgo
            // code than to compute in the external system. 
            //rsiBase = Indicators.RelativeStrengthIndex(DataSeries, RSIPeriod);
            rsi = Indicators.RelativeStrengthIndex(Source, RSIPeriod);
            stdDev = Indicators.StandardDeviation(Source, RSIPeriod, MovingAverageType.Simple);
            var ticktype = MarketSeries.TimeFrame.ToString();
            fiName = DataDir + "\\" + "exp-" + Symbol.Code + "-" + ticktype + "-rsi" + RSIPeriod + "-stddev" + StdDevPeriod + "-bars.csv";
            Print("fiName=" + fiName);

            if (System.IO.Directory.Exists(DataDir) == false)
            {
                System.IO.Directory.CreateDirectory(DataDir);
            }

            if (System.IO.File.Exists(fiName) == false)
            {
                // generate new file with CSV header only if
                // one does not already exist. 
                System.IO.File.WriteAllText(fiName, csvhead);
            }
            else
            {
                maxDate = get_most_recent_date(fiName);
            }
            Print("maxDate=" + maxDate);

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


        protected void save_bar(int last_minus)
        {
            var sa = new System.Collections.Generic.List<string>();
            var barTime = MarketSeries.OpenTime.Last(last_minus);
            var timestr = barTime.ToString(dateFormat);
            if (timestr.CompareTo(maxDate) > 0)
            {
                // only save if our date is newer than what 
                // we already have. 
                maxDate = timestr;
                // had to use last(1) because the onBar is called
                // when the bar is first formed. Otherwise it would 
                // return incomplete bars when running live.                 
                sa.Add(timestr);
                sa.Add(MarketSeries.Open.Last(last_minus).ToString("F6"));
                sa.Add(MarketSeries.Close.Last(last_minus).ToString("F6"));
                sa.Add(MarketSeries.High.Last(last_minus).ToString("F6"));
                sa.Add(MarketSeries.Low.Last(last_minus).ToString("F6"));
                // QUESTION:  HOw to Get the actual Buy / sell volume
                //   Rather than number of Ticks.  Both may be useful
                //   But the transaction volume is a critical component
                //   for many indicators.            
                sa.Add(MarketSeries.TickVolume.Last(last_minus).ToString("F6"));
                sa.Add(MarketSeries.WeightedClose.Last(last_minus).ToString("F6"));
                sa.Add(rsi.Result.Last(last_minus).ToString("F6"));
                sa.Add(stdDev.Result.Last(last_minus).ToString("F6"));
                sa.Add(Symbol.Spread.ToString("F6"));
                var sout = string.Join(",", sa);
                fwriter.WriteLine(sout);
            }
        }

        // Try to export recent bars to fill in the gap between
        // backtest and live running data. EG:  Backtest stoped on 
        // 12/19 but we have new bars that have occured on 12/21 when
        // starting live.   Not including the bars from earlier today
        // will skew most recent statistics. 
        protected void process_earlier_bars()
        {
            Print("process_earlier_bars last_date_in_file=" + maxDate);
            if (maxDate == "")
            {
                // brand new file so do not need to worry about gaps
                Print("maxDate is empty in process_earlier_bars");
            }
            else
            {
                Print("checking earlier bars last Date in File=" + maxDate);
                var lastBarTime = MarketSeries.OpenTime.Last(1);
                var priorBarTime = MarketSeries.OpenTime.Last(2);
                var lastSavedDate = System.DateTime.ParseExact(maxDate, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
                double secondsPerBar = (lastBarTime - priorBarTime).TotalSeconds;
                double missingseconds = (lastBarTime - lastSavedDate).TotalSeconds;
                int missingbars = (int)(missingseconds / secondsPerBar) + 1;
                Print("secondsPerBar=" + secondsPerBar + " missingseconds= " + missingseconds + " missingbars=" + missingbars);

                var fillBarTime = MarketSeries.OpenTime.Last(missingbars);
                Print("fillBarTime=" + fillBarTime.ToUniversalTime());
                for (int last_minus = missingbars; last_minus > 1; last_minus--)
                {
                    save_bar(last_minus);
                }
            }
        }


        // Event handler called by CAlgo for every bar that occurs
        protected override void OnBar()
        {
            if (isFirstBar)
            {
                process_earlier_bars();
                isFirstBar = false;
            }
            save_bar(1);
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
