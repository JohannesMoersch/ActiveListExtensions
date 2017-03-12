using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveConcat<T> : ActiveMultiListBase<T, T>
	{
		private int _concatIndex;

		public ActiveConcat(IActiveList<T> source, IEnumerable<T> concat)
			: base(source)
		{
			if (concat == null)
				throw new ArgumentNullException(nameof(concat));
			Initialize();
			AddSourceCollection(0, (concat as IReadOnlyList<T>) ?? concat.ToArray());
		}

		protected override void OnAdded(int collectionIndex, int index, T value) => ResultList.Add(_concatIndex + index, value);

		protected override void OnRemoved(int collectionIndex, int index, T value) => ResultList.Remove(_concatIndex + index);

		protected override void OnReplaced(int collectionIndex, int index, T oldValue, T newValue) => ResultList.Replace(_concatIndex + index, newValue);

		protected override void OnMoved(int collectionIndex, int oldIndex, int newIndex, T value) => ResultList.Move(_concatIndex + oldIndex, _concatIndex + newIndex);

		protected override void OnReset(int collectionIndex, IReadOnlyList<T> newItems) => ResultList.ReplaceRange(_concatIndex, Count - _concatIndex, newItems);

		protected override void OnAdded(int index, T value)
		{
			++_concatIndex;
			ResultList.Add(index, value);
		}

		protected override void OnRemoved(int index, T value)
		{
			--_concatIndex;
			ResultList.Remove(index);
		}

		protected override void OnReplaced(int index, T oldValue, T newValue)
		{
			ResultList.Replace(index, newValue);
		}

		protected override void OnMoved(int oldIndex, int newIndex, T value)
		{
			ResultList.Move(oldIndex, newIndex);
		}

		protected override void OnReset(IReadOnlyList<T> newItems)
		{
			ResultList.ReplaceRange(0, _concatIndex, newItems);
			_concatIndex = newItems.Count;
		}
	}
}
