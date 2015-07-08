using System;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.FileSystem)]
    public class Alerts : Indicator
    {
        [Parameter("Prices")]
        public string Prices { get; set; }

        [Parameter("Sound", DefaultValue = false)]
        public bool Sound { get; set; }

        [Parameter("Visual", DefaultValue = true)]
        public bool Visual { get; set; }
        
        [Parameter("Alert diapasone", DefaultValue = 10, MinValue = 0)]
        public int AlertDiapasone { get; set; }
        
        [Parameter("Contignous type", DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int ContignousType { get; set; }
        
        [Parameter("Thickness", DefaultValue = 1, MinValue = 1)]
        public int Thickness { get; set; }
       
        
        private class Alert
        {
            public DateTime setup;
            public double price;
            public int index = 0;
            public bool inArgs;
            public int played = 5;
            public DateTime? lastPlayed = null;

            public Alert(DateTime setup, double price, bool inArgs)
            {
                this.setup = setup;
                this.price = price;
                this.inArgs = inArgs;
            }
        }

        private List<Alert> alerts;

        private string lastError;

        private string getFName()
        {
            return string.Format("{0}\\{1}-alerts.csv", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), Symbol.Code);
        }

        private static Object lockObject = new Object();

        private bool load()
        {
            string fname = getFName();

            if (!File.Exists(fname))
            {
                return true;
            }

            StreamReader rd = new StreamReader(new FileStream(fname, FileMode.Open, FileAccess.Read, FileShare.Read));

            try
            {
                while (!rd.EndOfStream)
                {
                    string line = rd.ReadLine();
                    string []arr = line.Split(';');

                    if (arr.Length != 2)
                    {
                        lastError = string.Format("Read {0} error : invalid alert format - ", fname, line);
                        Print(lastError);
                        return false;
                    }

                    Alert alert;

                    try
                    {
                        alert = new Alert(DateTime.ParseExact(arr[1], "yyyyMMdd-HHmm", CultureInfo.InvariantCulture), Double.Parse(arr[0]), false);
                    }
                    catch(Exception e)
                    {
                        lastError = string.Format("Read {0} error : invalid alert format - {1}", fname, line, e.Message);
                        Print(lastError);
                        return false;
                    }

                    alerts.Add(alert);
                }
            }
            catch (IOException e)
            {
                lastError = string.Format("Read {0} error : {1}", fname, e.Message);
                Print(lastError);
                return false;
            }
            finally
            {
                rd.Close();
            }

            return true;
        }

        private void save()
        {
            if (alerts.Count == 0)
            {
                return;
            }

            string fname = getFName();
            StreamWriter wr = new StreamWriter(new FileStream(fname, FileMode.Create, FileAccess.Write, FileShare.None));

            try
            {
                foreach (Alert alert in alerts)
                {
                    wr.WriteLine(string.Format("{0};{1}", alert.price, alert.setup.ToString("yyyyMMdd-HHmm")));
                }
            }
            catch(IOException e)
            {
                Print("Write {0} error : {1}", fname, e.Message);
            }
            finally
            {
                wr.Close();
            }
        }

        private Alert alertExists(double price)
        {
            foreach (Alert alert in alerts)
            {
                if (alert.price == price)
                {
                    return alert;
                }
            }
            return null;
        }

        private bool parse()
        {
            if (Prices == null)
            {
                return true;
            }

            string []prices = Prices.Split(';');

            try
            {
                foreach (string s in prices)
                {
                    double price = double.Parse(s);
                    Alert exists = alertExists(price);
                    if (exists == null)
                    {
                        alerts.Add(new Alert(MarketSeries.OpenTime.LastValue, price, true));
                    }
                    else
                    {
                        exists.inArgs = true;
                    }
                }
            }
            catch (Exception e)
            {
                lastError = string.Format("Prices parsing error : {0}", e.Message);
                Print(lastError);
                return false;
            }

            return true;
        }

        private int Period()
        {
            if (TimeFrame == TimeFrame.Minute)
                return 1;
            if (TimeFrame == TimeFrame.Minute2)
                return 2;
            if (TimeFrame == TimeFrame.Minute3)
                return 3;
            if (TimeFrame == TimeFrame.Minute4)
                return 4;
            if (TimeFrame == TimeFrame.Minute5)
                return 5;
            if (TimeFrame == TimeFrame.Minute6)
                return 6;
            if (TimeFrame == TimeFrame.Minute7)
                return 7;
            if (TimeFrame == TimeFrame.Minute8)
                return 8;
            if (TimeFrame == TimeFrame.Minute9)
                return 9;
            if (TimeFrame == TimeFrame.Minute10)
                return 10;
            if (TimeFrame == TimeFrame.Minute15)
                return 15;
            if (TimeFrame == TimeFrame.Minute20)
                return 20;
            if (TimeFrame == TimeFrame.Minute30)
                return 30;
            if (TimeFrame == TimeFrame.Minute45)
                return 45;
            if (TimeFrame == TimeFrame.Hour)
                return 60;
            if (TimeFrame == TimeFrame.Hour2)
                return 120;
            if (TimeFrame == TimeFrame.Hour3)
                return 180;
            if (TimeFrame == TimeFrame.Hour4)
                return 240;
            if (TimeFrame == TimeFrame.Hour6)
                return 360;
            if (TimeFrame == TimeFrame.Hour8)
                return 480;
            if (TimeFrame == TimeFrame.Hour12)
                return 720;
            if (TimeFrame == TimeFrame.Daily)
                return 1440;
            if (TimeFrame == TimeFrame.Day2)
                return 2880;
            if (TimeFrame == TimeFrame.Day3)
                return 4320;
            if (TimeFrame == TimeFrame.Weekly)
                return 10080;
            return 43200;
        }

        private void ReDraw()
        {
            ChartObjects.RemoveAllObjects();
            foreach (Alert alert in alerts)
            {
                if (MarketSeries.OpenTime.LastValue.AddMinutes(Period()) < alert.setup)
                {
                    return;
                }

                if (!alert.inArgs)
                {
                    continue;
                }

                Colors color;
                int periods = MarketSeries.OpenTime.Count - MarketSeries.OpenTime.GetIndexByTime(alert.setup);
                
                bool crossed = MarketSeries.Close.HasCrossedAbove(alert.price, periods) || MarketSeries.Close.HasCrossedBelow(alert.price, periods);
                bool near = Math.Abs(MarketSeries.Close.LastValue - alert.price) <= AlertDiapasone * Symbol.PipSize;
                
                if (near && Sound && alert.played > 0 && (alert.lastPlayed == null || DateTime.UtcNow.AddMinutes(-1) >= alert.lastPlayed))
                {
                    Notifications.PlaySound(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "Ring.wav"));
                    alert.lastPlayed = DateTime.UtcNow;
                    --alert.played;
                }

                if (Visual && (crossed || near))
                {
                    color = Colors.Red;
                }
                else
                {
                    color = Colors.CornflowerBlue;
                }

                ChartObjects.DrawText(string.Format("alert_{0}", alert.price),
                                      string.Format("| {0}{1}", alert.price, near ? string.Format(" ({0})", Math.Round(Math.Abs(MarketSeries.Close.LastValue - alert.price) / Symbol.PipSize), 1) : ""),
                                      MarketSeries.OpenTime.Count + 1,
                                      alert.price,
                                      VerticalAlignment.Center,
                                      HorizontalAlignment.Right,
                                      color);

                if (ContignousType == 0)
                {
                    for (int j = MarketSeries.Close.Count - 1; j > 0 && alert.index == 0; --j)
                    {
                        if (MarketSeries.Low[j] <= alert.price && MarketSeries.High[j] >= alert.price)
                        {
                            alert.index = j;
                        }
                    }
                }
                else
                {
                    alert.index = 1;
                }

                if (alert.index != 0)
                {
                    ChartObjects.DrawLine(string.Format("alert_{0}_l", alert.price),
                                          alert.index,
                                          alert.price,
                                          MarketSeries.OpenTime.Count + 1,
                                          alert.price,
                                          color,
                                          Thickness,
                                          LineStyle.Dots);
                }
            }
        }

        private DateTime? lastModified;

        private void check()
        {
            DateTime? writeTime = null;

            lock (lockObject)
            {
                if (File.Exists(getFName()))
                {
                    writeTime = File.GetLastWriteTime(getFName());
                }

                if (lastModified == null || lastModified < writeTime)
                {
                    ChartObjects.RemoveAllObjects();
                    alerts.Clear();

                    if (!load() || !parse())
                    {
                        ChartObjects.DrawText("AlertsErrorMessage", string.Format("Alerts\n{0}", lastError), StaticPosition.TopRight, Colors.Red);
                    }
                    else
                    {
                        if (lastModified == null)
                        {
                            save();
                        }
                    }
                }

                lastModified = writeTime;
            }
        }

        protected override void Initialize()
        {
            alerts = new List<Alert>();
            check();
        }
        
        private int ticks = 10;
        private double last = 0;

        public override void Calculate(int index)
        {
            if (--ticks == 0 || Math.Abs(last - MarketSeries.Close.LastValue) > Symbol.PipSize)
            {
                check();
                ReDraw();
                ticks = 10;
                last = MarketSeries.Close.LastValue;
            }
        }
    }
}
