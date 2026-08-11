namespace PasteApeart.Helper
{
    /// <summary>
    /// Enum转Select
    /// </summary>
    public class VoloEnumProperty
    {
        /// <summary>
        /// 名称 示例全部
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 字段名 示例All
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 枚举值 示例30
        /// </summary>
        public object value { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int zindex { get; set; }

        /// <summary>
        /// enum类型
        /// </summary>
        public string key_type { get; set; } = "int";
    }

}
