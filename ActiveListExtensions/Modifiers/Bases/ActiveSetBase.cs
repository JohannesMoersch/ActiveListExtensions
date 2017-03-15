using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Modifiers.Bases
{
	internal abstract class ActiveSetBase<T, U> : ActiveMultiListListenerBase<T, T>
	{
		protected enum SetAction
		{
			None,
			Add,
			Remove
		}

		private class SourceSet
		{
			public int Index { get; }

			public int Count { get; private set; }

			public void IncrementCount() => ++Count;

			public void DecrementCount() => --Count;

			public SourceSet(int index) => Index = index;
		}

		private class ResultSet
		{
			public int Index { get; }

			public T Value { get; }

			public ResultSet(int index, T value)
			{
				Index = index;
				Value = value;
			}

			public bool IsSameInstance(T otherValue)
			{
				if (typeof(T).IsClass)
					return ReferenceEquals(Value, otherValue);
				return Equals(Value, otherValue);
			}
		}

		private class SourcePair
		{
			public U Key { get; }

			public T Value { get; }

			public SourcePair(U key, T value)
			{
				Key = key;
				Value = value;
			}
		}

		private int _nextIndex;

		public override int Count => _resultList.Count;

		public override T this[int index] => _resultList[index].Value;

		private readonly ObservableList<ResultSet> _resultList;

		private readonly IList<ResultSet> _cumulativeList;

		private readonly IDictionary<U, SourceSet> _leftCount;

		private readonly IDictionary<U, SourceSet> _rightCount;

		private readonly IList<SourcePair> _leftKeys;

		private readonly IList<SourcePair> _rightKeys;

		private readonly Func<T, U> _keySelector;

		public ActiveSetBase(IActiveList<T> leftSource, IActiveList<T> rightSource, Func<T, U> keySelector, IEnumerable<string> propertiesToWatch = null)
			: base(leftSource, propertiesToWatch)
		{
			_keySelector = keySelector;

			_leftKeys = new List<SourcePair>();
			_rightKeys = new List<SourcePair>();

			_cumulativeList = new List<ResultSet>();
			_leftCount = new Dictionary<U, SourceSet>();

			_resultList = new ObservableList<ResultSet>();
			_resultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(RewrapEventArgs(e));
			_resultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);

			Initialize();

			if (rightSource != null)
			{
				_rightCount = new Dictionary<U, SourceSet>();
				AddSourceCollection(0, rightSource, true);
			}
		}

		private NotifyCollectionChangedEventArgs RewrapEventArgs(NotifyCollectionChangedEventArgs args)
		{
			switch (args.Action)
			{
				case NotifyCollectionChangedAction.Add:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, ((ResultSet)args.NewItems[0]).Value, args.NewStartingIndex);
				case NotifyCollectionChangedAction.Remove:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, ((ResultSet)args.OldItems[0]).Value, args.OldStartingIndex);
				case NotifyCollectionChangedAction.Replace:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, ((ResultSet)args.NewItems[0]).Value, ((ResultSet)args.OldItems[0]).Value, args.NewStartingIndex);
				case NotifyCollectionChangedAction.Move:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, ((ResultSet)args.NewItems[0]).Value, args.NewStartingIndex, args.OldStartingIndex);
				case NotifyCollectionChangedAction.Reset:
					return new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
			}
			return null;
		}

		protected override void OnDisposed()
		{
			_resultList.Dispose();
			base.OnDisposed();
		}

		protected abstract SetAction OnAddedToLeft(bool existsInRight);

		protected abstract SetAction OnRemovedFromLeft(bool existsInRight);

		protected abstract SetAction OnAddedToRight(bool existsInLeft);

		protected abstract SetAction OnRemovedFromRight(bool existsInLeft);

		private void Add(U key, T value, IDictionary<U, SourceSet> addTo, IDictionary<U, SourceSet> other, Func<bool, SetAction> onAddedMethod)
		{
			int insertIndex;
			if (!addTo.TryGetValue(key, out SourceSet sourceSet))
			{
				sourceSet = new SourceSet(++_nextIndex);
				switch (onAddedMethod.Invoke(other.ContainsKey(key)))
				{
					case SetAction.Add:
						break;
					case SetAction.Remove:
						break;
				}
				insertIndex = _cumulativeList.Count;
			}
			else
				insertIndex = FindByIndex(_cumulativeList, sourceSet.Index) + 1;
			sourceSet.IncrementCount();

			var resultSet = new ResultSet(sourceSet.Index, value);
			_cumulativeList.Insert(insertIndex, resultSet);
		}

		private void Remove(U key, T value, IDictionary<U, SourceSet> removeFrom, IDictionary<U, SourceSet> other, Func<bool, SetAction> onRemovedMethod)
		{
			if (removeFrom.TryGetValue(key, out SourceSet sourceSet))
			{
				if (sourceSet.Count == 1)
				{
					switch (onRemovedMethod.Invoke(other.ContainsKey(key)))
					{
						case SetAction.Add:
							break;
						case SetAction.Remove:
							break;
					}
					removeFrom.Remove(key);
				}
				else
					sourceSet.DecrementCount();
			}
		}

		private int FindByIndex(IList<ResultSet> list, int index)
		{
			var bottom = 0;
			var top = list.Count - 1;
			while (bottom <= top)
			{
				var mid = bottom + (top - bottom) / 2;
				var midIndex = list[mid].Index;
				if (midIndex == index && (mid + 1 >= list.Count || list[mid + 1].Index != midIndex))
					return mid;
				else if (midIndex <= index)
					bottom = mid + 1;
				else
					top = mid - 1;
			}
			return -1;
		}

		protected override void OnAdded(int index, T value)
		{
			var key = _keySelector.Invoke(value);
			_leftKeys.Insert(index, new SourcePair(key, value));
			Add(key, value, _leftCount, _rightCount, OnAddedToLeft);
		}

		protected override void OnAdded(int collectionIndex, int index, T value)
		{
			var key = _keySelector.Invoke(value);
			_rightKeys.Insert(index, new SourcePair(key, value));
			Add(key, value, _rightCount, _leftCount, OnAddedToRight);
		}

		protected override void OnRemoved(int index, T value)
		{
			var sourcePair = _leftKeys[index];
			_leftKeys.RemoveAt(index);
			Remove(sourcePair.Key, sourcePair.Value, _leftCount, _rightCount, OnRemovedFromLeft);
		}

		protected override void OnRemoved(int collectionIndex, int index, T value)
		{
			var sourcePair = _rightKeys[index];
			_rightKeys.RemoveAt(index);
			Remove(sourcePair.Key, sourcePair.Value, _rightCount, _leftCount, OnRemovedFromRight);
		}

		protected override void OnReplaced(int index, T oldValue, T newValue)
		{
			var sourcePair = _leftKeys[index];
			Remove(sourcePair.Key, sourcePair.Value, _leftCount, _rightCount, OnRemovedFromLeft);

			var key = _keySelector.Invoke(newValue);
			_leftKeys[index] = new SourcePair(key, newValue);
			Add(key, newValue, _leftCount, _rightCount, OnAddedToLeft);
		}

		protected override void OnReplaced(int collectionIndex, int index, T oldValue, T newValue)
		{
			var sourcePair = _rightKeys[index];
			Remove(sourcePair.Key, sourcePair.Value, _rightCount, _leftCount, OnRemovedFromRight);

			var key = _keySelector.Invoke(newValue);
			_rightKeys[index] = new SourcePair(key, newValue);
			Add(key, newValue, _rightCount, _leftCount, OnAddedToRight);
		}

		protected override void OnMoved(int oldIndex, int newIndex, T value) { }

		protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, T value) { }

		protected override void OnReset(IReadOnlyList<T> newItems)
		{
			foreach (var value in _leftKeys)
				Remove(value.Key, value.Value, _leftCount, _rightCount, OnRemovedFromLeft);
			_leftKeys.Clear();
			foreach (var value in newItems)
			{
				var key = _keySelector.Invoke(value);
				_leftKeys.Add(new SourcePair(key, value));
				Add(key, value, _leftCount, _rightCount, OnAddedToLeft);
			}
		}

		protected override void OnReset(int collectionIndex, IReadOnlyList<T> newItems)
		{
			foreach (var value in _rightKeys)
				Remove(value.Key, value.Value, _rightCount, _leftCount, OnRemovedFromRight);
			_rightKeys.Clear();
			foreach (var value in newItems)
			{
				var key = _keySelector.Invoke(value);
				_rightKeys.Add(new SourcePair(key, value));
				Add(key, value, _rightCount, _leftCount, OnAddedToRight);
			}
		}

		protected override void ItemModified(int index, T value) => OnReplaced(index, value, value);

		protected override void ItemModified(int collectionIndex, int index, T value) => OnReplaced(collectionIndex, index, value, value); 

		public override IEnumerator<T> GetEnumerator() => _resultList.Select(v => v.Value).GetEnumerator();
	}
}