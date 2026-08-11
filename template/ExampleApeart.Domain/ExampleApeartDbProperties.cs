namespace $ext_safeprojectname$
{
    /// <summary>
    /// 
    /// </summary>
    public static class $ext_safeprojectname$DbProperties
    {
        /// <summary>
        /// 
        /// </summary>
        public static string DbTablePrefix { get; set; } = "PT";

        /// <summary>
        /// 
        /// </summary>
        public static string DbSchema { get; set; } = null;

        /// <summary>
        /// Pgsql主库
        /// </summary>
        public const string ConnectionStringName = "MainConnectionString";

        /// <summary>
        /// Pgsql从库
        /// </summary>
        public const string ReadonlyConnectionStringName = "MainReadonlyConnectionString";

        /// <summary>
        /// Sqlite主库
        /// </summary>
        public const string SqliteConnectionStringName = "SqliteConnectionString";

        /// <summary>
        /// Sqlite从库
        /// </summary>
        public const string SqliteReadOnlyConnectionStringName = "SqliteReadOnly";

        /// <summary>
        /// 审计日志的数据库链接
        /// </summary>
        public const string AuditingConnectionStringName = "SqliteAuditing";

    }
}
