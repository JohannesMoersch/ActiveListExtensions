using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Tests.Helpers
{
	public class IntegerTestClass : INotifyPropertyChanged
	{
		private int _key;
		public int Key
		{
			get { return _key; }
			set
			{
				if (_key == value)
					return;
				_key = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Key)));
			}
		}

		private int _property;
		public int Property
		{
			get { return _property; }
			set
			{
				if (_property == value)
					return;
				_property = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Property)));
			}
		}

		public void SetProperty(int value)
			=> Property = value;

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
