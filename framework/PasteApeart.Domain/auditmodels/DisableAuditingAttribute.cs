namespace PasteApeart
{
    /// <summary>
    /// 禁用日志收集
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Property)]
    public class DisableAuditingAttribute : Attribute
    {

    }

}
