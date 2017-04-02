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
	internal abstract class ActiveBase<TSource, TResult> : IActiveList<TResult>
	{
		public abstract int Count { get; }

		public abstract TResult this[int index] { get; }

		protected bool IsDisposed { get; private set; }

		public void Dispose()
		{
			if (IsDisposed)
				return;
			IsDisposed = true;
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
				if (!IsDisposed)
					_propertyChanged += value;
			}
			remove
			{
				if (!IsDisposed)
					_propertyChanged -= value;
			}
		}

		protected void NotifyOfCollectionChange(NotifyCollectionChangedEventArgs args) => _collectionChanged?.Invoke(this, args);

		private NotifyCollectionChangedEventHandler _collectionChanged;

		public event NotifyCollectionChangedEventHandler CollectionChanged
		{
			add
			{
				if (!IsDisposed)
					_collectionChanged += value;
			}
			remove
			{
				if (!IsDisposed)
					_collectionChanged -= value;
			}
		}

		public abstract IEnumerator<TResult> GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
