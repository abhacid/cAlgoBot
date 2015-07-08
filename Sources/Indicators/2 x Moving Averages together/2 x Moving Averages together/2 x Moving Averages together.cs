// ------------------------------------------------------------
// Paste this code into your cAlgo editor
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

namespace cAlgo.Indicators
{
    [Indicator(ScalePrecision = 5, AutoRescale = false, IsOverlay = true, AccessRights = AccessRights.None)]
    [Levels()]
    public class Custom3MovingAverageIndicator : Indicator
    {
        Mq4Double Mq4Init()
        {
            Mq4String short_name = "";

            SetIndexStyle(0, DRAW_LINE);
            SetIndexShift(0, MA_Shift);
            SetIndexStyle(1, DRAW_LINE);
            SetIndexShift(1, MA_Shift);

            switch (MA_Method)
            {
                case 1:
                    short_name = "EMA(";
                    break;
                case 2:
                    short_name = "SMMA(";
                    break;
                case 3:
                    short_name = "LWMA(";
                    break;
                default:
                    MA_Method = 0;
                    short_name = "SMA(";
                    break;

            }
            IndicatorShortName(short_name + MA_Period_0 + " " + MA_Period_1 + ")");
            SetIndexLabel(0, "MA" + DoubleToStr(MA_Period_0, 0));
            SetIndexLabel(1, "MA" + DoubleToStr(MA_Period_1, 0));
            SetIndexBuffer(0, ExtMapBuffer0);
            SetIndexBuffer(1, ExtMapBuffer1);
            return 0;
            return 0;
        }
        Mq4Double Mq4Start()
        {
            Mq4Double i = 0;
            Mq4Double counted_bars = 0;
            Mq4Double limit = 0;
            if (Bars <= MA_Period_0 || Bars <= MA_Period_1)
                return 0;

            counted_bars = IndicatorCounted();
            if (counted_bars > 0)
                counted_bars--;
            limit = Bars - counted_bars;
            for (i = 0; i < limit; i++)
            {
                ExtMapBuffer0[i] = iMA(NULL, 0, MA_Period_0, MA_Shift, MA_Method, Applied_Price, i);
                ExtMapBuffer1[i] = iMA(NULL, 0, MA_Period_1, MA_Shift, MA_Method, Applied_Price, i);
            }
            return 0;
            return 0;
        }

        [Parameter("MA_Period_0", DefaultValue = 5)]
        public double MA_Period_0_parameter { get; set; }
        bool _MA_Period_0Got;
        Mq4Double MA_Period_0_backfield;
        Mq4Double MA_Period_0
        {
            get
            {
                if (!_MA_Period_0Got)
                    MA_Period_0_backfield = MA_Period_0_parameter;
                return MA_Period_0_backfield;
            }
            set { MA_Period_0_backfield = value; }
        }

        [Parameter("MA_Period_1", DefaultValue = 20)]
        public double MA_Period_1_parameter { get; set; }
        bool _MA_Period_1Got;
        Mq4Double MA_Period_1_backfield;
        Mq4Double MA_Period_1
        {
            get
            {
                if (!_MA_Period_1Got)
                    MA_Period_1_backfield = MA_Period_1_parameter;
                return MA_Period_1_backfield;
            }
            set { MA_Period_1_backfield = value; }
        }

        [Parameter("MA_Method", DefaultValue = 1)]
        public double MA_Method_parameter { get; set; }
        bool _MA_MethodGot;
        Mq4Double MA_Method_backfield;
        Mq4Double MA_Method
        {
            get
            {
                if (!_MA_MethodGot)
                    MA_Method_backfield = MA_Method_parameter;
                return MA_Method_backfield;
            }
            set { MA_Method_backfield = value; }
        }

        [Parameter("MA_Shift", DefaultValue = 0)]
        public double MA_Shift_parameter { get; set; }
        bool _MA_ShiftGot;
        Mq4Double MA_Shift_backfield;
        Mq4Double MA_Shift
        {
            get
            {
                if (!_MA_ShiftGot)
                    MA_Shift_backfield = MA_Shift_parameter;
                return MA_Shift_backfield;
            }
            set { MA_Shift_backfield = value; }
        }

