
using cAlgo.API.Internals;

namespace cAlgo.MQ4
{
	public class CachedStandardIndicators
	{
		private readonly IIndicatorsAccessor _indicatorsAccessor;

		public CachedStandardIndicators(IIndicatorsAccessor indicatorsAccessor)
		{
			_indicatorsAccessor = indicatorsAccessor;
		}
	}
}
