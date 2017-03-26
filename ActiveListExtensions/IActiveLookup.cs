using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public interface IActiveLookup<TKey, out TSource> : IActiveList<IActiveGrouping<TKey, TSource>>
	{
		IEnumerable<TSource> this[TKey key] { get; }

		bool Contains(TKey key);
	}
}
