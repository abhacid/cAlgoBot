//#reference: ZigZag.algo
// This is version 1.01

using System;
using System.Linq;
using System.IO;
using System.Text;
using System.Collections.Generic;
using cAlgo.API;
namespace cAlgo.Indicators
{
    public class Previouslist
    {
        public double Preis { get; set; }
        public double Volumen { get; set; }
    }

    [Indicator("Bid/Ask Volumes (in Millions)", IsOverlay = false, AccessRights = AccessRights.FileSystem, ScalePrecision = 2)]
    public class AccumulativeBidAskVolume : Indicator
    {
        [Parameter("Minimum Volume (Millions)", DefaultValue = 10.0, MinValue = 0.0)]
        public double LowFilter { get; set; }

        [Parameter("Maximum Volume (Millions)", DefaultValue = 999.0, MinValue = 1.0)]
        public double HighFilter { get; set; }

        [Parameter("Read from file", DefaultValue = true)]
        public bool ReadFromFile { get; set; }

        [Parameter("Write to file", DefaultValue = true)]
        public bool WriteToFile { get; set; }

        [Parameter("Write interval (seconds)", DefaultValue = 60, MinValue = 20)]
        public int WriteInterval { get; set; }

        [Parameter("Filename (none=Auto)", DefaultValue = "")]
        public string FileName { get; set; }

        [Parameter("ZigZag Depth", DefaultValue = 12, MinValue = 0)]
        public int Depth { get; set; }

        [Parameter("ZigZag Deviation", DefaultValue = 5, MinValue = 0)]
        public int Deviation { get; set; }

        [Parameter("ZigZag Backstep", DefaultValue = 3, MinValue = 0)]
        public int BackStep { get; set; }

        [Output("Bid Volumes", PlotType = PlotType.Histogram, Color = Colors.Red, Thickness = 5)]
        public IndicatorDataSeries AccBidVolume { get; set; }

        [Output("Ask Volumes", PlotType = PlotType.Histogram, Color = Colors.Blue, Thickness = 5)]
        public IndicatorDataSeries AccAskVolume { get; set; }

        [Output("Ask-Bid Difference", PlotType = PlotType.Histogram, Color = Colors.White, Thickness = 5)]
        public IndicatorDataSeries AccAskBidDifference { get; set; }

        private double LowFilterM;
        private double HighFilterM;
        private int LastResultIndex;
        private double addPrevAccAskVolume;
        private double addPrevAccBidVolume;
        private bool DoneRewrite = false;
        private IndicatorDataSeries BidVolume;
        private IndicatorDataSeries AskVolume;
        ZigZag zigzag;
        private MarketDepth _marketDepth;
        private List<Previouslist> PreviousBidList = new List<Previouslist>();
        private List<Previouslist> PreviousAskList = new List<Previouslist>();
        private StringBuilder Table = new StringBuilder();
        private string fname;
        private char[] Delimiters = 
        {
            ',',
            ','
        };

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
            if (!double.IsNaN(zigzag.Result[index]))
                LastResultIndex = index;
            if (index - Depth == LastResultIndex && !DoneRewrite)
            {
                for (int i = LastResultIndex + 1; i < index; i++)
                {
                    if (i == LastResultIndex + 1)
                    {
                        AccBidVolume[i] = BidVolume[i];
                        AccAskVolume[i] = AskVolume[i];
                        AccAskBidDifference[i] = AccAskVolume[i] + AccBidVolume[i];
                    }
                    else
                    {
                        if (double.IsNaN(AccBidVolume[i - 1]))
                            addPrevAccBidVolume = 0;
                        else
                            addPrevAccBidVolume = AccBidVolume[i - 1];
                        if (double.IsNaN(AccAskVolume[i - 1]))
                            addPrevAccAskVolume = 0;
                        else
                            addPrevAccAskVolume = AccAskVolume[i - 1];

                        AccBidVolume[i] = BidVolume[i] + addPrevAccBidVolume;
                        AccAskVolume[i] = AskVolume[i] + addPrevAccAskVolume;
                        AccAskBidDifference[i] = AccAskVolume[i] + AccBidVolume[i];
                    }
                }
                DoneRewrite = true;
            }
            if (index - Depth != LastResultIndex)
                DoneRewrite = false;
            if (double.IsNaN(AccBidVolume[index - 1]))
                addPrevAccBidVolume = 0;
            else
                addPrevAccBidVolume = AccBidVolume[index - 1];
            if (double.IsNaN(AccAskVolume[index - 1]))
                addPrevAccAskVolume = 0;
            else
                addPrevAccAskVolume = AccAskVolume[index - 1];

            AccBidVolume[index] = BidVolume[index] + addPrevAccBidVolume;
            AccAskVolume[index] = AskVolume[index] + addPrevAccAskVolume;
            AccAskBidDifference[index] = AccAskVolume[index] + AccBidVolume[index];
        }

//--------------------------------------

