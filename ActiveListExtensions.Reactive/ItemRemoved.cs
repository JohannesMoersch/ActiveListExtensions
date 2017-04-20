using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Reactive
{
	public class ItemRemoved<T>
	{
		public T Item { get; }

		public int Index { get; }

		public ItemRemoved(T item, int index)
		{
			Item = item;
			Index = index;
		}
	}
}
