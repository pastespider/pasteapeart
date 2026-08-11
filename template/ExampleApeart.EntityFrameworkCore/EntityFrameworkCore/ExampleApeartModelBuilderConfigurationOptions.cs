namespace $ext_safeprojectname$
{
    /// <summary>
    /// 配置表前缀 Schema等 按需配置
    /// </summary>
    public class $ext_safeprojectname$ModelBuilderConfigurationOptions
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <param name="schema"></param>
        public $ext_safeprojectname$ModelBuilderConfigurationOptions(
            string tablePrefix = "",
            string schema = null)
        {
            TablePrefix = tablePrefix;
            Schema = schema;
        }

        /// <summary>
        /// 
        /// </summary>
        public string TablePrefix { get; set; } = "";

        /// <summary>
        /// 
        /// </summary>
        public string Schema { get; set; } = null;
    }
}