using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Reactive
{
	public class ObservableToActiveValue<T> : IObserver<T>, IActiveValue<T>
	{
		private T _value;
		public T Value
		{
			get => _value;
			set
			{
				if (Equals(_value, value))
					return;

				var oldValue = _value;
				_value = value;

				_valueChanged?.Invoke(this, new ActiveValueChangedEventArgs<T>(oldValue, _value));

				_propertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}
		}

		private bool _isDisposed;

		public void Dispose()
		{
			if (_isDisposed)
				return;
			_isDisposed = true;
			_valueChanged = null;
			_propertyChanged = null;
			Disposed?.Invoke();
			Disposed = null;
		}

		public void OnCompleted() => Dispose();

		public void OnError(Exception error) { }

		public void OnNext(T value)
		{
			if (!_isDisposed)
				Value = value;
		}

		private ActiveValueChangedEventHandler<T> _valueChanged;
		public event ActiveValueChangedEventHandler<T> ValueChanged
		{
			add
			{
				if (!_isDisposed)
					_valueChanged += value;
			}
			remove
			{
				if (!_isDisposed)
					_valueChanged -= value;
			}
		}

		private PropertyChangedEventHandler _propertyChanged;
		public event PropertyChangedEventHandler PropertyChanged
		{
			add
			{
				if (!_isDisposed)
					_propertyChanged += value;
			}
			remove
			{
				if (!_isDisposed)
					_propertyChanged -= value;
			}
		}

		internal event Action Disposed;
	}
}
