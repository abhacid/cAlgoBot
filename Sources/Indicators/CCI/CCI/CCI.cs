using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class CCI : Indicator
    {
        [Output("CCI", Color = Colors.Lime)]
        public IndicatorDataSeries CCIa { get; set; }

        [Output("CCIline", Color = Colors.Lime, IsHistogram = true)]
        public IndicatorDataSeries CCIbline { get; set; }

        [Output("Level 2", Color = Colors.Red)]
        public IndicatorDataSeries level2 { get; set; }

        [Output("Level 1", Color = Colors.DarkRed)]
        public IndicatorDataSeries level1 { get; set; }

        [Output("Level -2", Color = Colors.Blue)]
        public IndicatorDataSeries mlevel2 { get; set; }

        [Output("Level -1", Color = Colors.DarkBlue)]
        public IndicatorDataSeries mlevel1 { get; set; }

        [Output("ZeroLine", Color = Colors.Gray)]
        public IndicatorDataSeries zero { get; set; }

        [Parameter(DefaultValue = 14)]
        public int period { get; set; }

        private CommodityChannelIndex cci;
        private CommodityChannelIndex cciline;

        protected override void Initialize()
        {
            cci = Indicators.CommodityChannelIndex(period);
            cciline = Indicators.CommodityChannelIndex(period);
        }

        public override void Calculate(int index)
        {
            CCIa[index] = cci.Result[index];
            CCIbline[index] = cci.Result[index];
            level2[index] = 200;
            level1[index] = 100;
            zero[index] = 0;
            mlevel1[index] = -100;
            mlevel2[index] = -200;
        }
    }
}
