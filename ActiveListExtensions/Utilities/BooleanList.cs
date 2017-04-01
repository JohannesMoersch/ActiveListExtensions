using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal class BooleanList : IList<bool>
	{
		public bool this[int index]
		{
			get
			{
				if (index < 0 || index >= _count)
					throw new IndexOutOfRangeException();

				return ((_data[index >> 6] >> index % 64) & 0x1) > 0;
			}
			set
			{
				if (index < 0 || index >= _count)
					throw new IndexOutOfRangeException();

				var mask = (ulong)0x1 << (index % 64);

				index >>= 6;

				var newValue = _data[index] & ~mask;
				if (value)
					newValue |= mask;

				_data[index] = newValue;
			}
		}

		private int _capacity = 4 << 6;

		private int _count;
		public int Count
		{
			get => _count;
			private set
			{
				_count = value;

				if (_count >= _capacity)
				{
					var oldCapacity = _capacity;

					while (_count >= _capacity)
						_capacity *= 2;

					var oldData = _data;
					_data = new ulong[_capacity >> 6];
					Array.Copy(oldData, _data, oldCapacity >> 6);
				}
			}
		}

		bool ICollection<bool>.IsReadOnly => false;

		private ulong[] _data = new ulong[4];

		public void Add(bool item)
		{
			++Count;
			this[_count - 1] = item;
		}

		public void Insert(int index, bool item)
		{
			if (index < 0 || index > _count)
				throw new ArgumentOutOfRangeException();

			++Count;

			var max = (_count - 1) >> 6;
			var start = index >> 6;

			var carry = _data[start] & 0x8000000000000000;
			var mask = 0xFFFFFFFFFFFFFFFF << (index % 64);

			_data[start] = ((_data[start] << 1) & (mask << 1)) | (_data[start] & ~mask);

			for (int i = start + 1; i <= max; ++i)
			{
				var nextCarry = _data[i] & 0x8000000000000000;
				_data[i] = (_data[i] << 1) | (carry >> 63);
				carry = nextCarry;
			}

			this[index] = item;
		}

		public bool Remove(bool item)
		{
			var index = IndexOf(item);
			if (index < 0)
				return false;
			RemoveAt(index);
			return true;
		}

		public void RemoveAt(int index)
		{
			--Count;

			var max = _count >> 6;
			var start = index >> 6;

			var carry = (ulong)0;
			var mask = 0xFFFFFFFFFFFFFFFF << (index % 64);

			for (int i = max; i > start; --i)
			{
				var nextCarry = _data[i] & 0x0000000000000001;
				_data[i] = (_data[i] >> 1) | (carry << 63);
				carry = nextCarry;
			}

			_data[start] = ((_data[start] >> 1) & mask) | (_data[start] & ~mask) | (carry << 63);
		}

		public void Clear() => Count = 0;

		public bool Contains(bool item)
		{
			for (int i = 0; i < _count; ++i)
			{
				if (this[i] == item)
					return true;
			}
			return false;
		}

		public int IndexOf(bool item)
		{
			for (int i = 0; i < _count; ++i)
			{
				if (this[i] == item)
					return i;
			}
			return -1;
		}

		public void CopyTo(bool[] array, int arrayIndex)
		{
			int index = arrayIndex;
			foreach (var value in this)
				array[index++] = value;
		}

		public IEnumerator<bool> GetEnumerator()
		{
			int index = 0;
			for (int i = 0; i < _data.Length; ++i)
			{
				var value = _data[i];

				var mask = (ulong)0x1;

				for (int j = 0; j < 64; ++j)
				{
					if (index++ == _count)
						yield break;

					yield return (value & mask) >> j > 0;

					mask <<= 1;
				}
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}