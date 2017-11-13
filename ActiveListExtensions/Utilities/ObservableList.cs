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
	internal class ObservableList<T> : ObservableList<T, T, T>, IMutableActiveList<T>
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
		private int _skipStart1, _skipCount1;
		private int _skipStart2, _skipCount2;

		public TStore this[int index]
		{
			get
			{
				if (index >= _skipStart1)
					index += _skipCount1;
				if (index >= _skipStart2)
					index += _skipCount2;
				return List[index];
			}
		}

		TResult IReadOnlyList<TResult>.this[int index] => GetResultFromItem(this[index]);

		public int Count => List.Count - _skipCount1 - _skipCount2;

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
			List[index] = store;
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
			if (oldCount == 0 && newValues.Count == 0)
				return;

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
						List[i + diff] =  List[i];
					for (int i = oldCount; i < newValues.Count; ++i)
						List[startIndex + i] = GetStoreFromSource(newValues[i]);

					_skipStart1 = startIndex + oldCount;
					_skipCount1 = newValues.Count - oldCount;
					for (int i = oldCount; i < newValues.Count; ++i)
					{
						--_skipCount1;
						++_skipStart1;
						var index = startIndex + i;
						NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetResultFromItem(List[index]), index));
					}
				}
				else if (diff < 0)
				{
					var end = newValues.Count + startIndex;
					_skipStart1 = oldCount - 1 + startIndex;
					_skipCount1 = 0;
					for (int i = _skipStart1; i >= end; --i)
					{
						++_skipCount1;
						NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, GetResultFromItem(List[i]), i));
						--_skipStart1;
					}

					for (int i = bottom; i <= top; ++i)
						List[i + diff] = List[i];
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
				_skipStart1 = 0;
				_skipCount1 = 0;
			}
		}

		public virtual void MoveRange(int oldIndex, int newIndex, int count)
		{
			if (count == 0)
				return;

			if (oldIndex < 0 || oldIndex + count >= Count)
				throw new ArgumentOutOfRangeException(nameof(oldIndex));

			if (newIndex + count > Count || newIndex < 0)
				throw new ArgumentOutOfRangeException(nameof(newIndex));

			try
			{
				for (int i = 0; i < count; ++i)
					List.Add(List.Count, default(TStore));

				if (oldIndex < newIndex)
				{
					for (int i = List.Count - 1; i >= newIndex + count; --i)
						List[i] = List[i - count];

					for (int i = count - 1; i >= 0; --i)
						List[newIndex + count + i] = List[oldIndex + i];

					_skipStart1 = oldIndex + count;
					_skipStart2 = newIndex + count;
					_skipCount2 = count;

					for (int i = count - 1; i >= 0; --i)
					{
						--_skipStart1;
						++_skipCount1;
						--_skipCount2;

						NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, GetResultFromItem(List[_skipStart2 - _skipCount1 + count]), _skipStart2 - _skipCount1, _skipStart1));
					}

					for (int i = oldIndex; i < List.Count - count; ++i)
						List[i] = List[i + count];
				}
				else
				{
					for (int i = List.Count - 1; i >= newIndex + count; --i)
						List[i] = List[i - count];

					for (int i = 0; i < count; ++i)
						List[newIndex + i] = List[oldIndex + i + count];

					_skipStart1 = newIndex;
					_skipCount1 = count;
					_skipStart2 = oldIndex + count;

					for (int i = 0; i < count; ++i)
					{
						++_skipStart1;
						--_skipCount1;
						++_skipCount2;

						NotifyOfCollectionChange(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, GetResultFromItem(List[newIndex + i]), newIndex + i, oldIndex + i));
					}

					for (int i = oldIndex + count; i < List.Count - count; ++i)
						List[i] = List[i + count];
				}

				for (int i = 0; i < count; ++i)
					List.Remove(List.Count - 1);
			}
			finally
			{
				_skipStart1 = 0;
				_skipCount1 = 0;
				_skipStart2 = 0;
				_skipCount2 = 0;
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
