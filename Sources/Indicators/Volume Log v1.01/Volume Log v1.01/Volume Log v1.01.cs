// This is version 1.01

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using cAlgo.API;
namespace cAlgo.Indicators
{
    public class Listtype
    {
        public double Preis { get; set; }
        public DateTime Zeit { get; set; }
        public double Volumen { get; set; }
    }

    public class Previouslist
    {
        public double Preis { get; set; }
        public double Volumen { get; set; }
    }

    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class VolumeLog : Indicator
    {
        [Parameter("Minimum Volume (Millions)", DefaultValue = 16.0, MinValue = 0.0)]
        public double Filter { get; set; }

        [Parameter("List Length: Items", DefaultValue = 20, MinValue = 2, MaxValue = 50)]
        public int ListLength { get; set; }

        [Parameter("List Length: Bars", DefaultValue = 3, MinValue = 1)]
        public int BarsFilter { get; set; }

        [Parameter("Update Frequency (Seconds)", DefaultValue = 5, MinValue = 1)]
        public int UpdateFrequency { get; set; }

        [Parameter("Text Color", DefaultValue = "White")]
        public string TextColor { get; set; }

        private double FilterM;
        private int RollingListLength = 200;
        private MarketDepth _marketDepth;
        private List<Previouslist> PreviousBidList = new List<Previouslist>();
        private List<Previouslist> PreviousAskList = new List<Previouslist>();
        private List<Listtype> RollingListBid = new List<Listtype>(20);
        private List<Listtype> RollingListAsk = new List<Listtype>(20);

        private int BarsAgo(DateTime time)
        {
            for (int i = MarketSeries.OpenTime.Count - 1; i > 0; i--)
            {
                if (MarketSeries.OpenTime[i] <= time)
                    return MarketSeries.OpenTime.Count - 1 - i;
            }
            return -1;
        }
//--------------------------------------
        public override void Calculate(int index)
        {
        }
//--------------------------------------        
        protected override void Initialize()
        {
            _marketDepth = MarketData.GetMarketDepth(Symbol);
            _marketDepth.Updated += MarketDepthUpdated;

            foreach (var entry in _marketDepth.BidEntries)
            {
                PreviousBidList.Add(new Previouslist 
                {
                    Preis = entry.Price,
                    Volumen = entry.Volume
                });
            }

            foreach (var entry in _marketDepth.AskEntries)
            {
                PreviousAskList.Add(new Previouslist 
                {
                    Preis = entry.Price,
                    Volumen = entry.Volume
                });
            }

            FilterM = Filter * 1000000;

            var Table = new StringBuilder();
            Table.AppendLine("Bid\tTime\tMillion\t\tAsk\tTime\tMillion");
            Table.AppendLine("(Buys)\tago\tUnits\t\t(Sells)\tago\tUnits");
            Table.AppendLine("----------------------------------------------------------------------------------");
            ChartObjects.DrawText("Header", Table.ToString(), StaticPosition.TopLeft, (Colors)Enum.Parse(typeof(Colors), TextColor, true));
            Timer.Start(UpdateFrequency);
        }
//--------------------------------------
        protected override void OnTimer()
        {
            var BATable = new StringBuilder();
            BATable.AppendLine("\n\n");
            TimeSpan oldTime = new TimeSpan();
            int oldbago = new int();

            foreach (var Element in RollingListBid.Reverse<Listtype>().Take(ListLength))
            {
                int bago = BarsAgo(Element.Zeit);
                if (bago > BarsFilter)
                    break;
                double dPrice = (Element.Preis - Symbol.Bid) / Symbol.PipSize;
                BATable.Append(string.Format("{0}", dPrice.ToString("0.0")));
                TimeSpan dTime = Time - Element.Zeit;
                double dTimeTMtr = Math.Truncate(dTime.TotalMinutes);
                if (bago > 0)
                {
                    if (bago == oldbago)
                        BATable.Append(string.Format("\t"));
                    else
                        BATable.Append(string.Format("\t{0}{1}", bago, (bago == 1) ? " bar" : " bars"));
                }
                else
                {
                    if (dTimeTMtr >= 1)
                    {
                        if (Math.Truncate(oldTime.TotalMinutes) == dTimeTMtr)
                            BATable.Append(string.Format("\t"));
                        else
                            BATable.Append(string.Format("\t{0}{1}", dTimeTMtr, " min"));
                    }
                    else
                        BATable.Append(string.Format("\t{0}", (oldTime.TotalMinutes == 0) ? "<1 min" : ""));
                }
                oldTime = dTime;
                oldbago = bago;
                BATable.AppendLine(string.Format("\t{0}{1}", Math.Round(Element.Volumen / 1000000.0, 1), "m"));
            }
            ChartObjects.DrawText("Bids", BATable.ToString(), StaticPosition.TopLeft, (Colors)Enum.Parse(typeof(Colors), TextColor, true));
            BATable.Clear();
            BATable.AppendLine("\n\n");
            oldTime = new TimeSpan();
            oldbago = new int();

            foreach (var Element in RollingListAsk.Reverse<Listtype>().Take(ListLength))
            {
                int bago = BarsAgo(Element.Zeit);
                if (bago > BarsFilter)
                    break;
                double dPrice = (Element.Preis - Symbol.Ask) / Symbol.PipSize;
                BATable.Append(string.Format("\t\t\t\t{0}", dPrice.ToString("0.0")));
                TimeSpan dTime = Time - Element.Zeit;
                double dTimeTMtr = Math.Truncate(dTime.TotalMinutes);
                if (bago > 0)
                {
                    if (bago == oldbago)
                        BATable.Append(string.Format("\t"));
                    else
                        BATable.Append(string.Format("\t{0}{1}", bago, (bago == 1) ? " bar" : " bars"));
                }
                else
                {
                    if (dTimeTMtr >= 1)
                    {
                        if (Math.Truncate(oldTime.TotalMinutes) == dTimeTMtr)
                            BATable.Append(string.Format("\t"));
                        else
                            BATable.Append(string.Format("\t{0}{1}", dTimeTMtr, " min"));
                    }
                    else
                        BATable.Append(string.Format("\t{0}", (oldTime.TotalMinutes == 0) ? "<1 min" : ""));
                }
                oldTime = dTime;
                oldbago = bago;
                BATable.AppendLine(string.Format("\t{0}{1}", Math.Round(Element.Volumen / 1000000.0, 1), "m"));
            }
            ChartObjects.DrawText("Asks", BATable.ToString(), StaticPosition.TopLeft, (Colors)Enum.Parse(typeof(Colors), TextColor, true));
        }

//--------------------------------------
        void MarketDepthUpdated()
        {
            foreach (var entry in _marketDepth.BidEntries)
            {
                if (entry.Volume >= FilterM)
                {
                    int idx = PreviousBidList.FindIndex(r => r.Preis.Equals(entry.Price));
                    if (idx == -1)
                    {
                        RollingListBid.Add(new Listtype 
                        {
                            Preis = entry.Price,
                            Zeit = Time,
                            Volumen = entry.Volume
                        });
                    }
                    else
                    {
                        double DifferenceVolume = entry.Volume - PreviousBidList[idx].Volumen;
                        if (DifferenceVolume >= FilterM)
                        {
                            RollingListBid.Add(new Listtype 
                            {
                                Preis = entry.Price,
                                Zeit = Time,
                                Volumen = DifferenceVolume
                            });
                        }
                    }

                    if (RollingListBid.Count > RollingListLength)
                        RollingListBid.RemoveAt(0);
                }
            }

            foreach (var entry in _marketDepth.AskEntries)
            {
                if (entry.Volume >= FilterM)
                {
                    int idx = PreviousAskList.FindIndex(r => r.Preis.Equals(entry.Price));
                    if (idx == -1)
                    {
                        RollingListAsk.Add(new Listtype 
                        {
                            Preis = entry.Price,
                            Zeit = Time,
                            Volumen = entry.Volume
                        });
                    }
                    else
                    {
                        double DifferenceVolume = entry.Volume - PreviousAskList[idx].Volumen;
                        if (DifferenceVolume >= FilterM)
                        {
                            RollingListAsk.Add(new Listtype 
                            {
                                Preis = entry.Price,
                                Zeit = Time,
                                Volumen = DifferenceVolume
                            });
                        }
                    }

                    if (RollingListAsk.Count > RollingListLength)
                        RollingListAsk.RemoveAt(0);
                }
            }

            PreviousBidList.Clear();
            foreach (var entry in _marketDepth.BidEntries)
            {
                PreviousBidList.Add(new Previouslist 
                {
                    Preis = entry.Price,
                    Volumen = entry.Volume
                });
            }

            PreviousAskList.Clear();
            foreach (var entry in _marketDepth.AskEntries)
            {
                PreviousAskList.Add(new Previouslist 
                {
                    Preis = entry.Price,
                    Volumen = entry.Volume
                });
            }
        }
    }
}
