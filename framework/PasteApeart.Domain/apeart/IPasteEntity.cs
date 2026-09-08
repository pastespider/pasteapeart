using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasteApeart.Domain
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPasteEntity
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public interface IPasteEntity<TKey>
    {
        /// <summary>
        /// 
        /// </summary>
        TKey Id { get; set; }
    }

}
