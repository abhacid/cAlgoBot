using System;
using cAlgo.API;
using cAlgo.API.Internals;

// (C) 2014 marekfx

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None, AutoRescale = true)]
    public class Correlation : Indicator
    {
        private MarketSeries _symbol2Series;

        [Parameter(DefaultValue = "USDCHF")]
        public string Symbol2 { get; set; }

        [Parameter(DefaultValue = 50)]
        public int Lookback { get; set; }

        [Output("Correlation", PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _symbol2Series = MarketData.GetSeries(Symbol2, TimeFrame);
        }

        public override void Calculate(int index)
        {
            DateTime date = MarketSeries.OpenTime[index];

            //get index for Symbol 2 series
            var idx2 = _symbol2Series.OpenTime.GetIndexByExactTime(date);

            if (index < Lookback || idx2 < Lookback)
                return;

            double[] tab1 = new double[Lookback];
            double[] tab2 = new double[Lookback];

            //populate tab1 and tab2 arrays with close for Symbol 1 (MarketSeries) and Symbol 2
            for (int i = 0; i < Lookback; i++)
            {
                tab1[i] = MarketSeries.Close[index - Lookback + i];
                tab2[i] = _symbol2Series.Close[idx2 - Lookback + i];
            }

            Result[index] = Stat.Correlation(tab1, tab2);
        }
    }

    //Correlation from (c) http://mantascode.com/c-how-to-get-correlation-coefficient-of-two-arrays/
    public class Stat
    {
        public static double Correlation(double[] array1, double[] array2)
        {
            double[] array_xy = new double[array1.Length];
            double[] array_xp2 = new double[array1.Length];
            double[] array_yp2 = new double[array1.Length];
            for (int i = 0; i < array1.Length; i++)
                array_xy[i] = array1[i] * array2[i];
            for (int i = 0; i < array1.Length; i++)
                array_xp2[i] = Math.Pow(array1[i], 2.0);
            for (int i = 0; i < array1.Length; i++)
                array_yp2[i] = Math.Pow(array2[i], 2.0);
            double sum_x = 0;
            double sum_y = 0;
            foreach (double n in array1)
                sum_x += n;
            foreach (double n in array2)
                sum_y += n;
            double sum_xy = 0;
            foreach (double n in array_xy)
                sum_xy += n;
            double sum_xpow2 = 0;
            foreach (double n in array_xp2)
                sum_xpow2 += n;
            double sum_ypow2 = 0;
            foreach (double n in array_yp2)
                sum_ypow2 += n;
            double Ex2 = Math.Pow(sum_x, 2.0);
            double Ey2 = Math.Pow(sum_y, 2.0);

            return (array1.Length * sum_xy - sum_x * sum_y) / Math.Sqrt((array1.Length * sum_xpow2 - Ex2) * (array1.Length * sum_ypow2 - Ey2));
        }
    }
}
