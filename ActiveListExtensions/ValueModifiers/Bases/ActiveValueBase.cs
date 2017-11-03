using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers.Bases
{
	internal abstract class ActiveValueBase<TValue> : IActiveValue<TValue>
	{
		private TValue _value;
		public TValue Value
		{
			get => _value;
			protected set
			{
				if (!_allowsThrowChangeNotifications && Equals(_value, value))
					return;

				var oldValue = _value;
				_value = value;

				ValueChanged?.Invoke(this, new ActiveValueChangedEventArgs<TValue>(oldValue, _value));

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}
		}

		private bool _allowsThrowChangeNotifications;

		protected bool IsDisposed { get; private set; }

		public ActiveValueBase(bool allowsThrowChangeNotifications = false)
			=> _allowsThrowChangeNotifications = allowsThrowChangeNotifications;

		void IDisposable.Dispose()
		{
			if (IsDisposed)
				return;
			IsDisposed = true;
			OnDisposed();
		}

		protected abstract void OnDisposed();

		public event PropertyChangedEventHandler PropertyChanged;

		public event ActiveValueChangedEventHandler<TValue> ValueChanged;
	}
}
