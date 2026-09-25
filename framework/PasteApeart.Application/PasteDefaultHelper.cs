using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;

namespace PasteApeart.Application
{
    /// <summary>
    /// 通用接口帮助类
    /// </summary>
    public class PasteDefaultHelper : IPasteDefaultHelper
    {
        /// <summary>
        /// 
        /// </summary>
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        ///  typeof(InputSearchBase).Assembly.GetTypes();
        /// </summary>
        private readonly Type[] _dtoTypes;

        /// <summary>
        /// typeof(RoleInfo).Assembly.GetTypes();
        /// </summary>
        private readonly Type[] _entityTypes;

        /// <summary>
        /// LinqExtend的Type
        /// </summary>
        private readonly Type _linqExtendType;

        ///// <summary>
        ///// <![CDATA[services.AddSingleton(sp => new PasteApeartDefaultHelper(sp.GetRequiredService<IMemoryCache>(),() => typeof(RoleInfo).Assembly.GetTypes()))]]>
        ///// </summary>
        ///// <param name="memoryCache"></param>
        ///// <param name="typeDomainScanner"></param>
        ///// <param name="typeDtoScanner"></param>
        ///// <param name="extendType"></param>
        //public PasteDefaultHelper(IMemoryCache memoryCache, Func<Type[]> typeDomainScanner, Func<Type[]> typeDtoScanner, Type extendType)
        //{
        //    _memoryCache = memoryCache;
        //    _domainTypeScanner = typeDomainScanner;
        //    _dtoTypeScanner = typeDtoScanner;
        //    _linqExtendType = extendType;
        //}

        /// <summary>
        /// <![CDATA[services.AddSingleton(sp => new PasteApeartDefaultHelper(sp.GetRequiredService<IMemoryCache>(),() => typeof(RoleInfo).Assembly.GetTypes()))]]>
        /// </summary>
        /// <param name="memoryCache"></param>
        /// <param name="typeDomainScanner"></param>
        /// <param name="typeDtoScanner"></param>
        /// <param name="extendType"></param>
        public PasteDefaultHelper(IMemoryCache memoryCache, Type[] typeDomainScanner, Type[] typeDtoScanner, Type extendType)
        {
            _memoryCache = memoryCache;
            _entityTypes = typeDomainScanner;
            _dtoTypes = typeDtoScanner;
            _linqExtendType = extendType;
        }

        /// <summary>
        /// 等效于 ReadTypeProperties
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        private PropertyInfo[] MemoryProperties(Type t)
        {
            return ReadTypeProperties(t);
        }

