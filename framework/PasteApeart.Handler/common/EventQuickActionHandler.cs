using Microsoft.Extensions.Logging;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler.Extensions;
using PasteApeart.Helper;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;

namespace PasteApeart.Handler.common
{
    /// <summary>
    /// EventHanlder直接执行的帮助类
    /// </summary>
    public class EventQuickActionHandler : IEventQuickActionHandler
    {
        private readonly ILogger<EventQuickActionHandler> _logger;
        private readonly IEventActionHandler _actionHandler;
        private readonly Type[] _dtoTypeArray;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="actionHandler"></param>
        /// <param name="dtoTypeArray">EventModel的模型集合</param>
        public EventQuickActionHandler(ILogger<EventQuickActionHandler> logger, IEventActionHandler actionHandler, Type[] dtoTypeArray = null)
        {
            _logger = logger;
            _actionHandler = actionHandler;
            if (dtoTypeArray != null)
            {
                _dtoTypeArray = dtoTypeArray;
            }
        }

        /// <summary>
        /// 读取AddDto的数据模型
        /// </summary>
        /// <returns></returns>
        public VoloModelInfo ReadAddModel(string model_name)
        {
            if (string.IsNullOrEmpty(model_name))
            {
                throw new PasteArgsException(nameof(model_name));// PasteCodeException("");
            }
            if (_dtoTypeArray?.Any() != true)
            {
                throw new PasteArgsException("构造函数中需要先引入EventModel集合");
            }
            //AbsAutoFundEventModel AbsEventModel
            //var typeArray = typeof(UserInfoDto).Assembly.GetTypes();
            var find = _dtoTypeArray.FirstOrDefault(x => x.Name == model_name);
            if (find == null)
            {
                throw new PasteCodeException($"没有找到这样的对象{model_name}");
            }
            if (!typeof(IEventModel).IsAssignableFrom(find))
            {
                throw new PasteCodeException("对象必须继承于IEventModel");
            }

            var _model = BuilderHelper.DynamicReadTypeProperty(find, false);

            foreach (var item in _model.Properties)
            {
                if (string.Equals(item.Name, "eventName", StringComparison.OrdinalIgnoreCase))
                {
                    item.Value = model_name;
                }

                if (item.DataType == "String")
                {
                    if (item.Attributes == null)
                    {
                        item.Attributes = new List<VoloModelAttribute> { new VoloModelAttribute { Name = "text" } };
                    }
                    else
                    {
                        if (!item.Attributes.Any(x => x.Name == "maxlength"))
                        {
                            item.Attributes.Add(new VoloModelAttribute { Name = "text" });
                        }
                    }
                }
            }
            return _model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<PastePagedResult<QuickHandlerPropertyInfo>> PageAsync(InputSearchBase input)
        {
            var result = new PastePagedResult<QuickHandlerPropertyInfo>();

            //var typeArray = typeof(UserInfoDto).Assembly.GetTypes();

            if (_dtoTypeArray?.Any() != true)
            {
                throw new PasteArgsException("构造函数中需要先引入EventModel集合");
            }

            // 获取所有继承于 IEventModel 的非抽象类
            var eventModelTypes = _dtoTypeArray
                .Where(t => typeof(IEventModel).IsAssignableFrom(t)
                            && t.IsClass
                            && !t.IsAbstract)
                .PasteWhereIf(!string.IsNullOrEmpty(input.word), x => x.Name.Contains(input.word))
                .ToList();

            if (eventModelTypes?.Any() != true)
            {
                return null;
            }
            result.TotalCount = eventModelTypes.Count;
            var datas = eventModelTypes.OrderBy(x => x.Name).Skip(input.size * (input.page - 1)).Take(input.size).ToList();

            var modelList = new List<QuickHandlerPropertyInfo>();

            foreach (var type in datas)
            {
                // 获取类上的 XML 注释
                var xmlComment = GetXmlComment(type);

                var code = type.Name;                    // 类名作为 Code
                var name = ExtractName(xmlComment);      // summary 中空格之前的部分
                var desc = ExtractDesc(xmlComment);      // summary 中空格之后的部分

                // 字段个数（这里按属性数量统计，如需包含字段可自行调整）
                var fieldCount = type.GetProperties(BindingFlags.Public | BindingFlags.Instance).Length;
                // 若需要同时统计字段：

                modelList.Add(new QuickHandlerPropertyInfo
                {
                    Code = code,
                    Name = name,
                    Desc = desc,
                    FieldCount = fieldCount,
                    //Type = type
                });
            }
            result.Items = modelList; // 视 VoloModelInfo 的实际结构赋值
            return result;
        }

        /// <summary>
        /// 读取类型上的 XML Summary 注释
        /// </summary>
        private string GetXmlComment(Type type)
        {
            try
            {
                var member = type.Assembly.GetType(type.FullName ?? type.Name);
                if (member == null)
                {
                    _logger.LogWarning($"没有找到这样的类型：{type.FullName} {type.Name}");
                    return string.Empty;
                }

                // 使用反射读取 XML 文档（需要确保 XML 文件已生成并加载）
                var xmlDoc = GetXmlDocument(type.Assembly);
                if (xmlDoc == null)
                {
                    _logger.LogWarning($"没有读取到文档：{type.Assembly.GetName().Name}.xml");
                    return string.Empty;
                }

                var memberName = $"T:{type.FullName}";
                var node = xmlDoc.SelectSingleNode($"/doc/members/member[@name='{memberName}']/summary");
                if (node == null)
                {
                    _logger.LogWarning($"没有找到这个节点 {memberName}/summary");
                }
                return node?.InnerText?.Trim() ?? string.Empty;
            }
            catch (Exception exl)
            {
                _logger.LogException(exl);
                return string.Empty;
            }
        }

        /// <summary>
        /// 缓存复用
        /// </summary>
        private static readonly Dictionary<string, XmlDocument> _xmlDocCache = new();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="assembly"></param>
        /// <returns></returns>
        private XmlDocument GetXmlDocument(Assembly assembly)
        {
            var location = assembly.Location;
            if (_xmlDocCache.TryGetValue(location, out var cached))
                return cached;

            try
            {

                var xmlPath = Path.ChangeExtension(location, ".xml");
                if (!System.IO.File.Exists(xmlPath))
                {
                    _logger.LogWarning($"找不到文件 {xmlPath}");
                    return null;
                }

                var doc = new XmlDocument();
                doc.Load(xmlPath);
                _xmlDocCache[location] = doc;
                return doc;
            }
            catch (Exception exl)
            {
                _logger.LogException(exl);
                return null;
            }
        }

        /// <summary>
        /// 提取 summary 中第一个空格之前的内容作为 Name
        /// </summary>
        private string ExtractName(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return string.Empty;

            // 去掉换行、多余空白
            comment = Regex.Replace(comment, @"\s+", " ").Trim();
            var index = comment.IndexOf(' ');
            return index > 0 ? comment.Substring(0, index) : comment;
        }

        /// <summary>
        /// 提取 summary 中空格之后的内容作为 Desc
        /// </summary>
        private string ExtractDesc(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment)) return string.Empty;

            comment = Regex.Replace(comment, @"\s+", " ").Trim();
            var index = comment.IndexOf(' ');
            return index > 0 && index + 1 < comment.Length
                ? comment.Substring(index + 1).Trim()
                : string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="postBodyStr"></param>
        /// <returns></returns>
        public async Task<string> CreateItemAsync(string postBodyStr)
        {
            await _actionHandler.ActionMessageAsync(postBodyStr);
            return "执行完成";
        }
    }
}
