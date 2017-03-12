using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Modifiers.Bases
{
	internal abstract class ActiveBase<T, U> : IActiveList<U>
	{
		public abstract int Count { get; }

		public abstract U this[int index] { get; }

		private bool _isDisposed;
		public void Dispose()
		{
			if (_isDisposed)
				return;
			_isDisposed = true;
			OnDisposed();
			_propertyChanged = null;
			_collectionChanged = null;
		}

		protected virtual void OnDisposed() { }

		protected void NotifyOfPropertyChange(PropertyChangedEventArgs args) => _propertyChanged?.Invoke(this, args);

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

		protected void NotifyOfCollectionChange(NotifyCollectionChangedEventArgs args) => _collectionChanged?.Invoke(this, args);

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

		public abstract IEnumerator<U> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
