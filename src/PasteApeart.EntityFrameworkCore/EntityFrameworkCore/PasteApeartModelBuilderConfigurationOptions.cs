namespace PasteApeart
{
    /// <summary>
    /// 配置表前缀 Schema等 按需配置
    /// </summary>
    public class PasteApeartModelBuilderConfigurationOptions
    {


        /// <summary>
        /// 
        /// </summary>
        /// <param name="tablePrefix"></param>
        /// <param name="schema"></param>
        public PasteApeartModelBuilderConfigurationOptions(
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