using System.Text.Json.Serialization;

namespace PasteApeart.Helper
{
    /// <summary>
    /// 模型的字段信息
    /// </summary>
    public class VoloModelProperty
    {
        /// <summary>
        /// 字段名称 CreateDate
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字段类型 System.DateTime
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 字段默认值 0001/1/1 0:00:00
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// 字段中文 创建日期
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 注释的后部分 空格之后的，一般用于描述
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// XML文档注释内容
        /// </summary>
        [JsonIgnore]
        public string Summary { get; set; }

        /// <summary>
        /// 限定 最大长度
        /// </summary>
        public int Maxlength { get; set; } = 0;

        /// <summary>
        /// 是否必填
        /// </summary>
        public bool Required { get; set; } = false;

        /// <summary>
        /// Nullable格式
        /// </summary>
        public bool Nullable { get; set; } = false;

        /// <summary>
        /// 排序
        /// </summary>
        [JsonIgnore]
        public int Sort { get; set; } = 0;

        /// <summary>
        /// 是否有Description特性
        /// </summary>
        [JsonIgnore]
        public bool HasDescAttribute { get; set; } = false;

        /// <summary>
        /// 字段属性规则
        /// </summary>
        public List<VoloModelAttribute> Attributes { get; set; }

    }

}
