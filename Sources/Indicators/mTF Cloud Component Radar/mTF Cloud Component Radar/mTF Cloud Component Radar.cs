using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class mTFCloudComponentRadar : Indicator
    {
        [Parameter("Timeframe 1", DefaultValue = "Minute")]
        public TimeFrame TF1 { get; set; }
        [Parameter("Timeframe 2", DefaultValue = "Minute5")]
        public TimeFrame TF2 { get; set; }
        [Parameter("Timeframe 3", DefaultValue = "Minute15")]
        public TimeFrame TF3 { get; set; }
        [Parameter("Timeframe 4", DefaultValue = "Minute30")]
        public TimeFrame TF4 { get; set; }
        [Parameter("Timeframe 5", DefaultValue = "Hour")]
        public TimeFrame TF5 { get; set; }
        [Parameter("Timeframe 6", DefaultValue = "Hour4")]
        public TimeFrame TF6 { get; set; }
        [Parameter("Timeframe 7", DefaultValue = "Hour12")]
        public TimeFrame TF7 { get; set; }
        [Parameter("Timeframe 8", DefaultValue = "Daily")]
        public TimeFrame TF8 { get; set; }



        public MarketSeries series1;
        public MarketSeries series2;
        public MarketSeries series3;
        public MarketSeries series4;
        public MarketSeries series5;
        public MarketSeries series6;
        public MarketSeries series7;
        public MarketSeries series8;

        public MarketSeries series;

        public IchimokuKinkoHyo cloud1;
        public IchimokuKinkoHyo cloud2;
        public IchimokuKinkoHyo cloud3;
        public IchimokuKinkoHyo cloud4;
        public IchimokuKinkoHyo cloud5;
        public IchimokuKinkoHyo cloud6;
        public IchimokuKinkoHyo cloud7;
        public IchimokuKinkoHyo cloud8;

        public IchimokuKinkoHyo cloud;


        public int ind = 1, currindex, pastindex;

        public string[] PrC = new string[8];
        public string[] KjC = new string[8];
        public string[] ChC = new string[8];
        public string[] PrK = new string[8];
        public string[] ChK = new string[8];
        public string[] TnK = new string[8];
        public string[] ChP = new string[8];
        public string[] Res = new string[8];

        public string[] Space;

        public string Line0 = "\n";
        public string Line1 = "\n\n";
        public string Line2 = "\n\n\n";
        public string Line3 = "\n\n\n\n";
        public string Line4 = "\n\n\n\n\n";
        public string Line5 = "\n\n\n\n\n\n";
        public string Line6 = "\n\n\n\n\n\n\n";
        public string Line7 = "\n\n\n\n\n\n\n\n";
        public string Line8 = "\n\n\n\n\n\n\n\n\n";


        public TimeFrame _time;

        public double Pr, spanA, spanB, kijun, Tenkan, Chikou, PastKijun, PastPrice, PastSpanA, PastspanB;

        public Colors BullColor = Colors.DodgerBlue;
        public Colors BearColor = Colors.Red;
        public Colors NeutralColor = Colors.Yellow;

        public Colors[] PrCColor = new Colors[8];
        public Colors[] KjCColor = new Colors[8];
        public Colors[] ChCColor = new Colors[8];
        public Colors[] PrKColor = new Colors[8];
        public Colors[] ChKColor = new Colors[8];
        public Colors[] TnKColor = new Colors[8];
        public Colors[] ChPColor = new Colors[8];

        public string Bull = "UP";
        public string Bear = "DN";
        public string Neutral = "NT";


        protected override void Initialize()
        {


            series1 = MarketData.GetSeries(TF1);
            series2 = MarketData.GetSeries(TF2);
            series3 = MarketData.GetSeries(TF3);
            series4 = MarketData.GetSeries(TF4);
            series5 = MarketData.GetSeries(TF5);
            series6 = MarketData.GetSeries(TF6);
            series7 = MarketData.GetSeries(TF7);
            series8 = MarketData.GetSeries(TF8);

            cloud1 = Indicators.IchimokuKinkoHyo(series1, 9, 26, 52);
            cloud2 = Indicators.IchimokuKinkoHyo(series2, 9, 26, 52);
            cloud3 = Indicators.IchimokuKinkoHyo(series3, 9, 26, 52);
            cloud4 = Indicators.IchimokuKinkoHyo(series4, 9, 26, 52);
            cloud5 = Indicators.IchimokuKinkoHyo(series5, 9, 26, 52);
            cloud6 = Indicators.IchimokuKinkoHyo(series6, 9, 26, 52);
            cloud7 = Indicators.IchimokuKinkoHyo(series7, 9, 26, 52);
            cloud8 = Indicators.IchimokuKinkoHyo(series8, 9, 26, 52);

        }

        public override void Calculate(int index)
        {
            string LAB0 = string.Format("{0,0}", "------------------------Cloud-----------------KijunSen--------Past Price---");
            string LAB1 = string.Format("\n{0,-65}", "Pr");
            string LAB2 = string.Format("\n{0,-45}", "Kj");
            string LAB3 = string.Format("\n{0,-25}", "Ch");
            string LAB4 = string.Format("\n{0,15}", "Pr");
            string LAB5 = string.Format("\n{0,35}", "Ch");
            string LAB6 = string.Format("\n{0,55}", "Tn");
            string LAB7 = string.Format("\n{0,85}", "Ch");
            string LAB8 = string.Format("\n\n\n\n\n\n\n\n\n\n{0,-60}", "Pr: Price,   Kj: KijunSen,   Ch: Chikou,   Tn: TenkanSen");

            ChartObjects.DrawText("LABEL0", LAB0, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABEL1", LAB1, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABEL2", LAB2, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABEL3", LAB3, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABEL4", LAB4, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABEL5", LAB5, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABEL6", LAB6, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABEL7", LAB7, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABEL8", LAB8, StaticPosition.TopCenter, Colors.White);

            string tfLAB0 = string.Format("\n{0,-105}", "Component");
            string tfLAB1 = string.Format("\n\n{0,-100}", TF1);
            string tfLAB2 = string.Format("\n\n\n{0,-100}", TF2);
            string tfLAB3 = string.Format("\n\n\n\n{0,-100}", TF3);
            string tfLAB4 = string.Format("\n\n\n\n\n{0,-100}", TF4);
            string tfLAB5 = string.Format("\n\n\n\n\n\n{0,-100}", TF5);
            string tfLAB6 = string.Format("\n\n\n\n\n\n\n{0,-100}", TF6);
            string tfLAB7 = string.Format("\n\n\n\n\n\n\n\n{0,-100}", TF7);
            string tfLAB8 = string.Format("\n\n\n\n\n\n\n\n\n{0,-100}", TF8);

            ChartObjects.DrawText("tfLABEL0", tfLAB0, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("tfLABEL1", tfLAB1, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("tfLABEL2", tfLAB2, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("tfLABEL3", tfLAB3, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("tfLABEL4", tfLAB4, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("tfLABEL5", tfLAB5, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("tfLABEL6", tfLAB6, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("tfLABEL7", tfLAB7, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("tfLABEL8", tfLAB8, StaticPosition.TopCenter, Colors.White);


            for (int i = 0; i < 8; i++)
            {

                switch (i)
                {
                    case 0:
                        currindex = series1.Close.Count - ind;
                        pastindex = currindex - 26;
                        Pr = series1.Close[currindex];
                        spanA = cloud1.SenkouSpanA[currindex];
                        spanB = cloud1.SenkouSpanB[currindex];
                        kijun = cloud1.KijunSen[currindex];
                        Tenkan = cloud1.TenkanSen[currindex];
                        Chikou = cloud1.ChikouSpan[pastindex];
                        PastKijun = cloud1.KijunSen[pastindex];
                        PastPrice = series1.Close[pastindex];
                        PastSpanA = cloud1.SenkouSpanA[pastindex];
                        PastspanB = cloud1.SenkouSpanB[pastindex];
                        break;

                    case 1:
                        currindex = series2.Close.Count - ind;
                        pastindex = currindex - 26;
                        Pr = series2.Close[currindex];
                        spanA = cloud2.SenkouSpanA[currindex];
                        spanB = cloud2.SenkouSpanB[currindex];
                        kijun = cloud2.KijunSen[currindex];
                        Tenkan = cloud2.TenkanSen[currindex];
                        Chikou = cloud2.ChikouSpan[pastindex];
                        PastKijun = cloud2.KijunSen[pastindex];
                        PastPrice = series2.Close[pastindex];
                        PastSpanA = cloud2.SenkouSpanA[pastindex];
                        PastspanB = cloud2.SenkouSpanB[pastindex];
                        break;

                    case 2:
                        currindex = series3.Close.Count - ind;
                        pastindex = currindex - 26;
                        Pr = series3.Close[currindex];
                        spanA = cloud3.SenkouSpanA[currindex];
                        spanB = cloud3.SenkouSpanB[currindex];
                        kijun = cloud3.KijunSen[currindex];
                        Tenkan = cloud3.TenkanSen[currindex];
                        Chikou = cloud3.ChikouSpan[pastindex];
                        PastKijun = cloud3.KijunSen[pastindex];
                        PastPrice = series3.Close[pastindex];
                        PastSpanA = cloud3.SenkouSpanA[pastindex];
                        PastspanB = cloud3.SenkouSpanB[pastindex];
                        break;

                    case 3:
                        currindex = series4.Close.Count - ind;
                        pastindex = currindex - 26;
                        Pr = series4.Close[currindex];
                        spanA = cloud4.SenkouSpanA[currindex];
                        spanB = cloud4.SenkouSpanB[currindex];
                        kijun = cloud4.KijunSen[currindex];
                        Tenkan = cloud4.TenkanSen[currindex];
                        Chikou = cloud4.ChikouSpan[pastindex];
                        PastKijun = cloud4.KijunSen[pastindex];
                        PastPrice = series4.Close[pastindex];
                        PastSpanA = cloud4.SenkouSpanA[pastindex];
                        PastspanB = cloud4.SenkouSpanB[pastindex];
                        break;

                    case 4:
                        currindex = series5.Close.Count - ind;
                        pastindex = currindex - 26;
                        Pr = series5.Close[currindex];
                        spanA = cloud5.SenkouSpanA[currindex];
                        spanB = cloud5.SenkouSpanB[currindex];
                        kijun = cloud5.KijunSen[currindex];
                        Tenkan = cloud5.TenkanSen[currindex];
                        Chikou = cloud5.ChikouSpan[pastindex];
                        PastKijun = cloud5.KijunSen[pastindex];
                        PastPrice = series5.Close[pastindex];
                        PastSpanA = cloud5.SenkouSpanA[pastindex];
                        PastspanB = cloud5.SenkouSpanB[pastindex];
                        break;
                    case 5:
                        currindex = series6.Close.Count - ind;
                        pastindex = currindex - 26;
                        Pr = series6.Close[currindex];
                        spanA = cloud6.SenkouSpanA[currindex];
                        spanB = cloud6.SenkouSpanB[currindex];
                        kijun = cloud6.KijunSen[currindex];
                        Tenkan = cloud6.TenkanSen[currindex];
                        Chikou = cloud6.ChikouSpan[pastindex];
                        PastKijun = cloud6.KijunSen[pastindex];
                        PastPrice = series6.Close[pastindex];
                        PastSpanA = cloud6.SenkouSpanA[pastindex];
                        PastspanB = cloud6.SenkouSpanB[pastindex];
                        break;

                    case 6:
                        currindex = series7.Close.Count - ind;
                        pastindex = currindex - 26;
                        Pr = series7.Close[currindex];
                        spanA = cloud7.SenkouSpanA[currindex];
                        spanB = cloud7.SenkouSpanB[currindex];
                        kijun = cloud7.KijunSen[currindex];
                        Tenkan = cloud7.TenkanSen[currindex];
                        Chikou = cloud7.ChikouSpan[pastindex];
                        PastKijun = cloud7.KijunSen[pastindex];
                        PastPrice = series7.Close[pastindex];
                        PastSpanA = cloud7.SenkouSpanA[pastindex];
                        PastspanB = cloud7.SenkouSpanB[pastindex];
                        break;
                    case 7:
                        currindex = series8.Close.Count - ind;
                        pastindex = currindex - 26;
                        Pr = series8.Close[currindex];
                        spanA = cloud8.SenkouSpanA[currindex];
                        spanB = cloud8.SenkouSpanB[currindex];
                        kijun = cloud8.KijunSen[currindex];
                        Tenkan = cloud8.TenkanSen[currindex];
                        Chikou = cloud8.ChikouSpan[pastindex];
                        PastKijun = cloud8.KijunSen[pastindex];
                        PastPrice = series8.Close[pastindex];
                        PastSpanA = cloud8.SenkouSpanA[pastindex];
                        PastspanB = cloud8.SenkouSpanB[pastindex];
                        break;
                }




                //Price compared to Cloud 
                if (Pr < spanA && Pr < spanB)
                {
                    PrC[i] = string.Format("{0,-65}", Bear);
                    PrCColor[i] = BearColor;
                }
                else if (Pr > spanA && Pr > spanB)
                {
                    PrC[i] = string.Format("{0,-65}", Bull);
                    PrCColor[i] = BullColor;
                }
                else
                {
                    PrC[i] = string.Format("{0,-65}", Neutral);
                    PrCColor[i] = NeutralColor;
                }
                //KijunSen compared to Cloud
                if (kijun < spanA && kijun < spanB)
                {
                    KjC[i] = string.Format("{0,-45}", Bear);
                    KjCColor[i] = BearColor;
                }
                else if (kijun > spanA && kijun > spanB)
                {
                    KjC[i] = string.Format("{0,-45}", Bull);
                    KjCColor[i] = BullColor;
                }
                else
                {
                    KjC[i] = string.Format("{0,-45}", Neutral);
                    KjCColor[i] = NeutralColor;
                }
                //ChikouSpan compared to Cloud
                if (Chikou < PastSpanA && Chikou < PastspanB)
                {
                    ChC[i] = string.Format("{0,-25}", Bear);
                    ChCColor[i] = BearColor;
                }
                else if (Chikou > PastSpanA && Chikou > PastspanB)
                {
                    ChC[i] = string.Format("{0,-25}", Bull);
                    ChCColor[i] = BullColor;
                }
                else
                {
                    ChC[i] = string.Format("{0,-25}", Neutral);
                    ChCColor[i] = NeutralColor;
                }

                //Price compared to KijunSen
                if (Pr < kijun)
                {
                    PrK[i] = string.Format("{0,15}", Bear);
                    PrKColor[i] = BearColor;
                }
                else if (Pr > kijun)
                {
                    PrK[i] = string.Format("{0,15}", Bull);
                    PrKColor[i] = BullColor;
                }
                else
                {
                    PrK[i] = string.Format("{0,15}", Neutral);
                    PrKColor[i] = NeutralColor;
                }
                //ChikouSpan compared to Past KijunSen
                if (Chikou < PastKijun)
                {
                    ChK[i] = string.Format("{0,35}", Bear);
                    ChKColor[i] = BearColor;
                }
                else if (Chikou > PastKijun)
                {
                    ChK[i] = string.Format("{0,35}", Bull);
                    ChKColor[i] = BullColor;
                }
                else
                {
                    ChK[i] = string.Format("{0,35}", Neutral);
                    ChKColor[i] = NeutralColor;
                }
                //TenkanSen compared to Past KijunSen
                if (Tenkan < kijun)
                {
                    TnK[i] = string.Format("{0,55}", Bear);
                    TnKColor[i] = BearColor;
                }
                else if (Tenkan > kijun)
                {
                    TnK[i] = string.Format("{0,55}", Bull);
                    TnKColor[i] = BullColor;
                }
                else
                {
                    TnK[i] = string.Format("{0,55}", Neutral);
                    TnKColor[i] = NeutralColor;
                }

                //ChikouSpan compared to Past Price
                if (Chikou < PastPrice)
                {
                    ChP[i] = string.Format("{0,85}", Bear);
                    ChPColor[i] = BearColor;
                }
                else if (Chikou > PastPrice)
                {
                    ChP[i] = string.Format("{0,85}", Bull);
                    ChPColor[i] = BullColor;
                }
                else
                {
                    ChP[i] = string.Format("{0,85}", Neutral);
                    ChPColor[i] = NeutralColor;
                }



            }

            ChartObjects.DrawText("_PrC0", Line1 + PrC[0], StaticPosition.TopCenter, PrCColor[0]);
            ChartObjects.DrawText("_PrC1", Line2 + PrC[1], StaticPosition.TopCenter, PrCColor[1]);
            ChartObjects.DrawText("_PrC2", Line3 + PrC[2], StaticPosition.TopCenter, PrCColor[2]);
            ChartObjects.DrawText("_PrC3", Line4 + PrC[3], StaticPosition.TopCenter, PrCColor[3]);
            ChartObjects.DrawText("_PrC4", Line5 + PrC[4], StaticPosition.TopCenter, PrCColor[4]);
            ChartObjects.DrawText("_PrC5", Line6 + PrC[5], StaticPosition.TopCenter, PrCColor[5]);
            ChartObjects.DrawText("_PrC6", Line7 + PrC[6], StaticPosition.TopCenter, PrCColor[6]);
            ChartObjects.DrawText("_PrC7", Line8 + PrC[7], StaticPosition.TopCenter, PrCColor[7]);

            ChartObjects.DrawText("_KjC0", Line1 + KjC[0], StaticPosition.TopCenter, KjCColor[0]);
            ChartObjects.DrawText("_KjC1", Line2 + KjC[1], StaticPosition.TopCenter, KjCColor[1]);
            ChartObjects.DrawText("_KjC2", Line3 + KjC[2], StaticPosition.TopCenter, KjCColor[2]);
            ChartObjects.DrawText("_KjC3", Line4 + KjC[3], StaticPosition.TopCenter, KjCColor[3]);
            ChartObjects.DrawText("_KjC4", Line5 + KjC[4], StaticPosition.TopCenter, KjCColor[4]);
            ChartObjects.DrawText("_KjC5", Line6 + KjC[5], StaticPosition.TopCenter, KjCColor[5]);
            ChartObjects.DrawText("_KjC6", Line7 + KjC[6], StaticPosition.TopCenter, KjCColor[6]);
            ChartObjects.DrawText("_KjC7", Line8 + KjC[7], StaticPosition.TopCenter, KjCColor[7]);

            ChartObjects.DrawText("_ChC0", Line1 + ChC[0], StaticPosition.TopCenter, ChCColor[0]);
            ChartObjects.DrawText("_ChC1", Line2 + ChC[1], StaticPosition.TopCenter, ChCColor[1]);
            ChartObjects.DrawText("_ChC2", Line3 + ChC[2], StaticPosition.TopCenter, ChCColor[2]);
            ChartObjects.DrawText("_ChC3", Line4 + ChC[3], StaticPosition.TopCenter, ChCColor[3]);
            ChartObjects.DrawText("_ChC4", Line5 + ChC[4], StaticPosition.TopCenter, ChCColor[4]);
            ChartObjects.DrawText("_ChC5", Line6 + ChC[5], StaticPosition.TopCenter, ChCColor[5]);
            ChartObjects.DrawText("_ChC6", Line7 + ChC[6], StaticPosition.TopCenter, ChCColor[6]);
            ChartObjects.DrawText("_ChC7", Line8 + ChC[7], StaticPosition.TopCenter, ChCColor[7]);

            ChartObjects.DrawText("_PrK0", Line1 + PrK[0], StaticPosition.TopCenter, PrKColor[0]);
            ChartObjects.DrawText("_PrK1", Line2 + PrK[1], StaticPosition.TopCenter, PrKColor[1]);
            ChartObjects.DrawText("_PrK2", Line3 + PrK[2], StaticPosition.TopCenter, PrKColor[2]);
            ChartObjects.DrawText("_PrK3", Line4 + PrK[3], StaticPosition.TopCenter, PrKColor[3]);
            ChartObjects.DrawText("_PrK4", Line5 + PrK[4], StaticPosition.TopCenter, PrKColor[4]);
            ChartObjects.DrawText("_PrK5", Line6 + PrK[5], StaticPosition.TopCenter, PrKColor[5]);
            ChartObjects.DrawText("_PrK6", Line7 + PrK[6], StaticPosition.TopCenter, PrKColor[6]);
            ChartObjects.DrawText("_PrK7", Line8 + PrK[7], StaticPosition.TopCenter, PrKColor[7]);

            ChartObjects.DrawText("_ChK0", Line1 + ChK[0], StaticPosition.TopCenter, ChKColor[0]);
            ChartObjects.DrawText("_ChK1", Line2 + ChK[1], StaticPosition.TopCenter, ChKColor[1]);
            ChartObjects.DrawText("_ChK2", Line3 + ChK[2], StaticPosition.TopCenter, ChKColor[2]);
            ChartObjects.DrawText("_ChK3", Line4 + ChK[3], StaticPosition.TopCenter, ChKColor[3]);
            ChartObjects.DrawText("_ChK4", Line5 + ChK[4], StaticPosition.TopCenter, ChKColor[4]);
            ChartObjects.DrawText("_ChK5", Line6 + ChK[5], StaticPosition.TopCenter, ChKColor[5]);
            ChartObjects.DrawText("_ChK6", Line7 + ChK[6], StaticPosition.TopCenter, ChKColor[6]);
            ChartObjects.DrawText("_ChK7", Line8 + ChK[7], StaticPosition.TopCenter, ChKColor[7]);

            ChartObjects.DrawText("_TnK0", Line1 + TnK[0], StaticPosition.TopCenter, TnKColor[0]);
            ChartObjects.DrawText("_TnK1", Line2 + TnK[1], StaticPosition.TopCenter, TnKColor[1]);
            ChartObjects.DrawText("_TnK2", Line3 + TnK[2], StaticPosition.TopCenter, TnKColor[2]);
            ChartObjects.DrawText("_TnK3", Line4 + TnK[3], StaticPosition.TopCenter, TnKColor[3]);
            ChartObjects.DrawText("_TnK4", Line5 + TnK[4], StaticPosition.TopCenter, TnKColor[4]);
            ChartObjects.DrawText("_TnK5", Line6 + TnK[5], StaticPosition.TopCenter, TnKColor[5]);
            ChartObjects.DrawText("_TnK6", Line7 + TnK[6], StaticPosition.TopCenter, TnKColor[6]);
            ChartObjects.DrawText("_TnK7", Line8 + TnK[7], StaticPosition.TopCenter, TnKColor[7]);

            ChartObjects.DrawText("_ChP0", Line1 + ChP[0], StaticPosition.TopCenter, ChPColor[0]);
            ChartObjects.DrawText("_ChP1", Line2 + ChP[1], StaticPosition.TopCenter, ChPColor[1]);
            ChartObjects.DrawText("_ChP2", Line3 + ChP[2], StaticPosition.TopCenter, ChPColor[2]);
            ChartObjects.DrawText("_ChP3", Line4 + ChP[3], StaticPosition.TopCenter, ChPColor[3]);
            ChartObjects.DrawText("_ChP4", Line5 + ChP[4], StaticPosition.TopCenter, ChPColor[4]);
            ChartObjects.DrawText("_ChP5", Line6 + ChP[5], StaticPosition.TopCenter, ChPColor[5]);
            ChartObjects.DrawText("_ChP6", Line7 + ChP[6], StaticPosition.TopCenter, ChPColor[6]);
            ChartObjects.DrawText("_ChP7", Line8 + ChP[7], StaticPosition.TopCenter, ChPColor[7]);

        }





    }
}
