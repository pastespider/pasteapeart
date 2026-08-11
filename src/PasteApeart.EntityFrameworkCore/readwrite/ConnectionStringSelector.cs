namespace PasteApeart.EntityFrameworkCore
{
    /// <summary>
    /// 返回当前上下文的链接串名称，注意是名称，不是链接字符串
    /// </summary>
    public class ConnectionStringSelector : IConnectionStringSelector
    {
        ///// <summary>
        ///// 
        ///// </summary>
        //private string _connectionStringName = PasteApeartDbProperties.SqliteConnectionStringName;

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns></returns>
        //[Obsolete("use readonly")]
        //public string GetConnectionStringName() => _connectionStringName;

        /// <summary>
        /// 只读
        /// </summary>
        private bool _readOnly = false;

        /// <summary>
        /// 是否只读
        /// </summary>
        /// <returns></returns>
        public bool IsReadOnly() => _readOnly;

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="name"></param>
        //[Obsolete("use readonly")]
        //public void SetConnectionStringName(string name) => _connectionStringName = name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isread"></param>
        public void SetReadOnly(bool isread) => _readOnly = isread;
    }
}
