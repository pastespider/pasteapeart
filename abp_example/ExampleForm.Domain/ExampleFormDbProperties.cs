namespace ExampleForm
{
    /// <summary>
    /// 
    /// </summary>
    public static class ExampleFormDbProperties
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

    }
}
