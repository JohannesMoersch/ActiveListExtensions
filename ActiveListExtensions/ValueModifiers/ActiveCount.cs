﻿using ActiveListExtensions.Utilities;
using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveCount<TSource, TParameter> : ActiveListValueBase<TSource, TParameter, int>
	{
		private readonly Func<TSource, bool> _predicate;

		private readonly BooleanList _values;

		public ActiveCount(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			: this(source, null, predicate, propertiesToWatch, null)
		{
		}

		public ActiveCount(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, parameter, i => predicate.Invoke(i, parameter.Value), sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveCount(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) 
			: base(source, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_predicate = predicate;

			_values = new BooleanList();

			Initialize();
		}

		protected override void OnAdded(int index, TSource value)
		{
			if (_predicate.Invoke(value))
			{
				_values.Insert(index, true);
				++Value;
			}
			else
				_values.Insert(index, false);
		}

		protected override void OnRemoved(int index, TSource value)
		{
			var currentValue = _values[index];
			_values.RemoveAt(index);
			if (currentValue)
				--Value;
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
					++Value;
			}
			else if (_values[index])
			{
				_values[index] = false;
				--Value;
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

			Value = newCount;
		}

		protected override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);
	}
}