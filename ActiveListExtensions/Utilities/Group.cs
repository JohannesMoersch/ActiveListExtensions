using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class Group<TKey, TSource> : ObservableList<TSource>, IActiveGrouping<TKey, TSource>
	{
		public TKey Key { get; }

		public Group(TKey key) => Key = key;
	}
}
