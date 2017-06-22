using ActiveListExtensions.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	internal static class EnumerableExtensions
	{
		public static IReadOnlyList<T> ToReadOnlyList<T>(this IEnumerable<T> source)
			=> ConvertToReadOnlyList<T>(source);

		public static IReadOnlyList<object> ToReadOnlyList(this IEnumerable source)
			=> ConvertToReadOnlyList<object>(source);

		private static IReadOnlyList<T> ConvertToReadOnlyList<T>(object source)
		{
			if (source is IReadOnlyList<T> readOnlyList)
				return readOnlyList;
			if (source is IList<T> genericList)
				return new ListToReadOnlyWrapper<T>(genericList);
			if (source is IList list)
				return new ObjectListToReadOnlyWrapper<T>(list);
			if (source is ICollection<T> genericCollection)
			{
				var returnList = new T[genericCollection.Count];
				int index = 0;
				foreach (var item in genericCollection)
					returnList[index++] = item;
				return returnList;
			}
			if (source is ICollection collection)
			{
				var returnList = new T[collection.Count];
				int index = 0;
				foreach (var item in collection)
					returnList[index++] = (T)item;
				return returnList;
			}
			if (source is IEnumerable<T> enumerable)
				return enumerable.ToList();
			return (source as IEnumerable).Cast<T>().ToList();
		}
	}
}
