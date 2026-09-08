using PasteApeart.Domain;

namespace PasteApeart.Applciation.Contracts
{

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TKey"></typeparam>
    public abstract class PasteEntityDto<TKey> : IPasteEntity<TKey>
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
            if (obj is not PasteEntityDto<TKey> other)
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