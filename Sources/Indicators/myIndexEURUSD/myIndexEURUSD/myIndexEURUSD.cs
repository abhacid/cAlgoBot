using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AutoRescale = true, ScalePrecision = 0, TimeZone = TimeZones.UTC)]
    [Levels(-75, 75, -50, 50, -25, 25, 10, -10, 0, 100,
    -100)]
    public class myIndexEURUSD : Indicator
    {
        [Parameter(DefaultValue = false)]
        public bool HideYeilds { get; set; }

        [Output("USD Index", Color = Colors.Green)]
        public IndicatorDataSeries USDIDX { get; set; }
        [Output("USD Points", Color = Colors.LightGreen, Thickness = 2, PlotType = PlotType.Points)]
        public IndicatorDataSeries USDIDXPoints { get; set; }
        [Output("EUR Index", Color = Colors.Blue)]
        public IndicatorDataSeries EURIDX { get; set; }
        [Output("EUR Points", Color = Colors.LightBlue, Thickness = 2, PlotType = PlotType.Points)]
        public IndicatorDataSeries EURIDXPoints { get; set; }
        [Output("ProjYld", Color = Colors.Red)]
        public IndicatorDataSeries ProjYld { get; set; }
        [Output("ProjYld Points", Color = Colors.Pink, Thickness = 2, PlotType = PlotType.Points)]
        public IndicatorDataSeries ProjYldPoints { get; set; }
        [Output("ActYld", Color = Colors.Yellow)]
        public IndicatorDataSeries ActYld { get; set; }
        [Output("ActYld Points", Color = Colors.LightYellow, Thickness = 2, PlotType = PlotType.Points)]
        public IndicatorDataSeries ActYldPoints { get; set; }

        [Output("Delta", PlotType = PlotType.Histogram, Color = Colors.Purple)]
        public IndicatorDataSeries Delta { get; set; }

        //[Output("EURUSD", Color = Colors.Blue)]
        //public IndicatorDataSeries Yeild1 { get; set; }
        [Output("USDJPY", Color = Colors.Red)]
        public IndicatorDataSeries YldUSDJPY { get; set; }
        [Output("GBPUSD", Color = Colors.Yellow)]
        public IndicatorDataSeries YldGBPUSD { get; set; }
        [Output("AUDUSD", Color = Colors.Purple)]
        public IndicatorDataSeries YldAUDUSD { get; set; }
        [Output("USDCHF", Color = Colors.OrangeRed)]
        public IndicatorDataSeries YldUSDCHF { get; set; }

        [Output("EURJPY", Color = Colors.Pink)]
        public IndicatorDataSeries YldEURJPY { get; set; }
        [Output("EURGBP", Color = Colors.LightYellow)]
        public IndicatorDataSeries YldEURGBP { get; set; }
        [Output("EURAUD", Color = Colors.Orchid)]
        public IndicatorDataSeries YldEURAUD { get; set; }
        [Output("EURCHF", Color = Colors.Orange)]
        public IndicatorDataSeries YldEURCHF { get; set; }

        [Output("BuySignal", Color = Colors.Blue, Thickness = 5, PlotType = PlotType.Points)]
        public IndicatorDataSeries BuySignal { get; set; }
        [Output("SellSignal", Color = Colors.Red, Thickness = 5, PlotType = PlotType.Points)]
        public IndicatorDataSeries SellSignal { get; set; }

        [Output("Center", LineStyle = LineStyle.DotsRare, Color = Colors.White)]
        public IndicatorDataSeries CenterLine { get; set; }

        private MarketSeries msUSDJPY, msGBPUSD, msAUDUSD, msUSDCHF, msEURJPY, msEURGBP, msEURAUD, msEURCHF;

        protected override void Initialize()
        {
            string IndicatorName = GetType().ToString().Substring(GetType().ToString().LastIndexOf('.') + 1);
            //  returns ClassName
            Print("Indicator: " + IndicatorName);
            Print("IndicatorTimeZone: {0} Offset: {1} DST: {2}", TimeZone, TimeZone.BaseUtcOffset, TimeZone.SupportsDaylightSavingTime);

            msUSDJPY = MarketData.GetSeries("USDJPY", TimeFrame);
            msGBPUSD = MarketData.GetSeries("GBPUSD", TimeFrame);
            msAUDUSD = MarketData.GetSeries("AUDUSD", TimeFrame);
            msUSDCHF = MarketData.GetSeries("USDCHF", TimeFrame);

            msEURJPY = MarketData.GetSeries("EURJPY", TimeFrame);
            msEURGBP = MarketData.GetSeries("EURGBP", TimeFrame);
            msEURAUD = MarketData.GetSeries("EURAUD", TimeFrame);
            msEURCHF = MarketData.GetSeries("EURCHF", TimeFrame);
        }

        public override void Calculate(int index)
        {
            if (index < 1)
                return;

            int idxEURUSD = index;

            var idxUSDJPY = msUSDJPY.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idxEURUSD]);
            var idxGBPUSD = msGBPUSD.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idxEURUSD]);
            var idxAUDUSD = msAUDUSD.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idxEURUSD]);
            var idxUSDCHF = msUSDCHF.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idxEURUSD]);

            var idxEURJPY = msEURJPY.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idxEURUSD]);
            var idxEURGBP = msEURGBP.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idxEURUSD]);
            var idxEURAUD = msEURAUD.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idxEURUSD]);
            var idxEURCHF = msEURCHF.OpenTime.GetIndexByTime(MarketSeries.OpenTime[idxEURUSD]);

            int idxEURUSDn = DailyPeriodAdjustment(MarketSeries, idxEURUSD);

            var idxUSDJPYn = DailyPeriodAdjustment(msUSDJPY, idxUSDJPY);
            var idxGBPUSDn = DailyPeriodAdjustment(msGBPUSD, idxGBPUSD);
            var idxAUDUSDn = DailyPeriodAdjustment(msAUDUSD, idxAUDUSD);
            var idxUSDCHFn = DailyPeriodAdjustment(msUSDCHF, idxUSDCHF);

            var idxEURJPYn = DailyPeriodAdjustment(msEURJPY, idxEURJPY);
            var idxEURGBPn = DailyPeriodAdjustment(msEURGBP, idxEURGBP);
            var idxEURAUDn = DailyPeriodAdjustment(msEURAUD, idxEURAUD);
            var idxEURCHFn = DailyPeriodAdjustment(msEURCHF, idxEURCHF);

            double yldEURUSD = ((MarketSeries.Close[idxEURUSD] - MarketSeries.Close[idxEURUSD - idxEURUSDn]) / MarketSeries.Close[idxEURUSD - idxEURUSDn]) * 10000;

            double yldUSDJPY = ((msUSDJPY.Close[idxUSDJPY] - msUSDJPY.Close[idxUSDJPY - idxUSDJPYn]) / msUSDJPY.Close[idxUSDJPY - idxUSDJPYn]) * 10000;
            double yldGBPUSD = -((msGBPUSD.Close[idxGBPUSD] - msGBPUSD.Close[idxGBPUSD - idxGBPUSDn]) / msGBPUSD.Close[idxGBPUSD - idxGBPUSDn]) * 10000;
            double yldAUDUSD = -((msAUDUSD.Close[idxAUDUSD] - msAUDUSD.Close[idxAUDUSD - idxAUDUSDn]) / msAUDUSD.Close[idxAUDUSD - idxAUDUSDn]) * 10000;
            double yldUSDCHF = ((msUSDCHF.Close[idxUSDCHF] - msUSDCHF.Close[idxUSDCHF - idxUSDCHFn]) / msUSDCHF.Close[idxUSDCHF - idxUSDCHFn]) * 10000;

            double yldEURJPY = ((msEURJPY.Close[idxEURJPY] - msEURJPY.Close[idxEURJPY - idxEURJPYn]) / msEURJPY.Close[idxEURJPY - idxEURJPYn]) * 10000;
            double yldEURGBP = ((msEURGBP.Close[idxEURGBP] - msEURGBP.Close[idxEURGBP - idxEURGBPn]) / msEURGBP.Close[idxEURGBP - idxEURGBPn]) * 10000;
            double yldEURAUD = ((msEURAUD.Close[idxEURAUD] - msEURAUD.Close[idxEURAUD - idxEURAUDn]) / msEURAUD.Close[idxEURAUD - idxEURAUDn]) * 10000;
            double yldEURCHF = ((msEURCHF.Close[idxEURCHF] - msEURCHF.Close[idxEURCHF - idxEURCHFn]) / msEURCHF.Close[idxEURCHF - idxEURCHFn]) * 10000;

            if (!HideYeilds)
            {
                YldUSDJPY[index] = yldUSDJPY;
                YldGBPUSD[index] = yldGBPUSD;
                YldAUDUSD[index] = yldAUDUSD;
                YldUSDCHF[index] = yldUSDCHF;
                YldEURJPY[index] = yldEURJPY;
                YldEURGBP[index] = yldEURGBP;
                YldEURAUD[index] = yldEURAUD;
                YldEURCHF[index] = yldEURCHF;
            }

            double usdidx0 = (yldUSDJPY + yldGBPUSD + yldAUDUSD + yldUSDCHF) / 4;
            double euridx0 = (yldEURJPY + yldEURGBP + yldEURAUD + yldEURCHF) / 4;
            double usdidx1 = USDIDX[index - 1];
            double euridx1 = EURIDX[index - 1];
            double USDdelta = usdidx0 - usdidx1;
            double EURdelta = euridx0 - euridx1;

            USDIDX[index] = usdidx0;
            USDIDXPoints[index] = usdidx0;

            EURIDX[index] = euridx0;
            EURIDXPoints[index] = euridx0;

            ActYld[index] = yldEURUSD;
            ActYldPoints[index] = yldEURUSD;
            ProjYld[index] = euridx0 - usdidx0;
            ProjYldPoints[index] = euridx0 - usdidx0;

            Delta[index] = EURdelta - USDdelta;
            //if(USDdelta>0 && EURdelta<0 && -EURdelta>=USDdelta)SellSignal[index]=0;
            //if(delta1<0 && delta2>0)BuySignal[index]=0;

            CenterLine[index] = 0;

            string USDTxt = string.Format("USD{0}", Math.Round(USDIDX[index], 0));
            string EURTxt = string.Format("EUR{0}", Math.Round(EURIDX[index], 0));
            string ACTTxt = string.Format("Act{0}", Math.Round(ActYld[index], 0));
            string PROJTxt = string.Format("Pro{0}", Math.Round(ProjYld[index], 0));
            ChartObjects.DrawText("USDlabel", USDTxt, index, USDIDX[index], VerticalAlignment.Center, HorizontalAlignment.Right, Colors.Green);
            ChartObjects.DrawText("EURlabel", EURTxt, index, EURIDX[index], VerticalAlignment.Center, HorizontalAlignment.Right, Colors.Blue);
            ChartObjects.DrawText("ACTlabel", ACTTxt, index, ActYld[index], VerticalAlignment.Top, HorizontalAlignment.Right, Colors.Yellow);
            ChartObjects.DrawText("PROJlabel", PROJTxt, index, ProjYld[index], VerticalAlignment.Bottom, HorizontalAlignment.Right, Colors.Red);

        }

        private int DailyPeriodAdjustment(MarketSeries ms, int index)
        {
            if (index < 1)
                return 0;

            int periods = 1;

            DateTime CurrentDate = ms.OpenTime[index].AddHours(2);
            DateTime PreviousDate = ms.OpenTime[index - periods].AddHours(2);
            int DateDifference = (int)(CurrentDate.Date - PreviousDate.Date).TotalDays;

            while (CurrentDate.DayOfWeek == PreviousDate.DayOfWeek || PreviousDate.DayOfWeek == DayOfWeek.Sunday || CurrentDate.DayOfWeek == DayOfWeek.Saturday)
            {
                periods++;
                if (index < periods)
                    return periods - 1;
                PreviousDate = ms.OpenTime[index - periods].AddHours(2);
                DateDifference = (int)(CurrentDate.Date - PreviousDate.Date).TotalDays;
            }
            return periods - 1;
        }
    }
}
