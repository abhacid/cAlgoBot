// -------------------------------------------------------------------------------
//
//    Indicator that calculates the difference between two stochastic oscillators
//    for different currency pairs
//
// -------------------------------------------------------------------------------

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class StochasticsDifference : Indicator
    {
        [Parameter("Second Pair")]
        public string SecondPair { get; set; }

        [Parameter("Stochastic K Periods", DefaultValue = 9)]
        public int kPeriods { get; set; }

        [Parameter("Stochastic K Slowing", DefaultValue = 3)]
        public int kSlowing { get; set; }

        [Parameter("Stochastic D Periods", DefaultValue = 9)]
        public int dPeriods { get; set; }

        [Output("Stochastic 1", Color = Colors.Azure)]
        public IndicatorDataSeries FirstResult { get; set; }

        [Output("Stochastic 2", Color = Colors.Green)]
        public IndicatorDataSeries SecondResult { get; set; }

        [Output("Stochastic Difference", Color = Colors.DarkRed)]
        public IndicatorDataSeries StochDifference { get; set; }

        // Second symbol information
        private Symbol SecondSymbol;
        private MarketSeries SecondSeries;

        // Stochastics for both
        private StochasticOscillator FirstStoch;
        private StochasticOscillator SecondStoch;

        protected override void Initialize()
        {
            // Fetch market series for second symbol
            this.SecondSymbol = MarketData.GetSymbol(SecondPair);
            this.SecondSeries = MarketData.GetSeries(this.SecondSymbol, this.TimeFrame);


            // Now create a stochastic for both
            this.FirstStoch = Indicators.StochasticOscillator(this.MarketSeries, this.kPeriods, this.kSlowing, this.dPeriods, MovingAverageType.Simple);
            this.SecondStoch = Indicators.StochasticOscillator(this.SecondSeries, this.kPeriods, this.kSlowing, this.dPeriods, MovingAverageType.Simple);
        }

        public override void Calculate(int index)
        {
            FirstResult[index] = this.FirstStoch.PercentK[index];
            SecondResult[index] = this.SecondStoch.PercentK[index];
            StochDifference[index] = Math.Abs(FirstResult[index] - SecondResult[index]);
        }
    }
}
