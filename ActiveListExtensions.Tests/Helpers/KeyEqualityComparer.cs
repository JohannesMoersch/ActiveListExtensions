using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Tests.Helpers
{
	public class KeyEqualityComparer<T> : IEqualityComparer<T>
	{
		private Func<T, object> _keySelector;

		private Func<T, T, bool> _additionalComparer;

		public KeyEqualityComparer(Func<T, object> keySelector, Func<T, T, bool> additionalComparer)
		{
			_keySelector = keySelector;
			_additionalComparer = additionalComparer;
		}

		public int GetHashCode(T obj) => _keySelector?.Invoke(obj).GetHashCode() ?? obj.GetHashCode();

		public bool Equals(T x, T y) => Equals(_keySelector?.Invoke(x) ?? x, _keySelector?.Invoke(y) ?? y) && (_additionalComparer?.Invoke(x, y) ?? true);
	}
}