        /// <summary>
        /// 获取某一个字段的PropertyInfo信息 可能返回null
        /// </summary>
        /// <param name="t"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private PropertyInfo MemoryPropertyInfo(Type t, string propertyName)
        {
            var types = MemoryProperties(t);
            if (types != null)
            {
                return types.Where(x => x.Name == propertyName).FirstOrDefault();
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        private Type FindDomainTypeThrowException(string entityName)
        {
            var type = FindOnlyDomainType(entityName);
            if (type == null)
            {
                throw new PasteCodeException($"没有在项目Domain中找到:{entityName} ");
            }
            return type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        private Type FindOnlyDomainType(string entityName)
        {
            var domainTypeKey = $"domain.type.name.{entityName}";
            return _memoryCache.GetOrCreate(domainTypeKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(500);
                var types = ListDomainTypes();
                return types?.FirstOrDefault(t =>
                    string.Equals(t.Name, entityName, StringComparison.OrdinalIgnoreCase)
                );
            });

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        private Type FindDtoTypeThrowException(string entityName)
        {
            var type = FindOnlyDtoType(entityName);
            if (type == null)
            {
                throw new PasteCodeException($"没有在程序集:{nameof(InputSearchBase)} 中找到:{entityName} ");
            }
            return type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtoName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        private Type FindOnlyDtoType(string dtoName)
        {
            if (dtoName == nameof(InputSearchBase))
            {
                return typeof(InputSearchBase);
            }
            var domainTypeKey = $"dto.type.name.{dtoName}";
            return _memoryCache.GetOrCreate(domainTypeKey, entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(500);
                var types = ListDtoTypes();
                return types?.FirstOrDefault(t =>
                    string.Equals(t.Name, dtoName, StringComparison.OrdinalIgnoreCase)
                );
            });
        }



        /// <summary>
        /// 不要调用
        /// </summary>
        /// <returns></returns>
        private Type[] ListDomainTypes()
        {
            //return _memoryCache.GetOrCreate("domain.types", entry =>
            //{
            //    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(450);
            //    //return typeof(RoleInfo).Assembly.GetTypes(); // 耗时的程序集扫描
            //    return _entityTypes();
            //});
            return _entityTypes;
        }



        /// <summary>
        /// 不要调用
        /// </summary>
        /// <returns></returns>
        private Type[] ListDtoTypes()
        {
            //return _memoryCache.GetOrCreate("dto.types", entry =>
            //{
            //    entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(450);
            //    //return typeof(InputSearchBase).Assembly.GetTypes(); // 耗时的程序集扫描
            //    return _dtoTypes();
            //});
            return _dtoTypes;
        }

        /// <summary>
        /// 扩展 ExtendBind的赋值 查询某一个对象，有多少绑定，比如给角色绑定权限，给用户绑定角色
        /// </summary>
        /// <param name="listDtoType"></param>
        /// <param name="_dbContext"></param>
        /// <param name="dataList"></param>
        /// <param name="searchType"></param>
        /// <param name="searchModel"></param>
        public async Task ReadExtendBind(DbContext _dbContext, Type listDtoType, List<object> dataList, Type searchType, object searchModel)
        {
            var extendBindProperty = MemoryPropertyInfo(listDtoType, "ExtendBind");// listDtoType.GetProperty("ExtendBind");
            if (extendBindProperty == null)
            {
                return;
            }
            //var find_extend_bind = searchType.GetProperties().Where(x=>x.Attributes)
            var properties = MemoryProperties(searchType).Where(p => p.GetCustomAttributes(typeof(PasteExtendBindAttribute), true).Length > 0).ToList();
            if (properties?.Any() == false)
            {
                //Logger.LogError($"没有在:{searchType.Name} 找到具有PasteExtendBindAttribute的字段");
                return;
            }

            foreach (var extendProperty in properties)
            {
                //UserId  GradeId
                var extendGradeId = extendProperty.GetValue(searchModel);
                if (extendGradeId == null)
                {
                    //Logger.LogWarning($"{searchType.Name}.{extendProperty.Name}的值为null，不需要查询");
                    continue;
                }

                var gradeIdString = extendGradeId.ToString();
                if (gradeIdString == "0" || String.IsNullOrEmpty(gradeIdString))
                {
                    //Logger.LogWarning($"{searchType.Name}.{extendProperty.Name}的值为{gradeIdString}，不需要查询");
                    continue;
                }

                var gradeIdValue = Convert.ChangeType(gradeIdString, extendProperty.PropertyType);

                var attr = extendProperty.GetCustomAttribute<PasteExtendBindAttribute>();

                var bindEntityType = FindEntityType(attr.Args1);

                var dbSet = FindDbSetObject(_dbContext, bindEntityType);

                //var dbSet = dbSetProperty.GetValue(_dbContext);
                var whereMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(dbSet.GetType().GenericTypeArguments[0]);

                // 创建 lambda 参数和表达式
                var parameter = Expression.Parameter(dbSet.GetType().GenericTypeArguments[0], "x");
                var property = Expression.Property(parameter, attr.Args3);
                var constant = Expression.Constant(gradeIdValue);
                var equal = Expression.Equal(property, constant);
                var lambda = Expression.Lambda(equal, parameter);

                // 执行 Where 方法
                var filteredQuery = whereMethod.Invoke(null, new object[] { dbSet, lambda });

                var roleIdPropertyType = MemoryPropertyInfo(bindEntityType, attr.Args2).PropertyType;// bindEntityType.GetProperty(attr.Args2).PropertyType;
                                                                                                     // 执行 Select 方法获取 RoleId
                var selectMethod = typeof(Queryable).GetMethods()
                    .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(
                        dbSet.GetType().GenericTypeArguments[0],
                        roleIdPropertyType);

                // 创建 Select lambda
                var selectParameter = Expression.Parameter(dbSet.GetType().GenericTypeArguments[0], "x");
                var selectProperty = Expression.Property(selectParameter, attr.Args2);
                var selectLambda = Expression.Lambda(selectProperty, selectParameter);

                var projectedQuery = selectMethod.Invoke(null, new object[] { filteredQuery, selectLambda });
                //var bindIdProperty = bindEntityType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);
                var bindIdProperty = ReadCurrentPropertyThrowException(bindEntityType, attr.Args2);
                var extensionsType = typeof(EntityFrameworkQueryableExtensions);
                // 查找 ToListAsync 方法，匹配名称、泛型和参数
                var toListMethod = extensionsType.GetMethods(BindingFlags.Static | BindingFlags.Public)
                    .First(m =>
                        m.Name == "ToArrayAsync" &&
                        m.IsGenericMethodDefinition &&
                        m.GetParameters().Length == 2 &&  // 注意：有两个参数（查询和 CancellationToken）
                        m.GetParameters()[0].ParameterType == typeof(IQueryable<>).MakeGenericType(m.GetGenericArguments()[0]) &&
                        m.GetParameters()[1].ParameterType == typeof(CancellationToken));

                // 构建泛型方法
                var genericToListMethod = toListMethod.MakeGenericMethod(bindIdProperty.PropertyType);
                // 调用方法，传入 CancellationToken.None
                var task = (Task)genericToListMethod.Invoke(null, new object[] { projectedQuery, CancellationToken.None });
                await task;

                // 获取结果
                var resultProperty = task.GetType().GetProperty("Result");
                var entities = (IEnumerable)resultProperty.GetValue(task);
                var binds = entities.Cast<object>().ToList();

                if (binds != null && binds.Count > 0)
                {
                    var idProperty = ReadCurrentProperty(listDtoType, "Id");
                    if (idProperty != null && extendBindProperty != null &&
                        extendBindProperty.PropertyType == typeof(bool))
                    {
                        var ids = new List<string>();//转string因为Id可能是guid int long string 等
                        foreach (var bind in binds)
                        {
                            ids.Add(bind.ToString());
                        }
                        foreach (var item in dataList)
                        {
                            var idValue = idProperty.GetValue(item);
                            if (idValue != null)
                            {
                                if (ids.Contains(idValue.ToString()))
                                {
                                    extendBindProperty.SetValue(item, true);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 处理映射对象 搜索的信息检索
        /// 处理基础查询page size word等信息，如果有其他的则进行转化，由queryString转化成SearchObject
        /// </summary>
        /// <param name="_search"></param>
        /// <param name="searchType"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        public InputSearchBase MapperToQueryModel(string _search, Type searchType, object search)
        {

            var info = new InputSearchBase();

            if (!String.IsNullOrEmpty(_search))
            {
                if (_search.StartsWith("?"))
                {
                    _search = _search.Substring(1);
                }
                if (!String.IsNullOrEmpty(_search))
                {
                    var _dic = new Dictionary<string, object>();
                    var splits = _search.Split('&');
                    foreach (var _one in splits)
                    {
                        if (_one.Contains("="))
                        {
                            var ones = _one.Split('=');
                            var _key = ones[0];
                            var _val = ones[1];
                            if (String.IsNullOrEmpty(_val))
                            {
                                continue;
                            }
                            if (_val.Length > 1)
                            {
                                _val = HttpUtility.UrlDecode(_val);//转义 解码
                            }
                            if (!_dic.ContainsKey(_key))
                            {
                                _dic.Add(_key, _val);
                            }
                            else
                            {
                                var old_val = _dic[_key];
                                if (old_val.GetType() == typeof(List<object>))
                                {
                                    ((List<object>)old_val).Add(_val);
                                }
                                else
                                {
                                    var objs = new List<object> { old_val };
                                    objs.Add(_val);
                                    _dic[_key] = objs;
                                }
                            }
                        }
                    }
                    var pros = MemoryProperties(searchType);// searchType.GetProperties();
                    foreach (var key in _dic.Keys)
                    {
                        var val = _dic[key];
                        if (String.IsNullOrEmpty(val.ToString()))
                        {
                            continue;
                        }
                        var pro = pros.Where(x => String.Equals(x.Name, key, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (pro != null)
                        {
                            if (pro.PropertyType == typeof(bool))
                            {
                                switch (val.ToString())
                                {
                                    case "1":
                                    case "true":
                                    case "True":
                                    case "on":
                                        {
                                            pro.SetValue(search, true);
                                        }
                                        break;
                                    default:
                                        pro.SetValue(search, false);
                                        break;
                                }
                                continue;
                            }
                            if (pro.PropertyType == typeof(DateTime))
                            {
                                if (DateTime.TryParse(val.ToString(), out var _dt))
                                {
                                    pro.SetValue(search, _dt);
                                }
                                continue;
                            }
                            if (pro.PropertyType == typeof(DateTime?))
                            {
                                if (DateTime.TryParse(val.ToString(), out var _dt))
                                {
                                    pro.SetValue(search, _dt);
                                }
                                continue;
                            }
                            // 处理枚举数组类型
                            if (pro.PropertyType.IsArray)
                            {
                                if (pro.PropertyType.GetElementType().IsEnum)
                                {
                                    var elementType = pro.PropertyType.GetElementType();
                                    if (val is List<object> listVal)
                                    {
                                        var enumArray = Array.CreateInstance(elementType, listVal.Count);
                                        for (int i = 0; i < listVal.Count; i++)
                                        {
                                            try
                                            {
                                                var enumValue = Enum.Parse(elementType, listVal[i].ToString(), true);
                                                enumArray.SetValue(enumValue, i);
                                            }
                                            catch
                                            {
                                                if (int.TryParse(listVal[i].ToString(), out int numericValue))
                                                {
                                                    enumArray.SetValue(Enum.ToObject(elementType, numericValue), i);
                                                }
                                            }
                                        }
                                        pro.SetValue(search, enumArray);
                                    }
                                    else
                                    {
                                        var enumArray = Array.CreateInstance(elementType, 1);
                                        try
                                        {
                                            var enumValue = Enum.Parse(elementType, val.ToString(), true);
                                            enumArray.SetValue(enumValue, 0);
                                        }
                                        catch
                                        {
                                            if (int.TryParse(val.ToString(), out int numericValue))
                                            {
                                                enumArray.SetValue(Enum.ToObject(elementType, numericValue), 0);
                                            }
                                        }
                                        pro.SetValue(search, enumArray);
                                    }
                                    continue;
                                }
                                else
                                {
                                    //Console.WriteLine($"IsArray {pro.Name} {pro.PropertyType.Name} {pro.PropertyType.GetElementType().Name}");
                                    var elementType = pro.PropertyType.GetElementType();
                                    if (val is List<object> listVal)
                                    {
                                        var enumArray = Array.CreateInstance(elementType, listVal.Count);
                                        for (int i = 0; i < listVal.Count; i++)
                                        {
                                            var enumValue = Convert.ChangeType(listVal[i].ToString(), elementType);
                                            enumArray.SetValue(enumValue, i);
                                        }
                                        pro.SetValue(search, enumArray);
                                    }
                                    else
                                    {
                                        var enumArray = Array.CreateInstance(elementType, 1);

                                        var enumValue = Convert.ChangeType(val.ToString(), elementType);
                                        enumArray.SetValue(enumValue, 0);

                                        pro.SetValue(search, enumArray);
                                    }
                                    continue;
                                }
                            }

                            // 处理普通枚举类型
                            if (pro.PropertyType.IsEnum)
                            {
                                var enumValue = Enum.Parse(pro.PropertyType, val.ToString(), true);
                                pro.SetValue(search, enumValue);
                                continue;
                            }
                            //其他类型默认转化
                            var changeValue = Convert.ChangeType(val.ToString(), pro.PropertyType);
                            pro.SetValue(search, changeValue);
                        }
                    }
                }
            }

            //page字段恒为int类型
            var pro_page = ReadCurrentProperty(searchType, "page");
            if (pro_page != null)
            {
                var page = pro_page.GetValue(search, null);
                info.page = (int)page;
            }

            var pro_size = ReadCurrentProperty(searchType, "size");
            if (pro_size != null)
            {
                var page = pro_size.GetValue(search, null);
                info.size = (int)page;
            }

            var pro_word = ReadCurrentProperty(searchType, "word");
            if (pro_word != null)
            {
                var word = pro_page.GetValue(search, null);
                if (word != null)
                {
                    if (!String.IsNullOrEmpty(word.ToString()))
                    {
                        info.word = word.ToString();
                    }
                }
            }

            return info;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="dto"></param>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        public async Task BuildOuterQuery(DbContext _dbContext, object dto, Type dtoType)
        {
            #region 看看是否要执行外表查询
            foreach (var aty in MemoryProperties(dtoType))
            {
                //var _type = aty.PropertyType;
                var attributes = aty.GetCustomAttributes<PasteShortAttribute>();
                if (attributes?.Any() == true)
                {
                    var shortAttr = attributes.FirstOrDefault();
                    if (shortAttr != null && shortAttr != default)
                    {
                        var foreignKeyId = aty.GetValue(dto);
                        if (foreignKeyId == null || foreignKeyId.Equals(0))
                        {
                            continue;
                        }
                        //var shortAttr = short_attr;
                        //var pro_val =//??? 当前字段的值
                        //short_attr.Args1 = "GradeInfo";//表示对应的外表的EntityName
                        //short_attr.Args2 = "ExtendGrade";//表示结果写入到这个字段
                        //short_attr.Args3 = "ToShortUser()";//表示要转换的类型 比如_dbContext.GradeInfo.Where(x=>x.Id==pro_val).Select(x=>x.ToShortUser()).FirstOrDefaultAsync();
                        //如何获取字段的当前值?
                        //await _dbContext.GradeInfo.Where(x => x.Id == 1).Select(y => y.ToShortGrade()).FirstOrDefaultAsync();
                        //ToShortGrade()是GradeInfo的扩展，返回的就是ExtendGrade的类型

                        // 获取外表的Entity类型和目标属性
                        var relatedEntityType = FindEntityType(shortAttr.Args1);
                        var targetProperty = ReadCurrentProperty(dtoType, shortAttr.Args2);

                        if (relatedEntityType == null || targetProperty == null)
                        {
                            continue;
                        }
                        //try
                        //{
                        var dbSet = FindDbSetObject(_dbContext, relatedEntityType);
                        // 动态调用FindAsync方法获取关联实体
                        var findAsyncMethod = dbSet.GetType()
                            .GetMethod("FindAsync", new[] { typeof(object[]) });
                        // 执行查询
                        var valueTask = findAsyncMethod.Invoke(dbSet, new object[] { new object[] { foreignKeyId } });
                        // 获取ValueTask的结果
                        object relatedEntity;
                        // 检查是否是ValueTask
                        if (valueTask.GetType().Name.StartsWith("ValueTask"))
                        {
                            // 手动获取ValueTask的结果
                            var getAwaiterMethod = valueTask.GetType().GetMethod("GetAwaiter");
                            var awaiter = getAwaiterMethod.Invoke(valueTask, null);

                            var getResultMethod = awaiter.GetType().GetMethod("GetResult");
                            relatedEntity = getResultMethod.Invoke(awaiter, null);
                        }
                        else
                        {
                            // 普通Task，直接等待
                            await (Task)valueTask;
                            relatedEntity = ((Task<object>)valueTask).Result;
                        }

                        if (relatedEntity != null)
                        {
                            // 处理转换逻辑
                            object convertedResult = null;

                            // 检查是否需要特殊转换
                            if (!string.IsNullOrEmpty(shortAttr.Args3))
                            {
                                var extend = _linqExtendType.GetMethod(shortAttr.Args3.Replace("()", ""), new[] { relatedEntityType });
                                // 处理特殊转换方法（例如：ToShortUser()）
                                if (extend != null)
                                {
                                    //convertedResult = ConvertEntity(relatedEntity, relatedEntityType, shortAttr.Args3);
                                    convertedResult = extend.Invoke(null, new object[] { relatedEntity });
                                }
                                else
                                {
                                    convertedResult = relatedEntity;
                                    Serilog.Log.Warning($"没有找到{shortAttr.Args1} 的扩展:{shortAttr.Args3} !");
                                    continue;
                                }
                            }
                            else
                            {
                                // 默认转换：直接使用目标属性类型
                                convertedResult = relatedEntity;
                            }

                            // 设置目标属性的值
                            targetProperty.SetValue(dto, convertedResult);
                        }
                    }
                }
            }

            #endregion
        }

        /// <summary>
        /// 查询某一个数据对象 用于创建或者更新后返回ListDto使用
        /// 这个一般是给系统的其他AppService使用，比如UserInfo的创建后调用
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<object> ReadSingleListDtoByIdAsync(DbContext _dbContext, string entityName, object id)
        {

            // 1. 根据实体名称查找对应的实体类型和 DTO 类型
            var entityType = FindEntityType(entityName);
            var dtoType = FindDtoType(entityName, "ListDto");

            if (entityType == null || dtoType == null)
            {
                throw new PasteCodeException($"找不到对应的实体或 DTO 类型: {entityName}");
            }

            // 2. 动态查询实体
            var entity = await FindEntityByIdAsync(_dbContext, entityType, id);
            if (entity == null)
            {
                return null;//throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            }
            // 3. 动态映射实体到 DTO
            //var dto = MapEntityToDto(null, entity, entityType, dtoType);
            var dto = Activator.CreateInstance(dtoType);
            MapProperties(entity, dto, entityType, dtoType);

            //构建外表等 Include的如何搞定？？？
            await BuildOuterQuery(_dbContext, dto, dtoType);
            return dto;

        }


        /// <summary>
        /// 查找Entity类型，和RoleInfo放一起
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public Type FindEntityType(string entityName)
        {
            return FindDomainTypeThrowException(entityName);
        }

        /// <summary>
        /// 查找Dto的类型
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        public Type FindDtoType(string entityName, string suffix)
        {
            //var pascalCaseName = ConvertToPascalCase(entityName);
            var dtoName = $"{entityName}{suffix}";
            return this.FindDtoTypeThrowException(dtoName);
        }

        /// <summary>
        /// 基于名称大小写查找字段
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public PropertyInfo GetPropertyThrowException(Type entityType, string propertyName)
        {
            //var pauseName = ConvertToPascalCase(propertyName);
            var property = MemoryProperties(entityType).Where(x => string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (property == null || property == default)
            {
                throw new PasteCodeException("没有找到需要变更的字段!");
            }
            return property;
        }

        /// <summary>
        /// 从Domain或者Dto中查找类型
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public Type AutoFindType(string typeName)
        {
            if (typeName == nameof(InputSearchBase))
            {
                return typeof(InputSearchBase);
            }
            var find = FindOnlyDtoType(typeName);
            if (find != null)
            {
                return find;
            }
            find = FindOnlyDomainType(typeName);
            return find;
        }

        /// <summary>
        /// 获取InputQueryXXX的类型
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public Type FindQueryType(string entityName)
        {
            return AutoFindType("InputQuery" + ConvertToPascalCase(entityName));
        }

        /// <summary>
        /// 基于Id查询第一个数据 如果要查询Include的使用FindEntityByIdWithIncludeAsync
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="id"></param>
        /// <param name="_dbContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<object> FindEntityByIdAsync(DbContext _dbContext, Type entityType, object id, CancellationToken cancellationToken = default)
        {

            var dbSet = FindDbSetObject(_dbContext, entityType);
            // 获取 FindAsync 方法
            var findAsyncMethod = dbSet.GetType()
                .GetMethod("FindAsync", new[] { typeof(object[]), typeof(CancellationToken) });
            //await _dbContext.UserInfo.FindAsync(new object[] { id,id });
            var idProperty = ReadCurrentPropertyThrowException(entityType, "Id");
            var idType = idProperty.PropertyType;
            object idobj = Convert.ChangeType(id.ToString(), idType);
            //如何把object的id转化成对应的字段类型？？？
            //这里是否要处理Include的情况
            // 调用方法
            var valueTask = findAsyncMethod.Invoke(dbSet, new object[] { new object[] { idobj }, cancellationToken });
            // 处理 ValueTask<T>
            if (valueTask is Task task)
            {
                await task;
                var resultProperty = task.GetType().GetProperty("Result");
                return resultProperty.GetValue(task);
            }
            else
            {
                // 手动处理 ValueTask<T>
                var getAwaiterMethod = valueTask.GetType().GetMethod("GetAwaiter");
                var awaiter = getAwaiterMethod.Invoke(valueTask, null);
                var getResultMethod = awaiter.GetType().GetMethod("GetResult");
                return getResultMethod.Invoke(awaiter, null);
            }
        }

        #region 包含Include的查询

        /// <summary>
        /// 基于Id查询第一个数据，包含Include,使用PasteIncludeAttribute标记 待测试
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<object> FindEntityByIdWithIncludeAsync(DbContext _dbContext, Type entityType, object id, CancellationToken cancellationToken = default)
        {
            // 获取 DbSet
            var dbSet = FindDbSetObject(_dbContext, entityType);

            // 构建基础查询
            var query = BuildQueryable(dbSet, entityType);

            // 自动包含所有标记了 PasteInclude 的导航属性
            query = ApplyAllPasteIncludes(query, entityType);

            // 构建过滤表达式并执行查询
            var whereExpression = BuildIdFilterExpression(entityType, id);
            var result = await ExecuteFirstAsync(query, entityType, whereExpression, cancellationToken);
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbSet"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private object BuildQueryable(object dbSet, Type entityType)
        {
            var asQueryableMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "AsQueryable" && m.IsGenericMethod)
                .MakeGenericMethod(entityType);

            return asQueryableMethod.Invoke(null, new[] { dbSet });
        }

        /// <summary>
        /// 判断实体类型是否有任何标记了 PasteIncludeAttribute 的属性
        /// </summary>
        /// <param name="entityType">实体类型</param>
        /// <returns>true 表示有 PasteInclude 属性，需要使用 Include 版本</returns>
        public bool HasPasteIncludeProperties(Type entityType)
        {
            //return entityType.GetProperties().Any(p => p.IsDefined(typeof(PasteIncludeAttribute), false));
            return HasAttributeType(entityType, typeof(PasteIncludeAttribute));
        }

        /// <summary>
        /// 判断实体类型是否有任何标记了 XXXAttribute 的属性
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="attributeType">typeof(PasteIncludeAttribute)</param>
        /// <returns></returns>
        public bool HasAttributeType(Type entityType, Type attributeType)
        {
            return entityType.GetProperties().Any(p => p.IsDefined(attributeType, false));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public object ApplyAllPasteIncludes(object query, Type entityType)
        {
            // 获取所有标记了 PasteInclude 特性的属性
            var pasteIncludeProperties = entityType.GetProperties()
                .Where(p => p.IsDefined(typeof(PasteIncludeAttribute), false))
                .ToList();
            foreach (var property in pasteIncludeProperties)
            {
                query = ApplyInclude(query, entityType, property.Name);
            }
            return query;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private object ApplyInclude(object query, Type entityType, string propertyName)
        {
            // 使用字符串版本的 Include 方法
            var includeMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "Include" &&
                           m.GetParameters().Length == 2 &&
                           m.GetParameters()[1].ParameterType == typeof(string))
                .MakeGenericMethod(entityType);

            return includeMethod.Invoke(null, new[] { query, propertyName });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private LambdaExpression BuildIdFilterExpression(Type entityType, object id)
        {
            var parameter = Expression.Parameter(entityType, "x");
            var idProperty = ReadCurrentPropertyThrowException(entityType, "Id");

            var idTyped = Convert.ChangeType(id, idProperty.PropertyType);
            var left = Expression.Property(parameter, idProperty);
            var right = Expression.Constant(idTyped, idProperty.PropertyType);
            var body = Expression.Equal(left, right);

            return Expression.Lambda(body, parameter);
        }

        private async Task<object> ExecuteFirstAsync(object query, Type entityType, LambdaExpression predicate, CancellationToken cancellationToken)
        {
            // 找到正确的 FirstAsync 重载（带 predicate 的版本有3个参数）
            var firstAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "FirstAsync" &&
                           m.GetParameters().Length == 3 &&  // 应该是3个参数：query, predicate, cancellationToken
                           m.GetParameters()[0].ParameterType.IsGenericType &&
                           m.GetParameters()[0].ParameterType.GetGenericTypeDefinition() == typeof(IQueryable<>))
                .MakeGenericMethod(entityType);

            // 调用方法
            var task = firstAsyncMethod.Invoke(null, new[] { query, predicate, cancellationToken });

            // 等待任务完成并获取结果
            await (Task)task;

            // 通过反射获取 Result 属性
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        private PropertyInfo FindDbSetProperty(DbContext _dbContext, Type entityType)
        {
            ////_dbContext.GetType().GetProperties()
            var dbSetProperty = MemoryProperties(_dbContext.GetType())
                .FirstOrDefault(p =>
                    p.PropertyType.IsGenericType &&
                    p.PropertyType.GetGenericTypeDefinition() == typeof(DbSet<>) &&
                    p.PropertyType.GetGenericArguments()[0] == entityType);

            if (dbSetProperty == null)
            {
                throw new PasteCodeException($"找不到实体 {entityType.Name} 的 DbSet");
            }
            return dbSetProperty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public object FindDbSetObject(DbContext _dbContext, Type entityName)
        {
            var dbSetProperty = FindDbSetProperty(_dbContext, entityName);
            return dbSetProperty.GetValue(_dbContext);
        }

        /// <summary>
        /// 基于PasteMutex排重 id!=id and name==name
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="dtoType"></param>
        /// <param name="dtoValue"></param>
        /// <param name="_dbContext"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<object> FilterFirstOrDefaultAsync(DbContext _dbContext, Type entityType, Type dtoType, object dtoValue, object id, CancellationToken cancellationToken = default)
        {
            var dbSet = FindDbSetObject(_dbContext, entityType);
            // 创建参数表达式 x
            var parameter = Expression.Parameter(entityType, "x");

            // 获取所有标记了 PasteMutexAttribute 的属性
            var properties = MemoryProperties(entityType)
                .Where(p => p.GetCustomAttributes(typeof(PasteMutexAttribute), true).Length > 0)
                .ToList();

            if (properties?.Any() == false)
            {
                return null;
            }

            //多个Mutex是否是And关系，默认为Or关系
            var boolBatch = false;
            //boolBatch = MemoryProperties(entityType).Where(p => p.GetCustomAttributes(typeof(PasteBatchMutexAttribute), true).Length > 0).Any();
            //2026-01-22 修正，应该是class的特性，而不是字段的
            boolBatch = entityType.GetCustomAttributes(typeof(PasteBatchMutexAttribute), true).Any();

            BinaryExpression condition = null;
            if (id != null)
            {
                // 创建初始条件表达式 x.Id == id
                var idProperty = ReadCurrentPropertyThrowException(entityType, "Id");
                var idPropertyAccess = Expression.Property(parameter, idProperty);
                var idValue = Convert.ChangeType(id, idProperty.PropertyType);
                var idConstant = Expression.Constant(idValue, idProperty.PropertyType);
                condition = Expression.NotEqual(idPropertyAccess, idConstant);
            }
            //非Id之外的表达式
            BinaryExpression binaryExpression = null;
            // 为每个标记了特性的属性添加条件
            foreach (var property in properties)
            {
                // 获取属性的默认值 (对于引用类型为 null，值类型为默认值)
                //object propertyValue = property.PropertyType.IsValueType
                //    ? Activator.CreateInstance(property.PropertyType)
                //    : null;

                var dtoProperty = ReadCurrentProperty(dtoType, property.Name);

                if (dtoProperty == null || dtoProperty == default)
                {
                    continue;
                }

                object propertyValue = dtoProperty.GetValue(dtoValue);

                // 创建属性访问表达式
                var propertyAccess = Expression.Property(parameter, property);

                // 创建常量表达式
                var constant = Expression.Constant(propertyValue, property.PropertyType);

                // 创建相等表达式
                var equalExpression = Expression.Equal(propertyAccess, constant);
                if (binaryExpression != null)
                {
                    if (boolBatch)
                    {
                        binaryExpression = Expression.AndAlso(binaryExpression, equalExpression);
                    }
                    else
                    {
                        binaryExpression = Expression.OrElse(binaryExpression, equalExpression);
                    }
                }
                else
                {
                    binaryExpression = equalExpression;
                }
            }

            //最终表达式
            BinaryExpression finalExpression = null;
            if (condition == null)
            {
                finalExpression = binaryExpression;
            }
            else
            {
                finalExpression = Expression.AndAlso(condition, binaryExpression);
            }

            // 创建 lambda 表达式
            var lambda = Expression.Lambda(finalExpression, parameter);

            // 获取 Where 方法
            var whereMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType);

            // 调用 Where 方法
            var whereQuery = whereMethod.Invoke(null, new object[] { dbSet, lambda });
            // 获取 FirstOrDefaultAsync 方法
            var firstOrDefaultAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "FirstOrDefaultAsync" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType);

            // 调用 FirstOrDefaultAsync 方法
            var task = firstOrDefaultAsyncMethod.Invoke(null, new object[] { whereQuery, cancellationToken }) as Task;
            await task;

            // 获取结果
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }

        /// <summary>
        /// 查找符合条件的第一个数据
        /// 一般用于新增，或者更新排重使用
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="args"></param>
        /// <param name="_dbContext"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<object> FindFirstOrDefaultAsync(DbContext _dbContext, Type entityType, Dictionary<string, string> args, CancellationToken cancellationToken = default)
        {
            var dbSet = FindDbSetObject(_dbContext, entityType);
            // 创建参数表达式 x
            var parameter = Expression.Parameter(entityType, "x");
            // 获取所有标记了 PasteMutexAttribute 的属性

            //非Id之外的表达式
            BinaryExpression binaryExpression = null;
            // 为每个标记了特性的属性添加条件
            foreach (var arg in args)
            {
                var dtoProperty = ReadCurrentPropertyThrowException(entityType, arg.Key);
                object queryValue = Convert.ChangeType(arg.Value, dtoProperty.PropertyType);
                //object propertyValue = dtoProperty.GetValue(dtoValue);
                // 创建属性访问表达式
                var propertyAccess = Expression.Property(parameter, dtoProperty);
                // 创建常量表达式
                var constant = Expression.Constant(queryValue, dtoProperty.PropertyType);
                // 创建相等表达式
                var equalExpression = Expression.Equal(propertyAccess, constant);
                if (binaryExpression != null)
                {
                    // 组合条件 Andorsl 单个条件满足 及触发
                    binaryExpression = Expression.AndAlso(binaryExpression, equalExpression);
                }
                else
                {
                    binaryExpression = equalExpression;
                }
            }

            // 创建 lambda 表达式
            var lambda = Expression.Lambda(binaryExpression, parameter);

            // 获取 Where 方法
            var whereMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Where" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType);

            // 调用 Where 方法
            var whereQuery = whereMethod.Invoke(null, new object[] { dbSet, lambda });
            // 获取 FirstOrDefaultAsync 方法
            var firstOrDefaultAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .First(m => m.Name == "FirstOrDefaultAsync" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType);

            // 调用 FirstOrDefaultAsync 方法
            var task = firstOrDefaultAsyncMethod.Invoke(null, new object[] { whereQuery, cancellationToken }) as Task;
            await task;

            // 获取结果
            var resultProperty = task.GetType().GetProperty("Result");
            return resultProperty.GetValue(task);
        }

        /// <summary>
        /// 动态读取模型属性
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelType"></param>
        /// <param name="IsListModel">针对枚举的备注过滤</param>
        /// <returns></returns>
        public VoloModelInfo ReadModelProperties(object model, Type modelType, bool IsListModel = false)
        {
            return BuilderHelper.AttributeReadModelProperty(model, modelType, IsListModel);
        }

        /// <summary>
        /// 单词大小写转化
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private static string ConvertToPascalCase(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }
            if (char.IsUpper(input[0]))
            {
                return input;
            }
            return input.Substring(0, 1).ToUpper() + input.Substring(1);
        }

        /// <summary>
        /// 验证输入是否通过校验
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelType"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="PasteCodeException"></exception>
        public void ValidateModel(object model, Type modelType)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model);

            if (!Validator.TryValidateObject(model, validationContext, validationResults, true))
            {
                var errorMessages = validationResults
                    .SelectMany(r => r.MemberNames.Select(n => $"{n}: {r.ErrorMessage}"))
                    .ToList();

                throw new PasteCodeException("输入数据验证失败" + errorMessages[0], 400);
            }
        }

        /// <summary>
        /// 补充Extend外表查询 比如ExtendUser 可能有多个
        /// </summary>
        /// <param name="listDtoType"></param>
        /// <param name="dtos"></param>
        /// <param name="_dbContext"></param>
        /// <returns></returns>
        public async Task BuildOuterData(DbContext _dbContext, Type listDtoType, List<object> dtos)
        {
            var sears = MemoryProperties(listDtoType).Where(x => x.GetCustomAttributes<PasteShortAttribute>().Any()).ToList();
            if (sears?.Any() == true)
            {
                foreach (var _search in sears)
                {
                    var short_attr = _search.GetCustomAttributes<PasteShortAttribute>().FirstOrDefault();
                    if (short_attr != null && short_attr != default)
                    {
                        var out_id_field = _search.Name;//UserId
                        var out_entity_name = short_attr.Args1;//RoleInfo
                        var to_extend = short_attr.Args2;//ExtendUser

                        if (String.IsNullOrEmpty(out_entity_name))
                        {
                            //Logger.LogInformation($"FullName:{listDtoType.FullName} Has Short Attribute!");
                            continue;
                        }
                        else
                        {
                            //Serilog.Log.Information($"{nameof(DefaultHelper)} FullName:{listDtoType.FullName} Has Short Attribute Args1:{short_attr.Args1} Args2:{short_attr.Args2}!");
                            //Logger.LogInformation($"FullName:{listDtoType.FullName} Has Short Attribute Args1:{short_attr.Args1} Args2:{short_attr.Args2}!");
                        }

                        var target_property = MemoryProperties(listDtoType).Where(x => string.Equals(x.Name, to_extend, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (target_property == null || target_property == default)
                        {
                            continue;
                        }

                        var relatedDtoType = target_property.PropertyType;

                        var ids = new List<object>();
                        foreach (var dto in dtos)
                        {
                            //var id = dto.GetType().GetProperty(out_id_field, BindingFlags.Public | BindingFlags.Instance)?.GetValue(dto);
                            var id = ReadCurrentProperty(dto.GetType(), out_id_field)?.GetValue(dto);//.GetProperty(out_id_field, BindingFlags.Public | BindingFlags.Instance)?.GetValue(dto);
                            if (id != null && !ids.Contains(id))
                            {
                                ids.Add(id);
                            }
                        }
                        if (ids.Count > 0)
                        {
                            //从数据库读取数据
                            var relatedEntityType = FindEntityType(out_entity_name);
                            //转换成对应的dto
                            //赋值
                            if (relatedEntityType == null)
                            {
                                continue;
                            }
                            // 构建ID查询条件
                            var relatedQuery = BuildIdQuery(_dbContext, relatedEntityType, ids);
                            if (relatedQuery != null)
                            {
                                //直接查询Dto
                                var relatedDtos = await ExecuteProjectionQuery(relatedQuery, relatedEntityType, relatedDtoType);

                                // 3. 创建ID到DTO的映射字典
                                //var idToDto = relatedDtos.ToDictionary(d => d.GetType().GetProperty("Id", BindingFlags.Public | BindingFlags.Instance)?.GetValue(d));
                                var idToDto = relatedDtos.ToDictionary(d => ReadCurrentProperty(d.GetType(), "Id")?.GetValue(d));
                                // 4. 填充到主表DTO
                                foreach (var dto in dtos)
                                {
                                    //获取UserId的值
                                    //var id = dto.GetType().GetProperty(out_id_field, BindingFlags.Public | BindingFlags.Instance)?.GetValue(dto);
                                    var id = ReadCurrentProperty(dto.GetType(), out_id_field)?.GetValue(dto);//.GetProperty(out_id_field, BindingFlags.Public | BindingFlags.Instance)?.GetValue(dto);
                                    if (id != null && idToDto.TryGetValue(id, out var relatedDto))
                                    {
                                        //给ExtendUser填充值
                                        ReadCurrentProperty(dto.GetType(), to_extend)?.SetValue(dto, relatedDto);//.GetProperty(to_extend, BindingFlags.Public | BindingFlags.Instance)?.SetValue(dto, relatedDto);
                                    }
                                }
                            }

                        }

                    }
                }
            }
        }

        /// <summary>
        /// 校验外表是否合法 PasteLink的校验
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task CheckEntityShortData(DbContext _dbContext, Type entityType, object entity)
        {
            // TODO:下面有问题，outer的args1有可能是一个路径... .. .
            //PasteLink PateInclude 标记OuterId和Include模式
            var sears = MemoryProperties(entityType).Where(x => x.GetCustomAttributes<PasteLinkAttribute>().Any()).ToList();
            if (sears?.Any() == true)
            {
                foreach (var _search in sears)
                {
                    var outerValue = _search.GetValue(entity);
                    if (outerValue == null)
                    {
                        continue;
                    }
                    if (outerValue.ToString() == "0")
                    {
                        continue;
                    }
                    //if(_search.GetValue)
                    var short_attr = _search.GetCustomAttributes<PasteLinkAttribute>().FirstOrDefault();
                    if (short_attr != null && short_attr != default)
                    {
                        var outerEntityName = short_attr.Args1;//RoleInfo
                        if (String.IsNullOrEmpty(outerEntityName))
                        {
                            Serilog.Log.Warning($"{entityType.Name}.{_search.Name}.PasteLink.Args1 配置不能为空!");
                            continue;
                        }
                        var outerEntityType = FindEntityType(outerEntityName);
                        if (outerEntityType == null)
                        {

                            Serilog.Log.Warning($"没有找到:{short_attr.Args1}的 Entity!");
                            continue;
                        }
                        var args = new Dictionary<string, string> { };
                        args.Add("Id", outerValue.ToString());
                        var first = await this.FindFirstOrDefaultAsync(_dbContext, outerEntityType, args);
                        if (first == null)
                        {
                            throw new PasteCodeException($"没有从:{outerEntityName} 中找到对象:{outerValue.ToString()} !");
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 构建搜索项
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="searchType"></param>
        /// <param name="input"></param>
        /// <param name="_dbContext"></param>
        /// <returns></returns>
        public IQueryable BuildDynamicQuery(DbContext _dbContext, Type entityType, Type searchType, object input)
        {
            var dbSet = FindDbSetObject(_dbContext, entityType);
            // 获取IQueryable接口
            var queryable = (IQueryable)dbSet;
            // 获取所有属性（包括继承的）
            var selectedProperties = ReadTypeProperties(searchType);
            //var entityProperties = ReadTypeProperties(entityType);
            queryable = ApplyFilterConditions(queryable, entityType, selectedProperties, input);
            // 应用过滤条件
            return queryable;
        }

        /// <summary>
        /// 获取目标类型的PropertyInfo,如果有new的则进行基类过滤
        /// </summary>
        /// <param name="searchType"></param>
        /// <returns></returns>
        private PropertyInfo[] ReadTypeProperties(Type searchType)
        {

            var typeName = searchType.FullName;
            // 使用GetOrCreate替代TryGetValue+Set
            return _memoryCache.GetOrCreate(typeName, entry =>
            {
                // 这里的逻辑只会被一个线程执行（其他线程等待）
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(1800); // 设置过期时间
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
            });

        }

        /// <summary>
        /// 应用过滤条件的方法
        /// 后续要优化成 entityProperties和searchProperties
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <param name="searchProperties"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        private IQueryable ApplyFilterConditions(IQueryable query, Type entityType, PropertyInfo[] searchProperties, object input)
        {
            //需要忽略的字段，一般表示处理过了，或者是内置有特殊用法的字段
            var ignors = new List<string> { "page", "size", "orderby" };
            foreach (var inputProperty in searchProperties)
            {
                if (ignors.Contains(inputProperty.Name))
                {
                    continue;
                }
                // 获取属性值 空值 0 等的处理
                var value = inputProperty.GetValue(input);
                if (IsDefaultValue(value, inputProperty.PropertyType))
                {
                    //Serilog.Log.Information($"由于默认值为空，被忽略了:{inputProperty.Name} ");
                    continue;
                }
                // 特殊处理搜索关键词
                if (inputProperty.Name == "word" && !string.IsNullOrEmpty(value?.ToString()))
                {
                    //是否要一个字段标记这个?
                    query = ApplySearchKeyword(query, entityType, value.ToString());
                    continue;
                }
                //实际查询字段 比如level_str映射到实际中的Level,其实就是读取PasteField
                var entityProperty = FindSearchProperty(inputProperty, entityType);
                if (entityProperty != null)
                {
                    #region 基于时间区间查询，比如查询2025-05-01 00:00:00 - 2025-06-01 00:00:00 注册的用户，包含开始时间
                    var btype = inputProperty.PropertyType;
                    //时间区间查询处理
                    if (btype == typeof(DateTime) || btype == typeof(DateTime?))
                    {
                        var find = inputProperty.GetCustomAttributes<PasteDaterangeAttribute>().FirstOrDefault();
                        if (find != null && find != default)
                        {
                            var edate_pro = find.Args2;
                            var sdate_str = inputProperty.GetValue(input);
                            var end_property = searchProperties.Where(x => String.Equals(x.Name, edate_pro, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                            if (end_property != null && end_property != default)
                            {
                                if (DateTime.TryParse(sdate_str.ToString(), out var sdate))
                                {
                                    var edate_str = end_property.GetValue(input);
                                    if (DateTime.TryParse(edate_str.ToString(), out var edate))
                                    {
                                        query = ApplyComparisonFilter(query, entityType, entityProperty, sdate, "GreaterThanOrEqual");
                                        query = ApplyComparisonFilter(query, entityType, entityProperty, edate, "LessThan");
                                    }
                                }
                            }
                            ignors.Add(edate_pro);
                            continue;
                        }
                    }
                    #endregion

                    #region 基于数据段查询，比如查询成绩80-90分的学生，包含80不包含90
                    var isRange = inputProperty.GetCustomAttributes<PasteSearchRangeAttribute>().FirstOrDefault();
                    if (isRange != null && isRange != default)
                    {
                        var edate_pro = isRange.Args2;
                        var sdate_str = inputProperty.GetValue(input);
                        var end_property = searchProperties.Where(x => String.Equals(x.Name, edate_pro, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
                        if (end_property != null && end_property != default)
                        {
                            var edate_str = end_property.GetValue(input).ToString();
                            var sdate = Convert.ChangeType(sdate_str, entityProperty.PropertyType);
                            var edate = Convert.ChangeType(edate_str, entityProperty.PropertyType);
                            query = ApplyComparisonFilter(query, entityType, entityProperty, sdate, "GreaterThanOrEqual");
                            query = ApplyComparisonFilter(query, entityType, entityProperty, edate, "LessThan");
                        }
                        ignors.Add(edate_pro);
                        continue;
                    }
                    #endregion

                    // 处理枚举类型
                    if (inputProperty.PropertyType.IsEnum)
                    {
                        var enumValue = Enum.ToObject(inputProperty.PropertyType, value);
                        if (entityProperty.PropertyType == inputProperty.PropertyType.GetElementType())
                        {
                            // 枚举值为0表示全部，不进行查询
                            if (value is int intValue && intValue == 0)
                            {
                                continue;
                            }
                            // 将值转换为正确的枚举类型
                            //var enumValue = Enum.ToObject(property.PropertyType, value);
                            query = ApplyComparisonFilter(query, entityType, entityProperty, enumValue, ReturnOperator(inputProperty));
                            continue;
                        }
                        else
                        {
                            if (entityProperty.PropertyType == typeof(bool))
                            {
                                // 更健壮的布尔值判断
                                string enumString = enumValue.ToString().Trim();
                                bool boolValue;

                                if (bool.TryParse(enumString, out boolValue))
                                {
                                    // 如果能直接解析为bool
                                    query = ApplyComparisonFilter(query, entityType, entityProperty, boolValue, ReturnOperator(inputProperty));
                                }
                                else if (enumString.Equals("1"))
                                {
                                    // 处理数字1表示true的情况
                                    query = ApplyComparisonFilter(query, entityType, entityProperty, true, ReturnOperator(inputProperty));
                                }
                                else if (enumString.Equals("0"))
                                {
                                    // 处理数字0表示false的情况
                                    //query = ApplyComparisonFilter(query, entityType, entityProperty, false, ReturnOperator(inputProperty));
                                    continue;
                                }
                                else
                                {
                                    // 默认处理：任何无法识别的值都视为false，或者根据需求抛出异常
                                    query = ApplyComparisonFilter(query, entityType, entityProperty, false, ReturnOperator(inputProperty));
                                    // 或者抛出异常：throw new ArgumentException($"无法将值 '{enumString}' 转换为布尔类型");
                                }
                                continue;
                            }
                        }
                        //转化成需要的类型，比如输入是枚举，数据库中是Int
                        var change_value = Convert.ChangeType(enumValue, entityProperty.PropertyType);
                        query = ApplyComparisonFilter(query, entityType, entityProperty, change_value, ReturnOperator(inputProperty));
                        continue;
                    }

                    // 处理枚举数组类型
                    if (inputProperty.PropertyType.IsArray)
                    {
                        if (inputProperty.PropertyType.GetElementType().IsEnum)
                        {
                            if (entityProperty.PropertyType == inputProperty.PropertyType.GetElementType())
                            {
                                // 如果传入的是数组，直接使用
                                if (value is Array enumArray)
                                {
                                    query = ApplyComparisonFilter(query, entityType, entityProperty, value, ReturnOperator(inputProperty));
                                }
                                else
                                {
                                    // 如果传入的是单个值，转换为单元素数组 这里应该是有问题的，因为上一级已经处理这个转化了... .. .
                                    var elementType = inputProperty.PropertyType.GetElementType();
                                    var array = Array.CreateInstance(elementType, 1);
                                    array.SetValue(Enum.ToObject(elementType, value), 0);
                                    query = ApplyComparisonFilter(query, entityType, entityProperty, array, ReturnOperator(inputProperty));
                                }
                            }
                            else
                            {
                                // 输入是enum[]类型，表类型是int
                                // 使用枚举类型的泛型列表替代object列表
                                var enumType = Nullable.GetUnderlyingType(entityProperty.PropertyType) ?? entityProperty.PropertyType;
                                var arrs = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(enumType));

                                // 如果传入的是数组，直接使用
                                if (value is Array enumArray)
                                {
                                    foreach (var item in enumArray)
                                    {
                                        if (item != null)
                                        {
                                            var nval = Convert.ChangeType(item, enumType);
                                            arrs.Add(nval);
                                        }
                                    }
                                }
                                // 如果传入的是单个值，转换为单元素数组
                                else if (value != null)
                                {
                                    var nval = Convert.ChangeType(value, enumType);
                                    arrs.Add(nval);
                                }

                                if (arrs?.Count == 0)
                                {
                                    continue;
                                }
                                query = ApplyComparisonFilter(query, entityType, entityProperty, arrs, ReturnOperator(inputProperty));
                            }
                            continue;
                        }
                        else
                        {
                            //其他类型 比如Int32[] UInt32[]等
                            //什么情况下 检索是array而不是枚举，比如传入ags_array 基于年龄查询... .. .
                            query = ApplyComparisonFilter(query, entityType, entityProperty, value, ReturnOperator(inputProperty));
                            continue;
                        }
                    }

                    //转化到数据库字段类型，然后进行过滤
                    var new_val = Convert.ChangeType(value.ToString(), entityProperty.PropertyType);
                    if (new_val != null)
                    {
                        query = ApplyComparisonFilter(query, entityType, entityProperty, new_val, ReturnOperator(inputProperty));
                    }
                    else
                    {
                        //Serilog.Log.Information($"转化失败，无法继续查询 :{inputProperty.Name} ");
                    }
                }
                else
                {
                    //Serilog.Log.Information($"{nameof(DefaultHelper)} {inputProperty.Name} 没有找到检索字段对应Entity的Property信息，是否是未标记PasteField特性!");
                }
            }

            // 自动包含所有标记了 PasteInclude 的导航属性
            //query = ApplyAllPasteIncludes(query, entityType);

            return query;
        }

        /// <summary>
        /// 获取运算符
        /// </summary>
        /// <param name="property"></param>
        /// <returns></returns>
        private static string ReturnOperator(PropertyInfo property)
        {
            var finds = property.GetCustomAttributes<PasteFieldAttribute>();
            if (finds?.Any() == true)
            {
                var find = finds.FirstOrDefault();
                if (find != null && find != default)
                {
                    if (!String.IsNullOrEmpty(find.Args2))
                    {
                        switch (find.Args2)
                        {
                            case "=": return "Equals";
                            case "!=": return "NotEquals";
                            case ">": return "GreaterThan";
                            case ">=": return "GreaterThanOrEqual";
                            case "<": return "LessThan";
                            case "<=": return "LessThanOrEqual";
                            case "()": return "Contains";
                            default: return "Equals";
                        }
                    }
                }
            }
            return "Equals";
        }

        /// <summary>
        /// 查询属性 查询字段
        /// </summary>
        /// <param name="property"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        private PropertyInfo FindSearchProperty(PropertyInfo property, Type entityType)
        {
            var target_property = property.Name;
            if (property.GetCustomAttributes<PasteFieldAttribute>()?.Any() == true)
            {
                var find = property.GetCustomAttributes<PasteFieldAttribute>().FirstOrDefault();
                if (find != null && find != default)
                {
                    target_property = find.Args1;
                }
            }
            // 检查实体是否有对应的属性
            var entityProperty = ReadCurrentProperty(entityType, target_property);
            if (entityProperty != null)
            {
                return entityProperty;
            }
            return null;
        }

        /// <summary>
        /// 判断值是否为类型的默认值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool IsDefaultValue(object value, Type type)
        {
            if (value == null)
            {
                return true;
            }
            // 值类型处理
            if (type.IsValueType)
            {
                // 可空类型
                if (Nullable.GetUnderlyingType(type) != null)
                {
                    if (value == null)
                    {
                        return true;
                    }
                    // 获取基础类型
                    type = Nullable.GetUnderlyingType(type);
                }
                // 创建默认值实例
                object defaultValue = Activator.CreateInstance(type);
                // 比较值
                return value.Equals(defaultValue);
            }
            // 引用类型处理
            if (type == typeof(string))
            {
                return string.IsNullOrEmpty(value.ToString());
            }
            var strValue = value.ToString();
            if (String.IsNullOrEmpty(strValue) || strValue == "0")
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 应对有 new 字段的情况
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="entityType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private static MemberExpression GetPropertyAccess(ParameterExpression parameter, Type entityType, string propertyName)
        {
            try
            {
                // 方法1：尝试获取当前类声明的属性（包括new的）
                var property = entityType.GetProperty(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                if (property != null)
                    return Expression.Property(parameter, property);

                // 方法2：如果没有，获取继承的属性
                property = entityType.GetProperty(propertyName);
                return Expression.Property(parameter, property);
            }
            catch (AmbiguousMatchException)
            {
                // 方法3：如果出现歧义，手动选择
                var properties = entityType.GetProperties(
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

                var property = properties.FirstOrDefault(p => p.Name == propertyName)
                    ?? entityType.GetProperty(propertyName); // 回退

                return Expression.Property(parameter, property);
            }
        }

        /// <summary>
        /// 后续需要优化下 不然每次都获取Properties 主要是针对继承等情况... .. .
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <param name="property"></param>
        /// <param name="value"></param>
        /// <param name="operatorName"></param>
        /// <returns></returns>
        private static IQueryable ApplyComparisonFilter(IQueryable query, Type entityType, PropertyInfo property, object value, string operatorName = "Equals")
        {

            //Se.LogInformation($"Apply Query Queryable Property:{property.Name} Value:{value.ToString()} Operator:{operatorName}");

            var parameter = Expression.Parameter(entityType, "x");
            //var propertyAccess = Expression.Property(parameter, property.Name);
            var propertyAccess = GetPropertyAccess(parameter, entityType, property.Name);
            Expression comparisonExpression;

            switch (operatorName)
            {
                case "Equals":
                    // 处理数组/集合的等于查询（实际上是IN查询）
                    if (value is IEnumerable && !(value is string))
                    {
                        var containsMethod = typeof(Enumerable).GetMethods()
                            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                            .MakeGenericMethod(property.PropertyType);

                        comparisonExpression = Expression.Call(
                            null,
                            containsMethod,
                            Expression.Constant(value),
                            propertyAccess);
                    }
                    else
                    {
                        var constant = Expression.Constant(value, property.PropertyType);
                        comparisonExpression = Expression.Equal(propertyAccess, constant);
                    }
                    break;

                case "NotEquals":
                    var notEqualConstant = Expression.Constant(value, property.PropertyType);
                    comparisonExpression = Expression.NotEqual(propertyAccess, notEqualConstant);
                    break;

                case "GreaterThan":
                    var gtConstant = Expression.Constant(value, property.PropertyType);
                    comparisonExpression = Expression.GreaterThan(propertyAccess, gtConstant);
                    break;

                case "GreaterThanOrEqual":
                    var gteConstant = Expression.Constant(value, property.PropertyType);
                    comparisonExpression = Expression.GreaterThanOrEqual(propertyAccess, gteConstant);
                    break;

                case "LessThan":
                    var ltConstant = Expression.Constant(value, property.PropertyType);
                    comparisonExpression = Expression.LessThan(propertyAccess, ltConstant);
                    break;

                case "LessThanOrEqual":
                    var lteConstant = Expression.Constant(value, property.PropertyType);
                    comparisonExpression = Expression.LessThanOrEqual(propertyAccess, lteConstant);
                    break;

                case "Contains":
                    // 处理字符串Contains
                    if (property.PropertyType == typeof(string))
                    {
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        comparisonExpression = Expression.Call(propertyAccess, containsMethod, Expression.Constant(value));
                    }
                    // 处理集合Contains（查询值是否在集合中）
                    else if (value is IEnumerable && !(value is string))
                    {
                        var containsMethod = typeof(Enumerable).GetMethods()
                            .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                            .MakeGenericMethod(property.PropertyType);

                        comparisonExpression = Expression.Call(
                            null,
                            containsMethod,
                            Expression.Constant(value),
                            propertyAccess);
                    }
                    // 处理其他类型的Contains（如List<T>.Contains）
                    else
                    {
                        var containsMethod = property.PropertyType.GetMethod("Contains", new[] { property.PropertyType.GenericTypeArguments[0] });
                        comparisonExpression = Expression.Call(propertyAccess, containsMethod, Expression.Constant(value));
                    }
                    break;

                default:
                    comparisonExpression = Expression.Equal(propertyAccess, Expression.Constant(value, property.PropertyType));
                    break;
            }

            var lambda = Expression.Lambda(comparisonExpression, parameter);
            var whereCallExpression = Expression.Call(
                typeof(Queryable), "Where",
                new Type[] { entityType },
                query.Expression,
                lambda);

            return query.Provider.CreateQuery(whereCallExpression);
        }

        /// <summary>
        /// 应用关键字搜索
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        private IQueryable ApplySearchKeyword(IQueryable query, Type entityType, string keyword)
        {
            // 外部已确保keyword非空，这里仅做防御性校验
            if (string.IsNullOrEmpty(keyword))
            {
                return query;
            }

            var stringProperties = MemoryProperties(entityType)
                .Where(p => p.PropertyType == typeof(string) && p.GetCustomAttributes<PasteSearchAttribute>()?.Any() == true)
                .ToList();

            if (!stringProperties.Any())
            {
                return query;
            }

            var parameter = Expression.Parameter(entityType, "x");
            Expression orExpression = null;
            var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });

            foreach (var property in stringProperties)
            {
                // 只构建“属性包含关键词”的表达式，不添加任何与关键词为空相关的判断
                var propertyAccess = Expression.Property(parameter, property);
                var containsExpression = Expression.Call(
                    propertyAccess,
                    containsMethod,
                    Expression.Constant(keyword) // 直接使用关键词“bb”
                );

                orExpression = orExpression == null
                    ? containsExpression
                    : Expression.OrElse(orExpression, containsExpression);
            }

            var lambda = Expression.Lambda(orExpression, parameter);
            var whereCall = Expression.Call(
                typeof(Queryable),
        "Where",
                new[] { entityType },
                query.Expression,
                lambda
            );

            return query.Provider.CreateQuery(whereCall);
        }

        /// <summary>
        /// 上面的补充，用于其他查询
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <param name="searchType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public IQueryable ApplyPagingAndSorting(IQueryable query, Type entityType, Type searchType, Object input)
        {
            // 获取Id属性信息
            var idProperty = ReadCurrentPropertyThrowException(entityType, "Id");//.GetProperties().Where(x => x.Name == "Id").LastOrDefault();
            var page = 1;
            var size = 20;
            var orderby = String.Empty;
            foreach (var ii in MemoryProperties(searchType))
            {
                if (string.Equals(ii.Name, "page", StringComparison.CurrentCultureIgnoreCase))
                {
                    var _page = ii.GetValue(input);
                    int.TryParse(_page.ToString(), out page);
                    continue;
                }
                if (string.Equals(ii.Name, "size", StringComparison.CurrentCultureIgnoreCase))
                {
                    var _page = ii.GetValue(input);
                    int.TryParse(_page.ToString(), out size);
                    continue;
                }
                if (string.Equals(ii.Name, "orderby", StringComparison.CurrentCultureIgnoreCase))
                {
                    var _page = ii.GetValue(input);
                    orderby = _page.ToString().Trim();
                    continue;
                }
            }
            var actionOrderby = false;
            if (!String.IsNullOrEmpty(orderby))
            {
                #region 下面是旧版本，只支持单个排序
                //var fieldName = orderby.Split(" ")[0];
                //var findOrderProperty = ReadCurrentProperty(entityType, fieldName);
                //if (findOrderProperty != null)
                //{
                //    var orderType = "OrderByDescending";
                //    if (!orderby.Contains(" desc", StringComparison.OrdinalIgnoreCase))
                //    {
                //        orderType = "OrderBy";
                //    }
                //    // 构建排序表达式 默认 OrderByDescending(x=>x.Id)
                //    var parameter = Expression.Parameter(entityType, "x");
                //    var propertyAccess = Expression.Property(parameter, findOrderProperty);
                //    var lambda = Expression.Lambda(propertyAccess, parameter);
                //    // 使用正确的属性类型作为排序键类型
                //    var orderByDescendingCall = Expression.Call(
                //        typeof(Queryable), orderType,
                //        new Type[] { entityType, findOrderProperty.PropertyType }, // 使用实际的属性类型
                //        query.Expression,
                //        lambda);
                //    query = query.Provider.CreateQuery(orderByDescendingCall);
                //    actionOrderby = true;
                //}
                #endregion
                //支持多个查询，比如id,name desc
                query = PasteOrder(query, orderby);
            }
            if (!actionOrderby)
            {
                // 构建排序表达式 默认 OrderByDescending(x=>x.Id)
                var parameter = Expression.Parameter(entityType, "x");
                var propertyAccess = Expression.Property(parameter, idProperty);
                var lambda = Expression.Lambda(propertyAccess, parameter);
                // 使用正确的属性类型作为排序键类型
                var orderByDescendingCall = Expression.Call(
                    typeof(Queryable),
            "OrderByDescending",
                    new Type[] { entityType, idProperty.PropertyType }, // 使用实际的属性类型
                    query.Expression,
                    lambda);
                query = query.Provider.CreateQuery(orderByDescendingCall);
            }
            // 应用分页
            query = query.Skip((page - 1) * size).Take(size);
            return query;
        }

        /// <summary>
        /// 支持多排序 比如id,name desc等
        /// </summary>
        /// <param name="query"></param>
        /// <param name="ordering"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private IOrderedQueryable PasteOrder(IQueryable query, string ordering)
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
        /// 查找字段 应对new的情况
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        private PropertyInfo ReadCurrentPropertyThrowException(Type entityType, string propertyName)
        {
            var read = ReadCurrentProperty(entityType, propertyName);
            if (read != null)
            {
                return read;
            }
            // 3. 找不到Id属性
            throw new PasteCodeException($"实体 {entityType.Name} 不包含{propertyName}属性", 500);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public PropertyInfo ReadCurrentProperty(Type entityType, string propertyName)
        {
            var properties = ReadTypeProperties(entityType);
            if (properties != null)
            {
                //var find = properties.FirstOrDefault(x => x.Name == propertyName);
                var find = properties.FirstOrDefault(x => string.Equals(x.Name, propertyName, StringComparison.OrdinalIgnoreCase));
                if (find != null)
                {
                    return find;
                }
            }
            return null;
        }

        /// <summary>
        /// 执行计数查询的方法
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        public async Task<int> ExecuteCountQuery(IQueryable query, Type entityType)
        {
            var countMethod = typeof(Queryable)
                .GetMethods()
                .First(m => m.Name == "Count" && m.GetParameters().Length == 1)
                .MakeGenericMethod(entityType);

            return (int)await Task.FromResult(countMethod.Invoke(null, new object[] { query }));
        }

        /// <summary>
        /// 直接查询并投影为DTO 这个有问题，Include等不足
        /// </summary>
        public async Task<List<object>> ExecuteProjectionQuery(IQueryable query, Type entityType, Type dtoType)
        {
            // 构建Select表达式
            var selectExpression = BuildSelectExpression(entityType, dtoType);

            // 创建泛型Func类型 (EntityType -> DtoType)
            var funcType = typeof(Func<,>).MakeGenericType(entityType, dtoType);

            // 创建lambda表达式
            var lambda = Expression.Lambda(funcType, selectExpression.Body, selectExpression.Parameters[0]);

            // 构建Select方法
            var selectMethod = typeof(Queryable).GetMethods()
                .First(m => m.Name == "Select" && m.GetParameters().Length == 2)
                .MakeGenericMethod(entityType, dtoType);

            // 应用Select投影
            var projectedQuery = selectMethod.Invoke(null, new object[] { query, lambda });

            var toListMethod = typeof(EntityFrameworkQueryableExtensions).GetMethods()
                .First(m => m.Name == "ToListAsync" && m.GetParameters().Length == 2)
                .MakeGenericMethod(typeof(object));
            var task = (Task)toListMethod.Invoke(null, new object[] { projectedQuery, CancellationToken.None });
            await task;
            var resultProperty = task.GetType().GetProperty("Result");
            return (List<object>)resultProperty.GetValue(task);
        }

        /// <summary>
        /// 执行包含 Include 的查询并映射到 DTO 旧版本ExecuteProjectionQuery
        /// </summary>
        public async Task<List<object>> ExecuteProjectionQueryWithIncludes(IQueryable query, Type entityType, Type dtoType)
        {
            try
            {
                // 执行查询获取包含 Include 的实体列表
                var toListAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                    .GetMethods()
                    .First(m => m.Name == "ToListAsync" && m.GetParameters().Length == 2)
                    .MakeGenericMethod(entityType);

                var task = (Task)toListAsyncMethod.Invoke(null, new object[] { query, CancellationToken.None });
                await task;

                var resultProperty = task.GetType().GetProperty("Result");
                var entityList = resultProperty.GetValue(task);
                if (entityList == null) { return null; }
                //Console.WriteLine($"{Newtonsoft.Json.JsonConvert.SerializeObject(entityList)}");
                // 使用 ObjectMapper 将实体列表映射到 DTO 列表
                return MapEntityListToDtoListDirectly(entityList, entityType, dtoType);
            }
            catch (Exception ex)
            {
                Serilog.Log.Error(ex, "执行包含 Include 的投影查询失败");
                throw;
            }
        }

        /// <summary>
        /// 直接映射整个实体列表到 DTO 列表
        /// </summary>
        private List<object> MapEntityListToDtoListDirectly(object entityList, Type entityType, Type dtoType)
        {
            try
            {
                if (entityList == null) return null;
                // 修正：使用 TypeAdapter.Adapt 静态方法
                var result = Mapster.TypeAdapter.Adapt(entityList, entityList.GetType(), typeof(List<>).MakeGenericType(dtoType));
                if (result is System.Collections.IList list)
                {
                    return new List<object>(list.Cast<object>());
                }
                return null;
            }
            catch (Exception ex)
            {
                Serilog.Log.Warning(ex, "列表映射失败");
                return null;
            }

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        private LambdaExpression BuildSelectExpression(Type entityType, Type dtoType)
        {
            // 创建参数表达式 (x => ...)
            var parameter = Expression.Parameter(entityType, "x");

            // 创建DTO属性的绑定
            var bindings = MemoryProperties(dtoType)
                .Where(p => p.CanWrite)
                .Select(p =>
                {
                    // 查找实体类型中是否有同名属性
                    //var entityProperty = entityType.GetProperty(p.Name, BindingFlags.Public | BindingFlags.Instance);
                    var entityProperty = ReadCurrentProperty(entityType, p.Name);// entityType.GetProperty(p.Name, BindingFlags.Public | BindingFlags.Instance);
                    if (entityProperty != null && entityProperty.CanRead)
                    {
                        if (entityProperty.PropertyType == p.PropertyType)
                        {
                            var propertyAccess = Expression.Property(parameter, entityProperty);
                            return Expression.Bind(p, propertyAccess);
                        }
                    }

                    return null;
                })
                .Where(b => b != null)
                .ToList();

            // 创建DTO对象初始化表达式
            var dtoInit = Expression.MemberInit(
                Expression.New(dtoType),
                bindings);

            // 创建lambda表达式 x => new Dto { ... }
            return Expression.Lambda(dtoInit, parameter);
        }

        /// <summary>
        /// 构建ID查询条件
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <param name="ids"></param>
        /// <returns></returns>
        private IQueryable BuildIdQuery(DbContext _dbContext, Type entityType, List<object> ids)
        {
            // 获取DbSet
            //var dbSetMethod = _dbContext.GetType()
            //    .GetMethods()
            //    .First(m => m.Name == "Set" && m.IsGenericMethod);
            //var genericDbSetMethod = dbSetMethod.MakeGenericMethod(entityType);
            //var dbSet = genericDbSetMethod.Invoke(_dbContext, null);
            var dbSet = FindDbSetObject(_dbContext, entityType);
            var queryable = (IQueryable)dbSet;

            // 动态构建ID查询表达式: x => ids.Contains(x.Id)
            var parameter = Expression.Parameter(entityType, "x");
            var idProperty = ReadCurrentPropertyThrowException(entityType, "Id");// entityType.GetProperty("Id", BindingFlags.Public | BindingFlags.Instance);

            var propertyAccess = Expression.Property(parameter, idProperty);

            // 创建包含所有ID的常量表达式
            var method = typeof(Enumerable).GetMethods()
                .First(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                .MakeGenericMethod(idProperty.PropertyType);

            ConstantExpression constant;
            if (idProperty.PropertyType == typeof(int))
            {
                List<int> id_keys = new List<int>();
                foreach (var ii in ids)
                {
                    if (int.TryParse(ii.ToString(), out var _id))
                    {
                        if (_id == 0) { continue; }
                        id_keys.Add(_id);
                    }
                }
                if (id_keys.Count == 0)
                {
                    return null;
                }
                constant = Expression.Constant(id_keys);
            }
            else if (idProperty.PropertyType == typeof(long))
            {
                List<long> id_keys = new List<long>();
                foreach (var ii in ids)
                {
                    if (long.TryParse(ii.ToString(), out var _id))
                    {
                        if (_id == 0) { continue; }
                        id_keys.Add(_id);
                    }
                }
                if (id_keys.Count == 0)
                {
                    return null;
                }
                constant = Expression.Constant(id_keys);
            }
            else if (idProperty.PropertyType == typeof(Guid))
            {
                List<Guid> id_keys = new List<Guid>();
                foreach (var ii in ids)
                {
                    if (Guid.TryParse(ii.ToString(), out var _id))
                    {
                        if (_id == Guid.Empty) { continue; }
                        id_keys.Add(_id);
                    }
                }
                if (id_keys.Count == 0)
                {
                    return null;
                }
                constant = Expression.Constant(id_keys);
            }
            else
            {
                List<string> id_keys = new List<string>();
                foreach (var ii in ids)
                {
                    var id_str = ii.ToString();
                    if (!String.IsNullOrEmpty(id_str))
                    {
                        id_keys.Add(id_str);
                    }
                }
                if (id_keys.Count == 0)
                {
                    return null;
                }
                constant = Expression.Constant(id_keys);
            }

            var containsExpression = Expression.Call(method, constant, propertyAccess);

            var lambda = Expression.Lambda(containsExpression, parameter);
            var whereCallExpression = Expression.Call(typeof(Queryable), "Where", new Type[] { entityType }, queryable.Expression, lambda);

            return queryable.Provider.CreateQuery(whereCallExpression);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public object GetPropertyValue(object obj, string propertyName)
        {
            var property = ReadCurrentProperty(obj.GetType(), propertyName);//.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance);
            return property?.GetValue(obj);
        }

        /// <summary>
        /// 结果映射到实体Dto中
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        public void ForeachMapProperties(object source, object destination, Type sourceType, Type destinationType)
        {
            // 获取源类型的所有可读属性
            var sourceProperties = MemoryProperties(sourceType)
                .Where(p => p.CanRead)
                .ToList();

            // 获取目标类型的所有可写属性
            var destinationProperties = MemoryProperties(destinationType)
                .Where(p => p.CanWrite)
                .ToList();

            // 映射相同名称和类型的属性
            foreach (var sourceProperty in sourceProperties)
            {
                var destinationProperty = destinationProperties
                    .FirstOrDefault(p => string.Equals(p.Name, sourceProperty.Name, StringComparison.OrdinalIgnoreCase) && p.PropertyType == sourceProperty.PropertyType);

                if (destinationProperty != null)
                {
                    var value = sourceProperty.GetValue(source);
                    destinationProperty.SetValue(destination, value);
                }
            }
        }

        /// <summary>
        /// 使用 ObjectMapper 进行对象映射
        /// </summary>
        public void MapProperties(object source, object destination, Type sourceType, Type destinationType)
        {
            //下面这个不支持继承
            //source.Adapt(destination, sourceType, destinationType);
            //var target = Activator.CreateInstance(destinationType);
            var sourceProps = sourceType.GetProperties();
            var targetProps = destinationType.GetProperties();
            foreach (var sp in sourceProps)
            {
                var tp = targetProps.FirstOrDefault(p => p.Name == sp.Name && p.CanWrite);
                if (tp != null)
                {
                    var val = sp.GetValue(source);
                    if (val != null && tp.PropertyType.IsAssignableFrom(val.GetType()))
                    {
                        tp.SetValue(destination, val);
                    }
                }
            }
            //return target;
        }

    }
}
