using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveCombine<TValue1, TValue2, TResult> : ActiveValueBase<TResult>
	{
		private IActiveValue<TValue1> _value1;

		private IActiveValue<TValue2> _value2;

		private Func<TValue1, TValue2, TResult> _valueCombiner;

		public ActiveCombine(IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Func<TValue1, TValue2, TResult> valueCombiner)
		{
			_value1 = value1;
			_value2 = value2;
			_valueCombiner = valueCombiner;

			Value = _valueCombiner.Invoke(_value1.Value, _value2.Value);

			PropertyChangedEventManager.AddHandler(_value1, SourcePropertyChanged, nameof(IActiveValue<TValue1>.Value));
			PropertyChangedEventManager.AddHandler(_value2, SourcePropertyChanged, nameof(IActiveValue<TValue2>.Value));
		}

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(_value1, SourcePropertyChanged, nameof(IActiveValue<TValue1>.Value));
			PropertyChangedEventManager.RemoveHandler(_value2, SourcePropertyChanged, nameof(IActiveValue<TValue2>.Value));
		}

		private void SourcePropertyChanged(object key, PropertyChangedEventArgs args) => Value = _valueCombiner.Invoke(_value1.Value, _value2.Value);
	}
}
