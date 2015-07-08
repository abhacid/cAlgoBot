using System;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class PayBack : Robot
    {

        [Parameter("Start Buy", DefaultValue = true)]
        public bool Buy { get; set; }

        [Parameter("Change the direction", DefaultValue = true)]
        public bool change1 { get; set; }

        [Parameter("Start Automate Buy", DefaultValue = false)]
        public bool StartAutomate1 { get; set; }

        [Parameter("Volume Buy", DefaultValue = 10000, MinValue = 0)]
        public int InitialVolume { get; set; }

        [Parameter("Multiplier", DefaultValue = 2.1)]
        public double Multiplier { get; set; }

        [Parameter("Stop Loss", DefaultValue = 40)]
        public double StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 40)]
        public double TakeProfit { get; set; }

        ///////////////////////////////////////////////////////

        [Parameter("SETTING SELL", DefaultValue = "___SELL___")]
        public string Separator { get; set; }

        //////////////////////////////////////////////////////

        [Parameter("Start Sell", DefaultValue = true)]
        public bool Sell { get; set; }

        [Parameter("change the direction", DefaultValue = true)]
        public bool change2 { get; set; }

        [Parameter("Start Automate Sell", DefaultValue = false)]
        public bool StartAutomate2 { get; set; }

        [Parameter("Volume Sell", DefaultValue = 10000, MinValue = 0)]
        public int InitialVolume2 { get; set; }

        [Parameter("Multiplier", DefaultValue = 2.1)]
        public double Multiplier2 { get; set; }

        [Parameter("Stop Loss", DefaultValue = 40)]
        public double StopLoss2 { get; set; }

        [Parameter("Take Profit", DefaultValue = 40)]
        public double TakeProfit2 { get; set; }

        protected override void OnStart()
        {

            string text = "PayBacK  By TraderMatriX";

            base.ChartObjects.DrawText("PayBacK  By TraderMatriX", text, StaticPosition.TopCenter, new Colors?(Colors.Lime));


            Positions.Closed += OnPositionsClosed1;
            Positions.Closed += OnPositionsClosed2;
            Positions.Closed += OnPositionsClosed3;
            Positions.Closed += OnPositionsClosed4;

            buy();
            sell();
        }

        protected override void OnTick()
        {


            var netProfit = 0.0;


            foreach (var openedPosition in Positions)
            {

                netProfit += openedPosition.NetProfit + openedPosition.Commissions;
                ;

            }

            ChartObjects.DrawText("a", netProfit.ToString(), StaticPosition.BottomRight, new Colors?(Colors.Lime));



        }


        private void buy()
        {

            if (Buy == true)

                ExecuteMarketOrder(TradeType.Buy, Symbol, InitialVolume, "buy", StopLoss, TakeProfit);
        }
        private void sell()
        {
            if (Sell == true)

                ExecuteMarketOrder(TradeType.Sell, Symbol, InitialVolume2, "sell", StopLoss2, TakeProfit2);


        }


        private void OnPositionsClosed1(PositionClosedEventArgs args)
        {
            if (StartAutomate1 == true)
            {

                Print("Closed");

                var position = args.Position;

                if (position.Label != "buy" || position.SymbolCode != Symbol.Code)
                    return;


                if (position.Pips > 0)
                    buy();
                {




                    if (position.GrossProfit < 0)
                    {

                        if (change1 == true)
                        {
                            TradeType AA = TradeType.Sell;

                            if (position.TradeType == TradeType.Sell)

                                AA = TradeType.Buy;


                            ExecuteMarketOrder(AA, Symbol, Symbol.NormalizeVolume(position.Volume * Multiplier), "buy", StopLoss, TakeProfit);

                        }

                        else if (change1 == false)
                        {
                            TradeType BB = TradeType.Sell;



                            BB = TradeType.Buy;


                            ExecuteMarketOrder(BB, Symbol, Symbol.NormalizeVolume(position.Volume * Multiplier), "buy", StopLoss, TakeProfit);




                        }
                    }
                }
            }
        }
        private void OnPositionsClosed2(PositionClosedEventArgs args)
        {

            if (StartAutomate2 == true)
            {


                Print("Closed");

                var position = args.Position;

                if (position.Label != "sell" || position.SymbolCode != Symbol.Code)
                    return;


                if (position.Pips > 0)
                    sell();
                {


                    if (position.GrossProfit < 0)
                    {

                        if (change2 == true)
                        {
                            TradeType AA = TradeType.Sell;

                            if (position.TradeType == TradeType.Sell)

                                AA = TradeType.Buy;


                            ExecuteMarketOrder(AA, Symbol, Symbol.NormalizeVolume(position.Volume * Multiplier2), "sell", StopLoss2, TakeProfit2);

                        }

                        else if (change2 == false)
                        {
                            TradeType BB = TradeType.Buy;



                            BB = TradeType.Sell;


                            ExecuteMarketOrder(BB, Symbol, Symbol.NormalizeVolume(position.Volume * Multiplier2), "sell", StopLoss2, TakeProfit2);




                        }
                    }
                }
            }
        }


        private void OnPositionsClosed3(PositionClosedEventArgs args)
        {
            if (StartAutomate1 == false)
            {

                Print("Closed");

                var position = args.Position;

                if (position.Label != "buy" || position.SymbolCode != Symbol.Code)
                    return;


                if (position.Pips > 0)
                    return;
                {

                    if (position.GrossProfit < 0)
                    {

                        if (change1 == true)
                        {
                            TradeType AA = TradeType.Sell;

                            if (position.TradeType == TradeType.Sell)

                                AA = TradeType.Buy;


                            ExecuteMarketOrder(AA, Symbol, Symbol.NormalizeVolume(position.Volume * Multiplier), "buy", StopLoss, TakeProfit);

                        }


                        else if (change1 == false)
                        {
                            TradeType BB = TradeType.Sell;



                            BB = TradeType.Buy;


                            ExecuteMarketOrder(BB, Symbol, Symbol.NormalizeVolume(position.Volume * Multiplier), "buy", StopLoss, TakeProfit);




                        }
                    }
                }
            }
        }



        private void OnPositionsClosed4(PositionClosedEventArgs args)
        {

            if (StartAutomate2 == false)
            {


                Print("Closed");

                var position = args.Position;

                if (position.Label != "sell" || position.SymbolCode != Symbol.Code)
                    return;


                if (position.Pips > 0)
                    return;
                {

                    if (position.GrossProfit < 0)
                    {

                        if (change2 == true)
                        {
                            TradeType AA = TradeType.Sell;

                            if (position.TradeType == TradeType.Sell)

                                AA = TradeType.Buy;


                            ExecuteMarketOrder(AA, Symbol, Symbol.NormalizeVolume(position.Volume * Multiplier2), "sell", StopLoss2, TakeProfit2);

                        }


                        else if (change2 == false)
                        {
                            TradeType BB = TradeType.Buy;



                            BB = TradeType.Sell;


                            ExecuteMarketOrder(BB, Symbol, Symbol.NormalizeVolume(position.Volume * Multiplier2), "sell", StopLoss2, TakeProfit2);




                        }
                    }
                }
            }
        }

    }
}
