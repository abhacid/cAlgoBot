// ------------------------------------------------------------                   
// Paste this code into your cAlgo editor. 
// -----------------------------------------------------------
using System;
using System.Collections.Generic;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Threading;
using cAlgo.API.Requests;
// ---------------------------------------------------------------------------                   
// Converted from MQ4 to cAlgo with http://2calgo.com
// ---------------------------------------------------------------------------

namespace cAlgo.Indicators
{
    [Indicator(ScalePrecision = 5, AutoRescale = false, IsOverlay = true, AccessRights = AccessRights.None)]
    [Levels()]
    public class EverTrend_Indicator : Indicator
    {
        Mq4Double Mq4Init()
        {
            SRLowDateTime = Time[1];
            SRHighDateTime = Time[1];

            return 0;
            return 0;
        }
        Mq4Double deinitFunc()
        {
            ClearVariablesFunc();

            return 0;
            return 0;
        }
        Mq4Double Mq4Start()
        {
            Mq4Double LineIdx = 0;
            Mq4Double IDX = 0;
            Mq4Double counted_bars = 0;
            counted_bars = IndicatorCounted();


            ClearVariablesFunc();

            GetSRPointsFunc();

            if (ShowSRLinesWithInvalidPoints == false)
                HideInvalidatedSRPointsFunc();

            if (ShowTrendLines == true)
                DrawTrendLinesFunc();

            return 0;
            return 0;
        }
        Mq4Double GetSRPointsFunc()
        {
            Mq4Double IDX = 0;


            for (IDX = TrendLinePeriod; IDX >= 1; IDX--)
            {

                if (Low[IDX] <= SRLow)
                {
                    SRLow = Low[IDX];
                    SRLowDateTime = Time[IDX];
                    SRLowBar = IDX;
                }


                if (High[IDX] >= SRHigh)
                {
                    SRHigh = High[IDX];
                    SRHighDateTime = Time[IDX];
                    SRHighBar = IDX;
                }


                if (IDX <= SRLowBar - CycleSize && SRMode == "LOW")
                {
                    DrawHLineFunc("S", SRLowDateTime, SRLow, STYLE_SOLID, Crimson);

                    SRMode = "HIGH";
                    SRHigh = 0;
                    SRHighDateTime = Time[IDX];
                    SRHighBar = 0;

                    IDX = SRLowBar - 1;
                }


                if (IDX <= SRHighBar - CycleSize && SRMode == "HIGH")
                {
                    DrawHLineFunc("R", SRHighDateTime, SRHigh, STYLE_SOLID, MediumSeaGreen);

                    SRMode = "LOW";
                    SRLow = 9999;
                    SRLowDateTime = Time[IDX];
                    SRLowBar = 0;

                    IDX = SRHighBar - 1;
                }
            }
            return 0;
        }
        Mq4Double DrawTrendLinesFunc()
        {
            Mq4String FinalLineName = "";
            Mq4Double LineIdx = 0;
            Mq4String Point2 = "";
            Mq4String Point1 = "";
            Point1 = "NULL";
            Point2 = "NULL";




            for (LineIdx = 1; LineIdx <= LineCount - 2; LineIdx++)
            {
                if ((LineValid[LineIdx] == FALSE && ShowTrendLinesWithInvalidPoints == TRUE) || LineValid[LineIdx] == TRUE)
                {

                    if (LineType[LineIdx] == "S")
                    {
                        if (Point1 == "NULL")
                        {
                            Point1 = LineName[LineIdx];
                        }
                        else
                        {
                            Point2 = LineName[LineIdx];
                            FinalLineName = DrawTrendLineFunc("TLS", Point1, Point2, STYLE_SOLID, Blue);
                            Point1 = Point2;
                        }
                    }
                }
            }
            ObjectSet(FinalLineName, OBJPROP_RAY, TRUE);

            Point1 = "NULL";
            Point2 = "NULL";


            for (LineIdx = 1; LineIdx <= LineCount - 2; LineIdx++)
            {
                if ((LineValid[LineIdx] == FALSE && ShowTrendLinesWithInvalidPoints == TRUE) || LineValid[LineIdx] == TRUE)
                {

                    if (LineType[LineIdx] == "R")
                    {
                        if (Point1 == "NULL")
                        {
                            Point1 = LineName[LineIdx];
                        }
                        else
                        {
                            Point2 = LineName[LineIdx];
                            FinalLineName = DrawTrendLineFunc("TLR", Point1, Point2, STYLE_SOLID, Blue);
                            Point1 = Point2;
                        }
                    }
                }
            }
            ObjectSet(FinalLineName, OBJPROP_RAY, TRUE);

            return 0;
        }
        Mq4Double HideInvalidatedSRPointsFunc()
        {
            Mq4Double LineIdx = 0;
            Mq4Double IDX = 0;


            for (IDX = TrendLinePeriod; IDX > 1; IDX--)
            {

                for (LineIdx = 1; LineIdx <= LineCount; LineIdx++)
                {

                    if (ObjectGet(LineName[LineIdx], OBJPROP_PRICE1) <= High[IDX] && ObjectGet(LineName[LineIdx], OBJPROP_PRICE1) >= Low[IDX])
                    {


                        if (LineType[LineIdx] == "S" || LineType[LineIdx] == "R")
                        {
                            if (ObjectGet(LineName[LineIdx], OBJPROP_TIME1) < Time[IDX])
                            {

                                ObjectSet(LineName[LineIdx], OBJPROP_TIME2, ObjectGet(LineName[LineIdx], OBJPROP_TIME1));
                                LineValid[LineIdx] = FALSE;


                                if (LineType[LineIdx + 1] == "ARROW")
                                    ObjectSet(LineName[LineIdx + 1], OBJPROP_ARROWCODE, 4);
                            }
                        }
                    }
                }
            }
            return 0;
        }
        Mq4String DrawTrendLineFunc(Mq4String TrendLineType, Mq4String Point1, Mq4String Point2, Mq4Double Style, Mq4Double Color)
        {
            LineCount += 1;
            LineName[LineCount] = TrendLineType + "_" + DoubleToStr(LineCount, 0);
            LineType[LineCount] = TrendLineType;
            LineValid[LineCount] = TRUE;

            ObjectDelete(LineName[LineCount]);

            ObjectCreate(LineName[LineCount], OBJ_TREND, 0, 0, 0, 0, 0);
            ObjectSet(LineName[LineCount], OBJPROP_TIME1, ObjectGet(Point1, OBJPROP_TIME1));
            ObjectSet(LineName[LineCount], OBJPROP_PRICE1, ObjectGet(Point1, OBJPROP_PRICE1));
            ObjectSet(LineName[LineCount], OBJPROP_TIME2, ObjectGet(Point2, OBJPROP_TIME1));
            ObjectSet(LineName[LineCount], OBJPROP_PRICE2, ObjectGet(Point2, OBJPROP_PRICE1));
            ObjectSet(LineName[LineCount], OBJPROP_WIDTH, 2);
            ObjectSet(LineName[LineCount], OBJPROP_STYLE, Style);
            ObjectSet(LineName[LineCount], OBJPROP_COLOR, Color);
            ObjectSet(LineName[LineCount], OBJPROP_BACK, TRUE);
            ObjectSet(LineName[LineCount], OBJPROP_RAY, ShowTrendLineRays);

            return LineName[LineCount];

            return Mq4String.Empty;
        }
        Mq4Double DrawHLineFunc(Mq4String SRLineType, Mq4Double DayTime, Mq4Double Price, Mq4Double Style, Mq4Double Color)
        {
            LineCount += 1;
            LineName[LineCount] = "LINE_" + SRLineType + "_" + DoubleToStr(LineCount, 0);
            LineType[LineCount] = SRLineType;
            LineValid[LineCount] = TRUE;

            ObjectDelete(LineName[LineCount]);

            ObjectCreate(LineName[LineCount], OBJ_TREND, 0, 0, 0, 0, 0);
            ObjectSet(LineName[LineCount], OBJPROP_TIME1, DayTime);
            ObjectSet(LineName[LineCount], OBJPROP_PRICE1, Price);
            ObjectSet(LineName[LineCount], OBJPROP_TIME2, Time[0]);
            ObjectSet(LineName[LineCount], OBJPROP_PRICE2, Price);
            ObjectSet(LineName[LineCount], OBJPROP_STYLE, Style);
            ObjectSet(LineName[LineCount], OBJPROP_WIDTH, 2);
            ObjectSet(LineName[LineCount], OBJPROP_COLOR, Color);
            ObjectSet(LineName[LineCount], OBJPROP_BACK, TRUE);


            if (ShowSRLines == false)
                ObjectSet(LineName[LineCount], OBJPROP_TIME2, DayTime);



            LineCount += 1;
            LineType[LineCount] = "ARROW";
            LineValid[LineCount] = FALSE;
            LineName[LineCount] = "ARROW_" + SRLineType + "_" + DoubleToStr(LineCount, 0);

            ObjectDelete(LineName[LineCount]);

            ObjectCreate(LineName[LineCount], OBJ_ARROW, 0, DayTime, Price, 0, 0);
            ObjectSet(LineName[LineCount], OBJPROP_BACK, False);
            ObjectSet(LineName[LineCount], OBJPROP_COLOR, Color);

            if (ShowSRArrows == true)
            {
                if (SRLineType == "S")
                {
                    ObjectSet(LineName[LineCount], OBJPROP_ARROWCODE, SYMBOL_ARROWUP);
                }

                if (SRLineType == "R")
                {
                    ObjectSet(LineName[LineCount], OBJPROP_ARROWCODE, SYMBOL_ARROWDOWN);

                }
            }
            else
            {
                ObjectSet(LineName[LineCount], OBJPROP_PRICE1, 0);
            }


            return 0;
        }
        Mq4Double ClearVariablesFunc()
        {
            Mq4Double IDX = 0;
            for (IDX = 1; IDX <= LineCount; IDX++)
            {


                if (StringLen(LineName[IDX]) > 0)
                    ObjectDelete(LineName[IDX]);

                LineName[IDX] = NULL;
                LineType[IDX] = NULL;
                LineValid[IDX] = NULL;
            }



            LineCount = 0;

            return 0;
        }

        [Parameter("TrendLinePeriod", DefaultValue = 900)]
        public int TrendLinePeriod_parameter { get; set; }
        bool _TrendLinePeriodGot;
        Mq4Double TrendLinePeriod_backfield;
        Mq4Double TrendLinePeriod
        {
            get
            {
                if (!_TrendLinePeriodGot)
                    TrendLinePeriod_backfield = TrendLinePeriod_parameter;
                return TrendLinePeriod_backfield;
            }
            set { TrendLinePeriod_backfield = value; }
        }

        [Parameter("CycleSize", DefaultValue = 50)]
        public int CycleSize_parameter { get; set; }
        bool _CycleSizeGot;
        Mq4Double CycleSize_backfield;
        Mq4Double CycleSize
        {
            get
            {
                if (!_CycleSizeGot)
                    CycleSize_backfield = CycleSize_parameter;
                return CycleSize_backfield;
            }
            set { CycleSize_backfield = value; }
        }

        [Parameter("ShowTrendLines", DefaultValue = TRUE)]
        public bool ShowTrendLines_parameter { get; set; }
        bool _ShowTrendLinesGot;
        Mq4Double ShowTrendLines_backfield;
        Mq4Double ShowTrendLines
        {
            get
            {
                if (!_ShowTrendLinesGot)
                    ShowTrendLines_backfield = ShowTrendLines_parameter;
                return ShowTrendLines_backfield;
            }
            set { ShowTrendLines_backfield = value; }
        }

