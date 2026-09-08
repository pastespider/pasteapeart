using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteApeart.Domain.Entity
{
    /// <summary>
    /// 
    /// </summary>
    public interface IEntity
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IEntity<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        TKey Id { get; set; }
    }
}
