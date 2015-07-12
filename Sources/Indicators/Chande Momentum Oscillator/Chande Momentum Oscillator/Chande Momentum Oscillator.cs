using System;
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, AccessRights = AccessRights.None)]
    public class CMO : Indicator
    {
        [Parameter()]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 14)]
        public int period { get; set; }

        [Output("CMO", Color = Colors.Yellow)]
        public IndicatorDataSeries cmo { get; set; }

        double downs;
        double ups;

        private IndicatorDataSeries ddown;
        private IndicatorDataSeries dup;

        protected override void Initialize()
        {
            ddown = CreateDataSeries();
            dup = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            if (index == 0)
            {
                ddown[index] = 0;
                dup[index] = 0;
                return;
            }

            ddown[index] = Math.Max(Source[index - 1] - Source[index], 0);
            dup[index] = Math.Max(Source[index] - Source[index - 1], 0);

            downs = 0;
            ups = 0;
            for (int i = index; i > index - period; i--)
            {
                downs += ddown[i];
                ups += dup[i];
            }
            if (Math.Abs(ups + downs) < double.Epsilon)
            {
                cmo[index] = 0;
            }
            else
            {
                cmo[index] = (100 * ((ups - downs) / (ups + downs)));
            }
        }
    }
}
