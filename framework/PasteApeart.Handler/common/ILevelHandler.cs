using PasteApeart.Domain.Entity;

namespace PasteApeart.Handler.Level
{
    /// <summary>
    /// 
    /// </summary>
    public interface ILevelHandler
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="info"></param>
        /// <returns></returns>
        Task AfterAddAsync<T, Tkey>(ILevelEntity<Tkey> info) where T : class, ILevelEntity<Tkey>;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="info"></param>
        /// <param name="oldFatherStr"></param>
        /// <param name="oldLevel"></param>
        /// <returns></returns>
        Task AfterUpdateAsync<T, Tkey>(ILevelEntity<Tkey> info, string oldFatherStr, int oldLevel) where T : class, ILevelEntity<Tkey>;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="entities"></param>
        void AllResortByFatherIdAndSort<T, Tkey>(List<T> entities) where T : ILevelEntity<Tkey>;
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="Tkey"></typeparam>
        /// <param name="TableName"></param>
        /// <param name="input"></param>
        /// <param name="step"></param>
        /// <returns></returns>
        Task AutoSort<Tkey>(string TableName, ILevelEntity<Tkey> input, int step = 10);
    }
}