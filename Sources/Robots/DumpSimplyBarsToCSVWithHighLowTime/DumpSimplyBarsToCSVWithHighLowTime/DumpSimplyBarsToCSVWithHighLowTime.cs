using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FullAccess)]

    public class DumpSimplyBarsToCSVWithHighLowTime : Robot
    {

        private StreamWriter writer;
        string filePath = "";

        private bool firstBar = true;
        private int curIndex = 0;
        private int activBar = 0;

        private string OHLC_String = "";
        private double OPEN_B = 0, OPEN_A = 0;
        private double HIGH_B = 0, HIGH_A = 0;
        private double LOW_B = 0, LOW_A = 0;
        private double CLOSE_B = 0, CLOSE_A = 0;
        private DateTime HTime;
        private DateTime LTime;

        protected override void OnStart()
        {

            Thread.CurrentThread.CurrentCulture = CultureInfo.InvariantCulture;
            var desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var folderPath = Path.Combine(desktopFolder, "trendbars");
            Directory.CreateDirectory(folderPath);
            filePath = Path.Combine(folderPath, Symbol.Code + " " + TimeFrame + ".csv");
            writer = File.CreateText(filePath);

        }


        protected override void OnTick()
        {

            if (activBar == 0)
                return;
            else if (activBar == MarketSeries.Close.Count - 1 && OPEN_B != 0 && HIGH_B != 0 && LOW_B != 0)
            {
                if (Symbol.Bid > HIGH_B)
                {
                    HIGH_B = Symbol.Bid;
                    HIGH_A = Symbol.Ask;
                    HTime = Server.Time;
                }

                if (Symbol.Bid < LOW_B)
                {
                    LOW_B = Symbol.Bid;
                    LOW_A = Symbol.Ask;
                    LTime = Server.Time;
                }
            }

        }


        protected override void OnBar()
        {

            curIndex = MarketSeries.Close.Count - 2;
            // this is the index of completed candlestick 

            CLOSE_B = Symbol.Bid;
            CLOSE_A = Symbol.Ask;

            activBar = curIndex + 1;
            OHLC_String = ConcatWithComma(MarketSeries.OpenTime[curIndex], MarketSeries.OpenTime[curIndex].ToString("yyyyMMdd"), MarketSeries.OpenTime[curIndex].ToString("HH:mm"), OPEN_B, OPEN_A, HIGH_B, HIGH_A, LOW_B, LOW_A, CLOSE_B,
            CLOSE_A, HTime < LTime ? 1 : 0, HTime, LTime);

            // Dump into file with OpenTime, Open_Bid, Open_Ask, High_Bid, High_Ask , Low_Bid, Low_Ask, IsHighFirstFlag, HighTime, LowTime
            if (!firstBar)
                writer.WriteLine(OHLC_String);

            OPEN_B = Symbol.Bid;
            OPEN_A = Symbol.Ask;
            HIGH_B = OPEN_B;
            HIGH_A = OPEN_A;
            LOW_B = OPEN_B;
            LOW_A = OPEN_A;
            LTime = Server.Time;
            HTime = Server.Time;
            firstBar = false;

        }

        private string ConcatWithComma(params object[] parameters)
        {
            return string.Join(",", parameters.Select(p => p.ToString()));
        }
    }
}
