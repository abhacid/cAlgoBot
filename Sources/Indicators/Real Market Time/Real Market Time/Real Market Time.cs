using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true , AccessRights = AccessRights.None)]
    public class RealMarketTime : Indicator
    {
        
        
        [Parameter(DefaultValue = 0, MaxValue=1, MinValue=0)]
        public int paramFormat24HR { get; set; }
        
        public double TokyoOpen=9;
        public double TokyoClose=17;  //Some use a 6pm close for Tokyo.  I am using 5pm for all closings.
        public double SydneyOpen=9;
        public double SydneyClose=17; //Some use a 4pm close for Sydney
        public double LondonOpen=8;
        public double LondonClose=17;
        public double NYOpen=8;
        public double NYClose=17;
        
        public override void Calculate(int index)
        {
        TimeZoneInfo LocalTimeZone = TimeZoneInfo.Local;
                
        //Used to verify local time zone settings
        //Print("Local Time Zone Name:{0} Offset:{1} DST:{2} ",LocalTimeZone.DisplayName,LocalTimeZone.BaseUtcOffset,LocalTimeZone.SupportsDaylightSavingTime);
        
        TimeZoneInfo TokyoTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
        TimeZoneInfo NYTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        TimeZoneInfo LondonTimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
        TimeZoneInfo SydneyTimeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
		
		DateTime LocalTime=DateTime.Now;
		DateTime SydneyTime=TimeZoneInfo.ConvertTime(LocalTime,LocalTimeZone,SydneyTimeZone);
        DateTime LondonTime=TimeZoneInfo.ConvertTime(LocalTime,LocalTimeZone,LondonTimeZone);
        DateTime TokyoTime=TimeZoneInfo.ConvertTime(LocalTime,LocalTimeZone,TokyoTimeZone);
        DateTime NYTime=TimeZoneInfo.ConvertTime(LocalTime,LocalTimeZone,NYTimeZone);
        
        Colors LondonColor=Colors.White;
        Colors NYColor=Colors.White;
        Colors SydneyColor=Colors.White;
        Colors TokyoColor=Colors.White;
        
        string strLondonTime;
 		string strNYTime;
 		string strSydneyTime;
 		string strTokyoTime;
        
        string strLondonLabel=string.Format("{0,-60}","London");
        string strNYLabel=string.Format("{0,-20}","NY");
        string strSydneyLabel=string.Format("{0,25}","Sydney");
        string strTokyoLabel=string.Format("{0,60}","Tokyo");
        
        if (paramFormat24HR==1){
        strLondonTime=string.Format("\n{0,-60}",LondonTime.ToString("H:mm"));
        strNYTime=string.Format("\n{0,-25}",NYTime.ToString("H:mm"));
		strSydneyTime=string.Format("\n{0,25}",SydneyTime.ToString("H:mm"));
		strTokyoTime=string.Format("\n{0,60}",TokyoTime.ToString("H:mm"));        
        }
        else{
        strLondonTime=string.Format("\n{0,-60}",LondonTime.ToString("h:mmtt"));
        strNYTime=string.Format("\n{0,-25}",NYTime.ToString("h:mmtt"));
		strSydneyTime=string.Format("\n{0,25}",SydneyTime.ToString("h:mmtt"));
		strTokyoTime=string.Format("\n{0,60}",TokyoTime.ToString("h:mmtt"));
		}
		
        if(LondonTime.Hour>=LondonOpen & LondonTime.Hour<LondonClose)LondonColor=Colors.Yellow;
        if(NYTime.Hour>=NYOpen & NYTime.Hour<NYClose)NYColor=Colors.Blue;
       	if(SydneyTime.Hour>=SydneyOpen & SydneyTime.Hour<SydneyClose)SydneyColor=Colors.Green;      
       	if(TokyoTime.Hour>=TokyoOpen & TokyoTime.Hour<TokyoClose)TokyoColor=Colors.Red;
       	
       	ChartObjects.DrawText("TimeLabel1", strLondonLabel, StaticPosition.TopCenter, LondonColor);
       	ChartObjects.DrawText("TimeLabel2", strNYLabel, StaticPosition.TopCenter, NYColor);
       	ChartObjects.DrawText("TimeLabel3", strSydneyLabel, StaticPosition.TopCenter, SydneyColor);
       	ChartObjects.DrawText("TimeLabel4", strTokyoLabel, StaticPosition.TopCenter, TokyoColor);
       	ChartObjects.DrawText("Time1", strLondonTime, StaticPosition.TopCenter, LondonColor);
       	ChartObjects.DrawText("Time2", strNYTime, StaticPosition.TopCenter, NYColor);
       	ChartObjects.DrawText("Time3", strSydneyTime, StaticPosition.TopCenter, SydneyColor);
       	ChartObjects.DrawText("Time4", strTokyoTime, StaticPosition.TopCenter, TokyoColor);
        }
    }
}