using Microsoft.EntityFrameworkCore;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;
using PasteApeart.Handler.Extend;
using PasteApeart.Helper;
using System.Dynamic;
using System.Linq.Dynamic.Core;
using System.Reflection;
using System.Text.Json;

namespace PasteApeart.Application
{
    /// <summary>
    /// 
    ///</summary>
    public class PasteDefaultAppService : IPasteDefaultAppService
    {

        /// <summary>
        /// 
        /// </summary>
        private readonly IPasteDefaultHelper _defaultHelper;

        /// <summary>
        /// 
        /// </summary>
        public readonly DbContext _dbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultHelper"></param>
        /// <param name="dbContext"></param>
        public PasteDefaultAppService(IPasteDefaultHelper defaultHelper, DbContext dbContext)
        {
            _defaultHelper = defaultHelper;
            _dbContext = dbContext;
        }

        ///// <summary>
        ///// 映射EntityName，这样就不会把数据库表名称暴露出去了 选填
        ///// </summary>
        //public Func<string, string> FuncMapperEntityName { get; set; } = null;

        /// <summary>
        /// 映射名称 这样就不会把数据库表名称暴露出去了
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public virtual string DefaultMapperEntityName(string entityName)
        {
            //if (FuncMapperEntityName != null)
            //{
            //    return FuncMapperEntityName(entityName);
            //}
            return entityName;
        }


        #region 

        /// <summary>
        /// 获取新增格式
        /// </summary>
        /// <returns></returns>
        public async Task<VoloModelInfo> ReadAddModel(string entityName)
        {
            entityName = DefaultMapperEntityName(entityName);
            // 1. 根据实体名称查找对应的实体类型和 DTO 类型
            var entityType = _defaultHelper.FindEntityType(entityName);
            if (entityType == null)
            {
                throw new PasteCodeException($"找不到对应的实体类型: {entityName}");
            }
            await DefaultFilterDataAsync(entityType, null, "add");
            var dtoType = _defaultHelper.FindDtoType(entityName, "AddDto");

            if (dtoType == null)
            {
                throw new PasteCodeException($"找不到对应的 DTO 类型: {entityName}AddDto");
            }

            //new一个出来
            var dto = Activator.CreateInstance(dtoType);

            // 4. 动态读取 DTO 的属性信息
            //var dataModel = DefaultHelper.ReadModelProperties(dto, dtoType);
            //return dataModel;

            return await DefaultBuildPropertyInfo("add", dto, dtoType);
        }

        /// <summary>
        /// 获取更新数据
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<VoloModelInfo> ReadUpdateModel(string entityName, string id)
        {
            entityName = DefaultMapperEntityName(entityName);
            // 1. 根据实体名称查找对应的实体类型和 DTO 类型
            var entityType = _defaultHelper.FindEntityType(entityName);
            var dtoType = _defaultHelper.FindDtoType(entityName, "UpdateDto");

            if (entityType == null || dtoType == null)
            {
                throw new PasteCodeException($"找不到对应的实体或 DTO 类型: {entityName}");
            }

            var entity = await _defaultHelper.FindEntityByIdWithIncludeAsync(_dbContext, entityType, id);
            if (entity == null)
            {
                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            }

            await DefaultFilterDataAsync(entityType, entity, "update");

            var dto = Activator.CreateInstance(dtoType);
            _defaultHelper.MapProperties(entity, dto, entityType, dtoType);

            //构建外表等 Include的如何搞定？？？
            await _defaultHelper.BuildOuterQuery(_dbContext, dto, dtoType);

            // 4. 动态读取 DTO 的属性信息
            return await DefaultBuildPropertyInfo("update", dto, dtoType);
        }

