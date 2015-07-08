using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class CCIHistoColor : Indicator
    {
        private CommodityChannelIndex cci;

        private double g_icci_140;
        private double g_icci_148;
        private double gd_156;

        [Parameter("CCI Period", DefaultValue = 80)]
        public int CciPeriod { get; set; }

        [Parameter("Momentum", DefaultValue = 200)]
        public int Momentum { get; set; }

        [Output("Gray", Color = Colors.Gray, IsHistogram = true, Thickness = 4)]
        public IndicatorDataSeries g_ibuf_164 { get; set; }

        [Output("Blue", Color = Colors.Blue, IsHistogram = true, Thickness = 4)]
        public IndicatorDataSeries g_ibuf_168 { get; set; }

        [Output("Orange", Color = Colors.Orange, IsHistogram = true, Thickness = 4)]
        public IndicatorDataSeries g_ibuf_172 { get; set; }

        [Output("Red", Color = Colors.Red, IsHistogram = true, Thickness = 4)]
        public IndicatorDataSeries g_ibuf_176 { get; set; }

        protected override void Initialize()
        {
            cci = Indicators.CommodityChannelIndex(CciPeriod);
        }

        public override void Calculate(int index)
        {
            g_icci_140 = cci.Result[index];
            g_icci_148 = cci.Result[index - 1];

            g_ibuf_164[index] = 0;
            g_ibuf_168[index] = 0;
            g_ibuf_172[index] = 0;
            g_ibuf_176[index] = 0;

            if ((g_icci_148 >= 0.0 && g_icci_140 < 0.0) || (g_icci_148 <= 0.0 && g_icci_140 > 0.0)) gd_156 = 0;
            if (Math.Abs(g_icci_148) > Math.Abs(gd_156)) gd_156 = g_icci_148;

            if (g_icci_140 >= Momentum)
            {
                if (g_icci_140 > g_icci_148 && g_icci_140 >= gd_156) g_ibuf_176[index] = g_icci_140;
                if (g_icci_140 > g_icci_148 && g_icci_140 < gd_156) g_ibuf_168[index] = g_icci_140;
                if (g_icci_140 < g_icci_148) g_ibuf_172[index] = g_icci_140;
            }

            if (g_icci_140 <= -Momentum)
            {
                if (g_icci_140 < g_icci_148 && g_icci_140 <= gd_156) g_ibuf_176[index] = g_icci_140;
                if (g_icci_140 < g_icci_148 && g_icci_140 > gd_156) g_ibuf_168[index] = g_icci_140;
                if (g_icci_140 > g_icci_148) g_ibuf_172[index] = g_icci_140;
            }

            if (g_icci_140 > (-Momentum) && g_icci_140 < Momentum) g_ibuf_164[index] = g_icci_140;
        }
    }
}