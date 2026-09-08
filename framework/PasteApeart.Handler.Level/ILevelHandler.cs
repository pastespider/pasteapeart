using PasteApeart.Domain;

namespace PasteApeart.Handler.Level
{
    public interface ILevelHandler
    {
        Task AfterAddAsync<T, Tkey>(ILevelEntity<Tkey> info) where T : class, ILevelEntity<Tkey>;
        Task AfterUpdateAsync<T, Tkey>(ILevelEntity<Tkey> info, string oldFatherStr, int oldLevel) where T : class, ILevelEntity<Tkey>;
        void AllResortByFatherIdAndSort<T, Tkey>(List<T> entities) where T : ILevelEntity<Tkey>;
        Task AutoSort<Tkey>(string TableName, ILevelEntity<Tkey> input, int step = 10);
    }
}