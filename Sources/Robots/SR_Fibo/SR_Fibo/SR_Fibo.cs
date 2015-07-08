//#reference: ..\Indicators\FibonacciBands.algo
//#reference: ..\Indicators\Velocidade.algo
// -------------------------------------------------------------------------------
//
//    This is a Template used as a guideline to build your own Robot. 
//
// -------------------------------------------------------------------------------


using System;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Requests;
using cAlgo.Indicators;
using System.IO;
using System.Collections.Generic;

using System.Linq;
using System.Runtime.InteropServices;
using System.Globalization;


namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC)]
    public class SR_Fibo : Robot
    {
        [Parameter(DefaultValue = 100000)]
        public int Volume { get; set; }
        [Parameter(DefaultValue = 21)]
        public int AtrPeriod { get; set; }
        [Parameter(DefaultValue = 55)]
        public int emaPeriod { get; set; }
        [Parameter(DefaultValue = 10)]
        public int velPeriod { get; set; }

        List<double> fibo;
        int idx;

        Velocidade tendencia;
        FibonacciBands fbands;

        double stop, gain, price;
        PendingOrder ordemc, ordemv;
        Position posicao;
        private void seta_variaveis(TradeType oper)
        {
            carregar_fibo();

            if (oper == TradeType.Buy)
                price = precos(fibo, Symbol.Ask, oper);
            else
                price = precos(fibo, Symbol.Bid, oper);
            stop = stopcalc(oper, fibo);
            gain = gaincalc(oper, fibo);

        }

        protected override void OnPendingOrderCreated(PendingOrder newPendingOrder)
        {
            if (newPendingOrder.TradeType == TradeType.Sell)
                ordemv = newPendingOrder;
            else
                ordemc = newPendingOrder;

        }

        protected override void OnBar()
        {
            carregar_fibo();
            Trade.DeletePendingOrder(ordemv);
            Trade.DeletePendingOrder(ordemc);

            if (tendencia.velocidade.LastValue > 0 && Functions.IsRising(tendencia.velocidade))
                abreposicoes(TradeType.Buy);
            if (tendencia.velocidade.LastValue < 0 && Functions.IsFalling(tendencia.velocidade))
                abreposicoes(TradeType.Sell);



        }
        protected override void OnStart()
        {

            tendencia = Indicators.GetIndicator<Velocidade>(MarketSeries.Close, velPeriod);
            fbands = Indicators.GetIndicator<FibonacciBands>(emaPeriod, AtrPeriod);

            fibo = new List<double>();
            carregar_fibo();
            organizarVetores();
            abreposicoes(TradeType.Buy);
            abreposicoes(TradeType.Sell);
            Print("OK!!!");
        }

        protected override void OnTick()
        {

        }
        private void organizarVetores()
        {
            fibo.Sort();
            fibo.Reverse();


        }

        private void carregar_fibo()
        {
            fibo.Clear();
            fibo.Add(fbands.UpperBand4.LastValue);
            fibo.Add(fbands.UpperBand3.LastValue);
            fibo.Add(fbands.UpperBand2.LastValue);
            fibo.Add(fbands.UpperBand1.LastValue);
            fibo.Add(fbands.LowerBand1.LastValue);
            fibo.Add(fbands.LowerBand2.LastValue);
            fibo.Add(fbands.LowerBand3.LastValue);
            fibo.Add(fbands.LowerBand4.LastValue);
        }

        private void abreposicoes(TradeType oper)
        {

            seta_variaveis(oper);
            if (oper == TradeType.Buy)
                Trade.CreateBuyLimitOrder(Symbol, Volume, price, stop, gain, null);
            if (oper == TradeType.Sell)
            {
                Trade.CreateSellLimitOrder(Symbol, Volume, price, stop, gain, null);

            }
        }

        private double stopcalc(TradeType oper, List<double> sr)
        {
            organizarVetores();
            if (oper == TradeType.Sell)
                sr.Sort();
            if (idx < sr.Count - 1 && Math.Abs(sr[idx] - Symbol.Bid) / Symbol.PipSize > 5)
                return sr[idx + 1];
            else if (oper == TradeType.Buy)
                return sr[idx] - 20 * Symbol.PipSize;
            else if (oper == TradeType.Sell)
                return sr[idx] + 20 * Symbol.PipSize;



            return 0;
        }

        private double gaincalc(TradeType oper, List<double> sr)
        {
            organizarVetores();
            if (oper == TradeType.Sell)
                sr.Sort();

            return sr[idx - 1];
        }




        private double precos(List<double> sr, double price, TradeType oper)
        {
            organizarVetores();
            if (oper == TradeType.Buy)
            {
                for (var i = 1; i < sr.Count() - 1; i++)
                {
                    if (tendencia.velocidade.LastValue > 0 && price - sr[i] > 10 * Symbol.PipSize)
                        if (sr[i] < price)
                        {
                            idx = i;
                            return sr[i];
                            break;
                        }


                    if (tendencia.velocidade.LastValue < 0 && price - sr[i] > 10 * Symbol.PipSize)
                        if (sr[i] < price)
                        {

                            idx = i + 1;
                            return sr[i + 1];
                            break;


                        }

                }

            }
            if (oper == TradeType.Sell)
            {

                sr.Sort();

                for (var i = 1; i < sr.Count() - 1; i++)
                {

                    if (tendencia.velocidade.LastValue < 0 && sr[i] - price > 10 * Symbol.PipSize)
                    {

                        if (sr[i] > price)
                        {

                            idx = i;
                            return sr[i];
                            break;
                        }
                    }
                    if (tendencia.velocidade.LastValue > 0 && sr[i] - price > 10 * Symbol.PipSize)
                        if (sr[i] > price)
                        {

                            idx = i + 1;
                            return sr[i + 1];
                            break;


                        }
                }

            }

            return 0;
        }


        protected override void OnPositionOpened(Position posicaoaberta)
        {
            posicao = posicaoaberta;
            Print("Nova posição aberta\n");

        }

        protected override void OnPositionClosed(Position posicaoaberta)
        {
            posicao = null;


        }
        protected override void OnStop()
        {
            Trade.DeletePendingOrder(ordemv);
            Trade.DeletePendingOrder(ordemc);
        }
    }
}
