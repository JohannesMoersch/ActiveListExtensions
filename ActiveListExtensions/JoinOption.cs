using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public static class JoinOption
	{
		public static JoinOption<T> Some<T>(T value)
			=> new JoinOption<T>(value);

		public static JoinOption<T> None<T>()
			=> new JoinOption<T>();
	}

	public struct JoinOption<T>
	{
		private T _value;

		public T Value => HasValue ? _value : throw new InvalidOperationException("Option object must have a value.");

		public bool HasValue { get; }

		public JoinOption(T value)
		{
			_value = value;
			HasValue = true;
		}
	}
}
