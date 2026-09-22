using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace ExampleForm.toolmodels
{
    /// <summary>
    /// 图片信息
    /// </summary>
    public class UploadImageInfo:Entity<int>
    {
        /// <summary>
        /// 分类 存放在哪个大模块下,示例head/order/card等，最大8个字符
        /// </summary>
        [MaxLength(8)]
        public string Dir { get; set; }

        /// <summary>
        /// 路径 服务器上的文件路径，最大128字符
        /// </summary>
        [MaxLength(128)]
        public string Path { get; set; }

        /// <summary>
        /// 访问地址 最大128字符
        /// </summary>
        [MaxLength(128)]
        public string Url { get; set; }

        /// <summary>
        /// 文件码 最大32字符
        /// </summary>
        [MaxLength(32)]
        public string Md5 { get; set; }

        /// <summary>
        /// 文件大小
        /// </summary>
        public int Size { get; set; }

        /// <summary>
        /// 后缀 .png/.jpg等，最大8字符
        /// </summary>
        [MaxLength(8)]
        public string Extension { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; set; } = DateTime.Now;

        /// <summary>
        /// 宽度 多少px
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高度 多少px
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 上载云
        /// </summary>
        public bool UploadCloud { get; set; }

        /// <summary>
        /// 上传者
        /// </summary>
        public long UserId { get; set; }
    }
}
