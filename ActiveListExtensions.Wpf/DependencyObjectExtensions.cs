using ActiveListExtensions.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ActiveListExtensions
{
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static class DependencyObjectExtensions
	{
		public static IActiveValue<T> ToActiveValue<T>(this DependencyObject source, DependencyProperty property) => new DependencyPropertyWatcher<T>(source, property);

		public static IActiveList<T> ToActiveList<T>(this DependencyObject source, DependencyProperty property) => new DependencyPropertyWatcher<IEnumerable<T>>(source, property).ToActiveList();
	}
}
