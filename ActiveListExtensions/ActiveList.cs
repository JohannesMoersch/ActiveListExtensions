using ActiveListExtensions.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
    public static class ActiveList
    {
        public static IMutableActiveList<TItem> Create<TItem>() => new ObservableList<TItem>();

        public static IMutableActiveList<TItem> Create<TItem>(IEnumerable<TItem> items)
        {
            var list = Create<TItem>();
            list.Reset(items);
            return list;
        }
    }
}
