using ActiveListExtensions.Tests.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveMutateValueTests
	{
		[Fact]
		public void WhenMutatingContainerValueChanged()
		{
			var source = new ActiveMutateValueTestOuterClass() { Container = new IntegerTestClass() { Property = 100 } };
			var sut = source.ToActiveValue(c => c.Container).ActiveMutate(c => c.Property);

			source.Container = new IntegerTestClass() { Property = 200 };

			Assert.Equal(source.Container.Property, sut.Value);
		}

		[Fact]
		public void WhenMutatingPropertyValueChanged()
		{
			var source = new ActiveMutateValueTestOuterClass() { Container = new IntegerTestClass() { Property = 100 } };
			var sut = source.ToActiveValue(c => c.Container).ActiveMutate(c => c.Property);

			source.Container.Property = 200;

			Assert.Equal(source.Container.Property, sut.Value);
		}

		[Fact]
		public void WhenMutatingPropertyChangeNotificationIsThrown()
		{
			var source = new ActiveMutateValueTestOuterClass() { Container = new IntegerTestClass() { Property = 100 } };
			var sut = source.ToActiveValue(c => c.Container).ActiveMutate(c => c.Property);

			bool called = false;

			sut.PropertyChanged += (s, e) => called = true;

			source.Container.Property = 200;

			Assert.True(called);
		}
	}

	public class ActiveMutateValueTestOuterClass : INotifyPropertyChanged
	{
		private IntegerTestClass _container;
		public IntegerTestClass Container
		{
			get { return _container; }
			set
			{
				if (_container == value)
					return;
				_container = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Container)));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
