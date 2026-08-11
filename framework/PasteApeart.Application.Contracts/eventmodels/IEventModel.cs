namespace PasteApeart.Application.Contracts
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEventModel
    {
        /// <summary>
        /// 延迟 秒
        /// </summary>
        long ExpireSecond { get; set; }

        /// <summary>
        /// 截止 时间戳秒
        /// </summary>
        long ExpireTime { get; set; }

        /// <summary>
        /// 对象名称
        /// </summary>
        string EventName { get; set; }
    }


}
