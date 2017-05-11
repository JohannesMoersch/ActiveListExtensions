using System;
using System.Collections;
using System.Collections.Generic;

namespace ActiveListExtensions.Utilities
{
	internal struct QuickListEnumerator<T> : IEnumerator<T>, IEnumerator
	{
		private T _current;
		public T Current => _current;

		object IEnumerator.Current => _current;

		private QuickList<T> _quickList;

		private int _index;

		private int _version;

		public QuickListEnumerator(QuickList<T> quickList)
		{
			_current = default(T);
			_quickList = quickList;
			_index = 0;
			_version = quickList.Version;
		}

		public void Dispose() { }

		public bool MoveNext()
		{
			if (_version != _quickList.Version)
				throw new Exception("Collection changed during enumeration.");
			if (_index < _quickList.Count)
			{
				_current = _quickList[_index];
				++_index;
				return true;
			}
			_current = default(T);
			return false;
		}

		public void Reset()
		{
			_index = 0;
			_current = default(T);
		}
	}
}
