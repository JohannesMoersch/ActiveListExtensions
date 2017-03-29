using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveAny<TSource> : ActiveListValueBase<TSource, bool>
	{
		private readonly Func<TSource, bool> _predicate;

		private readonly List<bool> _values;

		private int _count;
		private int Count
		{
			get => _count;
			set
			{
				_count = value;
				Value = _count > 0;
			}
		}

		public ActiveAny(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch = null) 
			: base(source, propertiesToWatch)
		{
			_predicate = predicate;

			_values = new List<bool>();

			Initialize();
		}

		protected override void OnAdded(int index, TSource value)
		{
			if (_predicate.Invoke(value))
			{
				_values.Insert(index, true);
				++Count;
			}
			else
				_values.Insert(index, false);
		}

		protected override void OnRemoved(int index, TSource value)
		{
			var currentValue = _values[index];
			_values.RemoveAt(index);
			if (currentValue)
				--Count;
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			var currentValue = _values[oldIndex];
			_values.RemoveAt(oldIndex);
			_values.Insert(newIndex, currentValue);
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (_predicate.Invoke(newValue))
			{
				var currentValue = _values[index];
				_values[index] = true;
				if (!currentValue)
					++Count;
			}
			else if (_values[index])
			{
				_values[index] = false;
				--Count;
			}
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
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

			Count = newCount;
		}

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);
	}
}
