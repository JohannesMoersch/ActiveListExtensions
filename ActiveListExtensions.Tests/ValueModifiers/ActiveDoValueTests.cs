using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveListExtensions.Tests.Helpers;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveDoValueTests
	{
		[Fact]
		public void ExecutesImmediately()
		{
			var value = ActiveValue.Create(new IntegerTestClass());

			var sut = value.ActiveDo(o => o.SetProperty(10));

			Assert.Equal(10, sut.Value.Property);
		}

		[Fact]
		public void ExecutesOnValueChange()
		{
			var value = ActiveValue.Create(new IntegerTestClass());

			var sut = value.ActiveDo(o => o.SetProperty(10));

			value.Value = new IntegerTestClass();

			Assert.Equal(10, sut.Value.Property);
		}

		[Fact]
		public void ExecutesOnValuePropertyChange()
		{
			var value = ActiveValue.Create(new IntegerTestClass());

			var sut = value.ActiveDo(o => o.SetProperty(o.Key));

			value.Value.Key = 10;

			Assert.Equal(10, sut.Value.Property);
		}

		[Fact]
		public void ExecutesOnParameterChange()
		{
			var value = ActiveValue.Create(new IntegerTestClass());

			var parameter = ActiveValue.Create(new IntegerTestClass());

			var sut = value.ActiveDo(parameter, (o, p) => o.SetProperty(p.Property));

			parameter.Value = new IntegerTestClass() { Property = 10 };

			Assert.Equal(10, sut.Value.Property);
		}

		[Fact]
		public void ExecutesOnParameterPropertyChange()
		{
			var value = ActiveValue.Create(new IntegerTestClass());

			var parameter = ActiveValue.Create(new IntegerTestClass());

			var sut = value.ActiveDo(parameter, (o, p) => o.SetProperty(p.Property));

			parameter.Value.Property = 10;

			Assert.Equal(10, sut.Value.Property);
		}
	}
}
