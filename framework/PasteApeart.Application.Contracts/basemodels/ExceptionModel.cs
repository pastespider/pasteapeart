namespace PasteApeart.Application.Contracts
{
    /// <summary>
    /// 异常等返回的消息体
    /// </summary>
    public class ExceptionModel
    {
        /// <summary>
        /// StatusCode
        /// </summary>
        public string code { get; set; }

        /// <summary>
        /// 请求是否符合预期
        /// </summary>
        public bool success { get; set; }

        /// <summary>
        /// 附带的消息
        /// </summary>
        public string message { get; set; }
    }
}
