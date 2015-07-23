/*
  DreamzFX Position Manager
  http://dreamzfx.net
  Version: 1.0.1
*/

using System;
using System.Threading;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Indicators;
using cAlgo.API.Internals;
using cAlgo.Indicators;
using System.Collections.Generic;
using System.IO;

namespace cAlgo
{
    [Robot(TimeZone = TimeZones.UTC, AccessRights = AccessRights.FileSystem)]
    public class TradingHelper : Robot
    {
        [Parameter("StopLoss Initial", DefaultValue = 0, MinValue = 0)]
        public int StopLoss_Initial { get; set; }

        [Parameter("Take Profit Initial", DefaultValue = 0, MinValue = 0)]
        public int TakeProfit_Initial { get; set; }

        [Parameter("BreakEven After", DefaultValue = 0, MinValue = 0)]
        public int BreakEven_After { get; set; }

        [Parameter("BreakEven Profit Pips", DefaultValue = 2, MinValue = 0)]
        public int BreakEven_Profit { get; set; }

        [Parameter("BreakEven2 After", DefaultValue = 0, MinValue = 0)]
        public int BreakEven2_After { get; set; }

        [Parameter("BreakEven2 Profit Pips", DefaultValue = 0, MinValue = 0)]
        public int BreakEven2_Profit { get; set; }

        [Parameter("Jumping StopLoss", DefaultValue = 0, MinValue = 0)]
        public int Jumping_Stop { get; set; }

        [Parameter("Jumping Stop Distance", DefaultValue = 0, MinValue = 0)]
        public int Jumping_Distance { get; set; }

        [Parameter("Take Profit 1", DefaultValue = 0, MinValue = 0)]
        public int TakeProfit_1 { get; set; }

        [Parameter("Manage This Pair Only", DefaultValue = true)]
        public bool ManageThisPairOnly { get; set; }

        [Parameter("Manage By Comment", DefaultValue = false)]
        public bool ManageByComment { get; set; }

        [Parameter("Comment to Manage", DefaultValue = null)]
        public string ManagedComment { get; set; }

        [Parameter("Ignore Comment", DefaultValue = false)]
        public bool IgnoreByComment { get; set; }

        [Parameter("Comment to Ignore", DefaultValue = null)]
        public string IgnoredComment { get; set; }

        [Parameter("Manage by (0-both 1-buy 2-sell)", DefaultValue = 0)]
        public int Trade_Type { get; set; }

        /* Variables */
        private double last = 0;
        private int ticks = 10;
        public int i;
        public double Bid, Ask;
        public Symbol symbol;
        public List<int> Partial_Positions = new List<int>();
        TradeResult result;
        string FileName = "PosManager_Partials.txt";

        protected override void OnStart()
        {
            // Validate Parameters
            CheckParameters();

            // If running in Backtester, open a couple of positions
            if (IsBacktesting)
            {
                ExecuteMarketOrder(TradeType.Buy, Symbol, 2000, null, 0, 0, 0, "test");
                ExecuteMarketOrder(TradeType.Sell, Symbol, 2000);
            }
            Positions.Opened += OnPositionsOpened;
            Positions.Closed += OnPositionsClosed;
            FileRead();
            Timer.Start(1);
        }

        public void FileRead()
        {
            if (!File.Exists(FileName))
                File.Create(FileName);
            else
            {
                // Dump "FileName" into array
                string[] Ids = System.IO.File.ReadAllLines(FileName);

                File.Delete(FileName);

                StreamWriter _fileWriter;
                _fileWriter = File.AppendText(FileName);
                _fileWriter.AutoFlush = true;

                // Check which Positions from the File are Still active
                foreach (Position position in Positions)
                    foreach (string Id in Ids)
                        if (position.Id == Convert.ToInt32(Id))
                        {
                            // If the Position Exists, Add it to the Partial_Positions list and to the file
                            Partial_Positions.Add(Convert.ToInt32(Id));
                            _fileWriter.WriteLine(Id);
                            break;
                        }
                _fileWriter.Close();
            }
            for (i = 0; i < Partial_Positions.Count; i++)
                Print(Partial_Positions[i]);
        }

