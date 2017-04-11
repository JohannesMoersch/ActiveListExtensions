using ActiveListExtensions.ValueModifiers.Bases;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveListExtensions.ValueModifiers
{
	internal class ActiveContains<TSource> : ActiveListPredicateBase<TSource, object>
	{
		private IActiveValue<TSource> _value;

		public ActiveContains(IActiveList<TSource> source, IActiveValue<TSource> value)
			: base(source, null, item => Equals(item, value.Value))
		{
			_value = value;

			PropertyChangedEventManager.AddHandler(_value, SourceChanged, nameof(IActiveValue<TSource>.Value));

			Initialize();
		}

		private void SourceChanged(object key, PropertyChangedEventArgs args) => OnReset(SourceList);

		protected override void OnDisposed()
		{
			PropertyChangedEventManager.RemoveHandler(_value, SourceChanged, nameof(IActiveValue<TSource>.Value));
			base.OnDisposed();
		}

		protected override bool GetValue(bool predicateMet) => predicateMet;
	}
}