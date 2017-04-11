using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveSingleOrDefault<TSource, TParameter> : ActiveListValueBase<TSource, TParameter, TSource>
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
			: this(source, null, predicate, propertiesToWatch, null)
		{
		}

		public ActiveSingleOrDefault(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, TParameter, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: this(source, parameter, i => predicate.Invoke(i, parameter.Value), sourcePropertiesToWatch, parameterPropertiesToWatch)
		{
		}

		private ActiveSingleOrDefault(IActiveList<TSource> source, IActiveValue<TParameter> parameter, Func<TSource, bool> predicate, IEnumerable<string> sourcePropertiesToWatch, IEnumerable<string> parameterPropertiesToWatch)
			: base(source, parameter, sourcePropertiesToWatch, parameterPropertiesToWatch)
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
			if (index < FirstMatchIndex)
			{
				--_firstMatchIndex;
				--_secondMatchIndex;
			}
			else if (index == FirstMatchIndex)
			{
				_firstMatchIndex = SecondMatchIndex - 1;
				SecondMatchIndex = FindFirstMatch(FirstMatchIndex + 1);
			}
			else if (index < SecondMatchIndex)
				--_secondMatchIndex;
			else if (index == SecondMatchIndex)
				SecondMatchIndex = FindFirstMatch(index);
		}

		protected override void OnMoved(int oldIndex, int newIndex, TSource value)
		{
			if (oldIndex < FirstMatchIndex)
			{
				if (newIndex >= FirstMatchIndex)
					--_firstMatchIndex;
				if (newIndex >= SecondMatchIndex)
					--_secondMatchIndex;
			}
			else if (oldIndex == FirstMatchIndex)
			{
				if (newIndex < SecondMatchIndex)
					_firstMatchIndex = newIndex;
				else if (newIndex == SecondMatchIndex)
					FirstMatchIndex = SecondMatchIndex - 1;
				else
				{
					_firstMatchIndex = SecondMatchIndex - 1;
					SecondMatchIndex = FindFirstMatch(FirstMatchIndex + 1);
				}
			}
			else if (oldIndex < SecondMatchIndex)
			{
				if (newIndex <= FirstMatchIndex)
					++_firstMatchIndex;
				else if (newIndex >= SecondMatchIndex)
					--_secondMatchIndex;
			}
			else if (oldIndex == SecondMatchIndex)
			{
				if (newIndex <= FirstMatchIndex)
				{
					var oldFirstIndex = FirstMatchIndex;
					_firstMatchIndex = newIndex;
					SecondMatchIndex = oldFirstIndex + 1;
				}
				else if (newIndex <= SecondMatchIndex)
					_secondMatchIndex = newIndex;
				else
					SecondMatchIndex = FindFirstMatch(SecondMatchIndex);
			}
			else
			{
				if (newIndex <= FirstMatchIndex)
				{
					if (_predicate.Invoke(value))
					{
						var oldFirstIndex = FirstMatchIndex;
						_firstMatchIndex = newIndex;
						SecondMatchIndex = oldFirstIndex + 1;
					}
					else
					{
						++_firstMatchIndex;
						++_secondMatchIndex;
					}
				}
				else if (newIndex <= SecondMatchIndex)
				{
					if (_predicate.Invoke(value))
						SecondMatchIndex = newIndex;
					else
						++_secondMatchIndex;
				}
			}
		}

		protected override void OnReplaced(int index, TSource oldValue, TSource newValue)
		{
			if (index < FirstMatchIndex)
			{
				if (_predicate.Invoke(newValue))
				{
					_secondMatchIndex = FirstMatchIndex;
					FirstMatchIndex = index;
				}
			}
			else if (index == FirstMatchIndex)
			{
				if (!_predicate.Invoke(newValue))
				{
					var oldSecondIndex = SecondMatchIndex;

					if (SecondMatchIndex < SourceList.Count)
						_secondMatchIndex = FindFirstMatch(FirstMatchIndex + 1);

					FirstMatchIndex = oldSecondIndex;
				}
			}
			else if (index <= SecondMatchIndex)
			{
				if (_predicate.Invoke(newValue))
					SecondMatchIndex = index;
				else if (index == SecondMatchIndex)
					SecondMatchIndex = FindFirstMatch(index + 1);
			}
		}

		protected override void OnReset(IReadOnlyList<TSource> newItems)
		{
			_firstMatchIndex = FindFirstMatch(0);
			SecondMatchIndex = FindFirstMatch(FirstMatchIndex + 1);
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
