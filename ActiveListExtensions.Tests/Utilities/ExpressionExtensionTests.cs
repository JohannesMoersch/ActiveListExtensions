using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using ActiveListExtensions.Utilities;

namespace ActiveListExtensions.Tests.Utilities
{
	public class ExpressionExtensionTests
	{
		[Fact]
		public void AccessStaticFieldsWithoutException()
		{
			Expression<Func<Guid, bool>> blah = id => id == Guid.Empty;

			Assert.Empty(blah.GetReferencedProperties());
		}
	}
}
