using Microsoft.EntityFrameworkCore;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.Reflection;

namespace PasteApeart.Application
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPasteDefaultHelper
    {
        //static abstract Task<int> ExecuteCountQuery(IQueryable query, Type entityType);
        //static abstract VoloModelInfo ReadModelProperties(object model, Type modelType, bool IsListModel = false);
        //static abstract void ValidateModel(object model, Type modelType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        Task<int> ExecuteCountQuery(IQueryable query, Type entityType);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelType"></param>
        /// <param name="IsListModel"></param>
        /// <returns></returns>
        VoloModelInfo ReadModelProperties(object model, Type modelType, bool IsListModel = false);
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelType"></param>
        void ValidateModel(object model, Type modelType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <returns></returns>
        object ApplyAllPasteIncludes(object query, Type entityType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <param name="searchType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        IQueryable ApplyPagingAndSorting(IQueryable query, Type entityType, Type searchType, object input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typeName"></param>
        /// <returns></returns>
        Type AutoFindType(string typeName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <param name="searchType"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        IQueryable BuildDynamicQuery(DbContext _dbContext, Type entityType, Type searchType, object input);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="listDtoType"></param>
        /// <param name="dtos"></param>
        /// <returns></returns>
        Task BuildOuterData(DbContext _dbContext, Type listDtoType, List<object> dtos);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="dto"></param>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        Task BuildOuterQuery(DbContext _dbContext, object dto, Type dtoType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task CheckEntityShortData(DbContext _dbContext, Type entityType, object entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        Task<List<object>> ExecuteProjectionQuery(IQueryable query, Type entityType, Type dtoType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <param name="entityType"></param>
        /// <param name="dtoType"></param>
        /// <returns></returns>
        Task<List<object>> ExecuteProjectionQueryWithIncludes(IQueryable query, Type entityType, Type dtoType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <param name="dtoType"></param>
        /// <param name="dtoValue"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<object> FilterFirstOrDefaultAsync(DbContext _dbContext, Type entityType, Type dtoType, object dtoValue, object id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        object FindDbSetObject(DbContext _dbContext, Type entityName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="suffix"></param>
        /// <returns></returns>
        Type FindDtoType(string entityName, string suffix);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<object> FindEntityByIdAsync(DbContext _dbContext, Type entityType, object id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <param name="id"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<object> FindEntityByIdWithIncludeAsync(DbContext _dbContext, Type entityType, object id, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        Type FindEntityType(string entityName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityType"></param>
        /// <param name="args"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<object> FindFirstOrDefaultAsync(DbContext _dbContext, Type entityType, Dictionary<string, string> args, CancellationToken cancellationToken = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        Type FindQueryType(string entityName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        void ForeachMapProperties(object source, object destination, Type sourceType, Type destinationType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        PropertyInfo GetPropertyThrowException(Type entityType, string propertyName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        object GetPropertyValue(object obj, string propertyName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="attributeType"></param>
        /// <returns></returns>
        bool HasAttributeType(Type entityType, Type attributeType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        bool HasPasteIncludeProperties(Type entityType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_search"></param>
        /// <param name="searchType"></param>
        /// <param name="search"></param>
        /// <returns></returns>
        InputSearchBase MapperToQueryModel(string _search, Type searchType, object search);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="sourceType"></param>
        /// <param name="destinationType"></param>
        void MapProperties(object source, object destination, Type sourceType, Type destinationType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        PropertyInfo ReadCurrentProperty(Type entityType, string propertyName);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="listDtoType"></param>
        /// <param name="dataList"></param>
        /// <param name="searchType"></param>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        Task ReadExtendBind(DbContext _dbContext, Type listDtoType, List<object> dataList, Type searchType, object searchModel);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="_dbContext"></param>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<object> ReadSingleListDtoByIdAsync(DbContext _dbContext, string entityName, object id);
    }
}