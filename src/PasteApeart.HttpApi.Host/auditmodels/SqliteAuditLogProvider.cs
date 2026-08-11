using PasteApeart.auditmodels;

namespace PasteApeart.HttpApi.Host.auditmodels
{
    /// <summary>
    /// 入库 入库
    /// </summary>
    public class SqliteAuditLogProvider : IAuditLogProvider
    {
        private readonly MyAuditingDb _dbContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbContext"></param>
        public SqliteAuditLogProvider(MyAuditingDb dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        public async Task InsertAsync(ApAuditLog auditLog)
        {
            var logId = Guid.NewGuid().ToString("N");
            auditLog.LogId = logId;
            auditLog.EntityChanges?.ForEach(entity =>
            {
                entity.LogId = logId;
                entity.PropertyChanges?.ForEach(property => property.LogId = logId);
            });

            //Console.WriteLine(JsonSerializer.Serialize(auditLog));
            _dbContext.Add(auditLog);
            await _dbContext.SaveChangesAsync();
        }
    }

}
