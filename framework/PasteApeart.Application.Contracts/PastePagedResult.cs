namespace PasteApeart.Application.Contracts
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class PastePagedResult<T>
    {
        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 数据集
        /// </summary>
        public IReadOnlyList<T> Items { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 页大小 用于特殊情况下修改UI端，比如显示当前数据，最多31，也就是一页！保持一页
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public PastePagedResult()
        {

        }
    }
}
