using PasteApeart.Domain.PasteEntity;

namespace PasteApeart.Domain.Entity
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="Tkey"></typeparam>
    public interface ILevelEntity<Tkey> : IPasteEntity<Tkey>
    {

        /// <summary>
        /// 排序串 一层最高999，那就是6层？
        /// </summary>
        string SortStr { get; set; }

        /// <summary>
        /// 层级
        /// </summary>
        int Level { get; set; }

        /// <summary>
        /// 排序 层级内排序
        /// </summary>
        int Sort { get; set; }

        /// <summary>
        /// 父级
        /// </summary>
        Tkey FatherId { get; set; }

        /// <summary>
        /// 父级串 替换递归查询用
        /// </summary>
        string FatherStr { get; set; }

    }
}
