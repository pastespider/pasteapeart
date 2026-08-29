using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExampleApeart
{
    /// <summary>
    /// 支持 非递归多级查询，支持多级排序
    /// </summary>
    public interface ILevelEntity
    {
        /// <summary>
        /// 标识
        /// </summary>
        int Id { get; set; }

        /// <summary>
        /// 排序串
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
        int FatherId { get; set; }

        /// <summary>
        /// 父级串 替换递归查询用
        /// </summary>
        string FatherStr { get; set; }

        /// <summary>
        /// 根级 大块
        /// </summary>
        int RootId { get; set; }
    }

}
