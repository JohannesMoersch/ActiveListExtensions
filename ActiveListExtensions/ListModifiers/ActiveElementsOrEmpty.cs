using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveElementsOrEmpty<TSource> : ActiveMultiListBase<TSource, int, object, TSource>
	{
		private struct Item
		{
			public bool InList { get; }
			public int Index { get; }

			public Item(bool inList, int index)
			{
				InList = inList;
				Index = index;
			}
		}

		private QuickList<Item> _indexes;

		public ActiveElementsOrEmpty(IActiveList<TSource> source, IReadOnlyList<int> indexes)
			: base(source, null)
		{
			if (indexes == null)
				throw new ArgumentNullException(nameof(indexes));

			_indexes = new QuickList<Item>();

			Initialize();

			AddSourceCollection(0, indexes);
		}

		protected override void OnAdded(int index, TSource value)
		{
			int currentAdjustment = 0;
			for (int elementIndex = 0; elementIndex < _indexes.Count; ++elementIndex)
			{
				var item = _indexes[elementIndex];
				var sourceIndex = SourceLists[0][elementIndex];

				if (sourceIndex == SourceList.Count - 1)
				{
					item = new Item(true, item.Index + currentAdjustment++);
					_indexes[elementIndex] = item;
					ResultList.Add(item.Index, SourceList[sourceIndex]);
				}
				else 
				{
					if (currentAdjustment > 0)
					{
						item = new Item(item.InList, item.Index + currentAdjustment);
						_indexes[elementIndex] = item;
					}
					if (item.InList)
						ResultList.Replace(item.Index, SourceList[sourceIndex]);
				}
			}
		}

		protected override void OnRemoved(int index, TSource value)
		{
			int currentAdjustment = 0;
			for (int elementIndex = 0; elementIndex < _indexes.Count; ++elementIndex)
			{
				var item = _indexes[elementIndex];
				var sourceIndex = SourceLists[0][elementIndex];

				if (sourceIndex == SourceList.Count)
				{
					item = new Item(false, item.Index + currentAdjustment--);
					_indexes[elementIndex] = item;
					ResultList.Remove(item.Index);
				}
				else
				{
					if (currentAdjustment < 0)
					{
						item = new Item(item.InList, item.Index + currentAdjustment);
						_indexes[elementIndex] = item;
					}
					if (item.InList)
						ResultList.Replace(item.Index, SourceList[sourceIndex]);
				}
			}
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			int min, max;
			if (oldIndex < newIndex)
			{
				min = oldIndex;
				max = newIndex;
			}
			else
			{
				min = newIndex;
				max = oldIndex;
			}
			for (int elementIndex = 0; elementIndex < _indexes.Count; ++elementIndex)
			{
				var sourceIndex = SourceLists[0][elementIndex];
				if (sourceIndex >= min && sourceIndex <= max)
					ResultList.Replace(_indexes[elementIndex].Index, SourceList[sourceIndex]);
			}
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			for (int elementIndex = 0; elementIndex < _indexes.Count; ++elementIndex)
			{
				if (index == SourceLists[0][elementIndex])
					ResultList.Replace(_indexes[elementIndex].Index, newValue);
			}
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems) => Reset();

		protected override void OnAdded(int collectionIndex, int index, int value)
		{
			if (value >= 0 && value < SourceList.Count)
			{
				var item = GetItem(index, true);
				_indexes.Add(index, item);
				RecalculateIndexes(index + 1, _indexes.Count - 1);
				ResultList.Add(item.Index, SourceList[value]);
			}
			else
				_indexes.Add(index, GetItem(index, false));
		}

		protected override void OnRemoved(int collectionIndex, int index, int value)
		{
			var item = _indexes[index];
			_indexes.Remove(index);
			if (item.InList)
			{
				RecalculateIndexes(index, _indexes.Count - 1);
				ResultList.Remove(item.Index);
			}
		}

		protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, int value)
		{
			var oldItem = _indexes[oldIndex];

			_indexes.Move(oldIndex, newIndex);

			if (oldItem.InList)
			{
				if (oldIndex < newIndex)
					RecalculateIndexes(oldIndex, newIndex);
				else
					RecalculateIndexes(newIndex, oldIndex);

				var newItem = _indexes[newIndex];

				ResultList.Move(oldItem.Index, newItem.Index);
			}
			else
				_indexes[newIndex] = GetItem(newIndex, false);
		}

		protected override void OnReplaced(int collectionIndex, int index, int oldValue, int newValue)
		{
			var nowInList = newValue >= 0 && newValue < SourceList.Count;
			var item = _indexes[index];
			if (item.InList != nowInList)
			{
				_indexes[index] = GetItem(index, nowInList);
				if (nowInList)
					ResultList.Add(item.Index, SourceList[newValue]);
				else
					ResultList.Remove(item.Index);
				RecalculateIndexes(index + 1, _indexes.Count - 1);
			}
			else if (nowInList)
				ResultList.Replace(item.Index, SourceList[newValue]);
		}

		protected override void OnReset(int collectionIndex, IReadOnlyList<int> newItems) => Reset();

		private void Reset()
		{
			if (SourceLists.Count == 0)
				return;

			ResultList.Reset(GetResetValues());
		}

		private IEnumerable<TSource> GetResetValues()
		{
			_indexes.Clear();

			int count = 0;
			foreach (var index in SourceLists[0])
			{
				if (index >= 0 && index < SourceList.Count)
				{
					var item = new Item(true, count++);
					_indexes.Add(_indexes.Count, item);
					yield return SourceList[index];
				}
				else
					_indexes.Add(_indexes.Count, new Item(false, count));
			}
		}

		private Item GetItem(int index, bool inList)
		{
			if (index <= 0)
				return new Item(inList, 0);
			var item = _indexes[index - 1];
			return new Item(inList, item.InList ? item.Index + 1 : item.Index);
		}

		private void RecalculateIndexes(int startIndex, int endIndex)
		{
			var nextIndex = GetItem(startIndex, false).Index;

			for (int i = startIndex; i <= endIndex; ++i)
			{
				var oldItem = _indexes[i];

				if (oldItem.InList)
					_indexes[i] = new Item(true, nextIndex++);
				else
					_indexes[i] = new Item(false, nextIndex);
			}
		}
	}
}
