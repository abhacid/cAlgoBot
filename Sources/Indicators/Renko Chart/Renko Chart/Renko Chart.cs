using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using System.Collections.Specialized;
using System.Net;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class RenkoChart : Indicator
    {

        private MarketSeries M1;
        public double[] buff = new double[401];
        public double c;
        public double nu;
        public double onu;
        public int ind;
        public int initt = 0;
        [Parameter("Renko Pips", DefaultValue = 10)]
        public int pips { get; set; }

        protected override void Initialize()
        {
            M1 = MarketData.GetSeries(TimeFrame.Minute);
        }

        public override void Calculate(int index)
        {
            if (!IsLastBar)
            {
                c = M1.Close.Last(7000) / Symbol.PipSize;
                nu = (c - (c % pips)) * Symbol.PipSize;
                onu = nu;
                buff[0] = nu;
                for (int i = 7000; i > 0; i--)
                {
                    c = M1.Median.Last(i);
                    push();
                }
                return;
            }
            ind = index;
            c = M1.Close.Last(0);
            push();
            for (int i = 0; i < 150; i++)
            {
                if (buff[i + 1] < buff[i])
                    ChartObjects.DrawLine(string.Format("li_{0}", ind - i), ind - i, buff[i + 1], ind - i, buff[i], Colors.SeaGreen, 5, LineStyle.Solid);
                else
                    ChartObjects.DrawLine(string.Format("li_{0}", ind - i), ind - i, buff[i + 1] - Symbol.PipSize * pips, ind - i, buff[i] - Symbol.PipSize * pips, Colors.Tomato, 5, LineStyle.Solid);
            }
        }
        private void push()
        {
            nu = c;
            while (nu < onu - Symbol.PipSize * pips * 1.1)
            {
                onu -= Symbol.PipSize * pips;
                ppush();
            }
            while (nu > onu + Symbol.PipSize * pips * 1.1)
            {
                onu += Symbol.PipSize * pips;
                ppush();
            }
        }

        private void ppush()
        {
            if (buff[0] == onu)
                return;
            for (int j = 400; j > 0; j--)
            {
                buff[j] = buff[j - 1];
            }
            buff[0] = onu;
        }
    }

}
