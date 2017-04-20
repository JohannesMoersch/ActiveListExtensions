using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Reactive
{
	public class ItemAdded<T>
	{
		public T Item { get; }

		public int Index { get; }

		public ItemAdded(T item, int index)
		{
			Item = item;
			Index = index;
		}
	}
}
