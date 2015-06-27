// -------------------------------------------------------------------------------
//
//		TrailCut-I (17 juillet 2014)
//		version 1.2014.7.17.23h
//		Author : https://www.facebook.com/ab.hacid
//
//	Utiliser :
//			Symbol				=	GBPUSD
//			TimeFrame			=	m30
//			Volume				=	100000
//          SL					=	57 pips
//          TP					=	300 pips
//			Martingale			=	Non
//			TrailStart			=	30
//			TrailStep			=	4
//			PeriodWPR           =   14
//			commission			=	37.6 per Million
//			Spread fixe			=	1pip
//			Starting Capital	=	50000
//
//	Results :
//          Resultats			=	entre le 1/1/2014 et 17/7/2014 a 23:53 gain de 6904 euros(+16%).
//			Net profit			=	7889.76 euros
//			Ending Equity		=	7888.76 euros
//			Ratio de Sharpe		=	0.37
//			Ratio de Storino	=	0.45
// -------------------------------------------------------------------------------

#region advertisement
// -------------------------------------------------------------------------------
//			Trading using leverage carries a high degree of risk to your capital, and it is possible to lose more than
//			your initial investment. Only speculate with money you can afford to lose.
// -------------------------------------------------------------------------------
#endregion


using System;
using cAlgo.API;
using cAlgo;
using cAlgo.Indicators;

