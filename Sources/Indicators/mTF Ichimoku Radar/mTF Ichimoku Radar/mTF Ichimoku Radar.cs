using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class mTFIchimokuRadar : Indicator
    {
        private MarketSeries seriesM1;
        private MarketSeries seriesM5;
        private MarketSeries seriesM15;
        private MarketSeries seriesM30;
        private MarketSeries seriesH1;
        private MarketSeries seriesH4;
        private MarketSeries seriesH12;
        private MarketSeries seriesD1;

        private IchimokuKinkoHyo cloudM1;
        private IchimokuKinkoHyo cloudM5;
        private IchimokuKinkoHyo cloudM15;
        private IchimokuKinkoHyo cloudM30;
        private IchimokuKinkoHyo cloudH1;
        private IchimokuKinkoHyo cloudH4;
        private IchimokuKinkoHyo cloudH12;
        private IchimokuKinkoHyo cloudD1;

        public int ind = 1;

        private string resM1;
        private string resM5;
        private string resM15;
        private string resM30;
        private string resH1;
        private string resH4;
        private string resH12;
        private string resD1;


        protected override void Initialize()
        {
            seriesM1 = MarketData.GetSeries(TimeFrame.Minute);
            seriesM5 = MarketData.GetSeries(TimeFrame.Minute5);
            seriesM15 = MarketData.GetSeries(TimeFrame.Minute15);
            seriesM30 = MarketData.GetSeries(TimeFrame.Minute30);
            seriesH1 = MarketData.GetSeries(TimeFrame.Hour);
            seriesH4 = MarketData.GetSeries(TimeFrame.Hour4);
            seriesH12 = MarketData.GetSeries(TimeFrame.Hour12);
            seriesD1 = MarketData.GetSeries(TimeFrame.Daily);

            cloudM1 = Indicators.IchimokuKinkoHyo(seriesM1, 9, 26, 52);
            cloudM5 = Indicators.IchimokuKinkoHyo(seriesM5, 9, 26, 52);
            cloudM15 = Indicators.IchimokuKinkoHyo(seriesM15, 9, 26, 52);
            cloudM30 = Indicators.IchimokuKinkoHyo(seriesM30, 9, 26, 52);
            cloudH1 = Indicators.IchimokuKinkoHyo(seriesH1, 9, 26, 52);
            cloudH4 = Indicators.IchimokuKinkoHyo(seriesH4, 9, 26, 52);
            cloudH12 = Indicators.IchimokuKinkoHyo(seriesH12, 9, 26, 52);
            cloudD1 = Indicators.IchimokuKinkoHyo(seriesD1, 9, 26, 52);

        }

        public override void Calculate(int index)
        {
            string LABm1 = string.Format("{0,-180}", "m1");
            string LABm5 = string.Format("{0,-120}", "m5");
            string LABm15 = string.Format("{0,-60}", "m15");
            string LABm30 = string.Format("{0,0}", "m30");
            string LABh1 = string.Format("{0,60}", "H1");
            string LABh4 = string.Format("{0,120}", "H4");
            string LABh12 = string.Format("{0,180}", "H12");
            string LABd1 = string.Format("{0,240}", "D1");

            ChartObjects.DrawText("LABELm1", LABm1, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABELm5", LABm5, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABELm15", LABm15, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABELm30", LABm30, StaticPosition.TopCenter, Colors.White);

            ChartObjects.DrawText("LABELh1", LABh1, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABELh4", LABh4, StaticPosition.TopCenter, Colors.White);
            ChartObjects.DrawText("LABELh12", LABh12, StaticPosition.TopCenter, Colors.White);

            ChartObjects.DrawText("LABELd1", LABd1, StaticPosition.TopCenter, Colors.White);

            Colors m1Color = Colors.White;
            Colors m5Color = Colors.White;
            Colors m15Color = Colors.White;
            Colors m30Color = Colors.White;
            Colors h1Color = Colors.White;
            Colors h4Color = Colors.White;
            Colors h12Color = Colors.White;
            Colors d1Color = Colors.White;



            if (seriesM1.Close.LastValue < cloudM1.SenkouSpanA[seriesM1.Close.Count - ind] && seriesM1.Close.LastValue < cloudM1.SenkouSpanB[seriesM1.Close.Count - ind])
            {
                resM1 = string.Format("\n{0,-180}", "BEAR");
                m1Color = Colors.Red;
            }
            else if (seriesM1.Close.LastValue > cloudM1.SenkouSpanA[seriesM1.Close.Count - ind] && seriesM1.Close.LastValue > cloudM1.SenkouSpanB[seriesM1.Close.Count - ind])
            {
                resM1 = string.Format("\n{0,-180}", "BULL");
                m1Color = Colors.DodgerBlue;
            }
            else
            {
                resM1 = string.Format("\n{0,-180}", "NEUTRAL");
                m1Color = Colors.Yellow;
            }

            if (seriesM5.Close.LastValue < cloudM5.SenkouSpanA[seriesM5.Close.Count - ind] && seriesM5.Close.LastValue < cloudM5.SenkouSpanB[seriesM5.Close.Count - ind])
            {
                resM5 = string.Format("\n{0,-120}", "BEAR");
                m5Color = Colors.Red;
            }
            else if (seriesM5.Close.LastValue > cloudM5.SenkouSpanA[seriesM5.Close.Count - ind] && seriesM5.Close.LastValue > cloudM5.SenkouSpanB[seriesM5.Close.Count - ind])
            {
                resM5 = string.Format("\n{0,-120}", "BULL");
                m5Color = Colors.DodgerBlue;
            }
            else
            {
                resM5 = string.Format("\n{0,-120}", "NEUTRAL");
                m5Color = Colors.Yellow;
            }

            if (seriesM15.Close.LastValue < cloudM15.SenkouSpanA[seriesM15.Close.Count - ind] && seriesM15.Close.LastValue < cloudM15.SenkouSpanB[seriesM15.Close.Count - ind])
            {
                resM15 = string.Format("\n{0,-60}", "BEAR");
                m15Color = Colors.Red;
            }
            else if (seriesM15.Close.LastValue > cloudM15.SenkouSpanA[seriesM15.Close.Count - ind] && seriesM15.Close.LastValue > cloudM15.SenkouSpanB[seriesM15.Close.Count - ind])
            {
                resM15 = string.Format("\n{0,-60}", "BULL");
                m15Color = Colors.DodgerBlue;
            }
            else
            {
                resM15 = string.Format("\n{0,-60}", "NEUTRAL");
                m15Color = Colors.Yellow;
            }

            if (seriesM30.Close.LastValue < cloudM30.SenkouSpanA[seriesM30.Close.Count - ind] && seriesM30.Close.LastValue < cloudM30.SenkouSpanB[seriesM30.Close.Count - ind])
            {
                resM30 = string.Format("\n{0,0}", "BEAR");
                m30Color = Colors.Red;
            }
            else if (seriesM30.Close.LastValue > cloudM30.SenkouSpanA[seriesM30.Close.Count - ind] && seriesM30.Close.LastValue > cloudM30.SenkouSpanB[seriesM30.Close.Count - ind])
            {
                resM30 = string.Format("\n{0,0}", "BULL");
                m30Color = Colors.DodgerBlue;
            }
            else
            {
                resM30 = string.Format("\n{0,0}", "NEUTRAL");
                m30Color = Colors.Yellow;
            }

            if (seriesH1.Close.LastValue < cloudH1.SenkouSpanA[seriesH1.Close.Count - ind] && seriesH1.Close.LastValue < cloudH1.SenkouSpanB[seriesH1.Close.Count - ind])
            {
                resH1 = string.Format("\n{0,60}", "BEAR");
                h1Color = Colors.Red;
            }
            else if (seriesH1.Close.LastValue > cloudH1.SenkouSpanA[seriesH1.Close.Count - ind] && seriesH1.Close.LastValue > cloudH1.SenkouSpanB[seriesH1.Close.Count - ind])
            {
                resH1 = string.Format("\n{0,60}", "BULL");
                h1Color = Colors.DodgerBlue;
            }
            else
            {
                resH1 = string.Format("\n{0,60}", "NEUTRAL");
                h1Color = Colors.Yellow;
            }

            if (seriesH4.Close.LastValue < cloudH4.SenkouSpanA[seriesH4.Close.Count - ind] && seriesH4.Close.LastValue < cloudH4.SenkouSpanB[seriesH4.Close.Count - ind])
            {
                resH4 = string.Format("\n{0,120}", "BEAR");
                h4Color = Colors.Red;
            }
            else if (seriesH4.Close.LastValue > cloudH4.SenkouSpanA[seriesH4.Close.Count - ind] && seriesH4.Close.LastValue > cloudH4.SenkouSpanB[seriesH4.Close.Count - ind])
            {
                resH4 = string.Format("\n{0,120}", "BULL");
                h4Color = Colors.DodgerBlue;
            }
            else
            {
                resH4 = string.Format("\n{0,120}", "NEUTRAL");
                h4Color = Colors.Yellow;
            }

            if (seriesH12.Close.LastValue < cloudH12.SenkouSpanA[seriesH12.Close.Count - ind] && seriesH12.Close.LastValue < cloudH12.SenkouSpanB[seriesH12.Close.Count - ind])
            {
                resH12 = string.Format("\n{0,180}", "BEAR");
                h12Color = Colors.Red;
            }
            else if (seriesH12.Close.LastValue > cloudH12.SenkouSpanA[seriesH12.Close.Count - ind] && seriesH12.Close.LastValue > cloudH12.SenkouSpanB[seriesH12.Close.Count - ind])
            {
                resH12 = string.Format("\n{0,180}", "BULL");
                h12Color = Colors.DodgerBlue;
            }
            else
            {
                resH12 = string.Format("\n{0,180}", "NEUTRAL");
                h12Color = Colors.Yellow;
            }


            if (seriesD1.Close.LastValue < cloudD1.SenkouSpanA[seriesD1.Close.Count - ind] && seriesD1.Close.LastValue < cloudD1.SenkouSpanB[seriesD1.Close.Count - ind])
            {
                resD1 = string.Format("\n{0,240}", "BEAR");
                d1Color = Colors.Red;
            }
            else if (seriesD1.Close.LastValue > cloudD1.SenkouSpanA[seriesD1.Close.Count - ind] && seriesD1.Close.LastValue > cloudD1.SenkouSpanB[seriesD1.Close.Count - ind])
            {
                resD1 = string.Format("\n{0,240}", "BULL");
                d1Color = Colors.DodgerBlue;
            }
            else
            {
                resD1 = string.Format("\n{0,240}", "NEUTRAL");
                d1Color = Colors.Yellow;
            }


            ChartObjects.DrawText("m1", resM1, StaticPosition.TopCenter, m1Color);
            ChartObjects.DrawText("m5", resM5, StaticPosition.TopCenter, m5Color);
            ChartObjects.DrawText("m15", resM15, StaticPosition.TopCenter, m15Color);
            ChartObjects.DrawText("m30", resM30, StaticPosition.TopCenter, m30Color);

            ChartObjects.DrawText("H1", resH1, StaticPosition.TopCenter, h1Color);
            ChartObjects.DrawText("H4", resH4, StaticPosition.TopCenter, h4Color);
            ChartObjects.DrawText("H12", resH12, StaticPosition.TopCenter, h12Color);

            ChartObjects.DrawText("D1", resD1, StaticPosition.TopCenter, d1Color);

        }



    }
}
