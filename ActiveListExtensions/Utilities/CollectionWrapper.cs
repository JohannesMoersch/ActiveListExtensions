using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	public class CollectionWrapper<T> : IReadOnlyList<T>, IDisposable
	{
		public T this[int index]
		{
			get
			{
				if (_collection == null)
					throw new IndexOutOfRangeException(nameof(index));
				return _collection[index];
			}
		}

		public int Count => _collection?.Count ?? 0;

		public int CollectionIndex { get; set; }

		private PropertyWatcherList<T> _watcherList;

		private IReadOnlyList<T> _collection;

		private string[] _propertiesToWatch;

		public CollectionWrapper(IReadOnlyList<T> collection, string[] propertiesToWatch = null)
		{
			if (collection == null)
				throw new ArgumentNullException(nameof(collection));
			_collection = collection;

			if ((propertiesToWatch?.Length ?? 0) > 0)
				_propertiesToWatch = propertiesToWatch;

			if (typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(T)))
				_watcherList = SetupPropertyWatcher();

			if (_collection is INotifyCollectionChanged)
				CollectionChangedEventManager.AddHandler(_collection as INotifyCollectionChanged, HandleCollectionChange);
		}

		public void Reset() => HandleCollectionChange(_collection, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));

		private bool _isDisposed;
		public void Dispose()
		{
			if (_isDisposed)
				return;
			_isDisposed = true;

			if (_collection is INotifyCollectionChanged)
			{
				CollectionChangedEventManager.RemoveHandler(_collection as INotifyCollectionChanged, HandleCollectionChange);
				_watcherList?.Dispose();
				_watcherList = null;
				_collection = null;
				ItemModified = null;
			}
		}

		private PropertyWatcherList<T> SetupPropertyWatcher()
		{
			if (_collection is INotifyCollectionChanged && _propertiesToWatch != null)
			{
				var watcherList = new PropertyWatcherList<T>(_propertiesToWatch);
				watcherList.ValueChanged += (i, v) => ItemModified?.Invoke(this, i, v);
				return watcherList;
			}
			return null;
		}

		private void HandleCollectionChange(object sender, NotifyCollectionChangedEventArgs args)
		{
			if (_watcherList != null)
			{
				if (args.Action == NotifyCollectionChangedAction.Remove || args.Action == NotifyCollectionChangedAction.Replace || args.Action == NotifyCollectionChangedAction.Move)
				{
					if (args.OldItems.Count > 1)
						throw new NotSupportedException("Multi-value collection change notifications are not supported.");
					foreach (var item in args.OldItems.OfType<T>())
						_watcherList.Remove(args.OldStartingIndex);
				}
				if (args.Action == NotifyCollectionChangedAction.Add || args.Action == NotifyCollectionChangedAction.Replace || args.Action == NotifyCollectionChangedAction.Move)
				{
					if (args.NewItems.Count > 1)
						throw new NotSupportedException("Multi-value collection change notifications are not supported.");
					foreach (var item in args.NewItems.OfType<T>())
						_watcherList.Add(args.NewStartingIndex, item);
				}
				if (args.Action == NotifyCollectionChangedAction.Reset)
					_watcherList.Reset(_collection);
			}
			OnCollectionChanged(args);
		}

		protected virtual void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					ItemAdded?.Invoke(this, args.NewStartingIndex, (T)args.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Remove:
					ItemRemoved?.Invoke(this, args.OldStartingIndex, (T)args.OldItems[0]);
					break;
				case NotifyCollectionChangedAction.Move:
					ItemMoved?.Invoke(this, args.OldStartingIndex, args.NewStartingIndex, (T)args.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Replace:
					ItemReplaced?.Invoke(this, args.OldStartingIndex, (T)args.OldItems[0], (T)args.NewItems[0]);
					break;
				case NotifyCollectionChangedAction.Reset:
					ItemsReset?.Invoke(this);
					break;
			}
			CollectionChanged?.Invoke(this, args);
		}

		public IEnumerator<T> GetEnumerator() => _collection?.GetEnumerator() ?? Enumerable.Empty<T>().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		public event Action<CollectionWrapper<T>, int, T> ItemModified;

		public event Action<CollectionWrapper<T>, int, T> ItemAdded;

		public event Action<CollectionWrapper<T>, int, T> ItemRemoved;

		public event Action<CollectionWrapper<T>, int, int, T> ItemMoved;

		public event Action<CollectionWrapper<T>, int, T, T> ItemReplaced;

		public event Action<CollectionWrapper<T>> ItemsReset;

		public event Action<CollectionWrapper<T>, NotifyCollectionChangedEventArgs> CollectionChanged;
	}
}
