using PasteApeart.Application.Contracts;
using PasteApeart.Helper;

namespace PasteApeart.Application
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPasteDefaultAppService
    {
        //Func<(string entityName, string id), Task> FuncAfterRemove { get; set; }
        //Func<(string entityName, InputQueryUpdateState input), Task> FuncAfterUpdateState { get; set; }
        //Func<(string actionName, Type dtoType, object dtoObject), Task<VoloModelInfo>> FuncBuildProperty { get; set; }
        //Func<(string actionName, Type entityType, object entityObject), Task> FuncDataSet { get; set; }
        //Func<(string actionName, Type entityType, object entityObject), Task> FuncFilterData { get; set; }
        //Func<string, string> FuncMapperEntityName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="inputString"></param>
        /// <returns></returns>
        Task<dynamic> CreateItemAsync(string entityName, string inputString);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task DefaultAfterUpdateStateAsync(string entityName, InputQueryUpdateState input);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="dto"></param>
        /// <param name="dtoType"></param>
        /// <param name="listModel"></param>
        /// <returns></returns>
        Task<VoloModelInfo> DefaultBuildPropertyInfo(string action, object dto, Type dtoType, bool listModel = false);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        Task DefaultDataSetExtendAsync(Type entityType, object obj, string action);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        Task DefaultFilterDataAsync(Type entityType, object obj, string action);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        string DefaultMapperEntityName(string entityName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task DefaultAfterRemoveAsync(string entityName, string id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="queryString"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        Task<dynamic> Page(string entityName, string queryString, string searchName = "");

        /// <summary>
        /// 复用 按页查询
        /// </summary>
        /// <param name="searchObject"></param>
        /// <param name="entityType"></param>
        /// <param name="searchType"></param>
        /// <param name="input"></param>
        /// <param name="listDtoType"></param>
        /// <returns></returns>
        Task<dynamic> DirectPageAsync(object searchObject, Type entityType, Type searchType, InputSearchBase input, Type listDtoType);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        Task<VoloModelInfo> ReadAddModel(string entityName);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VoloModelInfo> ReadDetailModel(string entityName, string id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="searchName"></param>
        /// <returns></returns>
        Task<VoloModelInfo> ReadListModel(string entityName, string searchName = "");
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<VoloModelInfo> ReadUpdateModel(string entityName, string id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> Remove(string entityName, string id);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="inputString"></param>
        /// <returns></returns>
        Task<object> UpdateItemAsync(string entityName, string inputString);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        Task<string> UpdateState(string entityName, InputQueryUpdateState input);
    }
}