using cAlgo.API;

namespace cAlgo
{
    [Indicator(IsOverlay = true, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class ShowEquity : Indicator
    {

        [Parameter("Show \"Equity: \"", DefaultValue = true)]
        public bool showEquityWord { get; set; }
        [Parameter("Show currency of the account", DefaultValue = true)]
        public bool showCurrency { get; set; }

        public override void Calculate(int index)
        {
            if (!IsLastBar)
            {
                return;
            }
            else
            {
                ChartObjects.DrawText("equity", (showEquityWord ? "Equity: " : "") + Account.Equity.ToString("N") + (showCurrency ? " " + Account.Currency : ""), StaticPosition.TopLeft);
            }
        }

    }
}
