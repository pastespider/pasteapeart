using PasteApeart.Application.Contracts;
using PasteApeart.Helper;

namespace PasteApeart.Application
{
    public interface IPasteDefaultAppService
    {
        Func<(string entityName, string id), Task> FuncAfterRemove { get; set; }
        Func<(string entityName, InputQueryUpdateState input), Task> FuncAfterUpdateState { get; set; }
        Func<(string actionName, Type dtoType, object dtoObject), Task<VoloModelInfo>> FuncBuildProperty { get; set; }
        Func<(string actionName, Type entityType, object entityObject), Task> FuncDataSet { get; set; }
        Func<(string actionName, Type entityType, object entityObject), Task> FuncFilterData { get; set; }
        Func<string, string> FuncMapperEntityName { get; set; }

        Task<dynamic> CreateItemAsync(string entityName, string inputString);
        Task DefaultAfterUpdateStateAsync(string entityName, InputQueryUpdateState input);
        Task<VoloModelInfo> DefaultBuildPropertyInfo(string action, object dto, Type dtoType, bool listModel = false);
        Task DefaultDataSetExtendAsync(Type entityType, object obj, string action);
        Task DefaultFilterDataAsync(Type entityType, object obj, string action);
        string DefaultMapperEntityName(string entityName);
        Task DefaultRemoveAsync(string entityName, string id);
        Task<dynamic> Page(string entityName, string queryString, string searchName = "");
        Task<VoloModelInfo> ReadAddModel(string entityName);
        Task<VoloModelInfo> ReadDetailModel(string entityName, string id);
        Task<VoloModelInfo> ReadListModel(string entityName, string searchName = "");
        Task<VoloModelInfo> ReadUpdateModel(string entityName, string id);
        Task<string> Remove(string entityName, string id);
        Task<object> UpdateItemAsync(string entityName, string inputString);
        Task<string> UpdateState(string entityName, InputQueryUpdateState input);
    }
}