        [Parameter("ShowTrendLineRays", DefaultValue = TRUE)]
        public bool ShowTrendLineRays_parameter { get; set; }
        bool _ShowTrendLineRaysGot;
        Mq4Double ShowTrendLineRays_backfield;
        Mq4Double ShowTrendLineRays
        {
            get
            {
                if (!_ShowTrendLineRaysGot)
                    ShowTrendLineRays_backfield = ShowTrendLineRays_parameter;
                return ShowTrendLineRays_backfield;
            }
            set { ShowTrendLineRays_backfield = value; }
        }

        [Parameter("ShowTrendLinesWithInvalidPoints", DefaultValue = FALSE)]
        public bool ShowTrendLinesWithInvalidPoints_parameter { get; set; }
        bool _ShowTrendLinesWithInvalidPointsGot;
        Mq4Double ShowTrendLinesWithInvalidPoints_backfield;
        Mq4Double ShowTrendLinesWithInvalidPoints
        {
            get
            {
                if (!_ShowTrendLinesWithInvalidPointsGot)
                    ShowTrendLinesWithInvalidPoints_backfield = ShowTrendLinesWithInvalidPoints_parameter;
                return ShowTrendLinesWithInvalidPoints_backfield;
            }
            set { ShowTrendLinesWithInvalidPoints_backfield = value; }
        }

        [Parameter("ShowSRLines", DefaultValue = TRUE)]
        public bool ShowSRLines_parameter { get; set; }
        bool _ShowSRLinesGot;
        Mq4Double ShowSRLines_backfield;
        Mq4Double ShowSRLines
        {
            get
            {
                if (!_ShowSRLinesGot)
                    ShowSRLines_backfield = ShowSRLines_parameter;
                return ShowSRLines_backfield;
            }
            set { ShowSRLines_backfield = value; }
        }

        [Parameter("ShowSRLinesWithInvalidPoints", DefaultValue = FALSE)]
        public bool ShowSRLinesWithInvalidPoints_parameter { get; set; }
        bool _ShowSRLinesWithInvalidPointsGot;
        Mq4Double ShowSRLinesWithInvalidPoints_backfield;
        Mq4Double ShowSRLinesWithInvalidPoints
        {
            get
            {
                if (!_ShowSRLinesWithInvalidPointsGot)
                    ShowSRLinesWithInvalidPoints_backfield = ShowSRLinesWithInvalidPoints_parameter;
                return ShowSRLinesWithInvalidPoints_backfield;
            }
            set { ShowSRLinesWithInvalidPoints_backfield = value; }
        }

        [Parameter("ShowSRArrows", DefaultValue = FALSE)]
        public bool ShowSRArrows_parameter { get; set; }
        bool _ShowSRArrowsGot;
        Mq4Double ShowSRArrows_backfield;
        Mq4Double ShowSRArrows
        {
            get
            {
                if (!_ShowSRArrowsGot)
                    ShowSRArrows_backfield = ShowSRArrows_parameter;
                return ShowSRArrows_backfield;
            }
            set { ShowSRArrows_backfield = value; }
        }





        Mq4Double LineCount = 0;
        Mq4DoubleArray LineValid = new Mq4DoubleArray(10000);
        Mq4StringArray LineType = new Mq4StringArray(10000);
        Mq4StringArray LineName = new Mq4StringArray(10000);
        Mq4String SRMode = "HIGH";
        Mq4Double SRHighBar;
        Mq4Double SRHighDateTime;
        Mq4Double SRHigh = 0;
        Mq4Double SRLowBar;
        Mq4Double SRLowDateTime;
        Mq4Double SRLow = 9999;

        int indicator_buffers = 2;
        int indicator_color1 = Blue;
        int indicator_color2 = Blue;


        Mq4Double indicator_width1 = 1;
        Mq4Double indicator_width2 = 1;
        Mq4Double indicator_width3 = 1;
        Mq4Double indicator_width4 = 1;
        Mq4Double indicator_width5 = 1;
        Mq4Double indicator_width6 = 1;
        Mq4Double indicator_width7 = 1;
        Mq4Double indicator_width8 = 1;






        List<Mq4OutputDataSeries> AllBuffers = new List<Mq4OutputDataSeries>();
        public List<DataSeries> AllOutputDataSeries = new List<DataSeries>();

        protected override void Initialize()
        {



            CommonInitialize();

        }

        private bool _initialized;
        public override void Calculate(int index)
        {
            try
            {
                _currentIndex = index;


                if (IsLastBar)
                {
                    if (!_initialized)
                    {
                        Mq4Init();
                        _initialized = true;
                    }

                    Mq4Start();
                    _indicatorCounted = index;
                }
            } catch (Exception e)
            {

                throw;
            }
        }

        int _currentIndex;
        CachedStandardIndicators _cachedStandardIndicators;
        Mq4ChartObjects _mq4ChartObjects;
        Mq4ArrayToDataSeriesConverterFactory _mq4ArrayToDataSeriesConverterFactory;
        Mq4MarketDataSeries Open;
        Mq4MarketDataSeries High;
        Mq4MarketDataSeries Low;
        Mq4MarketDataSeries Close;
        Mq4MarketDataSeries Median;
        Mq4MarketDataSeries Volume;
        Mq4TimeSeries Time;

        private void CommonInitialize()
        {
            Open = new Mq4MarketDataSeries(MarketSeries.Open);
            High = new Mq4MarketDataSeries(MarketSeries.High);
            Low = new Mq4MarketDataSeries(MarketSeries.Low);
            Close = new Mq4MarketDataSeries(MarketSeries.Close);
            Volume = new Mq4MarketDataSeries(MarketSeries.TickVolume);
            Median = new Mq4MarketDataSeries(MarketSeries.Median);
            Time = new Mq4TimeSeries(MarketSeries.OpenTime);

            _cachedStandardIndicators = new CachedStandardIndicators(Indicators);
            _mq4ChartObjects = new Mq4ChartObjects(ChartObjects, MarketSeries.OpenTime);
            _mq4ArrayToDataSeriesConverterFactory = new Mq4ArrayToDataSeriesConverterFactory(() => CreateDataSeries());
        }
        private int Bars
        {
            get { return MarketSeries.Close.Count; }
        }

        Mq4Double Point
        {
            get
            {
                if (Symbol == null)
                    return 1E-05;

                return Symbol.TickSize;
            }
        }
        private int Period()
        {
            if (TimeFrame == TimeFrame.Minute)
                return 1;
            if (TimeFrame == TimeFrame.Minute2)
                return 2;
            if (TimeFrame == TimeFrame.Minute3)
                return 3;
            if (TimeFrame == TimeFrame.Minute4)
                return 4;
            if (TimeFrame == TimeFrame.Minute5)
                return 5;
            if (TimeFrame == TimeFrame.Minute10)
                return 10;
            if (TimeFrame == TimeFrame.Minute15)
                return 15;
            if (TimeFrame == TimeFrame.Minute30)
                return 30;
            if (TimeFrame == TimeFrame.Hour)
                return 60;
            if (TimeFrame == TimeFrame.Hour4)
                return 240;
            if (TimeFrame == TimeFrame.Hour12)
                return 720;
            if (TimeFrame == TimeFrame.Daily)
                return 1440;
            if (TimeFrame == TimeFrame.Weekly)
                return 10080;

            return 43200;
        }

        public TimeFrame PeriodToTimeFrame(int period)
        {
            switch (period)
            {
                case 0:
                    return TimeFrame;
                case 1:
                    return TimeFrame.Minute;
                case 2:
                    return TimeFrame.Minute2;
                case 3:
                    return TimeFrame.Minute3;
                case 4:
                    return TimeFrame.Minute4;
                case 5:
                    return TimeFrame.Minute5;
                case 10:
                    return TimeFrame.Minute10;
                case 15:
                    return TimeFrame.Minute15;
                case 30:
                    return TimeFrame.Minute30;
                case 60:
                    return TimeFrame.Hour;
                case 240:
                    return TimeFrame.Hour4;
                case 720:
                    return TimeFrame.Hour12;
                case 1440:
                    return TimeFrame.Daily;
                case 10080:
                    return TimeFrame.Weekly;
                case 43200:
                    return TimeFrame.Monthly;
                default:
                    throw new NotSupportedException(string.Format("TimeFrame {0} minutes isn't supported by cAlgo", period));
            }
        }



















        Mq4String DoubleToStr(double value, int digits)
        {
            return value.ToString("F" + digits);
        }






        int ToMq4ErrorCode(ErrorCode errorCode)
        {
            switch (errorCode)
            {
                case ErrorCode.BadVolume:
                    return ERR_INVALID_TRADE_VOLUME;
                case ErrorCode.NoMoney:
                    return ERR_NOT_ENOUGH_MONEY;
                case ErrorCode.MarketClosed:
                    return ERR_MARKET_CLOSED;
                case ErrorCode.Disconnected:
                    return ERR_NO_CONNECTION;
                case ErrorCode.Timeout:
                    return ERR_TRADE_TIMEOUT;
                default:
                    return ERR_COMMON_ERROR;
            }
        }








        int StringLen(Mq4String text)
        {
            return ((string)text).Length;
        }

























        const string NotSupportedMaShift = "Converter supports only ma_shift = 0";







































        private int _lastError;













        const string GlobalVariablesPath = "Software\\2calgo\\Global Variables\\";






        Symbol GetSymbol(string symbolCode)
        {
            if (symbolCode == "0" || string.IsNullOrEmpty(symbolCode))
            {
                return Symbol;
            }
            return MarketData.GetSymbol(symbolCode);
        }

        MarketSeries GetSeries(string symbol, int period)
        {
            var timeFrame = PeriodToTimeFrame(period);
            var symbolObject = GetSymbol(symbol);

            if (symbolObject == Symbol && timeFrame == TimeFrame)
                return MarketSeries;

            return MarketData.GetSeries(symbolObject.Code, timeFrame);
        }

        private DataSeries ToAppliedPrice(string symbol, int timeframe, int constant)
        {
            var series = GetSeries(symbol, timeframe);
            switch (constant)
            {
                case PRICE_OPEN:
                    return series.Open;
                case PRICE_HIGH:
                    return series.High;
                case PRICE_LOW:
                    return series.Low;
                case PRICE_CLOSE:
                    return series.Close;
                case PRICE_MEDIAN:
                    return series.Median;
                case PRICE_TYPICAL:
                    return series.Typical;
                case PRICE_WEIGHTED:
                    return series.WeightedClose;
            }
            throw new NotImplementedException("Converter doesn't support working with this type of AppliedPrice");
        }
        const string xArrow = "✖";

