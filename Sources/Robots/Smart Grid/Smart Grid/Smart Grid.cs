//+------------------------------------------------------------------+
//|                                                  Smart Grid      |
//|                                      Copyright 2014, MD SAIF     |
//|                                   http://www.facebook.com/cls.fx |
//+------------------------------------------------------------------+
//-Grid trader cBot based on Bar-Time & Trend. For range market & 15 minute TimeFrame is best.

using System;
using System.Linq;
using cAlgo.API;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class SmartGrid : Robot
    {
        [Parameter("Buy", DefaultValue = true)]
        public bool Buy { get; set; }

        [Parameter("Sell", DefaultValue = true)]
        public bool Sell { get; set; }

        [Parameter("Pip Step", DefaultValue = 10, MinValue = 1)]
        public int PipStep { get; set; }

        [Parameter("First Volume", DefaultValue = 1000, MinValue = 1000, Step = 1000)]
        public int FirstVolume { get; set; }

        [Parameter("Volume Exponent", DefaultValue = 1.0, MinValue = 0.1, MaxValue = 5.0)]
        public double VolumeExponent { get; set; }

        [Parameter("Max Spread", DefaultValue = 3.0)]
        public double MaxSpread { get; set; }

        [Parameter("Average TP", DefaultValue = 3, MinValue = 1)]
        public int AverageTP { get; set; }

        private string Label = "cls";
        private Position position;
        private DateTime tc_31;
        private DateTime tc_32;
        private int gi_21;
        private double sp_d;
        private bool is_12 = true;
        private bool cStop = false;
        protected override void OnStart()
        {
        }
        protected override void OnTick()
        {
            sp_d = (Symbol.Ask - Symbol.Bid) / Symbol.PipSize;
            if (o_tm(TradeType.Buy) > 0)
                f0_86(pnt_12(TradeType.Buy), AverageTP);
            if (o_tm(TradeType.Sell) > 0)
                f0_88(pnt_12(TradeType.Sell), AverageTP);
            if (MaxSpread >= sp_d && !cStop)
                Open_24();
            RCN();
        }
        protected override void OnError(Error error)
        {
            if (error.Code == ErrorCode.NoMoney)
            {
                cStop = true;
                Print("openning stopped because: not enough money");
            }
        }
        protected override void OnBar()
        {
            RefreshData();
        }
        protected override void OnStop()
        {
            ChartObjects.RemoveAllObjects();
        }
        private void Open_24()
        {
            if (is_12)
            {
                if (Buy && o_tm(TradeType.Buy) == 0 && MarketSeries.Close.Last(1) > MarketSeries.Close.Last(2))
                {
                    gi_21 = OrderSend(TradeType.Buy, fer(FirstVolume, 0));
                    if (gi_21 > 0)
                        tc_31 = MarketSeries.OpenTime.Last(0);
                    else
                        Print("First BUY openning error at: ", Symbol.Ask, "Error Type: ", LastResult.Error);
                }
                if (Sell && o_tm(TradeType.Sell) == 0 && MarketSeries.Close.Last(2) > MarketSeries.Close.Last(1))
                {
                    gi_21 = OrderSend(TradeType.Sell, fer(FirstVolume, 0));
                    if (gi_21 > 0)
                        tc_32 = MarketSeries.OpenTime.Last(0);
                    else
                        Print("First SELL openning error at: ", Symbol.Bid, "Error Type: ", LastResult.Error);
                }
            }
            N_28();
        }
        private void N_28()
        {
            if (o_tm(TradeType.Buy) > 0)
            {
                if (Math.Round(Symbol.Ask, Symbol.Digits) < Math.Round(D_TD(TradeType.Buy) - PipStep * Symbol.PipSize, Symbol.Digits) && tc_31 != MarketSeries.OpenTime.Last(0))
                {
                    long gl_57 = n_lt(TradeType.Buy);
                    gi_21 = OrderSend(TradeType.Buy, fer(gl_57, 2));
                    if (gi_21 > 0)
                        tc_31 = MarketSeries.OpenTime.Last(0);
                    else
                        Print("Next BUY openning error at: ", Symbol.Ask, "Error Type: ", LastResult.Error);
                }
            }
            if (o_tm(TradeType.Sell) > 0)
            {
                if (Math.Round(Symbol.Bid, Symbol.Digits) > Math.Round(U_TD(TradeType.Sell) + PipStep * Symbol.PipSize, Symbol.Digits))
                {
                    long gl_59 = n_lt(TradeType.Sell);
                    gi_21 = OrderSend(TradeType.Sell, fer(gl_59, 2));
                    if (gi_21 > 0)
                        tc_32 = MarketSeries.OpenTime.Last(0);
                    else
                        Print("Next SELL openning error at: ", Symbol.Bid, "Error Type: ", LastResult.Error);
                }
            }
        }
        private int OrderSend(TradeType TrdTp, long iVol)
        {
            int cd_8 = 0;
            if (iVol > 0)
            {
                TradeResult result = ExecuteMarketOrder(TrdTp, Symbol, iVol, Label, 0, 0, 0, "smart_grid");

                if (result.IsSuccessful)
                {
                    Print(TrdTp, "Opened at: ", result.Position.EntryPrice);
                    cd_8 = 1;
                }
                else
                    Print(TrdTp, "Openning Error: ", result.Error);
            }
            else
                Print("Volume calculation error: Calculated Volume is: ", iVol);
            return cd_8;
        }
        private void f0_86(double ai_4, int ad_8)
        {
            foreach (var position in Positions)
            {
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == TradeType.Buy)
                    {
                        double? li_16 = Math.Round(ai_4 + ad_8 * Symbol.PipSize, Symbol.Digits);
                        if (position.TakeProfit != li_16)
                            ModifyPosition(position, position.StopLoss, li_16);
                    }
                }
            }
        }
        private void f0_88(double ai_4, int ad_8)
        {
            foreach (var position in Positions)
            {
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == TradeType.Sell)
                    {
                        double? li_16 = Math.Round(ai_4 - ad_8 * Symbol.PipSize, Symbol.Digits);
                        if (position.TakeProfit != li_16)
                            ModifyPosition(position, position.StopLoss, li_16);
                    }
                }
            }
        }
        private void RCN()
        {
            if (o_tm(TradeType.Buy) > 1)
            {
                double y = pnt_12(TradeType.Buy);
                ChartObjects.DrawHorizontalLine("bpoint", y, Colors.Yellow, 2, LineStyle.Dots);
            }
            else
                ChartObjects.RemoveObject("bpoint");
            if (o_tm(TradeType.Sell) > 1)
            {
                double z = pnt_12(TradeType.Sell);
                ChartObjects.DrawHorizontalLine("spoint", z, Colors.HotPink, 2, LineStyle.Dots);
            }
            else
                ChartObjects.RemoveObject("spoint");
            ChartObjects.DrawText("pan", A_cmt_calc(), StaticPosition.TopLeft, Colors.Tomato);
        }
        private string A_cmt_calc()
        {
            string gc_78 = "";
            string wn_7 = "";
            string wn_8 = "";
            string sp_4 = "";
            string ppb = "";
            string lpb = "";
            string nb_6 = "";
            double dn_7 = 0;
            double dn_9 = 0;
            sp_4 = "\nSpread = " + Math.Round(sp_d, 1);
            nb_6 = "\nwww.facebook.com/cls.fx\n";
            if (dn_7 > 0)
                wn_7 = "\nBuy Positions = " + o_tm(TradeType.Buy);
            if (dn_9 > 0)
                wn_8 = "\nSell Positions = " + o_tm(TradeType.Sell);
            if (o_tm(TradeType.Buy) > 0)
            {
                double igl = Math.Round((pnt_12(TradeType.Buy) - Symbol.Bid) / Symbol.PipSize, 1);
                ppb = "\nBuy Target Away = " + igl;
            }
            if (o_tm(TradeType.Sell) > 0)
            {
                double osl = Math.Round((Symbol.Ask - pnt_12(TradeType.Sell)) / Symbol.PipSize, 1);
                lpb = "\nSell Target Away = " + osl;
            }
            if (sp_d > MaxSpread)
                gc_78 = "MAX SPREAD EXCEED";
            else
                gc_78 = "Smart Grid" + nb_6 + wn_7 + sp_4 + wn_8 + ppb + lpb;
            return (gc_78);
        }
        private int cnt_16()
        {
            int ASide = 0;

            for (int i = Positions.Count - 1; i >= 0; i--)
            {
                position = Positions[i];
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                    ASide++;
            }
            return ASide;
        }
        private int o_tm(TradeType TrdTp)
        {
            int TSide = 0;

            for (int i = Positions.Count - 1; i >= 0; i--)
            {
                position = Positions[i];
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == TrdTp)
                        TSide++;
                }
            }
            return TSide;
        }
        private double pnt_12(TradeType TrdTp)
        {
            double Result = 0;
            double AveragePrice = 0;
            long Count = 0;

            for (int i = Positions.Count - 1; i >= 0; i--)
            {
                position = Positions[i];
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == TrdTp)
                    {
                        AveragePrice += position.EntryPrice * position.Volume;
                        Count += position.Volume;
                    }
                }
            }
            if (AveragePrice > 0 && Count > 0)
                Result = Math.Round(AveragePrice / Count, Symbol.Digits);
            return Result;
        }
        private double D_TD(TradeType TrdTp)
        {
            double D_TD = 0;

            for (int i = Positions.Count - 1; i >= 0; i--)
            {
                position = Positions[i];
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == TrdTp)
                    {
                        if (D_TD == 0)
                        {
                            D_TD = position.EntryPrice;
                            continue;
                        }
                        if (position.EntryPrice < D_TD)
                            D_TD = position.EntryPrice;
                    }
                }
            }
            return D_TD;
        }
        private double U_TD(TradeType TrdTp)
        {
            double U_TD = 0;

            for (int i = Positions.Count - 1; i >= 0; i--)
            {
                position = Positions[i];
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == TrdTp)
                    {
                        if (U_TD == 0)
                        {
                            U_TD = position.EntryPrice;
                            continue;
                        }
                        if (position.EntryPrice > U_TD)
                            U_TD = position.EntryPrice;
                    }
                }
            }
            return U_TD;
        }
        private double f_tk(TradeType TrdTp)
        {
            double prc_4 = 0;
            int tk_4 = 0;
            for (int i = Positions.Count - 1; i >= 0; i--)
            {
                position = Positions[i];
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == TrdTp)
                    {
                        if (tk_4 == 0 || tk_4 > position.Id)
                        {
                            prc_4 = position.EntryPrice;
                            tk_4 = position.Id;
                        }
                    }
                }
            }
            return prc_4;
        }
        private long lt_8(TradeType TrdTp)
        {
            long lot_4 = 0;
            int tk_4 = 0;
            for (int i = Positions.Count - 1; i >= 0; i--)
            {
                position = Positions[i];
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == TrdTp)
                    {
                        if (tk_4 == 0 || tk_4 > position.Id)
                        {
                            lot_4 = position.Volume;
                            tk_4 = position.Id;
                        }
                    }
                }
            }
            return lot_4;
        }
        private long clt(TradeType TrdTp)
        {
            long Result = 0;
            for (int i = Positions.Count - 1; i >= 0; i--)
            {
                position = Positions[i];
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == TrdTp)
                        Result += position.Volume;
                }
            }
            return Result;
        }
        private int Grd_Ex(TradeType ai_0, TradeType ci_0)
        {
            double prc_4 = f_tk(ci_0);
            int tk_4 = 0;
            for (int i = Positions.Count - 1; i >= 0; i--)
            {
                position = Positions[i];
                if (position.Label == Label && position.SymbolCode == Symbol.Code)
                {
                    if (position.TradeType == ai_0 && ai_0 == TradeType.Buy)
                    {
                        if (Math.Round(position.EntryPrice, Symbol.Digits) <= Math.Round(prc_4, Symbol.Digits))
                            tk_4++;
                    }
                    if (position.TradeType == ai_0 && ai_0 == TradeType.Sell)
                    {
                        if (Math.Round(position.EntryPrice, Symbol.Digits) >= Math.Round(prc_4, Symbol.Digits))
                            tk_4++;
                    }
                }
            }
            return (tk_4);
        }
        private long n_lt(TradeType ca_8)
        {
            int ic_g = Grd_Ex(ca_8, ca_8);
            long gi_c = lt_8(ca_8);
            long ld_4 = Symbol.NormalizeVolume(gi_c * Math.Pow(VolumeExponent, ic_g));
            return (ld_4);
        }
        private long fer(long ic_9, int bk_4)
        {
            long ga_i = Symbol.VolumeMin;
            long gd_i = Symbol.VolumeStep;
            long dc_i = Symbol.VolumeMax;
            long ic_8 = ic_9;
            if (ic_8 < ga_i)
                ic_8 = ga_i;
            if (ic_8 > dc_i)
                ic_8 = dc_i;
            return (ic_8);
        }
    }
}


