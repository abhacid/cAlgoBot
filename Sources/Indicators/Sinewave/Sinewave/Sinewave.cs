//#reference:..\Indicators\CyclePeriod.algo

using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator()]
    public class Sinewave : Indicator
    {
        private IndicatorDataSeries _cycle;
        private CyclePeriod _cyclePeriod;
        private IndicatorDataSeries _period;
        private IndicatorDataSeries _price;
        private IndicatorDataSeries _smooth;

        [Parameter(DefaultValue = 0.07)]
        public double Alpha { get; set; }

        [Output("Sine", Color = Colors.Blue)]
        public IndicatorDataSeries Sine { get; set; }

        [Output("LeadSine", Color = Colors.Green)]
        public IndicatorDataSeries LeadSine { get; set; }


        readonly double _tempReal = Math.Atan(1.0);

        protected override void Initialize()
        {
            _period = CreateDataSeries();
            _cyclePeriod = Indicators.GetIndicator<CyclePeriod>(Alpha);
            _price = CreateDataSeries();
            _cycle = CreateDataSeries();
            _smooth = CreateDataSeries();
        }

        public override void Calculate(int index)
        {
            _price[index] = (MarketSeries.High[index] + MarketSeries.Low[index]) / 2;
            _smooth[index] = (_price[index] + 2 * _price[index - 1] + 2 * _price[index - 2] + _price[index - 3]) / 6;
            _period[index] = _cyclePeriod.Result[index];

            if (index < 7)
            {
                _cycle[index] = (_price[index] - 2 * _price[index - 1] + _price[index - 2]) / 4;
                return;
            }
            _cycle[index] = (1 - 0.5 * Alpha) * (1 - 0.5 * Alpha) * (_smooth[index] - 2 * _smooth[index - 1] + _smooth[index - 2]) + 2 * (1 - Alpha) * _cycle[index - 1] - (1 - Alpha) * (1 - Alpha) * (_cycle[index - 2]);

            var dcPeriod = (int)Math.Floor(_period[index]);
            double real = 0.0;
            double img = 0.0;
            double dcphase;

            double rad2Deg = 45.0 / _tempReal;
            double deg2Rad = 1.0 / rad2Deg;

            for (int i = 0; i < dcPeriod; i++)
            {
                real += Math.Sin(deg2Rad * 360 * i / dcPeriod) * _cycle[index - i];
                img += Math.Cos(deg2Rad * 360 * i / dcPeriod) * _cycle[index - i];
            }
            if (Math.Abs(img) > 0.001)
                dcphase = rad2Deg * Math.Atan(real / img);
            else if (real > 0.0)
                dcphase = 90;
            else
                dcphase = -90;

            dcphase += 90;
            if (img < 0)
                dcphase += 180;
            if (dcphase > 315)
                dcphase -= 360;

            Sine[index] = Math.Sin(dcphase * deg2Rad);
            LeadSine[index] = Math.Sin((dcphase + 45.0) * deg2Rad);
        }
    }
}
