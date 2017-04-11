using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveFirstOrDefault<TSource, TParameter> : ActiveListValueBase<TSource, TParameter, TSource>
	{
		private Func<TSource, bool> _predicate;

		private int _firstMatchIndex;
		private int FirstMatchIndex
		{
			get => _firstMatchIndex;
			set
			{
				_firstMatchIndex = value;
				if (_firstMatchIndex < SourceList.Count)
					Value = SourceList[_firstMatchIndex];
				else
					Value = default(TSource);
			}
		}

		public ActiveFirstOrDefault(IActiveList<TSource> source, Func<TSource, bool> predicate, IEnumerable<string> propertiesToWatch)
			: this(source, null, predicate, propertiesToWatch, null)
		{
		}

		public ActiveFirstOrDefault(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, parameter, i => predicate.Invoke(i, parameter.Value), sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveFirstOrDefault(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch) 
			: base(source, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
			_predicate = predicate;

			Initialize();
		}

		protected override void OnAdded(int index, TSource value)
		{
			if (index <= FirstMatchIndex)
			{
				if (_predicate.Invoke(value))
					FirstMatchIndex = index;
				else
					++_firstMatchIndex;
			}
		}

		protected override void OnRemoved(int index, TSource value)
		{
			if (index == FirstMatchIndex)
				FirstMatchIndex = FindFirstMatch(index);
			else if (index < FirstMatchIndex)
				--_firstMatchIndex;
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (oldIndex == FirstMatchIndex)
			{
				if (newIndex < oldIndex)
					_firstMatchIndex = newIndex;
				else
					FirstMatchIndex = FindFirstMatch(oldIndex);
			}
			else if (oldIndex < FirstMatchIndex)
			{
				if (newIndex >= FirstMatchIndex)
					--_firstMatchIndex;
			}
			else if (newIndex <= FirstMatchIndex)
			{
				if (_predicate.Invoke(value))
					FirstMatchIndex = newIndex;
				else
					++_firstMatchIndex;
			}
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (index <= FirstMatchIndex)
			{
				if (_predicate.Invoke(newValue))
					FirstMatchIndex = index;
				else if (index == FirstMatchIndex)
					FirstMatchIndex = FindFirstMatch(index + 1);
			}
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems) => FirstMatchIndex = FindFirstMatch(0);

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
