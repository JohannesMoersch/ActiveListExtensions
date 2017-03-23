using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Modifiers
{
	public class ActiveLookup<TSource, TKey> : IActiveLookup<TKey, TSource>
	{
		public ActiveLookup(Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
		{
		}

		public IEnumerable<TSource> this[TKey key] => throw new NotImplementedException();

		public IActiveGrouping<TKey, TSource> this[int index] => throw new NotImplementedException();

		public int Count => throw new NotImplementedException();

		public event PropertyChangedEventHandler PropertyChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public bool Contains(TKey key)
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public IEnumerator<IActiveGrouping<TKey, TSource>> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