        public static string GetArrowByCode(int code)
        {
            switch (code)
            {
                case 0:
                    return string.Empty;
                case 32:
                    return " ";
                case 33:
                    return "✏";
                case 34:
                    return "✂";
                case 35:
                    return "✁";
                case 40:
                    return "☎";
                case 41:
                    return "✆";
                case 42:
                    return "✉";
                case 54:
                    return "⌛";
                case 55:
                    return "⌨";
                case 62:
                    return "✇";
                case 63:
                    return "✍";
                case 65:
                    return "✌";
                case 69:
                    return "☜";
                case 70:
                    return "☞";
                case 71:
                    return "☝";
                case 72:
                    return "☟";
                case 74:
                    return "☺";
                case 76:
                    return "☹";
                case 78:
                    return "☠";
                case 79:
                    return "⚐";
                case 81:
                    return "✈";
                case 82:
                    return "☼";
                case 84:
                    return "❄";
                case 86:
                    return "✞";
                case 88:
                    return "✠";
                case 89:
                    return "✡";
                case 90:
                    return "☪";
                case 91:
                    return "☯";
                case 92:
                    return "ॐ";
                case 93:
                    return "☸";
                case 94:
                    return "♈";
                case 95:
                    return "♉";
                case 96:
                    return "♊";
                case 97:
                    return "♋";
                case 98:
                    return "♌";
                case 99:
                    return "♍";
                case 100:
                    return "♎";
                case 101:
                    return "♏";
                case 102:
                    return "♐";
                case 103:
                    return "♑";
                case 104:
                    return "♒";
                case 105:
                    return "♓";
                case 106:
                    return "&";
                case 107:
                    return "&";
                case 108:
                    return "●";
                case 109:
                    return "❍";
                case 110:
                    return "■";
                case 111:
                case 112:
                    return "□";
                case 113:
                    return "❑";
                case 114:
                    return "❒";
                case 115:
                case 116:
                    return "⧫";
                case 117:
                case 119:
                    return "◆";
                case 118:
                    return "❖";
                case 120:
                    return "⌧";
                case 121:
                    return "⍓";
                case 122:
                    return "⌘";
                case 123:
                    return "❀";
                case 124:
                    return "✿";
                case 125:
                    return "❝";
                case 126:
                    return "❞";
                case 127:
                    return "▯";
                case 128:
                    return "⓪";
                case 129:
                    return "①";
                case 130:
                    return "②";
                case 131:
                    return "③";
                case 132:
                    return "④";
                case 133:
                    return "⑤";
                case 134:
                    return "⑥";
                case 135:
                    return "⑦";
                case 136:
                    return "⑧";
                case 137:
                    return "⑨";
                case 138:
                    return "⑩";
                case 139:
                    return "⓿";
                case 140:
                    return "❶";
                case 141:
                    return "❷";
                case 142:
                    return "❸";
                case 143:
                    return "❹";
                case 144:
                    return "❺";
                case 145:
                    return "❻";
                case 146:
                    return "❼";
                case 147:
                    return "❽";
                case 148:
                    return "❾";
                case 149:
                    return "❿";
                case 158:
                    return "·";
                case 159:
                    return "•";
                case 160:
                case 166:
                    return "▪";
                case 161:
                    return "○";
                case 162:
                case 164:
                    return "⭕";
                case 165:
                    return "◎";
                case 167:
                    return "✖";
                case 168:
                    return "◻";
                case 170:
                    return "✦";
                case 171:
                    return "★";
                case 172:
                    return "✶";
                case 173:
                    return "✴";
                case 174:
                    return "✹";
                case 175:
                    return "✵";
                case 177:
                    return "⌖";
                case 178:
                    return "⟡";
                case 179:
                    return "⌑";
                case 181:
                    return "✪";
                case 182:
                    return "✰";
                case 195:
                case 197:
                case 215:
                case 219:
                case 223:
                case 231:
                    return "◀";
                case 196:
                case 198:
                case 224:
                    return "▶";
                case 213:
                    return "⌫";
                case 214:
                    return "⌦";
                case 216:
                    return "➢";
                case 220:
                    return "➲";
                case 232:
                    return "➔";
                case 233:
                case 199:
                case 200:
                case 217:
                case 221:
                case 225:
                    return "◭";
                case 234:
                case 201:
                case 202:
                case 218:
                case 222:
                case 226:
                    return "⧨";
                case 239:
                    return "⇦";
                case 240:
                    return "⇨";
                case 241:
                    return "◭";
                case 242:
                    return "⧨";
                case 243:
                    return "⬄";
                case 244:
                    return "⇳";
                case 245:
                case 227:
                case 235:
                    return "↖";
                case 246:
                case 228:
                case 236:
                    return "↗";
                case 247:
                case 229:
                case 237:
                    return "↙";
                case 248:
                case 230:
                case 238:
                    return "↘";
                case 249:
                    return "▭";
                case 250:
                    return "▫";
                case 251:
                    return "✗";
                case 252:
                    return "✓";
                case 253:
                    return "☒";
                case 254:
                    return "☑";
                default:
                    return xArrow;
            }
        }
        class Mq4OutputDataSeries : IMq4DoubleArray
        {
            public IndicatorDataSeries OutputDataSeries { get; private set; }
            private readonly IndicatorDataSeries _originalValues;
            private int _currentIndex;
            private int _shift;
            private double _emptyValue = EMPTY_VALUE;
            private readonly ChartObjects _chartObjects;
            private readonly int _style;
            private readonly int _bufferIndex;
            private readonly EverTrend_Indicator _indicator;

            public Mq4OutputDataSeries(EverTrend_Indicator indicator, IndicatorDataSeries outputDataSeries, ChartObjects chartObjects, int style, int bufferIndex, Func<IndicatorDataSeries> dataSeriesFactory, int lineWidth, Colors? color = null)
            {
                OutputDataSeries = outputDataSeries;
                _chartObjects = chartObjects;
                _style = style;
                _bufferIndex = bufferIndex;
                _indicator = indicator;
                Color = color;
                _originalValues = dataSeriesFactory();
                LineWidth = lineWidth;
            }

            public int LineWidth { get; private set; }
            public Colors? Color { get; private set; }

            public int Length
            {
                get { return OutputDataSeries.Count; }
            }

            public void Resize(int newSize)
            {
            }

            public void SetCurrentIndex(int index)
            {
                _currentIndex = index;
            }

            public void SetShift(int shift)
            {
                _shift = shift;
            }

            public void SetEmptyValue(double emptyValue)
            {
                _emptyValue = emptyValue;
            }

            public Mq4Double this[int index]
            {
                get
                {
                    var indexToGetFrom = _currentIndex - index + _shift;
                    if (indexToGetFrom < 0 || indexToGetFrom > _currentIndex)
                        return 0;
                    if (indexToGetFrom >= _originalValues.Count)
                        return _emptyValue;

                    return _originalValues[indexToGetFrom];
                }
                set
                {
                    var indexToSet = _currentIndex - index + _shift;
                    if (indexToSet < 0)
                        return;

                    _originalValues[indexToSet] = value;

                    var valueToSet = value;
                    if (valueToSet == _emptyValue)
                        valueToSet = double.NaN;

                    if (indexToSet < 0)
                        return;

                    OutputDataSeries[indexToSet] = valueToSet;

                    switch (_style)
                    {
                        case DRAW_ARROW:
                            var arrowName = GetArrowName(indexToSet);
                            if (double.IsNaN(valueToSet))
                                _chartObjects.RemoveObject(arrowName);
                            else
                            {
                                var color = Color.HasValue ? Color.Value : Colors.Red;
                                _chartObjects.DrawText(arrowName, _indicator.ArrowByIndex[_bufferIndex], indexToSet, valueToSet, VerticalAlignment.Center, HorizontalAlignment.Center, color);
                            }
                            break;
                        case DRAW_HISTOGRAM:
                            if (true)
                            {
                                var anotherLine = _indicator.AllBuffers.FirstOrDefault(b => b.LineWidth == LineWidth && b != this);
                                if (anotherLine != null)
                                {
                                    var name = GetNameOfHistogramLineOnChartWindow(indexToSet);
                                    Colors color;
                                    if (this[index] > anotherLine[index])
                                        color = Color ?? Colors.Green;
                                    else
                                        color = anotherLine.Color ?? Colors.Green;
                                    var lineWidth = LineWidth;
                                    if (lineWidth != 1 && lineWidth < 5)
                                        lineWidth = 5;

                                    _chartObjects.DrawLine(name, indexToSet, this[index], indexToSet, anotherLine[index], color, lineWidth);
                                }
                            }
                            break;
                    }
                }
            }

            private string GetNameOfHistogramLineOnChartWindow(int index)
            {
                return string.Format("Histogram on chart window {0} {1}", LineWidth, index);
            }

            private string GetArrowName(int index)
            {
                return string.Format("Arrow {0} {1}", GetHashCode(), index);
            }
        }
















        public Dictionary<int, string> ArrowByIndex = new Dictionary<int, string> 
        {
            {
                0,
                xArrow
            },
            {
                1,
                xArrow
            },
            {
                2,
                xArrow
            },
            {
                3,
                xArrow
            },
            {
                4,
                xArrow
            },
            {
                5,
                xArrow
            },
            {
                6,
                xArrow
            },
            {
                7,
                xArrow
            }
        };
        void SetIndexArrow(int index, int code)
        {
            ArrowByIndex[index] = GetArrowByCode(code);
        }

        private int _indicatorCounted;
        private int IndicatorCounted()
        {
            return _indicatorCounted;
        }
        int FILE_READ = 1;
        int FILE_WRITE = 2;
//int FILE_BIN = 8;
        int FILE_CSV = 8;

        int SEEK_END = 2;

        class FileInfo
        {
            public int Mode { get; set; }
            public int Handle { get; set; }
            public char Separator { get; set; }
            public string FileName { get; set; }
            public List<string> PendingParts { get; set; }
            public StreamWriter StreamWriter { get; set; }
            public StreamReader StreamReader { get; set; }
        }

        private Dictionary<int, FileInfo> _openedFiles = new Dictionary<int, FileInfo>();
        private int _handleCounter = 1000;







        class FolderPaths
        {
            public static string _2calgoAppDataFolder
            {
                get
                {
                    var result = Path.Combine(SystemAppData, "2calgo");
                    if (!Directory.Exists(result))
                        Directory.CreateDirectory(result);
                    return result;
                }
            }

            public static string _2calgoDesktopFolder
            {
                get
                {
                    var result = Path.Combine(Desktop, "2calgo");
                    if (!Directory.Exists(result))
                        Directory.CreateDirectory(result);
                    return result;
                }
            }

            static string SystemAppData
            {
                get { return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData); }
            }

            static string Desktop
            {
                get { return Environment.GetFolderPath(Environment.SpecialFolder.Desktop); }
            }
        }
        const int MODE_TRADES = 0;
        const int MODE_HISTORY = 1;
        const int SELECT_BY_POS = 0;
        const int SELECT_BY_TICKET = 1;

        T GetPropertyValue<T>(Func<Position, T> getFromPosition, Func<PendingOrder, T> getFromPendingOrder, Func<HistoricalTrade, T> getFromHistory)
        {
            if (_currentOrder == null)
                return default(T);

            return GetPropertyValue<T>(_currentOrder, getFromPosition, getFromPendingOrder, getFromHistory);
        }

        T GetPropertyValue<T>(object obj, Func<Position, T> getFromPosition, Func<PendingOrder, T> getFromPendingOrder, Func<HistoricalTrade, T> getFromHistory)
        {
            if (obj is Position)
                return getFromPosition((Position)obj);
            if (obj is PendingOrder)
                return getFromPendingOrder((PendingOrder)obj);

            return getFromHistory((HistoricalTrade)obj);
        }

        private Mq4Double GetTicket(object trade)
        {
            return new Mq4Double(GetPropertyValue<int>(trade, _ => _.Id, _ => _.Id, _ => _.ClosingDealId));
        }
        private int GetMagicNumber(string label)
        {
            int magicNumber;
            if (int.TryParse(label, out magicNumber))
                return magicNumber;

            return 0;
        }

        private int GetMagicNumber(object order)
        {
            var label = GetPropertyValue<string>(order, _ => _.Label, _ => _.Label, _ => _.Label);
            return GetMagicNumber(label);
        }




        object _currentOrder;
        double GetLots(object order)
        {
            var volume = GetPropertyValue<long>(order, _ => _.Volume, _ => _.Volume, _ => _.Volume);
            var symbolCode = GetPropertyValue<string>(order, _ => _.SymbolCode, _ => _.SymbolCode, _ => _.SymbolCode);
            var symbolObject = MarketData.GetSymbol(symbolCode);

            return symbolObject.ToLotsVolume(volume);
        }

        object GetOrderByTicket(int ticket)
        {
            var allOrders = Positions.OfType<object>().Concat(PendingOrders.OfType<object>()).ToArray();

            return allOrders.FirstOrDefault(_ => GetTicket(_) == ticket);
        }


