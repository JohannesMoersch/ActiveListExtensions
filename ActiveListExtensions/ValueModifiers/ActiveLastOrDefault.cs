using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveLastOrDefault<TSource> : ActiveListValueBase<TSource, TSource>
	{
		private Func<TSource, bool> _predicate;

		private int _lastMatchIndex = -1;
		private int LastMatchIndex
		{
			get => _lastMatchIndex;
			set
			{
				_lastMatchIndex = value;
				if (_lastMatchIndex >= 0)
					Value = SourceList[_lastMatchIndex];
				else
					Value = default(TSource);
			}
		}

		public ActiveLastOrDefault(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			: base(source, propertiesToWatch)
		{
			_predicate = predicate;

			Initialize();
		}

		protected override void OnAdded(int index, TSource value)
		{
			if (index <= LastMatchIndex)
				++_lastMatchIndex;
			else if (_predicate.Invoke(value))
				LastMatchIndex = index;
		}

		protected override void OnRemoved(int index, TSource value)
		{
			if (index == LastMatchIndex)
				LastMatchIndex = FindLastMatch(index - 1);
			else if (index < LastMatchIndex)
				--_lastMatchIndex;
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (oldIndex == LastMatchIndex)
			{
				if (newIndex > oldIndex)
					_lastMatchIndex = newIndex;
				else
					LastMatchIndex = FindLastMatch(oldIndex);
			}
			else if (oldIndex > LastMatchIndex)
			{
				if (newIndex <= LastMatchIndex)
					++_lastMatchIndex;
			}
			else if (newIndex >= LastMatchIndex)
			{
				if (_predicate.Invoke(value))
					LastMatchIndex = newIndex;
				else
					--_lastMatchIndex;
			}
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (index >= LastMatchIndex)
			{
				if (_predicate.Invoke(newValue))
					LastMatchIndex = index;
				else if (index == LastMatchIndex)
					LastMatchIndex = FindLastMatch(index - 1);
			}
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems) => LastMatchIndex = FindLastMatch(SourceList.Count - 1);

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);

		private int FindLastMatch(int startindex)
		{
			var newIndex = -1;

			for (int i = startindex; i >= 0; --i)
			{
				if (!_predicate.Invoke(SourceList[i]))
					continue;
				newIndex = i;
				break;
			}

			return newIndex;
		}
	}
}