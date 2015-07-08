using cAlgo.API;
using System;
using System.Text;

namespace cAlgo
{
    [Indicator("Position Stats", IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class PositionStats : Indicator
    {

        [Parameter("Show balance", DefaultValue = true)]
        public bool showBalance { get; set; }

        [Parameter("Show equity", DefaultValue = true)]
        public bool showEquity { get; set; }

        [Parameter("Buy/Sell Amount", DefaultValue = true)]
        public bool showBuySellAmount { get; set; }

        [Parameter("Buy/Sell Count", DefaultValue = true)]
        public bool showBuySellCount { get; set; }

        [Parameter("Total amount", DefaultValue = true)]
        public bool showTotalAmount { get; set; }

        [Parameter("Total count", DefaultValue = true)]
        public bool showTotalCount { get; set; }

        [Parameter("Pip cost", DefaultValue = true)]
        public bool showPipCost { get; set; }

        [Parameter("Margin Level", DefaultValue = true)]
        public bool showMarginLevel { get; set; }

        [Parameter("Chart corner, 1-8", DefaultValue = 1, MinValue = 1, MaxValue = 8)]
        public int corner { get; set; }

        [Parameter("Show labels", DefaultValue = true)]
        public bool showLabels { get; set; }

        [Parameter("Show account currency", DefaultValue = true)]
        public bool showCurrency { get; set; }

        [Parameter("Show base currency", DefaultValue = true)]
        public bool showBaseCurrency { get; set; }


        protected override void Initialize()
        {
            Positions.Opened += delegate(PositionOpenedEventArgs args) { update(); };
            Positions.Closed += delegate(PositionClosedEventArgs args) { update(); };
        }

        public override void Calculate(int index)
        {
            if (!IsLastBar)
            {
                return;
            }
            else
            {
                update();
            }
        }

        public void update()
        {
            double buy_amount = 0, sell_amount = 0, total_amount = 0;
            double buy_count = 0, sell_count = 0, total_count = 0;

            foreach (Position p in Positions)
            {
                if (p.SymbolCode != Symbol.Code)
                {
                    continue;
                }
                if (p.TradeType == TradeType.Buy)
                {
                    buy_amount += p.Volume;
                    buy_count++;
                }
                else
                {
                    sell_amount += p.Volume;
                    sell_count++;
                }
                total_amount = buy_amount - sell_amount;
                total_count = buy_count + sell_count;
            }
            StringBuilder s = new StringBuilder();
            if (showBalance)
            {
                if (showLabels)
                    s.Append("Balance: ");
                s.AppendFormat("{0:N2}", Account.Balance);
                if (showCurrency)
                {
                    s.Append(" ");
                    s.Append(Account.Currency);
                }
                s.AppendLine();
            }
            if (showEquity)
            {
                if (showLabels)
                    s.Append("Equity: ");
                s.AppendFormat("{0:N2}", Account.Equity);
                if (showCurrency)
                {
                    s.Append(" ");
                    s.Append(Account.Currency);
                }
                s.AppendLine();
            }
            if (showBuySellAmount)
            {
                if (showLabels)
                    s.Append("Buy vol: ");
                s.AppendFormat("{0:N0}", buy_amount);
                if (showBaseCurrency)
                {
                    s.Append(" ");
                    s.Append(Symbol.Code.Substring(0, 3));
                }
                s.AppendLine();

                if (showLabels)
                    s.Append("Sell vol: ");
                s.AppendFormat("{0:N0}", sell_amount);
                if (showBaseCurrency)
                {
                    s.Append(" ");
                    s.Append(Symbol.Code.Substring(0, 3));
                }
                s.AppendLine();
            }
            if (showBuySellCount)
            {
                if (showLabels)
                    s.Append("Buy cnt: ");
                s.AppendFormat("{0:N0}", buy_count);
                s.AppendLine();

                if (showLabels)
                    s.Append("Sell cnt: ");
                s.AppendFormat("{0:N0}", sell_count);
                s.AppendLine();
            }

            if (showTotalAmount)
            {
                if (showLabels)
                    s.Append("Total vol: ");
                s.AppendFormat("{0:+#,###;-#,###;0}", total_amount);
                s.AppendLine();
            }
            if (showTotalCount)
            {
                if (showLabels)
                    s.Append("Total cnt: ");
                s.AppendFormat("{0:N0}", total_count);
                s.AppendLine();
            }
            if (showPipCost)
            {
                if (showLabels)
                    s.Append("Pip cost: ");
                s.AppendFormat("{0:N2}", total_amount * Symbol.PipValue);
                if (showCurrency)
                {
                    s.Append(" ");
                    s.Append(Account.Currency);
                }
                s.AppendLine();
            }
            if (showMarginLevel)
            {
                if (showLabels)
                    s.Append("Margin level: ");
                if (Account.MarginLevel == null)
                {
                    s.Append("-");
                }
                else
                {
                    s.AppendFormat("{0:N2}", Account.MarginLevel);
                    s.Append("%");
                }
                s.AppendLine();
            }

            StaticPosition pos;
            switch (corner)
            {
                case 1:
                    pos = StaticPosition.TopLeft;
                    break;
                case 2:
                    pos = StaticPosition.TopCenter;
                    break;
                case 3:
                    pos = StaticPosition.TopRight;
                    break;
                case 4:
                    pos = StaticPosition.Right;
                    break;
                case 5:
                    pos = StaticPosition.BottomRight;
                    break;
                case 6:
                    pos = StaticPosition.BottomCenter;
                    break;
                case 7:
                    pos = StaticPosition.BottomLeft;
                    break;
                case 8:
                    pos = StaticPosition.Left;
                    break;
                default:
                    pos = StaticPosition.TopLeft;
                    break;
            }
            ChartObjects.DrawText("showInfo", s.ToString(), pos);
        }

    }
}
