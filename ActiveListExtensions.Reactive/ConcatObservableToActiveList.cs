using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Reactive
{
	public class ConcatObservableToActiveList<T> : IObserver<T>, IActiveList<T>
	{
		public int Count => _list.Count;

		public T this[int index] => _list[index];

		private IMutableActiveList<T> _list;

		private bool _isDisposed;

		public ConcatObservableToActiveList()
		{
			_list = ActiveList.Create<T>();

			_list.PropertyChanged += (s, e) => _propertyChanged?.Invoke(this, e);
			_list.CollectionChanged += (s, e) => _collectionChanged?.Invoke(this, e);
		}

		public void Dispose()
		{
			if (_isDisposed)
				return;
			_isDisposed = true;

			_list.Dispose();

			_collectionChanged = null;
			_propertyChanged = null;
			Disposed?.Invoke();
			Disposed = null;
		}

		public void OnCompleted() => Dispose();

		public void OnError(Exception error) { }

		public void OnNext(T value)
		{
			if (!_isDisposed)
				_list.Add(value);
		}

		public IEnumerator<T> GetEnumerator()
			=> _list.GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator()
			=> _list.GetEnumerator();

		private NotifyCollectionChangedEventHandler _collectionChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add
			{
				if (!_isDisposed)
					_collectionChanged += value;
			}
			remove
			{
				if (!_isDisposed)
					_collectionChanged -= value;
			}
		}

		private PropertyChangedEventHandler _propertyChanged;
		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (!_isDisposed)
					_propertyChanged += value;
			}
			remove
			{
				if (!_isDisposed)
					_propertyChanged -= value;
			}
		}

		internal event Action Disposed;
	}
}
