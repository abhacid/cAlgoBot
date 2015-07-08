using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class StandardDeviation : Indicator
    {
        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 14, MinValue = 2)]
        public int Periods { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MAType { get; set; }

        [Output("Result", Color = Colors.Orange)]
        public IndicatorDataSeries Result { get; set; }


        private MovingAverage movingAverage;

        protected override void Initialize()
        {
            movingAverage = Indicators.MovingAverage(Source, Periods, MAType);
        }

        public override void Calculate(int index)
        {
            var average = movingAverage.Result[index];

            double sum = 0;

            for (var period = 0; period < Periods; period++)
            {
                sum += Math.Pow(Source[index - period] - average, 2.0);
            }

            Result[index] = Math.Sqrt(sum / Periods);
        }
    }
}
