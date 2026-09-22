using PasteApeart.Domain;
using PasteApeart.Domain.PasteEntity;

namespace PasteApeart.Applciation.Contracts.EntityDto
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class EntityDto<TKey> : IPasteEntity<TKey>
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
            if (obj is not EntityDto<TKey> other)
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

    }

}