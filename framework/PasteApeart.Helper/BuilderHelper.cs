using Microsoft.Extensions.Caching.Memory;
using System.Collections;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace PasteApeart.Helper
{
    /// <summary>
    /// 
    /// </summary>
    public static class BuilderHelper
    {
        private const string SplitChar = "_*_";

        /// <summary>
        /// 读取枚举的信息组合成select信息
        /// 从XML中读取枚举的备注信息
        /// </summary>
        /// <param name="_classType"></param>
        /// <param name="ListModel"></param>
        /// <returns></returns>
        public static List<VoloEnumProperty> ReadEnumProperty(Type _classType, bool ListModel = false)
        {
            var _classModel = new VoloModelInfo();

            var _current_is_enum = false;
            if (_classType.BaseType != null)
            {
                var _baseType = _classType.BaseType;
                if (_baseType.FullName == "System.Enum")
                {
                    _current_is_enum = true;
                }
            }
            var _bases = _classType.GetBaseClasses();
            if (_bases != null)
            {
                foreach (var _base in _bases)
                {
                    if (_base.FullName != "System.Object")
                    {
                        if (_base.GenericTypeArguments != null)
                        {
                            foreach (var _arg in _base.GenericTypeArguments)
                            {
                                switch (_arg.Name)
                                {
                                    case "Int64":
                                        {
                                            _classModel.KeyType = "long";
                                        }
                                        break;
                                    case "String":
                                        {
                                            _classModel.KeyType = "string";
                                        }
                                        break;
                                    case "Guid":
                                        {
                                            _classModel.KeyType = "Guid";
                                        }
                                        break;
                                }
                            }
                        }
                    }
                    //Console.WriteLine(_base.FullName);
                }
            }

            #region 读取外围的文档等
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            var _path = Path.GetDirectoryName(assemblyPath);

            LoadXmlFiles(_path);
            LoadSummaryFromXelement(_classType.FullName, _classModel);

            #endregion

            if (!String.IsNullOrEmpty(_classModel.Summary))
            {
                _classModel.Title = _classModel.Summary.Split(' ')[0].Replace(SplitChar, " ");
                if (_classModel.Summary.Contains(" "))
                {
                    _classModel.Desc = _classModel.Summary.Split(' ')[1].Replace(SplitChar, " ");
                }
            }

            if (_current_is_enum)
            {
                //获取枚举的主类型，一般为int
                var _fields = _classType.GetFields();
                if (_fields != null && _fields.Length > 0)
                {
                    // value__
                    foreach (var _field in _fields)
                    {
                        if (_field.Name == "value__")
                        {
                            if (_field.FieldType != null)
                            {
                                switch (_field.FieldType.Name)
                                {
                                    case "Int32":
                                        {
                                            _classModel.KeyType = "int";
                                        }
                                        break;
                                    case "UInt32":
                                        {
                                            _classModel.KeyType = "uint";
                                        }
                                        break;
                                    case "Short":
                                        {
                                            _classModel.KeyType = "short";
                                        }
                                        break;
                                    case "UShort":
                                        {
                                            _classModel.KeyType = "ushort";
                                        }
                                        break;
                                    case "Int64":
                                        {
                                            _classModel.KeyType = "long";
                                        }
                                        break;
                                    case "UInt64":
                                        {
                                            _classModel.KeyType = "ulong";
                                        }
                                        break;
                                    case "String":
                                        {
                                            _classModel.KeyType = "string";
                                        }
                                        break;
                                    default:
                                        {
                                            _classModel.KeyType = "int";
                                        }
                                        break;
                                }
                            }
                            break;
                        }
                    }
                }

                var _names = _classType.GetEnumNames();
                var _values = _classType.GetEnumValues();

                var _zindex = 0;
                var enumPropertys = new List<VoloEnumProperty>();
                foreach (var item in _names)
                {
                    VoloEnumProperty _cpro = new VoloEnumProperty();
                    if (!String.IsNullOrEmpty(item))
                    {
                        _cpro.name = item.Replace(SplitChar, " ");
                    }
                    _cpro.key_type = _classModel.KeyType;
                    var typeElement = LoadXelement($"{_classType.FullName}.{item}", "F");
                    if (typeElement != null)
                    {
                        var _summary = typeElement.Element("summary")?.Value;
                        if (!String.IsNullOrEmpty(_summary))
                        {
                            _summary = _summary.Replace("\r\n", "").Trim();
                            if (ListModel)
                            {
                                _cpro.name = _summary.Split(" ")[0].Replace(SplitChar, " ");
                            }
                            else
                            {
                                _cpro.name = _summary.Replace(SplitChar, " ");
                            }
                        }
                    }
                    _cpro.zindex = _zindex;
                    _zindex++;
                    enumPropertys.Add(_cpro);
                }
                for (var k = 0; k < _values.Length; k++)
                {
                    var find = enumPropertys.Where(x => x.zindex == k).FirstOrDefault();
                    if (find != null && find != default)
                    {
                        find.value = _values.GetValue(k);
                    }
                }
                if (enumPropertys != null && enumPropertys.Count > 0)
                {
                    return enumPropertys;
                }
            }
            return null;
        }

        /// <summary>
        /// 直接从特性获取枚举的信息 非XML模式，使用特性模式
        /// </summary>
        /// <param name="mtype"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public static List<VoloEnumProperty> GetAllEnumItemInfo(Type mtype, bool list = false)
        {
            var result = new List<VoloEnumProperty>();

            foreach (var value in Enum.GetValues(mtype))
            {
                var enumValue = (Enum)value;
                var one = new VoloEnumProperty
                {
                    field = Enum.GetName(mtype, value),
                    value = Convert.ToInt32(value),
                    name = enumValue.GetDescription() ?? Enum.GetName(mtype, value)
                };

                if (list)
                {
                    if (!String.IsNullOrEmpty(one.name))
                    {
                        if (one.name.Contains(" "))
                        {
                            one.name = one.name.Split(" ")[0].Replace(SplitChar, " ");
                        }
                    }
                }

                if (!String.IsNullOrEmpty(one.name))
                {
                    if (one.name.Contains(SplitChar))
                    {
                        one.name = one.name.Replace(SplitChar, " ");
                    }
                }

                result.Add(one);
            }

            if (result.Count > 0)
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// 获取单个枚举项的描述 DescriptionAttribute
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetDescription(this Enum value)
        {
            if (value == null) return string.Empty;

            var field = value.GetType().GetField(value.ToString());
            if (field == null) return value.ToString();

            // 获取DescriptionAttribute属性
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }

        /// <summary>
        /// 读取对应结构和默认值
        /// </summary>
        /// <param name="_classType"></param>
        /// <param name="_obj"></param>
        /// <param name="ListModel"></param>
        /// <param name="xmlModel"></param>
        /// <returns></returns>
        private static VoloModelInfo ReadPropertyAndDefaultValue(Type _classType, object _obj, bool ListModel = false, bool xmlModel = true)
        {
            var _classModel = new VoloModelInfo();
            var _bases = _classType.GetBaseClasses();
            if (_bases != null)
            {
                foreach (var _base in _bases)
                {
                    if (_base.FullName != "System.Object")
                    {
                        if (_base.GenericTypeArguments != null)
                        {
                            foreach (var _arg in _base.GenericTypeArguments)
                            {
                                switch (_arg.Name)
                                {
                                    case "Int64":
                                        {
                                            _classModel.KeyType = "long";
                                        }
                                        break;
                                    case "String":
                                        {
                                            _classModel.KeyType = "string";
                                        }
                                        break;
                                    case "Guid":
                                        {
                                            _classModel.KeyType = "Guid";
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            if (xmlModel)
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                var _path = Path.GetDirectoryName(assemblyPath);
                LoadXmlFiles(_path);
                LoadSummaryFromXelement(_classType.FullName, _classModel, "T");
            }
            var _pro_list = new List<VoloModelProperty>();
            foreach (var _property in ReadTypeProperties(_classType))
            {
                if (!_property.PropertyType.IsPublic)
                {
                    continue;
                }
                var _cpro = new VoloModelProperty();
                if (xmlModel)
                {
                    ReadXmlSummary(_classType, _property, _cpro, 0);
                }
                if (_property.DeclaringType != null)
                {
                    var _declarType = _property.DeclaringType;
                }
                _cpro.Name = _property.Name.FirstLetterToLower();
                _cpro.DataType = _property.PropertyType.Name;
                //_cpro.Value = _property.GetValue(_new)?.ToString();
                var attributes = _property.GetCustomAttributes<Attribute>();
                var _attributes = HandlerAttribute2(attributes, _cpro);

                if (_attributes.Count > 0)
                {
                    _cpro.Attributes = _attributes;
                }
                if (!String.IsNullOrEmpty(_cpro.Summary))
                {
                    _cpro.Title = _cpro.Summary.Split(' ')[0].Replace(SplitChar, " ");
                    if (_cpro.Summary.Contains(" "))
                    {
                        _cpro.Placeholder = _cpro.Summary.Split(' ')[1].Replace(SplitChar, " ");
                    }
                }
                if (_cpro.DataType == "DateTime")
                {
                    _cpro.Required = true;
                }
                if (_cpro.DataType.Contains("Nullable"))
                {
                    SetNullable(_cpro, _property);
                }

                //因为上面可能会把新的数据类型穿透出来，所以在这里进行默认值的校验，特别是时间格式
                var _value = _property.GetValue(_obj);
                if (_value != null)
                {
                    if (_cpro.DataType == "DateTime")
                    {
                        if (DateTime.TryParse(_value.ToString(), out var _time))
                        {
                            if (_time.Year > 1000)
                            {
                                _cpro.Value = _time.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                    }
                    else
                    {
                        _cpro.Value = _value;
                    }
                }

                //判断是不是枚举类型
                if (_property.PropertyType != null)
                {
                    if (!_property.PropertyType.FullName.StartsWith("System"))
                    {
                        var _proType = _property.PropertyType;
                        if (_proType.BaseType != null)
                        {
                            if (_proType.BaseType.FullName == "System.Enum" || _proType.BaseType.FullName == "System.Array")
                            {
                                List<VoloEnumProperty> _reads = null;
                                if (_proType.BaseType.FullName == "System.Enum")
                                {
                                    if (xmlModel)
                                    {
                                        _reads = ReadEnumProperty(_property.PropertyType, ListModel);
                                    }
                                    else
                                    {
                                        _reads = GetAllEnumItemInfo(_property.PropertyType, ListModel);
                                    }
                                }
                                else
                                {
                                    var _next_type = _proType.GetElementType();
                                    if (_next_type.BaseType != null)
                                    {
                                        if (_next_type.BaseType.FullName == "System.Enum")
                                        {
                                            if (xmlModel)
                                            {
                                                _reads = ReadEnumProperty(_next_type, ListModel);
                                            }
                                            else
                                            {
                                                _reads = GetAllEnumItemInfo(_next_type, ListModel);
                                            }
                                        }
                                    }
                                }
                                if (_reads != null)
                                {
                                    HandlerEnumDatas(_cpro, _reads);

                                }
                            }
                            else
                            {
                                //object对象??? 如果是dynamic的模式，则继续读取... .. 这里要使用递归了
                            }
                        }
                    }
                }

                if (_cpro.Name == "id")
                {
                    if (String.IsNullOrEmpty(_cpro.Title))
                    {
                        _cpro.Title = "ID";
                    }
                    _pro_list.Insert(0, _cpro);
                }
                else
                {
                    _pro_list.Add(_cpro);
                }
            }

            //大于0 的追加到后面 小于0的追加到前面???为0的不动
            if (_pro_list != null && _pro_list.Count > 0 && _pro_list.Where(x => x.Sort != 0).Any())
            {
                var headers = _pro_list.Where(x => x.Sort < 0).OrderBy(x => x.Sort).ToList();
                var foots = _pro_list.Where(x => x.Sort > 0).OrderBy(x => x.Sort).ToList();
                var middle = _pro_list.Where(x => x.Sort == 0).ToList();
                _pro_list.Clear();
                if (headers.Count > 0)
                {
                    _pro_list.AddRange(headers);
                }
                if (middle.Count > 0)
                {
                    _pro_list.AddRange(middle);
                }
                if (foots.Count > 0)
                {
                    _pro_list.AddRange(foots);
                }
            }
            _classModel.Properties = _pro_list;//可能会打乱原来的排序，造成所有的都要设定sort,所以放业务中，业务按照需求自己排序

            if (!String.IsNullOrEmpty(_classModel.Summary))
            {
                _classModel.Title = _classModel.Summary.Split(' ')[0].Replace(SplitChar, " ");
                if (_classModel.Summary.Contains(" "))
                {
                    _classModel.Desc = _classModel.Summary.Split(' ')[1].Replace(SplitChar, " ");
                }
            }

            var class_attributes = _classType.GetCustomAttributes<Attribute>();
            var _class_attribute = HandlerAttribute2(class_attributes, null);
            if (_class_attribute != null && _class_attribute.Count > 0)
            {
                _classModel.Attributes = _class_attribute;
            }
            return _classModel;
        }

        /// <summary>
        /// 排序
        /// </summary>
        /// <param name="proList"></param>
        public static void Sort(List<VoloModelProperty> proList)
        {
            if (proList != null && proList.Count > 0 && proList.Where(x => x.Sort != 0).Any())
            {
                var headList = proList.Where(x => x.Sort < 0).OrderBy(x => x.Sort).ToList();
                var footList = proList.Where(x => x.Sort > 0).OrderBy(x => x.Sort).ToList();
                var middleList = proList.Where(x => x.Sort == 0).ToList();
                proList.Clear();
                if (headList.Count > 0)
                {
                    proList.AddRange(headList);
                }
                if (middleList.Count > 0)
                {
                    proList.AddRange(middleList);
                }
                if (footList.Count > 0)
                {
                    proList.AddRange(footList);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cpro"></param>
        /// <param name="_property"></param>
        private static void SetNullable(VoloModelProperty _cpro, PropertyInfo _property)
        {
            if (_cpro.DataType.Contains("Nullable"))
            {
                _cpro.Nullable = true;
                //Console.WriteLine(_property.PropertyType);
                if (_property.PropertyType.IsAssignableFrom(typeof(DateTime?)))
                {
                    _cpro.DataType = "DateTime";
                    _cpro.Required = false;
                }
                if (_property.PropertyType.IsAssignableFrom(typeof(Guid?)))
                {
                    _cpro.DataType = "Guid";
                    _cpro.Required = false;
                }
                if (_property.PropertyType.IsAssignableFrom(typeof(Int32?)))
                {
                    _cpro.DataType = "Int32";
                    _cpro.Required = false;
                }
                if (_property.PropertyType.IsAssignableFrom(typeof(Int64?)))
                {
                    _cpro.DataType = "Int64";
                    _cpro.Required = false;
                }
                if (_property.PropertyType.IsAssignableFrom(typeof(UInt32?)))
                {
                    _cpro.DataType = "Int32";
                    _cpro.Required = false;
                }
                if (_property.PropertyType.IsAssignableFrom(typeof(UInt64?)))
                {
                    _cpro.DataType = "Int64";
                    _cpro.Required = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_cpro"></param>
        /// <param name="_reads"></param>
        private static void HandlerEnumDatas(VoloModelProperty _cpro, List<VoloEnumProperty> _reads)
        {
            if (_cpro.Attributes == null)
            {
                _cpro.Attributes = new List<VoloModelAttribute>();
            }
            var find = _cpro.Attributes.Where(x => x.Name == "select" || x.Name == "lselect" || x.Name == "selects" || x.Name == "datalist").FirstOrDefault();
            if (find == null || find == default)
            {
                _cpro.Attributes.Add(new VoloModelAttribute { Name = "select", Args1 = System.Text.Json.JsonSerializer.Serialize(_reads) });
            }
            else
            {
                if (!String.IsNullOrEmpty(find.Args3))
                {
                    foreach (var _del in find.Args3.Split(","))
                    {
                        if (!String.IsNullOrEmpty(_del))
                        {
                            foreach (var ii in _reads)
                            {
                                var _val = ii.value.ToString();
                                switch (ii.key_type)
                                {
                                    case "int":
                                        {
                                            _val = ((int)ii.value).ToString();
                                        }
                                        break;
                                    case "uint":
                                        {
                                            _val = ((uint)ii.value).ToString();
                                        }
                                        break;
                                    case "byte":
                                        {
                                            _val = ((byte)ii.value).ToString();
                                        }
                                        break;
                                    case "sbyte":
                                        {
                                            _val = ((sbyte)ii.value).ToString();
                                        }
                                        break;
                                    case "short":
                                        {
                                            _val = ((short)ii.value).ToString();
                                        }
                                        break;
                                    case "ushort":
                                        {
                                            _val = ((ushort)ii.value).ToString();
                                        }
                                        break;
                                    case "long":
                                        {
                                            _val = ((long)ii.value).ToString();
                                        }
                                        break;
                                    case "ulong":
                                        {
                                            _val = ((ulong)ii.value).ToString();
                                        }
                                        break;
                                }
                                if (_val == _del)
                                {
                                    _reads.Remove(ii);
                                    break;
                                }
                            }
                        }
                    }
                }
                find.Args1 = System.Text.Json.JsonSerializer.Serialize(_reads);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_classType"></param>
        /// <param name="_obj"></param>
        /// <param name="ListModel">是否是表格数据模式</param>
        /// <param name="xmlModel">是否是文档模式</param>
        /// <returns></returns>
        private static VoloModelInfo DynamicReadPorperies1(Type _classType, Object _obj, bool ListModel = false, bool xmlModel = true)
        {
            var _classModel = new VoloModelInfo();
            var _bases = _classType.GetBaseClasses();
            if (_bases != null)
            {
                foreach (var _base in _bases)
                {
                    if (_base.FullName != "System.Object")
                    {
                        if (_base.GenericTypeArguments != null)
                        {
                            foreach (var _arg in _base.GenericTypeArguments)
                            {
                                switch (_arg.Name)
                                {
                                    case "Int64":
                                    case "UInt64":
                                        {
                                            _classModel.KeyType = "long";
                                        }
                                        break;
                                    case "String":
                                        {
                                            _classModel.KeyType = "string";
                                        }
                                        break;
                                    case "Guid":
                                        {
                                            _classModel.KeyType = "Guid";
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            if (xmlModel)
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                var _path = Path.GetDirectoryName(assemblyPath);
                LoadXmlFiles(_path);
                LoadSummaryFromXelement(_classType.FullName, _classModel, "T");
            }
            var fieldProperies = new List<VoloModelProperty>();
            fieldProperies = ReadForeachPorpety(_classType, _obj, "", null, ListModel, xmlModel);

            #region 处理排序问题
            //_classModel.Properties = _pro_list;//可能会打乱原来的排序，造成所有的都要设定sort,所以放业务中，业务按照需求自己排序
            //大于0 的追加到后面 小于0的追加到前面???为0的不动
            if (fieldProperies != null && fieldProperies.Count > 0 && fieldProperies.Where(x => x.Sort != 0).Any())
            {
                var headers = fieldProperies.Where(x => x.Sort < 0).OrderBy(x => x.Sort).ToList();
                var foots = fieldProperies.Where(x => x.Sort > 0).OrderBy(x => x.Sort).ToList();
                var middle = fieldProperies.Where(x => x.Sort == 0).ToList();
                fieldProperies.Clear();
                if (headers.Count > 0)
                {
                    fieldProperies.AddRange(headers);
                }
                if (middle.Count > 0)
                {
                    fieldProperies.AddRange(middle);
                }
                if (foots.Count > 0)
                {
                    fieldProperies.AddRange(foots);
                }
            }
            _classModel.Properties = fieldProperies;//可能会打乱原来的排序，造成所有的都要设定sort,所以放业务中，业务按照需求自己排序

            #endregion

            if (!String.IsNullOrEmpty(_classModel.Summary))
            {
                _classModel.Title = _classModel.Summary.Split(' ')[0].Replace(SplitChar, " ");
                if (_classModel.Summary.Contains(" "))
                {
                    _classModel.Desc = _classModel.Summary.Split(' ')[1].Replace(SplitChar, " ");
                }
            }
            var classAttributes = _classType.GetCustomAttributes<Attribute>();
            var voloAttributes = HandlerAttribute2(classAttributes, null, _classModel);
            if (voloAttributes?.Any() == true)
            {
                //转化AuthAttribute

                _classModel.Attributes = voloAttributes;
                //某一个字段？或者某一个class...
            }
            if (String.IsNullOrEmpty(_classModel.Title))
            {
                _classModel.Title = "未标注";
                //_classModel.Title = _classType.Name;
            }
            return _classModel;
        }

        /// <summary>
        /// 附带按钮权限得属性过滤
        /// </summary>
        /// <param name="_classType"></param>
        /// <param name="_obj"></param>
        /// <param name="action"></param>
        /// <param name="xmlModel"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        private static VoloModelInfo DynamicReadPorperies2(
            Type _classType,
            Object _obj,
            string action = "",
            bool xmlModel = true,
            IEnumerable<IPasteButtonRole> buttons = null)
        {
            var _classModel = new VoloModelInfo();
            var _bases = _classType.GetBaseClasses();
            if (_bases != null)
            {
                foreach (var _base in _bases)
                {
                    if (_base.FullName != "System.Object")
                    {
                        if (_base.GenericTypeArguments != null)
                        {
                            foreach (var _arg in _base.GenericTypeArguments)
                            {
                                switch (_arg.Name)
                                {
                                    case "Int64":
                                    case "UInt64":
                                        {
                                            _classModel.KeyType = "long";
                                        }
                                        break;
                                    case "String":
                                        {
                                            _classModel.KeyType = "string";
                                        }
                                        break;
                                    case "Guid":
                                        {
                                            _classModel.KeyType = "Guid";
                                        }
                                        break;
                                }
                            }
                        }
                    }
                }
            }
            if (xmlModel)
            {
                string assemblyPath = Assembly.GetExecutingAssembly().Location;
                var _path = Path.GetDirectoryName(assemblyPath);
                LoadXmlFiles(_path);
                LoadSummaryFromXelement(_classType.FullName, _classModel, "T");
            }
            var fieldProperies = new List<VoloModelProperty>();
            fieldProperies = ReadForeachPorpety2(_classType, _obj, "", null, _classType.Name, action, buttons, xmlModel);
            //_classModel.Properties = _pro_list;//可能会打乱原来的排序，造成所有的都要设定sort,所以放业务中，业务按照需求自己排序
            //大于0 的追加到后面 小于0的追加到前面???为0的不动
            if (fieldProperies != null && fieldProperies.Count > 0 && fieldProperies.Where(x => x.Sort != 0).Any())
            {
                var headers = fieldProperies.Where(x => x.Sort < 0).OrderBy(x => x.Sort).ToList();
                var foots = fieldProperies.Where(x => x.Sort > 0).OrderBy(x => x.Sort).ToList();
                var middle = fieldProperies.Where(x => x.Sort == 0).ToList();
                fieldProperies.Clear();
                if (headers.Count > 0)
                {
                    fieldProperies.AddRange(headers);
                }
                if (middle.Count > 0)
                {
                    fieldProperies.AddRange(middle);
                }
                if (foots.Count > 0)
                {
                    fieldProperies.AddRange(foots);
                }
            }
            _classModel.Properties = fieldProperies;//可能会打乱原来的排序，造成所有的都要设定sort,所以放业务中，业务按照需求自己排序

            if (!String.IsNullOrEmpty(_classModel.Summary))
            {
                _classModel.Title = _classModel.Summary.Split(' ')[0].Replace(SplitChar, " ");
                if (_classModel.Summary.Contains(" "))
                {
                    _classModel.Desc = _classModel.Summary.Split(' ')[1].Replace(SplitChar, " ");
                }
            }
            var classAttributes = _classType.GetCustomAttributes<Attribute>();
            var voloAttributes = HandlerAttribute2(classAttributes, null, _classModel, _classType.Name, action, buttons);
            if (voloAttributes != null && voloAttributes.Count > 0)
            {
                _classModel.Attributes = voloAttributes;
            }
            if (String.IsNullOrEmpty(_classModel.Title))
            {
                _classModel.Title = "未标注";
            }
            return _classModel;
        }

        /// <summary>
        /// 支持DirectSun模式，多层级模式
        /// 使用得是XML文档模式
        /// </summary>
        /// <param name="_classType"></param>
        /// <param name="_obj"></param>
        /// <param name="head"></param>
        /// <param name="group_attribute"></param>
        /// <param name="ListModel"></param>
        /// <param name="xmlModel"></param>
        /// <returns></returns>
        private static List<VoloModelProperty> ReadForeachPorpety(Type _classType, object _obj, string head = "", VoloModelAttribute group_attribute = null, bool ListModel = false, bool xmlModel = true)
        {
            //string assemblyPath = Assembly.GetExecutingAssembly().Location;
            //var _path = Path.GetDirectoryName(assemblyPath);
            //var _name = _classType.Assembly.GetName().Name;//这个获取的是来源的

            if (_obj == null)
            {
                _obj = Activator.CreateInstance(_classType);
            }
            //var _obj = Activator.CreateInstance(_classType);
            var _pro_list = new List<VoloModelProperty>();
            foreach (var _property in ReadTypeProperties(_classType))
            {
                if (!_property.PropertyType.IsPublic)
                {
                    //Console.WriteLine($"{_property.Name} is not public !");
                    continue;
                }

                if (Attribute.IsDefined(_property, typeof(JsonIgnoreAttribute)) || Attribute.IsDefined(_property, typeof(PasteIgnoreAttribute)))
                {
                    continue;
                }
                var proInfo = new VoloModelProperty();
                if (xmlModel)
                {
                    ReadXmlSummary(_classType, _property, proInfo, 0);
                }
                var jsonPropertyNameAttr = _property.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (jsonPropertyNameAttr != null)
                {
                    proInfo.Name = $"{head}{jsonPropertyNameAttr.Name}";
                }
                else
                {
                    proInfo.Name = $"{head}{_property.Name.FirstLetterToLower()}";
                }
                proInfo.DataType = _property.PropertyType.Name;
                var attributes = _property.GetCustomAttributes<Attribute>();
                var _attributes = HandlerAttribute2(attributes, proInfo);
                if (_attributes?.Count > 0)
                {
                    proInfo.Attributes = _attributes;
                }
                if (group_attribute != null && group_attribute != default)
                {
                    if (proInfo.Attributes == null)
                    {
                        proInfo.Attributes = new List<VoloModelAttribute>();
                    }
                    //标注的权重更高
                    var _find = proInfo.Attributes.Where(x => x.Name == "group").FirstOrDefault();
                    if (_find == null || _find == default)
                    {
                        proInfo.Attributes.Add(group_attribute);
                    }
                }
                if (!String.IsNullOrEmpty(proInfo.Summary))
                {
                    proInfo.Title = proInfo.Summary.Split(' ')[0].Replace(SplitChar, " ");
                    if (proInfo.Summary.Contains(" "))
                    {
                        proInfo.Placeholder = proInfo.Summary.Split(' ')[1].Replace(SplitChar, " ");
                    }
                }
                if (proInfo.DataType == "DateTime")
                {
                    proInfo.Required = true;
                }
                if (proInfo.DataType.Contains("Nullable"))
                {
                    SetNullable(proInfo, _property);
                }

                //因为上面可能会把新的数据类型穿透出来，所以在这里进行默认值的校验，特别是时间格式
                object _value = null;
                _value = _property.GetValue(_obj);

                if (_value != null)
                {
                    if (proInfo.DataType == "DateTime")
                    {
                        if (DateTime.TryParse(_value.ToString(), out var _time))
                        {
                            if (_time.Year > 1000)
                            {
                                proInfo.Value = _time.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                    }
                    else
                    {
                        proInfo.Value = _value;
                    }
                }

                VoloModelAttribute _g_attribute = null;
                if (proInfo.Attributes != null)
                {
                    _g_attribute = proInfo.Attributes.Where(x => x.Name == "group").FirstOrDefault();
                }
                //判断是不是枚举类型
                var _pro_full_name = _property.PropertyType.FullName;
                if (!_pro_full_name.StartsWith("System"))
                {

                    var _proType = _property.PropertyType;
                    if (_proType.BaseType != null)
                    {
                        var _base_full_name = _proType.BaseType.FullName;
                        if (_base_full_name == "System.Enum" || _base_full_name == "System.Array")
                        {
                            var _is_enum_type = false;
                            List<VoloEnumProperty> _reads = null;
                            if (_base_full_name == "System.Enum")
                            {
                                _is_enum_type = true;
                                if (xmlModel)
                                {
                                    _reads = ReadEnumProperty(_property.PropertyType, ListModel);
                                }
                                else
                                {
                                    _reads = GetAllEnumItemInfo(_property.PropertyType, ListModel);
                                }
                            }
                            else
                            {
                                var _next_type = _proType.GetElementType();
                                if (_next_type.BaseType != null)
                                {
                                    if (_next_type.BaseType.FullName == "System.Enum")
                                    {
                                        _is_enum_type = true;
                                        if (xmlModel)
                                        {
                                            _reads = ReadEnumProperty(_next_type, ListModel);
                                        }
                                        else
                                        {
                                            _reads = GetAllEnumItemInfo(_next_type, ListModel);
                                        }
                                    }
                                }
                            }
                            if (_reads != null)
                            {
                                HandlerEnumDatas(proInfo, _reads);
                            }
                            if (_is_enum_type)
                            {
                                _pro_list.Add(proInfo);
                                continue;
                            }
                        }

                        if (_property.PropertyType != _classType)
                        {
                            if (_attributes != null && _attributes.Any(x => x.Name == "directsun"))
                            {
                                //var sort = ReadDirectSunSortAttribute(_attributes);
                                //处理sort的问题，传递给下一级... .. .
                                var _read = ReadNextProperty(_property, _obj, proInfo, _pro_list, _g_attribute, ListModel, _attributes.Any(x => x.Name == "hidden"), xmlModel, proInfo.Sort);
                                if (_read)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (_pro_full_name.StartsWith("System.Collections.Generic"))
                    {
                        if (_attributes != null && _attributes.Any(x => x.Name == "directsun"))
                        {
                            //var sort = ReadDirectSunSortAttribute(_attributes);
                            var _read = ReadNextProperty(_property, _obj, proInfo, _pro_list, _g_attribute, ListModel, _attributes.Any(x => x.Name == "hidden"), xmlModel, proInfo.Sort);
                            if (_read)
                            {
                                continue;
                            }
                        }
                    }
                }

                //有Description特性的话，如果是空值，可能是故意这么设定的!
                if (String.IsNullOrEmpty(proInfo.Title) && !proInfo.HasDescAttribute)
                {
                    switch (proInfo.Name)
                    {
                        case "id":
                            {
                                proInfo.Title = "标识";
                            }
                            break;
                        case "createDate":
                            {
                                proInfo.Title = "创建时间";
                            }
                            break;
                        case "updateDate":
                            {
                                proInfo.Title = "更新时间";
                            }
                            break;
                        case "name":
                            {
                                proInfo.Title = "名称";
                            }
                            break;
                        case "desc":
                            {
                                proInfo.Title = "说明";
                            }
                            break;
                        case "mark":
                            {
                                proInfo.Title = "备注";
                            }
                            break;
                        case "userId":
                            {
                                proInfo.Title = "用户";
                            }
                            break;
                        case "extendBind":
                            {
                                proInfo.Title = "绑定";
                            }
                            break;
                        case "isEnable":
                            {
                                proInfo.Title = "状态";
                            }
                            break;
                        default:
                            {
                                if (proInfo.Name.EndsWith("__id"))
                                {
                                    proInfo.Title = "标识";
                                }
                                else
                                {
                                    proInfo.Title = proInfo.Name;
                                }
                            }
                            break;
                    }
                }
                //id写入到前面去
                if (proInfo.Name == "id")
                {
                    _pro_list.Insert(0, proInfo);
                }
                else
                {
                    _pro_list.Add(proInfo);
                }
            }
            return _pro_list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_classType"></param>
        /// <param name="_obj"></param>
        /// <param name="head"></param>
        /// <param name="group_attribute"></param>
        /// <param name="dtoName"></param>
        /// <param name="action"></param>
        /// <param name="buttons"></param>
        /// <param name="xmlModel"></param>
        /// <returns></returns>
        private static List<VoloModelProperty> ReadForeachPorpety2(
            Type _classType,
            object _obj,
            string head = "",
            VoloModelAttribute group_attribute = null,
            string dtoName = "",
            string action = "",
            IEnumerable<IPasteButtonRole> buttons = null,
            bool xmlModel = true)
        {
            var ListModel = (action == "list");
            if (_obj == null)
            {
                _obj = Activator.CreateInstance(_classType);
            }
            var _pro_list = new List<VoloModelProperty>();
            foreach (var _property in ReadTypeProperties(_classType))
            {
                if (!_property.PropertyType.IsPublic)
                {
                    continue;
                }
                if (Attribute.IsDefined(_property, typeof(JsonIgnoreAttribute)) || Attribute.IsDefined(_property, typeof(PasteIgnoreAttribute)))
                {
                    continue;
                }
                var proInfo = new VoloModelProperty();
                if (xmlModel)
                {
                    ReadXmlSummary(_classType, _property, proInfo, 0);
                }
                var jsonPropertyNameAttr = _property.GetCustomAttribute<JsonPropertyNameAttribute>();
                if (jsonPropertyNameAttr != null)
                {
                    proInfo.Name = $"{head}{jsonPropertyNameAttr.Name}";
                }
                else
                {
                    proInfo.Name = $"{head}{_property.Name.FirstLetterToLower()}";
                }

                proInfo.DataType = _property.PropertyType.Name;
                var attributes = _property.GetCustomAttributes<Attribute>();
                var _attributes = HandlerAttribute2(attributes, proInfo, null, dtoName, action, buttons);
                if (_attributes.Count > 0)
                {
                    proInfo.Attributes = _attributes;
                }
                if (group_attribute != null && group_attribute != default)
                {
                    if (proInfo.Attributes == null)
                    {
                        proInfo.Attributes = new List<VoloModelAttribute>();
                    }
                    //标注的权重更高
                    var _find = proInfo.Attributes.Where(x => x.Name == "group").FirstOrDefault();
                    if (_find == null || _find == default)
                    {
                        proInfo.Attributes.Add(group_attribute);
                    }
                }
                if (!String.IsNullOrEmpty(proInfo.Summary))
                {
                    proInfo.Title = proInfo.Summary.Split(' ')[0].Replace(SplitChar, " ");
                    if (proInfo.Summary.Contains(" "))
                    {
                        proInfo.Placeholder = proInfo.Summary.Split(' ')[1].Replace(SplitChar, " ");
                    }
                }
                if (proInfo.DataType == "DateTime")
                {
                    proInfo.Required = true;
                }
                if (proInfo.DataType.Contains("Nullable"))
                {
                    SetNullable(proInfo, _property);
                }

                //因为上面可能会把新的数据类型穿透出来，所以在这里进行默认值的校验，特别是时间格式
                object _value = null;
                _value = _property.GetValue(_obj);

                if (_value != null)
                {
                    if (proInfo.DataType == "DateTime")
                    {
                        if (DateTime.TryParse(_value.ToString(), out var _time))
                        {
                            if (_time.Year > 1000)
                            {
                                proInfo.Value = _time.ToString("yyyy-MM-dd HH:mm:ss");
                            }
                        }
                    }
                    else
                    {
                        proInfo.Value = _value;
                    }
                }

                VoloModelAttribute _g_attribute = null;
                if (proInfo.Attributes != null)
                {
                    _g_attribute = proInfo.Attributes.Where(x => x.Name == "group").FirstOrDefault();
                }
                //判断是不是枚举类型
                var _pro_full_name = _property.PropertyType.FullName;
                if (!_pro_full_name.StartsWith("System"))
                {

                    var _proType = _property.PropertyType;
                    if (_proType.BaseType != null)
                    {
                        var _base_full_name = _proType.BaseType.FullName;
                        if (_base_full_name == "System.Enum" || _base_full_name == "System.Array")
                        {
                            var _is_enum_type = false;
                            List<VoloEnumProperty> _reads = null;
                            if (_base_full_name == "System.Enum")
                            {
                                _is_enum_type = true;
                                if (xmlModel)
                                {
                                    _reads = ReadEnumProperty(_property.PropertyType, ListModel);
                                }
                                else
                                {
                                    _reads = GetAllEnumItemInfo(_property.PropertyType, ListModel);
                                }
                            }
                            else
                            {
                                var _next_type = _proType.GetElementType();
                                if (_next_type.BaseType != null)
                                {
                                    if (_next_type.BaseType.FullName == "System.Enum")
                                    {
                                        _is_enum_type = true;
                                        if (xmlModel)
                                        {
                                            _reads = ReadEnumProperty(_next_type, ListModel);
                                        }
                                        else
                                        {
                                            _reads = GetAllEnumItemInfo(_next_type, ListModel);
                                        }
                                    }
                                }
                            }
                            if (_reads != null)
                            {
                                HandlerEnumDatas(proInfo, _reads);
                            }
                            if (_is_enum_type)
                            {
                                _pro_list.Add(proInfo);
                                continue;
                            }
                        }

                        if (_property.PropertyType != _classType)
                        {
                            if (_attributes != null && _attributes.Any(x => x.Name == "directsun"))
                            {
                                //var sort = ReadDirectSunSortAttribute(_attributes);
                                //处理sort的问题，传递给下一级... .. .
                                var _read = ReadNextProperty(_property, _obj, proInfo, _pro_list, _g_attribute, ListModel, _attributes.Any(x => x.Name == "hidden"), xmlModel, proInfo.Sort);
                                if (_read)
                                {
                                    continue;
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (_pro_full_name.StartsWith("System.Collections.Generic"))
                    {
                        if (_attributes != null && _attributes.Any(x => x.Name == "directsun"))
                        {
                            //var sort = ReadDirectSunSortAttribute(_attributes);
                            var _read = ReadNextProperty(_property, _obj, proInfo, _pro_list, _g_attribute, ListModel, _attributes.Any(x => x.Name == "hidden"), xmlModel, proInfo.Sort);
                            if (_read)
                            {
                                continue;
                            }
                        }
                    }
                }

                //有Description特性的话，如果是空值，可能是故意这么设定的!
                if (String.IsNullOrEmpty(proInfo.Title) && !proInfo.HasDescAttribute)
                {
                    switch (proInfo.Name)
                    {
                        case "id":
                            {
                                proInfo.Title = "标识";
                            }
                            break;
                        case "createDate":
                            {
                                proInfo.Title = "创建时间";
                            }
                            break;
                        case "updateDate":
                            {
                                proInfo.Title = "更新时间";
                            }
                            break;
                        case "name":
                            {
                                proInfo.Title = "名称";
                            }
                            break;
                        case "desc":
                            {
                                proInfo.Title = "说明";
                            }
                            break;
                        case "mark":
                            {
                                proInfo.Title = "备注";
                            }
                            break;
                        case "userId":
                            {
                                proInfo.Title = "用户";
                            }
                            break;
                        case "extendBind":
                            {
                                proInfo.Title = "绑定";
                            }
                            break;
                        case "isEnable":
                            {
                                proInfo.Title = "状态";
                            }
                            break;
                        default:
                            {
                                if (proInfo.Name.EndsWith("__id"))
                                {
                                    proInfo.Title = "标识";
                                }
                                else
                                {
                                    proInfo.Title = proInfo.Name;
                                }
                            }
                            break;
                    }
                }
                //id写入到前面去
                if (proInfo.Name == "id")
                {
                    _pro_list.Insert(0, proInfo);
                }
                else
                {
                    _pro_list.Add(proInfo);
                }
            }
            return _pro_list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_property"></param>
        /// <param name="_obj"></param>
        /// <param name="_cpro"></param>
        /// <param name="_pro_list"></param>
        /// <param name="_group_attribute"></param>
        /// <param name="ListModel"></param>
        /// <param name="HideGroup"></param>
        /// <param name="xmlModel"></param>
        /// <param name="sort"></param>
        /// <returns></returns>
        private static bool ReadNextProperty(PropertyInfo _property, object _obj, VoloModelProperty _cpro, List<VoloModelProperty> _pro_list, VoloModelAttribute _group_attribute, bool ListModel, bool HideGroup = false, bool xmlModel = true, int sort = 0)
        {
            var _newobj = _property.GetValue(_obj);
            if (_property.PropertyType.IsArray || _property.PropertyType.IsGenericType)
            {
                if (_newobj != null)
                {
                    if (_newobj is IEnumerable enumerable)
                    {
                        var arrayIndex = 0;
                        var sortIndex = sort;
                        foreach (var item in enumerable)
                        {
                            VoloModelAttribute _target = null;
                            if (_group_attribute == null || _group_attribute == default)
                            {
                                if (!String.IsNullOrEmpty(_cpro.Title))
                                {
                                    _target = new VoloModelAttribute
                                    {
                                        Name = "group",
                                        Args1 = $"{_cpro.Name}__{(arrayIndex)}",
                                        Args2 = $"{_cpro.Title}",
                                        Args3 = "open",
                                        Args4 = "edit"
                                    };
                                    //如果父级配置了hidden，则要hide
                                    if (HideGroup)
                                    {
                                        _target.Args3 = "hidden";
                                    }
                                }
                            }
                            else
                            {
                                _target = _group_attribute;
                            }
                            var _reads = ReadForeachPorpety(item.GetType(), item, _cpro.Name + $"__{arrayIndex}__", _target, ListModel, xmlModel);
                            if (_reads != null && _reads.Count > 0)
                            {
                                if (sort != 0)
                                {
                                    foreach (var ii in _reads)
                                    {
                                        ii.Sort = sortIndex;
                                        sortIndex++;
                                    }
                                }
                                _pro_list.AddRange(_reads);
                            }
                            arrayIndex++;
                        }
                        return true;
                    }
                }
            }
            else
            {
                var _reads = ReadForeachPorpety(_property.PropertyType, _newobj, _cpro.Name + "__", _group_attribute, ListModel, xmlModel);
                if (_reads != null && _reads.Count > 0)
                {
                    if (sort != 0)
                    {
                        var sortIndex = sort;
                        foreach (var ii in _reads)
                        {
                            ii.Sort = sortIndex;
                            sortIndex++;
                        }
                    }
                    _pro_list.AddRange(_reads);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 读取一个类型的接口 读取默认值 多层级会使用__链接 需要标注特性directsun
        /// 建议使用Description特性模式的AttributeReadModelProperty
        /// </summary>
        /// <param name="_classType">typeof(Model)</param>
        /// <param name="ListModel"></param>
        public static VoloModelInfo DynamicReadTypeProperty(Type _classType, bool ListModel = false)
        {
            object _new = Activator.CreateInstance(_classType);
            var _read = DynamicReadPorperies1(_classType, _new, ListModel);
            FixGroupInfo(_read);
            return _read;
        }

        /// <summary>
        /// 读取一个Model的 结构和值 多层级会使用__链接  需要标注特性directsun
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_t"></param>
        /// <param name="ListModel"></param>
        /// <returns></returns>
        public static VoloModelInfo DynamicReadModelProperty<T>(T _t, bool ListModel = false)
        {
            var stop = new Stopwatch();
            stop.Start();
            var _classType = typeof(T);
            var _read = DynamicReadPorperies1(_classType, _t, ListModel);
            FixGroupInfo(_read);
            var time = stop.ElapsedMilliseconds;
            if (time > 1000)
            {
                Console.WriteLine($"ReadModelProperty {time}ms !");
            }
            stop.Stop();
            return _read;
        }

        /// <summary>
        /// 特性模式 不读取XML文档
        /// 需要用特性Description标注文档
        /// 当前为推荐模式
        /// 支持多层级directsun模式
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_t"></param>
        /// <param name="ListModel"></param>
        /// <returns></returns>
        public static VoloModelInfo AttributeReadModelProperty<T>(T _t, bool ListModel = false)
        {
            var _classType = typeof(T);
            if (!ListModel && _classType.Name.EndsWith("ListDto"))
            {
                ListModel = true;
            }
            var _read = DynamicReadPorperies1(_classType, _t, ListModel, false);
            FixGroupInfo(_read);
            return _read;
        }

        /// <summary>
        /// 特性模式 不读取XML文档
        /// 需要用特性Description标注文档
        /// 当前为推荐模式
        /// 支持多层级directsun模式
        /// 当前为不需要泛型的调用模式
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="classType"></param>
        /// <param name="ListModel"></param>
        /// <returns></returns>
        public static VoloModelInfo AttributeReadModelProperty(object obj, Type classType, bool ListModel = false)
        {
            if (!ListModel && classType.Name.EndsWith("ListDto"))
            {
                ListModel = true;
            }
            var _read = DynamicReadPorperies1(classType, obj, ListModel, false);
            FixGroupInfo(_read);
            return _read;
        }

        /// <summary>
        /// 读取某一个Model的模型数据
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="ListModel">不建议填写，自动从Dto中解析</param>
        /// <returns></returns>
        public static VoloModelInfo AttributeReadModelProperty(object obj, bool ListModel)
        {
            var _classType = obj.GetType();
            //return AttributeReadModelProperty(obj, _classType, ListModel);
            var _read = DynamicReadPorperies1(_classType, obj, ListModel, false);
            FixGroupInfo(_read);
            return _read;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static VoloModelInfo AttributeReadModelProperty(object obj)
        {
            var _classType = obj.GetType();
            var className = _classType.Name;
            //return AttributeReadModelProperty(obj, _classType, className.EndsWith("ListDto"));
            var _read = DynamicReadPorperies1(_classType, obj, className.EndsWith("ListDto"), false);
            FixGroupInfo(_read);
            return _read;
        }

        /// <summary>
        /// 附带按钮权限的过滤
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="buttons">为Null表示不启用，所以权限判断必然不为Null,传空也行,自行缓存用户权限</param>
        /// <returns></returns>
        public static VoloModelInfo AttributeReadModelProperty2(object obj, IEnumerable<IPasteButtonRole> buttons)
        {
            var classType = obj.GetType();
            var action = "detail";
            var className = classType.Name;
            if (className.EndsWith("AddDto"))
            {
                action = "add";
            }
            else if (className.EndsWith("UpdateDto"))
            {
                action = "update";
            }
            else if (className.EndsWith("ListDto"))
            {
                action = "list";
            }
            var _read = DynamicReadPorperies2(classType, obj, action, false, buttons);
            FixGroupInfo(_read);
            return _read;
            //return AttributeReadModelProperty(obj, _classType, ListModel);
        }

        /// <summary>
        /// 应对DefaultAppService的模式
        /// 附带按钮权限的过滤
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="classType"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        public static VoloModelInfo AttributeReadModelProperty2(object obj, Type classType, IEnumerable<IPasteButtonRole> buttons)
        {
            //var classType = obj.GetType();
            var action = "detail";
            var className = classType.Name;
            if (className.EndsWith("AddDto"))
            {
                action = "add";
            }
            else if (className.EndsWith("UpdateDto"))
            {
                action = "update";
            }
            else if (className.EndsWith("ListDto"))
            {
                action = "list";
            }
            var _read = DynamicReadPorperies2(classType, obj, action, false, buttons);
            FixGroupInfo(_read);
            return _read;
            //var _read = DynamicReadPorperies1(classType, obj, ListModel, false);
            //FixGroupInfo(_read);
            //return _read;
        }

        /// <summary>
        /// 缓存xml文档内容
        /// </summary>
        private static ConcurrentQueue<XDocument> cachedXmlDocuments = new ConcurrentQueue<XDocument>();

        /// <summary>
        /// 缓存数据对象
        /// </summary>
        private static ConcurrentDictionary<string, XElement> CacheXelements = new ConcurrentDictionary<string, XElement>();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="full_name"></param>
        /// <param name="_classModel"></param>
        /// <param name="name_space"></param>
        private static void LoadSummaryFromXelement(string full_name, VoloModelInfo _classModel, string name_space = "T")
        {
            var xelement = LoadXelement(full_name, name_space);
            if (xelement != null)
            {
                var _classSummary = xelement.Element("summary")?.Value;
                if (!string.IsNullOrEmpty(_classSummary))
                {
                    _classSummary = _classSummary.Replace("\r\n", "").Trim();
                    _classModel.Summary = _classSummary;
                }
            }

        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="full_name"></param>
        ///// <param name="name_space"></param>
        ///// <returns></returns>
        //private static XElement LoadXelement(string full_name, string name_space = "T")
        //{
        //    if (CacheXelements != null && CacheXelements.Any())
        //    {
        //        if (CacheXelements.TryGetValue(full_name, out var xelement))
        //        {
        //            return xelement;
        //        }
        //    }

        //    foreach (var xmlDoc in cachedXmlDocuments)
        //    {
        //        XElement typeElement = xmlDoc.Descendants("member").FirstOrDefault(member => member.Attribute("name")?.Value == $"{name_space}:{full_name}");
        //        if (typeElement != null)
        //        {
        //            CacheXelements.TryAdd(full_name, typeElement);
        //            return typeElement;
        //        }
        //    }
        //    return null;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="full_name"></param>
        /// <param name="name_space"></param>
        /// <returns></returns>
        private static XElement LoadXelement(string full_name, string name_space = "T")
        {
            var typeName = $"xml{full_name}";
            // 使用GetOrCreate替代TryGetValue+Set
            return _memoryCache.GetOrCreate(typeName, entry =>
            {
                // 这里的逻辑只会被一个线程执行（其他线程等待）
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1800); // 设置过期时间
                foreach (var xmlDoc in cachedXmlDocuments)
                {
                    XElement typeElement = xmlDoc.Descendants("member").FirstOrDefault(member => member.Attribute("name")?.Value == $"{name_space}:{full_name}");
                    if (typeElement != null)
                    {
                        //CacheXelements.TryAdd(full_name, typeElement);
                        return typeElement;
                    }
                }
                return null;
            });
        }


        /// <summary>
        /// 载入当前文件夹的所有文档
        /// </summary>
        /// <param name="currentDirectory"></param>
        private static void LoadXmlFiles(string currentDirectory)
        {
            if (cachedXmlDocuments.Count == 0)
            {
                string[] xmlFiles = Directory.GetFiles(currentDirectory, "*.xml");
                foreach (string xmlFilePath in xmlFiles)
                {
                    try
                    {
                        GetXmlDocument(xmlFilePath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"处理文件 {Path.GetFileName(xmlFilePath)} 时发生错误: {ex.Message}");
                    }
                }
            }
        }
        /// <summary>
        /// 获得xml文档
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        /// <exception cref="FileNotFoundException"></exception>
        private static void GetXmlDocument(string filePath)
        {
            if (File.Exists(filePath))
            {
                var _load = XDocument.Load(filePath);
                if (filePath.Contains(".Contracts"))
                {
                    cachedXmlDocuments.Enqueue(_load);
                }
                else if (filePath.Contains(".Domain"))
                {
                    if (cachedXmlDocuments.Count > 0)
                    {
                        cachedXmlDocuments.Enqueue(_load);
                    }
                    else
                    {
                        cachedXmlDocuments.Enqueue(_load);
                    }
                }
                else
                {
                    cachedXmlDocuments.Enqueue(_load);
                }
            }
        }
        /// <summary>
        /// 矫正group信息
        /// </summary>
        /// <param name="_read"></param>
        private static void FixGroupInfo(VoloModelInfo _read)
        {
            if (_read.Properties != null)
            {
                var _dic_group = new Dictionary<string, VoloModelAttribute>();
                foreach (var item in _read.Properties)
                {
                    if (item.Attributes != null)
                    {
                        var _find = item.Attributes.Where(x => x.Name == "group").FirstOrDefault();
                        if (_find != null && _find != default)
                        {
                            if (!_dic_group.ContainsKey(_find.Args1))
                            {
                                _dic_group.TryAdd(_find.Args1, _find);
                            }
                        }
                    }
                }
                if (_dic_group.Count > 0)
                {
                    if (_read.Attributes == null || _read.Attributes.Count == 0)
                    {
                        _read.Attributes = new List<VoloModelAttribute>();
                        _read.Attributes.AddRange(_dic_group.Values.ToList());
                    }
                    else
                    {
                        foreach (var ii in _read.Attributes)
                        {
                            if (ii.Name == "group")
                            {
                                if (_dic_group.ContainsKey(ii.Args1))
                                {
                                    _dic_group.Remove(ii.Args1);
                                }
                            }
                        }
                        if (_dic_group.Count > 0)
                        {
                            _read.Attributes.AddRange(_dic_group.Values.ToList());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 读取一个类型的接口 读取默认值
        /// </summary>
        /// <param name="_classType">typeof(Model)</param>
        /// <param name="ListModel"></param>
        public static VoloModelInfo ReadTypeProperty(Type _classType, bool ListModel = false)
        {
            object _new = Activator.CreateInstance(_classType);
            var _read = ReadPropertyAndDefaultValue(_classType, _new, ListModel);
            FixGroupInfo(_read);
            return _read;
        }

        /// <summary>
        /// 读取一个Model的 结构和值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="_new"></param>
        /// <param name="ListModel"></param>
        public static VoloModelInfo ReadModelProperty<T>(T _new, bool ListModel = false)
        {
            var stop = new Stopwatch();
            stop.Start();
            var _classType = typeof(T);
            var _read = ReadPropertyAndDefaultValue(_classType, _new, ListModel);
            FixGroupInfo(_read);
            var time = stop.ElapsedMilliseconds;
            if (time > 1000)
            {
                Console.WriteLine($"ReadModelProperty {time}ms !");
            }
            stop.Stop();
            return _read;
        }

        /// <summary>
        /// 取代ReadSummary
        /// </summary>
        /// <param name="_classType"></param>
        /// <param name="_property"></param>
        /// <param name="_cpro"></param>
        /// <param name="_index"></param>
        private static void ReadXmlSummary(Type _classType, PropertyInfo _property, VoloModelProperty _cpro, int _index = 0)
        {
            var typeElement = LoadXelement($"{_classType.FullName}.{_property.Name}", "P");
            //var _pro_full = $"P:{_classType.FullName}.{_property.Name}";
            //XElement typeElement = xmlDoc.Descendants("member").FirstOrDefault(member => member.Attribute("name")?.Value == _pro_full);
            if (typeElement != null)
            {
                var _summary = typeElement.Element("summary")?.Value;
                if (!String.IsNullOrEmpty(_summary))
                {
                    _summary = _summary.Replace("\r\n", "").Trim();
                    _cpro.Summary = _summary;
                }
            }
            else
            {
                if (_property.DeclaringType != null && _index < 6)
                {
                    var _declartype = _property.DeclaringType;
                    if (_declartype.FullName != _classType.FullName)
                    {
                        var _zindex = _index++;
                        ReadXmlSummary(_declartype, _property, _cpro, _zindex);
                    }
                }
            }
        }

        /// <summary>
        /// 获取特性列表
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="property">Property</param>
        /// <param name="model">Class</param>
        /// <returns></returns>
        [Obsolete]
        private static List<VoloModelAttribute> HandlerAttribute(IEnumerable<Attribute> attributes, VoloModelProperty property, VoloModelInfo model = null)
        {
            var _attributes = new List<VoloModelAttribute>();
            if (attributes != null)
            {
                foreach (var _attribute in attributes)
                {
                    var _attri = new VoloModelAttribute();
                    if (_attribute is DescriptionAttribute descriptionAttribute)
                    {
                        //var descriptionAttribute = (DescriptionAttribute)_attribute;
                        if (descriptionAttribute != null)
                        {
                            if (property != null)
                            {
                                var strs = descriptionAttribute.Description.Split(" ");
                                property.Title = strs[0].Trim();
                                property.HasDescAttribute = true;
                                property.Placeholder = strs.Length > 1 ? strs[1].Replace(SplitChar, " ") : "";
                            }
                        }
                        if (model != null)
                        {
                            var strs = descriptionAttribute.Description.Split(" ");
                            model.Title = strs[0].Trim();
                            model.Desc = strs.Length > 1 ? strs[1].Replace(SplitChar, " ") : "";
                        }
                        continue;
                    }

                    if (_attribute is MaxLengthAttribute maxAttribute)
                    {
                        //var _bute = (MaxLengthAttribute)_attribute;
                        _attri.Name = "maxlength";
                        _attri.Error = maxAttribute.ErrorMessage;
                        _attri.Args1 = maxAttribute.Length.ToString();
                        if (property != null)
                        {
                            property.Maxlength = maxAttribute.Length;
                        }
                    }
                    if (_attribute is RequiredAttribute requiredAttribute)
                    {
                        //var _bute = (RequiredAttribute)_attribute;
                        _attri.Name = "required";
                        if (property != null)
                        {
                            property.Required = true;
                        }
                        _attri.Error = requiredAttribute.ErrorMessage;
                    }

                    if (_attribute is RangeAttribute rangeAttribute)
                    {
                        //var _bute = (RangeAttribute)_attribute;
                        _attri.Name = "range";
                        _attri.Args1 = rangeAttribute.Minimum.ToString();
                        _attri.Args2 = rangeAttribute.Maximum.ToString();
                        _attri.Error = rangeAttribute.ErrorMessage;
                    }

                    if (_attribute is RegularExpressionAttribute regularAttribute)
                    {
                        //var _bute = (RegularExpressionAttribute)_attribute;
                        _attri.Name = "pattern";
                        _attri.Args1 = regularAttribute.Pattern.ToString();
                        _attri.Error = regularAttribute.ErrorMessage;
                    }

                    if (String.IsNullOrEmpty(_attri.Name))
                    {
                        //var _bute = _attribute as ColumnDataTypeAttribute;
                        if (_attribute is ColumnDataTypeAttribute _bute)
                        {
                            _attri.Name = _bute.Name;
                            _attri.Args1 = _bute.Args1;
                            _attri.Args2 = _bute.Args2;
                            _attri.Args3 = _bute.Args3;
                            _attri.Args4 = _bute.Args4;
                            _attri.Error = _bute.ErrorMessage;
                            _attri.Order = _bute.Order;
                            if (_bute.IsServerAttribute)
                            {
                                continue;
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(_attri.Name))
                    {
                        var __name = _attribute.GetType().ToString();
                        if (__name.Contains("."))
                        {
                            var splits = __name.Split('.');
                            _attri.Name = splits[splits.Length - 1];
                        }
                        else
                        {
                            _attri.Name = __name;
                        }
                        _attri.Args1 = _attribute.ToString();
                    }
                    if (!String.IsNullOrEmpty(_attri.Name))
                    {
                        if (_attri.Name == "sort")
                        {
                            if (int.TryParse(_attri.Args1, out var _sort))
                            {
                                if (property != null)
                                {
                                    property.Sort = _sort;
                                }
                            }
                            continue;
                        }
                        if (_attri.Name == "orderby")
                        {
                            if (property != null)
                            {
                                if (String.IsNullOrEmpty(_attri.Args1))
                                {
                                    _attri.Args1 = property.Name;
                                }
                                if (String.IsNullOrEmpty(_attri.Args2))
                                {
                                    _attri.Args2 = $"{property.Name} Desc";
                                }
                            }
                        }
                        _attributes.Add(_attri);
                    }
                }

                if (!_attributes.Any(x => x.Name == "enable" || x.Name == "disable"))
                {
                    IEnumerable<PasteAuthAttribute> pasteAuthAttributes = attributes.OfType<PasteAuthAttribute>();
                    if (pasteAuthAttributes.Any())
                    {
                        var auths = pasteAuthAttributes.Where(x => String.IsNullOrEmpty(x.Model) && String.IsNullOrEmpty(x.Role)).ToList();
                        if (auths.Any())
                        {
                            var voloAttr = ExtendHelper.ToEnableAttribute(null, auths);
                            if (voloAttr != null)
                            {
                                _attributes.Add(voloAttr);
                            }
                        }
                    }
                }
            }
            return _attributes?.Any() == true ? _attributes.OrderBy(x => x.Order).ToList() : null;
        }

        /// <summary>
        /// 获取属性列表
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="property"></param>
        /// <param name="model"></param>
        /// <param name="dtoName"></param>
        /// <param name="action"></param>
        /// <param name="buttons"></param>
        /// <returns></returns>
        private static List<VoloModelAttribute> HandlerAttribute2(
            IEnumerable<Attribute> attributes,
            VoloModelProperty property,
            VoloModelInfo model = null,
            string dtoName = "",
            string action = "",
            IEnumerable<IPasteButtonRole> buttons = null)
        {
            var _attributes = new List<VoloModelAttribute>();
            if (attributes != null)
            {
                foreach (var _attribute in attributes)
                {
                    var _attri = new VoloModelAttribute();
                    if (_attribute is DescriptionAttribute descriptionAttribute)
                    {
                        //var descriptionAttribute = (DescriptionAttribute)_attribute;
                        if (descriptionAttribute != null)
                        {
                            if (property != null)
                            {
                                var strs = descriptionAttribute.Description.Split(" ");
                                property.Title = strs[0].Trim().Replace(SplitChar, " ");
                                property.HasDescAttribute = true;
                                property.Placeholder = strs.Length > 1 ? strs[1].Replace(SplitChar, " ") : "";
                            }
                        }
                        if (model != null)
                        {
                            var strs = descriptionAttribute.Description.Split(" ");
                            model.Title = strs[0].Trim().Replace(SplitChar, " ");
                            model.Desc = strs.Length > 1 ? strs[1].Replace(SplitChar, " ") : "";
                        }
                        continue;
                    }

                    if (_attribute is MaxLengthAttribute maxAttribute)
                    {
                        //var _bute = (MaxLengthAttribute)_attribute;
                        _attri.Name = "maxlength";
                        _attri.Error = maxAttribute.ErrorMessage;
                        _attri.Args1 = maxAttribute.Length.ToString();
                        if (property != null)
                        {
                            property.Maxlength = maxAttribute.Length;
                        }
                    }
                    if (_attribute is RequiredAttribute requiredAttribute)
                    {
                        //var _bute = (RequiredAttribute)_attribute;
                        _attri.Name = "required";
                        if (property != null)
                        {
                            property.Required = true;
                        }
                        _attri.Error = requiredAttribute.ErrorMessage;
                    }

                    if (_attribute is RangeAttribute rangeAttribute)
                    {
                        //var _bute = (RangeAttribute)_attribute;
                        _attri.Name = "range";
                        _attri.Args1 = rangeAttribute.Minimum.ToString();
                        _attri.Args2 = rangeAttribute.Maximum.ToString();
                        _attri.Error = rangeAttribute.ErrorMessage;
                    }

                    if (_attribute is RegularExpressionAttribute regularAttribute)
                    {
                        //var _bute = (RegularExpressionAttribute)_attribute;
                        _attri.Name = "pattern";
                        _attri.Args1 = regularAttribute.Pattern.ToString();
                        _attri.Error = regularAttribute.ErrorMessage;
                    }

                    if (buttons != null)
                    {
                        if (_attribute is BtnCodeAttribute buttonAttribute)
                        {
                            if (!PrivateButtonAuth(buttons, dtoName, buttonAttribute.Code))
                            {
                                continue;
                            }
                        }
                        //如果是Hidden 有权限则隐藏 没权限则显示了... . .
                        if (_attribute is PasteShowAttribute showAttribute)
                        {
                            if (PrivateButtonAuth(buttons, dtoName, showAttribute.Code))
                            {
                                //这个特性不生效，也就是显示
                                continue;
                            }
                        }
                    }

                    if (String.IsNullOrEmpty(_attri.Name))
                    {
                        var _bute = _attribute as ColumnDataTypeAttribute;
                        if (_bute != null)
                        {
                            _attri.Name = _bute.Name;
                            _attri.Args1 = _bute.Args1;
                            _attri.Args2 = _bute.Args2;
                            _attri.Args3 = _bute.Args3;
                            _attri.Args4 = _bute.Args4;
                            _attri.Error = _bute.ErrorMessage;
                            _attri.Order = _bute.Order;
                            if (_bute.IsServerAttribute)
                            {
                                continue;
                            }
                        }
                    }
                    if (String.IsNullOrEmpty(_attri.Name))
                    {
                        var __name = _attribute.GetType().ToString();
                        if (__name.Contains("."))
                        {
                            var splits = __name.Split('.');
                            _attri.Name = splits[splits.Length - 1];
                        }
                        else
                        {
                            _attri.Name = __name;
                        }
                        _attri.Args1 = _attribute.ToString();
                    }
                    if (!String.IsNullOrEmpty(_attri.Name))
                    {
                        if (_attri.Name == "sort")
                        {
                            if (int.TryParse(_attri.Args1, out var _sort))
                            {
                                if (property != null)
                                {
                                    property.Sort = _sort;
                                }
                            }
                            continue;
                        }
                        if (_attri.Name == "orderby")
                        {
                            if (property != null)
                            {
                                if (String.IsNullOrEmpty(_attri.Args1))
                                {
                                    _attri.Args1 = property.Name;
                                }
                                if (String.IsNullOrEmpty(_attri.Args2))
                                {
                                    _attri.Args2 = $"{property.Name} Desc";
                                }
                            }
                        }
                        _attributes.Add(_attri);
                    }
                }

                if (!_attributes.Any(x => x.Name == "enable" || x.Name == "disable"))
                {
                    IEnumerable<PasteAuthAttribute> pasteAuthAttributes = attributes.OfType<PasteAuthAttribute>();
                    if (pasteAuthAttributes.Any())
                    {
                        var auths = pasteAuthAttributes.Where(x => String.IsNullOrEmpty(x.Model) && String.IsNullOrEmpty(x.Role)).ToList();
                        if (auths.Any())
                        {
                            var voloAttr = ExtendHelper.ToEnableAttribute(null, auths);
                            if (voloAttr != null)
                            {
                                _attributes.Add(voloAttr);
                            }
                        }
                    }
                }
            }
            return _attributes?.Any() == true ? _attributes.OrderBy(x => x.Order).ToList() : null;
        }

        /// <summary>
        /// 是否有权限 默认有权限
        /// </summary>
        /// <param name="buttons"></param>
        /// <param name="dtoName">示例UserInfoListDto</param>
        /// <param name="code"></param>
        /// <returns></returns>
        private static bool PrivateButtonAuth(IEnumerable<IPasteButtonRole> buttons, string dtoName, string code)
        {
            if (buttons != null && !String.IsNullOrEmpty(code))
            {
                if (!buttons.Any(x =>
                    (x.Path == dtoName && x.Code == code) ||      // 精确匹配
                    (x.Path == "*" && x.Code == code) ||          // 所有DTO的该按钮
                    (x.Path == dtoName && x.Code == "*") ||       // 该DTO的所有按钮
                    (x.Path == "*" && x.Code == "*")))            // 超级管理员：所有DTO的所有按钮
                {
                    //没有权限
                    return false;
                }
            }
            return true;
        }


        /// <summary>
        /// 首个单词小写模式
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string FirstLetterToLower(this string input)
        {
            if (!String.IsNullOrEmpty(input))
            {
                var _index = 0;
                for (var k = 0; k < input.Length; k++)
                {
                    var _char = input[k];
                    if ((int)_char >= 97)
                    {
                        _index = k - 1;
                        break;
                    }
                }
                if (_index > 0)
                {
                    return (input.Substring(0, _index).ToLower() + input.Substring(_index));
                }
                else
                {
                    return (input.Substring(0, 1).ToLower() + input.Substring(1));
                }
            }
            return input;
        }

        private static readonly IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());
        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchType"></param>
        /// <returns></returns>
        public static PropertyInfo[] ReadTypeProperties(Type searchType)
        {

            var typeName = searchType.FullName;
            // 使用GetOrCreate替代TryGetValue+Set
            //return _memoryCache.GetOrCreate(typeName, entry =>
            //{
            //    // 这里的逻辑只会被一个线程执行（其他线程等待）
            //    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(300); // 设置过期时间

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

            ////return t.GetProperties(); // 耗时的反射操作
            //var allProperties = searchType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //// 如果有属性被隐藏（new），才需要复杂处理
            //var hasDuplicates = allProperties.Length != allProperties.Select(p => p.Name).Distinct().Count();
            //if (!hasDuplicates)
            //{
            //    // 没有重复属性名，直接返回
            //    return allProperties;
            //}
            //// 有重复属性名，进行分组处理
            //var propertyGroups = allProperties.GroupBy(p => p.Name);
            //var result = new PropertyInfo[propertyGroups.Count()];
            //int index = 0;
            //foreach (var group in propertyGroups)
            //{
            //    // 手动查找，避免 LINQ 的开销
            //    PropertyInfo selected = null;
            //    foreach (var prop in group)
            //    {
            //        if (prop.DeclaringType == searchType)
            //        {
            //            selected = prop;
            //            break;  // 找到当前类声明的，立即退出
            //        }
            //        selected ??= prop;  // 否则保留第一个
            //    }
            //    result[index++] = selected!;
            //}
            //return result;


            //});

        }


        #region 脱敏处理

        /// <summary>
        /// 对数据进行脱敏处理
        /// </summary>
        /// <param name="data">要处理的数据（可以是object或List）</param>
        /// <param name="targetType">目标类型</param>
        /// <returns>脱敏后的数据</returns>
        public static object PasteMaskData(object data, Type targetType)
        {
            if (data == null || targetType == null)
            {
                return data;
            }
            // 检查类型是否有 [PasteHalfHide] 特性
            var classAttribute = targetType.GetCustomAttribute<PasteHalfHideAttribute>();

            // 如果类型没有脱敏特性，直接返回原数据
            if (classAttribute == null)
            {
                return data;
            }
            var properties = ReadTypeProperties(targetType);// targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
            if (classAttribute.CreateNewObj)
            {
                if (data is IEnumerable list)
                {
                    //var listType = list.GetType().GetGenericArguments()[0];
                    var resultList = new List<object>();
                    foreach (var item in list)
                    {
                        if (item.GetType() != targetType)
                        {
                            resultList.Add(item);
                            continue;
                        }
                        var maskedItem = MaskNewSingleObject(item, targetType, properties);
                        resultList.Add(maskedItem);
                    }
                    return resultList;
                }
            }
            else
            {
                if (data is IEnumerable list)
                {
                    foreach (var item in list)
                    {
                        MaskSingleObject(item, targetType, properties);
                    }
                }
            }

            if (classAttribute.CreateNewObj)
            {
                // 处理单个对象
                return MaskNewSingleObject(data, targetType, properties);
            }
            else
            {
                MaskSingleObject(data, targetType, properties);
                return data;
            }
        }

        /// <summary>
        /// 原对象上处理
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static void MaskSingleObject(object obj, Type type, PropertyInfo[] properties)
        {
            if (obj == null)
                return;

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<PasteHalfHideAttribute>();
                if (attr == null)
                    continue;

                var originalValue = property.GetValue(obj)?.ToString();
                if (string.IsNullOrEmpty(originalValue))
                    continue;

                var maskedValue = Desensitize(originalValue, attr);
                property.SetValue(obj, maskedValue);  // 直接修改原对象
            }

            //return obj;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="type"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        private static object MaskNewSingleObject(object obj, Type type, PropertyInfo[] properties)
        {
            if (obj == null)
                return null;

            // 创建对象副本
            var result = Activator.CreateInstance(type);
            //var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);

            foreach (var property in properties)
            {
                // 检查属性是否有 [PasteHalfHide] 特性
                var attr = property.GetCustomAttribute<PasteHalfHideAttribute>();

                if (attr == null)
                {
                    // 没有特性，直接复制值
                    var value = property.GetValue(obj);
                    property.SetValue(result, value);
                    continue;
                }

                // 获取属性值
                var originalValue = property.GetValue(obj)?.ToString();
                if (string.IsNullOrEmpty(originalValue))
                {
                    property.SetValue(result, originalValue);
                    continue;
                }

                // 进行脱敏处理
                var maskedValue = Desensitize(originalValue, attr);
                property.SetValue(result, maskedValue);
            }

            return result;
        }

        /// <summary>
        /// 执行脱敏
        /// </summary>
        private static string Desensitize(string value, PasteHalfHideAttribute attr)
        {
            if (string.IsNullOrEmpty(value))
            {
                return value;
            }

            return DesensitizeCustom(value, attr.StartLength, attr.EndLength, attr.MaskChar);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="startLength"></param>
        /// <param name="endLength"></param>
        /// <param name="maskChar"></param>
        /// <returns></returns>
        private static string DesensitizeCustom(string value, int startLength, int endLength, char maskChar)
        {
            if (string.IsNullOrEmpty(value)) return value;
            if (value.Length <= startLength + endLength) return value;

            return value.Substring(0, startLength) +
                   new string(maskChar, value.Length - startLength - endLength) +
                   value.Substring(value.Length - endLength);
        }

        #endregion

    }
}
