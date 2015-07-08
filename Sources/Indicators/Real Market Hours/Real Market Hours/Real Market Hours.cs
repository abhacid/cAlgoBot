using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Levels(800, 900, 1000, 1100, 1200, 1300, 1400, 1500, 1600)]
    [Indicator(IsOverlay = false, ScalePrecision = 0, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class MarketHours : Indicator
    {
        public double TokyoOpen = 900;
        public double TokyoClose = 1700;
        //Some use a 6pm close for Tokyo.  I am using 5pm for all closings.
        public double SydneyOpen = 900;
        public double SydneyClose = 1700;
        //Some use a 4pm close for Sydney
        public double LondonOpen = 800;
        public double LondonClose = 1700;
        public double NYOpen = 800;
        public double NYClose = 1700;
        public TimeZoneInfo BrokerTimeZone;

        [Output("London", Color = Colors.Yellow, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries London { get; set; }
        [Output("New York", Color = Colors.Blue, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries NewYork { get; set; }
        [Output("Sydney", Color = Colors.Green, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries Sydney { get; set; }
        [Output("Tokyo", Color = Colors.Red, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries Tokyo { get; set; }

        protected override void Initialize()
        {
            var attribute = (IndicatorAttribute)typeof(MarketHours).GetCustomAttributes(typeof(IndicatorAttribute), false)[0];
            BrokerTimeZone = TimeZoneInfo.FindSystemTimeZoneById(attribute.TimeZone);
        }

        public override void Calculate(int index)
        {
            DateTime BrokerTime = MarketSeries.OpenTime[index];

            //Used to verify local time zone settings
            Print("Broker Time Zone Name:{0} Offset:{1} DST:{2} Kind:{3}", BrokerTimeZone.DisplayName, BrokerTimeZone.BaseUtcOffset, BrokerTimeZone.SupportsDaylightSavingTime, BrokerTime.Kind);

            TimeZoneInfo TokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            TimeZoneInfo NYTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
            TimeZoneInfo LondonTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            TimeZoneInfo SydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");

            DateTime SydneyTime = TimeZoneInfo.ConvertTime(BrokerTime, BrokerTimeZone, SydneyTimeZone);
            DateTime LondonTime = TimeZoneInfo.ConvertTime(BrokerTime, BrokerTimeZone, LondonTimeZone);
            DateTime TokyoTime = TimeZoneInfo.ConvertTime(BrokerTime, BrokerTimeZone, TokyoTimeZone);
            DateTime NYTime = TimeZoneInfo.ConvertTime(BrokerTime, BrokerTimeZone, NYTimeZone);

            double LondonValue = (LondonTime.Hour * 100) + LondonTime.Minute;
            double NYValue = (NYTime.Hour * 100) + NYTime.Minute;
            double SydneyValue = (SydneyTime.Hour * 100) + SydneyTime.Minute;
            double TokyoValue = (TokyoTime.Hour * 100) + TokyoTime.Minute;

            if (LondonValue >= LondonOpen && LondonValue < LondonClose)
                London[index] = LondonValue;

            if (NYValue >= NYOpen && NYValue < NYClose)
                NewYork[index] = NYValue;

            if (SydneyValue >= SydneyOpen && SydneyValue < SydneyClose)
                Sydney[index] = SydneyValue;

            if (TokyoValue >= TokyoOpen && TokyoValue < TokyoClose)
                Tokyo[index] = TokyoValue;

        }
    }
    /* Same as above except displayed on a straight line
       	if(LondonTime.Hour>=LondonOpen & LondonTime.Hour<LondonClose)London[index]=3;
       	if(NYTime.Hour>=NYOpen & NYTime.Hour<NYClose)NewYork[index]=2;
       	if(SydneyTime.Hour>=SydneyOpen & SydneyTime.Hour<SydneyClose)Sydney[index]=1;        
       	if(TokyoTime.Hour>=TokyoOpen & TokyoTime.Hour<TokyoClose)Tokyo[index]=0;
       	*/
}
