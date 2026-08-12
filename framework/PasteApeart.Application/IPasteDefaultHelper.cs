using Microsoft.EntityFrameworkCore;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.Reflection;

namespace PasteApeart.Application
{
    public interface IPasteDefaultHelper
    {
        static abstract Task<int> ExecuteCountQuery(IQueryable query, Type entityType);
        static abstract VoloModelInfo ReadModelProperties(object model, Type modelType, bool IsListModel = false);
        static abstract void ValidateModel(object model, Type modelType);
        object ApplyAllPasteIncludes(object query, Type entityType);
        IQueryable ApplyPagingAndSorting(IQueryable query, Type entityType, Type searchType, object input);
        Type AutoFindType(string typeName);
        IQueryable BuildDynamicQuery(DbContext _dbContext, Type entityType, Type searchType, object input);
        Task BuildOuterData(DbContext _dbContext, Type listDtoType, List<object> dtos);
        Task BuildOuterQuery(DbContext _dbContext, object dto, Type dtoType);
        Task CheckEntityShortData(DbContext _dbContext, Type entityType, object entity);
        Task<List<object>> ExecuteProjectionQuery(IQueryable query, Type entityType, Type dtoType);
        Task<List<object>> ExecuteProjectionQueryWithIncludes(IQueryable query, Type entityType, Type dtoType);
        Task<object> FilterFirstOrDefaultAsync(DbContext _dbContext, Type entityType, Type dtoType, object dtoValue, object id, CancellationToken cancellationToken = default);
        object FindDbSetObject(DbContext _dbContext, Type entityName);
        Type FindDtoType(string entityName, string suffix);
        Task<object> FindEntityByIdAsync(DbContext _dbContext, Type entityType, object id, CancellationToken cancellationToken = default);
        Task<object> FindEntityByIdWithIncludeAsync(DbContext _dbContext, Type entityType, object id, CancellationToken cancellationToken = default);
        Type FindEntityType(string entityName);
        Task<object> FindFirstOrDefaultAsync(DbContext _dbContext, Type entityType, Dictionary<string, string> args, CancellationToken cancellationToken = default);
        Type FindQueryType(string entityName);
        void ForeachMapProperties(object source, object destination, Type sourceType, Type destinationType);
        PropertyInfo GetPropertyThrowException(Type entityType, string propertyName);
        object GetPropertyValue(object obj, string propertyName);
        bool HasAttributeType(Type entityType, Type attributeType);
        bool HasPasteIncludeProperties(Type entityType);
        InputSearchBase MapperToQueryModel(string _search, Type searchType, object search);
        void MapProperties(object source, object destination, Type sourceType, Type destinationType);
        PropertyInfo ReadCurrentProperty(Type entityType, string propertyName);
        Task ReadExtendBind(DbContext _dbContext, Type listDtoType, List<object> dataList, Type searchType, object searchModel);
        Task<object> ReadSingleListDtoByIdAsync(DbContext _dbContext, string entityName, object id);
    }
}