        protected override void OnTimer()
        {
            ticks = 0;
            OnTick();
        }

        private void OnPositionsOpened(PositionOpenedEventArgs args)
        {
            if (args.Position.SymbolCode == Symbol.Code)
            {
                ticks = 0;
                OnTick();
            }
        }

        private void OnPositionsClosed(PositionClosedEventArgs args)
        {
            // If position being closed is in the Partial_Positions list, remove it from the list
            for (i = 0; i < Partial_Positions.Count; i++)
                if (Partial_Positions[i] == args.Position.Id)
                    Partial_Positions.RemoveAt(i);
        }

        protected override void OnTick()
        {
            if (Monitor.TryEnter(this))
            {
                try
                {
                    DoWork();
                } finally
                {
                    Monitor.Exit(this);
                }
            }
        }

        // Main function to process positions
        private void DoWork()
        {
            if (--ticks > 0 && Math.Abs(last - MarketSeries.Close.LastValue) < symbol.PipSize / 2)
                return;

            RefreshData();

            ticks = 10;
            last = MarketSeries.Close.LastValue;

            foreach (Position position in Positions)
            {
                if (!ManageThisPosition(position))
                    continue;

                symbol = MarketData.GetSymbol(position.SymbolCode);
                Bid = symbol.Bid;
                Ask = symbol.Ask;

                // Set initial stops if necessary
                if ((StopLoss_Initial > 0 && position.StopLoss == null) || (TakeProfit_Initial > 0 && position.TakeProfit == null))
                    SetStops(position);

                // BreakEven
                if (BreakEven_After > 0 && position.Pips >= BreakEven_After)
                    BreakEven(position, BreakEven_Profit);

                // BreakEven2
                if (BreakEven2_After > 0 && position.Pips >= BreakEven2_After)
                    BreakEven(position, BreakEven2_Profit);

                // Partial Closure at Take Profit 1
                if (TakeProfit_1 > 0)
                    TakeProfit1(position);

                // Jumping StopLoss. SL jumps every X pips. Also keeps a distance of Y pips below market price before setting a new SL
                // Only triggers if we're in profit
                if (Jumping_Stop > 0 && position.Pips >= Jumping_Stop + Jumping_Distance && position.Pips >= 0)
                    JumpingStop(position);
            }
        }

        // Basic Parameter Check
        private void CheckParameters()
        {
            string msg = null;
            if (BreakEven_After != 0 && BreakEven_After < BreakEven_Profit)
            {
                msg = "ERROR: 'BreakEven_After' value should be greater than 'BreakEven_Profit";
                Print(msg);
            }
            if (BreakEven2_After != 0 && BreakEven2_After < BreakEven2_Profit)
            {
                msg = "ERROR: 'BreakEven2_After' value should be greater than 'BreakEven2_Profit";
                Print(msg);
            }

            if (msg != null)
                Stop();
        }

        // Returns rounded number
        private double RND(double p)
        {
            return Math.Round(p, Symbol.Digits);
        }

        // Test if position fits the rules to be managed
        private bool ManageThisPosition(Position position)
        {
            // Check if position matches the current chart
            if (ManageThisPairOnly && position.SymbolCode != Symbol.Code)
                return false;

            // If "ManageByComment" is set to true, check if comment matches
            if (ManageByComment && position.Comment != ManagedComment)
                return false;

            // Check if position comment is to be ignored
            if (IgnoreByComment && position.Comment == IgnoredComment)
                return false;

            return true;
        }

