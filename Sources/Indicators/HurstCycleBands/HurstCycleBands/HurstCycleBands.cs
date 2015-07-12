using System;
using cAlgo.API;

namespace cAlgo.Indicators
{
    [Indicator(IsOverlay = true, AccessRights = AccessRights.None)]
    public class HurstCycleBands : Indicator
    {

        double _yesterdayHigh;
        double _yesterdayLow;
        double _yesterdayClose;
        double _p;
        double _q;

        [Parameter(DefaultValue = 0, MinValue = 0, MaxValue = 23)]
        public int TimeZone { get; set; }

        [Parameter(DefaultValue = 1, MinValue = 0, MaxValue = 1)]
        public int Pivots { get; set; }

        [Parameter(DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int Camarilla { get; set; }

        [Parameter(DefaultValue = 0, MinValue = 0, MaxValue = 1)]
        public int MidPivots { get; set; }

        #region Pivots

        [Output("R1", Color = Colors.Green)]
        public IndicatorDataSeries R1 { get; set; }
        [Output("R2", Color = Colors.Green)]
        public IndicatorDataSeries R2 { get; set; }
        [Output("R3", Color = Colors.Green)]
        public IndicatorDataSeries R3 { get; set; }

        [Output("S1", Color = Colors.Red)]
        public IndicatorDataSeries S1 { get; set; }
        [Output("S2", Color = Colors.Red)]
        public IndicatorDataSeries S2 { get; set; }
        [Output("S3", Color = Colors.Red)]
        public IndicatorDataSeries S3 { get; set; }

        [Output("Pivot", Color = Colors.Violet)]
        public IndicatorDataSeries Pivot { get; set; }

        #endregion

        #region Camarilla

        [Output("C1", Color = Colors.Yellow)]
        public IndicatorDataSeries C1 { get; set; }
        [Output("C2", Color = Colors.Yellow)]
        public IndicatorDataSeries C2 { get; set; }
        [Output("C3", Color = Colors.Yellow)]
        public IndicatorDataSeries C3 { get; set; }
        [Output("C4", Color = Colors.Yellow)]
        public IndicatorDataSeries C4 { get; set; }

        #endregion

        #region MidPivots

        [Output("M1", Color = Colors.Blue)]
        public IndicatorDataSeries M1 { get; set; }
        [Output("M2", Color = Colors.Blue)]
        public IndicatorDataSeries M2 { get; set; }
        [Output("M3", Color = Colors.Blue)]
        public IndicatorDataSeries M3 { get; set; }
        [Output("M4", Color = Colors.Blue)]
        public IndicatorDataSeries M4 { get; set; }
        [Output("M5", Color = Colors.Blue)]
        public IndicatorDataSeries M5 { get; set; }
        [Output("M6", Color = Colors.Blue)]
        public IndicatorDataSeries M6 { get; set; }

        #endregion

        protected override void Initialize()
        {
            Print("Timeframe <= Daily");
        }
        public override void Calculate(int index)
        {
            DateTime today = DateTime.Now;
            DateTime yesterday = today.AddDays(-1);

            DateTime openTime = MarketSeries.OpenTime[index];

            if (openTime.Date == yesterday.Date || openTime.Date == today.Date)
            {

                //  Yesterday Open, Init Yesterday High & Low
                if (openTime.Date == yesterday.Date && openTime.Hour == TimeZone && openTime.Minute == 0)
                {
                    _yesterdayHigh = MarketSeries.High[index];
                    _yesterdayLow = MarketSeries.Low[index];
                }
                // Yesterday Close
                if (openTime.Date == today.Date && openTime.Hour == 0 && openTime.Minute == 0)
                {
                    _yesterdayClose = MarketSeries.Close[index];
                }
                // Calculate Yesterday High & Low
                if (openTime.Date == yesterday.Date)
                {
                    if (MarketSeries.High[index] > _yesterdayHigh)
                        _yesterdayHigh = MarketSeries.High[index];
                    if (MarketSeries.Low[index] < _yesterdayLow)
                        _yesterdayLow = MarketSeries.Low[index];

                }



                if (openTime.Date == today.Date)
                {
                    _q = (_yesterdayHigh - _yesterdayLow);
                    _p = (_yesterdayHigh + _yesterdayLow + _yesterdayClose) / 3;

                    double r1 = (2 * _p) - _yesterdayLow;
                    double r2 = _p + (_yesterdayHigh - _yesterdayLow);
                    double r3 = (2 * _p) + (_yesterdayHigh - (2 * _yesterdayLow));

                    double s1 = (2 * _p) - _yesterdayHigh;
                    double s2 = _p - (_yesterdayHigh - _yesterdayLow);
                    double s3 = (2 * _p) - ((2 * _yesterdayHigh) - _yesterdayLow);

                    if (Pivots == 1)
                    {
                        R1[0] = R1[index] = r1;
                        R2[0] = R2[index] = r2;
                        R3[0] = R3[index] = r3;

                        S1[0] = S1[index] = s1;
                        S2[0] = S2[index] = s2;
                        S3[0] = S3[index] = s3;

                        Pivot[0] = Pivot[index] = _p;
                    }

                    if (Camarilla == 1)
                    {
                        C4[0] = C4[index] = (_q * 0.55) + _yesterdayClose;
                        C3[0] = C3[index] = (_q * 0.27) + _yesterdayClose;
                        C2[0] = C2[index] = _yesterdayClose - (_q * 0.55);
                        C1[0] = C1[index] = _yesterdayClose - (_q * 0.27);

                    }

                    if (MidPivots == 1)
                    {
                        M6[0] = M6[index] = (r2 + r3) / 2;
                        M5[0] = M5[index] = (r1 + r2) / 2;
                        M4[0] = M4[index] = (_p + r1) / 2;
                        M3[0] = M3[index] = (_p + s1) / 2;
                        M2[0] = M2[index] = (s1 + s2) / 2;
                        M1[0] = M1[index] = (s2 + s3) / 2;
                    }
                }

            }

        }
    }
}
