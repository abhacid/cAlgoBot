using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class DashBoardSMA : Indicator
    {
        [Parameter("Period SMA 1", DefaultValue = 25, MinValue = 1)]
        public int per1 { get; set; }

        [Parameter("Period SMA 2", DefaultValue = 50, MinValue = 1)]
        public int per2 { get; set; }

        [Parameter("Period SMA 3", DefaultValue = 200, MinValue = 1)]
        public int per3 { get; set; }

        //+-----------------------------------------------------------------+

        private string[] ch;
        private Colors[] clr;
        private TimeFrame[] tf;
        private int[] per;

        private MovingAverage[,] sma;
        private MarketSeries[] series;

        //+-----------------------------------------------------------------+

        protected override void Initialize()
        {
            ch = new string[3];
            ch[0] = "Up";
            ch[1] = "Dn";
            ch[2] = " -- ";

            clr = new Colors[3];
            clr[0] = Colors.Green;
            clr[1] = Colors.Red;
            clr[2] = Colors.Gray;

            tf = new TimeFrame[9];
            tf[0] = TimeFrame.Minute;
            tf[1] = TimeFrame.Minute5;
            tf[2] = TimeFrame.Minute15;
            tf[3] = TimeFrame.Minute30;
            tf[4] = TimeFrame.Hour;
            tf[5] = TimeFrame.Hour4;
            tf[6] = TimeFrame.Daily;
            tf[7] = TimeFrame.Weekly;
            tf[8] = TimeFrame.Monthly;

            per = new int[3];
            per[0] = per1;
            per[1] = per2;
            per[2] = per3;

            series = new MarketSeries[9];
            for (int i = 0; i < 9; i++)
            {
                series[i] = MarketData.GetSeries(tf[i]);
            }

            sma = new MovingAverage[3, 9];
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    sma[i, j] = Indicators.MovingAverage(series[j].Close, per[i], MovingAverageType.Simple);
                }
            }

        }

        //+-----------------------------------------------------------------+

        public override void Calculate(int index)
        {
            if (!IsLastBar)
                return;

            string str = "";

            str = "Powered By: Iwori";
            ChartObjects.DrawText("top_1", str, StaticPosition.TopLeft, Colors.Green);

            str = xySpace(0, 1) + "eMail: Iwori@outlook.com  -  Skype: Iwori App";
            ChartObjects.DrawText("top_2", str, StaticPosition.TopLeft, Colors.Green);

            str = xySpace(0, 2) + "--------------------------------------------------------------------------------------------------------------------";
            ChartObjects.DrawText("sep_1", str, StaticPosition.TopLeft, Colors.Turquoise);

            str = xySpace(1, 3) + "m1";
            str += xySpace(1, 0) + "m5";
            str += xySpace(1, 0) + "m15";
            str += xySpace(1, 0) + "m30";
            str += xySpace(1, 0) + "h1";
            str += xySpace(1, 0) + "h4";
            str += xySpace(1, 0) + "d1";
            str += xySpace(1, 0) + "w1";
            str += xySpace(1, 0) + "M";
            ChartObjects.DrawText("tfs", str, StaticPosition.TopLeft, Colors.Turquoise);

            str = xySpace(0, 4) + per[0].ToString();
            str += xySpace(0, 1) + per[1].ToString();
            str += xySpace(0, 1) + per[2].ToString();
            ChartObjects.DrawText("pers", str, StaticPosition.TopLeft, Colors.Turquoise);

            str = xySpace(0, 7) + "--------------------------------------------------------------------------------------------------------------------";
            ChartObjects.DrawText("sep_2", str, StaticPosition.TopLeft, Colors.Turquoise);

            for (int i = 0; i < 9; i++)
            {
                int idx = GetIdx(index, 0, i);
                str = xySpace(1 + i, 4);
                str += ch[idx];
                ChartObjects.DrawText("s_0_" + i.ToString(), str, StaticPosition.TopLeft, clr[idx]);
            }

            for (int i = 0; i < 9; i++)
            {
                int idx = GetIdx(index, 1, i);
                str = xySpace(1 + i, 5);
                str += ch[idx];
                ChartObjects.DrawText("s_1_" + i.ToString(), str, StaticPosition.TopLeft, clr[idx]);
            }

            for (int i = 0; i < 9; i++)
            {
                int idx = GetIdx(index, 2, i);
                str = xySpace(1 + i, 6);
                str += ch[idx];
                ChartObjects.DrawText("s_2_" + i.ToString(), str, StaticPosition.TopLeft, clr[idx]);
            }
        }

        //+-----------------------------------------------------------------+

        private string xySpace(int x, int y)
        {
            if (x < 0 || y < 0)
                return ("");
            string str = "";
            for (int i = 0; i < y; i++)
                str += "\n";
            for (int i = 0; i < x; i++)
                str += "\t";
            return (str);
        }

        //+-----------------------------------------------------------------+

        private string AsciiToString(int unicode)
        {
            char character = (char)unicode;
            string text = character.ToString();
            return text;
        }

        //+-----------------------------------------------------------------+

        private int GetIdx(int index, int iPer, int iTf)
        {
            if (series[iTf].Close.LastValue > sma[iPer, iTf].Result.LastValue)
                return (0);
            if (series[iTf].Close.LastValue < sma[iPer, iTf].Result.LastValue)
                return (1);
            return (2);

        }

        //+-----------------------------------------------------------------+
    }
}
