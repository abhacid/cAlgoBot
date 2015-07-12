
using System;
using cAlgo.API;


namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = false, ScalePrecision = 5, AccessRights = AccessRights.None)]
    public class Volume : Indicator
    {
        [Output("BidEntries", Color = Colors.Red, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries BidResult { get; set; }

        [Output("AskEntries", Color = Colors.Blue, PlotType = PlotType.Histogram, Thickness = 5)]
        public IndicatorDataSeries AskResult { get; set; }

        MarketDepth GBPUSD;

        private int _askNo;
        private int _bidNo;

        protected override void Initialize()
        {
            GBPUSD = MarketData.GetMarketDepth(Symbol);
            GBPUSD.Updated += OnGbpUsdUpdated;
        }

        void OnGbpUsdUpdated()
        {
            _askNo = 0;
            _bidNo = 0;

            var index = MarketSeries.Close.Count - 1;


            for (var i = 0; i < AskResult.Count; i++)
                AskResult[i] = double.NaN;

            foreach (var entry in GBPUSD.AskEntries)
            {
                AskResult[index - _askNo] = (-1) * entry.Volume;
                _askNo++;
            }

            for (var i = 0; i < BidResult.Count; i++)
                BidResult[i] = double.NaN;

            foreach (var entry in GBPUSD.BidEntries)
            {
                BidResult[index - _bidNo] = entry.Volume;
                _bidNo++;
            }
        }

        public override void Calculate(int index)
        {
        }

    }
}
