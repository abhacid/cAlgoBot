using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class Envelopes : Indicator
    {
        private int Period = 10;
        private MovingAverage expo;


        [Parameter(DefaultValue = 20)]
        public int EnvelopePeriod { get; set; }

        [Parameter(DefaultValue = 0.5)]
        public double BandDistance { get; set; }

        [Output("Main")]
        public IndicatorDataSeries EnvelopeMain { get; set; }

        [Output("ChannelUp", Color = Colors.Red)]
        public IndicatorDataSeries ChannelUp { get; set; }

        [Output("ChannelLow", Color = Colors.Blue)]
        public IndicatorDataSeries ChannelLow { get; set; }

        [Parameter("MAType")]
        public MovingAverageType matype { get; set; }

        protected override void Initialize()
        {
            expo = Indicators.MovingAverage(MarketSeries.Close, EnvelopePeriod, matype);
        }

        public override void Calculate(int index)
        {
            EnvelopeMain[index] = expo.Result[index];
            ChannelUp[index] = expo.Result[index] + (expo.Result[index] * BandDistance) / 100;
            ChannelLow[index] = expo.Result[index] - (expo.Result[index] * BandDistance) / 100;
        }
    }
}
