//#reference: AverageTrueRange.algo

using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true)]
    public class TmaBands : Indicator
    {
        [Parameter("Sound File Path", DefaultValue = "C:\\Windows\\Media\\notify.wav")]
        public string PathToSoundFile { get; set; }

        [Parameter("To Email", DefaultValue = "someone@somewhere.com")]
        public string ToEmail { get; set; }

        [Parameter("From Email", DefaultValue = "someone@somewhere.com")]
        public string FromEmail { get; set; }

        [Parameter(DefaultValue = 20, MinValue = 1)]
        public int HalfLength { get; set; }

        [Parameter(DefaultValue = "current time frame")]
        public string TimeFrame { get; set; }

        [Parameter()]
        public DataSeries Price { get; set; }

        [Parameter(DefaultValue = 2.0)]
        public double ATRMultiplier { get; set; }

        [Parameter(DefaultValue = 100)]
        public int ATRPeriod { get; set; }

        [Parameter(DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int AlertsOn { get; set; }

        [Parameter(DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int AlertsOnCurrent { get; set; }

        [Parameter(DefaultValue = 1, MinValue = 0, MaxValue = 1)]
        public int AlertsOnHighLow { get; set; }


        [Parameter(DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int AlertsSound { get; set; }

        [Parameter(DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int AlertsEmail { get; set; }

        [Output("Buffer1", Color = Colors.Red, PlotType = PlotType.Points)]
        public IndicatorDataSeries Buffer1 { get; set; }

        [Output("Buffer2", Color = Colors.Green)]
        public IndicatorDataSeries Buffer2 { get; set; }

        [Output("Buffer3", Color = Colors.Green)]
        public IndicatorDataSeries Buffer3 { get; set; }

        private int[] _trend;
        private AverageTrueRange _atr;
        private DateTime _previousOpenTime = DateTime.MinValue;
        private string _previousAlert = string.Empty;
        private bool _calculateValue;

        protected override void Initialize()
        {
            _calculateValue = TimeFrame == "calculateValue";
            _trend = new int[MarketSeries.Close.Count];
            _atr = Indicators.GetIndicator<AverageTrueRange>(ATRPeriod);
        }



        public override void Calculate(int index)
        {
            if (index < HalfLength)
                return;

            if (_trend.Length < index)
                Array.Resize(ref _trend, index);

            double sum = (HalfLength + 1) * Price[index];
            double sumw = (HalfLength + 1);

            for (int j = 1, k = HalfLength; j <= HalfLength; j++,k--)
            {
                sum += k * Price[index - j];
                sumw += k;
            }

            double range = _atr.Result[index - 10] * ATRMultiplier;

            Buffer1[index] = sum / sumw;
            Buffer2[index] = Buffer1[index] + range;
            Buffer3[index] = Buffer1[index] - range;
            _trend[index] = 0;

            if (AlertsOnHighLow == 1)
            {
                if (MarketSeries.High[index] > Buffer2[index])
                    _trend[index] = 1;
                if (MarketSeries.Low[index] < Buffer3[index])
                    _trend[index] = -1;
            }
            else
            {
                if (MarketSeries.Close[index] > Buffer2[index])
                    _trend[index] = 1;
                if (MarketSeries.Close[index] < Buffer3[index])
                    _trend[index] = -1;
            }
            if (!_calculateValue)
                ManageAlerts(index);
        }

        protected void ManageAlerts(int index)
        {
            if (AlertsOn > 0)
            {
                int whichBar;
                if (AlertsOnCurrent > 0)
                    whichBar = index;
                else
                    whichBar = index - 1;


                if (_trend[whichBar] != _trend[whichBar - 1])
                {
                    if (_trend[whichBar] == 1)
                        DoAlert(whichBar, "up");
                    else if (_trend[whichBar] == -1)
                        DoAlert(whichBar, "down");
                }
            }
        }

        private void DoAlert(int previousIndex, string currentAlert)
        {

            if (_previousOpenTime != MarketSeries.OpenTime[previousIndex] || _previousAlert != currentAlert)
            {
                _previousOpenTime = MarketSeries.OpenTime[previousIndex];
                _previousAlert = currentAlert;

                if (AlertsEmail > 0)
                {
                    TimeSpan timeframespan = GetTimeFrame();
                    string timeframe = GetTimeFrameName(timeframespan);

                    string subject = String.Format("{0} TMA bands", Symbol.Code);
                    string message = String.Format("{0} at {1} {2} TMA bands price penetrated {3} band", Symbol.Code, DateTime.Now, timeframe, currentAlert);
                    Notifications.SendEmail(FromEmail, ToEmail, subject, message);
                }

                if (AlertsSound > 0)
                    Notifications.PlaySound(PathToSoundFile);
            }
        }

        /// <summary>
        /// Get the name representation of the timeframe used
        /// </summary>
        /// <param name="timeFrame">Time span between two consecutive bars OpenTime</param>
        /// <returns>The name representation of the TimeFrame</returns>
        private string GetTimeFrameName(TimeSpan timeFrame)
        {
            var totalMin = (int)timeFrame.TotalMinutes;
            string timeFrameName;

            if (totalMin > 10080)
                timeFrameName = "M1";
            else
            {
                switch (totalMin)
                {
                    case 1:
                        timeFrameName = "m1";
                        break;
                    case 2:
                        timeFrameName = "m2";
                        break;
                    case 3:
                        timeFrameName = "m3";
                        break;
                    case 4:
                        timeFrameName = "m4";
                        break;
                    case 5:
                        timeFrameName = "m5";
                        break;
                    case 10:
                        timeFrameName = "m10";
                        break;
                    case 15:
                        timeFrameName = "m15";
                        break;
                    case 30:
                        timeFrameName = "m30";
                        break;
                    case 60:
                        timeFrameName = "h1";
                        break;
                    case 240:
                        timeFrameName = "h4";
                        break;
                    case 720:
                        timeFrameName = "h12";
                        break;
                    case 1440:
                        timeFrameName = "D1";
                        break;
                    case 10080:
                        timeFrameName = "W1";
                        break;
                    default:
                        timeFrameName = "0";
                        break;

                }
            }

            return timeFrameName;
        }

        /// <summary>
        /// Get the time span between two consecutive bars OpenTime
        /// </summary>
        private TimeSpan GetTimeFrame()
        {
            if (MarketSeries.Close.Count > 2)
            {
                int currentIndex = MarketSeries.Close.Count - 1;
                DateTime currentOpenTime = MarketSeries.OpenTime[currentIndex];

                DateTime previousOpenTime = MarketSeries.OpenTime[currentIndex - 1];

                TimeSpan timeFrame = currentOpenTime - previousOpenTime;

                if (currentOpenTime.DayOfWeek == DayOfWeek.Monday && previousOpenTime.DayOfWeek != DayOfWeek.Monday)
                {
                    currentOpenTime = previousOpenTime;
                    previousOpenTime = MarketSeries.OpenTime[currentIndex - 2];
                    timeFrame = currentOpenTime - previousOpenTime;
                }

                return timeFrame;
            }
            // if bars are not available
            return TimeSpan.Zero;
        }
    }
}
