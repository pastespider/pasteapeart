namespace PasteApeart.Domain.PasteEntity
{
    /// <summary>
    /// 如何和ABP的冲突了，就用这个
    /// </summary>
    public interface IPasteEntity
    {

    }

    /// <summary>
    /// 如何和ABP的冲突了，就用这个
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
