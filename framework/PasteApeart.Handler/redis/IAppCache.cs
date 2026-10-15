using StackExchange.Redis;

namespace PasteApeart.Handler.redis
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
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<bool> ExistsAsync(string key);

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

        ///// <summary>
        ///// 获取锁。
        ///// </summary>
        ///// <param name="key">锁名称。</param>
        ///// <param name="seconds">过期时间（秒）。</param>
        ///// <returns>是否已锁。</returns>
        //bool Lock(string key, int seconds);

        ///// <summary>
        ///// 释放锁。
        ///// </summary>
        ///// <param name="key">锁名称。</param>
        ///// <returns>是否成功。</returns>
        //bool UnLock(string key);

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
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //List<HashKeyValue> HashGetAll(string key);

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
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="isnum"></param>
        /// <returns></returns>
        Task HashSetAsync(string key, string field, RedisValue value, bool isnum = false);

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        string Print();

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="field"></param>
        ///// <param name="num"></param>
        ///// <returns></returns>
        //Task<long> HashIncrementAsync(string key, string field, long num = 1);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="field"></param>
        ///// <returns></returns>
        //Task<long> HashDecrementAsync(string key, string field);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="isNum"></param>
        /// <returns></returns>
        Task<int> HashGetIntFieldAsync(string key, string field, bool isNum = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="isNum"></param>
        /// <returns></returns>
        Task<long> HashGetLongFieldAsync(string key, string field, bool isNum = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        Task<bool> HashExistsAsync(string key, string filed);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<bool> HashDeleteAsync(string key, string field);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<T> HashGetObjectAsync<T>(string key, string field);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> HashSetObjectAsync<T>(string key, string field, T entity);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pattern"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="isincom"></param>
        /// <returns></returns>
        Task<List<HashEntry>> HashScanAsync(string key, string pattern, int page, int size, bool isincom = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sunkey"></param>
        /// <param name="isnum"></param>
        /// <returns></returns>
        Task<string> HashGetAsync(string key, string sunkey, bool isnum = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <param name="isinnum"></param>
        /// <returns></returns>
        Task<RedisValue[]> HashGetAsync(string key, RedisValue[] fields, bool isinnum = false);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        Task<bool> HashDeleteAsync(string key, RedisValue[] fields);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<long> HashLengthAsync(string key);
        #endregion

        #region sortedset

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        Task<RedisValue[]> SortedSetRangeByScoreAsync(string key, long start = 0, long stop = 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<long> SortedSetIncrementAsync(string key, string field, long value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<long> SortedSetDecrementAsync(string key, string field, long value);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        Task<bool> SortedSetRemoveAsync(string key, string field);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        Task<bool> SortedSetAddAsync(string key, string field, long value);
        #endregion


        //--------------------------------------------------------------------------------

        ///// <summary>
        ///// 
        ///// </summary>
        //bool IsRedis { get; }

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
        bool SetValue(string key, string val, int second);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //Task<bool> ExistsAsync(string key);

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

        ///// <summary>
        ///// 异步添加一个字符串对象。
        ///// </summary>
        ///// <param name="key">键。</param>
        ///// <param name="value">值。</param>
        ///// <param name="expiry">过期时间（时间间隔）。</param>
        ///// <returns>返回是否执行成功。</returns>
        //Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null);

        ///// <summary>
        ///// 异步添加一个字符串对象。
        ///// </summary>
        ///// <param name="key">键。</param>
        ///// <param name="value">值。</param>
        ///// <param name="seconds">过期时间（秒）。</param>
        ///// <returns>返回是否执行成功。</returns>
        //Task<bool> SetAsync(string key, string value, int seconds);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="key"></param>
        ///// <param name="value"></param>
        ///// <param name="seconds"></param>
        ///// <returns></returns>
        //Task<bool> SetObjectAsync<T>(string key, T value, int seconds = 300);

        ///// <summary>
        ///// 异步获取一个对象。
        ///// </summary>
        ///// <typeparam name="T">对象的类型。</typeparam>
        ///// <param name="key">值。</param>
        ///// <returns>返回对象的值。</returns>
        //Task<T> GetObjectAsync<T>(string key);

        ///// <summary>
        ///// 异步获取一个字符串对象。
        ///// </summary>
        ///// <param name="key">值。</param>
        ///// <returns>返回对象的值。</returns>
        //Task<string> GetAsync(string key);

        ///// <summary>
        ///// 异步删除一个对象。
        ///// </summary>
        ///// <param name="key">键。</param>
        ///// <returns>返回是否执行成功。</returns>
        //Task<bool> DeleteAsync(string key);

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task BatchRemoveAsync(string[] keys);

        ///// <summary>
        ///// 异步设置一个键的过期时间。
        ///// </summary>
        ///// <param name="key">键。</param>
        ///// <param name="seconds">过期时间（秒）。</param>
        ///// <returns>返回是否执行成功。</returns>
        //Task<bool> SetExpireAsync(string key, int seconds);

        ///// <summary>
        ///// 异步设置一个键的过期时间。
        ///// </summary>
        ///// <param name="key">键。</param>
        ///// <param name="expiry">过期时间（时间间隔）。</param>
        ///// <returns>返回是否执行成功。</returns>
        //Task<bool> SetExpireAsync(string key, TimeSpan? expiry);

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

        ///// <summary>
        ///// 异步获取锁。
        ///// </summary>
        ///// <param name="key">锁名称。</param>
        ///// <param name="seconds">过期时间（秒）。</param>
        ///// <returns>是否成功。</returns>
        //Task<bool> LockAsync(string key, int seconds);

        ///// <summary>
        ///// 异步释放锁。
        ///// </summary>
        ///// <param name="key">锁名称。</param>
        ///// <returns>是否成功。</returns>
        //Task<bool> UnLockAsync(string key);

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
        HashEntry[] HashGetAllEntry(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<HashEntry[]> HashGetAllEntryAsync(string key);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //Task<List<HashKeyValue>> HashGetAllAsync(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        Task<long> HashIncrementAsync(string key, RedisValue field, long num = 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        Task<long> HashDecrementAsync(string key, RedisValue field, long num = 1);

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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="field"></param>
        ///// <returns></returns>
        //Task<bool> HashDeleteAsync(string key, string field);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pattern"></param>
        /// <param name="size"></param>
        /// <param name="cursor"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        Task<HashEntry[]> HashScanAsync(string key, string pattern, int size = 1, int cursor = 0, int offset = 0);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sunkey"></param>
        /// <returns></returns>
        string HashGet(string key, string sunkey);

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="sunkey"></param>
        ///// <returns></returns>
        //Task<string> HashGetAsync(string key, string sunkey);

        /// <summary>
        /// 随机从Hash中获取一个
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<HashEntry> HashRandomFieldsWithValuesAsync(string key);
        #endregion



        #region -----------------------blog---------------------------------



        ///// <summary>
        ///// 添加某一个数据
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="felid"></param>
        ///// <param name="num"></param>
        //Task HashIncrementAsync(string key, string felid, int num = 1);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<string> ReadStringAsync(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        Task<T> ReadObjectAsync<T>(string key);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        Task<bool> SetAddAsync(string skey, int val);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        Task<bool> SetContainsAsync(string skey, int filed);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        Task<bool> SetRemoveAsync(string skey, int filed);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        Task<long> SetRandomMemberAsync(string skey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        Task<long> SetLengthAsync(string skey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="sval"></param>
        /// <returns></returns>
        Task<long> ListRightPushAsync(string skey, string sval);

        /// <summary>
        /// 返回最小的一个 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="score">时间戳秒</param>
        /// <returns></returns>
        Task<string> SortedSetRangeByScoreAsync(string skey, long score);

        /// <summary>
        /// 获取当前排期最长的任务时间戳 如果没有则返回当前时间戳
        /// </summary>
        /// <param name="skey"></param>
        /// <returns>时间戳秒</returns>
        Task<long> SortedSetRangeByRankWithScoresAsync(string skey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        Task<long> ListLengthAsync(string skey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        Task<string> ListLeftPopAsync(string skey);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        Task<List<string>> ListRangeAsync(string skey, int start = 0, int stop = 30);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        Task<List<T>> ListRangeAsync<T>(string skey, int start = 0, int stop = 30);


        #endregion


    }

}
