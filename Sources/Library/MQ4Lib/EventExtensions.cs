using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using cAlgo.API;
using cAlgo.API.Internals;

namespace cAlgo.MQ4
{
	static class EventExtensions
	{
		public static void Raise<T1, T2>(this Action<T1, T2> action, T1 arg1, T2 arg2)
		{
			if (action != null)
				action(arg1, arg2);
		}
	}


}
