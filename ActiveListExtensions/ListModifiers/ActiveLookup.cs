using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveLookup<TKey, TSource, TParameter> : ActiveListBase<TSource, ActiveLookup<TKey, TSource, TParameter>.GroupData, TParameter, IActiveGrouping<TKey, TSource>>, IActiveLookup<TKey, TSource>
	{
		internal class Group : ObservableList<ItemData, TSource>, IActiveGrouping<TKey, TSource>
		{
			public TKey Key { get; }

			public Group(TKey key) : base(i => i.Value) => Key = key;
		}

		internal class ItemData
		{
			public int SourceIndex { get; set; }

			public int TargetIndex { get; set; }

			public TKey Key { get; }

			public TSource Value { get; }

			public ItemData(TKey key, TSource value)
			{
				Key = key;
				Value = value;
			}

			public override string ToString() => $"Source:{SourceIndex}|Target:{TargetIndex}|Value:{Value}";
		}

		internal class GroupData
		{
			public int SourceIndex => Items.Count > 0 ? Items[0].SourceIndex : -1;

			public int TargetIndex { get; set; }

			public Group Items { get; }

			public GroupData(TKey key) => Items = new Group(key);
		}

		private readonly QuickList<ItemData> _sourceData;

		private readonly IDictionary<TKey, GroupData> _resultSet;

		private readonly IDictionary<TKey, GroupData> _emptyGroups;

		private readonly Func<TSource, TKey> _keySelector;

		public IActiveList<TSource> this[TKey key]
		{
			get
			{
				if (!_resultSet.TryGetValue(key, out var group) && !_emptyGroups.TryGetValue(key, out group))
				{
					group = new GroupData(key);
					_emptyGroups.Add(key, group);
				}

				return group.Items.ToActiveList();
			}
		}

		public ActiveLookup(IActiveList<TSource> source, Func<TSource, TKey> keySelector, IEnumerable<string> propertiesToWatch)
			: this(source, keySelector, null, propertiesToWatch, null)
		{
		}

		public ActiveLookup(IActiveList<TSource> source, Func<TSource, TParameter, TKey> keySelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, i => keySelector.Invoke(i, parameter.Value), parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveLookup(IActiveList<TSource> source, Func<TSource, TKey> keySelector, IActiveValue<TParameter> parameter, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, i => i.Items, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_sourceData = new QuickList<ItemData>();
			_resultSet = new Dictionary<TKey, GroupData>();
			_emptyGroups = new Dictionary<TKey, GroupData>();

			_keySelector = keySelector;

			Initialize();
		}

		public bool Contains(TKey key) => _resultSet.ContainsKey(key);

		protected override void OnAdded(int index, TSource value)
		{
			var item = new ItemData(_keySelector.Invoke(value), value);
			item.SourceIndex = index;
			_sourceData.Add(index, item);

			for (int i = index + 1; i < _sourceData.Count; ++i)
				_sourceData[i].SourceIndex = i;

			AddToGroup(item);
		}

		protected override void OnRemoved(int index, TSource value)
		{
			var item = _sourceData[index];
			_sourceData.Remove(index);

			item.SourceIndex = -1;

			for (int i = index; i < _sourceData.Count; ++i)
				_sourceData[i].SourceIndex = i;

			RemoveFromGroup(item);
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			var oldItem = _sourceData[index];
			var newKey = _keySelector.Invoke(newValue);
			if (Equals(oldItem.Key, newKey))
			{
				var newItem = new ItemData(newKey, newValue);
				newItem.TargetIndex = oldItem.TargetIndex;
				_resultSet[oldItem.Key].Items.Replace(oldItem.TargetIndex, newItem);

				_sourceData[index] = newItem;
			}
			else
			{
				OnRemoved(index, oldValue);
				OnAdded(index, newValue);
			}
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			var item = _sourceData[oldIndex];
			_sourceData.Move(oldIndex, newIndex);

			var min = oldIndex < newIndex ? oldIndex : newIndex;
			var max = oldIndex < newIndex ? newIndex : oldIndex;

			for (int i = min; i <= max; ++i)
				_sourceData[i].SourceIndex = i;

			item.SourceIndex = oldIndex;

			if (_resultSet.TryGetValue(item.Key, out GroupData group))
			{
				var oldTargetIndex = item.TargetIndex;
				item.TargetIndex = FindTargetIndex(group.Items, newIndex);
				if (item.TargetIndex > oldTargetIndex)
					--item.TargetIndex;

				item.SourceIndex = newIndex;

				group.Items.Move(oldTargetIndex, item.TargetIndex);

				min = oldTargetIndex < item.TargetIndex ? oldTargetIndex : item.TargetIndex;
				max = oldTargetIndex < item.TargetIndex ? item.TargetIndex : oldTargetIndex;

				for (int i = min; i <= max; ++i)
					group.Items[i].TargetIndex = i;

				if (oldTargetIndex == 0 || item.TargetIndex == 0)
					UpdateGroupIndex(group);
			}
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			_sourceData.Clear();

			foreach (var set in _resultSet)
			{
				try { set.Value.Items.Reset(Enumerable.Empty<ItemData>()); }
				catch { }
				set.Value.TargetIndex = 0;
				_emptyGroups.Add(set.Key, set.Value);
			}

			_resultSet.Clear();

			foreach (var value in newItems)
			{
				var item = new ItemData(_keySelector.Invoke(value), value);
				item.SourceIndex = _sourceData.Count;
				_sourceData.Add(_sourceData.Count, item);
				AddToGroup(item, true);
			}

			ResultList.Reset(_resultSet.Values.Select((g, i) =>
			{
				g.TargetIndex = i;
				return g;
			}));
		}

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);

		private void AddToGroup(ItemData item, bool isResetting = false)
		{
			bool addToResultList = !isResetting;

			if (!_resultSet.TryGetValue(item.Key, out var group))
			{
				if (!_emptyGroups.TryGetValue(item.Key, out group))
					group = new GroupData(item.Key);
				else
					_emptyGroups.Remove(item.Key);

				_resultSet.Add(item.Key, group);
			}
			else
				addToResultList = false;

			item.TargetIndex = FindTargetIndex(group.Items, item.SourceIndex);
			group.Items.Add(item.TargetIndex, item);

			for (int i = item.TargetIndex + 1; i < group.Items.Count; ++i)
				group.Items[i].TargetIndex = i;

			if (addToResultList)
			{
				group.TargetIndex = FindTargetIndexForGroup(group.SourceIndex, -1);

				if (!isResetting)
				{
					for (int i = group.TargetIndex; i < ResultList.Count; ++i)
						ResultList[i].TargetIndex = i + 1;
				}

				ResultList.Add(group.TargetIndex, group);
			}
			else if (item.TargetIndex == 0 && !isResetting)
				UpdateGroupIndex(group);
		}

		private void RemoveFromGroup(ItemData item)
		{
			if (_resultSet.TryGetValue(item.Key, out GroupData group))
			{
				var oldTargetIndex = item.TargetIndex;

				group.Items.Remove(item.TargetIndex);

				for (int i = item.TargetIndex; i < group.Items.Count; ++i)
					group.Items[i].TargetIndex = i;

				item.TargetIndex = -1;

				if (group.Items.Count == 0)
				{
					var removeIndex = group.TargetIndex;

					group.TargetIndex = 0;
					
					_emptyGroups.Add(item.Key, group);
					_resultSet.Remove(item.Key);

					for (int i = removeIndex + 1; i < ResultList.Count; ++i)
						ResultList[i].TargetIndex = i - 1;

					ResultList.Remove(removeIndex);
				}
				else if (oldTargetIndex == 0)
					UpdateGroupIndex(group);
			}
		}

		private void UpdateGroupIndex(GroupData group)
		{
			var oldTargetIndex = group.TargetIndex;
			group.TargetIndex = FindTargetIndexForGroup(group.SourceIndex, group.TargetIndex);

			if (group.TargetIndex != oldTargetIndex)
			{
				if (group.TargetIndex > oldTargetIndex)
				{
					for (int i = oldTargetIndex + 1; i <= group.TargetIndex; ++i)
						ResultList[i].TargetIndex = i - 1;
				}
				else
				{
					for (int i = group.TargetIndex; i < oldTargetIndex; ++i)
						ResultList[i].TargetIndex = i + 1;
				}

				ResultList.Move(oldTargetIndex, group.TargetIndex);
			}
		}

		private int FindTargetIndex(Group group, int sourceIndex)
		{
			var bottom = 0;
			var top = group.Count - 1;
			while (bottom <= top)
			{
				var mid = bottom + (top - bottom) / 2;
				var midItem = group[mid];
				var comparison = sourceIndex.CompareTo(midItem.SourceIndex);
				if (comparison > 0)
					bottom = mid + 1;
				else
					top = mid - 1;
			}
			return bottom;
		}

		private int FindTargetIndexForGroup(int sourceIndex, int currentIndex)
		{
			var bottom = 0;
			var top = currentIndex >= 0 ? ResultList.Count - 2 : ResultList.Count - 1;
			while (bottom <= top)
			{
				var mid = bottom + (top - bottom) / 2;
				var midItem = ResultList[currentIndex >= 0 && mid >= currentIndex ? mid + 1 : mid];
				var comparison = sourceIndex.CompareTo(midItem.SourceIndex);
				if (comparison > 0)
					bottom = mid + 1;
				else
					top = mid - 1;
			}
			return bottom;
		}
	}
}