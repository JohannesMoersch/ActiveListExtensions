using ActiveListExtensions.ListModifiers.Bases;
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

		private IList<Item> _indexes;

		public ActiveElementsOrEmpty(IActiveList<TSource> source, IReadOnlyList<int> indexes)
			: base(source, null)
		{
			if (indexes == null)
				throw new ArgumentNullException(nameof(indexes));

			_indexes = new List<Item>();

			AddSourceCollection(0, indexes);

			Initialize();
		}

		protected override void OnAdded(int index, TSource value)
		{
			int currentAdjustment = 0;
			for (int elementIndex = 0; elementIndex < SourceLists[0].Count; ++elementIndex)
			{
				var item = _indexes[elementIndex];
				var sourceIndex = SourceLists[0][elementIndex];

				var sourceListCount = SourceList.Count - 1;

				if (sourceIndex == sourceListCount)
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
			throw new NotImplementedException();
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			throw new NotImplementedException();
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			throw new NotImplementedException();
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems) => ResultList.Reset(ElementsAtOrEmpty(SourceList, SourceLists[0]));

		protected override void OnAdded(int collectionIndex, int index, int value)
		{
			if (value >= 0 && value < SourceList.Count)
			{
				var item = GetItem(index, true);
				_indexes.Insert(index, item);
				AdjustIndexes(index + 1, 1);
				ResultList.Add(item.Index, SourceList[value]);
			}
			else
				_indexes.Insert(index, GetItem(index, false));
		}

		protected override void OnRemoved(int collectionIndex, int index, int value)
		{
			throw new NotImplementedException();
		}

		protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, int value)
		{
			throw new NotImplementedException();
		}

		protected override void OnReplaced(int collectionIndex, int index, int oldValue, int newValue)
		{
			throw new NotImplementedException();
		}

		protected override void OnReset(int collectionIndex, IReadOnlyList<int> newItems) => ResultList.Reset(ElementsAtOrEmpty(SourceList, SourceLists[0]));

		private IEnumerable<TSource> ElementsAtOrEmpty(IReadOnlyList<TSource> listOne, IReadOnlyList<int> listTwo)
		{
			foreach (var index in listTwo)
			{
				if (index >= 0 && index < listOne.Count)
					yield return listOne[index];
			}
		}

		private Item GetItem(int index, bool inList)
		{
			if (index <= 0)
				return new Item(inList, 0);
			var item = _indexes[index - 1];
			return new Item(inList, item.InList ? item.Index + 1 : item.Index);
		}

		private void AdjustIndexes(int index, int adjustment)
		{
			for (int i = _indexes.Count - 1; i >= index; --i)
			{
				var oldItem = _indexes[i];
				var newIndex = oldItem.Index + adjustment;
				if (oldItem.InList && newIndex >= SourceList.Count)
				{
					_indexes[i] = new Item(false, newIndex);
					ResultList.Remove(oldItem.Index);
				}
				else
					_indexes[i] = new Item(oldItem.InList, newIndex);
			}
		}
	}
}
