using System.Collections;
using System.Reflection;

namespace PasteApeart.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public class HalfHideHelper
    {

        #region 脱敏处理 跟级的Class一定要标记PasteHalfHide特性

        /// <summary>
        /// 对字段进行脱敏处理，注意[PasteHalfHide]如果没在Class标记，则使用默认值，就近原则！
        /// </summary>
        /// <param name="data"></param>
        /// <param name="targetType"></param>
        /// <returns></returns>
        public static object PasteMaskData(object data, Type targetType)
        {
            if (data == null || targetType == null)
            {
                return data;
            }

            var classAttribute = targetType.GetCustomAttribute<PasteHalfHideAttribute>();
            if (classAttribute == null)
            {
                //跟级一定要有class的特性，这样可以避免无用扫描
                return data;
            }

            var properties = ReadTypeProperties(targetType);
            bool createNew = classAttribute.CreateNewObj;   // 唯一决策点，向下传

            if (data is IEnumerable list && data is not string)
            {
                if (createNew)
                {
                    var resultList = new List<object>();
                    foreach (var item in list)
                    {
                        if (item == null || !targetType.IsInstanceOfType(item))
                        {
                            resultList.Add(item);
                            continue;
                        }
                        resultList.Add(MaskNewSingleObject(item, targetType, properties, createNew, classAttribute));
                    }
                    return resultList;
                }
                else
                {
                    foreach (var item in list)
                    {
                        if (item == null || !targetType.IsInstanceOfType(item))
                            continue;
                        MaskSingleObject(item, targetType, properties, createNew, classAttribute);
                    }
                    return data;
                }
            }

            if (createNew)
                return MaskNewSingleObject(data, targetType, properties, createNew, classAttribute);

            MaskSingleObject(data, targetType, properties, createNew, classAttribute);
            return data;
        }

        /// <summary>
        /// 原地修改模式（createNew 恒 false）
        /// 字段特性就近优先；字段没标但类标了 -> 用类级；都没标 -> 跳过
        /// </summary>
        private static void MaskSingleObject(object obj, Type type, PropertyInfo[] properties, bool createNew, PasteHalfHideAttribute inputAttribute)
        {
            if (obj == null) return;

            var classAttr = type.GetCustomAttribute<PasteHalfHideAttribute>();
            if (classAttr == null)
            {
                classAttr = inputAttribute;
            }

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite) continue;

                var propAttr = property.GetCustomAttribute<PasteHalfHideAttribute>();
                var value = property.GetValue(obj);

                bool shouldMask = propAttr != null || classAttr != null;
                if (!shouldMask) continue;

                // string
                if (property.PropertyType == typeof(string))
                {
                    var s = value as string;
                    if (string.IsNullOrEmpty(s)) continue;
                    var attr = propAttr ?? classAttr;   // 就近优先
                    property.SetValue(obj, Desensitize(s, attr));
                    continue;
                }

                // 值类型
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(DateTime))
                    continue;

                // 集合
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    if (value is not IEnumerable srcList) continue;
                    var elementType = GetEnumerableElementType(property.PropertyType);
                    if (elementType == null) continue;
                    var elementProps = ReadTypeProperties(elementType);
                    foreach (var item in srcList)
                    {
                        if (item == null || !elementType.IsInstanceOfType(item)) continue;
                        MaskSingleObject(item, elementType, elementProps, createNew, classAttr);
                    }
                    continue;
                }

                // 复杂对象
                if (value == null) continue;
                var childType = property.PropertyType;
                var childProps = ReadTypeProperties(childType);
                MaskSingleObject(value, childType, childProps, createNew, classAttr);
            }
        }

        /// <summary>
        /// 副本模式（createNew 恒 true）
        /// 不吞值：字段没标 -> 原样复制原值；字段标了 -> 处理
        /// 类没标不再提前 return 空对象
        /// </summary>
        private static object MaskNewSingleObject(object obj, Type type, PropertyInfo[] properties, bool createNew, PasteHalfHideAttribute inputAttribute)
        {
            if (obj == null) return null;

            var classAttr = type.GetCustomAttribute<PasteHalfHideAttribute>();
            if (classAttr == null)
            {
                classAttr = inputAttribute;
            }
            var result = Activator.CreateInstance(type);

            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite) continue;

                var propAttr = property.GetCustomAttribute<PasteHalfHideAttribute>();
                var value = property.GetValue(obj);

                bool shouldMask = propAttr != null || classAttr != null;

                // string
                if (property.PropertyType == typeof(string))
                {
                    var s = value as string;
                    if (!shouldMask || string.IsNullOrEmpty(s))
                    {
                        property.SetValue(result, s);   // 没标/空值：原样复制，不吞
                        continue;
                    }
                    var attr = propAttr ?? classAttr;
                    property.SetValue(result, Desensitize(s, attr));
                    continue;
                }

                // 值类型
                if (property.PropertyType.IsValueType || property.PropertyType == typeof(DateTime))
                {
                    property.SetValue(result, value);
                    continue;
                }

                // 集合
                if (typeof(IEnumerable).IsAssignableFrom(property.PropertyType))
                {
                    if (value is not IEnumerable srcList)
                    {
                        property.SetValue(result, value);
                        continue;
                    }
                    var elementType = GetEnumerableElementType(property.PropertyType);
                    if (elementType == null)
                    {
                        property.SetValue(result, value);
                        continue;
                    }
                    if (!shouldMask)
                    {
                        // 没标：原样复制集合引用
                        property.SetValue(result, value);
                        continue;
                    }
                    var elementProps = ReadTypeProperties(elementType);
                    var newList = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));
                    foreach (var item in srcList)
                    {
                        if (item == null || !elementType.IsInstanceOfType(item))
                        {
                            newList.Add(item);
                            continue;
                        }
                        newList.Add(MaskNewSingleObject(item, elementType, elementProps, createNew, classAttr));
                    }
                    property.SetValue(result, newList);
                    continue;
                }

                // 复杂对象
                if (!shouldMask || value == null)
                {
                    property.SetValue(result, value);   // 没标/空值：原样复制，不吞
                    continue;
                }

                var childType = property.PropertyType;
                var childProps = ReadTypeProperties(childType);
                property.SetValue(result, MaskNewSingleObject(value, childType, childProps, createNew, classAttr));
            }

            return result;
        }

        private static Type GetEnumerableElementType(Type t)
        {
            if (t.IsArray) return t.GetElementType();
            if (t.IsGenericType && t.GetGenericArguments().Length == 1)
                return t.GetGenericArguments()[0];
            var iface = t.GetInterfaces().FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return iface?.GetGenericArguments()[0];
        }

        private static string Desensitize(string value, PasteHalfHideAttribute attr)
        {
            if (string.IsNullOrEmpty(value)) return value;
            return DesensitizeCustom(value, attr.StartLength, attr.EndLength, attr.MaskChar);
        }

        private static string DesensitizeCustom(string value, int startLength, int endLength, char maskChar)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (startLength < 0) startLength = 0;
            if (endLength < 0) endLength = 0;
            if (value.Length <= startLength + endLength) return value;

            return value.Substring(0, startLength) +
                   new string(maskChar, value.Length - startLength - endLength) +
                   value.Substring(value.Length - endLength);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchType"></param>
        /// <returns></returns>
        private static PropertyInfo[] ReadTypeProperties(Type searchType)
        {

            var typeName = searchType.FullName;

            var allProperties = searchType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // 辅助方法：计算类型的继承深度
            int GetInheritanceDepth(Type type)
            {
                int depth = 0;
                while (type.BaseType != null && type.BaseType != typeof(object))
                {
                    depth++;
                    type = type.BaseType;
                }
                return depth;
            }

            // 按继承深度从远到近排序
            var sortedProperties = allProperties
                .OrderBy(p => GetInheritanceDepth(p.DeclaringType))
                .ToArray();

            var hasDuplicates = sortedProperties.Length != sortedProperties.Select(p => p.Name).Distinct().Count();
            if (!hasDuplicates)
            {
                return sortedProperties;
            }

            // 去重：保留最近的（后面的覆盖前面的）
            var propertyDict = new Dictionary<string, PropertyInfo>();
            foreach (var prop in sortedProperties)
            {
                propertyDict[prop.Name] = prop;
            }

            return propertyDict.Values
                .OrderBy(p => GetInheritanceDepth(p.DeclaringType))
                .ToArray();

        }

    }
}
