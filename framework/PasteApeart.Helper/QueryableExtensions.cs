using System.Linq.Expressions;
using System.Reflection;

namespace PasteApeart.Helper.Extensions
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
                //var propertyInfo = eType.GetProperty(fieldName,
                //    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);

                //if (propertyInfo == null)
                //{
                //    // 如果当前类型没有，查找基类的属性
                //    propertyInfo = eType.GetProperty(fieldName,
                //        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                //}
                var propertyInfo = ReadTypeProperties(eType).FirstOrDefault(x => string.Equals(x.Name, fieldName, StringComparison.OrdinalIgnoreCase));
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
                //var propertyInfo = eType.GetProperty(fieldName,
                //    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.IgnoreCase);
                var propertyInfo = ReadTypeProperties(eType).FirstOrDefault(x => string.Equals(x.Name, fieldName, StringComparison.OrdinalIgnoreCase));
                //if (propertyInfo == null)
                //{
                //    // 如果当前类型没有，查找基类的属性
                //    propertyInfo = eType.GetProperty(fieldName,
                //        BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                //}

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchType"></param>
        /// <returns></returns>
        private static PropertyInfo[] ReadTypeProperties(Type searchType)
        {

            var typeName = searchType.FullName;
            // 使用GetOrCreate替代TryGetValue+Set
            //return t.GetProperties(); // 耗时的反射操作
            var allProperties = searchType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 如果有属性被隐藏（new），才需要复杂处理
            var hasDuplicates = allProperties.Length != allProperties.Select(p => p.Name).Distinct().Count();
            if (!hasDuplicates)
            {
                // 没有重复属性名，直接返回
                return allProperties;
            }
            // 有重复属性名，进行分组处理
            var propertyGroups = allProperties.GroupBy(p => p.Name);
            var result = new PropertyInfo[propertyGroups.Count()];

            int index = 0;
            foreach (var group in propertyGroups)
            {
                // 手动查找，避免 LINQ 的开销
                PropertyInfo selected = null;
                foreach (var prop in group)
                {
                    if (prop.DeclaringType == searchType)
                    {
                        selected = prop;
                        break;  // 找到当前类声明的，立即退出
                    }
                    selected ??= prop;  // 否则保留第一个
                }
                result[index++] = selected!;
            }

            return result;


        }
    }
}
