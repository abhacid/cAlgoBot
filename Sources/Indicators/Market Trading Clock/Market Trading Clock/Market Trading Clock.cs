// --------------------------------------
// TRADING HOURS OF MAJOR STOCK EXCHANGES
//---------------------------------------

// Author: Paul Hayes    
// Date:   09/05/2015
// Version 1.6
//
// How to change opening and closing times?
// Simply modify the values in the EXCHANGE creation from the Initialize method.
//
//      Exchange exchange = new Exchange(ExchangeName.Wellington);
//      exchange.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
//      exchange.UtcOffset =  "UTC/GMT Time difference for exchange";
//      exchange.OpeningTime = "09:00:00"; (OPENING TIME)
//      exchange.ClosingTime = "17:00:00"; (CLOSING TIME)
//      exchange.LabelFormat = "{0,-134}";
//      exchange.TimeFormat = "\n{0,-131}";
//      exchange.DisplayOrder = 1;
//      marketClocks.Exchanges.Add(exchange);
//
// To add a new exchange just add a new exchange object using the code above together with a new value in the enum ExchangeName.
// LabelFormat, TimeFormat = positioning of clock.
// 
// Coding Guidlines: https://github.com/dotnet/corefx/wiki/Framework-Design-Guidelines-Digest

using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace cAlgo.Indicators
{
    // add new value here for a new exchange
    public enum ExchangeName
    {
        Wellington,
        Sydney,
        Tokyo,
        Singapore,
        Frankfurt,
        London,
        NewYork
    }

    /// <summary>
    /// CTrader API Events
    /// </summary>
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class MarketTradingClock : Indicator
    {

        #region Public User Defined Fields

        [Parameter("Show My TimeZone?", DefaultValue = 1)]
        public bool IsLocalTimeZone { get; set; }

        [Parameter("Open Color", DefaultValue = "White")]
        public string OpenColor { get; set; }

        [Parameter("Closed Color", DefaultValue = "Gray")]
        public string ClosedColor { get; set; }

        [Parameter("Opening Bell", DefaultValue = 1)]
        public bool MarketAlert { get; set; }

        [Parameter("24Hr Clock?", DefaultValue = 0)]
        public bool Is24Hr { get; set; }

        // User enters the actual path to the media file.
        [Parameter("Media File Path", DefaultValue = "c:\\windows\\media\\Notify.wav")]
        public string MediaFile { get; set; }

        [Parameter("Show Opening Times?", DefaultValue = 1)]
        public bool ShowOpeningTimes { get; set; }

        [Parameter("Show Closing Times?", DefaultValue = 0)]
        public bool ShowClosingTimes { get; set; }

        // how many minutes until a warning sound and message tell user market is about to open
        [Parameter("Alert Market Opens (mins)", DefaultValue = 60, MinValue = 0, MaxValue = 60)]
        public int AlertBeforeMarketOpens { get; set; }

        [Parameter("Show TimeZone Label?", DefaultValue = 1)]
        public bool ShowTimeZonelabel { get; set; }

        [Parameter("FxPro Broker?", DefaultValue = 0)]
        public bool IsFxProBroker { get; set; }

        #endregion

        #region Private Fields

        private MarketHoursWidget widget;
        private bool errorOccured = false;
        private string errorMsg = "\n\n";

        #endregion

        #region CTrader Events

        /// <summary>
        /// create exchange object list and initialise the clock widget.
        /// </summary>
        protected override void Initialize()
        {
            try
            {
                Colors openColor;
                Colors closedColor;

                // are the color input strings valid?    
                try
                {
                    openColor = (Colors)Enum.Parse(typeof(Colors), OpenColor, true);
                    closedColor = (Colors)Enum.Parse(typeof(Colors), ClosedColor, true);
                } catch
                {
                    throw new Exception("Open or closed color name(s) are incorrect.");
                }

                // setup a collection of the market exchanges
                MarketClocks marketClocks = new MarketClocks();

                // create clock entities. * CHANGE CLOCK TIMES HERE *
                Exchange exchange = new Exchange(ExchangeName.Wellington);
                exchange.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("New Zealand Standard Time");
                exchange.UtcOffset = 12;
                exchange.OpeningTime = "09:05:00";
                exchange.ClosingTime = "18:00:00";
                exchange.LabelFormat = "{0,-134}";
                exchange.TimeFormat = "\n{0,-131}";
                exchange.DisplayOrder = 1;
                marketClocks.Exchanges.Add(exchange);

                exchange = new Exchange(ExchangeName.Sydney);
                exchange.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("AUS Eastern Standard Time");
                exchange.UtcOffset = 10;
                exchange.OpeningTime = "07:05:00";
                exchange.ClosingTime = "16:00:00";
                exchange.LabelFormat = "{0,-79}";
                exchange.TimeFormat = "\n{0,-80}";
                exchange.DisplayOrder = 2;
                marketClocks.Exchanges.Add(exchange);

                exchange = new Exchange(ExchangeName.Tokyo);
                exchange.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                exchange.UtcOffset = 9;
                exchange.OpeningTime = "09:00:00";
                exchange.ClosingTime = "18:00:00";
                exchange.LabelFormat = "{0,-29}";
                exchange.TimeFormat = "\n{0,-31}";
                exchange.DisplayOrder = 3;
                marketClocks.Exchanges.Add(exchange);

                exchange = new Exchange(ExchangeName.Singapore);
                exchange.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                exchange.UtcOffset = 8;
                exchange.OpeningTime = "09:00:00";
                exchange.ClosingTime = "17:00:00";
                exchange.LabelFormat = "{0,33}";
                exchange.TimeFormat = "\n{0,31}";
                exchange.DisplayOrder = 4;
                marketClocks.Exchanges.Add(exchange);

                exchange = new Exchange(ExchangeName.Frankfurt);
                exchange.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("W. Europe Standard Time");
                exchange.UtcOffset = 2;
                exchange.OpeningTime = "08:00:00";
                exchange.ClosingTime = "17:00:00";
                exchange.LabelFormat = "{0,82}";
                exchange.TimeFormat = "\n{0,80}";
                exchange.DisplayOrder = 5;
                marketClocks.Exchanges.Add(exchange);

                exchange = new Exchange(ExchangeName.London);
                exchange.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
                exchange.UtcOffset = 1;
                exchange.OpeningTime = "08:00:00";
                exchange.ClosingTime = "17:00:00";
                exchange.LabelFormat = "{0,127}";
                exchange.TimeFormat = "\n{0,128}";
                exchange.DisplayOrder = 6;
                marketClocks.Exchanges.Add(exchange);

                exchange = new Exchange(ExchangeName.NewYork);
                exchange.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
                exchange.UtcOffset = -4;
                exchange.OpeningTime = "08:00:00";
                exchange.ClosingTime = "17:00:00";
                exchange.LabelFormat = "{0,175}";
                exchange.TimeFormat = "\n{0,175}";
                exchange.DisplayOrder = 7;
                marketClocks.Exchanges.Add(exchange);

                // ADD NEW EXCHANGE HERE
                //exchange = new Exchange(ExchangeName.NewYork);
                //exchange.TimeZone = TimeZoneInfo.FindSystemTimeZoneById("ADD TIME ZONE");
                //exchange.UtcOffset = "UTC/GMT Time difference for exchange";
                //exchange.OpeningTime = "ADD OPEN TIME";
                //exchange.ClosingTime = "ADD CLOSE TIME";
                //exchange.LabelFormat = "ADD HORIZONTAL POSITIONING";
                //exchange.TimeFormat = "ADD HORIZONTAL POSITIONING";
                //exchange.DisplayOrder = 8; // No. 8 is last on right.
                //marketClocks.Exchanges.Add(exchange);

                // set common properties
                if (Is24Hr)
                    marketClocks.ClockFormat = "HH:mm";
                else
                    marketClocks.ClockFormat = "hh:mmtt";

                // if your local time is selected and not exchange local time.
                if (IsLocalTimeZone)
                {
                    foreach (Exchange ex in marketClocks.Exchanges)
                    {
                        ex.IsLocalTimeZone = true;
                        ex.TimeZone = TimeZoneInfo.Local;
                    }
                }

                foreach (Exchange ex in marketClocks.Exchanges)
                    ex.IsLocalTimeZone = IsLocalTimeZone;

                // setup the clock's default settings
                widget = new MarketHoursWidget(marketClocks);
                widget.OpenColor = openColor;
                widget.ClosedColor = closedColor;
                widget.MarketAlert = MarketAlert;
                widget.Position = StaticPosition.TopCenter;

                widget.AlertTimeBeforeOpen = AlertBeforeMarketOpens;

            } catch (Exception e)
            {
                errorOccured = true;
                errorMsg += e.Message;
                System.Media.SystemSounds.Asterisk.Play();
            }
        }

        public override void Calculate(int index)
        {
            if (errorOccured)
            {
                ChartObjects.DrawText("errorlabel", "* Market Trading Clock *" + errorMsg, StaticPosition.TopCenter, Colors.Red);
                return;
            }

            var linePos = "\n\n";

            // get all exhanges in display order
            var exchanges = from wq in widget.marketClocks.Exchanges
                orderby wq.DisplayOrder ascending
                select wq;

            // iterate through exchanges and draw objects onto chart
            Colors clockColor;
            foreach (var exchange in exchanges)
            {
                ChartObjects.DrawText("l_" + exchange.ExchangeName.ToString(), exchange.Label, widget.Position, clockColor = (exchange.IsOpen) ? widget.OpenColor : widget.ClosedColor);
                ChartObjects.DrawText("t_" + exchange.ExchangeName.ToString(), exchange.TimeLabel, widget.Position, clockColor = (exchange.IsOpen) ? widget.OpenColor : widget.ClosedColor);

                if (exchange.IsOpen)
                    ChartObjects.RemoveObject("mo_" + exchange.ExchangeName.ToString());
            }

            if (ShowTimeZonelabel)
            {
                if (IsLocalTimeZone)
                    ChartObjects.DrawText("tz", TimeZoneInfo.Local.DisplayName, StaticPosition.TopLeft, widget.OpenColor);
                else
                    ChartObjects.DrawText("tz", "Exchange Local Time", StaticPosition.TopLeft, widget.OpenColor);
            }

            // show exchange opening times when they are closed if user has chosen option.
            if (ShowOpeningTimes)
            {
                foreach (var exchange in exchanges.Where(x => x.IsOpen == false))
                {
                    if (exchange.ExchangeTime.DayOfWeek == DayOfWeek.Saturday || exchange.ExchangeTime.DayOfWeek == DayOfWeek.Sunday || exchange.ExchangeTime.DayOfWeek == DayOfWeek.Friday && exchange.ExchangeTime < exchange.Closses && exchange.ExchangeTime > exchange.Opens && exchange.ExchangeTime.DayOfWeek != DayOfWeek.Monday)
                        ChartObjects.DrawText("p_" + exchange.ExchangeName.ToString(), linePos + exchange.Label.Replace(exchange.ExchangeName, "Monday"), widget.Position, widget.OpenColor);
                    else
                        ChartObjects.DrawText("p_" + exchange.ExchangeName.ToString(), linePos + exchange.Label.Replace(exchange.ExchangeName, exchange.Opens.ToString(widget.marketClocks.ClockFormat)), widget.Position, widget.OpenColor);
                }
            }

            // show exchange opening times when they are closed if user has chosen option.
            if (ShowClosingTimes)
            {
                foreach (var exchange in exchanges.Where(x => x.IsOpen))
                {
                    ChartObjects.DrawText("c_" + exchange.ExchangeName.ToString(), linePos + exchange.Label.Replace(exchange.ExchangeName, exchange.Closses.ToString(widget.marketClocks.ClockFormat)), widget.Position, widget.ClosedColor);
                }
            }

            // Play sound when market opens or closes
            try
            {
                if (exchanges.Any(x => x.MarketJustOpened()))
                {
                    ChartObjects.RemoveAllObjects();
                    if (MarketAlert && MediaFile != string.Empty)
                        Notifications.PlaySound(MediaFile);
                }
            } catch
            {
                // soft error continue showing clocks.
                ChartObjects.DrawText("errorlabel", errorMsg, StaticPosition.TopCenter, Colors.Red);
                Print("Market Trading Clock: " + "Media file path could be incorrect.");
            }

            // alert user just before market opens (value in mins set by user) returns exchange.
            if (AlertBeforeMarketOpens > 0)
            {
                if (ShowOpeningTimes)
                    linePos += "\n";

                IList<Exchange> ext = widget.marketClocks.IsMarketOpening(AlertBeforeMarketOpens);

                foreach (var ex in ext)
                {
                    ChartObjects.DrawText("mo_" + ex.ExchangeName.ToString(), linePos + ex.Label.Replace(ex.ExchangeName, ex.OpeningMins()), StaticPosition.TopCenter, widget.OpenColor);
                }
            }

            // show message when market is closed for the weekend
            if (exchanges.All(x => x.IsOpen == false))
            {
                if (this.Time.DayOfWeek == DayOfWeek.Saturday || this.Time.DayOfWeek == DayOfWeek.Sunday)
                    ChartObjects.DrawText("l8", string.Format("\n\n\n\n{0,40}", "- MARKETS CLOSED -"), StaticPosition.TopCenter, (Colors)Enum.Parse(typeof(Colors), OpenColor, true));
            }

            // FxPro inactivity message at midnight cyprus time.
            if (IsFxProBroker && widget.marketClocks.IsInactive(this.Server.Time))
                ChartObjects.DrawText("l8", string.Format("\n\n\n\n{0,60}", "- TRADING INACTIVE FOR 5 MINUTES -"), StaticPosition.TopCenter, (Colors)Enum.Parse(typeof(Colors), OpenColor, true));

            // debug
            //Print("FxPro Server Time: " + this.widget.FxProServerTime);
            //Print("local: " + exchanges.LastOrDefault().LocalTime.ToString());
            //Print("opens: " + exchanges.LastOrDefault().Opens.ToString());
            //Print("close: " + exchanges.LastOrDefault().Closses.ToString());
            //Print("time offset from UTC: " + TimeZoneInfo.Local.BaseUtcOffset.Hours.ToString());
        }

        #endregion
    }

    #region Widget Logic

    public class MarketHoursWidget
    {
        #region public properties

        // list collection to hold all the exchange objects
        public MarketClocks marketClocks;

        TimeZoneInfo LocalTimeZone = TimeZoneInfo.Local;
        TimeZoneInfo FxProTimeZone = TimeZoneInfo.FindSystemTimeZoneById("E. Europe Standard Time");

        public DateTime FxProServerTime
        {
            get
            {
                DateTime FxProTime = TimeZoneInfo.ConvertTime(DateTime.Now, LocalTimeZone, FxProTimeZone);
                return FxProTime;
            }
        }

        // alert user at set time period before market opens
        private int alertTimeBeforeOpen = 0;
        public int AlertTimeBeforeOpen
        {
            get { return alertTimeBeforeOpen; }
            set { alertTimeBeforeOpen = value; }
        }

        // all markets closed for weekend?
        private bool marketsClosed = false;
        public bool MarketsClosed
        {
            get { return marketsClosed; }
            set { marketsClosed = value; }
        }

        private bool marketAlert;
        public bool MarketAlert
        {
            get { return marketAlert; }
            set { marketAlert = value; }
        }

        private Colors openColor;
        public Colors OpenColor
        {
            get { return openColor; }
            set { openColor = value; }
        }

        private Colors closedColor;
        public Colors ClosedColor
        {
            get { return closedColor; }
            set { closedColor = value; }
        }

        private StaticPosition position;
        public StaticPosition Position
        {
            get { return position; }
            set { position = value; }
        }

        #endregion

        // Any construction logic to be included here later
        public MarketHoursWidget(MarketClocks marketClocks)
        {
            this.marketClocks = marketClocks;
        }
    }

    #endregion

    #region Exchange Entity Class

    /// <summary>
    /// The collection of exchanges and their common properties
    /// </summary>
    public class MarketClocks
    {
        private IList<Exchange> exchanges;
        public MarketClocks()
        {
            exchanges = new List<Exchange>();
        }

        public IList<Exchange> Exchanges
        {
            get { return exchanges; }
            set { exchanges = value; }
        }

        private string clockFormat = string.Empty;
        public string ClockFormat
        {
            get { return clockFormat; }
            set
            {
                foreach (var ex in exchanges)
                {
                    ex.ClockFormat = value;
                }
                clockFormat = value;
            }
        }
        public bool IsInactive(DateTime serverTime)
        {
            TimeSpan start = new TimeSpan(23, 59, 55);
            TimeSpan stop = new TimeSpan(0, 5, 0);

            if (serverTime.TimeOfDay >= start && serverTime.TimeOfDay <= stop)
                return true;
            else
                return false;
        }

        // returns all exchanges that is about to open.
        public IList<Exchange> IsMarketOpening(int AlertBeforeMarketOpens)
        {
            AlertBeforeMarketOpens--;

            var exchanges = from ex in this.Exchanges
                where (ex.ExchangeTime.TimeOfDay >= ex.Opens.AddMinutes(-AlertBeforeMarketOpens).TimeOfDay && ex.ExchangeTime.TimeOfDay <= ex.Opens.TimeOfDay) && (ex.ExchangeTime.DayOfWeek != DayOfWeek.Saturday && ex.ExchangeTime.DayOfWeek != DayOfWeek.Sunday)
                select ex;

            return exchanges.ToList();
        }

        // Is it the weekend for all exchanges (weekend for everyone). all exchanges must be either sat or sun state
        // * redundant method.
        public bool MarketsClosed
        {
            get
            {
                bool closed = true;
                foreach (var ex in exchanges)
                {
                    if (ex.ExchangeTime.DayOfWeek != DayOfWeek.Saturday && ex.ExchangeTime.DayOfWeek != DayOfWeek.Sunday && ex.IsOpen)
                        closed = false;
                }
                return closed;
            }
        }
    }

    public class Exchange
    {
        #region public properties

        // if user has selected another timezone then override the timezones.
        int utcLocalMachineOffsetHours = TimeZoneInfo.Local.BaseUtcOffset.Hours;

        private int utcOffset;
        public int UtcOffset
        {
            get { return utcOffset; }
            set
            {
                // convert to positive if negative and negative if positive.
                if (value < 0)
                    utcOffset = Math.Abs(value);
                else
                    utcOffset = value * -1;
            }
        }

        private bool isLocalTimeZone = false;
        public bool IsLocalTimeZone
        {
            get { return isLocalTimeZone; }
            set { isLocalTimeZone = value; }
        }

        private TimeZoneInfo timeZone;
        public TimeZoneInfo TimeZone
        {
            get { return timeZone; }
            set { timeZone = value; }
        }

        // return the exact date and time for the exchange locality.
        public DateTime ExchangeTime
        {
            get { return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local, TimeZone); }
        }

        public DateTime LocalTime
        {
            get { return TimeZoneInfo.ConvertTime(DateTime.Now, TimeZoneInfo.Local); }
        }

        private string openingTime;
        public string OpeningTime
        {
            get { return openingTime; }
            set { openingTime = value; }
        }

        private string closingTime;
        public string ClosingTime
        {
            get { return closingTime; }
            set { closingTime = value; }
        }

        private DateTime opens;
        public DateTime Opens
        {
            get
            {
                this.opens = this.ExchangeTime;
                this.opens = DateTime.Parse(this.openingTime);
                if (IsLocalTimeZone)
                {
                    // Convert to UTC/GMT (0)
                    this.opens = this.opens.AddHours(this.UtcOffset + 1);
                    // Adjust opening time for local machines UTC offset.
                    this.opens = this.opens.AddHours(utcLocalMachineOffsetHours);
                    return opens;
                }
                else
                    return opens;
            }
            set { opens = value; }
        }

        private DateTime closes;
        public DateTime Closses
        {
            get
            {
                this.closes = this.ExchangeTime;
                this.closes = DateTime.Parse(this.closingTime);
                if (IsLocalTimeZone)
                {
                    // Convert to UTC/GMT (0)
                    this.closes = this.closes.AddHours(this.UtcOffset + 1);
                    // Adjust opening time for local machines UTC offset.
                    this.closes = this.closes.AddHours(utcLocalMachineOffsetHours);
                    return closes;
                }
                else
                    return closes;
            }
            set { closes = value; }
        }

        // set the clock color for the exchanges for when they are open or closed
        public bool IsOpen
        {
            get { return ExchangeTime.TimeOfDay >= Opens.TimeOfDay && ExchangeTime.TimeOfDay < Closses.TimeOfDay && ExchangeTime.DayOfWeek != DayOfWeek.Saturday && ExchangeTime.DayOfWeek != DayOfWeek.Sunday ? true : false; }
        }

        // Check to see if the market has just opened or closed
        public bool MarketJustOpened()
        {
            if (ExchangeTime.Hour == Opens.Hour && ExchangeTime.Minute == Opens.Minute && ExchangeTime.Second <= 2)
                return true;
            else
                return false;

        }

        private int displayOrder = 1;
        public int DisplayOrder
        {
            get { return displayOrder; }
            set { displayOrder = value; }
        }

        private string exchangeName = string.Empty;
        public string ExchangeName
        {
            get { return exchangeName; }
            set { exchangeName = value; }
        }

        public string Label
        {
            get { return string.Format(LabelFormat, this.ExchangeName.ToString()); }
        }

        public string TimeLabel
        {
            get { return string.Format(this.TimeFormat, ExchangeTime.ToString(ClockFormat)); }
        }

        private string clockFormat = string.Empty;
        public string ClockFormat
        {
            get { return clockFormat; }
            set { clockFormat = value; }
        }

        private string labelFormat;
        public string LabelFormat
        {
            get { return labelFormat; }
            set { labelFormat = value; }
        }

        private string timeFormat;
        public string TimeFormat
        {
            get { return timeFormat; }
            set { timeFormat = value; }
        }

        #endregion

        // construct a new exchange object
        public Exchange(ExchangeName Name)
        {
            this.exchangeName = Name.ToString();
        }

        public string OpeningMins()
        {
            TimeSpan ts = this.Opens.TimeOfDay - this.ExchangeTime.TimeOfDay;
            int mins = ts.Minutes;
            mins++;
            return "(" + mins.ToString() + " mins)";
        }
    }

    #endregion
}
