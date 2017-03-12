using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Modifiers.Bases;

namespace ActiveListExtensions.Modifiers
{
	internal class ActiveUnion<T, U> : ActiveSetBase<T, U> 
	{
		public ActiveUnion(IActiveList<T> leftSource, IActiveList<T> rightSource, Func<T, U> keySelector, IEnumerable<string> propertiesToWatch = null) 
			: base(leftSource, rightSource, keySelector, propertiesToWatch)
		{
		}

		protected override SetAction OnAddedToLeft(bool existsInRight) => !existsInRight ? SetAction.Add : SetAction.None;

		protected override SetAction OnAddedToRight(bool existsInLeft) => !existsInLeft ? SetAction.Add : SetAction.None;

		protected override SetAction OnRemovedFromLeft(bool existsInRight) => !existsInRight ? SetAction.Remove : SetAction.None;

		protected override SetAction OnRemovedFromRight(bool existsInLeft) => !existsInLeft ? SetAction.Remove : SetAction.None;
	}
}
