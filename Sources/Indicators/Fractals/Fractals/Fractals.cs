using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class Fractals : Indicator
    {
        [Parameter("Show prices", DefaultValue = true)]
        public bool ShowPrices { get; set; }

        [Parameter("Days backward", DefaultValue = 10)]
        public int DaysBackward { get; set; }
        
        private MarketSeries m;
        
        private double lastHi;
        private double lastLo;

        protected override void Initialize()
        {
            m = MarketData.GetSeries(TimeFrame.Daily);
            lastHi = double.MinValue;
            lastLo = double.MaxValue;
            OnTimer();
            Timer.Start(10);
        }

        private int GetIndexByPrice(DataSeries series, double price)
        {
            for (int i = 1; MarketSeries.OpenTime[series.Count - i] >= m.OpenTime[DaysBackward]; ++i)
            {
                if (series[series.Count - i] == price)
                {
                    return series.Count - i;
                }
            }
            return -1;
        }

        public override void Calculate(int index)
        {
        }

        private void DrawLine(DataSeries series, double price, int d, VerticalAlignment a)
        {
            int indexLast = GetIndexByPrice(series, price);
            if (indexLast != -1)
            {
                string l_name = string.Format("p_{0}", price);
                string t_name = string.Format("t_{0}", price);
                ChartObjects.DrawLine(l_name, MarketSeries.OpenTime[indexLast], price, MarketSeries.OpenTime.LastValue.AddYears(2), price, Colors.DarkGray, 1, LineStyle.LinesDots);
                if (ShowPrices)
                {
                    ChartObjects.DrawText(t_name, string.Format("{0}", price), indexLast, price + d * Symbol.PipSize, a, HorizontalAlignment.Right, Colors.DarkGray);
                }
            }
        }

        protected override void OnTimer()
        {
            if (lastHi > m.High.Maximum(1) && lastLo <  m.Low.Minimum(1))
            {
                return;
            }
            
            ChartObjects.RemoveAllObjects();
            
            double prevHi = double.MinValue, prevLo = double.MaxValue;
            
            for (int i = 1; i <= DaysBackward; ++i)
            {
                double hi = m.High.Maximum(i);
                double lo = m.Low.Minimum(i);

                if (prevHi < hi)
                {
                    DrawLine(MarketSeries.High, hi, 2, VerticalAlignment.Top);
                    prevHi = hi;
                }

                if (prevLo > lo)
                {
                    DrawLine(MarketSeries.Low, lo, -2, VerticalAlignment.Bottom);
                    prevLo = lo;
                }
            }
            
            lastHi = m.High.Maximum(1);
            lastLo = m.Low.Minimum(1);
        }
    }
}
