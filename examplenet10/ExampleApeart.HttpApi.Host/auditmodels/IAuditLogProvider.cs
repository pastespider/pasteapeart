using PasteApeart.auditmodels;

namespace ExampleApeart.HttpApi.Host.auditmodels
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuditLogProvider
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="auditLog"></param>
        /// <returns></returns>
        Task InsertAsync(ApAuditLog auditLog);
    }

}
