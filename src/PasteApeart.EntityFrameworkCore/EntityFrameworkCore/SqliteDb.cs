using Microsoft.EntityFrameworkCore;

namespace PasteApeart.EntityFrameworkCore
{
    /// <summary>
    /// 表示使用Sqlite数据库
    /// </summary>
    public class SqliteDb : AbsFilterDbContext<SqliteDb>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="isDesign"></param>
        public SqliteDb(DbContextOptions<SqliteDb> options, bool isDesign = false) : base(options, isDesign) { }
    }


}
