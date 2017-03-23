using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveConcat<TSource> : ActiveMultiListBase<TSource, TSource, TSource>
	{
		private int _concatIndex;

		public ActiveConcat(IActiveList<TSource> source, IEnumerable<TSource> concat)
			: base(source)
		{
			if (concat == null)
				throw new ArgumentNullException(nameof(concat));
			Initialize();
			AddSourceCollection(0, (concat as IReadOnlyList<TSource>) ?? concat.ToArray());
		}

		protected override void OnAdded(int collectionIndex, int index, TSource value) => ResultList.Add(_concatIndex + index, value);

		protected override void OnRemoved(int collectionIndex, int index, TSource value) => ResultList.Remove(_concatIndex + index);

		protected override void OnReplaced(int collectionIndex, int index, TSource oldValue, TSource newValue) => ResultList.Replace(_concatIndex + index, newValue);

		protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, TSource value) => ResultList.Move(_concatIndex + oldIndex, _concatIndex + newIndex);

		protected override void OnReset(int collectionIndex, IReadOnlyList<TSource> newItems) => ResultList.ReplaceRange(_concatIndex, Count - _concatIndex, newItems);

		protected override void OnAdded(int index, TSource value)
		{
			++_concatIndex;
			ResultList.Add(index, value);
		}

		protected override void OnRemoved(int index, TSource value)
		{
			--_concatIndex;
			ResultList.Remove(index);
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			ResultList.Replace(index, newValue);
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			ResultList.Move(oldIndex, newIndex);
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			ResultList.ReplaceRange(0, _concatIndex, newItems);
			_concatIndex = newItems.Count;
		}
	}
}
