using ActiveListExtensions.ListModifiers.Bases;
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
			var first = startOffset;
			var last = ResultList.Count - endOffset - 1;
			/*
			while (first <= last)
			{
				var firstElement = SourceList[first];

				if (counts.TryGetValue(firstElement, out var num) && !Equals(ResultList[i], firstElement))
				{
					if (num == 1)
						counts.Remove(firstElement);
					else
						counts[firstElement] = num - 1;

					ResultList.Add(i, firstElement);
				}
			}*/
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
