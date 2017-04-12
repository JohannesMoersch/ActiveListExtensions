using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveOrderBy<TSource, TKey> : ActiveListBase<TSource, ActiveOrderBy<TSource, TKey>.ItemSet, ListSortDirection, TSource>
		where TKey : IComparable<TKey>
	{
		internal class ItemSet
		{
			public int SourceIndex { get; set; }

			public int TargetIndex { get; set; }

			public TKey Key { get; }

			public TSource Value { get; }

			public ItemSet(TKey key, TSource value)
			{
				Key = key;
				Value = value;
			}
		}

		private readonly List<ItemSet> _sourceList;

		private readonly Func<TSource, TKey> _keySelector;

		public ActiveOrderBy(IActiveList<TSource> source, Func<TSource, TKey> keySelector, IActiveValue<ListSortDirection> sortDirection, IEnumerable<string> propertiesToWatch) 
			: base(source, i => i.Value, sortDirection, propertiesToWatch)
		{
			_keySelector = keySelector;

			_sourceList = new List<ItemSet>();

			Initialize();
		}

		private int FindByKey(TKey key, int sourceIndex)
		{
			var bottom = 0;
			var top = ResultList.Count - 1;
			while (bottom <= top)
			{
				var mid = bottom + (top - bottom) / 2;
				var midItem = ResultList[mid];
				var comparison = key.CompareTo(midItem.Key) * (ParameterValue == ListSortDirection.Ascending ? 1 : -1);
				if (comparison == 0)
					comparison = sourceIndex.CompareTo(midItem.SourceIndex);
				if (comparison > 0)
					bottom = mid + 1;
				else
					top = mid - 1;
			}
			return bottom;
		}

		private void UpdateSourceIndexes(int startIndex, int? endIndex = null)
		{
			if (!endIndex.HasValue)
				endIndex = _sourceList.Count - 1;
			for (int i = startIndex; i <= endIndex; ++i)
				_sourceList[i].SourceIndex = i;
		}

		private void UpdateTargetIndexes(int startIndex, int? endIndex = null)
		{
			if (!endIndex.HasValue)
				endIndex = ResultList.Count - 1;
			for (int i = startIndex; i <= endIndex; ++i)
				ResultList[i].TargetIndex = i;
		}

		protected override void OnAdded(int index, TSource value)
		{
			var key = _keySelector.Invoke(value);
			var targetIndex = FindByKey(key, index);
			var itemSet = new ItemSet(key, value)
			{
				SourceIndex = index,
				TargetIndex = targetIndex
			};
			_sourceList.Insert(index, itemSet);
			ResultList.Add(targetIndex, itemSet);

			UpdateSourceIndexes(index + 1);
			UpdateTargetIndexes(targetIndex + 1);
		}

		protected override void OnRemoved(int index, TSource value)
		{
			var itemSet = _sourceList[index];
			_sourceList.RemoveAt(itemSet.SourceIndex);
			ResultList.Remove(itemSet.TargetIndex);

			UpdateSourceIndexes(itemSet.SourceIndex);
			UpdateTargetIndexes(itemSet.TargetIndex);
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			var oldItemSet = _sourceList[index];

			var newKey = _keySelector.Invoke(newValue);

			var newTargetIndex = FindByKey(newKey, index);
			if (newTargetIndex > oldItemSet.TargetIndex)
				--newTargetIndex;

			var newItemSet = new ItemSet(newKey, newValue)
			{
				SourceIndex = index,
				TargetIndex = newTargetIndex
			};

			_sourceList[index] = newItemSet;

			if (newItemSet.TargetIndex == oldItemSet.TargetIndex)
				ResultList.Replace(newItemSet.TargetIndex, newItemSet);
			else
			{
				ResultList.Remove(oldItemSet.TargetIndex);
				ResultList.Add(newItemSet.TargetIndex, newItemSet);

				if (oldItemSet.TargetIndex < newItemSet.TargetIndex)
					UpdateTargetIndexes(oldItemSet.TargetIndex, newItemSet.TargetIndex - 1);
				else
					UpdateTargetIndexes(newItemSet.TargetIndex + 1, oldItemSet.TargetIndex);
			}
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			var itemSet = _sourceList[oldIndex];

			var newTargetIndex = FindByKey(itemSet.Key, newIndex);
			if (newTargetIndex > itemSet.TargetIndex)
				--newTargetIndex;

			_sourceList.RemoveAt(oldIndex);
			_sourceList.Insert(newIndex, itemSet);

			if (oldIndex < newIndex)
				UpdateSourceIndexes(oldIndex, newIndex);
			else
				UpdateSourceIndexes(newIndex, oldIndex);
			
			if (itemSet.TargetIndex != newTargetIndex)
			{
				ResultList.Remove(itemSet.TargetIndex);
				ResultList.Add(newTargetIndex, itemSet);

				if (itemSet.TargetIndex < newTargetIndex)
					UpdateTargetIndexes(itemSet.TargetIndex, newTargetIndex);
				else
					UpdateTargetIndexes(newTargetIndex, itemSet.TargetIndex);
			}
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			_sourceList.Clear();
			for (int i = 0; i < newItems.Count; ++i)
				_sourceList.Add(new ItemSet(_keySelector.Invoke(newItems[i]), newItems[i]) { SourceIndex = i });

			IEnumerable<ItemSet> sortedItems;
			if (ParameterValue == ListSortDirection.Ascending)
				sortedItems = _sourceList.OrderBy(set => set.Key);
			else
				sortedItems = _sourceList.OrderByDescending(set => set.Key);

			sortedItems = sortedItems.Select((set, index) =>
			{
				set.TargetIndex = index;
				return set;
			});
			
			ResultList.Reset(sortedItems);
		}

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);
	}
}