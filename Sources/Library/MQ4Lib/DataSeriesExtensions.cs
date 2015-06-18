
using cAlgo.API;

namespace cAlgo.MQ4
{

	static class DataSeriesExtensions
	{
		public static Mq4Double FromEnd(this DataSeries dataSeries, int index)
		{
			if (index >= 0 && index < dataSeries.Count)
				return dataSeries[dataSeries.InvertIndex(index)];
			return 0;
		}

		public static int InvertIndex(this DataSeries dataSeries, int index)
		{
			return dataSeries.Count - 1 - index;
		}
	}

}
