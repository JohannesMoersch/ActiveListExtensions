using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
    public interface IMutableActiveList<T> : IActiveList<T>
    {
        void Add(int index, T value);

        void Remove(int index);

        void Replace(int index, T newValue);

        void Move(int oldIndex, int newIndex);

        void Reset(IEnumerable<T> values);
    }
}
