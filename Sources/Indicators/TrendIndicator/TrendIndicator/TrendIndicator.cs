using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    [Levels(0.0)]
    public class TrendIndicator : Indicator
    {
        [Parameter]
        public DataSeries Source { get; set; }

        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        [Output("Main", Color = Colors.Red)]
        public IndicatorDataSeries Result { get; set; }

        [Output("Lag", Color = Colors.Turquoise)]
        public IndicatorDataSeries Lag { get; set; }

        public override void Calculate(int index)
        {
            if (index < 3)
                Result[index] = Source[index];
            else
            {
                if (index < 7)
                    Result[index] = (Source[index] - 2*Source[index - 1] + Source[index - 2])/4;
                else
                    Result[index] = (Alpha - Alpha*Alpha/4)*Source[index]
                                    + Alpha*Alpha*Source[index - 1]/2
                                    - (Alpha - 0.75*Alpha*Alpha)*Source[index - 2]
                                    + 2*(1 - Alpha)*Result[index - 1]
                                    - (1 - Alpha)*(1 - Alpha)*Result[index - 2];

                Lag[index] = 2*Result[index] - Result[index - 2];
            }
        }
    }
}