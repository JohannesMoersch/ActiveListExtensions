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

		private string[] _value1PropertiesToWatch;

		private string[] _value2PropertiesToWatch;

		private TValue1 _source1;
		private TValue1 Source1
		{
			get => _source1;
			set
			{
				if (_source1 is INotifyPropertyChanged oldPropertyChangedSource && _value1PropertiesToWatch != null)
				{
					foreach (var propertyName in _value1PropertiesToWatch)
						PropertyChangedEventManager.RemoveHandler(oldPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				_source1 = value;

				if (_source1 is INotifyPropertyChanged newPropertyChangedSource && _value1PropertiesToWatch != null)
				{
					foreach (var propertyName in _value1PropertiesToWatch)
						PropertyChangedEventManager.AddHandler(newPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				if (_valueCombiner != null)
					Value = _valueCombiner.Invoke(Source1, Source2);
			}
		}

		private TValue2 _source2;
		private TValue2 Source2
		{
			get => _source2;
			set
			{
				if (_source2 is INotifyPropertyChanged oldPropertyChangedSource && _value2PropertiesToWatch != null)
				{
					foreach (var propertyName in _value2PropertiesToWatch)
						PropertyChangedEventManager.RemoveHandler(oldPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				_source2 = value;

				if (_source2 is INotifyPropertyChanged newPropertyChangedSource && _value2PropertiesToWatch != null)
				{
					foreach (var propertyName in _value2PropertiesToWatch)
						PropertyChangedEventManager.AddHandler(newPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				if (_valueCombiner != null)
					Value = _valueCombiner.Invoke(Source1, Source2);
			}
		}

		public ActiveCombine(IActiveValue<TValue1> value1, IActiveValue<TValue2> value2, Func<TValue1, TValue2, TResult> valueCombiner, IEnumerable<string> value1PropertiesToWatch, IEnumerable<string> value2PropertiesToWatch)
		{
			_value1 = value1;
			_value2 = value2;
			_value1PropertiesToWatch = value1PropertiesToWatch?.ToArray();
			_value2PropertiesToWatch = value2PropertiesToWatch?.ToArray();

			PropertyChangedEventManager.AddHandler(_value1, Source1Changed, nameof(IActiveValue<TValue1>.Value));
			PropertyChangedEventManager.AddHandler(_value2, Source2Changed, nameof(IActiveValue<TValue2>.Value));

			Source1 = _value1.Value;

			_valueCombiner = valueCombiner;

			Source2 = _value2.Value;
		}

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(_value1, Source1Changed, nameof(IActiveValue<TValue1>.Value));
			PropertyChangedEventManager.RemoveHandler(_value2, Source2Changed, nameof(IActiveValue<TValue2>.Value));
			Source1 = default(TValue1);
			Source2 = default(TValue2);
		}

		private void Source1Changed(object key, PropertyChangedEventArgs args)
		{
			if (!IsDisposed)
				Source1 = _value1.Value;
		}

		private void Source2Changed(object key, PropertyChangedEventArgs args)
		{
			if (!IsDisposed)
				Source2 = _value2.Value;
		}

		private void SourcePropertyChanged(object key, PropertyChangedEventArgs args) => Value = _valueCombiner.Invoke(_value1.Value, _value2.Value);
	}
}
