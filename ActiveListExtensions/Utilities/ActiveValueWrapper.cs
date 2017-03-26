using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	public class ActiveValueWrapper<TValue> : IActiveValue<TValue>
	{
		public TValue Value { get; }

		public ActiveValueWrapper(TValue value) => Value = value;

		public void Dispose() { }

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
