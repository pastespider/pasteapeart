using ExampleApeart.Application.Contracts.eventmodels;
using ExampleApeart.EntityFrameworkCore;
using ExampleApeart.usermodels;
using Microsoft.EntityFrameworkCore;
using PasteApeart;
using PasteApeart.Application;
using PasteApeart.Application.Contracts;
using PasteApeart.Handler;

namespace ExampleApeart.Application
{
    /// <summary>
    /// 
    /// </summary>
    public class ExampleApeartDefaultService : PasteDefaultAppService
    {
        private readonly IChannelHelper _channelHelper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defaultHelper"></param>
        /// <param name="channelHelper"></param>
        /// <param name="dbContext"></param>
        public ExampleApeartDefaultService(IPasteDefaultHelper defaultHelper, IChannelHelper channelHelper, IExampleApeartDbContext dbContext) : base(defaultHelper, dbContext as DbContext)
        {
            //用于自定义实现一些默认接口的动作，比如写入前检查，绑定后检查等！！！
            _channelHelper = channelHelper;
        }

        /// <summary>
        /// 修改状态后 包含 绑定
        /// </summary>
        /// <param name="entityName"></param>
        /// <param name="input"></param>
        /// <returns></returns>
        public override Task DefaultAfterUpdateStateAsync(string entityName, InputQueryUpdateState input)
        {
            return base.DefaultAfterUpdateStateAsync(entityName, input);

        }

        /// <summary>
        /// 生成元数据
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
        /// 数据赋值后，主要用于比如是否可以编辑，是否可以新增等，也可以做默认数据的输入
        /// </summary>
        /// <param name="entityType"></param>
        /// <param name="obj"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        public override async Task DefaultDataSetExtendAsync(Type entityType, object obj, string action)
        {
            //return base.DefaultDataSetExtendAsync(entityType, obj, action);
            if (action != "update")
            {
                return;
            }
            var entityName = entityType.Name;
            if (string.Equals(entityName, nameof(GradeInfo), StringComparison.OrdinalIgnoreCase))
            {
                await _channelHelper.WriteAutoQueueAsync(new EventDemoModel { });

            }
            else
            {
                Console.WriteLine($"{entityName} {action}");
            }
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
        /// 自定义名称，比如UserInfo你要对外接口为member就在这里配置
        /// </summary>
        /// <param name="entityName">管理端输入的TableName</param>
        /// <returns>Dbset的Name</returns>
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
        public override Task DefaultAfterRemoveAsync(string entityName, string id)
        {
            return base.DefaultAfterRemoveAsync(entityName, id);
        }

    }
}
