using PasteApeart.Helper;
using ExampleForm.testmodels;
using ExampleForm.usermodels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExampleForm.Application.testmodels
{
    /// <summary>
    /// 
    /// </summary>
    public class SelectDto
    {

        /////<summary>
        /////单选 一般表示状态，第二个参数表示日类型这个选项将不会显示，被忽略了
        /////</summary>
        //[ColumnDataType("mark", "test", "datetype")]
        //[PasteSelect("[{\"name\":\"日类型\",\"value\":0},{\"name\":\"月类型\",\"value\":1},{\"name\":\"年类型\",\"value\":2}]","0")]
        //public int DateType { get; set; }
        ///// <summary>
        ///// 单选 注意字段类型为string
        ///// </summary>
        //[PasteSelects("[{\"name\":\"日类型\",\"value\":\"day\"},{\"name\":\"月类型\",\"value\":\"month\"},{\"name\":\"年类型\",\"value\":\"year\"}]")]
        //public string DateName { get; set; }

        ///// <summary>
        ///// 年龄 多个之间使用,隔开
        ///// </summary>
        //[ColumnDataType("splitarray")]
        //public int[] Ages { get; set; }
        ///// <summary>
        ///// 复选 多个之间用逗号隔开
        ///// </summary>
        //[PasteSelects("[{\"name\":\"日类型\",\"value\":\"day\"},{\"name\":\"月类型\",\"value\":\"month\"},{\"name\":\"年类型\",\"value\":\"year\"}]", ",")]
        //public string TypeStrs { get; set; }
        ///// <summary>
        ///// 复选数组 配置最多选择1项，由args4配置，当前等效于PasteSelects
        ///// </summary>
        //[ColumnDataType("selects", "[{\"name\":\"日类型\",\"value\":\"day\"},{\"name\":\"月类型\",\"value\":\"month\"},{\"name\":\"年类型\",\"value\":\"year\"}]","","","1")]
        //public string[] Types { get; set; }
        ///// <summary>
        ///// 分类 多个之间使用,隔开,有默认值1,2,5，前端为字符串输入，用分隔符分割，后端接收到的为int[]
        ///// </summary>
        //[ColumnDataType("splitarray")]
        //public int[] Tabs { get; set; } = new int[] { 1, 2, 5 };
        ///// <summary>
        ///// 曾用名 多个之间使用,隔开，表示的是用户用分隔符输入，然后提交给后端的时候变更为对应的数组，比如当前的string[]
        ///// </summary>
        //[ColumnDataType("splitarray")]
        //public string[] Names { get; set; }

        ///// <summary>
        ///// 动作类型 这是一个Enum类型
        ///// </summary>
        //public ActionEnum ActionType { get; set; }

        ///// <summary>
        ///// 动作类型 由于过滤，UI只会显示1和4也就是正常和停止
        ///// </summary>
        //[PasteSelect("","-1,5")]
        //public ActionEnum SameAction { get; set; }

        /// <summary>
        /// 横向分类 这是一个Enum类型,而且使用了过滤，过滤掉-1和0的值的选项不返回前端
        /// </summary>
        [PasteLselect("", "-1,0")]
        [Description("横向分类 这是一个Enum类型,而且使用了过滤，过滤掉-1和0的值的选项不返回前端")]
        public ActionEnum ClassType { get; set; }

        /// <summary>
        /// 动作类型 由于过滤，UI只会显示1和4也就是正常和停止
        /// </summary>
        [PasteSelect("", "-1,5")]
        [Description("动作类型 由于过滤，UI只会显示1和4也就是正常和停止")]
        public ActionEnum[] Actions { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StringDto
    {
        ///<summary>
        ///姓名 模拟短文本输入
        ///</summary>
        [MaxLength(32)]
        [Required]
        [PasteMark("str", "desc")]
        [PasteSort(5)]
        [Description("姓名 模拟短文本输入")]
        public string Name { get; set; }

        ///<summary>
        ///文本区域 模拟文本区域的输入
        ///</summary>
        [MaxLength(128)]
        [PasteMark("str", "desc")]
        [PasteSort(4)]
        [Description("文本区域 模拟文本区域的输入")]
        public string Desc { get; set; }

        ///<summary>
        ///文本 虽然长度限制256，不过还是以text的形式展示
        ///</summary>
        [MaxLength(256)]
        [PasteText]
        [PasteSort(3)]
        [Description("文本 虽然长度限制256，不过还是以text的形式展示")]
        public string Mark { get; set; }

        ///<summary>
        ///文本区域 长度大于128则自动为textarea
        ///</summary>
        [MaxLength(256)]
        [PasteSort(2)]
        [Description("文本区域 长度大于128则自动为textarea")]
        public string Text { get; set; }

        ///<summary>
        ///内容 虽然设置了长度256，但还是会以富文本的形式展示
        ///</summary>
        [MaxLength(256)]
        [PasteRichText]
        [Description("内容 虽然设置了长度256，但还是会以富文本的形式展示")]
        public string Html { get; set; }

        ///<summary>
        ///富文本 模拟富文本，前端HTML的是使用wangEditv5,默认不配置maxlength的就是html
        ///</summary>
        [Description("富文本 模拟富文本，前端HTML的是使用wangEditv5,默认不配置maxlength的就是html")]
        public string Blog { get; set; }

        ///<summary>
        ///MarkDown 内容将提交给Mark1和MarkHtml 
        ///</summary>
        [PasteMarkDown(300, "markHtml")]
        [Description("MarkDown 内容将提交给Mark1和MarkHtml")]
        public string Mark1 { get; set; }

        /// <summary>
        /// 这个就是Mark1的Html代码
        /// </summary>
        [PasteHidden]
        [Description("这个就是Mark1的Html代码")]
        public string MarkHtml { get; set; }

        /////<summary>
        /////MarkDown2 有默认值的
        /////</summary>
        //[ColumnDataType("markdown")]
        //public string Mark2 { get; set; } = "## 今日成果";

        /// <summary>
        /// 文本 这个可以编辑
        /// </summary>
        [PasteText]
        [Description("文本 这个可以编辑")]
        public string Name1 { get; set; }

        /// <summary>
        /// 文本 这个不能编辑
        /// </summary>
        [PasteText]
        [PasteReadOnly]
        [Description("文本 这个不能编辑")]
        public string Name2 { get; set; } = "给你看看";


    }

    /// <summary>
    /// 时间案例
    /// </summary>
    [PasteGroup("v1", "组别一")]
    [PasteGroup("gg", "子壹组别")]
    [Description("时间案例")]
    public class DateTimeDto
    {

        /// <summary>
        /// 金额
        /// </summary>
        [PasteFenToYuan]
        [Description("金额")]
        public long Amount { get; set; } = 1600;

        ///// <summary>
        ///// 数量
        ///// </summary>
        //[PastePrefix("高于")]
        //[PasteUnit("cm")]
        //public long Val1 { get; set; } = 1600;

        ///// <summary>
        ///// ID 默认以,分割内容，示例1,3,6
        ///// </summary>
        //[PasteSplit]
        //public int[] ids1 { get; set; }

        ///// <summary>
        ///// ID 默认以,分割内容，示例1,3,6
        ///// </summary>
        //[PasteSplit]
        //public int[] ids2 { get; set; } = new int[] { 5, 7 };

        ///// <summary>
        ///// 值 需要以|分割输入项,示例：星期一|星期二
        ///// </summary>
        //[PasteSplit("|")]
        //public string[] vals1 { get; set; }

        ///// <summary>
        ///// 值 需要以|分割输入项,示例：星期一|星期二
        ///// </summary>
        //[PasteSplit("|")]
        //public string[] vals2 { get; set; } = new string[] { "星期六", "星期天" };

        /////<summary>
        /////时间1 默认为datetiem，yyyy-MM-dd HH:mm:ss格式
        /////</summary>
        //public DateTime date1 { get; set; }

        /////<summary>
        /////时间2 给定一个默认值
        /////</summary>
        //public DateTime date5 { get; set; } = DateTime.Now.AddMinutes(-5);

        /////<summary>
        /////时间3 设置为可不填
        /////</summary>
        //public DateTime? date6 { get; set; }

        /////<summary>
        /////时间4 使用了format的时间,最好设置为string格式
        /////</summary>
        //[ColumnDataType("datetime", "yyyy-MM-dd HH")]
        //public string date2 { get; set; }

        /////<summary>
        /////时间区间 时间区间，一般由2个字段承载，所以尾巴字段得隐藏，默认为sdate和edate搭配
        /////</summary>
        //[PasteDaterange("date3", "date4")]
        //public DateTime? date3 { get; set; }

        ///// <summary>
        ///// 由于3设置了，所以这里不显示，只是作为接收使用
        ///// </summary>
        //[PasteHidden]
        //public DateTime? date4 { get; set; }

        ///// <summary>
        ///// 外表
        ///// </summary>
        //[PasteSunform("text")]
        //[PasteGroupItem("v1")]
        //public StringDto abc { get; set; }

        /// <summary>
        /// 枚举一
        /// </summary>
        [Description("枚举一")]
        public ActionEnum m1 { get; set; }

        /// <summary>
        /// 枚举二
        /// </summary>
        [PasteSelects]
        [Description("枚举二")]
        public ActionEnum[] m2 { get; set; }

        /// <summary>
        /// 子1
        /// </summary>
        [PasteDirectsun]
        [PasteGroupItem("gg")]
        [Description("子1")]
        public ObjectDto obj2 { get; set; } = new ObjectDto { Desc = "11111111111111", Name = "2222222", Text = "33333333333" };

        /// <summary>
        /// 子2 这种情况就是错误的，因为没有值，不会被渲染
        /// </summary>
        [PasteDirectsun]
        [Description("子2 这种情况就是错误的，因为没有值，不会被渲染")]
        public List<Model1> sel2 { get; set; }

        /// <summary>
        /// 子3
        /// </summary>
        [PasteDirectsun]
        [Description("子3")]
        public Model1[] sel3 { get; set; } = new Model1[] { new Model1 { Id = 1, Name = "mod1" }, new Model1 { Id = 2, Name = "mod2" } };

        /// <summary>
        /// 子4
        /// </summary>
        [PasteDirectsun]
        [Description("子4")]
        public List<Model1> sel4 { get; set; } = new List<Model1> { new Model1 { Id = 4, Name = "mod4" } };

        /// <summary>
        /// 子5
        /// </summary>
        [PasteDirectsun]
        [Description("子5")]
        public Xavels xitem { get; set; } = new Xavels { };
    }

    /// <summary>
    /// XY
    /// </summary>
    [Description("XY")]
    public class Xavels
    {
        /// <summary>
        /// XX
        /// </summary>
        [PasteDirectsun]
        [Description("XX")]
        public Model1[] sel5 { get; set; } = new Model1[] { new Model1 { Name = "x1" }, new Model1 { Name = "x2" } };
    }

    /// <summary>
    /// object outer 案例
    /// </summary>
    [PasteGroup("g2", "G2", false)]
    [Description("object outer 案例")]
    public class ObjectDto
    {

        ///<summary>
        ///姓名 模拟短文本输入
        ///</summary>
        [MaxLength(32)]
        [Required]
        [Description("姓名 模拟短文本输入")]
        public string Name { get; set; }

        ///<summary>
        ///文本区域 模拟文本区域的输入
        ///</summary>
        [MaxLength(128)]
        [Description("文本区域 模拟文本区域的输入")]
        public string Desc { get; set; }

        ///<summary>
        ///文本区域 长度大于128则自动为textarea
        ///</summary>
        [MaxLength(256)]
        [Description("文本区域 长度大于128则自动为textarea")]
        public string Text { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [PasteOuter("gradeInfo", "extendGrade", "id", "name")]
        [PasteLoaditem("/api/app/obj/item?grade_id={{:=item.gradeId}}")]
        [Description("角色")]
        public int GradeId { get; set; }

        /// <summary>
        /// 角色
        /// </summary>
        [PasteOuters("gradeInfo", "extendGrade", "id+:+name", "name+:+code")]
        [Description("角色")]
        public int[] grade_ids { get; set; }

        /// <summary>
        /// 角色的扩展信息，编辑的时候显示用
        /// </summary>
        [PasteHidden]
        [Description("角色的扩展信息，编辑的时候显示用")]
        public GradeShort ExtendGrade { get; set; }

        /// <summary>
        /// 外表时间
        /// </summary>
        [PasteObject("mod1", "", "id", "name")]
        [Description("外表时间")]
        public Model1 date2 { get; set; }

        /// <summary>
        /// 外表字符
        /// </summary>
        [PasteObjects("mod2", "id", "mark", "")]
        [Description("外表字符")]
        public Model2[] str2 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [PasteSunform("mod1", 200)]
        [PasteGroupItem("g2", "G2组", true)]
        public Model1 mod1 { get; set; } = new Model1 { Id = 3, Name = "3333333333" };

    }

    /// <summary>
    /// 
    /// </summary>
    public class Model1
    {
        /// <summary>
        /// ID 这个用于排重
        /// </summary>
        [Description("ID 这个用于排重")]
        public int Id { get; set; }

        /// <summary>
        /// 名称 这个用于显示到UI
        /// </summary>
        [MaxLength(32)]
        [Description("名称 这个用于显示到UI")]
        public string Name { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class Model2
    {
        /// <summary>
        /// ID 这个用于排重
        /// </summary>
        [Description("ID 这个用于排重")]
        public int Id { get; set; }

        /// <summary>
        /// 名称 这个用于显示到UI
        /// </summary>
        [MaxLength(32)]
        [Description("名称 这个用于显示到UI")]
        public string Mark { get; set; }


        /// <summary>
        /// 描述
        /// </summary>
        [MaxLength(64)]
        [Description("描述")]
        public string Desc { get; set; }
    }

    /// <summary>
    /// 图片案例
    /// </summary>
    [Description("图片案例")]
    public class ImageModel
    {
        /// <summary>
        /// 图片1
        /// </summary>
        [PasteImage(1, "/api/app/Upload/UpWord?type=word", "1500*0")]
        [Description("图片1")]
        public string Img1 { get; set; }

        /// <summary>
        /// 图片文字识别
        /// </summary>
        [PasteImage(1, "word", "750*0")]
        [Description("图片2")]
        [PasteUnit(".")]
        public string Img2 { get; set; }

        /// <summary>
        /// 图片3
        /// </summary>
        [PasteImage(3, "head", "120*0")]
        [Description("图片3")]
        [PasteUnit(".")]
        public string Img3 { get; set; }

        /// <summary>
        /// 图片4
        /// </summary>
        [PasteImage(5, "head", "120*0")]
        [Description("图片4")]
        public string[] Img4 { get; set; }
    }

    /// <summary>
    /// 上传文件案例
    /// </summary>
    [Description("上传文件案例")]
    public class FileModel
    {
        /// <summary>
        /// 文件1 默认配置，使用默认的路径上传
        /// </summary>
        [PasteFile()]
        [Description("文件1 默认配置，使用默认的路径上传")]
        public string File1 { get; set; } = "";

        /// <summary>
        /// 文件2 定义存储路径
        /// </summary>
        [PasteFile("", "xpath")]
        [Description("文件2 定义存储路径")]
        public string File2 { get; set; } = "";

        /// <summary>
        /// 文件3 自定义上传地址,注意headers
        /// </summary>
        [PasteFile("/api/app/Upload/UpFile?type=abc")]
        [Description("文件3 自定义上传地址,注意headers")]
        public string File3 { get; set; } = "";

        /// <summary>
        /// 文件4 自定义onchange函数
        /// </summary>
        [PasteFile("", "", "funcUploadFile(this);")]
        [Description("文件4 自定义onchange函数")]
        public string File4 { get; set; } = "";
    }

    /// <summary>
    /// query案例
    /// </summary>
    [Description("query案例")]
    public class LinkQueryModel
    {

        ///<summary>
        ///姓名 模拟短文本输入
        ///</summary>
        [MaxLength(32)]
        [Required]
        [Description("姓名 模拟短文本输入")]
        public string Name { get; set; }

        ///<summary>
        ///文本区域 模拟文本区域的输入
        ///</summary>
        [MaxLength(128)]
        [Description("文本区域 模拟文本区域的输入")]
        public string Desc { get; set; }

    }

    /// <summary>
    /// 
    /// </summary>
    public class DataFormat
    {
        /// <summary>
        /// Int
        /// </summary>
        [Description("Int")]
        [PasteClass("cusclass")]
        public int Int1 { get; set; }

        /// <summary>
        /// Int?
        /// </summary>
        [Description("Int?")]
        [PasteStyle("width:50px;")]
        public int? Int2 { get; set; }

        /// <summary>
        /// Long
        /// </summary>
        [Description("Long")]
        public long Long1 { get; set; } = 1744152780507;

        /// <summary>
        /// Long?
        /// </summary>
        [Description("Long?")]
        public long? Long2 { get; set; } = 1744152780507;

        /// <summary>
        /// Date TimeStamp
        /// </summary>
        [PasteDate("yyyy-MM-dd", "times")]
        [Description("Date TimeStamp")]
        public long Time1 { get; set; } = 1744152780;

        /// <summary>
        /// Date TimeStamp?
        /// </summary>
        [PasteDate("yyyy-MM-dd", "times")]
        [Description("Date TimeStamp?")]
        public long? Time2 { get; set; } = 1744252780;

        /// <summary>
        /// Date TimeStampms
        /// </summary>
        [PasteDate("yyyy-MM-dd HH:mm:ss", "timems")]
        [Description("Date TimeStampms")]
        public long Time3 { get; set; } = 1744352780123;

        /// <summary>
        /// Date TimeStampms?
        /// </summary>
        [PasteDate("yyyy-MM-dd", "timems")]
        [Description("Date TimeStampms?")]
        public long? Time4 { get; set; } = 1744452780507;

        /// <summary>
        /// Enum[]
        /// </summary>
        [PasteSplit(",")]
        [Description("Enum[]")]
        public EnumIsEnableState[] state1 { get; set; }

        /// <summary>
        /// long[]
        /// </summary>
        [PasteSplit(",")]
        [Description("long[]")]
        public long[] state2 { get; set; }

        /// <summary>
        /// int[]
        /// </summary>
        [PasteSplit(",")]
        [Description("int[]")]
        public int[] state3 { get; set; }

        /// <summary>
        /// uint32[]
        /// </summary>
        [PasteSplit(",")]
        [Description("uint32[]")]
        public UInt32[] state4 { get; set; }

    }
}
