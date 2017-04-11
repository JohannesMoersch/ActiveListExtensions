using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers.Bases
{
	internal abstract class ActiveListPredicateBase<TSource, TParameter> : ActiveListValueBase<TSource, TParameter, bool>
	{
		private Func<TSource, bool> _predicate;

		private int? _valueIndex;

		public ActiveListPredicateBase(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, bool> predicate, IEnumerable<string> sourcePropertiesToWatch = null, IEnumerable<string> parameterPropertiesToWatch = null)
			: base(source, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_predicate = predicate;

			Value = GetValue(false);

			Initialize();
		}

		protected abstract bool GetValue(bool predicateMet);

		protected sealed override void OnAdded(int index, TSource value)
		{
			if (_valueIndex.HasValue)
			{
				if (index <= _valueIndex)
					++_valueIndex;
			}
			else if (_predicate.Invoke(value))
				_valueIndex = index;

			Value = GetValue(_valueIndex.HasValue);
		}

		protected sealed override void OnRemoved(int index, TSource value)
		{
			if (!_valueIndex.HasValue)
				return;

			if (_valueIndex == index)
				_valueIndex = FindMatch();
			else if (index < _valueIndex)
				--_valueIndex;

			Value = GetValue(_valueIndex.HasValue);
		}

		protected sealed override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (!_valueIndex.HasValue)
				return;

			if (oldIndex == _valueIndex)
				_valueIndex = newIndex;
			else if (oldIndex < _valueIndex)
			{
				if (newIndex > _valueIndex)
					--_valueIndex;
			}
			else if (newIndex < _valueIndex)
				++_valueIndex;

			Value = GetValue(_valueIndex.HasValue);
		}

		protected sealed override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (_predicate.Invoke(newValue))
			{
				if (!_valueIndex.HasValue)
					_valueIndex = index;
			}
			else if (_valueIndex == index)
				_valueIndex = FindMatch();

			Value = GetValue(_valueIndex.HasValue);
		}

		protected sealed override void OnReset(IReadOnlyList<TSource> newItems)
		{
			_valueIndex = FindMatch();

			Value = GetValue(_valueIndex.HasValue);
		}

		protected sealed override void ItemModified(int index, TSource value) => OnReplaced(index, value, value);

		private int? FindMatch()
		{
			for (int i = 0; i < SourceList.Count; ++i)
			{
				if (_predicate.Invoke(SourceList[i]))
					return i;
			}
			return null;
		}
	}
}
