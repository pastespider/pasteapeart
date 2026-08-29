namespace ExampleApeart
{
    /// <summary>
    /// 
    /// </summary>
    public static class PublicString
    {
        /// <summary>
        /// Token在Header或者Cookie中的名称
        /// </summary>
        public const string TokenHeadName = "token";

        /// <summary>
        /// 
        /// </summary>
        public const string CacheImageHead = "cache:code:img:";

        /// <summary>
        /// 
        /// </summary>
        public const int TokenTimeOut = 3600 * 24;

        /// <summary>
        /// 注意 注意 这个是重点，如果是模板则用$guid1$这样创建的项目就行新的，或者改成手动赋值！
        /// </summary>
        public const string TokenKey = "6d507da3-e5d7-43b5-8811-ad055c30a7fb";
    }
}
