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
		private IActiveValue<TValue> _value;

		private Func<TValue, TResult> _mutator;

		private string[] _propertiesToWatch;

		private TValue _source;
		private TValue Source
		{
			get => _source;
			set
			{
				if (_source is INotifyPropertyChanged oldPropertyChangedSource && _propertiesToWatch != null)
				{
					foreach (var propertyName in _propertiesToWatch)
						PropertyChangedEventManager.RemoveHandler(oldPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				_source = value;

				if (_source is INotifyPropertyChanged newPropertyChangedSource && _propertiesToWatch != null)
				{
					foreach (var propertyName in _propertiesToWatch)
						PropertyChangedEventManager.AddHandler(newPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				Value = _mutator.Invoke(Source);
			}
		}

		public ActiveMutateValue(IActiveValue<TValue> value, Func<TValue, TResult> mutator, IEnumerable<string> propertiesToWatch)
		{
			_value = value;
			_mutator = mutator;
			_propertiesToWatch = propertiesToWatch?.ToArray();

			PropertyChangedEventManager.AddHandler(_value, SourceChanged, nameof(IActiveValue<TValue>.Value));

			Source = _value.Value;
		}

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(_value, SourceChanged, nameof(IActiveValue<TValue>.Value));
			Source = default(TValue);
		}

		private void SourceChanged(object key, PropertyChangedEventArgs args)
		{
			if (!IsDisposed)
				Source = _value.Value;
		}

		private void SourcePropertyChanged(object key, PropertyChangedEventArgs args) => Value = _mutator.Invoke(Source);
	}
}
