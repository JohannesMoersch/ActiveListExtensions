using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ListToReadOnlyWrapper<T> : IReadOnlyList<T>, INotifyCollectionChanged
	{
		public T this[int index] => _source[index];

		public int Count => _source.Count;

		private IList<T> _source;

		public ListToReadOnlyWrapper(IList<T> source)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			_source = source;
			if (_source is INotifyCollectionChanged)
				CollectionChangedEventManager.AddHandler(_source as INotifyCollectionChanged, HandleCollectionChanged);
		}

		private void HandleCollectionChanged(object sender, NotifyCollectionChangedEventArgs args) => CollectionChanged?.Invoke(this, args);

		public event NotifyCollectionChangedEventHandler CollectionChanged;

		public IEnumerator<T> GetEnumerator() => _source.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => _source.GetEnumerator();
	}
}
