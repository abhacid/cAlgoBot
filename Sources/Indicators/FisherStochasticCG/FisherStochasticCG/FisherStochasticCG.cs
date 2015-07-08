using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    [Levels(0.0)]
    public class FisherStochasticCG : Indicator    
    {
        [Parameter(DefaultValue = 8)]
        public int Length { get; set; }

        [Output("CG", Color = Colors.Red)]
        public IndicatorDataSeries CG { get; set; }

        [Output("Trigger", Color = Colors.Blue)]
        public IndicatorDataSeries Trigger { get; set; }

        private IndicatorDataSeries _price;
        private IndicatorDataSeries _buffer1;
        private IndicatorDataSeries _buffer2;
        
        protected override void Initialize()
        {
            _price = CreateDataSeries();
            _buffer1 = CreateDataSeries();
            _buffer2 = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            _price[index] = (MarketSeries.High[index] + MarketSeries.Low[index])/2;

            if (index < Length - 1)
            {
                _buffer1[index] = 0;
                _buffer2[index] = 0;
                CG[index] = 0;
                return;
            }

            double numerator = 0;
            double denominator = 0;

            for (int i = 0; i < Length; i++)
            {
                numerator += (1 + i)*_price[index - i];
                denominator += _price[index - i];
            }

            double epsilon = Math.Pow(10, -10);
            if (Math.Abs(denominator) > epsilon)
                _buffer1[index] = -numerator/denominator + (Length + 1)/2;
            
            double maxCG = Functions.Maximum(_buffer1, Length);
            double minCG = Functions.Minimum(_buffer1, Length);
            
            if (Math.Abs(maxCG - minCG) > epsilon)
                _buffer2[index] = (_buffer1[index] - minCG)/(maxCG - minCG);

            double avgValue = (4*_buffer2[index] + 3*_buffer2[index - 1] + 2*_buffer2[index - 2] + _buffer2[index - 3])/10;
            
            CG[index] = 0.5*Math.Log((1 + 1.98*(avgValue - 0.5))/(1 - 1.98*(avgValue - 0.5)));
            Trigger[index] = CG[index - 1];
        }
    }
}
