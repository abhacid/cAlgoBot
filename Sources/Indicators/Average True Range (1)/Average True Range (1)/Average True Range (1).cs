using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class AverageTrueRange : Indicator
    {
        private TrueRange _tri;

        [Parameter(DefaultValue = 14, MinValue = 2)]
        public int Period { get; set; }

        [Output("ATR", Color = Colors.Orange)]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _tri = Indicators.TrueRange();
        }

        public override void Calculate(int index)
        {
            if (index < Period + 1)
            {
                Result[index] = _tri.Result[index];
            }
            if (index >= Period)
            {
                Result[index] = (Result[index - 1] * (Period - 1) + _tri.Result[index]) / Period;
            }
        }
    }
}
