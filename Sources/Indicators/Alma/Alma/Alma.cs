using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Alma : Indicator
    {
        private double[] _aDoubles = new double[1];

        [Parameter("Size", DefaultValue = 9, MinValue = 2)]
        public int Size { get; set; }

        [Parameter("Sigma", DefaultValue = 6, MinValue = 1)]
        public double Sigma { get; set; }

        [Parameter("Offset", DefaultValue = 0.5)]
        public double Offset { get; set; }

        [Output("Alma", Color = Colors.Cyan)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            Array.Resize(ref _aDoubles, Size);
            ResetWindow();
        }
        public override void Calculate(int index)
        {
            if(index < Size)
                return;
            
            double sum = 0;
			double norm = 0;

            for (int i = 0; i < Size; i++)
            {
                sum += _aDoubles[i]*MarketSeries.Close[index - i];
                norm += _aDoubles[i];
            }

            // Normalize the result
			if (Math.Abs(norm - 0) > double.Epsilon)
                sum /= norm;
			
            // Draw Indicator
            Result[index] = sum;

        }

        private void ResetWindow()
        {
            var m = Math.Floor(Offset * (Size - 1));
            
            var s = Size/Sigma;

            for (int i = 0; i < Size; i++)
            {
                _aDoubles[i] = Math.Exp(-((i - m) * (i - m)) / (2 * s * s));
            }
        }
    
    }
}