        /// <summary>
        /// 获取详细 获取详细对应的元数据
        /// </summary>
        /// <returns></returns>
        public async Task<VoloModelInfo> ReadDetailModel(string entityName, string id)
        {
            entityName = DefaultMapperEntityName(entityName);

            // 1. 根据实体名称查找对应的实体类型和 DTO 类型
            var entityType = _defaultHelper.FindEntityType(entityName);
            var dtoType = _defaultHelper.FindDtoType(entityName, "Dto");

            if (entityType == null || dtoType == null)
            {
                throw new PasteCodeException($"找不到对应的实体或 DTO 类型: {entityName}");
            }

            var dto = await ReadDetailObjectAsync(entityType,dtoType,id);

            //// 2. 动态查询实体
            //var entity = await _defaultHelper.FindEntityByIdWithIncludeAsync(_dbContext, entityType, id);
            //if (entity == null)
            //{
            //    throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            //}

            //await DefaultFilterDataAsync(entityType, entity, "detail");
            //var dto = Activator.CreateInstance(dtoType);
            //_defaultHelper.MapProperties(entity, dto, entityType, dtoType);

            ////构建外表等 Include的如何搞定？？？
            //await _defaultHelper.BuildOuterQuery(_dbContext, dto, dtoType);

            ////进行脱敏处理
            //var hasHalfHideAttribute = dtoType.GetCustomAttribute<PasteHalfHideAttribute>();
            //if (hasHalfHideAttribute != null)
            //{
            //    HalfHideHelper.PasteMaskData(dto, dtoType);
            //}

            // 4. 动态读取 DTO 的属性信息
            return await DefaultBuildPropertyInfo("detail", dto, dtoType);

        }


        /// <summary>
        /// 获取详细 获取数据本身
        /// </summary>
        /// <returns></returns>
        public async Task<object> ReadDetailObjectAsync(Type entityType,Type dtoType, string id)
        {

            //entityName = DefaultMapperEntityName(entityName);

            //// 1. 根据实体名称查找对应的实体类型和 DTO 类型
            //var entityType = _defaultHelper.FindEntityType(entityName);
            //var dtoType = _defaultHelper.FindDtoType(entityName, "Dto");

            //if (entityType == null || dtoType == null)
            //{
            //    throw new PasteCodeException($"找不到对应的实体或 DTO 类型: {entityName}");
            //}

            // 2. 动态查询实体
            var entity = await _defaultHelper.FindEntityByIdWithIncludeAsync(_dbContext, entityType, id);
            if (entity == null)
            {
                throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            }

            await DefaultFilterDataAsync(entityType, entity, "detail");
            var dto = Activator.CreateInstance(dtoType);
            _defaultHelper.MapProperties(entity, dto, entityType, dtoType);
            // 3. 动态映射实体到 DTO
            //var dto = DefaultHelper.MapEntityToDto(ObjectMapper, entity, entityType, dtoType);



            //构建外表等 Include的如何搞定？？？
            await _defaultHelper.BuildOuterQuery(_dbContext, dto, dtoType);

            //进行脱敏处理
            var hasHalfHideAttribute = dtoType.GetCustomAttribute<PasteHalfHideAttribute>();
            if (hasHalfHideAttribute != null)
            {
                HalfHideHelper.PasteMaskData(dto, dtoType);
            }

            // 4. 动态读取 DTO 的属性信息
            //var dataModel = DefaultHelper.ReadModelProperties(dto, dtoType);
            //return dataModel;
            //return await DefaultBuildPropertyInfo("detail", dto, dtoType);
            return dto;
        }


        /// <summary>
        /// 获取列表格式
        /// </summary>
        /// <returns></returns>
        public async Task<VoloModelInfo> ReadListModel(string entityName, string searchName = "")
        {
            return await ReadListQueryModel(entityName, searchName);
        }

