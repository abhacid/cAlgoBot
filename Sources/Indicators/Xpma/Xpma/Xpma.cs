//#reference: ..\Indicators\LWMA.algo
//#reference: ..\Indicators\TEMA.algo
//#reference: ..\Indicators\DEMA.algo
//#reference: ..\Indicators\WSMA.algo
//#reference: ..\Indicators\T3MA.algo
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class Xpma : Indicator
    {
        private Dema _dema;
        private ExponentialMovingAverage _ema;
        private SimpleMovingAverage _sma;
        private T3MA _t3MA;
        private Tema _tema;
        private Wsma _wsma;
        private Lwma _lwma;

        [Parameter(DefaultValue = 25)]
        public int MAPeriod { get; set; }

        //MODE_TEMA
        [Parameter(DefaultValue = 5)]
        public int MAType { get; set; }

        [Parameter()]
        public DataSeries MAApplied { get; set; }

        [Parameter(DefaultValue = 0.8)]
        public double T3MAVolumeFactor { get; set; }

        [Parameter(DefaultValue = 1)]
        public int StepPeriod { get; set; }

        [Output("XP Moving Average", Color = Colors.Yellow, PlotType = PlotType.Line, LineStyle = LineStyle.Dots, Thickness = 3)]
        public IndicatorDataSeries XPBuffer { get; set; }

        [Output("UpBuffer", Color = Colors.Maroon, PlotType = PlotType.Line, LineStyle = LineStyle.Dots, Thickness = 3)]
        public IndicatorDataSeries UpBuffer { get; set; }

        [Output("DownBuffer", Color = Colors.DarkGreen, PlotType = PlotType.Line, LineStyle = LineStyle.Dots, Thickness = 3)]
        public IndicatorDataSeries DownBuffer { get; set; }

        public IndicatorDataSeries Buffer { get; set; }


        protected override void Initialize()
        {
            Buffer = CreateDataSeries();

            // Moving Average Type according to input MA_Type
            switch (MAType)
            {
                case 1:
                    // Simple Moving Average
                    _sma = Indicators.SimpleMovingAverage(MAApplied, MAPeriod);
                    break;
                case 2:
                    // Exponential Moving Average
                    _ema = Indicators.ExponentialMovingAverage(MAApplied, MAPeriod);
                    break;
                case 3:
                    // Smoothed Moving Average		
                    _wsma = Indicators.GetIndicator<Wsma>(MAApplied, MAPeriod);
                    break;
                case 4:
                    // Linear Weighted Moving Average  
                    _lwma = Indicators.GetIndicator<Lwma>(MAApplied, MAPeriod);
                    break;
                case 5:
                    // Double Exponential Moving Average
                    _dema = Indicators.GetIndicator<Dema>(MAApplied, MAPeriod);
                    break;
                case 6:
                    // Triple Exponential Moving Average
                    _tema = Indicators.GetIndicator<Tema>(MAApplied, MAPeriod);
                    break;
                case 7:
                    // T3 Moving Average
                    _t3MA = Indicators.GetIndicator<T3MA>(MAApplied, MAPeriod, T3MAVolumeFactor);
                    break;
            }
        }

        public override void Calculate(int index)
        {
            // Moving Average Type to be displayed according to MA_Type input
            switch (MAType)
            {
                case 1:
                    // Simple        			
                    Buffer[index] = _sma.Result[index];
                    break;

                case 2:
                    // Exponential
                    Buffer[index] = _ema.Result[index];
                    break;
                case 3:
                    // Smoothed Moving Average
                    Buffer[index] = _wsma.Result[index];
                    break;
                case 4:
                    // Linear Weighted Moving Average
                    Buffer[index] = _lwma.Result[index];
                    break;
                case 5:
                    // Double Exponential
                    Buffer[index] = _dema.Result[index];
                    break;
                case 6:
                    // Triple Exponential Moving Average. 	
                    Buffer[index] = _tema.Result[index];
                    break;
                case 7:
                    // T3 Moving Average 
                    Buffer[index] = _t3MA.Result[index];
                    break;
            }

            UpBuffer[index] = Buffer[index];
            DownBuffer[index] = Buffer[index];
            XPBuffer[index] = Buffer[index];

        }
    }
}
