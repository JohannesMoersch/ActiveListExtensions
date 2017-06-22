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
				var oldValue = _value;
				_value = value;

				ValueChanged?.Invoke(this, new ActiveValueChangedEventArgs<TValue>(oldValue, _value));

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}
		}

		protected bool IsDisposed { get; private set; }

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
