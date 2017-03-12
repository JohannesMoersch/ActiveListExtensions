using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveSelectMany<T, U> : ActiveMultiListBase<T, U>
	{
		private struct ListInfo
		{
			public readonly int Offset;
			public readonly int Count;

			public ListInfo(int offset, int count)
			{
				Offset = offset;
				Count = count;
			}
		}

		private Func<T, IEnumerable<U>> _selector;

		private bool _resetting = false;

		private IList<ListInfo> _listInfo = new List<ListInfo>();

		public ActiveSelectMany(IActiveList<T> source, Func<T, IEnumerable<U>> selector, IEnumerable<string> propertiesToWatch = null) 
			: base(source, propertiesToWatch)
		{
			if (selector == null)
				throw new ArgumentNullException(nameof(selector));
			_selector = selector;
			Initialize();
		}

		private void AdjustIndices(int startIndex, int offset)
		{
			for (int i = startIndex; i < _listInfo.Count; ++i)
			{
				var info = _listInfo[i];
				_listInfo[i] = new ListInfo(info.Offset + offset, info.Count);
			}
		}

		protected override void OnAdded(int index, T value)
		{
			var enumerable = _selector.Invoke(value);
			var list = (enumerable as IReadOnlyList<U>) ?? enumerable.ToArray();

			_listInfo.Insert(index, new ListInfo(_listInfo[index].Offset, 0));

			AddSourceCollection(index, list, false);
		}

		protected override void OnRemoved(int index, T value)
		{
			RemoveSourceCollection(index);

			_listInfo.RemoveAt(index);
		}

		protected override void OnMoved(int oldIndex, int newIndex, T value)
		{
			OnRemoved(oldIndex, value);
			OnAdded(newIndex, value);
		}

		protected override void OnReplaced(int index, T oldValue, T newValue)
		{
			OnRemoved(index, oldValue);
			OnAdded(index, newValue);
		}

		protected override void OnReset(IReadOnlyList<T> newItems)
		{
			_resetting = true;
			try
			{
				for (int i = SourceLists.Count - 1; i >= 0; --i)
					RemoveSourceCollection(i);
				for (int i = 0; i < newItems.Count; ++i)
				{
					var list = _selector.Invoke(newItems[i]);
					AddSourceCollection(i, (list as IReadOnlyList<U>) ?? list.ToArray(), false);
				}
				_listInfo.Clear();
				var lastInfo = new ListInfo(0, 0);
				foreach (var list in SourceLists)
				{
					var info = new ListInfo(lastInfo.Offset + lastInfo.Count, list.Count);
					_listInfo.Add(info);
					lastInfo = info;
				}
				_listInfo.Add(new ListInfo(lastInfo.Offset + lastInfo.Count, 0));
				ResultList.Reset(SourceLists.SelectMany(s => s));
			}
			finally
			{
				_resetting = false;
			}
		}

		protected override void ItemModified(int index, T value) => OnReplaced(index, value, value);

		protected override void OnAdded(int collectionIndex, int index, U value)
		{
			var info = _listInfo[collectionIndex];
			var adjustedIndex = info.Offset + index;
			ResultList.Add(adjustedIndex, value);
			AdjustIndices(collectionIndex + 1, 1);
			_listInfo[collectionIndex] = new ListInfo(info.Offset, info.Count + 1);
		}

		protected override void OnRemoved(int collectionIndex, int index, U value)
		{
			var info = _listInfo[collectionIndex];
			var adjustedIndex = info.Offset + index;
			ResultList.Remove(adjustedIndex);
			AdjustIndices(collectionIndex + 1, -1);
			_listInfo[collectionIndex] = new ListInfo(info.Offset, info.Count - 1);
		}

		protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, U value)
		{
			var adjustedOldIndex = _listInfo[collectionIndex].Offset + oldIndex;
			var adjustedNewIndex = _listInfo[collectionIndex].Offset + newIndex;
			ResultList.Move(adjustedOldIndex, adjustedNewIndex);
		}

		protected override void OnReplaced(int collectionIndex, int index, U oldValue, U newValue)
		{
			var adjustedIndex = _listInfo[collectionIndex].Offset + index;
			ResultList.Replace(adjustedIndex, newValue);
		}

		protected override void OnReset(int collectionIndex, IReadOnlyList<U> newItems)
		{
			if (_resetting)
				return;
			var info = _listInfo[collectionIndex];
			ResultList.ReplaceRange(info.Offset, info.Count, newItems);
			AdjustIndices(collectionIndex + 1, newItems.Count - info.Count);
			_listInfo[collectionIndex] = new ListInfo(info.Offset, newItems.Count);
		}
	}
}
