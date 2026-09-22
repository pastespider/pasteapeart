using JetBrains.Annotations;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    public class ExampleFormModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <param name="schema"></param>
        public ExampleFormModelBuilderConfigurationOptions(
            [NotNull] string tablePrefix = "",
            [CanBeNull] string schema = null)
            : base(
                tablePrefix,
                schema)
        {

        }
    }
}