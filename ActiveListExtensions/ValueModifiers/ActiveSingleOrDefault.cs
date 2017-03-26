using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveSingleOrDefault<TSource> : ActiveListValueBase<TSource, TSource>
	{
		private Func<TSource, bool> _predicate;

		private int _firstMatchIndex;
		private int FirstMatchIndex
		{
			get => _firstMatchIndex;
			set
			{
				_firstMatchIndex = value;
				UpdateValue();
			}
		}

		private int _secondMatchIndex;
		private int SecondMatchIndex
		{
			get => _secondMatchIndex;
			set
			{
				_secondMatchIndex = value;
				UpdateValue();
			}
		}

		public ActiveSingleOrDefault(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			: base(source, propertiesToWatch)
		{
			_predicate = predicate;

			Initialize();
		}

		private void UpdateValue() => Value = SecondMatchIndex == SourceList.Count && FirstMatchIndex < SourceList.Count ? SourceList[FirstMatchIndex] : default(TSource);

		protected override void OnAdded(int index, TSource value)
		{
			if (index <= FirstMatchIndex)
			{
				++_secondMatchIndex;
				if (_predicate.Invoke(value))
					FirstMatchIndex = index;
				else
					++_firstMatchIndex;
			}
			else if (index <= SecondMatchIndex)
			{
				if (_predicate.Invoke(value))
					SecondMatchIndex = index;
				else
					++_secondMatchIndex;
			}
		}

		protected override void OnRemoved(int index, TSource value)
		{
		}

		// TEST NOT FAILING! ALWAYS MORE THAN ONE RESULT!
		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			//_firstMatchIndex = FindFirstMatch(0);
			//SecondMatchIndex = FindFirstMatch(FirstMatchIndex);
		}

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);

		private int FindFirstMatch(int startindex)
		{
			var newIndex = SourceList.Count;

			for (int i = startindex; i < SourceList.Count; ++i)
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
