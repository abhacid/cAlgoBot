using System;
using cAlgo.API;
using cAlgo.API.Internals;
using cAlgo.API.Indicators;
using cAlgo.Indicators;

namespace cAlgo {

	[Indicator (IsOverlay = false, TimeZone = TimeZones.UTC, AccessRights = AccessRights.None)]
	public class Ratio : Indicator {
	
		[Parameter ()]
		public DataSeries Source1 { get; set; }
		
		[Parameter ()]
		public DataSeries Source2 { get; set; }
		
		[Output	("Result")]
		public IndicatorDataSeries Result { get; set; }

		protected override void Initialize () {
		}

		public override void Calculate (int index) {
			Result[index] = Source1[index] / Source2[index];
		}
	}
}
