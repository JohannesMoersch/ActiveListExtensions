using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.ListModifiers.Bases
{
	internal abstract class ActiveSetBase<TKey, TSource, TParameter> : ActiveMultiListListenerBase<TSource, TSource, TParameter, TSource>, IActiveSetList<TSource>
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

			public TSource Value { get; }

			public bool InResultList { get; set; }

			public ResultSet(int index, TSource value)
			{
				Index = index;
				Value = value;
			}

			public bool IsSameInstance(TSource otherValue)
			{
				if (typeof(TSource).IsClass)
					return ReferenceEquals(Value, otherValue);
				return Equals(Value, otherValue);
			}

			public override string ToString() => $"{Index}|{Value}";
		}

		private class SourcePair
		{
			public TKey Key { get; }

			public TSource Value { get; }

			public SourcePair(TKey key, TSource value)
			{
				Key = key;
				Value = value;
			}
		}

		private int _nextIndex;

		public override int Count => _resultList.Count;

		public override TSource this[int index] => _resultList[index].Value;

		private readonly ObservableList<ResultSet> _resultList;

		private readonly List<ResultSet> _cumulativeList;

		private readonly IDictionary<TKey, SourceSet> _leftCount;

		private readonly IDictionary<TKey, SourceSet> _rightCount;

		private readonly QuickList<SourcePair> _leftKeys;

		private readonly QuickList<SourcePair> _rightKeys;

		private readonly Func<TSource, TKey> _keySelector;

		public ActiveSetBase(IActiveList<TSource> leftSource, IReadOnlyList<TSource> rightSource, Func<TSource, TKey> keySelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(leftSource, parameter, sourcePropertiesToWatch, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_keySelector = keySelector ?? throw new ArgumentNullException(nameof(keySelector));

			_leftKeys = new QuickList<SourcePair>();
			_rightKeys = new QuickList<SourcePair>();

			_cumulativeList = new List<ResultSet>();
			_leftCount = new Dictionary<TKey, SourceSet>();

			_resultList = new ObservableList<ResultSet>();
			_resultList.CollectionChanged += (s, e) => NotifyOfCollectionChange(RewrapEventArgs(e));
			_resultList.PropertyChanged += (s, e) => NotifyOfPropertyChange(e);

			if (rightSource != null)
			{
				_rightCount = new Dictionary<TKey, SourceSet>();
				AddSourceCollection(0, rightSource, true);
			}

			Initialize();
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

		public bool Contains(TSource value)
		{
			var key = _keySelector.Invoke(value);

			var existsInLeft = _leftCount.ContainsKey(key);
			var existsInRight = _rightCount?.ContainsKey(key) ?? false;

			if (existsInLeft)
			{
				if ((OnAddedToLeft(existsInRight) == SetAction.Add || OnAddedToLeft(false) == SetAction.Add) && (!existsInRight || OnAddedToRight(true) != SetAction.Remove))
					return true;
			}
			else if (existsInRight && OnAddedToRight(false) == SetAction.Add)
				return true;

			return false;
		}

		protected abstract SetAction OnAddedToLeft(bool existsInRight);

		protected abstract SetAction OnRemovedFromLeft(bool existsInRight);

		protected abstract SetAction OnAddedToRight(bool existsInLeft);

		protected abstract SetAction OnRemovedFromRight(bool existsInLeft);

		private void AddToResultList(ResultSet resultSet)
		{
			var insertIndex = FindByIndex(_resultList, resultSet.Index);
			_resultList.Add(insertIndex, resultSet);
			resultSet.InResultList = true;
		}

		private void RemoveFromResultList(int index)
		{
			var removeIndex = FindByIndex(_resultList, index);
			_resultList[removeIndex].InResultList = false;
			_resultList.Remove(removeIndex);
		}

		private void ReplaceInResultList(ResultSet resultSet)
		{
			var replaceIndex = FindByIndex(_resultList, resultSet.Index);
			_resultList[replaceIndex].InResultList = false;
			_resultList.Replace(replaceIndex, resultSet);
			resultSet.InResultList = true;
		}

		private void Add(TKey key, TSource value, IDictionary<TKey, SourceSet> addTo, IDictionary<TKey, SourceSet> other, Func<bool, SetAction> onAddedMethod)
		{
			int? insertIndex = null;
			var action = SetAction.None;
			if (!addTo.TryGetValue(key, out SourceSet sourceSet))
			{
				int? otherIndex = null;
				if (other != null && other.TryGetValue(key, out SourceSet otherSourceSet))
					otherIndex = otherSourceSet.Index;
				else
					insertIndex = _cumulativeList.Count;

				sourceSet = new SourceSet(otherIndex ?? ++_nextIndex);
				addTo.Add(key, sourceSet);

				action = onAddedMethod.Invoke(otherIndex.HasValue);
			}

			if (!insertIndex.HasValue)
				insertIndex = FindByIndex(_cumulativeList, sourceSet.Index) + 1;

			sourceSet.IncrementCount();

			var resultSet = new ResultSet(sourceSet.Index, value);
			_cumulativeList.Insert(insertIndex.Value, resultSet);

			switch (action)
			{
				case SetAction.Add:
					AddToResultList(_cumulativeList[FindFirstByIndex(_cumulativeList, insertIndex.Value, sourceSet.Index)]);
					break;
				case SetAction.Remove:
					RemoveFromResultList(sourceSet.Index);
					break;
			}
		}

		private void Remove(TKey key, TSource value, IDictionary<TKey, SourceSet> removeFrom, IDictionary<TKey, SourceSet> other, Func<bool, SetAction> onRemovedMethod)
		{
			if (removeFrom.TryGetValue(key, out SourceSet sourceSet))
			{
				var index = FindByIndex(_cumulativeList, sourceSet.Index);

				var action = SetAction.None;

				var removeIndex = index;
				while (removeIndex >= 0 && !_cumulativeList[removeIndex].IsSameInstance(value))
					--removeIndex;
				var resultSet = _cumulativeList[removeIndex];
				_cumulativeList.RemoveAt(removeIndex);

				if (sourceSet.Count == 1)
				{
					action = onRemovedMethod.Invoke(other?.ContainsKey(key) ?? false);
					removeFrom.Remove(key);
				}
				else
					sourceSet.DecrementCount();

				switch (action)
				{
					case SetAction.Add:
						AddToResultList(_cumulativeList[FindFirstByIndex(_cumulativeList, removeIndex, sourceSet.Index)]);
						break;
					case SetAction.Remove:
						RemoveFromResultList(sourceSet.Index);
						break;
					case SetAction.None:
						if (resultSet.InResultList)
							ReplaceInResultList(_cumulativeList[FindFirstByIndex(_cumulativeList, removeIndex, sourceSet.Index)]);
						break;
				}
			}
		}

		private int FindFirstByIndex(IReadOnlyList<ResultSet> list, int startPosition, int index)
		{
			if (startPosition >= list.Count)
				startPosition = list.Count - 1;
			while (startPosition > 0 && list[startPosition - 1].Index == index)
				--startPosition;
			return startPosition;
		}

		private int FindByIndex(IReadOnlyList<ResultSet> list, int index)
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
			return bottom;
		}

		protected override void OnAdded(int index, TSource value)
		{
			var key = _keySelector.Invoke(value);
			_leftKeys.Add(index, new SourcePair(key, value));
			Add(key, value, _leftCount, _rightCount, OnAddedToLeft);
		}

		protected override void OnAdded(int collectionIndex, int index, TSource value)
		{
			var key = _keySelector.Invoke(value);
			_rightKeys.Add(index, new SourcePair(key, value));
			Add(key, value, _rightCount, _leftCount, OnAddedToRight);
		}

		protected override void OnRemoved(int index, TSource value)
		{
			var sourcePair = _leftKeys[index];
			_leftKeys.Remove(index);
			Remove(sourcePair.Key, sourcePair.Value, _leftCount, _rightCount, OnRemovedFromLeft);
		}

		protected override void OnRemoved(int collectionIndex, int index, TSource value)
		{
			var sourcePair = _rightKeys[index];
			_rightKeys.Remove(index);
			Remove(sourcePair.Key, sourcePair.Value, _rightCount, _leftCount, OnRemovedFromRight);
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			var sourcePair = _leftKeys[index];
			Remove(sourcePair.Key, sourcePair.Value, _leftCount, _rightCount, OnRemovedFromLeft);

			var key = _keySelector.Invoke(newValue);
			_leftKeys[index] = new SourcePair(key, newValue);
			Add(key, newValue, _leftCount, _rightCount, OnAddedToLeft);
		}

		protected override void OnReplaced(int collectionIndex, int index, TSource oldValue, TSource newValue)
		{
			var sourcePair = _rightKeys[index];
			Remove(sourcePair.Key, sourcePair.Value, _rightCount, _leftCount, OnRemovedFromRight);

			var key = _keySelector.Invoke(newValue);
			_rightKeys[index] = new SourcePair(key, newValue);
			Add(key, newValue, _rightCount, _leftCount, OnAddedToRight);
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
			=> _leftKeys.Move(oldIndex, newIndex);

		protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, TSource value)
			=> _rightKeys.Move(oldIndex, newIndex);

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			foreach (var value in _leftKeys)
				Remove(value.Key, value.Value, _leftCount, _rightCount, OnRemovedFromLeft);
			_leftKeys.Clear();
			foreach (var value in newItems)
			{
				var key = _keySelector.Invoke(value);
				_leftKeys.Add(_leftKeys.Count, new SourcePair(key, value));
				Add(key, value, _leftCount, _rightCount, OnAddedToLeft);
			}
		}

		protected override void OnReset(int collectionIndex, IReadOnlyList<TSource> newItems)
		{
			foreach (var value in _rightKeys)
				Remove(value.Key, value.Value, _rightCount, _leftCount, OnRemovedFromRight);
			_rightKeys.Clear();
			foreach (var value in newItems)
			{
				var key = _keySelector.Invoke(value);
				_rightKeys.Add(_rightKeys.Count, new SourcePair(key, value));
				Add(key, value, _rightCount, _leftCount, OnAddedToRight);
			}
		}

		protected override void OnParameterChanged()
		{
			OnReset(SourceList);
			if (SourceLists.Count > 0)
				OnReset(0, SourceLists[0]);
		}

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);

		protected override void ItemModified(int collectionIndex, int index, TSource value) => OnReplaced(collectionIndex, index, value, value); 

		public override IEnumerator<TSource> GetEnumerator() => _resultList.Select(v => v.Value).GetEnumerator();
	}
}