

namespace PasteApeart.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class AbpPasteExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetFullNameWithAssemblyName(this Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TTarget"></typeparam>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool IsAssignableTo<TTarget>(this Type type)
        {
            AbpPasteCheck.NotNull(type, "type");
            return IsAssignableTo(type, typeof(TTarget));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static bool IsAssignableTo(this Type type, Type targetType)
        {
            AbpPasteCheck.NotNull(type, "type");
            AbpPasteCheck.NotNull(targetType, "targetType");
            return targetType.IsAssignableFrom(type);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="includeObject"></param>
        /// <returns></returns>
        public static Type[] GetBaseClasses(this Type type, bool includeObject = true)
        {
            AbpPasteCheck.NotNull(type, "type");
            List<Type> list = new List<Type>();
            AddTypeAndBaseTypesRecursively(list, type.BaseType, includeObject);
            return list.ToArray();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <param name="stoppingType"></param>
        /// <param name="includeObject"></param>
        /// <returns></returns>
        public static Type[] GetBaseClasses(this Type type, Type stoppingType, bool includeObject = true)
        {
            AbpPasteCheck.NotNull(type, "type");
            List<Type> list = new List<Type>();
            AddTypeAndBaseTypesRecursively(list, type.BaseType, includeObject, stoppingType);
            return list.ToArray();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="types"></param>
        /// <param name="type"></param>
        /// <param name="includeObject"></param>
        /// <param name="stoppingType"></param>
        private static void AddTypeAndBaseTypesRecursively(List<Type> types, Type type, bool includeObject, Type stoppingType = null)
        {
            if (!(type == null) && !(type == stoppingType) && (includeObject || !(type == typeof(object))))
            {
                AddTypeAndBaseTypesRecursively(types, type.BaseType, includeObject, stoppingType);
                types.Add(type);
            }
        }


        /// <summary>
        /// 是否有正确的值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool PasteHasValue(this int id)
        {
            return id > 0;
        }

        /// <summary>
        /// 是否有正确的值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool PasteHasValue(this long id)
        {
            return id > 0;
        }

        /// <summary>
        /// 是否有正确的值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool PasteHasValue(this Guid id)
        {
            return id != Guid.Empty;
        }

        /// <summary>
        /// 是否有正确的值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static bool PasteHasValue(this string id)
        {
            return !String.IsNullOrWhiteSpace(id);
        }

    }
}
