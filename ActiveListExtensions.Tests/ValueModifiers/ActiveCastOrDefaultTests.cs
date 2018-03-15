using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveCastOrDefaultTests
	{
		private class TypeBase { }
		private class TypeA : TypeBase { }
		private class TypeB : TypeBase { }

		[Fact]
		public void ValueTypeIsIncorrectAndThenCorrect()
		{
			var value = ActiveValue.Create<TypeBase>(new TypeA());

			var sut = value.ActiveCastOrDefault<TypeA>();

			Assert.Equal(value.Value, sut.Value);

			value.Value = new TypeB();

			Assert.Null(sut.Value);
		}

		[Fact]
		public void ValueTypeIsCorrectAndThenIncorrect()
		{
			var value = ActiveValue.Create<TypeBase>(new TypeA());

			var sut = value.ActiveCastOrDefault<TypeB>();

			Assert.Null(sut.Value);

			value.Value = new TypeB();

			Assert.Equal(value.Value, sut.Value);
		}

		[Fact]
		public void ValueTypeIsNull()
		{
			var value = ActiveValue.Create<TypeBase>(null);

			var sut = value.ActiveCastOrDefault<TypeA>();

			Assert.Null(sut.Value);

			value.Value = new TypeA();

			Assert.Equal(value.Value, sut.Value);
		}
	}
}
