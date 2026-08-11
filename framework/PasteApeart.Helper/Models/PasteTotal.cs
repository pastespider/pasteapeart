namespace PasteApeart.Helper.Models
{
    /// <summary>
    /// 
    /// </summary>
    public class PasteTotalItem
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Value { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PasteResponse<T> where T : class
    {
        /// <summary>
        /// 当页数据
        /// </summary>
        public List<T> Items { get; set; }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// 页大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 统计 有统计信息的时候返回适用,也可以标记PasteSum由前端统计当前页,这个一般用于条件综合查询，后端处理
        /// </summary>
        public List<PasteTotalItem> PasteTotal { get; set; }

        /// <summary>
        /// 数据模型 一并返回的时候适用
        /// </summary>
        public VoloModelInfo PasteModel { get; set; }
    }
}