namespace cAlgo.Robots
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class TrailCut : Robot
    {
        #region cBot Parameters
        [Parameter("Volume", DefaultValue = 100000, MinValue = 0)]
        public int InitialVolume { get; set; }

        [Parameter("Stop Loss", DefaultValue = 57)]
        public int StopLoss { get; set; }

        [Parameter("Take Profit", DefaultValue = 300)]
        public int TakeProfit { get; set; }

        [Parameter("Martingale", DefaultValue = 0)]
        public bool Martingale { get; set; }

        [Parameter("TrailStart", DefaultValue = 30, MinValue = 1)]
        public int TrailStart { get; set; }

        [Parameter("TrailStep", DefaultValue = 4, MinValue = 1)]
        public int TrailStep { get; set; }

        [Parameter("PeriodWPR", DefaultValue = 14, MinValue = 1)]
        public int PeriodWPR { get; set; }
        #endregion

        #region cBot globals
        // Slippage maximun en point, si l'execution de l'ordre impose un slippage superieur, l'ordre n'est pas execute.
        private const double slippage = 2;
        // Nom du robot
        private const string botName = "TrailCut-I";
        // Prefixe des ordres passes par le robot
        private const string botPrefix = "TCI";
        // Label des  ordres passes par le robot
        private string botLabel;

        private const int Buy = 1;
        private const int Sell = -1;

        private WPRIndicator wpr;
        #endregion

        #region cBot Events
        protected override void OnStart()
        {
            botLabel = string.Format("{0}-{1} {2}", botPrefix, Symbol.Code, TimeFrame);

            wpr = Indicators.GetIndicator<WPRIndicator>(PeriodWPR);

            Positions.Opened += OnPositionOpened;
            Positions.Closed += OnPositionClosed;

            controlBuyAndSell();
        }

        protected void OnPositionOpened(PositionOpenedEventArgs args)
        {
        }

        // Gere une Martingale selective.
        protected void OnPositionClosed(PositionClosedEventArgs args)
        {
            Position position = args.Position;
            bool isBuy = TradeType.Buy == position.TradeType;

            if (Martingale && (position.Pips < 0))
                splitAndExecuteOrder(position.TradeType == TradeType.Buy ? TradeType.Sell : TradeType.Buy, position.Volume * 1.2, botPrefix + "Mart-");

            Print("{0}, Volume : {1}, G/P : {2}, open : {3}, close : {4}, {5}", Symbol.Code, position.Volume, position.Pips, position.EntryPrice, isBuy ? Symbol.Bid : Symbol.Ask, position.Id);

        }

        // Controle des positions et relance.        
        protected override void OnTick()
        {
            controlClose();

            controlTrail();


        }

        // A Chaque nouvelle bougie on peut evaluer la possibilite d'achat ou de vente ou de neutralite
        protected override void OnBar()
        {
            controlBuyAndSell();
        }

        protected override void OnError(Error error)
        {
            if (error.Code == ErrorCode.NoMoney)
                Print("ERROR!!! No money for order open");
            else if (error.Code == ErrorCode.BadVolume)
                Print("ERROR!!! Bad volume for order open");
        }
        #endregion


        // Gere les cloture des positions pertes selon l'adage laisser courrir les gains, cloturer les pertes.
        private void controlClose()
        {

            foreach (var position in Positions.FindAll(botLabel, Symbol))
            {
                if (position.TakeProfit.HasValue && position.StopLoss.HasValue)
                {
                    string label = position.Comment.Substring(position.Comment.Length - 1, 1);

                    bool isBuy = TradeType.Buy == position.TradeType;
                    double profitLoss = isBuy ? Symbol.Bid - position.EntryPrice : position.EntryPrice - Symbol.Ask;
                    double potentialProfit = isBuy ? Symbol.Bid - position.EntryPrice : position.EntryPrice - Symbol.Ask;
                    double potentialLoss = isBuy ? Symbol.Bid - position.StopLoss.Value : position.StopLoss.Value - Symbol.Ask;

                    double percentGain = profitLoss / potentialProfit;
                    double percentLoss = profitLoss / potentialLoss;

                    if ((percentLoss <= -0.33) && (label == "1"))
                        ClosePosition(position);

                    if ((percentLoss <= -0.66) && (label == "2"))
                        ClosePosition(position);
                }
            }


        }

        // Decoupe un ordre en trois ordres de volume 1/2, 3/10, 2/10 du volume initial demande.
        private void splitAndExecuteOrder(TradeType tradeType, double volume, string prefixLabel)
        {
            const double percent1 = 0.5;
            const double percent2 = 0.3;
            const double percent3 = 1 - percent1 - percent2;

            long volume1 = (long)Math.Floor(volume * percent1);
            long volume2 = (long)Math.Floor(volume * percent2);
            long volume3 = (long)Math.Floor(volume * percent3);

            executeOrder(tradeType, volume1, String.Format("{0}-{1}-1", prefixLabel, tradeType));
            executeOrder(tradeType, volume2, String.Format("{0}-{1}-2", prefixLabel, tradeType));
            executeOrder(tradeType, volume3, String.Format("{0}-{1}-3", prefixLabel, tradeType));

        }

        private void executeOrder(TradeType tradeType, long volume, string label)
        {
            if (volume <= 0)
                return;

            // il faut que le volume soit un multiple de "microVolume".
            long v = Symbol.NormalizeVolume(volume, RoundingMode.ToNearest);

            if (v > 0)
            {
                var result = ExecuteMarketOrder(tradeType, Symbol, v, botName, StopLoss, TakeProfit, slippage, label);
                if (!result.IsSuccessful)
                    Print("error : {0}, {1}", result.Error, v);
            }
        }

        // Gere la prise de position
        private void controlBuyAndSell()
        {
            int signal = signalStrategie();

            if (signal != 0)
                splitAndExecuteOrder((signal == 1) ? TradeType.Buy : TradeType.Sell, InitialVolume, botLabel);
        }



        // Gere le stop suiveur dynamique
        private void controlTrail()
        {
            foreach (Position position in Positions)
            {
                if (position.Pips > TrailStart)
                {
                    bool isBuy = TradeType.Buy == position.TradeType;
                    double? actualPotentialLoss = isBuy ? Symbol.Bid - position.StopLoss : position.StopLoss - Symbol.Ask;
                    double? newStopLoss = position.StopLoss + (isBuy ? 1 : -1) * TrailStep * Symbol.PipSize;

                    if (actualPotentialLoss > TrailStep * Symbol.PipSize)
                    {
                        ModifyPosition(position, newStopLoss, position.TakeProfit);
                    }
                }

            }

        }



        // STRATEGIE DE TRADING
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // Renvoie un signal d'achat, de vente ou neutre. C'est ici qu'il faut ecrire la strategie de declanchement des ordres
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private int signalStrategie()
        {
            // retourne 
            //  0 : pas de signal (neutre), 
            //  1 : achat, 
            // -1: vente



            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            // Strategie : deux bougies haussieres donne un signal d'achat, deux bougies baissieres un signal de vente
            /////////////////////////////////////////////////////////////////////////////////////////////////////////////////

            //double step = 7 * Symbol.PipSize;
            //int LastBarIndex = MarketSeries.Close.Count - 2;
            //int PrevBarIndex = LastBarIndex - 1;

            //if ((MarketSeries.Close[LastBarIndex] > MarketSeries.Open[LastBarIndex]+step) && (MarketSeries.Close[PrevBarIndex] > MarketSeries.Open[PrevBarIndex]+step))
            //	return Buy;

            //if ((MarketSeries.Close[LastBarIndex] +step < MarketSeries.Open[LastBarIndex]) && (MarketSeries.Close[PrevBarIndex] +step< MarketSeries.Open[PrevBarIndex]))
            //	return Sell;

            //return 0;



            ///////////////////////////////////////////////////////
            // Strategie selon indicateur Williams Percent Range
            ///////////////////////////////////////////////////////
            //if (wpr.Result[wpr.Result.Count - 1] < -95)
            //	foreach (Position position in Positions.FindAll(botName, Symbol, TradeType.Sell)) 
            //		ClosePosition(position);

            //if (wpr.Result[wpr.Result.Count - 1] > -5)
            //	foreach (Position position in Positions.FindAll(botName, Symbol, TradeType.Buy))
            //		ClosePosition(position);


            if ((wpr.Result[wpr.Result.Count - 2] < -80) && (wpr.Result[wpr.Result.Count - 1] > -80))
            {
                foreach (Position position in Positions.FindAll(botName, Symbol, TradeType.Sell))
                    ClosePosition(position);

                return Buy;
            }
            else if ((wpr.Result[wpr.Result.Count - 2] > -20) && (wpr.Result[wpr.Result.Count - 1] < -20))
            {
                foreach (Position position in Positions.FindAll(botName, Symbol, TradeType.Buy))
                    ClosePosition(position);

                return Sell;
            }

            return 0;
        }
    }
}


