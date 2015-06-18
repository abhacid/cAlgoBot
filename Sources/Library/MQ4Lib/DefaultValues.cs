

namespace cAlgo.MQ4
{
	static class DefaultValues
	{
		public static object GetDefaultValue<T>()
		{
			if (typeof(T) == typeof(Mq4Double))
				return new Mq4Double(0);
			if (typeof(T) == typeof(string))
				return string.Empty;
			if (typeof(T) == typeof(Mq4String))
				return new Mq4String(string.Empty);

			return default(T);
		}
	}


}
