using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Xunit;

namespace ActiveListExtensions.Tests.Wpf
{
	public class DependencyPropertyWatcherTests
	{
		[Fact]
		public void ToActiveValueTest()
		{
			var source = new TestDependencyObject();

			var value = source.ToActiveValue<int>(TestDependencyObject.TestPropertyProperty).ActiveSelect(o => o);

			Assert.Equal(0, value.Value);

			source.TestProperty = 10;

			Assert.Equal(10, value.Value);
		}

		[Fact]
		public void ToActiveListTest()
		{
			var source = new TestDependencyObject();

			var collection = source.ToActiveList<int>(TestDependencyObject.TestCollectionProperty).ActiveSelect(o => o);

			Assert.False(collection.Any());

			source.TestCollection = new[] { 1, 2, 3 };

			Assert.True(collection.SequenceEqual(new[] { 1, 2, 3 }));
		}
	}

	public class TestDependencyObject : DependencyObject
	{
		public int TestProperty
		{
			get { return (int)GetValue(TestPropertyProperty); }
			set { SetValue(TestPropertyProperty, value); }
		}
		public static readonly DependencyProperty TestPropertyProperty = DependencyProperty.Register("TestProperty", typeof(int), typeof(TestDependencyObject), new PropertyMetadata(0));

		public IEnumerable<int> TestCollection
		{
			get { return (IEnumerable<int>)GetValue(TestCollectionProperty); }
			set { SetValue(TestCollectionProperty, value); }
		}
		public static readonly DependencyProperty TestCollectionProperty = DependencyProperty.Register("TestCollection", typeof(IEnumerable<int>), typeof(TestDependencyObject), new PropertyMetadata(null));
	}
}
