using PasteApeart.Application.Contracts;

namespace ExampleApeart.Application.Contracts.eventmodels
{
    /// <summary>
    /// 角色创建后 案例事件，角色创建后
    /// </summary>
    public class EventAfterGradeCreateModel : AbsEventModel
    {
        /// <summary>
        /// 角色
        /// </summary>
        public int GradeId { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        public int CreateUserId { get; set; }
    }
}
