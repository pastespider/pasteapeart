using $ext_safeprojectname$.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PasteApeart.Application;
using PasteApeart.Application.Contracts;

namespace $ext_safeprojectname$.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class $ext_safeprojectname$DefaultAppService : PasteDefaultAppService
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultHelper"></param>
        /// <param name="dbContext"></param>
        public $ext_safeprojectname$DefaultAppService(IPasteDefaultHelper defaultHelper, I$ext_safeprojectname$DbContext dbContext) : base(defaultHelper, dbContext as DbContext)
        {
            //用于自定义实现一些默认接口的动作，比如写入前检查，绑定后检查等！！！
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Task DefaultAfterUpdateStateAsync(string entityName, InputQueryUpdateState input)
        {
            return base.DefaultAfterUpdateStateAsync(entityName, input);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="action"></param>
        /// <param name="dto"></param>
        /// <param name="dtoType"></param>
        /// <param name="listModel"></param>
        /// <returns></returns>
        public override Task<PasteApeart.Helper.VoloModelInfo> DefaultBuildPropertyInfo(string action, object dto, Type dtoType, bool listModel = false)
        {
            return base.DefaultBuildPropertyInfo(action, dto, dtoType, listModel);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public override Task DefaultDataSetExtendAsync(Type entityType, object obj, string action)
        {
            return base.DefaultDataSetExtendAsync(entityType, obj, action);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public override Task DefaultFilterDataAsync(Type entityType, object obj, string action)
        {
            return base.DefaultFilterDataAsync(entityType, obj, action);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <returns></returns>
        public override string DefaultMapperEntityName(string entityName)
        {
            return base.DefaultMapperEntityName(entityName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public override Task DefaultRemoveAsync(string entityName, string id)
        {
            return base.DefaultRemoveAsync(entityName, id);
        }

    }
}
