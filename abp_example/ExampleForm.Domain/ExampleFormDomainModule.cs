using System;
using Volo.Abp.Domain;
using Volo.Abp.Modularity;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    [DependsOn(
        typeof(AbpDddDomainModule)
    )]
    public class ExampleFormDomainModule : AbpModule
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void ConfigureServices(ServiceConfigurationContext context)
        {
            //Console.WriteLine("---ExampleFormDomainModule.ConfigureServices---");

        }
    }
}
