using Volo.Abp.Application;
using Volo.Abp.Authorization;
using Volo.Abp.Modularity;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    [DependsOn(typeof(AbpDddApplicationContractsModule))]
    public class ExampleFormApplicationContractsModule : AbpModule
    {

    }
}
