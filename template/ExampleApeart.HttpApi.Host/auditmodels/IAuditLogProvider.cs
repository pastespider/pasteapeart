using PasteApeart.auditmodels;

namespace $ext_safeprojectname$.HttpApi.Host.auditmodels
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
