using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveSelectValueTests
	{
		[Fact]
		public void WhenMutatingContainerValueChanged()
		{
			var source = new ActiveSelectValueTestOuterClass() { Container = new ActiveSelectValueTestInnerClass() { Property = 100 } };
			var sut = source.ToActiveValue(c => c.Container).ActiveSelect(c => c.Property);

			source.Container = new ActiveSelectValueTestInnerClass() { Property = 200 };

			Assert.Equal(source.Container.Property, sut.Value);
		}

		[Fact]
		public void WhenMutatingPropertyValueChanged()
		{
			var source = new ActiveSelectValueTestOuterClass() { Container = new ActiveSelectValueTestInnerClass() { Property = 100 } };
			var sut = source.ToActiveValue(c => c.Container).ActiveSelect(c => c.Property);

			source.Container.Property = 200;

			Assert.Equal(source.Container.Property, sut.Value);
		}

		[Fact]
		public void WhenMutatingPropertyChangeNotificationIsThrown()
		{
			var source = new ActiveSelectValueTestOuterClass() { Container = new ActiveSelectValueTestInnerClass() { Property = 100 } };
			var sut = source.ToActiveValue(c => c.Container).ActiveSelect(c => c.Property);

			bool called = false;

			sut.PropertyChanged += (s, e) => called = true;

			source.Container.Property = 200;

			Assert.True(called);
		}
	}

	public class ActiveSelectValueTestInnerClass : INotifyPropertyChanged
	{
		private int _property;
		public int Property
		{
			get { return _property; }
			set
			{
				if (_property == value)
					return;
				_property = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Property)));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class ActiveSelectValueTestOuterClass : INotifyPropertyChanged
	{
		private ActiveSelectValueTestInnerClass _container;
		public ActiveSelectValueTestInnerClass Container
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
