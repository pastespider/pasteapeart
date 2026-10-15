using Microsoft.Extensions.Logging;
using System.Linq.Expressions;

namespace PasteApeart.Handler
{

    /// <summary>
    /// 
    /// </summary>
    [Obsolete]
    public static class ApeartExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="exl"></param>
        [Obsolete("Use PasteApeart.Handler.Extensions")]
        public static void PasteLogException(this ILogger logger, Exception exl)
        {
            logger.LogError(exl.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <param name="head"></param>
        /// <returns></returns>
        [Obsolete("Use PasteApeart.Handler.Extensions")]
        public static string PasteRemovePostFix(this string str, string head)
        {
            if (!String.IsNullOrEmpty(str))
            {
                if (str.StartsWith(head))
                {
                    return str.Substring(head.Length);
                }
            }
            return str;
        }

        /// <summary>
        /// Concatenates the members of a constructed <see cref="IEnumerable{T}"/> collection of type System.String, using the specified separator between each member.
        /// This is a shortcut for string.Join(...)
        /// </summary>
        /// <param name="source">A collection that contains the strings to concatenate.</param>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
        /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
        public static string PasteJoinAsString(this IEnumerable<string> source, string separator)
        {
            return string.Join(separator, source);
        }

        /// <summary>
        /// Concatenates the members of a collection, using the specified separator between each member.
        /// This is a shortcut for string.Join(...)
        /// </summary>
        /// <param name="source">A collection that contains the objects to concatenate.</param>
        /// <param name="separator">The string to use as a separator. separator is included in the returned string only if values has more than one element.</param>
        /// <typeparam name="T">The type of the members of values.</typeparam>
        /// <returns>A string that consists of the members of values delimited by the separator string. If values has no members, the method returns System.String.Empty.</returns>
        public static string PasteJoinAsString<T>(this IEnumerable<T> source, string separator)
        {
            return string.Join(separator, source);
        }

        /// <summary>
        /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
        /// </summary>
        /// <param name="source">Enumerable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="predicate">Predicate to filter the enumerable</param>
        /// <returns>Filtered or not filtered enumerable based on <paramref name="condition"/></returns>
        public static IEnumerable<T> PasteWhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, bool> predicate)
        {
            return condition
                ? source.Where(predicate)
                : source;
        }

        /// <summary>
        /// Filters a <see cref="IEnumerable{T}"/> by given predicate if given condition is true.
        /// </summary>
        /// <param name="source">Enumerable to apply filtering</param>
        /// <param name="condition">A boolean value</param>
        /// <param name="predicate">Predicate to filter the enumerable</param>
        /// <returns>Filtered or not filtered enumerable based on <paramref name="condition"/></returns>
        public static IEnumerable<T> PasteWhereIf<T>(this IEnumerable<T> source, bool condition, Func<T, int, bool> predicate)
        {
            return condition
                ? source.Where(predicate)
                : source;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <returns></returns>
        public static IQueryable<T> PastePageBy<T>(this IQueryable<T> query, int skipCount, int maxResultCount)
        {
            Check.PasteNotNull(query, "query");
            return query.Skip(skipCount).Take(maxResultCount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TQueryable"></typeparam>
        /// <param name="query"></param>
        /// <param name="skipCount"></param>
        /// <param name="maxResultCount"></param>
        /// <returns></returns>
        public static TQueryable PastePageBy<T, TQueryable>(this TQueryable query, int skipCount, int maxResultCount) where TQueryable : IQueryable<T>
        {
            Check.PasteNotNull(query, "query");
            return (TQueryable)query.Skip(skipCount).Take(maxResultCount);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<T> PasteWhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, bool>> predicate)
        {
            Check.PasteNotNull(query, "query");
            if (!condition)
            {
                return query;
            }

            return query.Where(predicate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TQueryable"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryable PasteWhereIf<T, TQueryable>(this TQueryable query, bool condition, Expression<Func<T, bool>> predicate) where TQueryable : IQueryable<T>
        {
            Check.PasteNotNull(query, "query");
            if (!condition)
            {
                return query;
            }

            return (TQueryable)query.Where(predicate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static IQueryable<T> PasteWhereIf<T>(this IQueryable<T> query, bool condition, Expression<Func<T, int, bool>> predicate)
        {
            Check.PasteNotNull(query, "query");
            if (!condition)
            {
                return query;
            }

            return query.Where(predicate);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TQueryable"></typeparam>
        /// <param name="query"></param>
        /// <param name="condition"></param>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static TQueryable PasteWhereIf<T, TQueryable>(this TQueryable query, bool condition, Expression<Func<T, int, bool>> predicate) where TQueryable : IQueryable<T>
        {
            Check.PasteNotNull(query, "query");
            if (!condition)
            {
                return query;
            }

            return (TQueryable)query.Where(predicate);
        }
    }
}
