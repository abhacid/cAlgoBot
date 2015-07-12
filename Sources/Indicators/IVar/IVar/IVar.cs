using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class IVar : Indicator
    {
        [Parameter("n", DefaultValue = 5)]
        public int n { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }


        protected override void Initialize()
        {
            // Initialize and create nested indicators
        }

        public override void Calculate(int index)
        {
            int ihigh, ilow, nInterval;
            double Delta, Xс, Yс, Sx, Sy, Sxx, Sxy;
            Sx = 0;
            Sy = 0;
            Sxx = 0;
            Sxy = 0;
            for (int i = 0; i < n; i++)
            {
                nInterval = (int)Math.Pow(2, n - i);
                Delta = 0;
                for (int k = 0; k < Math.Pow(2, i); k++)
                {
                    ihigh = iHighest(nInterval, index - nInterval * k);
                    ilow = iLowest(nInterval, index - nInterval * k);
                    Delta += MarketSeries.High[ihigh] - MarketSeries.Low[ilow];

                }
                Xс = (n - i) * Math.Log(2.0);
                Yс = Math.Log(Delta);
                Sx += Xс;
                Sy += Yс;
                Sxx += Xс * Xс;
                Sxy += Xс * Yс;
            }
            Result[index] = -(Sx * Sy - (n + 1) * Sxy) / (Sx * Sx - (n + 1) * Sxx);
        }

        private int iHighest(int count, int index)
        {
            int res = index;
            for (int i = 1; i < count; i++)
                if (MarketSeries.High[res] < MarketSeries.High[index - i])
                    res = index - i;
            return res;
        }

        private int iLowest(int count, int index)
        {
            int res = index;
            for (int i = 1; i < count; i++)
                if (MarketSeries.Low[res] > MarketSeries.Low[index - i])
                    res = index - i;
            return res;
        }
    }
}
