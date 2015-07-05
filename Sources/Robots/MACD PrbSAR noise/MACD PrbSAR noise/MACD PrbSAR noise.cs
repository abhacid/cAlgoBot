//+------------------------------------------------------------------+
//+                          Code generated using FxPro Quant 2.0.12 |
//+------------------------------------------------------------------+

using System;
using System.Threading;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.API.Requests;
using cAlgo.Indicators;


namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC)]
    public class MACDPrbSARnoise : Robot
    {

        [Parameter("Noise_MACD_sm", DefaultValue = 41)]
        public int _Noise_MACD_sm { get; set; }
        [Parameter("Step_PrbSAR", DefaultValue = 0.031)]
        public double _Step_PrbSAR { get; set; }
        [Parameter("Lots", DefaultValue = 0.1)]
        public double _Lots { get; set; }
        [Parameter("Period_SlowEMA", DefaultValue = 26)]
        public int _Period_SlowEMA { get; set; }
        [Parameter("Noise_MACD_s0", DefaultValue = 21)]
        public int _Noise_MACD_s0 { get; set; }
        [Parameter("Period_FastEMA", DefaultValue = 12)]
        public int _Period_FastEMA { get; set; }
        [Parameter("Period_MACD_SMA", DefaultValue = 9)]
        public int _Period_MACD_SMA { get; set; }
        [Parameter("Stop_Loss", DefaultValue = 3000)]
        public int _Stop_Loss { get; set; }
        [Parameter("Noise_Prb_SAR_ema", DefaultValue = 351)]
        public int _Noise_Prb_SAR_ema { get; set; }
        [Parameter("Noise_MACD_m0", DefaultValue = 161)]
        public int _Noise_MACD_m0 { get; set; }

        //Global declaration
        private MacdHistogram i_MACD_main;
        private MacdHistogram i_MCAD_signal;
        private SimpleMovingAverage i_MA_Close;
        private ParabolicSAR i_Parabolic_SAR;
        private SimpleMovingAverage i_MA_Open;
        private ExponentialMovingAverage i_EMAf;
        double _MACD_main;
        double _MCAD_signal;
        double _MA_Close;
        double _Parabolic_SAR;
        bool _Compare;
        bool _Compare_1;
        bool _Compare_2;
        bool _Compare_3;

        DateTime LastTradeExecution = new DateTime(0);

        protected override void OnStart()
        {
            i_MACD_main = Indicators.MacdHistogram(MarketSeries.Close, (int)_Period_SlowEMA, (int)_Period_FastEMA, (int)_Period_MACD_SMA);
            i_MCAD_signal = Indicators.MacdHistogram(MarketSeries.Close, (int)_Period_SlowEMA, (int)_Period_FastEMA, (int)_Period_MACD_SMA);
            i_MA_Close = Indicators.SimpleMovingAverage(MarketSeries.Close, 1);
            i_Parabolic_SAR = Indicators.ParabolicSAR(_Step_PrbSAR, 0.1);
            i_MA_Open = Indicators.SimpleMovingAverage(MarketSeries.Open, 1);
            i_EMAf = Indicators.ExponentialMovingAverage(MarketSeries.Close, (int)_Period_FastEMA);

        }

        protected override void OnTick()
        {
            if (Trade.IsExecuting)
                return;

            //Local declaration
            TriState _Open_Buy = new TriState();
            TriState _Open_Sell = new TriState();
            TriState _Close_Sell = new TriState();
            TriState _Close_Buy = new TriState();

            //Step 1
            _MACD_main = i_MACD_main.Histogram.Last(0);
            _MCAD_signal = i_MCAD_signal.Signal.Last(0);
            _MA_Close = i_MA_Close.Result.Last(0);
            _Parabolic_SAR = i_Parabolic_SAR.Result.Last(0);

            //Step 2
            _Compare = (_MACD_main > 0);
            _Compare_1 = (_MACD_main > _MCAD_signal);
            _Compare_2 = (_MCAD_signal > 0);
            _Compare_3 = (_Parabolic_SAR < _MA_Close);

            //Step 3

            //Step 4

            //Step 5

            //Step 6
            if ((_Compare_1 && _Compare && _Compare_3 && (Math.Abs((_MACD_main - (_MCAD_signal))) > (_Noise_MACD_sm / (Math.Pow(10, Symbol.Digits)))) && (Math.Abs(_MACD_main) > (_Noise_MACD_m0 / (Math.Pow(10, Symbol.Digits)))) && (Math.Abs((_Parabolic_SAR - (i_EMAf.Result.Last(0)))) > (_Noise_Prb_SAR_ema / (Math.Pow(10, Symbol.Digits)))) && (Math.Abs(_MCAD_signal) > (_Noise_MACD_s0 / (Math.Pow(10, Symbol.Digits)))) && _Compare_2))
                _Open_Buy = _OpenPosition(1, true, Symbol.Code, TradeType.Buy, _Lots, 0, _Stop_Loss, 0, "");
            if ((!_Compare_1 && !_Compare && !_Compare_3 && (Math.Abs((_MACD_main - (_MCAD_signal))) > (_Noise_MACD_sm / (Math.Pow(10, Symbol.Digits)))) && (Math.Abs(_MACD_main) > (_Noise_MACD_m0 / (Math.Pow(10, Symbol.Digits)))) && (Math.Abs((_Parabolic_SAR - (i_EMAf.Result.Last(0)))) > (_Noise_Prb_SAR_ema / (Math.Pow(10, Symbol.Digits)))) && (Math.Abs(_MCAD_signal) > (_Noise_MACD_s0 / (Math.Pow(10, Symbol.Digits)))) && !_Compare_2))
                _Open_Sell = _OpenPosition(1, true, Symbol.Code, TradeType.Sell, _Lots, 0, _Stop_Loss, 0, "");

            //Step 7

            //Step 8
            if (((_OrderStatus(1, Symbol.Code, 4) && ((new Func<string, double, double>((symbolCode, magicIndex) =>
            {
                Symbol symbol = (symbolCode == "" || symbolCode == Symbol.Code) ? Symbol : MarketData.GetSymbol(symbolCode);
                var pos = Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                if (pos != null)
                    return pos.NetProfit;
                var po = PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                return po == null ? 0 : pos.NetProfit;
            })("", 1)) > Math.Abs(((new Func<string, double, double>((symbolCode, magicIndex) =>
            {
                Symbol symbol = (symbolCode == "" || symbolCode == Symbol.Code) ? Symbol : MarketData.GetSymbol(symbolCode);
                var pos = Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                if (pos != null)
                    return pos.Commissions;
                var po = PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                return po == null ? 0 : pos.Commissions;
            })("", 1)) + ((new Func<string, double, double>((symbolCode, magicIndex) =>
            {
                Symbol symbol = (symbolCode == "" || symbolCode == Symbol.Code) ? Symbol : MarketData.GetSymbol(symbolCode);
                var pos = Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                if (pos != null)
                    return pos.Swap;
                var po = PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                return po == null ? 0 : pos.Swap;
            })("", 1)))))) && _Compare_1 && _Compare_3)))
                _Close_Sell = _ClosePosition(1, Symbol.Code, 0);
            if (((_OrderStatus(1, Symbol.Code, 3) && ((new Func<string, double, double>((symbolCode, magicIndex) =>
            {
                Symbol symbol = (symbolCode == "" || symbolCode == Symbol.Code) ? Symbol : MarketData.GetSymbol(symbolCode);
                var pos = Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                if (pos != null)
                    return pos.NetProfit;
                var po = PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                return po == null ? 0 : pos.NetProfit;
            })("", 1)) > Math.Abs(((new Func<string, double, double>((symbolCode, magicIndex) =>
            {
                Symbol symbol = (symbolCode == "" || symbolCode == Symbol.Code) ? Symbol : MarketData.GetSymbol(symbolCode);
                var pos = Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                if (pos != null)
                    return pos.Commissions;
                var po = PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                return po == null ? 0 : pos.Commissions;
            })("", 1)) + ((new Func<string, double, double>((symbolCode, magicIndex) =>
            {
                Symbol symbol = (symbolCode == "" || symbolCode == Symbol.Code) ? Symbol : MarketData.GetSymbol(symbolCode);
                var pos = Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                if (pos != null)
                    return pos.Swap;
                var po = PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
                return po == null ? 0 : pos.Swap;
            })("", 1)))))) && !_Compare_1 && !_Compare_3)))
                _Close_Buy = _ClosePosition(1, Symbol.Code, 0);

        }

        bool NoOrders(string symbolCode, double[] magicIndecies)
        {
            if (symbolCode == "")
                symbolCode = Symbol.Code;
            string[] labels = new string[magicIndecies.Length];
            for (int i = 0; i < magicIndecies.Length; i++)
            {
                labels[i] = "FxProQuant_" + magicIndecies[i].ToString("F0");
            }
            foreach (Position pos in Positions)
            {
                if (pos.SymbolCode != symbolCode)
                    continue;
                if (labels.Length == 0)
                    return false;
                foreach (var label in labels)
                {
                    if (pos.Label == label)
                        return false;
                }
            }
            foreach (PendingOrder po in PendingOrders)
            {
                if (po.SymbolCode != symbolCode)
                    continue;
                if (labels.Length == 0)
                    return false;
                foreach (var label in labels)
                {
                    if (po.Label == label)
                        return false;
                }
            }
            return true;
        }

        TriState _OpenPosition(double magicIndex, bool noOrders, string symbolCode, TradeType tradeType, double lots, double slippage, double? stopLoss, double? takeProfit, string comment)
        {
            Symbol symbol = (Symbol.Code == symbolCode) ? Symbol : MarketData.GetSymbol(symbolCode);
            if (noOrders && Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol) != null)
                return new TriState();

            if (stopLoss < 1)
                stopLoss = null;

            if (takeProfit < 1)
                takeProfit = null;

            if (symbol.Digits == 5 || symbol.Digits == 3)
            {
                if (stopLoss != null)
                    stopLoss /= 10;

                if (takeProfit != null)
                    takeProfit /= 10;

                slippage /= 10;
            }

            int volume = (int)(lots * 100000);

            if (!ExecuteMarketOrder(tradeType, symbol, volume, "FxProQuant_" + magicIndex.ToString("F0"), stopLoss, takeProfit, slippage, comment).IsSuccessful)
            {
                Thread.Sleep(400);
                return false;
            }

            return true;
        }

        TriState _SendPending(double magicIndex, bool noOrders, string symbolCode, PendingOrderType poType, TradeType tradeType, double lots, int priceAction, double priceValue, double? stopLoss, double? takeProfit,
        
		DateTime? expiration, string comment)
        {
            Symbol symbol = (Symbol.Code == symbolCode) ? Symbol : MarketData.GetSymbol(symbolCode);
            if (noOrders && PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol) != null)
                return new TriState();

            if (stopLoss < 1)
                stopLoss = null;
            if (takeProfit < 1)
                takeProfit = null;
            if (symbol.Digits == 5 || symbol.Digits == 3)
            {
                if (stopLoss != null)
                    stopLoss /= 10;
                if (takeProfit != null)
                    takeProfit /= 10;
            }
            int volume = (int)(lots * 100000);
            double targetPrice;
            switch (priceAction)
            {
                case 0:
                    targetPrice = priceValue;
                    break;
                case 1:
                    targetPrice = symbol.Bid - priceValue * symbol.TickSize;
                    break;
                case 2:
                    targetPrice = symbol.Bid + priceValue * symbol.TickSize;
                    break;
                case 3:
                    targetPrice = symbol.Ask - priceValue * symbol.TickSize;
                    break;
                case 4:
                    targetPrice = symbol.Ask + priceValue * symbol.TickSize;
                    break;
                default:
                    targetPrice = priceValue;
                    break;
            }
            if (expiration.HasValue && expiration.Value.Ticks == 0)
                expiration = null;
            if (poType == PendingOrderType.Limit)
            {
                if (!PlaceLimitOrder(tradeType, symbol, volume, targetPrice, "FxProQuant_" + magicIndex.ToString("F0"), stopLoss, takeProfit, expiration, comment).IsSuccessful)
                {
                    Thread.Sleep(400);
                    return false;
                }
                return true;
            }
            else if (poType == PendingOrderType.Stop)
            {
                if (!PlaceStopOrder(tradeType, symbol, volume, targetPrice, "FxProQuant_" + magicIndex.ToString("F0"), stopLoss, takeProfit, expiration, comment).IsSuccessful)
                {
                    Thread.Sleep(400);
                    return false;
                }
                return true;
            }
            return new TriState();
        }

        TriState _ModifyPosition(double magicIndex, string symbolCode, int slAction, double slValue, int tpAction, double tpValue)
        {
            Symbol symbol = (Symbol.Code == symbolCode) ? Symbol : MarketData.GetSymbol(symbolCode);
            var pos = Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);

            if (pos == null)
                new TriState();

            double? sl, tp;

            if (slValue == 0)
                sl = null;
            else
            {
                switch (slAction)
                {
                    case 0:
                        sl = pos.StopLoss;
                        break;
                    case 1:
                        if (pos.TradeType == TradeType.Buy)
                            sl = pos.EntryPrice - slValue * symbol.TickSize;
                        else
                            sl = pos.EntryPrice + slValue * symbol.TickSize;
                        break;
                    case 2:
                        sl = slValue;
                        break;
                    default:
                        sl = pos.StopLoss;
                        break;
                }
            }

            if (tpValue == 0)
                tp = null;
            else
            {
                switch (tpAction)
                {
                    case 0:
                        tp = pos.TakeProfit;
                        break;
                    case 1:
                        if (pos.TradeType == TradeType.Buy)
                            tp = pos.EntryPrice + tpValue * symbol.TickSize;
                        else
                            tp = pos.EntryPrice - tpValue * symbol.TickSize;
                        break;
                    case 2:
                        tp = tpValue;
                        break;
                    default:
                        tp = pos.TakeProfit;
                        break;
                }
            }

            if (!ModifyPosition(pos, sl, tp).IsSuccessful)
            {
                Thread.Sleep(400);
                return false;
            }

            return true;
        }

        TriState _ModifyPending(double magicIndex, string symbolCode, int slAction, double slValue, int tpAction, double tpValue, int priceAction, double priceValue, int expirationAction, DateTime? expiration)
        {
            Symbol symbol = (Symbol.Code == symbolCode) ? Symbol : MarketData.GetSymbol(symbolCode);
            var po = PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
            if (po == null)
                new TriState();
            double targetPrice;
            double? sl, tp;
            if (slValue == 0)
                sl = null;
            else
            {
                switch (slAction)
                {
                    case 0:
                        sl = po.StopLoss;
                        break;
                    case 1:
                        if (po.TradeType == TradeType.Buy)
                            sl = po.TargetPrice - slValue * symbol.TickSize;
                        else
                            sl = po.TargetPrice + slValue * symbol.TickSize;
                        break;
                    case 2:
                        sl = slValue;
                        break;
                    default:
                        sl = po.StopLoss;
                        break;
                }
            }
            if (tpValue == 0)
                tp = null;
            else
            {
                switch (tpAction)
                {
                    case 0:
                        tp = po.TakeProfit;
                        break;
                    case 1:
                        if (po.TradeType == TradeType.Buy)
                            tp = po.TargetPrice + tpValue * symbol.TickSize;
                        else
                            tp = po.TargetPrice - tpValue * symbol.TickSize;
                        break;
                    case 2:
                        tp = tpValue;
                        break;
                    default:
                        tp = po.TakeProfit;
                        break;
                }
            }
            switch (priceAction)
            {
                case 0:
                    targetPrice = po.TargetPrice;
                    break;
                case 1:
                    targetPrice = priceValue;
                    break;
                case 2:
                    targetPrice = po.TargetPrice + priceValue * symbol.TickSize;
                    break;
                case 3:
                    targetPrice = po.TargetPrice - priceValue * symbol.TickSize;
                    break;
                case 4:
                    targetPrice = symbol.Bid - priceValue * symbol.TickSize;
                    break;
                case 5:
                    targetPrice = symbol.Bid + priceValue * symbol.TickSize;
                    break;
                case 6:
                    targetPrice = symbol.Ask - priceValue * symbol.TickSize;
                    break;
                case 7:
                    targetPrice = symbol.Ask + priceValue * symbol.TickSize;
                    break;
                default:
                    targetPrice = po.TargetPrice;
                    break;
            }
            if (expiration.HasValue && expiration.Value.Ticks == 0)
                expiration = null;
            if (expirationAction == 0)
                expiration = po.ExpirationTime;
            if (!ModifyPendingOrder(po, targetPrice, sl, tp, expiration).IsSuccessful)
            {
                Thread.Sleep(400);
                return false;
            }
            return true;
        }

        TriState _ClosePosition(double magicIndex, string symbolCode, double lots)
        {
            Symbol symbol = (Symbol.Code == symbolCode) ? Symbol : MarketData.GetSymbol(symbolCode);
            var pos = Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
            if (pos == null)
                new TriState();

            TradeResult result;
            if (lots == 0)
            {
                result = ClosePosition(pos);
            }
            else
            {
                int volume = (int)(lots * 100000);
                result = ClosePosition(pos, volume);
            }
            if (!result.IsSuccessful)
            {
                Thread.Sleep(400);
                return false;
            }
            return true;
        }

        TriState _DeletePending(double magicIndex, string symbolCode)
        {
            Symbol symbol = (Symbol.Code == symbolCode) ? Symbol : MarketData.GetSymbol(symbolCode);
            var po = PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
            if (po == null)
                new TriState();
            if (!CancelPendingOrder(po).IsSuccessful)
            {
                Thread.Sleep(400);
                return false;
            }
            return true;
        }

        bool _OrderStatus(double magicIndex, string symbolCode, int test)
        {
            Symbol symbol = (Symbol.Code == symbolCode) ? Symbol : MarketData.GetSymbol(symbolCode);
            var pos = Positions.Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
            if (pos != null)
            {
                if (test == 1)
                    return true;
                if (test == 3)
                    return pos.TradeType == TradeType.Buy;
                if (test == 4)
                    return pos.TradeType == TradeType.Sell;
            }
            var po = PendingOrders.__Find("FxProQuant_" + magicIndex.ToString("F0"), symbol);
            if (po != null)
            {
                if (test == 2)
                    return true;
                if (test == 3)
                    return po.TradeType == TradeType.Buy;
                if (test == 4)
                    return po.TradeType == TradeType.Sell;
                if (test == 5)
                    return po.OrderType == PendingOrderType.Limit;
                if (test == 6)
                    return po.OrderType == PendingOrderType.Stop;
            }
            return false;
        }

        int TimeframeToInt(TimeFrame tf)
        {
            if (tf == TimeFrame.Minute)
                return 1;
            else if (tf == TimeFrame.Minute2)
                return 2;
            else if (tf == TimeFrame.Minute3)
                return 3;
            else if (tf == TimeFrame.Minute4)
                return 4;
            else if (tf == TimeFrame.Minute5)
                return 5;
            else if (tf == TimeFrame.Minute10)
                return 10;
            else if (tf == TimeFrame.Minute15)
                return 15;
            else if (tf == TimeFrame.Minute30)
                return 30;
            else if (tf == TimeFrame.Hour)
                return 60;
            else if (tf == TimeFrame.Hour4)
                return 240;
            else if (tf == TimeFrame.Daily)
                return 1440;
            else if (tf == TimeFrame.Weekly)
                return 10080;
            else if (tf == TimeFrame.Monthly)
                return 43200;
            return 1;
        }
    }
}

