using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace ActiveListExtensions.Tests.ValueModifiers
{
	public class ActiveCombineTests
	{
		[Fact]
		public void WhenContainerLeftChangesValueChanges()
		{
			var left = new ActiveCombineLeftTestClass() { PropertyLeft = 100 };
			var right = new ActiveCombineRightTestClass() { PropertyRight = 200 };
			var outer = new ActiveCombineTestOuterClass() { ContainerLeft = left, ContainerRight = right };

			var sut = outer.ToActiveValue(i => i.ContainerLeft).ActiveCombine(outer.ToActiveValue(i => i.ContainerRight), (l, r) => l.PropertyLeft + r.PropertyRight);

			outer.ContainerLeft = new ActiveCombineLeftTestClass() { PropertyLeft = 300 };

			Assert.Equal(sut.Value, outer.ContainerLeft.PropertyLeft + outer.ContainerRight.PropertyRight);
		}

		[Fact]
		public void WhenContainerRightChangesValueChanges()
		{
			var left = new ActiveCombineLeftTestClass() { PropertyLeft = 100 };
			var right = new ActiveCombineRightTestClass() { PropertyRight = 200 };
			var outer = new ActiveCombineTestOuterClass() { ContainerLeft = left, ContainerRight = right };

			var sut = outer.ToActiveValue(i => i.ContainerLeft).ActiveCombine(outer.ToActiveValue(i => i.ContainerRight), (l, r) => l.PropertyLeft + r.PropertyRight);

			outer.ContainerRight = new ActiveCombineRightTestClass() { PropertyRight = 300 };

			Assert.Equal(sut.Value, outer.ContainerLeft.PropertyLeft + outer.ContainerRight.PropertyRight);
		}

		[Fact]
		public void WhenPropertyLeftChangesValueChanges()
		{
			var left = new ActiveCombineLeftTestClass() { PropertyLeft = 100 };
			var right = new ActiveCombineRightTestClass() { PropertyRight = 200 };
			var outer = new ActiveCombineTestOuterClass() { ContainerLeft = left, ContainerRight = right };

			var sut = outer.ToActiveValue(i => i.ContainerLeft).ActiveCombine(outer.ToActiveValue(i => i.ContainerRight), (l, r) => l.PropertyLeft + r.PropertyRight);

			left.PropertyLeft = 300;

			Assert.Equal(sut.Value, outer.ContainerLeft.PropertyLeft + outer.ContainerRight.PropertyRight);
		}

		[Fact]
		public void WhenPropertyRightChangesValueChanges()
		{
			var left = new ActiveCombineLeftTestClass() { PropertyLeft = 100 };
			var right = new ActiveCombineRightTestClass() { PropertyRight = 200 };
			var outer = new ActiveCombineTestOuterClass() { ContainerLeft = left, ContainerRight = right };

			var sut = outer.ToActiveValue(i => i.ContainerLeft).ActiveCombine(outer.ToActiveValue(i => i.ContainerRight), (l, r) => l.PropertyLeft + r.PropertyRight);

			right.PropertyRight = 300;

			Assert.Equal(sut.Value, outer.ContainerLeft.PropertyLeft + outer.ContainerRight.PropertyRight);
		}

		[Fact]
		public void WhenPropertyLeftChangesChangeNotificationIsThrown()
		{
			var left = new ActiveCombineLeftTestClass() { PropertyLeft = 100 };
			var right = new ActiveCombineRightTestClass() { PropertyRight = 200 };
			var outer = new ActiveCombineTestOuterClass() { ContainerLeft = left, ContainerRight = right };

			var sut = outer.ToActiveValue(i => i.ContainerLeft).ActiveCombine(outer.ToActiveValue(i => i.ContainerRight), (l, r) => l.PropertyLeft + r.PropertyRight);

			bool called = false;

			sut.PropertyChanged += (s, e) => called = true;

			left.PropertyLeft = 300;

			Assert.True(called);
		}

		[Fact]
		public void WhenPropertyRightChangesChangeNotificationIsThrown()
		{
			var left = new ActiveCombineLeftTestClass() { PropertyLeft = 100 };
			var right = new ActiveCombineRightTestClass() { PropertyRight = 200 };
			var outer = new ActiveCombineTestOuterClass() { ContainerLeft = left, ContainerRight = right };

			var sut = outer.ToActiveValue(i => i.ContainerLeft).ActiveCombine(outer.ToActiveValue(i => i.ContainerRight), (l, r) => l.PropertyLeft + r.PropertyRight);

			bool called = false;

			sut.PropertyChanged += (s, e) => called = true;

			right.PropertyRight = 300;

			Assert.True(called);
		}
	}

	public class ActiveCombineTestOuterClass : INotifyPropertyChanged
	{
		private ActiveCombineLeftTestClass _containerLeft;
		public ActiveCombineLeftTestClass ContainerLeft
		{
			get { return _containerLeft; }
			set
			{
				if (_containerLeft == value)
					return;
				_containerLeft = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContainerLeft)));
			}
		}

		private ActiveCombineRightTestClass _containerRight;
		public ActiveCombineRightTestClass ContainerRight
		{
			get { return _containerRight; }
			set
			{
				if (_containerRight == value)
					return;
				_containerRight = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ContainerRight)));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}


	public class ActiveCombineLeftTestClass : INotifyPropertyChanged
	{
		private int _property;
		public int PropertyLeft
		{
			get { return _property; }
			set
			{
				if (_property == value)
					return;
				_property = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertyLeft)));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}

	public class ActiveCombineRightTestClass : INotifyPropertyChanged
	{
		private int _property;
		public int PropertyRight
		{
			get { return _property; }
			set
			{
				if (_property == value)
					return;
				_property = value;
				PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(PropertyRight)));
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
	}
}
