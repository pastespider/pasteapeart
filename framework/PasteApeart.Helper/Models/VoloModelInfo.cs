using System.Text.Json.Serialization;

namespace PasteApeart.Helper
{
    /// <summary>
    /// 模型的信息
    /// </summary>
    public class VoloModelInfo
    {
        /// <summary>
        /// 中文名称 注释的空格前部分
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// 描述 注释的空格后部分
        /// </summary>
        public string Desc { get; set; } = "";

        /// <summary>
        /// 文档注释
        /// </summary>
        [JsonIgnore]
        public string Summary { get; set; } = "";

        /// <summary>
        /// 主键类型
        /// </summary>
        public string KeyType { get; set; } = "int";

        /// <summary>
        /// 模型自己的属性规则列表
        /// </summary>
        public List<VoloModelAttribute> Attributes { get; set; }

        /// <summary>
        /// 字段信息
        /// </summary>
        public List<VoloModelProperty> Properties { get; set; }

        /// <summary>
        /// 查询的属性，如果有的话，用于列表页
        /// </summary>
        public List<VoloModelProperty> QueryProperties { get; set; }
    }

}