        [Parameter("Applied_Price", DefaultValue = 0)]
        public double Applied_Price_parameter { get; set; }
        bool _Applied_PriceGot;
        Mq4Double Applied_Price_backfield;
        Mq4Double Applied_Price
        {
            get
            {
                if (!_Applied_PriceGot)
                    Applied_Price_backfield = Applied_Price_parameter;
                return Applied_Price_backfield;
            }
            set { Applied_Price_backfield = value; }
        }



        [Output("ExtMapBuffer0", Color = Colors.Yellow, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries ExtMapBuffer0_AlgoOutputDataSeries { get; set; }
        [Output("ExtMapBuffer1", Color = Colors.Blue, PlotType = PlotType.DiscontinuousLine)]
        public IndicatorDataSeries ExtMapBuffer1_AlgoOutputDataSeries { get; set; }


        Mq4Double ExtCountedBars = 0;

        int indicator_buffers = 2;
        int indicator_color1 = Yellow;
        int indicator_color2 = Blue;


        Mq4Double indicator_width1 = 1;
        Mq4Double indicator_width2 = 1;
        Mq4Double indicator_width3 = 1;
        Mq4Double indicator_width4 = 1;
        Mq4Double indicator_width5 = 1;
        Mq4Double indicator_width6 = 1;
        Mq4Double indicator_width7 = 1;
        Mq4Double indicator_width8 = 1;




        protected override void Initialize()
        {
            _closeExtremums = new DataSeriesExtremums(MarketSeries.Close);
            if (ExtMapBuffer0_AlgoOutputDataSeries == null)
                ExtMapBuffer0_AlgoOutputDataSeries = CreateDataSeries();
            ExtMapBuffer0 = new Mq4OutputDataSeries(this, ExtMapBuffer0_AlgoOutputDataSeries, _closeExtremums, ChartObjects, 0, 0, () => CreateDataSeries(), 1, Colors.Yellow);
            AllBuffers.Add(ExtMapBuffer0);
            if (ExtMapBuffer1_AlgoOutputDataSeries == null)
                ExtMapBuffer1_AlgoOutputDataSeries = CreateDataSeries();
            ExtMapBuffer1 = new Mq4OutputDataSeries(this, ExtMapBuffer1_AlgoOutputDataSeries, _closeExtremums, ChartObjects, 0, 1, () => CreateDataSeries(), 1, Colors.Blue);
            AllBuffers.Add(ExtMapBuffer1);

            AllOutputDataSeries.Add(ExtMapBuffer0_AlgoOutputDataSeries);
            AllOutputDataSeries.Add(ExtMapBuffer1_AlgoOutputDataSeries);


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

        int _currentIndex;
        CachedStandardIndicators _cachedStandardIndicators;
        Mq4ChartObjects _mq4ChartObjects;
        private Mq4OutputDataSeries ExtMapBuffer0;
        private Mq4OutputDataSeries ExtMapBuffer1;

        Mq4MarketDataSeries Open;
        Mq4MarketDataSeries High;
        Mq4MarketDataSeries Low;
        Mq4MarketDataSeries Close;
        Mq4MarketDataSeries Median;
        Mq4MarketDataSeries Volume;
        Mq4TimeSeries Time;

        DataSeriesExtremums _closeExtremums;
        List<Mq4OutputDataSeries> AllBuffers = new List<Mq4OutputDataSeries>();
        Mq4ArrayToDataSeriesConverterFactory _mq4ArrayToDataSeriesConverterFactory;

        public List<DataSeries> AllOutputDataSeries = new List<DataSeries>();

        private bool _initialized;
        public override void Calculate(int index)
        {
            try
            {
                _currentIndex = index;
                ExtMapBuffer0.SetCurrentIndex(index);
                ExtMapBuffer1.SetCurrentIndex(index);


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

        Symbol GetSymbol(string symbolCode)
        {
            if (symbolCode == "0" || string.IsNullOrEmpty(symbolCode))
                symbolCode = Symbol.Code;
            return MarketData.GetSymbol(symbolCode);
        }

        MarketSeries GetSeries(string symbol, int period)
        {
            var timeFrame = PeriodToTimeFrame(period);
            var symbolObject = GetSymbol(symbol);

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
                    return series.Weighted;
            }
            throw new NotImplementedException("Converter doesn't support working with this type of AppliedPrice");
        }

        private int Bars
        {
            get { return MarketSeries.Close.Count; }
        }
        private int Digits
        {
            get
            {
                if (Symbol == null)
                    return 0;
                return Symbol.Digits;
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

























        int TimeCurrent()
        {
            return Mq4TimeSeries.ToInteger(Server.Time);
        }













        const string NotSupportedDifferentTimeFrame = "Converter doesn't support working with different timeframe";
        const string NotSupportedMaShift = "Converter supports only ma_shift = 0";
        const string NotSupportedBandsShift = "Converter supports only bands_shift = 0";
        const string AdxSupportsOnlyClosePrice = "Adx can be applied only to Close price";













        Mq4Double MarketInfo(Mq4String symbol, int type)
        {
            var symbolObject = GetSymbol(symbol);
            switch (type)
            {
                case MODE_LOW:
                    return GetSeries(symbol, PERIOD_D1).Low.LastValue;
                case MODE_HIGH:
                    return GetSeries(symbol, PERIOD_D1).High.LastValue;
                case MODE_DIGITS:
                    return symbolObject.Digits;
                case MODE_TIME:
                    return TimeCurrent();
                case MODE_ASK:
                    return symbolObject.Ask;
                case MODE_BID:
                    return symbolObject.Bid;
                case MODE_SPREAD:
                    return symbolObject.Spread / symbolObject.PointSize;
                case MODE_PROFITCALCMODE:
                    return 0;
                case MODE_FREEZELEVEL:
                    return 0;
                case MODE_TRADEALLOWED:
                    return 1;
                case MODE_POINT:
                    return symbolObject.PointSize;
                case MODE_TICKSIZE:
                    return symbolObject.PointSize;
                case MODE_SWAPTYPE:
                    return 0;
                case MODE_MARGINCALCMODE:
                    return 0;
            }
            return 0;
        }

        Mq4Double Bid
        {
            get
            {
                if (Symbol == null || double.IsNaN(Symbol.Bid))
                    return 0;
                return Symbol.Bid;
            }
        }

        Mq4Double Ask
        {
            get
            {
                if (Symbol == null || double.IsNaN(Symbol.Ask))
                    return 0;
                return Symbol.Ask;
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
//{
        #region iMA
        private double iMA(Mq4String symbol, int timeframe, int period, int ma_shift, int ma_method, int applied_price, int shift)
        {
            if (ma_shift != 0)
                throw new NotImplementedException(NotSupportedMaShift);

            var series = ToAppliedPrice(symbol, timeframe, applied_price);

            return CalculateiMA(series, period, ma_method, shift);
        }

        private double iMAOnArray(Mq4OutputDataSeries invertedDataSeries, int total, int period, int ma_shift, int ma_method, int shift)
        {
            return CalculateiMA(invertedDataSeries.OutputDataSeries, period, ma_method, shift);
        }

        private double iMAOnArray(Mq4Array<Mq4Double> mq4Array, int total, int period, int ma_shift, int ma_method, int shift)
        {
            var dataSeries = _mq4ArrayToDataSeriesConverterFactory.Create(mq4Array);
            return CalculateiMA(dataSeries, period, ma_method, shift);
        }

        private double CalculateiMA(DataSeries dataSeries, int period, int ma_method, int shift)
        {
            if (ma_method == MODE_SMMA)
            {
                return CalculateWellesWilderSmoothing(dataSeries, period, shift);
            }

            var maType = ToMaType(ma_method);
            var indicator = _cachedStandardIndicators.MovingAverage(dataSeries, period, maType);

            return indicator.Result.FromEnd(shift);
        }

        private double CalculateWellesWilderSmoothing(DataSeries dataSeries, int period, int shift)
        {
            var indicator = _cachedStandardIndicators.WellesWilderSmoothing(dataSeries, period);

            return indicator.Result.FromEnd(shift);
        }
        #endregion
        //}



















        class CachedStandardIndicators
        {
            private readonly IIndicatorsAccessor _indicatorsAccessor;

            public CachedStandardIndicators(IIndicatorsAccessor indicatorsAccessor)
            {
                _indicatorsAccessor = indicatorsAccessor;
            }
//{
            #region iMA
            private struct MAParameters
            {
                public DataSeries Source;
                public int Periods;
                public MovingAverageType MovingAverageType;
            }

            private struct WellesWilderSmoothingParameters
            {
                public DataSeries Source;
                public int Periods;
            }

            private Dictionary<MAParameters, MovingAverage> _movingAverages = new Dictionary<MAParameters, MovingAverage>();
            private Dictionary<WellesWilderSmoothingParameters, WellesWilderSmoothing> _wellesWilderSmoothings = new Dictionary<WellesWilderSmoothingParameters, WellesWilderSmoothing>();

            public MovingAverage MovingAverage(DataSeries source, int periods, MovingAverageType maType)
            {
                var maParameters = new MAParameters 
                {
                    MovingAverageType = maType,
                    Periods = periods,
                    Source = source
                };
                if (_movingAverages.ContainsKey(maParameters))
                    return _movingAverages[maParameters];

                var indicator = _indicatorsAccessor.MovingAverage(source, periods, maType);
                _movingAverages.Add(maParameters, indicator);

                return indicator;
            }

            public WellesWilderSmoothing WellesWilderSmoothing(DataSeries source, int periods)
            {
                var parameters = new WellesWilderSmoothingParameters 
                {
                    Periods = periods,
                    Source = source
                };
                if (_wellesWilderSmoothings.ContainsKey(parameters))
                    return _wellesWilderSmoothings[parameters];

                var indicator = _indicatorsAccessor.WellesWilderSmoothing(source, periods);
                _wellesWilderSmoothings.Add(parameters, indicator);

                return indicator;
            }
            #endregion
            //}










        }

        void SetIndexStyle(int index, int type, int style = EMPTY, int width = EMPTY, int clr = CLR_NONE)
        {
        }

        void IndicatorDigits(int digits)
        {
        }

        void IndicatorDigits(double digits)
        {
        }




        bool SetIndexBuffer(int index, Mq4OutputDataSeries dataSeries)
        {
            return true;
        }

        void IndicatorShortName(Mq4String name)
        {
        }

        void SetIndexLabel(int index, string text)
        {
        }
        const string xArrow = "✖";
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
        void SetIndexShift(int index, int shift)
        {
            AllBuffers[index].SetShift(shift);
        }

        private int _indicatorCounted;
        private int IndicatorCounted()
        {
            return _indicatorCounted;
        }
        const bool True = true;
        const bool False = false;
        const bool TRUE = true;
        const bool FALSE = false;
        Mq4Null NULL;
        const int EMPTY = -1;
        const double EMPTY_VALUE = double.NaN;
        const int WHOLE_ARRAY = 0;

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
        /*const int MODE_STOPLEVEL = 14;
    const int MODE_LOTSIZE = 15;
    const int MODE_TICKVALUE = 16;    
    const int MODE_SWAPLONG = 18;
    const int MODE_SWAPSHORT = 19;
    const int MODE_STARTING = 20;
    const int MODE_EXPIRATION = 21;    
    const int MODE_MINLOT = 23;
    const int MODE_LOTSTEP = 24;
    const int MODE_MAXLOT = 25;
    const int MODE_MARGININIT = 29;
    const int MODE_MARGINMAINTENANCE = 30;
    const int MODE_MARGINHEDGED = 31;
    const int MODE_MARGINREQUIRED = 32;*/

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

        const int MODE_ASCEND = 1;
        const int MODE_DESCEND = 2;
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

            public void Set(string name, int index, Mq4Double value)
            {
                if (!_mq4ObjectByName.ContainsKey(name))
                    return;
                _mq4ObjectByName[name].Set(index, value);
                _mq4ObjectByName[name].Draw();
            }
            public void SetText(string name, string text, int font_size, string font, int color)
            {
                if (!_mq4ObjectByName.ContainsKey(name))
                    return;

                Set(name, OBJPROP_COLOR, color);
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
                        arrowString = Custom3MovingAverageIndicator.GetArrowByCode(ArrowCode);
                        horizontalAlignment = HorizontalAlignment.Center;
                        break;
                }
                DrawText(Name, arrowString, _index, Price1, VerticalAlignment.Center, horizontalAlignment, Color);
            }
        }













        class DataSeriesExtremums
        {
            private int? _lastCheckedIndex;
            private readonly DataSeries _dataSeries;
            private double _min = double.MaxValue;
            private double _max = double.MinValue;

            public DataSeriesExtremums(DataSeries dataSeries)
            {
                _dataSeries = dataSeries;
            }

            private void UpdateMinAndMax()
            {
                var indexFrom = _lastCheckedIndex != null ? _lastCheckedIndex.Value + 1 : 0;
                for (var i = indexFrom; i < _dataSeries.Count - 1; i++)
                {
                    if (_dataSeries[i] < _min)
                        _min = _dataSeries[i];
                    if (_dataSeries[i] > _max)
                        _max = _dataSeries[i];
                    _lastCheckedIndex = i;
                }
            }

            public double Min
            {
                get
                {
                    UpdateMinAndMax();
                    return _min;
                }
            }

            public double Max
            {
                get
                {
                    UpdateMinAndMax();
                    return _max;
                }
            }
        }
        class Mq4MarketDataSeries : IMq4Array<Mq4Double>
        {
            private DataSeries _dataSeries;

            public Mq4MarketDataSeries(DataSeries dataSeries)
            {
                _dataSeries = dataSeries;
            }

            public Mq4Double this[int index]
            {
                get { return _dataSeries.FromEnd(index); }
                set { }
            }

            public int Length
            {
                get { return _dataSeries.Count; }
            }
        }
        class Mq4OutputDataSeries : IMq4Array<Mq4Double>
        {
            public IndicatorDataSeries OutputDataSeries { get; private set; }
            private readonly IndicatorDataSeries _originalValues;
            private int _currentIndex;
            private int _shift;
            private double _emptyValue = double.NaN;
            private readonly DataSeriesExtremums _closeExtremums;
            private readonly ChartObjects _chartObjects;
            private readonly int _style;
            private readonly int _bufferIndex;
            private readonly Custom3MovingAverageIndicator _indicator;

            public Mq4OutputDataSeries(Custom3MovingAverageIndicator indicator, IndicatorDataSeries outputDataSeries, DataSeriesExtremums closeExtremums, ChartObjects chartObjects, int style, int bufferIndex, Func<IndicatorDataSeries> dataSeriesFactory, int lineWidth, Colors? color = null)
            {
                OutputDataSeries = outputDataSeries;
                _closeExtremums = closeExtremums;
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
                    if (indexToGetFrom < 0 || indexToGetFrom >= _originalValues.Count)
                        return 0;

                    return _originalValues[_currentIndex - index + _shift];
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

                    if (true)
                    {
                        var validRange = _closeExtremums.Max - _closeExtremums.Min;
                        if (value > _closeExtremums.Max + validRange || value < _closeExtremums.Min - validRange)
                            return;
                    }

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
        interface IMq4Array<T>
        {
            T this[int index] { get; set; }
            int Length { get; }
        }
        class Mq4Array<T> : IMq4Array<T>, IEnumerable
        {
            private List<T> _data = new List<T>();
            private readonly T _defaultValue;

            public Mq4Array(int size = 0)
            {
                _defaultValue = (T)DefaultValues.GetDefaultValue<T>();
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

            public void Add(T value)
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

            public T this[int index]
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
            public event Action<int, T> Changed;
        }
        static class DefaultValues
        {
            public static object GetDefaultValue<T>()
            {
                if (typeof(T) == typeof(Mq4Double))
                    return new Mq4Double(0);
                if (typeof(T) == typeof(string))
                    return string.Empty;
                if (typeof(T) == typeof(Mq4String))
                    return new Mq4String(string.Empty);

                return default(T);
            }
        }
        class Mq4ArrayToDataSeriesConverter
        {
            private readonly Mq4Array<Mq4Double> _mq4Array;
            private readonly IndicatorDataSeries _dataSeries;

            public Mq4ArrayToDataSeriesConverter(Mq4Array<Mq4Double> mq4Array, IndicatorDataSeries dataSeries)
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
            private readonly Dictionary<Mq4Array<Mq4Double>, IndicatorDataSeries> _cachedAdapters = new Dictionary<Mq4Array<Mq4Double>, IndicatorDataSeries>();
            private Func<IndicatorDataSeries> _dataSeriesFactory;

            public Mq4ArrayToDataSeriesConverterFactory(Func<IndicatorDataSeries> dataSeriesFactory)
            {
                _dataSeriesFactory = dataSeriesFactory;
            }

            public DataSeries Create(Mq4Array<Mq4Double> mq4Array)
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

    //Custom Indicators Place Holder

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
        public static Mq4Double FromEnd(this DataSeries dataSeries, int index)
        {
            if (index >= 0 && index < dataSeries.Count)
                return dataSeries[dataSeries.InvertIndex(index)];
            return 0;
        }

        public static int InvertIndex(this DataSeries dataSeries, int index)
        {
            return dataSeries.Count - 1 - index;
        }
    }
    static class TimeSeriesExtensions
    {
        public static DateTime FromEnd(this TimeSeries timeSeries, int index)
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
}