public struct TriState
{
    public static readonly TriState NonExecution = new TriState(0);
    public static readonly TriState False = new TriState(-1);
    public static readonly TriState True = new TriState(1);
    sbyte value;

    TriState(int value)
    {
        this.value = (sbyte)value;
    }

    public bool IsNonExecution
    {
        get { return value == 0; }
    }

    public static implicit operator TriState(bool x)
    {
        return x ? True : False;
    }

    public static TriState operator ==(TriState x, TriState y)
    {
        if (x.value == 0 || y.value == 0)
            return NonExecution;

        return x.value == y.value ? True : False;
    }

    public static TriState operator !=(TriState x, TriState y)
    {
        if (x.value == 0 || y.value == 0)
            return NonExecution;

        return x.value != y.value ? True : False;
    }

    public static TriState operator !(TriState x)
    {
        return new TriState(-x.value);
    }

    public static TriState operator &(TriState x, TriState y)
    {
        return new TriState(x.value < y.value ? x.value : y.value);
    }

    public static TriState operator |(TriState x, TriState y)
    {
        return new TriState(x.value > y.value ? x.value : y.value);
    }

    public static bool operator true(TriState x)
    {
        return x.value > 0;
    }

    public static bool operator false(TriState x)
    {
        return x.value < 0;
    }

    public static implicit operator bool(TriState x)
    {
        return x.value > 0;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is TriState))
            return false;

        return value == ((TriState)obj).value;
    }

    public override int GetHashCode()
    {
        return value;
    }

    public override string ToString()
    {
        if (value > 0)
            return "True";

        if (value < 0)
            return "False";

        return "NonExecution";
    }
}

public static class PendingEx
{
    public static PendingOrder __Find(this cAlgo.API.PendingOrders pendingOrders, string label, Symbol symbol)
    {
        foreach (PendingOrder po in pendingOrders)
        {
            if (po.SymbolCode == symbol.Code && po.Label == label)
                return po;
        }
        return null;
    }
}
