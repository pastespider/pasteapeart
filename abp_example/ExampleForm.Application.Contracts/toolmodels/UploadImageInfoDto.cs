//using Volo.Abp.Application.Dtos;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.ComponentModel;
//using System.ComponentModel.DataAnnotations;
//using Volo.Abp.Domain.Entities;
//using PasteApeart.Helper;
//using ExampleForm.Application.Contracts;

//namespace ExampleForm.toolmodels
//{

//    ///<summary>
//    ///图片信息
//    ///</summary>
//    public class UploadImageInfoAddDto
//    {

//        ///<summary>
//        ///分类 存放在哪个大模块下,示例head/order/card等，最大8个字符
//        ///</summary>
//        [MaxLength(8)]
//        [Description("分类 存放在哪个大模块下,示例head/order/card等，最大8个字符")]
//        public string Dir { get; set; }

//        ///<summary>
//        ///路径 服务器上的文件路径，最大128字符
//        ///</summary>
//        [MaxLength(128)]
//        [Description("路径 服务器上的文件路径，最大128字符")]
//        public string Path { get; set; }

//        ///<summary>
//        ///访问地址 最大128字符
//        ///</summary>
//        [MaxLength(128)]
//        [Description("访问地址 最大128字符")]
//        public string Url { get; set; }

//        ///<summary>
//        ///文件码 最大32字符
//        ///</summary>
//        [MaxLength(32)]
//        [Description("文件码 最大32字符")]
//        public string Md5 { get; set; }

//        ///<summary>
//        ///文件大小
//        ///</summary>
//        [Description("文件大小")]
//        public int Size { get; set; }

//        ///<summary>
//        ///后缀 .png/.jpg等，最大8字符
//        ///</summary>
//        [MaxLength(8)]
//        [Description("后缀 .png/.jpg等，最大8字符")]
//        public string Extension { get; set; }

//        ///<summary>
//        ///宽度 多少px
//        ///</summary>
//        [Description("宽度 多少px")]
//        public int Width { get; set; }

//        ///<summary>
//        ///高度 多少px
//        ///</summary>
//        [Description("高度 多少px")]
//        public int Height { get; set; }

//        ///<summary>
//        ///上载云
//        ///</summary>
//        [Description("上载云")]
//        public bool UploadCloud { get; set; }

//        ///<summary>
//        ///上传者
//        ///</summary>
//        [Description("上传者")]
//        [PasteOuter("userInfo","extendUser","id","userName")]
//        [PasteShort("UserInfo","ExtendUser","ToUserShort()")]
//        public long UserId { get; set; }
//    }

//    ///<summary>
//    ///图片信息
//    ///</summary>
//    public class UploadImageInfoUpdateDto:UploadImageInfoAddDto
//    {

//        ///<summary>
//        ///ID
//        ///</summary>
//        [PasteHidden]
//        public int Id { get; set; }

//        ///<summary>
//        ///用户 表示管理者
//        ///</summary>
//        [PasteDisplay]
//        public UserShort ExtendUser { get; set; }

//    }

//    ///<summary>
//    ///图片信息
//    ///</summary>
//    public class UploadImageInfoDto:EntityDto<int>
//    {
//        ///<summary>
//        ///分类 存放在哪个大模块下,示例head/order/card等，最大8个字符
//        ///</summary>
//        [MaxLength(8)]
//        [Description("分类 存放在哪个大模块下,示例head/order/card等，最大8个字符")]
//        public string Dir { get; set; }

//        ///<summary>
//        ///路径 服务器上的文件路径，最大128字符
//        ///</summary>
//        [MaxLength(128)]
//        [Description("路径 服务器上的文件路径，最大128字符")]
//        public string Path { get; set; }

//        ///<summary>
//        ///访问地址 最大128字符
//        ///</summary>
//        [MaxLength(128)]
//        [Description("访问地址 最大128字符")]
//        public string Url { get; set; }

//        ///<summary>
//        ///文件码 最大32字符
//        ///</summary>
//        [MaxLength(32)]
//        [Description("文件码 最大32字符")]
//        public string Md5 { get; set; }

