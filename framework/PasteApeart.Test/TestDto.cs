using PasteApeart;
using PasteApeart.Applciation.Contracts;

namespace PasteApeart.Test
{
    /// <summary>
    /// 
    /// </summary>
    public class TestBase : PasteEntityDto<int>
    {
        /// <summary>
        /// 
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Age { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int SubjectId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TestLevel2 : TestBase
    {
        /// <summary>
        /// 
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public new int Age { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int GroupId { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class TestDto : TestLevel2
    {
        /// <summary>
        /// 
        /// </summary>
        public string Stype { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public new string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Desc { get; set; }
    }
}
