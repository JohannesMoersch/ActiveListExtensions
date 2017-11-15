using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveAggregate<TSource, TValue> : ActiveValueBase<TValue>
	{
		private IActiveValue<TSource> _sourceValue;

		private string[] _sourcePropertiesToWatch;

		private TSource _source;
		private TSource Source
		{
			get => _source;
			set
			{
				if (_source is INotifyPropertyChanged oldPropertyChangedSource && _sourcePropertiesToWatch != null)
				{
					foreach (var propertyName in _sourcePropertiesToWatch)
						PropertyChangedEventManager.RemoveHandler(oldPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				_source = value;

				if (_source is INotifyPropertyChanged newPropertyChangedSource && _sourcePropertiesToWatch != null)
				{
					foreach (var propertyName in _sourcePropertiesToWatch)
						PropertyChangedEventManager.AddHandler(newPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				if (_aggregatorFunction != null)
					Value = _aggregatorFunction.Invoke(Value, _source);
			}
		}

		private readonly Func<TValue, TSource, TValue> _aggregatorFunction;

		public ActiveAggregate(TValue initialValue, IActiveValue<TSource> source, Func<TValue, TSource, TValue> aggregatorFunction, IEnumerable<string> sourcePropertiesToWatch)
		{
			Value = initialValue;

			_sourceValue = source;

			_aggregatorFunction = aggregatorFunction;

			_sourcePropertiesToWatch = sourcePropertiesToWatch?.ToArray();

			PropertyChangedEventManager.AddHandler(_sourceValue, SourceChanged, nameof(IActiveValue<TSource>.Value));

			Source = _sourceValue.Value;
		}

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(_sourceValue, SourceChanged, nameof(IActiveValue<TSource>.Value));
			Source = default(TSource);
		}

		private void SourceChanged(object key, PropertyChangedEventArgs args)
		{
			if (!IsDisposed)
				Source = _sourceValue.Value;
		}

		private void SourcePropertyChanged(object key, PropertyChangedEventArgs args) 
			=> Value = _aggregatorFunction.Invoke(Value, _source);
	}
}
