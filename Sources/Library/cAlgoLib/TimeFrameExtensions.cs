#region Licence
//The MIT License (MIT)
//Copyright (c) 2014 abdallah HACID, https://www.facebook.com/ab.hacid

//Permission is hereby granted, free of charge, to any person obtaining a copy of this software
//and associated documentation files (the "Software"), to deal in the Software without restriction,
//including without limitation the rights to use, copy, modify, merge, publish, distribute,
//sublicense, and/or sell copies of the Software, and to permit persons to whom the Software
//is furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all copies or
//substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING
//BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
//NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
//DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.

// Project Hosting for Open Source Software on Github : https://github.com/abhacid/cAlgoBot
#endregion

using System;
using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.Lib
{
	/// <summary>
	/// Méthodes d'extensions du type cAlgo.API.TimeFrame
	/// </summary>
	public static class TimeFrameExtensions
	{
		public enum TimeFrameEnum
		{
			Minute,
			Minute2,
			Minute3,
			Minute4,
			Minute5,
			Minute6,
			Minute7,
			Minute8,
			Minute9,
			Minute10,
			Minute15,
			Minute20,
			Minute30,
			Minute45,
			Hour,
			Hour12,
			Hour2,
			Hour3,
			Hour4,
			Hour6,
			Hour8,
			Daily,
			Day2,
			Day3,
			Weekly,
			Monthly,
		}

		public static TimeFrameEnum ToTimeFrameEnum(this TimeFrame timeFrame)
		{
			TimeFrameEnum timeFrameEnum;

			Enum.TryParse<TimeFrameEnum>(timeFrame.ToString(), out timeFrameEnum);

			return timeFrameEnum; 
		}


		public static TimeSpan ToTimeSpan(this TimeFrame timeFrame)
		{
			TimeSpan timeSpan;
			long ticks=0;


			// instruction to transform each TimeFrame.Time in TimeSpan.

			switch(timeFrame.ToTimeFrameEnum())
			{
				case TimeFrameEnum.Minute:
					ticks = TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute2:
					ticks = 2 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute3:
					ticks =3 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute4:
					ticks = 4 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute5:
					ticks = 5 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute6:
					ticks = 6 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute7:
					ticks = 7 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute8:
					ticks = 8 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute9:
					ticks = 9 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute10:
					ticks = 10 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute15:
					ticks = 15 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute20:
					ticks = 20 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute30:
					ticks = 30 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Minute45:
					ticks = 45 * TimeSpan.TicksPerMinute;
					break;

				case TimeFrameEnum.Hour:
					ticks = TimeSpan.TicksPerHour;
					break;

				case TimeFrameEnum.Hour2:
					ticks = 2 * TimeSpan.TicksPerHour;
					break;

				case TimeFrameEnum.Hour3:
					ticks = 3 * TimeSpan.TicksPerHour;
					break;

				case TimeFrameEnum.Hour4:
					ticks = 4 * TimeSpan.TicksPerHour;
					break;			
			
				case TimeFrameEnum.Hour6:
					ticks = 6 * TimeSpan.TicksPerHour;
					break;

				case TimeFrameEnum.Hour8:
					ticks = 8 * TimeSpan.TicksPerHour;
					break;

				case TimeFrameEnum.Hour12:
					ticks = 12 * TimeSpan.TicksPerHour;
					break;

				case TimeFrameEnum.Daily:
					ticks = TimeSpan.TicksPerDay;
					break;

				case TimeFrameEnum.Day2:
					ticks = 2 * TimeSpan.TicksPerDay;
					break;

				case TimeFrameEnum.Day3:
					ticks = 3 * TimeSpan.TicksPerDay;
					break;

				case TimeFrameEnum.Weekly:
					ticks = 7 * TimeSpan.TicksPerDay;
					break;

				case TimeFrameEnum.Monthly:
					ticks = (long)(30.5 * TimeSpan.TicksPerDay);
					break;
			}

			timeSpan = TimeSpan.FromTicks(ticks);

			return timeSpan;
		}
	}
}