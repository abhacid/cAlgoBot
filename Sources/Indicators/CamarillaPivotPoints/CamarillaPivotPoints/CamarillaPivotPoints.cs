using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class CamarillaPivotPoints : Indicator
    {

        private double _close;
        private double _higher;
        private double _lower;
        
        #region Output

        [Output("Pivot", LineStyle = LineStyle.Lines)]
        public IndicatorDataSeries Pivot { get; set; }

        [Output("R1", LineStyle = LineStyle.Lines, Color = Colors.Blue)]
        public IndicatorDataSeries R1 { get; set; }

        [Output("R2", LineStyle = LineStyle.Lines, Color = Colors.Blue)]
        public IndicatorDataSeries R2 { get; set; }

        [Output("R3", LineStyle = LineStyle.Lines, Color = Colors.Blue)]
        public IndicatorDataSeries R3 { get; set; }

        [Output("R4", LineStyle = LineStyle.Lines, Color = Colors.Blue)]
        public IndicatorDataSeries R4 { get; set; }

        [Output("S1", LineStyle = LineStyle.Lines, Color = Colors.Red)]
        public IndicatorDataSeries S1 { get; set; }

        [Output("S2", LineStyle = LineStyle.Lines, Color = Colors.Red)]
        public IndicatorDataSeries S2 { get; set; }

        [Output("S3", PlotType = PlotType.Line, LineStyle = LineStyle.Lines, Color = Colors.Red)]
        public IndicatorDataSeries S3 { get; set; }

        [Output("S4", PlotType = PlotType.Line, LineStyle = LineStyle.Lines, Color = Colors.Red)]
        public IndicatorDataSeries S4 { get; set; }

        #endregion

        #region Input Parameters

        [Parameter("Number of Pivots", DefaultValue = 4, MinValue = 1, MaxValue = 4)]
        public int NoPiv { get; set; }

        [Parameter("DrawingWidth", DefaultValue = 50, MaxValue = 100)]
        public int DrawingWidth { get; set; }

        #endregion

        public override void Calculate(int index)
        {
            DateTime currentOpenTime = MarketSeries.OpenTime[index];
            DateTime previousOpenTime = MarketSeries.OpenTime[index - 1];
            DateTime today = DateTime.Now;
            DateTime yesterday = today.AddDays(-1);

            // Initialize High & Low
            if (currentOpenTime.Day == yesterday.Day && previousOpenTime.Day != yesterday.Day)
            {
                _higher = MarketSeries.High[index];
                _lower = MarketSeries.Low[index];
            }

            // Calculate High & Low of previous day
            if ((currentOpenTime.Day == yesterday.Day && today.DayOfWeek != DayOfWeek.Monday) ||
            (today.DayOfWeek == DayOfWeek.Monday && currentOpenTime.DayOfYear == today.AddDays(-3).Day))
            {
                if (MarketSeries.High[index] > _higher)
                {
                    _higher = MarketSeries.High[index];
                }
                if (MarketSeries.Low[index] < _lower)
                {
                    _lower = MarketSeries.Low[index];
                }
            }

            // Set Close of previous day - Close of Last Bar of prevous Day
            if (previousOpenTime.Day == yesterday.Day && currentOpenTime.Day == today.Day)
            {
                _close = MarketSeries.Close[index - 1];
            }

            // Only show output in todays timeframe
            if (currentOpenTime.Date != today.Date) return;

            // Calculate output

            Pivot[index] = (_higher + _lower + _close) / 3;

            R1[index] = _close + (_higher - _lower) * 0.0916;
            S1[index] = _close - (_higher - _lower) * 0.0916;
                
            // Display additional pivots according to input
            if (NoPiv >= 2)
            {
                R2[index] = _close + (_higher - _lower) * 0.1833;
                S2[index] = _close - (_higher - _lower) * 0.1833;
            }
            if (NoPiv >= 3)
            {
                R3[index] = _close + (_higher - _lower) * 0.275;
                S3[index] = _close - (_higher - _lower) * 0.275;
            }
            if (NoPiv < 4) return;
            
            R4[index] = _close + (_higher - _lower) * 0.55;
            S4[index] = _close - (_higher - _lower) * 0.55;

        }
    }
}
