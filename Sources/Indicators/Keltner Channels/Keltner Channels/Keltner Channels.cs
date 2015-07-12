using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class KeltnerChannels : Indicator
    {
        private int Period = 10;
        private IndicatorDataSeries atr;
        private MovingAverage expo;
        private TrueRange tri;


        [Parameter(DefaultValue = 20)]
        public int KeltnerPeriod { get; set; }

        [Parameter(DefaultValue = 2.0)]
        public double BandDistance { get; set; }

        [Output("Main")]
        public IndicatorDataSeries KeltnerMain { get; set; }

        [Output("ChannelUp", Color = Colors.Red)]
        public IndicatorDataSeries ChannelUp { get; set; }

        [Output("ChannelLow", Color = Colors.Blue)]
        public IndicatorDataSeries ChannelLow { get; set; }

        [Parameter("MAType")]
        public MovingAverageType matype { get; set; }

        protected override void Initialize()
        {
            atr = CreateDataSeries();
            tri = Indicators.TrueRange();
            expo = Indicators.MovingAverage(MarketSeries.Close, KeltnerPeriod, matype);
        }

        public override void Calculate(int index)
        {
            if (index < Period + 1)
            {
                atr[index] = tri.Result[index];
            }
            if (index >= Period)
            {
                atr[index] = (atr[index - 1] * (Period - 1) + tri.Result[index]) / Period;
            }

            KeltnerMain[index] = expo.Result[index];
            ChannelUp[index] = expo.Result[index] + BandDistance * atr[index];
            ChannelLow[index] = expo.Result[index] - BandDistance * atr[index];
        }
    }
}
