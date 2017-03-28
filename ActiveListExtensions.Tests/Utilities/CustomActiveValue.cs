using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Tests.Utilities
{
	public class CustomActiveValue<TValue> : IActiveValue<TValue>
	{
		private TValue _value;
		public TValue Value
		{
			get => _value;
			set
			{
				if (Equals(_value, value))
					return;
				_value = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
			}
		}

		public void Dispose() { }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
