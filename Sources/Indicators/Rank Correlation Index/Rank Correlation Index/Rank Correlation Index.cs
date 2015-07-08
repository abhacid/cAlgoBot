/* Copyright (c) 2015 daneko
 * All rights reserved.
 * 
 * Permission is hereby granted, free  of charge, to any person obtaining
 * a  copy  of this  software  and  associated  documentation files  (the
 * "Software"), to  deal in  the Software without  restriction, including
 * without limitation  the rights to  use, copy, modify,  merge, publish,
 * distribute,  sublicense, and/or sell  copies of  the Software,  and to
 * permit persons to whom the Software  is furnished to do so, subject to
 * the following conditions:
 * 
 * The  above  copyright  notice  and  this permission  notice  shall  be
 * included in all copies or substantial portions of the Software.
 * 
 * THE  SOFTWARE IS  PROVIDED  "AS  IS", WITHOUT  WARRANTY  OF ANY  KIND,
 * EXPRESS OR  IMPLIED, INCLUDING  BUT NOT LIMITED  TO THE  WARRANTIES OF
 * MERCHANTABILITY,    FITNESS    FOR    A   PARTICULAR    PURPOSE    AND
 * NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
 * LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
 * OF CONTRACT, TORT OR OTHERWISE,  ARISING FROM, OUT OF OR IN CONNECTION
 * WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 */
/*
 * site: http://daneko.blog.jp/archives/1028380181.html
 * http://ctdn.com/algos/indicators/show/849
 */
using System;
using System.Collections.Generic;
using System.Linq;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;

namespace cAlgo
{
    [Indicator(IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
    public class RankCorrelationIndex : Indicator
    {
        [Parameter("Source (default close)")]
        public DataSeries Source { get; set; }

        [Parameter("periods", DefaultValue = 9, MinValue = 1)]
        public int periods { get; set; }

        [Output("Main")]
        public IndicatorDataSeries Result { get; set; }

        /**
         * for cBot
         */
        public static RankCorrelationIndex Create(IIndicatorsAccessor indicators, DataSeries source, int periods = 9)
        {
            if (periods < 1)
            {
                throw new ArgumentException("period < 1");
            }

            return indicators.GetIndicator<RankCorrelationIndex>(source, periods);
        }

        protected override void Initialize()
        {
            if (IsBacktesting)
            {
                var test = new List<double> 
                {
                    99,
                    98,
                    97,
                    96,
                    105
                };
                var d = CalculateD(test);
                if (d != 20)
                    throw new InvalidProgramException();
            }
        }

        public override void Calculate(int index)
        {
            if (index < periods)
            {
                return;
            }

            var d = CalculateD(Enumerable.Range(0, periods).Select(i => Source[index - i]));

            Result[index] = (1 - (6 * d) / (periods * (Math.Pow(periods, 2) - 1))) * 100;
        }


        /**
         * var test = new List<double> { 99,98,97,96,105 };
         * var d = CalculateD(test);
         * assert d == 20
         * @param source desc time
         */
        int CalculateD(IEnumerable<double> source)
        {
            return source.Select((v, i) => Tuple.Create(i + 1, v)).OrderByDescending(v => v.Item2).Select((v, i) => (int)Math.Pow(i + 1 - v.Item1, 2)).Sum();
        }
    }
}