        double GetOpenPrice(object order)
        {
            return GetPropertyValue<double>(order, _ => _.EntryPrice, _ => _.TargetPrice, _ => _.EntryPrice);
        }


        private double GetStopLoss(object order)
        {
            var nullableValue = GetPropertyValue<double?>(order, _ => _.StopLoss, _ => _.StopLoss, _ => 0);
            return nullableValue ?? 0;
        }

        private double GetTakeProfit(object order)
        {
            var nullableValue = GetPropertyValue<double?>(order, _ => _.TakeProfit, _ => _.TakeProfit, _ => 0);
            return nullableValue ?? 0;
        }























        class ParametersKey
        {
            private readonly object[] _parameters;

            public ParametersKey(params object[] parameters)
            {
                _parameters = parameters;
            }

            public override bool Equals(object obj)
            {
                var other = (ParametersKey)obj;
                for (var i = 0; i < _parameters.Length; i++)
                {
                    if (!_parameters[i].Equals(other._parameters[i]))
                        return false;
                }
                return true;
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    var hashCode = 0;
                    foreach (var parameter in _parameters)
                    {
                        hashCode = (hashCode * 397) ^ parameter.GetHashCode();
                    }
                    return hashCode;
                }
            }
        }

        class Cache<TValue>
        {
            private Dictionary<ParametersKey, TValue> _dictionary = new Dictionary<ParametersKey, TValue>();

            public bool TryGetValue(out TValue value, params object[] parameters)
            {
                var key = new ParametersKey(parameters);
                return _dictionary.TryGetValue(key, out value);
            }

            public void Add(TValue value, params object[] parameters)
            {
                var key = new ParametersKey(parameters);
                _dictionary.Add(key, value);
            }
        }


        private static MovingAverageType ToMaType(int constant)
        {
            switch (constant)
            {
                case MODE_SMA:
                    return MovingAverageType.Simple;
                case MODE_EMA:
                    return MovingAverageType.Exponential;
                case MODE_LWMA:
                    return MovingAverageType.Weighted;
                default:
                    throw new ArgumentOutOfRangeException("Not supported moving average type");
            }
        }


























        class CachedStandardIndicators
        {
            private readonly IIndicatorsAccessor _indicatorsAccessor;

            public CachedStandardIndicators(IIndicatorsAccessor indicatorsAccessor)
            {
                _indicatorsAccessor = indicatorsAccessor;
            }

        }
        const bool True = true;
        const bool False = false;
        const bool TRUE = true;
        const bool FALSE = false;
        Mq4Null NULL;
        const int EMPTY = -1;
        const double EMPTY_VALUE = 2147483647;
        public const int WHOLE_ARRAY = 0;

        const int MODE_SMA = 0;
        //Simple moving average
        const int MODE_EMA = 1;
        //Exponential moving average,
        const int MODE_SMMA = 2;
        //Smoothed moving average,
        const int MODE_LWMA = 3;
        //Linear weighted moving average. 
        const int PRICE_CLOSE = 0;
        //Close price. 
        const int PRICE_OPEN = 1;
        //Open price. 
        const int PRICE_HIGH = 2;
        //High price. 
        const int PRICE_LOW = 3;
        //Low price. 
        const int PRICE_MEDIAN = 4;
        //Median price, (high+low)/2. 
        const int PRICE_TYPICAL = 5;
        //Typical price, (high+low+close)/3. 
        const int PRICE_WEIGHTED = 6;
        //Weighted close price, (high+low+close+close)/4. 
        const int DRAW_LINE = 0;
        const int DRAW_SECTION = 1;
        const int DRAW_HISTOGRAM = 2;
        const int DRAW_ARROW = 3;
        const int DRAW_ZIGZAG = 4;
        const int DRAW_NONE = 12;

        const int STYLE_SOLID = 0;
        const int STYLE_DASH = 1;
        const int STYLE_DOT = 2;
        const int STYLE_DASHDOT = 3;
        const int STYLE_DASHDOTDOT = 4;

        const int MODE_OPEN = 0;
        const int MODE_LOW = 1;
        const int MODE_HIGH = 2;
        const int MODE_CLOSE = 3;
        const int MODE_VOLUME = 4;
        const int MODE_TIME = 5;
        const int MODE_BID = 9;
        const int MODE_ASK = 10;
        const int MODE_POINT = 11;
        const int MODE_DIGITS = 12;
        const int MODE_SPREAD = 13;
        const int MODE_TRADEALLOWED = 22;
        const int MODE_PROFITCALCMODE = 27;
        const int MODE_MARGINCALCMODE = 28;
        const int MODE_SWAPTYPE = 26;
        const int MODE_TICKSIZE = 17;
        const int MODE_FREEZELEVEL = 33;
        const int MODE_STOPLEVEL = 14;
        const int MODE_LOTSIZE = 15;
        const int MODE_TICKVALUE = 16;
        /*const int MODE_SWAPLONG = 18;
const int MODE_SWAPSHORT = 19;
const int MODE_STARTING = 20;
const int MODE_EXPIRATION = 21;    
*/
        const int MODE_MINLOT = 23;
        const int MODE_LOTSTEP = 24;
        const int MODE_MAXLOT = 25;
        /*const int MODE_MARGININIT = 29;
const int MODE_MARGINMAINTENANCE = 30;
const int MODE_MARGINHEDGED = 31;*/
        const int MODE_MARGINREQUIRED = 32;

        const int OBJ_VLINE = 0;
        const int OBJ_HLINE = 1;
        const int OBJ_TREND = 2;
        const int OBJ_FIBO = 10;

        /*const int OBJ_TRENDBYANGLE = 3;
    const int OBJ_REGRESSION = 4;
    const int OBJ_CHANNEL = 5;
    const int OBJ_STDDEVCHANNEL = 6;
    const int OBJ_GANNLINE = 7;
    const int OBJ_GANNFAN = 8;
    const int OBJ_GANNGRID = 9;
    const int OBJ_FIBOTIMES = 11;
    const int OBJ_FIBOFAN = 12;
    const int OBJ_FIBOARC = 13;
    const int OBJ_EXPANSION = 14;
    const int OBJ_FIBOCHANNEL = 15;*/
        const int OBJ_RECTANGLE = 16;
        /*const int OBJ_TRIANGLE = 17;
    const int OBJ_ELLIPSE = 18;
    const int OBJ_PITCHFORK = 19;
    const int OBJ_CYCLES = 20;*/
        const int OBJ_TEXT = 21;
        const int OBJ_ARROW = 22;
        const int OBJ_LABEL = 23;

        const int OBJPROP_TIME1 = 0;
        const int OBJPROP_PRICE1 = 1;
        const int OBJPROP_TIME2 = 2;
        const int OBJPROP_PRICE2 = 3;
        const int OBJPROP_TIME3 = 4;
        const int OBJPROP_PRICE3 = 5;
        const int OBJPROP_COLOR = 6;
        const int OBJPROP_STYLE = 7;
        const int OBJPROP_WIDTH = 8;
        const int OBJPROP_BACK = 9;
        const int OBJPROP_RAY = 10;
        const int OBJPROP_ELLIPSE = 11;
        //const int OBJPROP_SCALE = 12;
        const int OBJPROP_ANGLE = 13;
        //angle for text rotation
        const int OBJPROP_ARROWCODE = 14;
        const int OBJPROP_TIMEFRAMES = 15;
        //const int OBJPROP_DEVIATION = 16;
        const int OBJPROP_FONTSIZE = 100;
        const int OBJPROP_CORNER = 101;
        const int OBJPROP_XDISTANCE = 102;
        const int OBJPROP_YDISTANCE = 103;
        const int OBJPROP_FIBOLEVELS = 200;
        const int OBJPROP_LEVELCOLOR = 201;
        const int OBJPROP_LEVELSTYLE = 202;
        const int OBJPROP_LEVELWIDTH = 203;
        const int OBJPROP_FIRSTLEVEL = 210;

        const int PERIOD_M1 = 1;
        const int PERIOD_M5 = 5;
        const int PERIOD_M15 = 15;
        const int PERIOD_M30 = 30;
        const int PERIOD_H1 = 60;
        const int PERIOD_H4 = 240;
        const int PERIOD_D1 = 1440;
        const int PERIOD_W1 = 10080;
        const int PERIOD_MN1 = 43200;

        const int TIME_DATE = 1;
        const int TIME_MINUTES = 2;
        const int TIME_SECONDS = 4;

        const int MODE_MAIN = 0;
        const int MODE_PLUSDI = 1;
        const int MODE_MINUSDI = 2;
        const int MODE_SIGNAL = 1;

        const int MODE_UPPER = 1;
        const int MODE_LOWER = 2;

        const int MODE_GATORLIPS = 3;
        const int MODE_GATORJAW = 1;
        const int MODE_GATORTEETH = 2;

        const int CLR_NONE = 32768;

        const int White = 16777215;
        const int Snow = 16448255;
        const int MintCream = 16449525;
        const int LavenderBlush = 16118015;
        const int AliceBlue = 16775408;
        const int Honeydew = 15794160;
        const int Ivory = 15794175;
        const int Seashell = 15660543;
        const int WhiteSmoke = 16119285;
        const int OldLace = 15136253;
        const int MistyRose = 14804223;
        const int Lavender = 16443110;
        const int Linen = 15134970;
        const int LightCyan = 16777184;
        const int LightYellow = 14745599;
        const int Cornsilk = 14481663;
        const int PapayaWhip = 14020607;
        const int AntiqueWhite = 14150650;
        const int Beige = 14480885;
        const int LemonChiffon = 13499135;
        const int BlanchedAlmond = 13495295;
        const int LightGoldenrod = 13826810;
        const int Bisque = 12903679;
        const int Pink = 13353215;
        const int PeachPuff = 12180223;
        const int Gainsboro = 14474460;
        const int LightPink = 12695295;
        const int Moccasin = 11920639;
        const int NavajoWhite = 11394815;
        const int Wheat = 11788021;
        const int LightGray = 13882323;
        const int PaleTurquoise = 15658671;
        const int PaleGoldenrod = 11200750;
        const int PowderBlue = 15130800;
        const int Thistle = 14204888;
        const int PaleGreen = 10025880;
        const int LightBlue = 15128749;
        const int LightSteelBlue = 14599344;
        const int LightSkyBlue = 16436871;
        const int Silver = 12632256;
        const int Aquamarine = 13959039;
        const int LightGreen = 9498256;
        const int Khaki = 9234160;
        const int Plum = 14524637;
        const int LightSalmon = 8036607;
        const int SkyBlue = 15453831;
        const int LightCoral = 8421616;
        const int Violet = 15631086;
        const int Salmon = 7504122;
        const int HotPink = 11823615;
        const int BurlyWood = 8894686;
        const int DarkSalmon = 8034025;
        const int Tan = 9221330;
        const int MediumSlateBlue = 15624315;
        const int SandyBrown = 6333684;
        const int DarkGray = 11119017;
        const int CornflowerBlue = 15570276;
        const int Coral = 5275647;
        const int PaleVioletRed = 9662683;
        const int MediumPurple = 14381203;
        const int Orchid = 14053594;
        const int RosyBrown = 9408444;
        const int Tomato = 4678655;
        const int DarkSeaGreen = 9419919;
        const int Cyan = 16776960;
        const int MediumAquamarine = 11193702;
        const int GreenYellow = 3145645;
        const int MediumOrchid = 13850042;
        const int IndianRed = 6053069;
        const int DarkKhaki = 7059389;
        const int SlateBlue = 13458026;
        const int RoyalBlue = 14772545;
        const int Turquoise = 13688896;
        const int DodgerBlue = 16748574;
        const int MediumTurquoise = 13422920;
        const int DeepPink = 9639167;
        const int LightSlateGray = 10061943;
        const int BlueViolet = 14822282;
        const int Peru = 4163021;
        const int SlateGray = 9470064;
        const int Gray = 8421504;
        const int Red = 255;
        const int Magenta = 16711935;
        const int Blue = 16711680;
        const int DeepSkyBlue = 16760576;
        const int Aqua = 16776960;
        const int SpringGreen = 8388352;
        const int Lime = 65280;
        const int Chartreuse = 65407;
        const int Yellow = 65535;
        const int Gold = 55295;
        const int Orange = 42495;
        const int DarkOrange = 36095;
        const int OrangeRed = 17919;
        const int LimeGreen = 3329330;
        const int YellowGreen = 3329434;
        const int DarkOrchid = 13382297;
        const int CadetBlue = 10526303;
        const int LawnGreen = 64636;
        const int MediumSpringGreen = 10156544;
        const int Goldenrod = 2139610;
        const int SteelBlue = 11829830;
        const int Crimson = 3937500;
        const int Chocolate = 1993170;
        const int MediumSeaGreen = 7451452;
        const int MediumVioletRed = 8721863;
        const int FireBrick = 2237106;
        const int DarkViolet = 13828244;
        const int LightSeaGreen = 11186720;
        const int DimGray = 6908265;
        const int DarkTurquoise = 13749760;
        const int Brown = 2763429;
        const int MediumBlue = 13434880;
        const int Sienna = 2970272;
        const int DarkSlateBlue = 9125192;
        const int DarkGoldenrod = 755384;
        const int SeaGreen = 5737262;
        const int OliveDrab = 2330219;
        const int ForestGreen = 2263842;
        const int SaddleBrown = 1262987;
        const int DarkOliveGreen = 3107669;
        const int DarkBlue = 9109504;
        const int MidnightBlue = 7346457;
        const int Indigo = 8519755;
        const int Maroon = 128;
        const int Purple = 8388736;
        const int Navy = 8388608;
        const int Teal = 8421376;
        const int Green = 32768;
        const int Olive = 32896;
        const int DarkSlateGray = 5197615;
        const int DarkGreen = 25600;
        const int Fuchsia = 16711935;
        const int Black = 0;

        const int SYMBOL_LEFTPRICE = 5;
        const int SYMBOL_RIGHTPRICE = 6;

        const int SYMBOL_ARROWUP = 241;
        const int SYMBOL_ARROWDOWN = 242;
        const int SYMBOL_STOPSIGN = 251;
        /*
const int SYMBOL_THUMBSUP = 67;
const int SYMBOL_THUMBSDOWN = 68;	
const int SYMBOL_CHECKSIGN = 25;
*/

        public const int MODE_ASCEND = 1;
        public const int MODE_DESCEND = 2;

        const int MODE_TENKANSEN = 1;
        const int MODE_KIJUNSEN = 2;
        const int MODE_SENKOUSPANA = 3;
        const int MODE_SENKOUSPANB = 4;
        const int MODE_CHINKOUSPAN = 5;
        const int OP_BUY = 0;
        const int OP_SELL = 1;
        const int OP_BUYLIMIT = 2;
        const int OP_SELLLIMIT = 3;
        const int OP_BUYSTOP = 4;
        const int OP_SELLSTOP = 5;
        const int OBJ_PERIOD_M1 = 0x1;
        const int OBJ_PERIOD_M5 = 0x2;
        const int OBJ_PERIOD_M15 = 0x4;
        const int OBJ_PERIOD_M30 = 0x8;
        const int OBJ_PERIOD_H1 = 0x10;
        const int OBJ_PERIOD_H4 = 0x20;
        const int OBJ_PERIOD_D1 = 0x40;
        const int OBJ_PERIOD_W1 = 0x80;
        const int OBJ_PERIOD_MN1 = 0x100;
        const int OBJ_ALL_PERIODS = 0x1ff;

        const int REASON_REMOVE = 1;
        const int REASON_RECOMPILE = 2;
        const int REASON_CHARTCHANGE = 3;
        const int REASON_CHARTCLOSE = 4;
        const int REASON_PARAMETERS = 5;
        const int REASON_ACCOUNT = 6;
        const int ERR_NO_ERROR = 0;
        const int ERR_NO_RESULT = 1;
        const int ERR_COMMON_ERROR = 2;
        const int ERR_INVALID_TRADE_PARAMETERS = 3;
        const int ERR_SERVER_BUSY = 4;
        const int ERR_OLD_VERSION = 5;
        const int ERR_NO_CONNECTION = 6;
        const int ERR_NOT_ENOUGH_RIGHTS = 7;
        const int ERR_TOO_FREQUENT_REQUESTS = 8;
        const int ERR_MALFUNCTIONAL_TRADE = 9;
        const int ERR_ACCOUNT_DISABLED = 64;
        const int ERR_INVALID_ACCOUNT = 65;
        const int ERR_TRADE_TIMEOUT = 128;
        const int ERR_INVALID_PRICE = 129;
        const int ERR_INVALID_STOPS = 130;
        const int ERR_INVALID_TRADE_VOLUME = 131;
        const int ERR_MARKET_CLOSED = 132;
        const int ERR_TRADE_DISABLED = 133;
        const int ERR_NOT_ENOUGH_MONEY = 134;
        const int ERR_PRICE_CHANGED = 135;
        const int ERR_OFF_QUOTES = 136;
        const int ERR_BROKER_BUSY = 137;
        const int ERR_REQUOTE = 138;
        const int ERR_ORDER_LOCKED = 139;
        const int ERR_LONG_POSITIONS_ONLY_ALLOWED = 140;
        const int ERR_TOO_MANY_REQUESTS = 141;
        const int ERR_TRADE_MODIFY_DENIED = 145;
        const int ERR_TRADE_CONTEXT_BUSY = 146;
        const int ERR_TRADE_EXPIRATION_DENIED = 147;
        const int ERR_TRADE_TOO_MANY_ORDERS = 148;
        const int ERR_TRADE_HEDGE_PROHIBITED = 149;
        const int ERR_TRADE_PROHIBITED_BY_FIFO = 150;
        const int ERR_NO_MQLERROR = 4000;
        const int ERR_WRONG_FUNCTION_POINTER = 4001;
        const int ERR_ARRAY_INDEX_OUT_OF_RANGE = 4002;
        const int ERR_NO_MEMORY_FOR_CALL_STACK = 4003;
        const int ERR_RECURSIVE_STACK_OVERFLOW = 4004;
        const int ERR_NOT_ENOUGH_STACK_FOR_PARAM = 4005;
        const int ERR_NO_MEMORY_FOR_PARAM_STRING = 4006;
        const int ERR_NO_MEMORY_FOR_TEMP_STRING = 4007;
        const int ERR_NOT_INITIALIZED_STRING = 4008;
        const int ERR_NOT_INITIALIZED_ARRAYSTRING = 4009;
        const int ERR_NO_MEMORY_FOR_ARRAYSTRING = 4010;
        const int ERR_TOO_LONG_STRING = 4011;
        const int ERR_REMAINDER_FROM_ZERO_DIVIDE = 4012;
        const int ERR_ZERO_DIVIDE = 4013;
        const int ERR_UNKNOWN_COMMAND = 4014;
        const int ERR_WRONG_JUMP = 4015;
        const int ERR_NOT_INITIALIZED_ARRAY = 4016;
        const int ERR_DLL_CALLS_NOT_ALLOWED = 4017;
        const int ERR_CANNOT_LOAD_LIBRARY = 4018;
        const int ERR_CANNOT_CALL_FUNCTION = 4019;
        const int ERR_EXTERNAL_CALLS_NOT_ALLOWED = 4020;
        const int ERR_NO_MEMORY_FOR_RETURNED_STR = 4021;
        const int ERR_SYSTEM_BUSY = 4022;
        const int ERR_INVALID_FUNCTION_PARAMSCNT = 4050;
        const int ERR_INVALID_FUNCTION_PARAMVALUE = 4051;
        const int ERR_STRING_FUNCTION_INTERNAL = 4052;
        const int ERR_SOME_ARRAY_ERROR = 4053;
        const int ERR_INCORRECT_SERIESARRAY_USING = 4054;
        const int ERR_CUSTOM_INDICATOR_ERROR = 4055;
        const int ERR_INCOMPATIBLE_ARRAYS = 4056;
        const int ERR_GLOBAL_VARIABLES_PROCESSING = 4057;
        const int ERR_GLOBAL_VARIABLE_NOT_FOUND = 4058;
        const int ERR_FUNC_NOT_ALLOWED_IN_TESTING = 4059;
        const int ERR_FUNCTION_NOT_CONFIRMED = 4060;
        const int ERR_SEND_MAIL_ERROR = 4061;
        const int ERR_STRING_PARAMETER_EXPECTED = 4062;
        const int ERR_INTEGER_PARAMETER_EXPECTED = 4063;
        const int ERR_DOUBLE_PARAMETER_EXPECTED = 4064;
        const int ERR_ARRAY_AS_PARAMETER_EXPECTED = 4065;
        const int ERR_HISTORY_WILL_UPDATED = 4066;
        const int ERR_TRADE_ERROR = 4067;
        const int ERR_END_OF_FILE = 4099;
        const int ERR_SOME_FILE_ERROR = 4100;
        const int ERR_WRONG_FILE_NAME = 4101;
        const int ERR_TOO_MANY_OPENED_FILES = 4102;
        const int ERR_CANNOT_OPEN_FILE = 4103;
        const int ERR_INCOMPATIBLE_FILEACCESS = 4104;
        const int ERR_NO_ORDER_SELECTED = 4105;
        const int ERR_UNKNOWN_SYMBOL = 4106;
        const int ERR_INVALID_PRICE_PARAM = 4107;
        const int ERR_INVALID_TICKET = 4108;
        const int ERR_TRADE_NOT_ALLOWED = 4109;
        const int ERR_LONGS_NOT_ALLOWED = 4110;
        const int ERR_SHORTS_NOT_ALLOWED = 4111;
        const int ERR_OBJECT_ALREADY_EXISTS = 4200;
        const int ERR_UNKNOWN_OBJECT_PROPERTY = 4201;
        const int ERR_OBJECT_DOES_NOT_EXIST = 4202;
        const int ERR_UNKNOWN_OBJECT_TYPE = 4203;
        const int ERR_NO_OBJECT_NAME = 4204;
        const int ERR_OBJECT_COORDINATES_ERROR = 4205;
        const int ERR_NO_SPECIFIED_SUBWINDOW = 4206;
        const int ERR_SOME_OBJECT_ERROR = 4207;
        class Mq4ChartObjects
        {
            private readonly ChartObjects _algoChartObjects;
            private readonly TimeSeries _timeSeries;

            private readonly Dictionary<string, Mq4Object> _mq4ObjectByName = new Dictionary<string, Mq4Object>();
            private readonly List<string> _mq4ObjectNameByIndex = new List<string>();

            public Mq4ChartObjects(ChartObjects chartObjects, TimeSeries timeSeries)
            {
                _algoChartObjects = chartObjects;
                _timeSeries = timeSeries;
            }

            public void Create(string name, int type, int window, int time1, double price1, int time2, double price2, int time3, double price3)
            {
                Mq4Object mq4Object = null;
                switch (type)
                {




//{
                    case OBJ_TREND:
                        mq4Object = new Mq4TrendLine(name, type, _algoChartObjects, _timeSeries);
                        break;
                    //}


//{
                    case OBJ_ARROW:
                        mq4Object = new Mq4Arrow(name, type, _algoChartObjects, _timeSeries);
                        break;
                    //}

                }
                if (mq4Object == null)
                    return;

                _algoChartObjects.RemoveObject(name);
                if (_mq4ObjectByName.ContainsKey(name))
                {
                    _mq4ObjectByName.Remove(name);
                    _mq4ObjectNameByIndex.Remove(name);
                }
                _mq4ObjectByName[name] = mq4Object;

                mq4Object.Set(OBJPROP_TIME1, time1);
                mq4Object.Set(OBJPROP_TIME2, time2);
                mq4Object.Set(OBJPROP_TIME3, time3);
                mq4Object.Set(OBJPROP_PRICE1, price1);
                mq4Object.Set(OBJPROP_PRICE2, price2);
                mq4Object.Set(OBJPROP_PRICE3, price3);

                mq4Object.Draw();
            }
            public void Set(string name, int index, Mq4Double value)
            {
                if (!_mq4ObjectByName.ContainsKey(name))
                    return;
                _mq4ObjectByName[name].Set(index, value);
                _mq4ObjectByName[name].Draw();
            }
            public Mq4Double Get(string name, int index)
            {
                if (!_mq4ObjectByName.ContainsKey(name))
                    return 0;

                return _mq4ObjectByName[name].Get(index);
            }
            public void SetText(string name, string text, int font_size, string font, int color)
            {
                if (!_mq4ObjectByName.ContainsKey(name))
                    return;

                Set(name, OBJPROP_COLOR, color);
            }
            public void Delete(string name)
            {
                Mq4Object mq4Object;
                if (!_mq4ObjectByName.TryGetValue(name, out mq4Object))
                    return;

                mq4Object.Dispose();
                _mq4ObjectByName.Remove(name);
                _mq4ObjectNameByIndex.Remove(name);
            }

            public int DeleteAll(int type)
            {
                if (type == EMPTY)
                {
                    var count = _mq4ObjectByName.Count;
                    _algoChartObjects.RemoveAllObjects();
                    _mq4ObjectByName.Clear();
                    _mq4ObjectNameByIndex.Clear();
                    return count;
                }
                var deletedCount = 0;
                foreach (var mq4Object in _mq4ObjectByName.Select(kvp => kvp.Value).ToArray())
                {
                    if (mq4Object.Type == type)
                    {
                        Delete(mq4Object.Name);
                        deletedCount++;
                    }
                }
                return deletedCount;
            }






            private T GetObject<T>(string name) where T : Mq4Object
            {
                Mq4Object mq4Object;
                if (!_mq4ObjectByName.TryGetValue(name, out mq4Object))
                    return null;
                return mq4Object as T;
            }

        }

        abstract class Mq4Object : IDisposable
        {
            private readonly ChartObjects _chartObjects;

            protected Mq4Object(string name, int type, ChartObjects chartObjects)
            {
                Name = name;
                Type = type;
                _chartObjects = chartObjects;
            }

            public int Type { get; private set; }

            public string Name { get; private set; }

            protected DateTime Time1
            {
                get
                {
                    int seconds = Get(OBJPROP_TIME1);
                    return Mq4TimeSeries.ToDateTime(seconds);
                }
            }

            protected double Price1
            {
                get { return Get(OBJPROP_PRICE1); }
            }

            protected DateTime Time2
            {
                get
                {
                    int seconds = Get(OBJPROP_TIME2);
                    return Mq4TimeSeries.ToDateTime(seconds);
                }
            }

            protected double Price2
            {
                get { return Get(OBJPROP_PRICE2); }
            }

            protected Colors Color
            {
                get
                {
                    int intColor = Get(OBJPROP_COLOR);
                    if (intColor != CLR_NONE)
                        return Mq4Colors.GetColorByInteger(intColor);

                    return Colors.Yellow;
                }
            }

            protected int Width
            {
                get { return Get(OBJPROP_WIDTH); }
            }

            protected int Style
            {
                get { return Get(OBJPROP_STYLE); }
            }

            public abstract void Draw();

            private readonly Dictionary<int, Mq4Double> _properties = new Dictionary<int, Mq4Double> 
            {
                {
                    OBJPROP_WIDTH,
                    new Mq4Double(1)
                },
                {
                    OBJPROP_COLOR,
                    new Mq4Double(CLR_NONE)
                },
                {
                    OBJPROP_RAY,
                    new Mq4Double(1)
                },

                {
                    OBJPROP_LEVELCOLOR,
                    new Mq4Double(CLR_NONE)
                },
                {
                    OBJPROP_LEVELSTYLE,
                    new Mq4Double(0)
                },
                {
                    OBJPROP_LEVELWIDTH,
                    new Mq4Double(1)
                },
                {
                    OBJPROP_FIBOLEVELS,
                    new Mq4Double(9)
                },
                {
                    OBJPROP_FIRSTLEVEL + 0,
                    new Mq4Double(0)
                },
                {
                    OBJPROP_FIRSTLEVEL + 1,
                    new Mq4Double(0.236)
                },
                {
                    OBJPROP_FIRSTLEVEL + 2,
                    new Mq4Double(0.382)
                },
                {
                    OBJPROP_FIRSTLEVEL + 3,
                    new Mq4Double(0.5)
                },
                {
                    OBJPROP_FIRSTLEVEL + 4,
                    new Mq4Double(0.618)
                },
                {
                    OBJPROP_FIRSTLEVEL + 5,
                    new Mq4Double(1)
                },
                {
                    OBJPROP_FIRSTLEVEL + 6,
                    new Mq4Double(1.618)
                },
                {
                    OBJPROP_FIRSTLEVEL + 7,
                    new Mq4Double(2.618)
                },
                {
                    OBJPROP_FIRSTLEVEL + 8,
                    new Mq4Double(4.236)
                }
            };

            public virtual void Set(int index, Mq4Double value)
            {
                _properties[index] = value;
            }

            public Mq4Double Get(int index)
            {
                return _properties.ContainsKey(index) ? _properties[index] : new Mq4Double(0);
            }

            private readonly List<string> _addedAlgoChartObjects = new List<string>();

            protected void DrawText(string objectName, string text, int index, double yValue, VerticalAlignment verticalAlignment = VerticalAlignment.Center, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Center, Colors? color = null)
            {
                _addedAlgoChartObjects.Add(objectName);
                _chartObjects.DrawText(objectName, text, index, yValue, verticalAlignment, horizontalAlignment, color);
            }

            protected void DrawText(string objectName, string text, StaticPosition position, Colors? color = null)
            {
                _addedAlgoChartObjects.Add(objectName);
                _chartObjects.DrawText(objectName, text, position, color);
            }

            protected void DrawLine(string objectName, int index1, double y1, int index2, double y2, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
            {
                _addedAlgoChartObjects.Add(objectName);
                _chartObjects.DrawLine(objectName, index1, y1, index2, y2, color, thickness, style);
            }

            protected void DrawLine(string objectName, DateTime date1, double y1, DateTime date2, double y2, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
            {
                _addedAlgoChartObjects.Add(objectName);
                _chartObjects.DrawLine(objectName, date1, y1, date2, y2, color, thickness, style);
            }

            protected void DrawVerticalLine(string objectName, DateTime date, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
            {
                _addedAlgoChartObjects.Add(objectName);
                _chartObjects.DrawVerticalLine(objectName, date, color, thickness, style);
            }

            protected void DrawVerticalLine(string objectName, int index, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
            {
                _addedAlgoChartObjects.Add(objectName);
                _chartObjects.DrawVerticalLine(objectName, index, color, thickness, style);
            }

            protected void DrawHorizontalLine(string objectName, double y, Colors color, double thickness = 1.0, cAlgo.API.LineStyle style = cAlgo.API.LineStyle.Solid)
            {
                _addedAlgoChartObjects.Add(objectName);
                _chartObjects.DrawHorizontalLine(objectName, y, color, thickness, style);
            }

            public void Dispose()
            {
                foreach (var name in _addedAlgoChartObjects)
                {
                    _chartObjects.RemoveObject(name);
                }
            }
        }




        class Mq4TrendLine : Mq4Object
        {
            private readonly TimeSeries _timeSeries;

            public Mq4TrendLine(string name, int type, ChartObjects chartObjects, TimeSeries timeSeries) : base(name, type, chartObjects)
            {
                _timeSeries = timeSeries;
            }

            public override void Draw()
            {
                bool isRay = Get(OBJPROP_RAY);
                if (!isRay)
                {
                    DrawLine(Name, Time1, Price1, Time2, Price2, Color, Width, Mq4LineStyles.ToLineStyle(Style));
                }
                else
                {
                    const int indexDistance = 1000;
                    var index1 = _timeSeries.GetIndexByTime(Time1);
                    var index2 = _timeSeries.GetIndexByTime(Time2);
                    if (index1 < index2)
                    {
                        var index3 = index1 + indexDistance;
                        var price3 = (-1 * index1 * Price1 + index2 * Price1 - indexDistance * Price1 + indexDistance * Price2) / (index2 - index1);

                        DrawLine(Name, index1, Price1, index3, price3, Color, Width, Mq4LineStyles.ToLineStyle(Style));
                    }
                    if (index2 < index1)
                    {
                        var index3 = index2 - indexDistance;
                        var price3 = (indexDistance * Price1 - index1 * Price2 + index2 * Price2 - indexDistance * Price2) / (index2 - index1);

                        DrawLine(Name, index1, Price1, index3, price3, Color, Width, Mq4LineStyles.ToLineStyle(Style));
                    }
                }
            }

            public double GetValueByShift(int shift)
            {
                var index = _timeSeries.InvertIndex(shift);
                var index1 = _timeSeries.GetIndexByTime(Time1);
                var index2 = _timeSeries.GetIndexByTime(Time2);

                if (index2 == index1)
                    return 0;

                return (index - index1) * (Price2 - Price1) / (index2 - index1) + Price1;
            }
        }

        class Mq4Arrow : Mq4Object
        {
            private readonly TimeSeries _timeSeries;
            private int _index;

            public Mq4Arrow(string name, int type, ChartObjects chartObjects, TimeSeries timeSeries) : base(name, type, chartObjects)
            {
                _timeSeries = timeSeries;
            }

            public override void Set(int index, Mq4Double value)
            {
                base.Set(index, value);
                switch (index)
                {
                    case OBJPROP_TIME1:
                        _index = _timeSeries.GetIndexByTime(Time1);
                        break;
                }
            }

            private int ArrowCode
            {
                get { return Get(OBJPROP_ARROWCODE); }
            }

            public override void Draw()
            {
                string arrowString;
                HorizontalAlignment horizontalAlignment;
                switch (ArrowCode)
                {
                    case SYMBOL_RIGHTPRICE:
                        horizontalAlignment = HorizontalAlignment.Right;
                        arrowString = Price1.ToString();
                        break;
                    case SYMBOL_LEFTPRICE:
                        horizontalAlignment = HorizontalAlignment.Left;
                        arrowString = Price1.ToString();
                        break;
                    default:
                        arrowString = EverTrend_Indicator.GetArrowByCode(ArrowCode);
                        horizontalAlignment = HorizontalAlignment.Center;
                        break;
                }
                DrawText(Name, arrowString, _index, Price1, VerticalAlignment.Center, horizontalAlignment, Color);
            }
        }

        bool ObjectSet(Mq4String name, int index, Mq4Double value)
        {
            _mq4ChartObjects.Set(name, index, value);
            return true;
        }

        Mq4Double ObjectGet(Mq4String name, int index)
        {
            return _mq4ChartObjects.Get(name, index);
        }


        bool ObjectCreate(Mq4String name, int type, int window, int time1, double price1, int time2 = 0, double price2 = 0, int time3 = 0, double price3 = 0)
        {
            _mq4ChartObjects.Create(name, type, window, time1, price1, time2, price2, time3, price3);
            return true;
        }

        bool ObjectDelete(Mq4String name)
        {
            _mq4ChartObjects.Delete(name);
            return true;
        }


        int ObjectsDeleteAll(int window = EMPTY, int type = EMPTY)
        {
            return _mq4ChartObjects.DeleteAll(type);
        }








    }

    //Custom Indicators Place Holder

    class Mq4DoubleComparer : IComparer<Mq4Double>
    {
        public int Compare(Mq4Double x, Mq4Double y)
        {
            return x.CompareTo(y);
        }
    }
    class Mq4String
    {
        private readonly string _value;

        public Mq4String(string value)
        {
            _value = value;
        }

        public static implicit operator Mq4String(string value)
        {
            return new Mq4String(value);
        }

        public static implicit operator Mq4String(int value)
        {
            return new Mq4String(value.ToString());
        }

        public static implicit operator Mq4String(Mq4Null mq4Null)
        {
            return new Mq4String(null);
        }

        public static implicit operator string(Mq4String mq4String)
        {
            return mq4String._value;
        }

        public static implicit operator Mq4String(Mq4Double mq4Double)
        {
            return new Mq4String(mq4Double.ToString());
        }

        public static bool operator <(Mq4String x, Mq4String y)
        {
            return string.Compare(x._value, y._value) == -1;
        }

        public static bool operator >(Mq4String x, Mq4String y)
        {
            return string.Compare(x._value, y._value) == 1;
        }

        public static bool operator <(Mq4String x, string y)
        {
            return string.Compare(x._value, y) == -1;
        }

        public static bool operator >(Mq4String x, string y)
        {
            return string.Compare(x._value, y) == 1;
        }
        public static bool operator <=(Mq4String x, Mq4String y)
        {
            return string.Compare(x._value, y._value) <= 0;
        }

        public static bool operator >=(Mq4String x, Mq4String y)
        {
            return string.Compare(x._value, y._value) >= 0;
        }

        public static bool operator <=(Mq4String x, string y)
        {
            return string.Compare(x._value, y) <= 0;
        }

        public static bool operator >=(Mq4String x, string y)
        {
            return string.Compare(x._value, y) >= 0;
        }

        public static bool operator ==(Mq4String x, Mq4String y)
        {
            return string.Compare(x._value, y._value) == 0;
        }

        public static bool operator !=(Mq4String x, Mq4String y)
        {
            return string.Compare(x._value, y._value) != 0;
        }

        public static bool operator ==(Mq4String x, string y)
        {
            return string.Compare(x._value, y) == 0;
        }

        public static bool operator !=(Mq4String x, string y)
        {
            return string.Compare(x._value, y) != 0;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public static readonly Mq4String Empty = new Mq4String(string.Empty);
    }
    struct Mq4Char
    {
        char _char;

        public Mq4Char(byte code)
        {
            _char = Encoding.Unicode.GetString(new byte[] 
            {
                code,
                0
            })[0];
        }

        public Mq4Char(char @char)
        {
            _char = @char;
        }

        public static implicit operator char(Mq4Char mq4Char)
        {
            return mq4Char._char;
        }

        public static implicit operator Mq4Char(int code)
        {
            return new Mq4Char((byte)code);
        }

        public static implicit operator Mq4Char(string str)
        {
            if (string.IsNullOrEmpty(str) || str.Length == 0)
                return new Mq4Char(' ');
            return new Mq4Char(str[0]);
        }
    }
    struct Mq4Null
    {
        public static implicit operator string(Mq4Null mq4Null)
        {
            return (string)null;
        }

        public static implicit operator int(Mq4Null mq4Null)
        {
            return 0;
        }

        public static implicit operator double(Mq4Null mq4Null)
        {
            return 0;
        }
    }
    static class Comparers
    {
        public static IComparer<T> GetComparer<T>()
        {
            if (typeof(T) == typeof(Mq4Double))
                return (IComparer<T>)new Mq4DoubleComparer();

            return Comparer<T>.Default;
        }
    }
    static class DataSeriesExtensions
    {
        public static int InvertIndex(this DataSeries dataSeries, int index)
        {
            return dataSeries.Count - 1 - index;
        }

        public static Mq4Double Last(this DataSeries dataSeries, int shift, DataSeries sourceDataSeries)
        {
            return dataSeries[sourceDataSeries.Count - 1 - shift];
        }
    }
    static class TimeSeriesExtensions
    {
        public static DateTime Last(this TimeSeries timeSeries, int index)
        {
            return timeSeries[timeSeries.InvertIndex(index)];
        }

        public static int InvertIndex(this TimeSeries timeSeries, int index)
        {
            return timeSeries.Count - 1 - index;
        }

        public static int GetIndexByTime(this TimeSeries timeSeries, DateTime time)
        {
            var index = timeSeries.Count - 1;
            for (var i = timeSeries.Count - 1; i >= 0; i--)
            {
                if (timeSeries[i] < time)
                {
                    index = i + 1;
                    break;
                }
            }
            return index;
        }
    }
    static class Mq4Colors
    {
        public static Colors GetColorByInteger(int integer)
        {
            switch (integer)
            {
                case 16777215:
                    return Colors.White;
                case 16448255:
                    return Colors.Snow;
                case 16449525:
                    return Colors.MintCream;
                case 16118015:
                    return Colors.LavenderBlush;
                case 16775408:
                    return Colors.AliceBlue;
                case 15794160:
                    return Colors.Honeydew;
                case 15794175:
                    return Colors.Ivory;
                case 16119285:
                    return Colors.WhiteSmoke;
                case 15136253:
                    return Colors.OldLace;
                case 14804223:
                    return Colors.MistyRose;
                case 16443110:
                    return Colors.Lavender;
                case 15134970:
                    return Colors.Linen;
                case 16777184:
                    return Colors.LightCyan;
                case 14745599:
                    return Colors.LightYellow;
                case 14481663:
                    return Colors.Cornsilk;
                case 14020607:
                    return Colors.PapayaWhip;
                case 14150650:
                    return Colors.AntiqueWhite;
                case 14480885:
                    return Colors.Beige;
                case 13499135:
                    return Colors.LemonChiffon;
                case 13495295:
                    return Colors.BlanchedAlmond;
                case 12903679:
                    return Colors.Bisque;
                case 13353215:
                    return Colors.Pink;
                case 12180223:
                    return Colors.PeachPuff;
                case 14474460:
                    return Colors.Gainsboro;
                case 12695295:
                    return Colors.LightPink;
                case 11920639:
                    return Colors.Moccasin;
                case 11394815:
                    return Colors.NavajoWhite;
                case 11788021:
                    return Colors.Wheat;
                case 13882323:
                    return Colors.LightGray;
                case 15658671:
                    return Colors.PaleTurquoise;
                case 11200750:
                    return Colors.PaleGoldenrod;
                case 15130800:
                    return Colors.PowderBlue;
                case 14204888:
                    return Colors.Thistle;
                case 10025880:
                    return Colors.PaleGreen;
                case 15128749:
                    return Colors.LightBlue;
                case 14599344:
                    return Colors.LightSteelBlue;
                case 16436871:
                    return Colors.LightSkyBlue;
                case 12632256:
                    return Colors.Silver;
                case 13959039:
                    return Colors.Aquamarine;
                case 9498256:
                    return Colors.LightGreen;
                case 9234160:
                    return Colors.Khaki;
                case 14524637:
                    return Colors.Plum;
                case 8036607:
                    return Colors.LightSalmon;
                case 15453831:
                    return Colors.SkyBlue;
                case 8421616:
                    return Colors.LightCoral;
                case 15631086:
                    return Colors.Violet;
                case 7504122:
                    return Colors.Salmon;
                case 11823615:
                    return Colors.HotPink;
                case 8894686:
                    return Colors.BurlyWood;
                case 8034025:
                    return Colors.DarkSalmon;
                case 9221330:
                    return Colors.Tan;
                case 15624315:
                    return Colors.MediumSlateBlue;
                case 6333684:
                    return Colors.SandyBrown;
                case 11119017:
                    return Colors.DarkGray;
                case 15570276:
                    return Colors.CornflowerBlue;
                case 5275647:
                    return Colors.Coral;
                case 9662683:
                    return Colors.PaleVioletRed;
                case 14381203:
                    return Colors.MediumPurple;
                case 14053594:
                    return Colors.Orchid;
                case 9408444:
                    return Colors.RosyBrown;
                case 4678655:
                    return Colors.Tomato;
                case 9419919:
                    return Colors.DarkSeaGreen;
                case 11193702:
                    return Colors.MediumAquamarine;
                case 3145645:
                    return Colors.GreenYellow;
                case 13850042:
                    return Colors.MediumOrchid;
                case 6053069:
                    return Colors.IndianRed;
                case 7059389:
                    return Colors.DarkKhaki;
                case 13458026:
                    return Colors.SlateBlue;
                case 14772545:
                    return Colors.RoyalBlue;
                case 13688896:
                    return Colors.Turquoise;
                case 16748574:
                    return Colors.DodgerBlue;
                case 13422920:
                    return Colors.MediumTurquoise;
                case 9639167:
                    return Colors.DeepPink;
                case 10061943:
                    return Colors.LightSlateGray;
                case 14822282:
                    return Colors.BlueViolet;
                case 4163021:
                    return Colors.Peru;
                case 9470064:
                    return Colors.SlateGray;
                case 8421504:
                    return Colors.Gray;
                case 255:
                    return Colors.Red;
                case 16711935:
                    return Colors.Magenta;
                case 16711680:
                    return Colors.Blue;
                case 16760576:
                    return Colors.DeepSkyBlue;
                case 16776960:
                    return Colors.Aqua;
                case 8388352:
                    return Colors.SpringGreen;
                case 65280:
                    return Colors.Lime;
                case 65407:
                    return Colors.Chartreuse;
                case 65535:
                    return Colors.Yellow;
                case 55295:
                    return Colors.Gold;
                case 42495:
                    return Colors.Orange;
                case 36095:
                    return Colors.DarkOrange;
                case 17919:
                    return Colors.OrangeRed;
                case 3329330:
                    return Colors.LimeGreen;
                case 3329434:
                    return Colors.YellowGreen;
                case 13382297:
                    return Colors.DarkOrchid;
                case 10526303:
                    return Colors.CadetBlue;
                case 64636:
                    return Colors.LawnGreen;
                case 10156544:
                    return Colors.MediumSpringGreen;
                case 2139610:
                    return Colors.Goldenrod;
                case 11829830:
                    return Colors.SteelBlue;
                case 3937500:
                    return Colors.Crimson;
                case 1993170:
                    return Colors.Chocolate;
                case 7451452:
                    return Colors.MediumSeaGreen;
                case 8721863:
                    return Colors.MediumVioletRed;
                case 13828244:
                    return Colors.DarkViolet;
                case 11186720:
                    return Colors.LightSeaGreen;
                case 6908265:
                    return Colors.DimGray;
                case 13749760:
                    return Colors.DarkTurquoise;
                case 2763429:
                    return Colors.Brown;
                case 13434880:
                    return Colors.MediumBlue;
                case 2970272:
                    return Colors.Sienna;
                case 9125192:
                    return Colors.DarkSlateBlue;
                case 755384:
                    return Colors.DarkGoldenrod;
                case 5737262:
                    return Colors.SeaGreen;
                case 2330219:
                    return Colors.OliveDrab;
                case 2263842:
                    return Colors.ForestGreen;
                case 1262987:
                    return Colors.SaddleBrown;
                case 3107669:
                    return Colors.DarkOliveGreen;
                case 9109504:
                    return Colors.DarkBlue;
                case 7346457:
                    return Colors.MidnightBlue;
                case 8519755:
                    return Colors.Indigo;
                case 128:
                    return Colors.Maroon;
                case 8388736:
                    return Colors.Purple;
                case 8388608:
                    return Colors.Navy;
                case 8421376:
                    return Colors.Teal;
                case 32768:
                    return Colors.Green;
                case 32896:
                    return Colors.Olive;
                case 5197615:
                    return Colors.DarkSlateGray;
                case 25600:
                    return Colors.DarkGreen;
                case 0:
                default:
                    return Colors.Black;
            }
        }
    }
    static class EventExtensions
    {
        public static void Raise<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
        {
            if (action != null)
                action(arg1, arg2);
        }
    }
    static class Mq4LineStyles
    {
        public static LineStyle ToLineStyle(int style)
        {
            switch (style)
            {
                case 1:
                    return LineStyle.Lines;
                case 2:
                    return LineStyle.Dots;
                case 3:
                case 4:
                    return LineStyle.LinesDots;
                default:
                    return LineStyle.Solid;
            }
        }
    }
    class Mq4TimeSeries
    {
        private readonly TimeSeries _timeSeries;
        private static readonly DateTime StartDateTime = new DateTime(1970, 1, 1);

        public Mq4TimeSeries(TimeSeries timeSeries)
        {
            _timeSeries = timeSeries;
        }

        public static int ToInteger(DateTime dateTime)
        {
            return (int)(dateTime - StartDateTime).TotalSeconds;
        }

        public static DateTime ToDateTime(int seconds)
        {
            return StartDateTime.AddSeconds(seconds);
        }

        public int this[int index]
        {
            get
            {
                if (index < 0 || index >= _timeSeries.Count)
                    return 0;

                DateTime dateTime = _timeSeries[_timeSeries.Count - 1 - index];

                return ToInteger(dateTime);
            }
        }
    }
    static class ConvertExtensions
    {
        public static double? ToNullableDouble(this double protection)
        {
            if (protection == 0)
                return null;
            return protection;
        }

        public static DateTime? ToNullableDateTime(this int time)
        {
            if (time == 0)
                return null;

            return Mq4TimeSeries.ToDateTime(time);
        }

        public static long ToUnitsVolume(this Symbol symbol, double lots)
        {
            return symbol.NormalizeVolume(symbol.ToNotNormalizedUnitsVolume(lots));
        }

        public static double ToNotNormalizedUnitsVolume(this Symbol symbol, double lots)
        {
            if (symbol.Code.Contains("XAU") || symbol.Code.Contains("XAG"))
                return 100 * lots;

            return 100000 * lots;
        }

        public static double ToLotsVolume(this Symbol symbol, long volume)
        {
            if (symbol.Code.Contains("XAU") || symbol.Code.Contains("XAG"))
                return volume * 1.0 / 100;

            return volume * 1.0 / 100000;
        }
    }
    struct Mq4Double : IComparable, IComparable<Mq4Double>
    {
        private readonly double _value;

        public Mq4Double(double value)
        {
            _value = value;
        }

        public static implicit operator double(Mq4Double property)
        {
            return property._value;
        }

        public static implicit operator int(Mq4Double property)
        {
            return (int)property._value;
        }

        public static implicit operator bool(Mq4Double property)
        {
            return (int)property._value != 0;
        }

        public static implicit operator Mq4Double(double value)
        {
            return new Mq4Double(value);
        }

        public static implicit operator Mq4Double(int value)
        {
            return new Mq4Double(value);
        }

        public static implicit operator Mq4Double(bool value)
        {
            return new Mq4Double(value ? 1 : 0);
        }

        public static implicit operator Mq4Double(Mq4Null value)
        {
            return new Mq4Double(0);
        }

        public static Mq4Double operator +(Mq4Double d1, Mq4Double d2)
        {
            return new Mq4Double(d1._value + d2._value);
        }

        public static Mq4Double operator -(Mq4Double d1, Mq4Double d2)
        {
            return new Mq4Double(d1._value - d2._value);
        }

        public static Mq4Double operator -(Mq4Double d)
        {
            return new Mq4Double(-d._value);
        }

        public static Mq4Double operator +(Mq4Double d)
        {
            return new Mq4Double(+d._value);
        }

        public static Mq4Double operator *(Mq4Double d1, Mq4Double d2)
        {
            return new Mq4Double(d1._value * d2._value);
        }

        public static Mq4Double operator /(Mq4Double d1, Mq4Double d2)
        {
            return new Mq4Double(d1._value / d2._value);
        }

        public static bool operator ==(Mq4Double d1, Mq4Double d2)
        {
            return d1._value == d2._value;
        }

        public static bool operator >(Mq4Double d1, Mq4Double d2)
        {
            return d1._value > d2._value;
        }

        public static bool operator >=(Mq4Double d1, Mq4Double d2)
        {
            return d1._value >= d2._value;
        }

        public static bool operator <(Mq4Double d1, Mq4Double d2)
        {
            return d1._value < d2._value;
        }

        public static bool operator <=(Mq4Double d1, Mq4Double d2)
        {
            return d1._value <= d2._value;
        }

        public static bool operator !=(Mq4Double d1, Mq4Double d2)
        {
            return d1._value != d2._value;
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public int CompareTo(object obj)
        {
            return _value.CompareTo(obj);
        }

        public int CompareTo(Mq4Double obj)
        {
            return _value.CompareTo(obj);
        }
    }
    class Mq4DoubleTwoDimensionalArray
    {
        private List<Mq4Double> _data = new List<Mq4Double>();
        private List<Mq4DoubleArray> _arrays = new List<Mq4DoubleArray>();
        private readonly Mq4Double _defaultValue;
        private readonly int _size2;

        public Mq4DoubleTwoDimensionalArray(int size2)
        {
            _defaultValue = 0;
            _size2 = size2;
        }

        public void Add(Mq4Double value)
        {
            _data.Add(value);
        }

        private void EnsureCountIsEnough(int index)
        {
            while (_arrays.Count <= index)
                _arrays.Add(new Mq4DoubleArray());
        }

        public void Initialize(Mq4Double value)
        {
            for (var i = 0; i < _data.Count; i++)
                _data[i] = value;
        }

        public int Range(int index)
        {
            if (index == 0)
                return _data.Count;
            return this[0].Length;
        }

        public Mq4DoubleArray this[int index]
        {
            get
            {
                if (index < 0)
                    return new Mq4DoubleArray();

                EnsureCountIsEnough(index);

                return _arrays[index];
            }
        }

        public Mq4Double this[int index1, int index2]
        {
            get
            {
                if (index1 < 0)
                    return 0;

                EnsureCountIsEnough(index1);

                return _arrays[index1][index2];
            }
            set
            {
                if (index1 < 0)
                    return;

                EnsureCountIsEnough(index1);

                _arrays[index1][index2] = value;
            }
        }
    }
    class Mq4DoubleArray : IMq4DoubleArray, IEnumerable
    {
        private List<Mq4Double> _data = new List<Mq4Double>();
        private readonly Mq4Double _defaultValue;

        public Mq4DoubleArray(int size = 0)
        {
            _defaultValue = 0;
        }

        public IEnumerator GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        private bool _isInverted;
        public bool IsInverted
        {
            get { return _isInverted; }
            set { _isInverted = value; }
        }

        public void Add(Mq4Double value)
        {
            _data.Add(value);
        }

        private void EnsureCountIsEnough(int index)
        {
            while (_data.Count <= index)
                _data.Add(_defaultValue);
        }

        public int Length
        {
            get { return _data.Count; }
        }

        public void Resize(int newSize)
        {
            while (newSize < _data.Count)
                _data.RemoveAt(_data.Count - 1);

            while (newSize > _data.Count)
                _data.Add(_defaultValue);
        }

        public Mq4Double this[int index]
        {
            get
            {
                if (index < 0)
                    return _defaultValue;

                EnsureCountIsEnough(index);

                return _data[index];
            }
            set
            {
                if (index < 0)
                    return;

                EnsureCountIsEnough(index);

                _data[index] = value;
                Changed.Raise(index, value);
            }
        }
        public event Action<int, Mq4Double> Changed;
    }
    class Mq4MarketDataSeries : IMq4DoubleArray
    {
        private DataSeries _dataSeries;

        public Mq4MarketDataSeries(DataSeries dataSeries)
        {
            _dataSeries = dataSeries;
        }

        public Mq4Double this[int index]
        {
            get { return _dataSeries.Last(index); }
            set { }
        }

        public int Length
        {
            get { return _dataSeries.Count; }
        }

        public void Resize(int newSize)
        {
        }
    }
    class Mq4StringArray : IEnumerable
    {
        private List<Mq4String> _data = new List<Mq4String>();
        private readonly Mq4String _defaultValue;

        public Mq4StringArray(int size = 0)
        {
            _defaultValue = "";
        }

        public IEnumerator GetEnumerator()
        {
            return _data.GetEnumerator();
        }

        private bool _isInverted;
        public bool IsInverted
        {
            get { return _isInverted; }
            set { _isInverted = value; }
        }

        public void Add(Mq4String value)
        {
            _data.Add(value);
        }

        private void EnsureCountIsEnough(int index)
        {
            while (_data.Count <= index)
                _data.Add(_defaultValue);
        }

        public int Length
        {
            get { return _data.Count; }
        }

        public void Resize(int newSize)
        {
            while (newSize < _data.Count)
                _data.RemoveAt(_data.Count - 1);

            while (newSize > _data.Count)
                _data.Add(_defaultValue);
        }

        public Mq4String this[int index]
        {
            get
            {
                if (index < 0)
                    return _defaultValue;

                EnsureCountIsEnough(index);

                return _data[index];
            }
            set
            {
                if (index < 0)
                    return;

                EnsureCountIsEnough(index);

                _data[index] = value;
            }
        }
    }
    interface IMq4DoubleArray
    {
        Mq4Double this[int index] { get; set; }
        int Length { get; }
        void Resize(int newSize);
    }
    class Mq4ArrayToDataSeriesConverter
    {
        private readonly Mq4DoubleArray _mq4Array;
        private readonly IndicatorDataSeries _dataSeries;

        public Mq4ArrayToDataSeriesConverter(Mq4DoubleArray mq4Array, IndicatorDataSeries dataSeries)
        {
            _mq4Array = mq4Array;
            _dataSeries = dataSeries;
            _mq4Array.Changed += OnValueChanged;
            CopyAllValues();
        }

        private void CopyAllValues()
        {
            for (var i = 0; i < _mq4Array.Length; i++)
            {
                if (_mq4Array.IsInverted)
                    _dataSeries[_mq4Array.Length - i] = _mq4Array[i];
                else
                    _dataSeries[i] = _mq4Array[i];
            }
        }

        private void OnValueChanged(int index, Mq4Double value)
        {
            int indexToSet;
            if (_mq4Array.IsInverted)
                indexToSet = _mq4Array.Length - index;
            else
                indexToSet = index;

            if (indexToSet < 0)
                return;

            _dataSeries[indexToSet] = value;
        }
    }
    class Mq4ArrayToDataSeriesConverterFactory
    {
        private readonly Dictionary<Mq4DoubleArray, IndicatorDataSeries> _cachedAdapters = new Dictionary<Mq4DoubleArray, IndicatorDataSeries>();
        private Func<IndicatorDataSeries> _dataSeriesFactory;

        public Mq4ArrayToDataSeriesConverterFactory(Func<IndicatorDataSeries> dataSeriesFactory)
        {
            _dataSeriesFactory = dataSeriesFactory;
        }

        public DataSeries Create(Mq4DoubleArray mq4Array)
        {
            IndicatorDataSeries dataSeries;

            if (_cachedAdapters.TryGetValue(mq4Array, out dataSeries))
                return dataSeries;

            dataSeries = _dataSeriesFactory();
            new Mq4ArrayToDataSeriesConverter(mq4Array, dataSeries);
            _cachedAdapters[mq4Array] = dataSeries;

            return dataSeries;
        }
    }

}

