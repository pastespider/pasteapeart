using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteApeart
{
    /// <summary>
    /// 链接信息
    /// </summary>
    public class ConnectionConfig
    {
        /// <summary>
        /// 
        /// </summary>
        public string MainConnectionString { get; set; }

        /// <summary>
        /// sql的主链接
        /// </summary>
        public string SqliteConnectionString { get; set; }

        /// <summary>
        /// 只读链接 sqlite的只读链接
        /// </summary>
        public string SqliteReadOnly { get; set; }

        /// <summary>
        /// 数据库类型 sqlite pgsql mysql sqlserver
        /// </summary>
        public string DataType { get; set; } = "sqlite";

        /// <summary>
        /// 缓存类型 memory redis
        /// </summary>
        public string CacheType { get; set; } = "memory";

    }
}
