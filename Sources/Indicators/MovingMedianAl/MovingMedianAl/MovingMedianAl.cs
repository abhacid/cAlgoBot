using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MovingMedianAl : Indicator
    {
        private double[] _array;



        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 20, MinValue = 5)]
        public int Period { get; set; }

        [Output("Main", Color = Colors.DarkBlue)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _array = new double[Period];
        }

        public override void Calculate(int index)
        {
            if (index < Period)
            {
                for (int i = 0; i < Period; i++ )
                {
                    if (i <= index)
                        _array[i] = Source[index - i];
                    else
                        _array[i] = 0.0;
                }
                    
                Array.Sort(_array);

                if ((index + 1) % 2 == 0)
                    Result[index] = 0.5*(_array[Period - (index + 1)/2] 
                        + _array[Period - (index + 1)/2 - 1]);
                else
                    Result[index] = _array[Period - (index + 2) / 2];

                return;
            }

            for (int i = 0; i < Period; i++)
                _array[i] = Source[index - i];
            
            Array.Sort(_array);

            if (Period%2 == 0)
                Result[index] = 0.5*(_array[Period/2] + _array[Period/2 - 1]);
            else
                Result[index] = _array[(Period-1) / 2];


        }
    }
}