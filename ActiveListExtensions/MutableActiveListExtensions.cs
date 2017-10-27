using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public static class MutableActiveListExtensions
	{
		public static void Add<T>(this IMutableActiveList<T> list, T value)
			=> list.Add(list.Count, value);

		public static void Clear<T>(this IMutableActiveList<T> list)
			=> list.Reset(Enumerable.Empty<T>());
	}
}
