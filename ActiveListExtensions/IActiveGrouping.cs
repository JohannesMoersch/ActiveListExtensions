using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public interface IActiveGrouping<out TKey, out TSource> : IActiveList<TSource>
	{
		TKey Key { get; }
	}
}
