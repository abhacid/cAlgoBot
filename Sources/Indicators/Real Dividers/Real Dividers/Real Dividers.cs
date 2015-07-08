using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class RealDividers : Indicator
    {
        [Parameter(DefaultValue = false)]
        public bool HideWeekDividers {get; set;}
        [Parameter(DefaultValue = false)]
        public bool HideDayDividers {get; set;}
        [Parameter(DefaultValue = false)]
        public bool HideDayLabels {get; set;}
        
        [Output("Dayend",PlotType = PlotType.Points, Thickness = 3, Color = Colors.Orange)]
        public IndicatorDataSeries Dayend { get; set; }
        [Output("Weekend",PlotType = PlotType.Points, Thickness = 3, Color = Colors.Red)]
        public IndicatorDataSeries Weekend { get; set; }
        
        private int PeriodDivisor;
        private int PeriodsPerDay;

        protected override void Initialize()
        {
            Print("IndicatorTimeZone Setting: {0}", TimeZone);
            Print("Offset: {0}", TimeZone.BaseUtcOffset);
            Print("DST: {0}", TimeZone.SupportsDaylightSavingTime);
            switch (Convert.ToString(TimeFrame))
            {
                case "Minute": PeriodDivisor = 1;break;
                case "Minute5": PeriodDivisor = 5;break;
                case "Minute10": PeriodDivisor = 10;break;
                case "Minute15": PeriodDivisor = 15;break;
                case "Minute30": PeriodDivisor = 30;break;
                case "Hour": PeriodDivisor = 60;break;
                default: PeriodDivisor = 0;break;
            }
            PeriodsPerDay=720/PeriodDivisor;//Used to place label 
        }

        public override void Calculate(int index)
        { 
            if (index - 1 < 0)return;
            
            DateTime CurrentDate = MarketSeries.OpenTime[index].AddHours(2);
            DateTime PreviousDate = MarketSeries.OpenTime[index-1].AddHours(2);
            int DateDifference = (int)(CurrentDate.Date-PreviousDate.Date).TotalDays;
            
            //**Dayend
            if (CurrentDate.DayOfWeek != PreviousDate.DayOfWeek && PreviousDate.DayOfWeek!=DayOfWeek.Sunday)
            {
                if(!HideDayLabels)ChartObjects.DrawText("DayLabel"+index," "+Convert.ToString(CurrentDate.DayOfWeek),index,MarketSeries.Low.Minimum(PeriodsPerDay),VerticalAlignment.Bottom, HorizontalAlignment.Right);
                if(!HideDayDividers)ChartObjects.DrawVerticalLine("Dayend"+index, index, Colors.Orange, 1, LineStyle.DotsRare);
                Dayend[index]=MarketSeries.Median[index];
                //Print(CurrentDate.Date+" "+DateDifference);
            }

            //** Weekend
			
			if(CurrentDate.DayOfWeek<PreviousDate.DayOfWeek || DateDifference>6)
			//if(DateDifference>1)
			{
    		  if(!HideWeekDividers)ChartObjects.DrawVerticalLine("Weekend"+index, index, Colors.Red,1, LineStyle.DotsRare);
              Weekend[index]=MarketSeries.Median[index];
              //Print(CurrentDate.Date+" "+DateDifference);
            }
        }
    }
}