        /// <summary>
        /// 读取模型数据
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        private async Task<VoloModelInfo> ReadListQueryModel(string entityName, string searchName = "")
        {
            entityName = DefaultMapperEntityName(entityName);

            Type searchType = null;
            if (String.IsNullOrEmpty(searchName))
            {
                //var extendQuery = "InputQuery" + DefaultHelper.ConvertToPascalCase(entityName);
                searchType = _defaultHelper.FindQueryType(entityName);
                if (searchType == null)
                {
                    searchName = "InputSearchBase";
                    searchType = _defaultHelper.AutoFindType(searchName);
                }
            }
            else
            {
                searchType = _defaultHelper.AutoFindType(searchName);
            }

            if (searchType == null)
            {
                throw new PasteCodeException("没有检索到搜索Model,请确认InputSearchBase这个基础检索Model存在!");
            }
            // 动态查找实体对应的 ListDto 类型
            var listDtoType = _defaultHelper.FindDtoType(entityName, "ListDto");
            // 创建 ListDto 实例
            var listDtoInstance = Activator.CreateInstance(listDtoType);
            // 动态读取 ListDto 的属性信息
            //var listModel = BuilderHelper.AttributeReadModelProperty(listDtoInstance, true);
            //var listModel = DefaultHelper.ReadModelProperties(listDtoInstance, listDtoType, true);
            var listModel = await DefaultBuildPropertyInfo("list", listDtoInstance, listDtoType, true);
            if (searchType != null)
            {
                var search_model = Activator.CreateInstance(searchType);
                //var searchModel = BuilderHelper.AttributeReadModelProperty(search_model);
                var searchModel = BuilderHelper.AttributeReadModelProperty(search_model, searchType, true);
                if (searchModel != null)
                {
                    listModel.QueryProperties = searchModel.Properties;
                }
            }
            //}
            return listModel;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="queryString"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<dynamic> Page(string entityName, string queryString, string searchName = "")
        {
            entityName = DefaultMapperEntityName(entityName);
            // [FromQuery] InputSearchBase input
            // 1. 根据实体名称查找对应的实体类型和DTO类型
            var entityType = _defaultHelper.FindEntityType(entityName);
            var listDtoType = _defaultHelper.FindDtoType(entityName, "ListDto");

            if (entityType == null || listDtoType == null)
            {
                throw new PasteCodeException($"找不到对应的实体或DTO类型: {entityName}", 404);
            }
            await DefaultFilterDataAsync(entityType, null, "page");
            //这里应该要改版，直接利用反射，赋值
            //var query_string = base._httpContext.Request.QueryString.ToString();

            Type searchType = null;
            if (String.IsNullOrEmpty(searchName))
            {
                //var extendQuery = "InputQuery" + DefaultHelper.ConvertToPascalCase(entityName);
                searchType = _defaultHelper.FindQueryType(entityName);
                if (searchType == null)
                {
                    searchName = nameof(InputSearchBase);
                    searchType = _defaultHelper.AutoFindType(searchName);
                }
            }
            else
            {
                searchType = _defaultHelper.AutoFindType(searchName);
            }

            if (searchType == null)
            {
                throw new PasteCodeException("没有检索到搜索Model,请确认InputSearchBase这个基础检索Model存在!");
            }


            object searchObject = null;
            searchObject = Activator.CreateInstance(searchType);
            var input = _defaultHelper.MapperToQueryModel(queryString, searchType, searchObject);


            dynamic pagedto = await DirectPageAsync(searchObject, entityType, searchType, input.page, listDtoType);
            if (input.page == 1)
            {
                if (queryString.Contains("request=one"))
                {
                    pagedto.pasteModel = await ReadListQueryModel(entityName, searchName);
                }
            }
            return pagedto;

            //dynamic pagedto = new ExpandoObject();
            //if (input.page == 1)
            //{
            //    if (queryString.Contains("request=one"))
            //    {
            //        pagedto.pasteModel = await ReadListQueryModel(entityName, searchName);
            //    }
            //}
            //IQueryable query = _defaultHelper.BuildDynamicQuery(_dbContext, entityType, searchType, searchObject);

            //if (input.page == 1)
            //{
            //    pagedto.totalCount = await _defaultHelper.ExecuteCountQuery(query, entityType);
            //}

            //var isIncludeType = _defaultHelper.HasPasteIncludeProperties(entityType);

            //if (isIncludeType)
            //{
            //    var includeQuery = _defaultHelper.ApplyAllPasteIncludes(query, entityType);
            //    if (includeQuery != null)
            //    {
            //        query = includeQuery as IQueryable;
            //    }
            //}
            //query = _defaultHelper.ApplyPagingAndSorting(query, entityType, searchType, searchObject);
            //List<object> dtos;
            //if (isIncludeType)
            //{
            //    dtos = await _defaultHelper.ExecuteProjectionQueryWithIncludes(query, entityType, listDtoType);
            //}
            //else
            //{
            //    dtos = await _defaultHelper.ExecuteProjectionQuery(query, entityType, listDtoType);
            //}
            //if (dtos == null) { return null; }
            //if (dtos?.Any() == false)
            //{
            //    return null;
            //}
            ////Console.WriteLine($"line.315 {Newtonsoft.Json.JsonConvert.SerializeObject(dtos)}");
            ////查询外表 并实现 这里是简单的外表ID的查询 如果有更复杂的，还需要实现
            //await _defaultHelper.BuildOuterData(_dbContext, listDtoType, dtos);
            //// 7. 设置结果并返回
            ////pagedDto.Items = dtos.Cast<object>().ToList();
            //pagedto.items = dtos;

            ////计算ExtendBind的数据... ExtendBind=true
            //await _defaultHelper.ReadExtendBind(_dbContext, listDtoType, dtos, searchType, searchObject);

            //return pagedto;
        }

        /// <summary>
        /// 直接获取页面数据 用于快捷开发
        /// </summary>
        /// <param name="searchObject"></param>
        /// <param name="entityType"></param>
        /// <param name="searchType"></param>
        /// <param name="page">第1页要返回totalCount</param>
        /// <param name="listDtoType"></param>
        /// <returns></returns>
        public async Task<dynamic> DirectPageAsync(object searchObject, Type entityType, Type searchType, int page, Type listDtoType)
        {

            dynamic pageDto = new ExpandoObject();
            IQueryable query = _defaultHelper.BuildDynamicQuery(_dbContext, entityType, searchType, searchObject);
            if (page == 1)
            {
                pageDto.totalCount = await _defaultHelper.ExecuteCountQuery(query, entityType);
            }
            var isIncludeType = _defaultHelper.HasPasteIncludeProperties(entityType);
            if (isIncludeType)
            {
                var includeQuery = _defaultHelper.ApplyAllPasteIncludes(query, entityType);
                if (includeQuery != null)
                {
                    query = includeQuery as IQueryable;
                }
            }
            query = _defaultHelper.ApplyPagingAndSorting(query, entityType, searchType, searchObject);
            List<object> dtos;
            if (isIncludeType)
            {
                dtos = await _defaultHelper.ExecuteProjectionQueryWithIncludes(query, entityType, listDtoType);
            }
            else
            {
                dtos = await _defaultHelper.ExecuteProjectionQuery(query, entityType, listDtoType);
            }
            if (dtos == null) { return null; }
            if (dtos?.Any() == false)
            {
                return null;
            }
            //查询外表 并实现 这里是简单的外表ID的查询 如果有更复杂的，还需要实现
            await _defaultHelper.BuildOuterData(_dbContext, listDtoType, dtos);

            //进行脱敏处理
            var hasHalfHideAttribute = listDtoType.GetCustomAttribute<PasteHalfHideAttribute>();
            if (hasHalfHideAttribute != null)
            {
                HalfHideHelper.PasteMaskData(dtos, listDtoType);
            }

            // 7. 设置结果并返回
            //pagedDto.Items = dtos.Cast<object>().ToList();
            pageDto.items = dtos;
            //计算ExtendBind的数据... ExtendBind=true
            await _defaultHelper.ReadExtendBind(_dbContext, listDtoType, dtos, searchType, searchObject);
            return pageDto;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="inputString"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<dynamic> CreateItemAsync(string entityName, string inputString)
        {
            entityName = DefaultMapperEntityName(entityName);
            // 获取实体类型和添加DTO类型
            var entityType = _defaultHelper.FindEntityType(entityName);
            var addDtoType = _defaultHelper.FindDtoType(entityName, "AddDto");

            if (entityType == null || addDtoType == null)
            {
                throw new PasteCodeException($"找不到对应的实体或添加DTO类型: {entityName}", 404);
            }
            //using var read = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
            //var inputString = await read.ReadToEndAsync();

            //Console.WriteLine(inputString);

            // 反序列化输入数据到添加DTO
            //var addDto = JsonConvert.DeserializeObject(inputString, addDtoType);
            //var addDto = Mapster.TypeAdapter(inputString,addDtoType);
            var addDto = JsonSerializer.Deserialize(inputString, addDtoType, JsonConfig.Options);
            _defaultHelper.ValidateModel(addDto, addDtoType);

            var mutexFind = await _defaultHelper.FilterFirstOrDefaultAsync(_dbContext, entityType, addDtoType, addDto, null);
            if (mutexFind != null && mutexFind != default)
            {
                throw new PasteCodeException("已经有相同的数据了，修改后重试!");
            }
            // 创建实体实例
            var entity = Activator.CreateInstance(entityType);

            // 映射DTO到实体
            _defaultHelper.MapProperties(addDto, entity, addDtoType, entityType);

            //ObjectMapper.Map<UserInfoAddDto,UserInfo>(null,null);

            await DefaultDataSetExtendAsync(entityType, entity, "add");
            //下面这个和上面注释的意思一样

            await _defaultHelper.CheckEntityShortData(_dbContext, entityType, entity);

            // 添加到数据库
            var dbSet = _defaultHelper.FindDbSetObject(_dbContext, entityType);
            dbSet.GetType().GetMethod("Add").Invoke(dbSet, new[] { entity });

            // 保存更改
            await _dbContext.SaveChangesAsync();

            await DefaultAfterSaveAsync(entityType, entity, addDto, "add");

            var idProperty = _defaultHelper.ReadCurrentProperty(entityType, "Id");
            if (idProperty != null && idProperty != default)
            {
                var idVal = idProperty.GetValue(entity).ToString();

                return await ReadListDtoByIdAsync(entityType, entityName, idVal);
            }
            return 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="inputString"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<object> UpdateItemAsync(string entityName, string inputString)
        {
            entityName = DefaultMapperEntityName(entityName);
            // 获取实体类型和更新DTO类型
            var entityType = _defaultHelper.FindEntityType(entityName);
            var updateDtoType = _defaultHelper.FindDtoType(entityName, "UpdateDto");

            if (entityType == null || updateDtoType == null)
            {
                throw new PasteCodeException($"找不到对应的实体或更新DTO类型: {entityName}", 404);
            }

            //可以执行归属校验 entityName entityType

            //using var read = new StreamReader(_httpContext.Request.Body, Encoding.UTF8);
            //var inputString = await read.ReadToEndAsync();
            // 反序列化输入数据到更新DTO
            //var updateDto = JsonConvert.DeserializeObject(inputString, updateDtoType);
            var updateDto = JsonSerializer.Deserialize(inputString, updateDtoType, JsonConfig.Options);

            // 手动执行模型验证
            _defaultHelper.ValidateModel(updateDto, updateDtoType);

            // 获取ID属性值
            var id = _defaultHelper.GetPropertyValue(updateDto, "Id");
            if (id == null)
            {
                throw new PasteCodeException("更新DTO必须包含Id属性", 400);
            }

            var mutexFind = await _defaultHelper.FilterFirstOrDefaultAsync(_dbContext, entityType, updateDtoType, updateDto, id);
            if (mutexFind != null && mutexFind != default)
            {
                throw new PasteCodeException("已经有相同的数据了，修改后重试!");
            }
            // 查找实体
            var entity = await _defaultHelper.FindEntityByIdAsync(_dbContext, entityType, id);
            if (entity == null)
            {
                throw new PasteCodeException("需要更新的记录不存在", 404);
            }
            await DefaultFilterDataAsync(entityType, entity, "update");
            // 映射DTO到实体
            _defaultHelper.MapProperties(updateDto, entity, updateDtoType, entityType);

            await DefaultDataSetExtendAsync(entityType, entity, "update");

            await _defaultHelper.CheckEntityShortData(_dbContext, entityType, entity);

            // 保存更改
            await _dbContext.SaveChangesAsync();

            await DefaultAfterSaveAsync(entityType, entity, updateDto, "update");

            return await ReadListDtoByIdAsync(entityType, entityName, id);
        }

        /// <summary>
        /// 用于返回ListDto单项数据
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        private async Task<object> ReadItemListDtoAsync(string entityName, object id)
        {
            entityName = DefaultMapperEntityName(entityName);

            // 1. 根据实体名称查找对应的实体类型和 DTO 类型
            var entityType = _defaultHelper.FindEntityType(entityName);
            var dtoType = _defaultHelper.FindDtoType(entityName, "ListDto");

            if (entityType == null || dtoType == null)
            {
                throw new PasteCodeException($"找不到对应的实体或 DTO 类型: {entityName}");
            }

            // 2. 动态查询实体
            //var entity = await _defaultHelper.FindEntityByIdAsync(_dbContext, entityType, id);
            var entity = await _defaultHelper.FindEntityByIdWithIncludeAsync(_dbContext, entityType, id);
            if (entity == null)
            {
                return null;//throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            }

            //await FilterDataAsync(entityType, entity, "detail");
            var dto = Activator.CreateInstance(dtoType);
            _defaultHelper.MapProperties(entity, dto, entityType, dtoType);
            // 3. 动态映射实体到 DTO
            //var dto = DefaultHelper.MapEntityToDto(ObjectMapper, entity, entityType, dtoType);

            //构建外表等 Include的如何搞定？？？
            await _defaultHelper.BuildOuterQuery(_dbContext, dto, dtoType);

            // 4. 动态读取 DTO 的属性信息
            //var dataModel = DefaultHelper.ReadModelProperties(dto, dtoType);
            //return dataModel;
            //return await this.DefaultVoloModelPropertyAsync("detail", dto, dtoType);
            return dto;

        }

        /// <summary>
        /// 基于ID获取对象ListDto用于新增或更新后同步到Table的UI中
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        private async Task<object> ReadListDtoByIdAsync(Type entityType, string entityName, object id)
        {
            entityName = DefaultMapperEntityName(entityName);
            var dtoType = _defaultHelper.FindDtoType(entityName, "ListDto");
            if (entityType == null || dtoType == null)
            {
                throw new PasteCodeException($"找不到对应的实体或 DTO 类型: {entityName}");
            }
            var entity = await _defaultHelper.FindEntityByIdWithIncludeAsync(_dbContext, entityType, id);
            if (entity == null)
            {
                return null;//throw new PasteCodeException("查询的信息不存在，无法执行编辑操作！");
            }
            var dto = Activator.CreateInstance(dtoType);
            _defaultHelper.MapProperties(entity, dto, entityType, dtoType);
            await _defaultHelper.BuildOuterQuery(_dbContext, dto, dtoType);
            return dto;
        }

        /// <summary>
        /// 通用状态更新方法
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<string> UpdateState(string entityName, InputQueryUpdateState input)
        {
            entityName = DefaultMapperEntityName(entityName);
            // 获取实体类型
            var entityType = _defaultHelper.FindEntityType(entityName);

            if (entityType == null)
            {
                throw new PasteCodeException($"找不到对应的实体类型: {entityName}", 404);
            }
            if (input.name != "extendBind")
            {
                // 查找实体
                var entity = await _defaultHelper.FindEntityByIdAsync(_dbContext, entityType, input.id);
                if (entity == null)
                {
                    throw new PasteCodeException("没有找到对应的操作对象，无法继续执行", 404);
                }

                await DefaultFilterDataAsync(entityType, entity, "state");

                var property = _defaultHelper.GetPropertyThrowException(entityType, input.name);// entityType.GetProperties().Where(x => x.Name == input.name || x.Name == pauseName).FirstOrDefault();

                if (!property.CanWrite)
                {
                    throw new PasteCodeException($"属性 {input.name} 不可写", 400);
                }
                // 处理可空布尔类型
                if (property.PropertyType == typeof(bool?))
                {
                    property.SetValue(entity, input.state ? (bool?)true : (bool?)false);
                }
                else
                {
                    property.SetValue(entity, input.state);
                }
                // 保存更改
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var listDtoType = _defaultHelper.FindDtoType(entityName, "ListDto");
                if (listDtoType == null || listDtoType == default)
                {
                    throw new PasteCodeException($"没有找到这个实体{entityName}的ListDto数据模型!");
                }
                var property = listDtoType.GetProperty("ExtendBind");
                if (property == null || property == default)
                {
                    throw new PasteCodeException("没有找到这个字段ExtendBind的信息");
                }

                var attrs = property.GetCustomAttributes<PasteExtendBindAttribute>();
                if (attrs?.Any() == false)
                {
                    throw new PasteCodeException($"请为{entityName}ListDto的字段ExtendBind配置PasteExtendBind特性后再试,支持绑定多个!");
                }

                foreach (var attr in attrs)
                {
                    var bindEntity = _defaultHelper.FindEntityType(attr.Args1);
                    if (bindEntity == null || bindEntity == default)
                    {
                        throw new PasteCodeException($"PasteExtendBind的args1:{attr.Args1} 配置错误，没有这样的Entity信息!");
                    }
                    //Console.WriteLine($"准备计算Bind的逻辑... .. .{attr.Args1} {attr.Args2} {attr.Args3} {attr.Args4} ");
                    var args = input.search.ToQuerys();
                    if (args.TryGetValue(attr.Args4, out var grade_id))
                    {
                        var dics = new Dictionary<string, string>();
                        dics.Add(attr.Args3, grade_id);
                        dics.Add(attr.Args2, input.id);

                        var find_bind = await _defaultHelper.FindFirstOrDefaultAsync(_dbContext, bindEntity, dics, default);
                        if (find_bind == null || find_bind == default)
                        {
                            if (input.state)
                            {
                                //新增
                                var bind_entity = Activator.CreateInstance(bindEntity);

                                //为GradeId RoleId赋值
                                foreach (var arg in dics)
                                {
                                    //var arg_pro = bindEntity.GetProperty(arg.Key, BindingFlags.Public | BindingFlags.Instance);
                                    var argPro = _defaultHelper.ReadCurrentProperty(bindEntity, arg.Key);
                                    if (argPro == null || argPro == default)
                                    {
                                        throw new PasteCodeException($"没有在Entity:{attr.Args1} 获取到字段:{arg.Key}");
                                    }
                                    var arg_val = Convert.ChangeType(arg.Value, argPro.PropertyType);
                                    argPro.SetValue(bind_entity, arg_val);
                                }

                                // 添加到数据库
                                var dbSet = _defaultHelper.FindDbSetObject(_dbContext, bindEntity);
                                dbSet.GetType().GetMethod("Add").Invoke(dbSet, new[] { bind_entity });
                                // 保存更改
                                await _dbContext.SaveChangesAsync();
                                //Console.WriteLine("执行Bind的新增逻辑... .. .");
                            }
                            else
                            {
                                //Console.WriteLine("数据不存在，且指令是取消绑定");
                            }
                        }
                        else
                        {
                            if (!input.state)
                            {
                                // 执行删除操作
                                //var dbSet = _defaultHelper.FindDbSetObject(_dbContext, bindEntity);
                                //dbSet.GetType().GetMethod("Remove").Invoke(dbSet, new[] { find_bind });
                                _dbContext.Remove(find_bind);
                                await _dbContext.SaveChangesAsync();
                                //Console.WriteLine("执行Bind的删除逻辑... .. .");
                            }
                            else
                            {
                                //Console.WriteLine("数据存在，且指令是绑定数据");
                            }
                        }
                    }
                }

            }

            await DefaultAfterUpdateStateAsync(entityName, input);
            return "操作成功";
        }

        /// <summary>
        /// 通用删除 遵照ListDto的PasteDisable特性
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="PasteCodeException"></exception>
        public async Task<string> Remove(string entityName, string id)
        {
            entityName = DefaultMapperEntityName(entityName);
            // 获取实体类型
            var entityType = _defaultHelper.FindEntityType(entityName);

            if (entityType == null)
            {
                throw new PasteCodeException($"找不到对应的实体类型: {entityName}", 404);
            }
            #region 是否有标记禁止删除
            var listDtoType = _defaultHelper.FindDtoType(entityName, "ListDto");
            if (listDtoType != null)
            {
                var finds = listDtoType.GetCustomAttributes<PasteEnableAttribute>();
                if (finds?.Any() == true)
                {
                    var find = finds.FirstOrDefault();
                    if (find.Args3 == "false")
                    {
                        throw new PasteCodeException($"实体 {entityName} 不允许删除操作", 403);
                    }
                }
            }
            #endregion

            // 查找实体
            var entity = await _defaultHelper.FindEntityByIdAsync(_dbContext, entityType, id);
            if (entity == null)
            {
                throw new PasteCodeException("需要删除的记录不存在", 404);
            }
            await DefaultFilterDataAsync(entityType, entity, "remove");
            // 执行删除操作
            //var dbSet = _defaultHelper.FindDbSetObject(_dbContext, entityType);
            //dbSet.GetType().GetMethod("Remove").Invoke(dbSet, new[] { entity });
            _dbContext.Remove(entity);
            // 保存更改
            await _dbContext.SaveChangesAsync();

            await DefaultAfterRemoveAsync(entityName, id);

            return "删除成功";
        }


        #endregion

        #region 以下是数据过滤和自动赋值模块

        ///// <summary>
        ///// 删除后 调用
        ///// </summary>
        //public Func<(string entityName, string id), Task> FuncAfterRemove { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual async Task DefaultAfterRemoveAsync(string entityName, string id)
        {
            //if (FuncAfterRemove != null)
            //{
            //    await FuncAfterRemove((entityName, id));
            //}
        }

        ///// <summary>
        ///// 读取数据后的权限判断，注意entityObject可能为null
        ///// </summary>
        //public Func<(string actionName, Type entityType, object entityObject), Task> FuncFilterData { get; set; } = null;

        /// <summary>
        /// 权限判断 就是这个
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="obj">可能为null比如add的时候</param>
        /// <param name="action">add/update/remove/state/page/detail</param>
        /// <returns></returns>
        public virtual async Task DefaultFilterDataAsync(Type entityType, object obj, string action)
        {

        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public Func<(string entityName, InputQueryUpdateState input), Task> FuncAfterUpdateState { get; set; } = null;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public virtual async Task DefaultAfterUpdateStateAsync(string entityName, InputQueryUpdateState input)
        {
            //if (FuncAfterUpdateState != null)
            //{
            //    await FuncAfterUpdateState((entityName, input));
            //}
        }


        /// <summary>
        /// 递归查找基类方法的辅助方法
        /// </summary>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="bindingAttr"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        private MethodInfo FindMethodInBaseTypes(Type type, string methodName, BindingFlags bindingAttr, Type[] parameterTypes)
        {
            if (type == null)
            {
                return null;
            }
            //Console.WriteLine($"{type.Name} {type.FullName}");
            // 尝试在当前类型中查找方法
            //var method = type.GetMethod(methodName, bindingAttr, null, parameterTypes, null);
            var method = type.GetMethod(methodName, bindingAttr);

            if (method != null)
                return method;

            // 递归查找父类
            return FindMethodInBaseTypes(type.BaseType, methodName, bindingAttr, parameterTypes);
        }

        /// <summary>
        /// 新增和更新前 比如一些特殊字段的处理
        /// </summary>
        public Func<(string actionName, Type entityType, object entityObject), Task> FuncDataSet { get; set; } = null;

        /// <summary>
        /// 为Entity赋值，比如赋值UserId等 基类需要定义SetDataExtendAsync(Type,Entity,action)
        /// 一般是一些默认字段输入，比如创建者
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="obj"></param>
        /// <param name="action">add或update</param>
        /// <returns></returns>
        public virtual async Task DefaultDataSetExtendAsync(Type entityType, object obj, string action)
        {

        }

        /// <summary>
        /// 数据保存后 比如新增 比如更新
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="entityObject"></param>
        /// <param name="inputObject"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public virtual async Task DefaultAfterSaveAsync(Type entityType, object entityObject, object inputObject, string action)
        {

        }

        ///// <summary>
        ///// 
        ///// </summary>
        //public Func<(string actionName, Type dtoType, object dtoObject), Task<VoloModelInfo>> FuncBuildProperty { get; set; } = null;

        /// <summary>
        /// 构建元数据返回给前端
        /// </summary>
        /// <param name="action"></param>
        /// <param name="dto"></param>
        /// <param name="dtoType"></param>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public virtual async Task<VoloModelInfo> DefaultBuildPropertyInfo(string action, object dto, Type dtoType, bool listModel = false)
        {
            //if (FuncBuildProperty != null)
            //{
            //    return await FuncBuildProperty((action, dtoType, dto));
            //}
            if (!listModel)
            {
                if (dtoType.Name.EndsWith("ListDto"))
                {
                    listModel = true;
                }
            }
            var _model = BuilderHelper.AttributeReadModelProperty(dto, dtoType, listModel);
            return await Task.FromResult(_model);
        }

        #endregion


    }

}
