// -------------------------------------------------------------------------------
//
//    Display of countdown to the next bar on the top right of the chart. 
//    Timeframes supported up to D1/
//    Format HH:mm:ss
//
//-------------------------------------------------------------------------------

using System;
using cAlgo.API;
using System.Timers;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class CountdownTimerDisplay : Indicator
    {
        private readonly Timer _timer = new Timer();
        private TimeSpan _timeFrame;
        private TimeSpan _nextOpenTime;
        private int _serverHourDiff;
        private DateTime _serverTime;
        private int _index;

        protected override void Initialize()
        {
            _timer.Elapsed += OnTimedEvent;
            //  OnTimedEvent is called at each timer tick
            _timer.Interval = 1000;
            // Timer will tick every Interval (milliseconds)
            _timer.Enabled = true;
            // Enable the timer
            _timer.Start();
            // Start the timer
        }


        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {

            _index = MarketSeries.Close.Count - 1;
            _timeFrame = MarketSeries.OpenTime[_index] - MarketSeries.OpenTime[_index - 1];

            if (_timeFrame <= TimeSpan.FromHours(24.0))
            {
                _nextOpenTime = MarketSeries.OpenTime[_index].TimeOfDay + _timeFrame;
                _serverHourDiff = MarketSeries.OpenTime[_index].TimeOfDay.Hours - DateTime.Now.TimeOfDay.Hours;
                _serverTime = DateTime.Now.AddHours(_serverHourDiff);

                ChartObjects.DrawText("CountDown", (_nextOpenTime - _serverTime.TimeOfDay).ToString().Substring(0, 8), StaticPosition.TopRight, Colors.White);
            }
        }

        public override void Calculate(int index)
        {
            // Calculate value at specified index
            // Result[index] = ...
        }
    }
}

