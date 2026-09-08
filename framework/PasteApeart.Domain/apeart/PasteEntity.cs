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
    public abstract class PasteEntity
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class PasteEntity<TKey> : IPasteEntity
    {
        /// <summary>
        /// 
        /// </summary>
        public virtual TKey Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is not PasteEntity<TKey> other)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (Id == null || other.Id == null)
                return false;

            return Id.Equals(other.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return Id?.GetHashCode() ?? 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(PasteEntity<TKey> left, PasteEntity<TKey> right)
        {
            return Equals(left, right);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(PasteEntity<TKey> left, PasteEntity<TKey> right)
        {
            return !Equals(left, right);
        }
    }

}
