using ActiveListExtensions.ListModifiers.Bases;
using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ListModifiers
{
	internal class ActiveRecastResetNotifications<TElement> : ActiveListBase<TElement, TElement, object, TElement>
	{
		private IDictionary<TElement, int> _counts = new Dictionary<TElement, int>();

		private IDictionary<TElement, IList<int>> _indexes = new Dictionary<TElement, IList<int>>();

		private IList<IList<int>> _listCache = new List<IList<int>>();

		public ActiveRecastResetNotifications(IActiveList<TElement> source)
			: base(source, i => i, null)
		{
			Initialize();
		}

		protected override void OnAdded(int index, TElement value) => ResultList.Add(index, value);

		protected override void OnMoved(int oldIndex, int newIndex, TElement value) => ResultList.Move(oldIndex, newIndex);

		protected override void OnRemoved(int index, TElement value) => ResultList.Remove(index);

		protected override void OnReplaced(int index, TElement oldValue, TElement newValue) => ResultList.Replace(index, newValue);

		protected override void OnReset(IReadOnlyList<TElement> newItems)
		{
			var max = SourceList.Count < ResultList.Count ? SourceList.Count : ResultList.Count;

			var startOffset = 0;
			while (startOffset < max && Equals(SourceList[startOffset], ResultList[startOffset]))
				++startOffset;

			var endOffset = 0;
			while (endOffset < max && Equals(SourceList[SourceList.Count - endOffset - 1], ResultList[ResultList.Count - endOffset - 1]))
				++endOffset;

			CountSourceList(startOffset, endOffset, _counts);

			RemoveExtrasFromResultList(startOffset, endOffset, _counts);

			IndexResultList(startOffset, endOffset, _indexes);

			AlignResultList(startOffset, endOffset, _counts, _indexes);

			_counts.Clear();

			foreach (var value in _indexes.Values)
				ReleaseList(value);

			_indexes.Clear();
		}

		private void CountSourceList(int startOffset, int endOffset, IDictionary<TElement, int> counts)
		{
			for (int i = startOffset; i < SourceList.Count - endOffset; ++i)
			{
				var element = SourceList[i];

				if (counts.TryGetValue(element, out var num))
					counts[element] = ++num;
				else
					counts.Add(element, 1);
			}
		}

		private void RemoveExtrasFromResultList(int startOffset, int endOffset, IDictionary<TElement, int> counts)
		{
			for (int i = ResultList.Count - endOffset - 1; i >= startOffset; --i)
			{
				var element = ResultList[i];

				if (counts.TryGetValue(element, out var num))
				{
					if (num == 1)
						counts.Remove(element);
					else
						counts[element] = num - 1;
				}
				else
					ResultList.Remove(i);
			}
		}

		private void IndexResultList(int startOffset, int endOffset, IDictionary<TElement, IList<int>> indexes)
		{
			for (int i = startOffset; i < ResultList.Count - endOffset; ++i)
			{
				var element = ResultList[i];

				if (!indexes.TryGetValue(element, out var list))
				{
					list = ClaimList();
					indexes.Add(element, list);
				}
				list.Add(i);
			}
		}

		private void AlignResultList(int startOffset, int endOffset, IDictionary<TElement, int> counts, IDictionary<TElement, IList<int>> indexes)
		{
			int mainOffset = 0;
			var offsets = IntegerMapNode.CreateRoot();

			var first = startOffset;
			var lastInResult = ResultList.Count - endOffset - 1;
			var lastInSource = SourceList.Count - endOffset - 1;
			
			while (first <= lastInSource)
			{
				while (first <= lastInResult && Equals(SourceList[first], ResultList[first]))
				{
					if (indexes.TryGetValue(ResultList[first], out var list))
						list.RemoveAt(0);
					++first;
				}

				if (first > lastInSource)
					break;

				var firstElement = SourceList[first];

				if (counts.TryGetValue(firstElement, out var num))
				{
					if (num == 1)
						counts.Remove(firstElement);
					else
						counts[firstElement] = num - 1;

					ResultList.Add(first, firstElement);
					++mainOffset;

					++lastInResult;
					++first;
					continue;
				}

				while (first <= lastInSource && lastInResult > 0 && Equals(SourceList[lastInSource], ResultList[lastInResult]))
				{
					if (indexes.TryGetValue(ResultList[lastInResult], out var list))
						list.RemoveAt(list.Count - 1);
					--lastInSource;
					--lastInResult;
				}

				if (first > lastInSource)
					break;

				var lastElement = SourceList[lastInSource];

				var firstList = indexes[firstElement];
				var firstValue = firstList.First();
				var firstResultIndex = firstValue + offsets.GetOffset(firstValue) + mainOffset;

				int lastResultIndex = Int32.MaxValue;
				int lastValue = 0;
				if (indexes.TryGetValue(lastElement, out var lastList) && lastList.Any())
				{
					lastValue = lastList.Last();
					lastResultIndex = lastValue + offsets.GetOffset(lastValue) + mainOffset;
				}

				if (firstResultIndex - first > lastInResult - lastResultIndex)
				{
					ResultList.Move(firstResultIndex, first);
					++first;
					firstList.RemoveAt(0);
					offsets.Update(firstValue, 1, IntegerMapNode.IntegerMapOffsetType.LessThanDivider);
				}
				else
				{
					ResultList.Move(lastResultIndex, lastInResult);
					--lastInResult;
					--lastInSource;
					lastList.RemoveAt(lastList.Count - 1);
					offsets.Update(lastValue + 1, -1, IntegerMapNode.IntegerMapOffsetType.GreaterThanOrEqualToDivider);
				}
			}
		}

		private IList<int> ClaimList()
		{
			if (_listCache.Count > 0)
			{
				var ret = _listCache[_listCache.Count - 1];

				_listCache.RemoveAt(_listCache.Count - 1);

				return ret;
			}
			return new List<int>();
		}

		private void ReleaseList(IList<int> list)
		{
			list.Clear();
			_listCache.Add(list);
		}
	}
}
