//#reference: ..\Indicators\Sinewave.algo

using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class SinewaveSupportResistance : Indicator
    {
        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        [Output("Resistance", Color = Colors.Blue, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries Resistance { get; set; }

        [Output("Support", Color = Colors.Magenta, PlotType = PlotType.Points, Thickness = 3)]
        public IndicatorDataSeries Support { get; set; }

        Sinewave sine;

        int _flag;
        double _value1;

        protected override void Initialize()
        {
            sine = Indicators.GetIndicator<Sinewave>(Alpha);
        }

        public override void Calculate(int index)
        {
            sine.Calculate(index);
            if (Functions.HasCrossedAbove(sine.LeadSine, sine.Sine, 0))
            {
                _flag = 1;
                _value1 = MarketSeries.Low[index - 1];
            }
            if (Functions.HasCrossedBelow(sine.LeadSine, sine.Sine, 0))
            {
                _flag = -1;
                _value1 = MarketSeries.High[index - 1];
            }
            if (_flag == 1)
            {
                Support[index] = _value1;
            }
            if (_flag == -1)
            {
                Resistance[index] = _value1;
            }
        }
    }
}