        protected override void Initialize()
        {
            _marketDepth = MarketData.GetMarketDepth(Symbol);
            _marketDepth.Updated += MarketDepthUpdated;

            BidVolume = CreateDataSeries();
            AskVolume = CreateDataSeries();

            zigzag = Indicators.GetIndicator<ZigZag>(Depth, Deviation, BackStep);

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

            LowFilterM = LowFilter * 1000000;
            HighFilterM = HighFilter * 1000000;
            fname = string.Format("{0}{1}{2}{3}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "\\", FileName == "" ? Symbol.Code : FileName, ".csv");
            if (ReadFromFile && System.IO.File.Exists(fname) == true)
            {
                using (StreamReader Fstream = new StreamReader(fname))
                {
                    string line;
                    while ((line = Fstream.ReadLine()) != null)
                    {
                        try
                        {
                            string[] words = line.Split(Delimiters);
                            double vol = Convert.ToDouble(words[1]);
                            if (vol >= HighFilterM || vol < LowFilterM)
                                continue;
                            int bago = BarsAgo(Convert.ToDateTime(words[0]));
                            if (bago == -1)
                                continue;
                            int bidx = MarketSeries.Close.Count - 1 - bago;
                            if (double.IsNaN(AskVolume[bidx]))
                                AskVolume[bidx] = 0;
                            if (double.IsNaN(BidVolume[bidx]))
                                BidVolume[bidx] = 0;
                            switch (words[2])
                            {
                                case "A":
                                    AskVolume[bidx] += (vol / 1000000);
                                    break;
                                case "B":
                                    BidVolume[bidx] -= (vol / 1000000);
                                    break;
                            }
                        } catch
                        {
                            continue;
                        }
                    }
                }
            }
            if (WriteToFile)
            {
                if (System.IO.File.Exists(fname) == false)
                    System.IO.File.WriteAllText(fname, "");
                Timer.Start(WriteInterval);
            }
        }
//--------------------------------------

        protected override void OnTimer()
        {
            if (Table.Length != 0)
            {
                using (StreamWriter Swrite = File.AppendText(fname))
                {
                    Swrite.Write(Table.ToString());
                }
                Table.Clear();
            }
        }

//--------------------------------------

        void MarketDepthUpdated()
        {
            if (double.IsNaN(BidVolume[MarketSeries.Close.Count - 1]))
                BidVolume[MarketSeries.Close.Count - 1] = 0;
            if (double.IsNaN(AskVolume[MarketSeries.Close.Count - 1]))
                AskVolume[MarketSeries.Close.Count - 1] = 0;

            foreach (var entry in _marketDepth.BidEntries)
            {
                int idx = 0;
                if (WriteToFile)
                {
                    idx = PreviousBidList.FindIndex(r => r.Preis.Equals(entry.Price));
                    if (idx == -1)
                        Table.AppendLine(string.Format("{0},{1},{2}", Time, entry.Volume, "B"));
                    else
                    {
                        double DifferenceVolume = entry.Volume - PreviousBidList[idx].Volumen;
                        if (DifferenceVolume > 0)
                            Table.AppendLine(string.Format("{0},{1},{2}", Time, entry.Volume - PreviousBidList[idx].Volumen, "B"));
                    }
                }
                if (entry.Volume >= LowFilterM)
                {
                    if (!WriteToFile)
                    {
                        idx = PreviousBidList.FindIndex(r => r.Preis.Equals(entry.Price));
                    }
                    if (idx == -1 && entry.Volume < HighFilterM)
                        BidVolume[MarketSeries.Close.Count - 1] -= (entry.Volume / 1000000);
                    if (idx != -1)
                    {
                        double DifferenceVolume = entry.Volume - PreviousBidList[idx].Volumen;
                        if (DifferenceVolume >= LowFilterM && DifferenceVolume < HighFilterM)
                            BidVolume[MarketSeries.Close.Count - 1] -= (DifferenceVolume / 1000000);
                    }
                }
            }

            foreach (var entry in _marketDepth.AskEntries)
            {
                int idx = 0;
                if (WriteToFile)
                {
                    idx = PreviousAskList.FindIndex(r => r.Preis.Equals(entry.Price));
                    if (idx == -1)
                        Table.AppendLine(string.Format("{0},{1},{2}", Time, entry.Volume, "A"));
                    else
                    {
                        double DifferenceVolume = entry.Volume - PreviousAskList[idx].Volumen;
                        if (DifferenceVolume > 0)
                            Table.AppendLine(string.Format("{0},{1},{2}", Time, entry.Volume - PreviousAskList[idx].Volumen, "A"));
                    }
                }

                if (entry.Volume >= LowFilterM)
                {
                    if (!WriteToFile)
                    {
                        idx = PreviousAskList.FindIndex(r => r.Preis.Equals(entry.Price));
                    }
                    if (idx == -1 && entry.Volume < HighFilterM)
                        AskVolume[MarketSeries.Close.Count - 1] += (entry.Volume / 1000000);
                    if (idx != -1)
                    {
                        double DifferenceVolume = entry.Volume - PreviousAskList[idx].Volumen;
                        if (DifferenceVolume >= LowFilterM && DifferenceVolume < HighFilterM)
                            AskVolume[MarketSeries.Close.Count - 1] += (DifferenceVolume / 1000000);
                    }
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
