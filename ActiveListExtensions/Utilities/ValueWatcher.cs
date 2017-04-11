using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ValueWatcher<TValue> : IDisposable
	{
		private IActiveValue<TValue> _activeValue;

		private TValue _value;
		public TValue Value
		{
			get => _value;
			private set
			{
				if (_value is INotifyPropertyChanged oldPropertyChangedSource && _propertiesToWatch != null)
				{
					foreach (var propertyName in _propertiesToWatch)
						PropertyChangedEventManager.RemoveHandler(oldPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				_value = value;

				if (_value is INotifyPropertyChanged newPropertyChangedSource && _propertiesToWatch != null)
				{
					foreach (var propertyName in _propertiesToWatch)
						PropertyChangedEventManager.AddHandler(newPropertyChangedSource, SourcePropertyChanged, propertyName);
				}

				ValueOrValuePropertyChanged?.Invoke();
			}
		}

		private IEnumerable<string> _propertiesToWatch;

		public ValueWatcher(IActiveValue<TValue> value, IEnumerable<string> propertiesToWatch)
		{
			_activeValue = value ?? throw new ArgumentNullException(nameof(value));

			_propertiesToWatch = propertiesToWatch;

			PropertyChangedEventManager.AddHandler(_activeValue, SourceChanged, nameof(IActiveValue<TValue>.Value));

			Value = _activeValue.Value;
		}

		public void Dispose()
		{
			if (_activeValue == null)
				return;

			Value = default(TValue);

			PropertyChangedEventManager.RemoveHandler(_activeValue, SourceChanged, nameof(IActiveValue<TValue>.Value));

			_activeValue = null;
			_propertiesToWatch = null;
		}


		private void SourceChanged(object key, PropertyChangedEventArgs args)
		{
			if (_activeValue == null)
				return;

			Value = _activeValue.Value;
		}

		private void SourcePropertyChanged(object key, PropertyChangedEventArgs args) => ValueOrValuePropertyChanged?.Invoke();

		public event Action ValueOrValuePropertyChanged;
	}
}
