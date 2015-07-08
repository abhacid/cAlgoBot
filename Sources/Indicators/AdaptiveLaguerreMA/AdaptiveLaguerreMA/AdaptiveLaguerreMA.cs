using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class AdaptiveLaguerreMA:Indicator
    {

        private double[,] _priceMatrix = new double[5,500];
        private double[] _price = new double[1];
        

        private TriangularMovingAverage _triangularMovingAverage;

        private IndicatorDataSeries _lastLow;
        private IndicatorDataSeries _lastHi;
        private IndicatorDataSeries _gamma;
        private IndicatorDataSeries _adaptiveGamma;
        private IndicatorDataSeries _diff;
        private IndicatorDataSeries _genTriangMovingAverage;

        [Parameter]
        public DataSeries Source { get; set; }
        
        [Parameter("Length", DefaultValue = 20, MaxValue = 100)]
        public int Length { get; set; }
        
        [Parameter("Period", DefaultValue = 4, MaxValue = 10)]
        public int Period { get; set; }

        // 1 - kaufman method, 2-Ehlers method
        [Parameter("Mode", DefaultValue = 2, MinValue = 1, MaxValue = 2)]
        public int Mode { get; set; }

        [Parameter("Smooth Period", DefaultValue = 5, MaxValue = 100)]
        public int Smooth { get; set; }
        
        [Parameter("Smooth Mode", DefaultValue = 5, MaxValue = 19)]
        public int SmoothMode { get; set; }

        [Output("Laguerre", Color = Colors.Red)]
        public IndicatorDataSeries Laguerre { get; set; }


        protected override void Initialize()
        {
            _lastLow = CreateDataSeries();
            _lastHi = CreateDataSeries();
            _gamma = CreateDataSeries();
            _adaptiveGamma = CreateDataSeries();
            _diff = CreateDataSeries();
            _genTriangMovingAverage = CreateDataSeries();
            
            _triangularMovingAverage = Indicators.TriangularMovingAverage(_adaptiveGamma, Smooth);
            
            Array.Resize(ref _price, Period+2);
        }

        public override void Calculate(int index)
        {
            if(index < Length)
            {
                _lastLow[index] = 0;
                _lastHi[index] = 0;
                _adaptiveGamma[index] = 0;
                _diff[index] = 0;
                _gamma[index] = 0;
                _genTriangMovingAverage[index] = 0;
                
                return;
            }           			
						
			//Compute Difference between price and Generalized Triangular moving average
			_diff[index] = Math.Abs(Source[index] - _genTriangMovingAverage[index - 1]);

            SetPriceArray(index);
            
            _genTriangMovingAverage[index] = GetGeneralTMA(ref _price, Period + 2);
            
            Laguerre[index] = _genTriangMovingAverage[index];


        }

        /// <summary>
        /// Fill price array
        /// </summary>
        /// <param name="index"></param>
        private void SetPriceArray(int index)
        {
            if (Mode == 1)
                _adaptiveGamma[index] = GetAdaptiveGamma(Mode, (IndicatorDataSeries)Source, Length, index);
            else
                _adaptiveGamma[index] = GetAdaptiveGamma(Mode, _diff, Length, index);

            if (SmoothMode == 5)
                _gamma[index] = _triangularMovingAverage.Result[index];

            for (int i = 0; i < Period + 2; i++)
            {
                if (index == Length)
                {
                    _priceMatrix[1, i] = Source[index];
                    _price[i] = _priceMatrix[1, i];
                }
                else
                {
                    if (i == 0)
                    {
                        _priceMatrix[0, i] = (1 - _gamma[index]) * Source[index] + _gamma[index] * _priceMatrix[1, i];
                        _priceMatrix[1, i] = _priceMatrix[0, i];
                    }
                    else
                    {
                        _priceMatrix[1, i] = _priceMatrix[0, i];
                        _priceMatrix[0, i] = -(_gamma[index]) * _priceMatrix[0, i - 1] + _priceMatrix[1, i - 1] + _gamma[index] * _priceMatrix[1, i];
                    }

                    _price[i] = _priceMatrix[0, i];

                }

            }		
        }

        /// <summary>
        /// Adaptive Gamma Formula
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="source"></param>
        /// <param name="period"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private double GetAdaptiveGamma(int mode, IndicatorDataSeries source, int period, int index)
		{
			double sum = 0;
            double eff;
            double max = 0;
            double min = source[0];

            for(int i = 0 ; i < period ; i++) 
			{
				if(mode == 1)
                    sum += Math.Abs(source[index - i] - source[index - i - 1]);			    
                else if (mode == 2)
			    {
                    if (source[index - i] > max)
                        max = source[index - i];
			        if (source[index - i] < min)
                        min = source[index - i];
			    }
			}
									
			if(mode == 1 && sum > 0)
                eff = Math.Abs(source[index] - source[index - period]) / sum;
			else 
				if(mode == 2 && max - min > 0)
                    eff = (source[index] - min) / (max - min);
			else
				eff = 0;

			return 1 - eff;  
		}		
				
   		/// <summary>
        ///  Generalized Triangular Moving Average 
		/// </summary>
		/// <param name="price"></param>
		/// <param name="period"></param>
		/// <returns></returns>
        private double GetGeneralTMA(ref double[] price, int period)
		{
   		    double sumTotal = 0;   		     
            var periodFloor = Math.Floor( (period + 1 ) * 0.5);
			var periodCeiling = Math.Ceiling( (period + 1) * 0.5);
								
			for(int i = 1 ; i <= periodCeiling  ; i++) 
			{
				double sum = 0;
				for(int j = 1 ; j <= periodFloor ; j++)
				{                    
                    sum = sum + (double) price.GetValue(j);
				}
				               
                sumTotal += (sum / periodFloor);
							
			}

            return sumTotal / periodCeiling;
		}		

    }
}
