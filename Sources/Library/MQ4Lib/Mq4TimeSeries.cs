using System;

using cAlgo.API;

namespace cAlgo.MQ4
{
	public class Mq4TimeSeries
	{
		private readonly TimeSeries _timeSeries;
		private static readonly DateTime StartDateTime = new DateTime(1970, 1, 1);

		public Mq4TimeSeries(TimeSeries timeSeries)
		{
			_timeSeries = timeSeries;
		}

		public static int ToInteger(DateTime dateTime)
		{
			return (int)(dateTime - StartDateTime).TotalSeconds;
		}

		public static DateTime ToDateTime(int seconds)
		{
			return StartDateTime.AddSeconds(seconds);
		}

		public int this[int index]
		{
			get
			{
				if (index < 0 || index >= _timeSeries.Count)
					return 0;

				DateTime dateTime = _timeSeries[_timeSeries.Count - 1 - index];

				return ToInteger(dateTime);
			}
		}
	}


}
