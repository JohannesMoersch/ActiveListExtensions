using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public static class ActiveValue
	{
		public static IMutableActiveValue<TValue> Create<TValue>() => new ActiveValue<TValue>(default(TValue));

		public static IMutableActiveValue<TValue> Create<TValue>(TValue value) => new ActiveValue<TValue>(value);
	}

	internal class ActiveValue<TValue> : IMutableActiveValue<TValue>
	{
		private TValue _value;
		public TValue Value
		{
			get => _value;
			set
			{
				if (Equals(_value, value))
					return;

				var oldValue = _value;
				_value = value;

				ValueChanged?.Invoke(this, new ActiveValueChangedEventArgs<TValue>(oldValue, _value));

				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}
		}

		public ActiveValue()
		{
		}

		public ActiveValue(TValue initialValue)
		{
			Value = initialValue;
		}

		public void Dispose() { }

		public event PropertyChangedEventHandler PropertyChanged;

		public event ActiveValueChangedEventHandler<TValue> ValueChanged;
	}
}
