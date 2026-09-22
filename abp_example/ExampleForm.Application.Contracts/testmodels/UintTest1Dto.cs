using ExampleForm.Application.Contracts;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Volo.Abp.Application.Dtos;

namespace ExampleForm.testmodels
{

    ///<summary>
    ///测试UINT
    ///</summary>
    public class UintTest1AddDto
    {

        ///<summary>
        ///名称
        ///</summary>
        [MaxLength(16)]
        [Description("名称")]
        public string Name { get; set; }

        ///<summary>
        ///说明
        ///</summary>
        [MaxLength(64)]
        [Description("说明")]
        public string Desc { get; set; }
    }

    ///<summary>
    ///测试UINT
    ///</summary>
    public class UintTest1UpdateDto : UintTest1AddDto
    {

        ///<summary>
        ///ID
        ///</summary>
        [PasteHidden]
        public uint Id { get; set; }

    }

    ///<summary>
    ///测试UINT
    ///</summary>
    public class UintTest1Dto : EntityDto<uint>
    {
        ///<summary>
        ///名称
        ///</summary>
        [MaxLength(16)]
        [Description("名称")]
        public string Name { get; set; }

        ///<summary>
        ///说明
        ///</summary>
        [MaxLength(64)]
        [Description("说明")]
        public string Desc { get; set; }

    }

    ///<summary>
    ///测试UINT
    ///</summary>
    public class UintTest1ListDto : EntityDto<uint>
    {
        ///<summary>
        ///名称
        ///</summary>
        [MaxLength(16)]
        [Description("名称")]
        public string Name { get; set; }

        ///<summary>
        ///说明
        ///</summary>
        [MaxLength(64)]
        [Description("说明")]
        public string Desc { get; set; }

    }





    ///<summary>
    /// 查询
    ///</summary>
    public class InputQueryUintTest1 : InputSearchBase
    {
    }

}