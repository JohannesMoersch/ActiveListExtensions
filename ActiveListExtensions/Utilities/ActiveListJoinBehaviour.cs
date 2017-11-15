using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	[Flags]
	public enum ActiveListJoinBehaviour
	{
		None = 0,
		Inner = 1,
		LeftExcluding = 2,
		RightExcluding = 4,

		Left = Inner | LeftExcluding,
		Right = Inner | RightExcluding,
		Outer = Inner | LeftExcluding | RightExcluding,
		OuterExcluding = LeftExcluding | RightExcluding
	}
}
