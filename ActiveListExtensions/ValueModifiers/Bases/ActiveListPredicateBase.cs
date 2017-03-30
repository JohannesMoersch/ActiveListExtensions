using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers.Bases
{
	internal abstract class ActiveListPredicateBase<TSource, TResult> : ActiveListValueBase<TSource, TResult>
	{
		private readonly Func<TSource, bool> _predicate;

		private readonly List<bool> _values;

		private int _count;

		public ActiveListPredicateBase(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch = null)
			: base(source, propertiesToWatch)
		{
			_predicate = predicate;

			_values = new List<bool>();

			Value = GetValue(_count);
		}

		protected abstract TResult GetValue(int count);

		protected sealed override void OnAdded(int index, TSource value)
		{
			if (_predicate.Invoke(value))
			{
				_values.Insert(index, true);
				++_count;
			}
			else
				_values.Insert(index, false);

			Value = GetValue(_count);
		}

		protected sealed override void OnRemoved(int index, TSource value)
		{
			var currentValue = _values[index];
			_values.RemoveAt(index);
			if (currentValue)
				--_count;

			Value = GetValue(_count);
		}

		protected sealed override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			var currentValue = _values[oldIndex];
			_values.RemoveAt(oldIndex);
			_values.Insert(newIndex, currentValue);

			Value = GetValue(_count);
		}

		protected sealed override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (_predicate.Invoke(newValue))
			{
				var currentValue = _values[index];
				_values[index] = true;
				if (!currentValue)
					++_count;
			}
			else if (_values[index])
			{
				_values[index] = false;
				--_count;
			}

			Value = GetValue(_count);
		}

		protected sealed override void OnReset(IReadOnlyList<TSource> newItems)
		{
			_values.Clear();

			var newCount = 0;
			for (int i = 0; i < newItems.Count; ++i)
			{
				if (_predicate.Invoke(newItems[i]))
				{
					_values.Add(true);
					++newCount;
				}
				else
					_values.Add(false);
			}

			_count = newCount;

			Value = GetValue(_count);
		}

		protected sealed override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);
	}
}