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
	internal class ObservableList<T> : ObservableList<T, T>
	{
		public ObservableList()
			: base(i => i)
		{
		}
	}

	internal class ObservableList<TSource, TResult> : IActiveList<TResult>
	{
		private int _skipStart, _skipCount;

		public TSource this[int index]
		{
			get
			{
				if (index >= _skipStart)
					index += _skipCount;
				return _list[index];
			}
		}

		TResult IReadOnlyList<TResult>.this[int index] => GetResultFromItem(this[index]);

		public int Count => _list.Count - _skipCount;

		private IList<TSource> _list = new List<TSource>();

		private readonly Func<TSource, TResult> _itemSelector;

		public ObservableList(Func<TSource, TResult> itemSelector)
		{
			_itemSelector = itemSelector;
		}

		public void Dispose()
		{
			PropertyChanged = null;
			CollectionChanged = null;
			_list.Clear();
		}

		public void Add(int index, TSource value)
		{
			_list.Insert(index, value);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetResultFromItem(value), index));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		public void Remove(int index)
		{
			var value = _list[index];
			_list.RemoveAt(index);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, GetResultFromItem(value), index));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		public void Replace(int index, TSource newValue)
		{
			var oldValue = _list[index];
			_list[index] = newValue;
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, GetResultFromItem(newValue), GetResultFromItem(oldValue), index));
		}

		public void Move(int oldIndex, int newIndex)
		{
			if (oldIndex == newIndex)
				return;
			var value = _list[oldIndex];
			_list.RemoveAt(oldIndex);
			_list.Insert(newIndex, value);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, GetResultFromItem(value), newIndex, oldIndex));
		}

		public void Reset(IEnumerable<TSource> values)
		{
			var oldCount = Count;
			_list.Clear();
			foreach (var value in values)
				_list.Add(value);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			if (oldCount != Count)
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		public void ReplaceRange(int startIndex, int oldCount, IReadOnlyList<TSource> newValues)
		{
			try
			{
				var top = _list.Count - 1;
				var bottom = startIndex + oldCount;
				var diff = newValues.Count - oldCount;

				int min = diff < 0 ? newValues.Count : oldCount;
				for (int i = 0; i < min; ++i)
					Replace(startIndex + i, newValues[i]);

				if (diff > 0)
				{
					for (int i = 0; i < diff; ++i)
						_list.Add(default(TSource));
					for (int i = top; i >= bottom; --i)
						_list[i + diff] = _list[i];
					for (int i = oldCount; i < newValues.Count; ++i)
						_list[startIndex + i] = newValues[i];

					_skipStart = startIndex + oldCount;
					_skipCount = newValues.Count - oldCount;
					for (int i = oldCount; i < newValues.Count; ++i)
					{
						--_skipCount;
						++_skipStart;
						CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, GetResultFromItem(newValues[i]), startIndex + i));
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
						CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, GetResultFromItem(_list[i]), i));
						--_skipStart;
					}

					for (int i = bottom; i <= top; ++i)
						_list[i + diff] = _list[i];
					for (int i = 0; i < -diff; ++i)
						_list.RemoveAt(_list.Count - 1);
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

		private TResult GetResultFromItem(TSource item) => _itemSelector.Invoke(item);

		public event PropertyChangedEventHandler PropertyChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private IEnumerable<TSource> EnumerateCollection()
		{
			for (int i = 0; i < Count; ++i)
				yield return this[i];
		}

		public IEnumerator<TSource> GetEnumerator() => EnumerateCollection().GetEnumerator();

		IEnumerator<TResult> IEnumerable<TResult>.GetEnumerator() => EnumerateCollection().Select(i => GetResultFromItem(i)).GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => EnumerateCollection().Select(i => GetResultFromItem(i)).GetEnumerator();
	}
}
