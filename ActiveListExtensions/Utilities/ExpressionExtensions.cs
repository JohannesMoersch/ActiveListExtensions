using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ActiveListExtensions.Utilities
{
	internal static class ExpressionExtensions
	{
		private class PropertyVisitor : ExpressionVisitor
		{
			private ThreadLocal<List<string>> _properties = new ThreadLocal<List<string>>(() => new List<string>());

			protected override Expression VisitMember(MemberExpression node)
			{
				_properties.Value.Add(node.Member.Name);
				return base.VisitMember(node);
			}

			public IEnumerable<string> GetReferencedProperties(Expression expression)
			{
				_properties.Value.Clear();
				Visit(expression);
				return _properties.Value;
			}
		}

		private static ConcurrentDictionary<Expression, string[]> _propertyCache = new ConcurrentDictionary<Expression, string[]>();

		public static IEnumerable<string> GetReferencedProperties<T, U>(this Expression<Func<T, U>> expression) => _propertyCache.GetOrAdd(expression, e => new PropertyVisitor().GetReferencedProperties(expression).ToArray());
	}
}
