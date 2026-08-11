namespace PasteApeart.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public interface IConnectionStringSelector
    {
        //[Obsolete("use readonly")]
        //string GetConnectionStringName();

        //[Obsolete("use readonly")]
        //void SetConnectionStringName(string name);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        bool IsReadOnly();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isread"></param>
        void SetReadOnly(bool isread);
    }
}
