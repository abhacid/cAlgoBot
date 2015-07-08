using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MarketClocks : Indicator
    {

        [Parameter(DefaultValue = 1, MaxValue = 1, MinValue = 0)]
        public int paramFormat24HR { get; set; }

        public double TokyoOpen = 8;
        public double TokyoClose = 17;
        //Some use a 6pm close for Tokyo.  I am using 5pm for all closings.
        public double SydneyOpen = 8;
        public double SydneyClose = 16.5;
        //Some use a 4pm close for Sydney
        public double LondonOpen = 8;
        public double LondonClose = 17;
        public double NYOpen = 8;
        public double NYClose = 17;
        public double EuOpen = 9;
        public double EuClose = 17.5;

        public override void Calculate(int index)
        {
            TimeZoneInfo LocalTimeZone = TimeZoneInfo.Local;

            //Used to verify local time zone settings
            //Print("Local Time Zone Name:{0} Offset:{1} DST:{2} ",LocalTimeZone.DisplayName,LocalTimeZone.BaseUtcOffset,LocalTimeZone.SupportsDaylightSavingTime);

            TimeZoneInfo TokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            TimeZoneInfo NYTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            TimeZoneInfo LondonTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            TimeZoneInfo SydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
            TimeZoneInfo EuTimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");

            DateTime LocalTime = DateTime.Now;
            DateTime SydneyTime = TimeZoneInfo.ConvertTime(LocalTime, LocalTimeZone, SydneyTimeZone);
            DateTime LondonTime = TimeZoneInfo.ConvertTime(LocalTime, LocalTimeZone, LondonTimeZone);
            DateTime TokyoTime = TimeZoneInfo.ConvertTime(LocalTime, LocalTimeZone, TokyoTimeZone);
            DateTime NYTime = TimeZoneInfo.ConvertTime(LocalTime, LocalTimeZone, NYTimeZone);
            DateTime EuTime = TimeZoneInfo.ConvertTime(LocalTime, LocalTimeZone, EuTimeZone);

            Colors EuColor = Colors.DarkGray;
            Colors LondonColor = Colors.DarkGray;
            Colors NYColor = Colors.DarkGray;
            Colors SydneyColor = Colors.DarkGray;
            Colors TokyoColor = Colors.DarkGray;

            string strEuTime;
            string strLondonTime;
            string strNYTime;
            string strSydneyTime;
            string strTokyoTime;

            string strNYLabel = string.Format("{0,-90}", "New York");
            string strLondonLabel = string.Format("{0,-48}", "London");
            string strEuLabel = string.Format("{0,0}", "Euro");
            string strTokyoLabel = string.Format("{0,48}", "Tokyo");
            string strSydneyLabel = string.Format("{0,90}", "Sydney");


            strNYTime = string.Format("\n{0,-90}", NYTime.ToString("HH:mm"));
            strLondonTime = string.Format("\n{0,-48}", LondonTime.ToString("HH:mm"));
            strEuTime = string.Format("\n{0,0}", EuTime.ToString("HH:mm"));
            strTokyoTime = string.Format("\n{0,48}", TokyoTime.ToString("HH:mm"));
            strSydneyTime = string.Format("\n{0,89}", SydneyTime.ToString("HH:mm"));



            if (EuTime.Hour >= EuOpen & EuTime.Hour < EuClose)
                EuColor = Colors.Magenta;
            if (LondonTime.Hour >= LondonOpen & LondonTime.Hour < LondonClose)
                LondonColor = Colors.Blue;
            if (NYTime.Hour >= NYOpen & NYTime.Hour < NYClose)
                NYColor = Colors.Blue;
            if (SydneyTime.Hour >= SydneyOpen & SydneyTime.Hour < SydneyClose)
                SydneyColor = Colors.Red;
            if (TokyoTime.Hour >= TokyoOpen & TokyoTime.Hour < TokyoClose)
                TokyoColor = Colors.Red;

            ChartObjects.DrawText("TimeLabel1", strLondonLabel, StaticPosition.TopCenter, LondonColor);
            ChartObjects.DrawText("TimeLabel2", strNYLabel, StaticPosition.TopCenter, NYColor);
            ChartObjects.DrawText("TimeLabel3", strSydneyLabel, StaticPosition.TopCenter, SydneyColor);
            ChartObjects.DrawText("TimeLabel4", strTokyoLabel, StaticPosition.TopCenter, TokyoColor);
            ChartObjects.DrawText("TimeLabel5", strEuLabel, StaticPosition.TopCenter, EuColor);

            ChartObjects.DrawText("Time1", strLondonTime, StaticPosition.TopCenter, LondonColor);
            ChartObjects.DrawText("Time2", strNYTime, StaticPosition.TopCenter, NYColor);
            ChartObjects.DrawText("Time3", strSydneyTime, StaticPosition.TopCenter, SydneyColor);
            ChartObjects.DrawText("Time4", strTokyoTime, StaticPosition.TopCenter, TokyoColor);
            ChartObjects.DrawText("Time5", strEuTime, StaticPosition.TopCenter, EuColor);
        }
    }
}
