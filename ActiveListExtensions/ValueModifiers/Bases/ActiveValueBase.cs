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
				if (Equals(_value, value))
					return;
				_value = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}
		}

		public abstract void Dispose();

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
