using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	internal static class EnumerableExtensions
	{
		public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
		{
			if (source is IReadOnlyList<T> readOnlyList)
				return readOnlyList;
			if (source is ICollection<T> collection)
			{
				var list = new List<T>(collection.Count);
				foreach (var item in source)
					list.Add(item);
				return new ListToReadOnlyWrapper<T>(list);
			}
			return source.ToList();
		}
	}
}
