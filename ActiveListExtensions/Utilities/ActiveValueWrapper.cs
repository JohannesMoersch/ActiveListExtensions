using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class ActiveValueWrapper<TValue> : IActiveValue<TValue>
	{
		public TValue Value { get; }

		public ActiveValueWrapper(TValue value) => Value = value;

		public void Dispose() { }

		event PropertyChangedEventHandler INotifyPropertyChanged.PropertyChanged { add { } remove { } }
	}
}
