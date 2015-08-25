using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class VolumeOscilatorHistogram : Indicator
    {
        private VolumeOscillator _volumeOscillator;
        private double currentVolume;
        private double preVolume;
        private double pre2Volume;

        [Parameter("Short Term", DefaultValue = 5)]
        public int ShortTerm { get; set; }
        [Parameter("Long Term", DefaultValue = 30)]
        public int LongTerm { get; set; }
        [Output("Volume Oscilator", Color = Colors.DodgerBlue, Thickness = 2)]
        public IndicatorDataSeries Result { get; set; }
        [Output("Plus", Color = Colors.DodgerBlue, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries Plus { get; set; }
        [Output("Minus", Color = Colors.Gray, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries Minus { get; set; }
        protected override void Initialize()
        {
            _volumeOscillator = Indicators.VolumeOscillator(ShortTerm, LongTerm);
        }
        public override void Calculate(int index)
        {
            // Display Result of Indicator
            Result[index] = _volumeOscillator.Result[index];
            currentVolume = _volumeOscillator.Result[index];
            preVolume = _volumeOscillator.Result[index - 1];
            pre2Volume = _volumeOscillator.Result[index - 2];

            if (currentVolume >= preVolume)
            {
                Plus[index] = currentVolume;
            }
            else
            {
                Minus[index] = currentVolume;
            }

            // Normalization of histogram painting
            if (preVolume >= pre2Volume)
            {
                Plus[index - 1] = preVolume;
            }
            else
            {
                Minus[index - 1] = preVolume;
            }

        }

    }
}
