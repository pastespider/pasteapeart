//using PasteApeart.Helper;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;

//namespace ExampleForm.Application.Contracts
//{
//    /// <summary>
//    /// 分页查询基础模型
//    /// </summary>
//    [Description("分页查询基础模型")]
//    public class InputSearchBase
//    {

//        /// <summary>
//        /// 页码
//        /// </summary>
//        [Description("页码")]
//        [Range(1, 1000, ErrorMessage = "页码范围不符合约定，最小1，最大1000，请重新输入!")]
//        public int page { get; set; } = 1;

//        /// <summary>
//        /// 页大小
//        /// </summary>
//        [Description("页大小")]
//        [Range(5, 1000, ErrorMessage = "每页大小不符合约定，最小5，最大1000，请重新输入!")]
//        public int size { get; set; } = 20;

//        /// <summary>
//        /// 关键字 键入关键字进行搜索
//        /// </summary>
//        [MaxLength(32)]
//        [Description("关键字 键入关键字进行搜索")]
//        public string word { get; set; } = "";

//        /// <summary>
//        /// 标记是否第一页数据和模型信息一起返回，值需要为one的时候才会启用
//        /// </summary>
//        [MaxLength(8)]
//        [PasteHidden]
//        public string request { get; set; } = "";

//    }
//}
