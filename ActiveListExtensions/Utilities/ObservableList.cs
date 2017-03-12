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
	internal class ObservableList<T> : IReadOnlyList<T>, IDisposable, INotifyPropertyChanged, INotifyCollectionChanged
	{
		private int _skipStart, _skipCount;

		public T this[int index]
		{
			get
			{
				if (index >= _skipStart)
					index += _skipCount;
				return _list[index];
			}
		}

		public int Count => _list.Count - _skipCount;

		private IList<T> _list = new List<T>();

		public void Dispose()
		{
			PropertyChanged = null;
			CollectionChanged = null;
			_list.Clear();
		}

		public void Add(int index, T value)
		{
			_list.Insert(index, value);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, value, index));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		public void Remove(int index)
		{
			var value = _list[index];
			_list.RemoveAt(index);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, value, index));
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		public void Replace(int index, T newValue)
		{
			var oldValue = _list[index];
			_list[index] = newValue;
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, newValue, oldValue, index));
		}

		public void Move(int oldIndex, int newIndex)
		{
			var value = _list[oldIndex];
			_list.RemoveAt(oldIndex);
			_list.Insert(newIndex, value);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, value, newIndex, oldIndex));
		}

		public void Reset(IEnumerable<T> values)
		{
			var oldCount = Count;
			_list.Clear();
			foreach (var value in values)
				_list.Add(value);
			CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			if (oldCount != Count)
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Count)));
		}

		public void ReplaceRange(int startIndex, int oldCount, IReadOnlyList<T> newValues)
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
						_list.Add(default(T));
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
						CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, newValues[i], startIndex + i));
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
						CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _list[i], i));
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

		public event PropertyChangedEventHandler PropertyChanged;
		public event NotifyCollectionChangedEventHandler CollectionChanged;

		private IEnumerable<T> EnumerateCollection()
		{
			for (int i = 0; i < Count; ++i)
				yield return this[i];
		}

		public IEnumerator<T> GetEnumerator() => EnumerateCollection().GetEnumerator();

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
