// -------------------------------------------------------------------------------------------------
//
//    A technical indicator developed by Tushar Chande to numerically identify trends in candlestick charting. 
//    It is calculated by taking an N period moving average of the difference between the open and closing prices. 
//    A Qstick value greater than zero means that the majority of the last 'n' days have been up, 
//    indicating that buying pressure has been increasing.
//
// -------------------------------------------------------------------------------------------------
using cAlgo.API;
using cAlgo.API.Indicators;

namespace cAlgo.Indicators
{
    [Indicator(AccessRights = AccessRights.None)]
    public class QStick : Indicator
    {
        private MovingAverage _ma;
        private IndicatorDataSeries _price;

        [Parameter( DefaultValue = 14)]
        public int Period { get; set; }

        [Parameter("MA Type", DefaultValue = MovingAverageType.Simple)]
        public MovingAverageType MaType { get; set; }

        [Output("QStick")]
        public IndicatorDataSeries Result { get; set; }

        protected override void Initialize()
        {
            _price = CreateDataSeries();
            _ma = Indicators.MovingAverage(_price, Period, MaType);
        }
        public override void Calculate(int index)
        {
            _price[index] = MarketSeries.Close[index] - MarketSeries.Open[index];
            Result[index] = _ma.Result[index];
        }
    }
}
