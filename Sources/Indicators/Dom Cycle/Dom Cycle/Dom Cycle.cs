
using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class DomCycle : Indicator
    {
        readonly double pi = 4 * Math.Atan(1);
        readonly double a = (1 - Math.Sin(2 * 4 * Math.Atan(1) / 30)) / Math.Cos(2 * 4 * Math.Atan(1) / 30);

        private double MaxAmpl;
        private DateTime pTime;

        double[] OlderI = new double[60];
        double[] OldI = new double[60];
        double[] I = new double[60];
        double[] OlderQ = new double[60];
        double[] OldQ = new double[60];
        double[] Q = new double[60];
        double[] OlderReal = new double[60];
        double[] OldReal = new double[60];
        double[] Real = new double[60];
        double[] OlderImag = new double[60];
        double[] OldImag = new double[60];
        double[] Imag = new double[60];
        double[] DB = new double[60];
        double[] OldDB = new double[60];
        double[] Ampl = new double[60];

        private IndicatorDataSeries price;
        private IndicatorDataSeries mDomCyc;
        private IndicatorDataSeries DC;
        private IndicatorDataSeries smoothHp;
        private IndicatorDataSeries hp;

        [Output("Dom Cycle")]
        public IndicatorDataSeries DomCyc { get; set; }

        protected override void Initialize()
        {
            price = CreateDataSeries();
            mDomCyc = CreateDataSeries();
            DC = CreateDataSeries();
            smoothHp = CreateDataSeries();
            hp = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            double delta = 0;

            price[index] = MarketSeries.Close[index];

            if (index == 0)
                hp[index] = 0;

            if (index > 0)
            {
                hp[index] = 0.5 * (1 + a) * (price[index] - price[index - 1]) + a * hp[index - 1];
                delta = Math.Max(0.1, -0.015 * (index - 1) + 0.5);

            }
            if (index > 4)
                smoothHp[index] = (hp[index] + 2.0 * hp[index - 1] + 3.0 * hp[index - 2] + 3.0 * hp[index - 3] + 2.0 * hp[index - 4] + hp[index - 5]) / 12.0;

            if (index < 6)
                smoothHp[index] = price[index] - price[index - 1];

            if (index > 11)
            {
                if (MarketSeries.OpenTime[index - 1] != pTime)
                {
                    for (int n = 11; n <= 59; n++)
                    {
                        OlderI[n] = OldI[n];
                        OldI[n] = I[n];
                        OlderQ[n] = OldQ[n];
                        OldQ[n] = Q[n];
                        OlderReal[n] = OldReal[n];
                        OldReal[n] = Real[n];
                        OlderImag[n] = OldImag[n];
                        OldImag[n] = Imag[n];
                        OldDB[n] = DB[n];
                    }

                    pTime = MarketSeries.OpenTime[index - 1];
                }

                for (int n = 11; n <= 59; n++)
                {
                    double beta = Math.Cos(4 * pi / (n + 1));
                    double gamma = 1.0 / Math.Cos(8 * pi * delta / (n + 1));
                    double alpha = gamma - Math.Sqrt(gamma * gamma - 1);
                    Q[n] = ((n + 1) / 4 / pi) * (smoothHp[index] - smoothHp[index - 1]);
                    I[n] = smoothHp[index];
                    Real[n] = 0.5 * (1 - alpha) * (I[n] - OlderI[n]) + beta * (1 + alpha) * OldReal[n] - alpha * OlderReal[n];
                    Imag[n] = 0.5 * (1 - alpha) * (Q[n] - OlderQ[n]) + beta * (1 + alpha) * OldImag[n] - alpha * OlderImag[n];
                    Ampl[n] = (Real[n] * Real[n] + Imag[n] * Imag[n]);
                }

                MaxAmpl = Ampl[11];

                for (int n = 11; n <= 59; n++)
                    if (Ampl[n] > MaxAmpl)
                    {
                        MaxAmpl = Ampl[n];
                    }

                for (int n = 11; n <= 59; n++)
                {
                    double dB = 0;
                    if (MaxAmpl != 0)
                        if (Ampl[n] / MaxAmpl > 0)
                            dB = -10.0 * Math.Log(0.01 / (1 - 0.99 * Ampl[n] / MaxAmpl)) / Math.Log(10.0);

                    DB[n] = 0.33 * dB + 0.67 * OldDB[n];

                    if (DB[n] > 20)
                        DB[n] = 20;
                }

                double Num = 0;
                double Denom = 0;

                for (int n = 11; n <= 59; n++)
                {
                    if (DB[n] <= 6)
                    {
                        Num = Num + (n + 1) * (20 - DB[n]);
                        Denom = Denom + (20 - DB[n]);
                    }
                    if (Denom != 0)
                        DC[index] = 0.5 * Num / Denom;
                    else
                        DC[index] = DC[index - 1];
                    Print("{0}", DC[index]);
                }

                mDomCyc[index] = Median(5, index);
            }

            DomCyc[index - 1] = mDomCyc[index];
        }

        private double Median(int per, int bar)
        {
            double[] array = new double[per];

            for (int i = 0; i < per; i++)
                array[i] = DC[bar - i];
            Array.Sort(array);

            int num = (int)Math.Round((double)((per - 1) / 2));

            double median;
            if ((per % 2) > 0)
                median = array[num];
            else
                median = 0.5 * (array[num] + array[num + 1]);

            return (median);
        }


    }
}