        // Set initial StopLoss and TakeProfit
        private void SetStops(Position position)
        {
            double? NewSL = position.StopLoss;
            double? NewTP = position.TakeProfit;

            if (StopLoss_Initial > 0 && position.StopLoss == null)
            {
                // Manually close position if it's too late to set a SL
                if (position.Pips <= StopLoss_Initial * (-1))
                {
                    ClosePosition(position);
                    return;
                }

                if (position.TradeType == TradeType.Buy)
                    NewSL = RND(position.EntryPrice - StopLoss_Initial * symbol.PipSize);
                else
                    NewSL = RND(position.EntryPrice + StopLoss_Initial * symbol.PipSize);
            }

            if (TakeProfit_Initial > 0 && position.TakeProfit == null)
            {
                // Manually close position if it's too late to set a TP
                if (position.Pips >= TakeProfit_Initial)
                {
                    ClosePosition(position);
                    return;
                }

                if (position.TradeType == TradeType.Buy)
                    NewTP = RND(position.EntryPrice + TakeProfit_Initial * symbol.PipSize);
                else
                    NewTP = RND(position.EntryPrice - TakeProfit_Initial * symbol.PipSize);
            }


            result = ModifyPosition(position, NewSL, NewTP);
            if (!result.IsSuccessful)
                Print("ERROR: Setting Stops!", result.Error);
        }

        // Set a BreakEven sl at "BreakEven_Profit" after reaching "BreakEven_After"
        private void BreakEven(Position position, double BE_Profit)
        {
            double BE_Price;
            if (position.TradeType == TradeType.Buy)
            {
                BE_Price = RND(position.EntryPrice + BE_Profit * symbol.PipSize);
                if (!(position.StopLoss >= BE_Price) && BE_Price < Bid)
                {
                    result = ModifyPosition(position, BE_Price, position.TakeProfit);
                    if (!result.IsSuccessful)
                        Print("ERROR: Setting BreakEven!", result.Error);
                }
            }
            else
            {
                BE_Price = RND(position.EntryPrice - BE_Profit * symbol.PipSize);
                if (!(position.StopLoss <= BE_Price) && BE_Price > Ask)
                {
                    result = ModifyPosition(position, BE_Price, position.TakeProfit);
                    if (!result.IsSuccessful)
                        Print("ERROR: Setting BreakEven!", result.Error);
                }
            }
        }

        // Jumping StopLoss
        private void JumpingStop(Position position)
        {
            double Max = position.Pips - Jumping_Distance;
            double NewJump = Max - (Max % Jumping_Stop);
            double NewSL;

            if (position.TradeType == TradeType.Buy)
            {
                NewSL = position.EntryPrice + NewJump * symbol.PipSize;
                if (NewSL <= position.StopLoss)
                    return;
            }
            else
            {
                NewSL = position.EntryPrice - NewJump * symbol.PipSize;
                if (NewSL >= position.StopLoss)
                    return;
            }
            result = ModifyPosition(position, NewSL, position.TakeProfit);
            if (!result.IsSuccessful)
                Print("ERROR: Setting Jumping Stop!", result.Error);
        }

        // Close half the position when "Take_Profit_1" is reached
        private void TakeProfit1(Position position)
        {
            if (position.Pips >= TakeProfit_1)
            {
                long Close_Lots = Symbol.NormalizeVolume(position.Volume / 2);

                for (i = 0; i < Partial_Positions.Count; i++)
                    if (Partial_Positions[i] == position.Id)
                        return;
                // Make sure the position is divisible (not 0.01 lots)
                if (Close_Lots != position.Volume)
                {
                    result = ClosePosition(position, Close_Lots);
                    if (!result.IsSuccessful)
                        Print("ERROR: Closing Half Position!", result.Error);
                    else
                    {
                        Partial_Positions.Add(position.Id);
                        using (System.IO.StreamWriter file = new System.IO.StreamWriter(FileName, true))
                            file.WriteLine(position.Id);
                    }
                }
            }
        }

        protected override void OnStop()
        {
        }
    }
}
