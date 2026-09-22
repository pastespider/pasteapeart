using ExampleForm.Application.Contracts;
using PasteApeart.Application.Contracts;
using PasteApeart.Helper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExampleForm.testmodels
{

    ///<summary>
    ///
    ///</summary>
    public class IncludeTableAddDto
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
    ///
    ///</summary>
    public class IncludeTableUpdateDto : IncludeTableAddDto
    {

        ///<summary>
        ///ID
        ///</summary>
        [PasteHidden]
        [Description("ID")]
        public int Id { get; set; }

    }

    ///<summary>
    ///
    ///</summary>
    public class IncludeTableDto : IncludeTableUpdateDto
    {

    }

    ///<summary>
    ///
    ///</summary>
    public class IncludeTableListDto : IncludeTableUpdateDto
    {

    }




    ///<summary>
    /// 查询
    ///</summary>
    [Description("查询")]
    public class InputQueryIncludeTable : InputSearchBase
    {
    }

}