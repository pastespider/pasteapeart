using System.Linq.Expressions;
using System.Reflection;

namespace PasteApeart.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// 取代System.Linq.XXX的OrderBy慢的问题
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="ordering"></param>
        /// <returns></returns>
        public static IOrderedQueryable<T> PasteOrderBy<T>(this IQueryable<T> query, string ordering)
        {
            if (string.IsNullOrWhiteSpace(ordering))
                return query.OrderBy(x => 1);

            // 分割多个排序字段
            var orderClauses = ordering.Split(',', StringSplitOptions.RemoveEmptyEntries);
            IOrderedQueryable<T> orderedQuery = null;
            var eType = typeof(T);
            foreach (var clause in orderClauses)
            {
                var parts = clause.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                var fieldName = parts[0];
                var descending = false;
                if (parts.Length > 1)
                {
                    descending = parts.Length > 1 &&
                                  (parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase) ||
                                   parts[1].Equals("descending", StringComparison.OrdinalIgnoreCase));
                }
                //// 构建表达式
                //var parameter = Expression.Parameter(typeof(T), "x");
                //var property = Expression.Property(parameter, fieldName);
                //var lambda = Expression.Lambda(property, parameter);
                // 使用 GetProperty 而不是 Expression.Property
                var propertyInfo = eType.GetProperty(fieldName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);

                if (propertyInfo == null)
                {
                    // 如果当前类型没有，查找基类的属性
                    propertyInfo = eType.GetProperty(fieldName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                }

                if (propertyInfo == null)
                {
                    throw new ArgumentException($"类型 '{eType.Name}' 中不存在属性 '{fieldName}'");
                }

                // 构建表达式
                var parameter = Expression.Parameter(eType, "x");
                var property = Expression.Property(parameter, propertyInfo);
                var lambda = Expression.Lambda(property, parameter);

                if (orderedQuery == null)
                {
                    // 第一个排序字段
                    var methodName = descending ? "OrderByDescending" : "OrderBy";
                    var methodCall = Expression.Call(
                        typeof(Queryable), methodName,
                        new Type[] { typeof(T), property.Type },
                        query.Expression, Expression.Quote(lambda));

                    orderedQuery = (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(methodCall);
                }
                else
                {
                    // 后续排序字段
                    var methodName = descending ? "ThenByDescending" : "ThenBy";
                    var methodCall = Expression.Call(
                        typeof(Queryable), methodName,
                        new Type[] { typeof(T), property.Type },
                        orderedQuery.Expression, Expression.Quote(lambda));

                    orderedQuery = (IOrderedQueryable<T>)orderedQuery.Provider.CreateQuery<T>(methodCall);
                }
            }

            return orderedQuery ?? (IOrderedQueryable<T>)query;
        }

        /// <summary>
        /// 测试可用 DefaultAppService的默认
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ordering"></param>
        /// <returns></returns>
        public static IOrderedQueryable PasteOrder(this IQueryable query, string ordering)
        {
            //if (string.IsNullOrWhiteSpace(ordering))
            //return query.OrderBy(x => 1);
            if (String.IsNullOrEmpty(ordering))
            {
                return (IOrderedQueryable)query;
            }

            // 分割多个排序字段
            var orderClauses = ordering.Split(',', StringSplitOptions.RemoveEmptyEntries);
            IOrderedQueryable orderedQuery = null;

            var eType = query.ElementType;

            foreach (var clause in orderClauses)
            {
                var parts = clause.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                var fieldName = parts[0];
                var descending = false;
                if (parts.Length > 1)
                {
                    descending = parts.Length > 1 &&
                                   (parts[1].Equals("desc", StringComparison.OrdinalIgnoreCase) ||
                                    parts[1].Equals("descending", StringComparison.OrdinalIgnoreCase));
                }
                //// 构建表达式
                //var parameter = Expression.Parameter(eType, "x");
                //var property = Expression.Property(parameter, fieldName);
                //var lambda = Expression.Lambda(property, parameter);

                // 使用 GetProperty 而不是 Expression.Property
                var propertyInfo = eType.GetProperty(fieldName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);

                if (propertyInfo == null)
                {
                    // 如果当前类型没有，查找基类的属性
                    propertyInfo = eType.GetProperty(fieldName,
                        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                }

                if (propertyInfo == null)
                {
                    throw new ArgumentException($"类型 '{eType.Name}' 中不存在属性 '{fieldName}'");
                }

                // 构建表达式
                var parameter = Expression.Parameter(eType, "x");
                var property = Expression.Property(parameter, propertyInfo);
                var lambda = Expression.Lambda(property, parameter);

                if (orderedQuery == null)
                {
                    // 第一个排序字段
                    var methodName = descending ? "OrderByDescending" : "OrderBy";
                    var methodCall = Expression.Call(
                        typeof(Queryable), methodName,
                        new Type[] { eType, property.Type },
                        query.Expression, Expression.Quote(lambda));

                    orderedQuery = (IOrderedQueryable)query.Provider.CreateQuery(methodCall);
                }
                else
                {
                    // 后续排序字段
                    var methodName = descending ? "ThenByDescending" : "ThenBy";
                    var methodCall = Expression.Call(
                        typeof(Queryable), methodName,
                        new Type[] { eType, property.Type },
                        orderedQuery.Expression, Expression.Quote(lambda));

                    orderedQuery = (IOrderedQueryable)orderedQuery.Provider.CreateQuery(methodCall);
                }
            }

            return orderedQuery ?? (IOrderedQueryable)query;
        }
    }
}
