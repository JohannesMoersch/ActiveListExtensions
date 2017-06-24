using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions
{
	public interface IActiveSetList<TValue> : IActiveList<TValue>, IActiveSet<TValue>
	{
	}
}
