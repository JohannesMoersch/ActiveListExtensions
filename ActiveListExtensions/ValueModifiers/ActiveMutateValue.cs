using ActiveListExtensions.Utilities;
using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveMutateValue<TValue, TResult> : ActiveValueBase<TResult>
	{
		private Func<TValue, TResult> _mutator;

		private ValueWatcher<TValue> _valueWatcher;

		public ActiveMutateValue(IActiveValue<TValue> value, Func<TValue, TResult> mutator, IEnumerable<string> propertiesToWatch)
		{
			_mutator = mutator;

			_valueWatcher = new ValueWatcher<TValue>(value, propertiesToWatch);
			_valueWatcher.ValueOrValuePropertyChanged += () => Value = _mutator.Invoke(_valueWatcher.Value);

			Value = _mutator.Invoke(_valueWatcher.Value);
		}

		protected override void OnDisposed() => _valueWatcher.Dispose();
	}
}
