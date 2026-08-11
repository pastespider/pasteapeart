using System.ComponentModel;

namespace PasteApeart.Application.Contracts
{
    /// <summary>
    /// 异常分类
    /// </summary>
    public enum ExceptionCode
    {
        /// <summary>
        /// 提示性错误
        /// </summary>
        ShowInfo = 700,
        /// <summary>
        /// 输入错误
        /// </summary>
        ModelErr = 400,
        /// <summary>
        /// 未登录或者密钥错误
        /// </summary>
        UnAuth = 401,
        /// <summary>
        /// 没有权限
        /// </summary>
        UnRole = 403,
        /// <summary>
        /// 没有数据
        /// </summary>
        NotFound=404
    }
}
