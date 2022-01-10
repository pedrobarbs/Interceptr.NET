using System;
using System.Linq;

namespace Interceptr
{
	public class InterceptrStack
	{
		internal readonly IInterceptr[] _interceptors;

		public InterceptrStack(params IInterceptr[] interceptors)
		{
			if ((!interceptors?.Any()) ?? true)
				throw new ArgumentNullException(nameof(interceptors));

			_interceptors = interceptors;
		}
	}
}

