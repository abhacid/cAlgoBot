// -------
// PURPOSE
// -------
// Alerts user when there is high volatility in the market over a period of seconds.
// Userful signal when not looking at charts or when not at computer, configurable audible sound alerts for each currency.
// Good for scalping.

// Author: Paul Hayes    
// Date:   09/05/2015
// Version 1.4
//
// Coding Guidlines: https://github.com/dotnet/corefx/wiki/Framework-Design-Guidelines-Digest
//
// Bug fix: display formatting issue.

using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ScalpersBuddy : Indicator
    {
        #region user defined parameters

        [Parameter("Alert ON", DefaultValue = 1, MaxValue = 1, MinValue = 0)]
        public bool AlertOn { get; set; }

        [Parameter("Sound ON", DefaultValue = 0, MaxValue = 1, MinValue = 0)]
        public bool PlaySound { get; set; }

        // How much movement in pips triggers the alert.
        [Parameter("Volatility Pips", DefaultValue = 10, MaxValue = 20, MinValue = 1)]
        public int VolatilityPips { get; set; }

        // User enters the actual path to the media file.
        [Parameter("Media File", DefaultValue = "c:\\windows\\media\\notify.wav")]
        public string MediaFile { get; set; }

        [Parameter("Display Position, 1-8", DefaultValue = 1, MinValue = 1, MaxValue = 8)]
        public int WarningPostion { get; set; }

        [Parameter("Warning Color", DefaultValue = "Red")]
        public string WarningColor { get; set; }

        [Parameter("Show Spread", DefaultValue = 1, MaxValue = 1, MinValue = 0)]
        public bool ShowSpread { get; set; }

        [Parameter("Show Volatility", DefaultValue = 1, MaxValue = 1, MinValue = 0)]
        public bool ShowVolatility { get; set; }

        [Parameter("Spread Color", DefaultValue = "White")]
        public string SpreadColor { get; set; }

        #endregion

        #region Private properties

        private MarketSeries minuteSeries;
        private StaticPosition position;
        private Colors warningTextColor;
        private Colors spreadTextColor;
        private bool errorOccured = false;
        private string lowerPosition = string.Empty;

        #endregion

        const string errorMsg = "\n\n\n\n\n Scalpers Buddy Indicator: An error has occured, view log events window for more information.";

        #region cTrader Events

        protected override void Initialize()
        {
            try
            {
                // Get the 1 min timeframe series of data
                minuteSeries = MarketData.GetSeries(Symbol, TimeFrame.Minute);
                warningTextColor = (Colors)Enum.Parse(typeof(Colors), WarningColor, true);
                spreadTextColor = (Colors)Enum.Parse(typeof(Colors), SpreadColor, true);
            } catch (Exception e)
            {
                errorOccured = true;
                Print("Scalpers Buddy: " + e.Message);
            }

            // position alert message on screen
            switch (WarningPostion)
            {
                case 1:
                    position = StaticPosition.TopLeft;
                    break;
                case 2:
                    position = StaticPosition.TopCenter;
                    break;
                case 3:
                    position = StaticPosition.TopRight;
                    //lowerPosition = "\n\n";
                    break;
                case 4:
                    position = StaticPosition.Right;
                    lowerPosition = "\n\n";
                    break;
                case 5:
                    position = StaticPosition.BottomRight;
                    lowerPosition = "\n\n";
                    break;
                case 6:
                    position = StaticPosition.BottomCenter;
                    lowerPosition = "\n\n";
                    break;
                case 7:
                    position = StaticPosition.BottomLeft;
                    lowerPosition = "\n\n";
                    break;
                case 8:
                    position = StaticPosition.Left;
                    lowerPosition = "\n\n";
                    break;
                default:
                    position = StaticPosition.TopLeft;
                    break;
            }
        }

        public override void Calculate(int index)
        {
            if (errorOccured)
            {
                ChartObjects.DrawText("errorlabel", errorMsg, StaticPosition.TopCenter, Colors.Red);
                return;
            }

            // get the last highest price value
            double high = (minuteSeries.High.LastValue);
            // get the last lowest price value
            double low = (minuteSeries.Low.LastValue);

            // difference between high and low devided by the current instruments pip size = sudden movement in pips
            double pips = (high - low) / Symbol.PipSize;

            string pipsVolatility = "Vol   : " + pips.ToString("0.00") + " pips";

            // display error message to screen.
            if (ShowVolatility)
            {
                ChartObjects.DrawText("volatilityMsg", pipsVolatility += lowerPosition, position, spreadTextColor);
            }

            // if pip movement > volatility setting 
            if (Math.Ceiling(pips) > VolatilityPips)
            {
                if (AlertOn)
                {
                    ChartObjects.DrawText("alertMsg", pipsVolatility, position, warningTextColor);
                }

                if (PlaySound)
                {
                    if (MediaFile != string.Empty)
                        Notifications.PlaySound(MediaFile);
                }
            }
            else
            {
                ChartObjects.RemoveObject("alertMsg");
            }

            // if user wants to see the current bid/ask spread size, * feature seperate from volatility alert.
            if (ShowSpread)
            {
                var spread = Math.Round(Symbol.Spread / Symbol.PipSize, 2);
                string s = string.Format("{0:N2}", spread);

                ChartObjects.DrawText("spreadMsg", "\nSpread: " + s, position, spreadTextColor);
            }
        }

        #endregion
    }
}
