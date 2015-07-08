using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class LaguerreWma : Indicator
    {
        #region Variables


        private WeightedMovingAverage _wma;
        private ExponentialMovingAverage _ema;
        private double[] ol, pol, hl, phl, ll, pll, cl, pcl;
        private IndicatorDataSeries unsmoothed;
        private int lastSeenBar = -1;
        private double[] unsmBuffer;

        #endregion

        #region parameters

        [Parameter(DefaultValue = 0.3)]
        public double Gamma { get; set; }

        [Parameter(DefaultValue = 8)]
        public int SmoothingPeriod { get; set; }

        [Parameter(DefaultValue = 5)]
        public int TriggerPeriod { get; set; }

        #endregion

        #region output

        [Output("Zero", Color = Colors.Gray)]
        public IndicatorDataSeries ZeroLine { get; set; }

        [Output("ECO", Color = Colors.Red)]
        public IndicatorDataSeries ECO { get; set; }

        [Output("Histogram", PlotType = PlotType.Histogram, Color = Colors.Blue)]
        public IndicatorDataSeries Histogram { get; set; }

        [Output("Trigger", Color = Colors.Gold)]
        public IndicatorDataSeries Trigger { get; set; }

        #endregion

        protected override void Initialize()
        {
            unsmBuffer = new double[1];
            unsmoothed = CreateDataSeries();
            _wma = Indicators.WeightedMovingAverage(unsmoothed, SmoothingPeriod);
            _ema = Indicators.ExponentialMovingAverage(_wma.Result, TriggerPeriod);

            ol = new double[4];
            pol = new double[4];
            hl = new double[4];
            phl = new double[4];
            ll = new double[4];
            pll = new double[4];
            cl = new double[4];
            pcl = new double[4];
        }

        public override void Calculate(int index)
        {
            if (index == 1)
            {
                for (int i = 0; i < 4; ++i)
                {
                    pol[i] = MarketSeries.Open[index];
                    phl[i] = MarketSeries.High[index];
                    pll[i] = MarketSeries.Low[index];
                    pcl[i] = MarketSeries.Close[index];

                    lastSeenBar = index;
                }
            }

            if (index != lastSeenBar)
            {
                for (int i = 0; i < 4; ++i)
                {
                    pol[i] = ol[i]; // remember previous bar value
                    phl[i] = hl[i];
                    pll[i] = ll[i];
                    pcl[i] = cl[i];
                }

                lastSeenBar = index;
            }

            // update all the Laguerre numbers
            ol[0] = (1 - Gamma) * MarketSeries.Open[index] + Gamma * pol[0];
            hl[0] = (1 - Gamma) * MarketSeries.High[index] + Gamma * phl[0];
            ll[0] = (1 - Gamma) * MarketSeries.Low[index] + Gamma * pll[0];
            cl[0] = (1 - Gamma) * MarketSeries.Close[index] + Gamma * pcl[0];
            
            for (int i = 1; i < 4; ++i)
            {
                ol[i] = -Gamma * ol[i - 1] + pol[i - 1] + Gamma * pol[i];
                hl[i] = -Gamma * hl[i - 1] + phl[i - 1] + Gamma * phl[i];
                ll[i] = -Gamma * ll[i - 1] + pll[i - 1] + Gamma * pll[i];
                cl[i] = -Gamma * cl[i - 1] + pcl[i - 1] + Gamma * pcl[i];
            }

            double value1 = 0.0;
            double value2 = 0.0;

            for (int i = 0; i < 4; ++i)
            {
                value1 += (cl[i] - ol[i]);
                value2 += (hl[i] - ll[i]);
            }


            if (Math.Abs(value2 - 0.0) > double.Epsilon)
            {
                unsmoothed[index] = (100.0 * value1 / value2);                
            }
            else
            {               
                if ((index > 1) && Array.IndexOf(unsmBuffer, 0.0) > 0 )
                {
                    unsmoothed[index] = unsmoothed[index -1];
                }
                else
                {
                    unsmoothed[index] =0;
                }
            }

            Array.Resize(ref unsmBuffer, unsmBuffer.Length+1);
            unsmBuffer[index] = unsmoothed[index];

            ECO[index] = _wma.Result[index];
            Trigger[index] = _ema.Result[index];
            Histogram[index] = _wma.Result[index] - _ema.Result[index];
        }
    }
}
