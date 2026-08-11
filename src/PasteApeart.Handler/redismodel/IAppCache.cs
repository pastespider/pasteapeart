using StackExchange.Redis;

namespace PasteApeart.Handler
{
    /// <summary>
    /// 缓存通用接口
    /// </summary>
    public interface IAppCache
    {
        /// <summary>
        /// 
        /// </summary>
        bool IsRedis { get; }

        /// <summary>
        /// 添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        bool Set(string key, string value, TimeSpan? expiry = null);

        /// <summary>
        /// 添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        bool Set(string key, string value, int seconds);

        /// <summary>
        /// 添加一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        bool SetObject<T>(string key, T value, TimeSpan? expiry = null);

        /// <summary>
        /// 添加一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        bool SetObject<T>(string key, T value, int seconds);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        [Obsolete]
        bool SetValue(string key, string val, int second);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [Obsolete]
        bool SetValue(string key, string val);

        /// <summary>
        /// 获取一个对象。
        /// </summary>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        T GetObject<T>(string key);

        /// <summary>
        /// 获取一个字符串对象。
        /// </summary>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        [Obsolete]
        string Get(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        [Obsolete]
        string GetStringValue(string key);

        /// <summary>
        /// 删除一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回是否执行成功。</returns>
        [Obsolete]
        bool Delete(string key);

        /// <summary>
        /// 返回键是否存在。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回键是否存在。</returns>
        [Obsolete]
        bool Exists(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        [Obsolete]
        bool SetExpire(string key, TimeSpan? expiry);

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        [Obsolete]
        bool SetExpire(string key, int seconds);

        /// <summary>
        /// 异步添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null);

        /// <summary>
        /// 异步添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        Task<bool> SetAsync(string key, string value, int seconds);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        Task<bool> SetObjectAsync<T>(string key, T value, int seconds = 300);

        /// <summary>
        /// 异步获取一个对象。
        /// </summary>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        Task<T> GetObjectAsync<T>(string key);

        /// <summary>
        /// 异步获取一个字符串对象。
        /// </summary>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        Task<string> GetAsync(string key);

        /// <summary>
        /// 异步删除一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回是否执行成功。</returns>
        Task<bool> DeleteAsync(string key);

        /// <summary>
        /// 异步设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        Task<bool> SetExpireAsync(string key, int seconds);

        /// <summary>
        /// 异步设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        Task<bool> SetExpireAsync(string key, TimeSpan? expiry);

        #region 分布式锁...

        /// <summary>
        /// 获取锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否已锁。</returns>
        bool Lock(string key, int seconds);

        /// <summary>
        /// 释放锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <returns>是否成功。</returns>
        bool UnLock(string key);

        /// <summary>
        /// 异步获取锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否成功。</returns>
        Task<bool> LockAsync(string key, int seconds);

        /// <summary>
        /// 异步释放锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <returns>是否成功。</returns>
        Task<bool> UnLockAsync(string key);

        #endregion


        #region hashtable
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        List<HashKeyValue> HashGetAll(string key);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<List<HashKeyValue>> HashGetAllAsync(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        void HashSet(string key, HashEntry[] vals);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        Task HashSetAsync(string key, HashEntry[] vals);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        void HashSet(string key, System.Collections.Generic.List<HashKeyValue> vals);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        Task HashSetAsync(string key, System.Collections.Generic.List<HashKeyValue> vals);



        string HashGet(string key, string sunkey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sunkey"></param>
        /// <returns></returns>
        Task<string> HashGetAsync(string key, string sunkey);
        #endregion
    }

}