//        ///<summary>
//        ///文件大小
//        ///</summary>
//        [Description("文件大小")]
//        public int Size { get; set; }

//        ///<summary>
//        ///后缀 .png/.jpg等，最大8字符
//        ///</summary>
//        [MaxLength(8)]
//        [Description("后缀 .png/.jpg等，最大8字符")]
//        public string Extension { get; set; }

//        ///<summary>
//        ///创建日期
//        ///</summary>
//        [Description("创建日期")]
//        public DateTime CreateDate { get; set; }

//        ///<summary>
//        ///宽度 多少px
//        ///</summary>
//        [Description("宽度 多少px")]
//        public int Width { get; set; }

//        ///<summary>
//        ///高度 多少px
//        ///</summary>
//        [Description("高度 多少px")]
//        public int Height { get; set; }

//        ///<summary>
//        ///上载云
//        ///</summary>
//        [Description("上载云")]
//        public bool UploadCloud { get; set; }

//        ///<summary>
//        ///上传者
//        ///</summary>
//        [Description("上传者")]
//        [PasteOuter("userInfo","extendUser","id","userName")]
//        [PasteShort("UserInfo","ExtendUser","ToUserShort()")]
//        public long UserId { get; set; }

//        ///<summary>
//        ///用户 表示管理者
//        ///</summary>
//        [PasteDisplay]
//        public UserShort ExtendUser { get; set; }

//    }

//    ///<summary>
//    ///图片信息
//    ///</summary>
//    public class UploadImageInfoListDto:EntityDto<int>
//    {
//        ///<summary>
//        ///分类 存放在哪个大模块下,示例head/order/card等，最大8个字符
//        ///</summary>
//        [MaxLength(8)]
//        [Description("分类 存放在哪个大模块下,示例head/order/card等，最大8个字符")]
//        public string Dir { get; set; }

//        ///<summary>
//        ///路径 服务器上的文件路径，最大128字符
//        ///</summary>
//        [MaxLength(128)]
//        [Description("路径 服务器上的文件路径，最大128字符")]
//        public string Path { get; set; }

//        ///<summary>
//        ///访问地址 最大128字符
//        ///</summary>
//        [MaxLength(128)]
//        [Description("访问地址 最大128字符")]
//        public string Url { get; set; }

//        ///<summary>
//        ///文件码 最大32字符
//        ///</summary>
//        [MaxLength(32)]
//        [Description("文件码 最大32字符")]
//        public string Md5 { get; set; }

//        ///<summary>
//        ///文件大小
//        ///</summary>
//        [Description("文件大小")]
//        public int Size { get; set; }

//        ///<summary>
//        ///后缀 .png/.jpg等，最大8字符
//        ///</summary>
//        [MaxLength(8)]
//        [Description("后缀 .png/.jpg等，最大8字符")]
//        public string Extension { get; set; }

//        ///<summary>
//        ///创建日期
//        ///</summary>
//        [Description("创建日期")]
//        public DateTime CreateDate { get; set; }

//        ///<summary>
//        ///宽度 多少px
//        ///</summary>
//        [Description("宽度 多少px")]
//        public int Width { get; set; }

//        ///<summary>
//        ///高度 多少px
//        ///</summary>
//        [Description("高度 多少px")]
//        public int Height { get; set; }

//        ///<summary>
//        ///上载云
//        ///</summary>
//        [Description("上载云")]
//        public bool UploadCloud { get; set; }

//        ///<summary>
//        ///上传者
//        ///</summary>
//        [Description("上传者")]
//        [PasteShort("UserInfo","ExtendUser","ToUserShort()")]
//        [PasteHidden]
//        public long UserId { get; set; }

//        ///<summary>
//        ///用户 表示管理者
//        ///</summary>
//        [PasteDisplay]
//        public UserShort ExtendUser { get; set; }

//    }


            

	
//	///<summary>
//	/// 查询
//	///</summary>
//	public class InputQueryUploadImageInfo:InputSearchBase{
//	}

// }