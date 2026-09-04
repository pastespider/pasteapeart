using System.Text.Json.Serialization;

namespace PasteApeart.Helper
{
    /// <summary>
    /// 字段的属性规则
    /// </summary>
    public class VoloModelAttribute
    {
        /// <summary>
        /// 验证名称 MaxLength命名主要是为了兼容前端
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// 过滤器值1 128
        /// </summary>
        public string Args1 { get; set; } = "";

        /// <summary>
        /// 过滤器值2
        /// </summary>
        public string Args2 { get; set; } = "";

        /// <summary>
        /// 过滤器值3
        /// </summary>
        public string Args3 { get; set; } = "";

        /// <summary>
        /// 过滤器值4
        /// </summary>
        public string Args4 { get; set; } = "";

        /// <summary>
        /// 过滤器验证信息
        /// </summary>
        public string Error { get; set; } = "";

        /// <summary>
        /// 排序
        /// </summary>
        [JsonIgnore]
        public int Order { get; set; }
    }

}
