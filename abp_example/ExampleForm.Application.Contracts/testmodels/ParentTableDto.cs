using ExampleForm.Application.Contracts;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace ExampleForm.testmodels
{

    ///<summary>
    ///测试表
    ///</summary>
    public class ParentTableAddDto
    {

        ///<summary>
        ///姓名
        ///</summary>
        [MaxLength(16)]
        [PasteSearch()]
        [Description("姓名")]
        public string Name { get; set; }

        ///<summary>
        ///年龄
        ///</summary>
        [Description("年龄")]
        public int Age { get; set; } = 12;

        ///<summary>
        ///包含
        ///</summary>
        [Description("包含")]
        public IncludeTableUpdateDto Front { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [Description("状态")]
        public bool IsEnable { get; set; } = true;
    }

    ///<summary>
    ///测试表
    ///</summary>
    public class ParentTableUpdateDto : ParentTableAddDto
    {

        ///<summary>
        ///ID
        ///</summary>
        [PasteHidden]
        public int Id { get; set; }

    }

    ///<summary>
    ///测试表
    ///</summary>
    public class ParentTableDto : EntityDto<int>
    {
        ///<summary>
        ///姓名
        ///</summary>
        [MaxLength(16)]
        [PasteSearch()]
        [Description("姓名")]
        public string Name { get; set; }

        ///<summary>
        ///创建日期
        ///</summary>
        [Description("创建日期")]
        public DateTime CreateDate { get; set; }

        ///<summary>
        ///年龄
        ///</summary>
        [Description("年龄")]
        public int Age { get; set; }

        ///<summary>
        ///包含
        ///</summary>
        [Description("包含")]
        public IncludeTableDto Front { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [Description("状态")]
        public bool IsEnable { get; set; }

    }

    ///<summary>
    ///测试表
    ///</summary>
    public class ParentTableListDto : EntityDto<int>
    {
        ///<summary>
        ///姓名
        ///</summary>
        [MaxLength(16)]
        [PasteSearch()]
        [Description("姓名")]
        public string Name { get; set; }

        ///<summary>
        ///创建日期
        ///</summary>
        [Description("创建日期")]
        public DateTime CreateDate { get; set; }

        ///<summary>
        ///年龄
        ///</summary>
        [Description("年龄")]
        public int Age { get; set; }

        ///<summary>
        ///包含
        ///</summary>
        [Description("包含")]
        public IncludeTableListDto Front { get; set; }

        ///<summary>
        ///状态
        ///</summary>
        [Description("状态")]
        public bool IsEnable { get; set; }

    }





    ///<summary>
    /// 查询
    ///</summary>
    public class InputQueryParentTable : InputSearchBase
    {

        ///<summary>
        ///姓名
        ///</summary>
        [MaxLength(16)]
        [Description("姓名")]
        public string Name { get; set; }
    }

}