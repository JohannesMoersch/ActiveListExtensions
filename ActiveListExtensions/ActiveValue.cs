using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public class ActiveValue<TValue> : IActiveValue<TValue>
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
