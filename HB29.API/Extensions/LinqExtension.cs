using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace hb29.API.Extensions
{
    public static class LinqExtensions
    {
        /// <summary>
        /// Includes Where clause in query if a condition is satisfied.
        /// </summary>
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> whereClause)
        {
            if (condition)
            {
                return query.Where(whereClause);
            }
            return query;
        }


        private static MethodInfo _containsMethodInfo = typeof(string).GetMethod("Contains", new Type[] { typeof(string) })!;

        public static IQueryable<T> WhereIfUsingContains<T>(this IQueryable<T> query, bool condition, Expression<Func<T, string>> prop, IList<string> items)
        {
            if (!condition)
                return query;

            if (items.Count == 0)
                return query.Where(e => false);

            var param = prop.Parameters[0];

            var predicate = items
                .Select(i => (Expression)Expression.Call(prop.Body, _containsMethodInfo, Expression.Constant(i, typeof(string))))
                .Aggregate(Expression.OrElse);

            var lambda = Expression.Lambda<Func<T, bool>>(predicate, param);

            return query.Where(lambda);
        }
    }
}
