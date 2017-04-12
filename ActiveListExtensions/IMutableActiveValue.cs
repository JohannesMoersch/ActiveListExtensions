using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public interface IMutableActiveValue<T> : IActiveValue<T>
	{
		new T Value { get; set; }
	}
}
