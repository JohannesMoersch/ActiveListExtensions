using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	public class ObjectListToReadOnlyWrapper<T> : IReadOnlyList<T>
	{
		public T this[int index] => (T)_list[index];

		public int Count => _list.Count;

		private IList _list;

		public ObjectListToReadOnlyWrapper(IList list) => _list = list;

		public IEnumerator<T> GetEnumerator() => _list.Cast<T>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _list.GetEnumerator();
	}
}
