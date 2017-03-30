using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers.Bases
{
	internal abstract class ActiveListCompareBase<TSource, TValue> : ActiveListValueBase<TSource, TValue>
	{
		private readonly Func<TSource, TValue> _selector;

		private int _currentIndex = -1;

		public ActiveListCompareBase(IActiveList<TSource> source, Func<TSource, TValue> selector, IEnumerable<string> propertiesToWatch = null)
			: base(source, propertiesToWatch)
		{
			_selector = selector;
		}

		protected abstract int Compare(TValue leftValue, TValue rightValue);

		protected sealed override void OnAdded(int index, TSource value)
		{
			var selectedValue = _selector.Invoke(value);

			if (SourceList.Count == 1 || Compare(selectedValue, Value) > 0)
			{
				Value = selectedValue;
				_currentIndex = index;
			}
			else if (index <= _currentIndex)
				++_currentIndex;
		}

		protected sealed override void OnRemoved(int index, TSource value)
		{
			if (index == _currentIndex)
				Value = FindMaxValue(out _currentIndex);
			else if (index < _currentIndex)
				--_currentIndex;
		}

		protected sealed override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (oldIndex == _currentIndex)
				_currentIndex = newIndex;
			else if (oldIndex < _currentIndex)
			{
				if (newIndex > _currentIndex)
					--_currentIndex;
			}
			else if (newIndex < _currentIndex)
				++_currentIndex;
		}

		protected sealed override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			var selectedValue = _selector.Invoke(newValue);

			if (Compare(selectedValue, Value) >= 0)
			{
				Value = selectedValue;
				_currentIndex = index;
			}
			else if (index == _currentIndex)
				Value = FindMaxValue(out _currentIndex);
		}

		protected sealed override void OnReset(IReadOnlyList<TSource> newItems) => Value = FindMaxValue(out _currentIndex);

		protected sealed override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);

		private TValue FindMaxValue(out int index)
		{
			index = -1;
			TValue maxValue = default(TValue);

			for (int i = 0; i < SourceList.Count; ++i)
			{
				var selectedValue = _selector.Invoke(SourceList[i]);
				if (index < 0 || Compare(selectedValue, maxValue) > 0)
				{
					maxValue = selectedValue;
					index = i;
				}
			}

			return maxValue;
		}
	}
}