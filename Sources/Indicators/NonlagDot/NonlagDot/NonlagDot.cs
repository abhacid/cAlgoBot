using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator("NonLagDot", IsOverlay = true, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class NonLagDot : Indicator
    {
        #region Input

        [Parameter]
        public DataSeries Price { get; set; }

        [Parameter("Length", DefaultValue = 60)]
        public int Length { get; set; }

        [Parameter("Displace", DefaultValue = 0)]
        public int Displace { get; set; }

        [Parameter("Filter", DefaultValue = 0)]
        public int Filter { get; set; }

        [Parameter("Color", DefaultValue = 1)]
        public int ColorFront { get; set; }

        [Parameter("ColorBarBack", DefaultValue = 2)]
        public int ColorBarBack { get; set; }

        [Parameter("Deviation", DefaultValue = 0)]
        public double Deviation { get; set; }

        #endregion

        #region indicator line
        [Output("NLD", Color = Colors.Yellow, PlotType = PlotType.Points)]
        public IndicatorDataSeries MABuffer { get; set; }

        [Output("Up", Color = Colors.RoyalBlue, PlotType = PlotType.Points)]
        public IndicatorDataSeries UpBuffer { get; set; }

        [Output("Dn", Color = Colors.Red, PlotType = PlotType.Points)]
        public IndicatorDataSeries DnBuffer { get; set; }

        #endregion

        private IndicatorDataSeries price;
        private IndicatorDataSeries trend;
        private const double Cycle = 4;

        /// <summary>
        /// Indicator initialization function
        /// </summary>
        protected override void Initialize()
        {
            price = CreateDataSeries();
            trend = CreateDataSeries();

        }

        /// <summary>
        /// NonLagMA_v4   main logic
        /// </summary>
        /// <param name="index"></param>
        public override void Calculate(int index)
        {
            if (index < Length*Cycle + Length)
            {
                MABuffer[index] = 0;
                UpBuffer[index] = 0;
                DnBuffer[index] = 0;
                return;
            }

            const double pi = 3.1415926535;
            const double Coeff = 3*pi;
            int Phase = Length - 1;
            double Len = Length*Cycle + Phase;
            double Weight = 0;
            double Sum = 0;
            double t = 0;

            for (int i = 0; i <= Len - 1; i++)
            {
                double g = 1.0/(Coeff*t + 1);
                if (t <= 0.5) 
                    g = 1;
                
                double beta = Math.Cos(pi*t);
                double alfa = g*beta;
                price[i] = Price[index - i];
                Sum += alfa*price[i];
                Weight += alfa;
                
                if (t < 1) 
                    t += 1.0/(Phase - 1);
                else if (t < Len - 1) 
                    t += (2*Cycle - 1)/(Cycle*Length - 1);
            }

            if (Weight > 0) 
                MABuffer[index] = (1.0 + Deviation/100)*Sum/Weight;
           
            double filterFactor = Filter*Symbol.PointSize;
            
            if (Filter > 0)
            {
                if (Math.Abs(MABuffer[index] - MABuffer[index - 1]) < filterFactor)
                    MABuffer[index] = MABuffer[index - 1];
            }

            if (ColorFront <= 0) return;
            trend[index] = trend[index - 1];

            if (MABuffer[index] - MABuffer[index - 1] > filterFactor) 
                trend[index] = 1;
            if (MABuffer[index - 1] - MABuffer[index] > filterFactor) 
                trend[index] = -1;

            DnBuffer[index] = double.NaN;
            UpBuffer[index] = double.NaN;

            if (trend[index] > 0)
            {
                UpBuffer[index] = MABuffer[index];
                if (trend[index - ColorBarBack] < 0)
                    UpBuffer[index - ColorBarBack] = MABuffer[index - ColorBarBack];
            }
            else if (trend[index] < 0)
            {
                DnBuffer[index] = MABuffer[index];
                if (trend[index - ColorBarBack] > 0)
                    DnBuffer[index - ColorBarBack] = MABuffer[index - ColorBarBack];
            }
        }
    }
}

