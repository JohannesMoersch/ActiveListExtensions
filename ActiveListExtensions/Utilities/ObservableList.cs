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
	internal class ObservableList<T> : ObservableList<T, T, T>
	{
		public ObservableList()
			: base(i => i)
		{
		}

		protected override T GetStoreFromSource(T source) => source;

		protected override void DisposeOfStore(T store) { }
	}

	internal class ObservableList<TSource, TResult> : ObservableList<TSource, TSource, TResult>
	{
		public ObservableList(Func<TSource, TResult> itemSelector)
			: base(itemSelector)
		{
		}

		protected override TSource GetStoreFromSource(TSource source) => source;

		protected override void DisposeOfStore(TSource store) { }
	}

	internal abstract class ObservableList<TSource, TStore, TResult> : IActiveList<TResult>
	{
		private int _skipStart, _skipCount;

		public TStore this[int index]
		{
			get
			{
				if (index >= _skipStart)
					index += _skipCount;
				return List[index];
			}
		}

		TResult IReadOnlyList<TResult>.this[int index] => GetResultFromItem(this[index]);

		public int Count => List.Count - _skipCount;

		protected QuickList<TStore> List { get; }

		private readonly Func<TStore, TResult> _itemSelector;

		public ObservableList(Func<TStore, TResult> itemSelector)
		{
			List = new QuickList<TStore>();

			_itemSelector = itemSelector;
		}

		protected abstract TStore GetStoreFromSource(TSource source);

		protected abstract void DisposeOfStore(TStore store);

		public virtual void Dispose()
		{
			PropertyChanged = null;
			CollectionChanged = null;
			List.Clear();
		}

		public virtual void Add(int index, TSource value)
		{
			var store = GetStoreFromSource(value);
			List.Add(index, store);
			NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetResultFromItem(store), index));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		public virtual void Remove(int index)
		{
			var store = List[index];
			List.Remove(index);
			var value = GetResultFromItem(store);
			DisposeOfStore(store);
			NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		public virtual void Replace(int index, TSource newValue)
		{
			var oldStore = List[index];
			var store = GetStoreFromSource(newValue);
			List.Replace(index, store);
			var oldValue = GetResultFromItem(oldStore);
			DisposeOfStore(oldStore);
			NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, GetResultFromItem(store), oldValue, index));
		}

		public virtual void Move(int oldIndex, int newIndex)
		{
			List.Move(oldIndex, newIndex);
			NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, GetResultFromItem(List[newIndex]), newIndex, oldIndex));
		}

		public virtual void Reset(IEnumerable<TSource> values)
		{
			var oldCount = Count;
			foreach (var item in List)
				DisposeOfStore(item);
			List.Clear();
			foreach (var value in values)
				List.Add(List.Count, GetStoreFromSource(value));
			NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			if (oldCount != Count)
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		public virtual void ReplaceRange(int startIndex, int oldCount, IReadOnlyList<TSource> newValues)
		{
			try
			{
				var top = List.Count - 1;
				var bottom = startIndex + oldCount;
				var diff = newValues.Count - oldCount;

				int min = diff < 0 ? newValues.Count : oldCount;
				for (int i = 0; i < min; ++i)
					Replace(startIndex + i, newValues[i]);

				if (diff > 0)
				{
					for (int i = 0; i < diff; ++i)
						List.Add(List.Count, default(TStore));
					for (int i = top; i >= bottom; --i)
						List.Replace(i + diff, List[i]);
					for (int i = oldCount; i < newValues.Count; ++i)
						List.Replace(startIndex + i, GetStoreFromSource(newValues[i]));

					_skipStart = startIndex + oldCount;
					_skipCount = newValues.Count - oldCount;
					for (int i = oldCount; i < newValues.Count; ++i)
					{
						--_skipCount;
						++_skipStart;
						var index = startIndex + i;
						NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetResultFromItem(List[index]), index));
					}
				}
				else if (diff < 0)
				{
					var end = newValues.Count + startIndex;
					_skipStart = oldCount - 1 + startIndex;
					_skipCount = 0;
					for (int i = _skipStart; i >= end; --i)
					{
						++_skipCount;
						NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, GetResultFromItem(List[i]), i));
						--_skipStart;
					}

					for (int i = bottom; i <= top; ++i)
						List.Replace(i + diff, List[i]);
					for (int i = 0; i < -diff; ++i)
					{
						var store = List[List.Count - 1];
						List.Remove(List.Count - 1);
						DisposeOfStore(store);
					}
				}

				if (diff != 0)
					PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
			}
			finally
			{
				_skipStart = 0;
				_skipCount = 0;
			}
		}

		protected TResult GetResultFromItem(TStore item) => _itemSelector.Invoke(item);

		protected void NotifyOfCollectionChange(NotifyCollectionChangedEventArgs e) => CollectionChanged?.Invoke(this, e);

		public event PropertyChangedEventHandler PropertyChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private IEnumerable<TStore> EnumerateCollection()
		{
			for (int i = 0; i < Count; ++i)
				yield return this[i];
		}

		public IEnumerator<TStore> GetEnumerator() => EnumerateCollection().GetEnumerator();

		IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => EnumerateCollection().Select(i => GetResultFromItem(i)).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => EnumerateCollection().Select(i => GetResultFromItem(i)).GetEnumerator();
	}
}
