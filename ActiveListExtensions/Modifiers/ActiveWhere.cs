using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveWhere<T> : ActiveListBase<T, T>
	{
		private IList<int> _indexList;

		private Func<T, bool> _predicate;

		public ActiveWhere(IActiveList<T> source, Func<T, bool> predicate, IEnumerable<string> propertiesToWatch = null) 
			: base(source, propertiesToWatch)
		{
			if (predicate == null)
				throw new ArgumentNullException(nameof(predicate));
			_indexList = new List<int>();
			_predicate = predicate;
			Initialize();
		}

		private int FindTargetIndex(int index)
		{
			for (int i = index - 1; i >= 0; --i)
			{
				if (_indexList[i] >= 0)
					return _indexList[i] + 1;
			}
			return 0;
		}

		private int AddIndex(int index, bool updateOnly)
		{
			var targetIndex = FindTargetIndex(index);
			if (updateOnly)
				_indexList[index] = targetIndex;
			else
				_indexList.Insert(index, targetIndex);
			for (int i = index + 1; i < _indexList.Count; ++i)
			{
				if (_indexList[i] >= 0)
					++_indexList[i];
			}
			return targetIndex;
		}

		private void RemoveIndex(int index, bool updateOnly)
		{
			if (updateOnly)
				_indexList[index] = -1;
			else
				_indexList.RemoveAt(index);
			for (int i = index; i < _indexList.Count; ++i)
			{
				if (_indexList[i] >= 0)
					--_indexList[i];
			}
		}

		protected override void OnAdded(int index, T value)
		{
			if (_predicate.Invoke(value))
				ResultList.Add(AddIndex(index, false), value);
			else
				_indexList.Insert(index, -1);
		}

		protected override void OnRemoved(int index, T value)
		{
			var targetIndex = _indexList[index];
			if (targetIndex >= 0)
			{
				RemoveIndex(index, false);
				ResultList.Remove(targetIndex);
			}
			else
				_indexList.RemoveAt(index);
		}

		protected override void OnMoved(int oldIndex, int newIndex, T value)
		{
			var oldTargetIndex = _indexList[oldIndex];
			if (oldTargetIndex >= 0)
			{

				RemoveIndex(oldIndex, false);
				var newTargetIndex = AddIndex(newIndex, false);
				ResultList.Move(oldTargetIndex, newTargetIndex);
			}
			else
			{
				_indexList.RemoveAt(oldIndex);
				_indexList.Insert(newIndex, -1);
			}
		}

		protected override void OnReplaced(int index, T oldValue, T newValue)
		{
			var targetIndex = _indexList[index];
			if (_predicate.Invoke(newValue))
			{
				if (targetIndex >= 0)
					ResultList.Replace(targetIndex, newValue);
				else
					ResultList.Add(AddIndex(index, true), newValue);

			}
			else if (targetIndex >= 0)
			{
				RemoveIndex(index, true);
				ResultList.Remove(targetIndex);
			}
		}

		protected override void ItemModified(int index, T value)
		{
			var targetIndex = _indexList[index];
			if (_predicate.Invoke(value))
			{
				if (targetIndex < 0)
					ResultList.Add(AddIndex(index, true), value);
			}
			else if (targetIndex >= 0)
			{
				RemoveIndex(index, true);
				ResultList.Remove(targetIndex);
			}
		}

		protected override void OnReset(IReadOnlyList<T> newItems) => ResultList.Reset(ResetEnumerable(newItems));

		private IEnumerable<T> ResetEnumerable(IReadOnlyList<T> newItems)
		{
			_indexList.Clear();
			int index = 0;
			foreach (var item in newItems)
			{
				if (_predicate.Invoke(item))
				{
					yield return item;
					_indexList.Add(index++);
				}
				else
					_indexList.Add(-1);
			}
		}
	}
}
