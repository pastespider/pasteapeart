using Microsoft.Extensions.Options;
using PasteApeart.Handler.Extend;
using StackExchange.Redis;
using System.Text.Json;

namespace PasteApeart.Handler.redis
{
    /// <summary>
    /// 封装 Redis 相关操作的方法类。
    /// </summary>
    public class RedisClient : IAppCache
    {
        //private readonly RedisConfig _redisconfig;
        //private IDatabase _database { get { return _mainconnect.Value.GetDatabase(); } }
        //private Lazy<ConnectionMultiplexer> _mainconnect;
        //private string PrefixHead = String.Empty;
        //public RedisClient(IOptions<RedisConfig> options)
        //{
        //    _redisconfig = options.Value;
        //    var oaconfig = ConfigurationOptions.Parse(_redisconfig.MainConnection);
        //    PrefixHead = _redisconfig.Prefix;
        //    oaconfig.ClientName = _redisconfig.ClientName;
        //    _mainconnect = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(oaconfig));
        //}

        private readonly RedisConfig _redisconfig;
        private readonly Lazy<ConnectionMultiplexer>[] _connectionPool;
        private int _currentConnectionIndex = 0;
        private readonly object _lock = new object();

        /// <summary>
        /// Key的前缀
        /// </summary>
        private string PrefixHead = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        public RedisClient(IOptions<RedisConfig> options)
        {
            _redisconfig = options.Value;
            PrefixHead = _redisconfig.Prefix;
            _connectionPool = new Lazy<ConnectionMultiplexer>[4];
            for (int i = 0; i < _connectionPool.Length; i++)
            {
                var config = ConfigurationOptions.Parse(_redisconfig.MainConnection);
                config.ClientName = $"{_redisconfig.ClientName}_{i}"; // 为每个连接添加序号
                _connectionPool[i] = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(config));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private IDatabase GetDatabase()
        {
            lock (_lock)
            {
                var connection = _connectionPool[_currentConnectionIndex];
                _currentConnectionIndex = (_currentConnectionIndex + 1) % _connectionPool.Length;
                return connection.Value.GetDatabase();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private IDatabase _database { get { return GetDatabase(); } }


        private string setKey(string key)
        {
            return PrefixHead + key;
        }



        #region 同步方法...

        /// <summary>
        /// 添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool Set(string key, string value, TimeSpan? expiry = null)
        {
            if (expiry == null)
            {
                return _database.StringSet(setKey(key), value);
            }
            return _database.StringSet(setKey(key), value, expiry.Value);
        }

        /// <summary>
        /// 设置值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool SetValue(string key, string val, int second)
        {
            return _database.StringSet(setKey(key), val, TimeSpan.FromSeconds(second));
        }

        /// <summary>
        /// 添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool Set(string key, string value, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            return _database.StringSet(setKey(key), value, expiry);
        }

        /// <summary>
        /// 添加一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool SetObject<T>(string key, T value, TimeSpan? expiry = null)
        {
            var data = System.Text.Json.JsonSerializer.Serialize<T>(value, JsonConfig.Options);// JsonConvert.SerializeObject(value);
            if (expiry == null)
            {
                return _database.StringSet(setKey(key), data);
            }
            else
            {
                return _database.StringSet(setKey(key), data, expiry.Value);
            }
        }

        /// <summary>
        /// 添加一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool SetObject<T>(string key, T value, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            var data = System.Text.Json.JsonSerializer.Serialize<T>(value, JsonConfig.Options); //JsonConvert.SerializeObject(value);
            return _database.StringSet(setKey(key), data, expiry);
        }

        /// <summary>
        /// 获取一个对象。
        /// </summary>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public T GetObject<T>(string key)
        {
            string json = _database.StringGet(setKey(key));
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            T entity = JsonSerializer.Deserialize<T>(json, JsonConfig.Options);// JsonConvert.DeserializeObject<T>(json);
            return entity;
        }

        /// <summary>
        /// 获取一个字符串对象。
        /// </summary>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public string Get(string key)
        {
            return _database.StringGet(setKey(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetStringValue(string key)
        {
            return _database.StringGet(setKey(key));
        }

        /// <summary>
        /// 删除一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool Delete(string key)
        {
            return _database.KeyDelete(setKey(key));
        }

        /// <summary>
        /// 返回键是否存在。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回键是否存在。</returns>
        public bool Exists(string key)
        {
            return _database.KeyExists(setKey(key));
        }

        /// <summary>
        /// 返回键是否存在。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回键是否存在。</returns>
        public async Task<bool> ExistsAsync(string key)
        {
            return await _database.KeyExistsAsync(setKey(key));
        }

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool SetExpire(string key, TimeSpan? expiry)
        {
            return _database.KeyExpire(setKey(key), expiry);
        }

        /// <summary>
        /// 设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public bool SetExpire(string key, int seconds)
        {

            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            return _database.KeyExpire(setKey(key), expiry);
        }

        #endregion

        #region 异步方法...

        /// <summary>
        /// 异步添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            if (expiry != null)
            {
                return await _database.StringSetAsync(setKey(key), value, expiry.Value);
            }
            return await _database.StringSetAsync(setKey(key), value);
        }

        /// <summary>
        /// 异步添加一个字符串对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="value">值。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> SetAsync(string key, string value, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            return await _database.StringSetAsync(setKey(key), value, expiry);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public async Task<bool> SetObjectAsync<T>(string key, T value, int second)
        {
            var data = JsonSerializer.Serialize<T>(value, JsonConfig.Options);// JsonConvert.SerializeObject(value);
            return await _database.StringSetAsync(setKey(key), data, TimeSpan.FromSeconds(second));
        }

        /// <summary>
        /// 异步获取一个对象。
        /// </summary>
        /// <typeparam name="T">对象的类型。</typeparam>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public async Task<T> GetObjectAsync<T>(string key)
        {
            string json = await _database.StringGetAsync(setKey(key));
            if (string.IsNullOrWhiteSpace(json))
            {
                return default(T);
            }
            T entity = JsonSerializer.Deserialize<T>(json, JsonConfig.Options);// JsonConvert.DeserializeObject<T>(json);
            return entity;
        }

        /// <summary>
        /// 异步获取一个字符串对象。
        /// </summary>
        /// <param name="key">值。</param>
        /// <returns>返回对象的值。</returns>
        public async Task<string> GetAsync(string key)
        {
            return await _database.StringGetAsync(setKey(key));
        }

        /// <summary>
        /// 异步删除一个对象。
        /// </summary>
        /// <param name="key">键。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> DeleteAsync(string key)
        {
            return await _database.KeyDeleteAsync(setKey(key));
        }

        /// <summary>
        /// 异步设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> SetExpireAsync(string key, int seconds)
        {
            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
            return await _database.KeyExpireAsync(setKey(key), expiry);
        }

        /// <summary>
        /// 异步设置一个键的过期时间。
        /// </summary>
        /// <param name="key">键。</param>
        /// <param name="expiry">过期时间（时间间隔）。</param>
        /// <returns>返回是否执行成功。</returns>
        public async Task<bool> SetExpireAsync(string key, TimeSpan? expiry)
        {
            return await _database.KeyExpireAsync(setKey(key), expiry);
        }

        #endregion

        #region 分布式锁...

        /// <summary>
        /// 分布式锁 Token。
        /// </summary>
        private static readonly RedisValue LockToken = Environment.MachineName;

        /// <summary>
        /// 是redis缓存
        /// </summary>
        public bool IsRedis => true;

        /// <summary>
        /// 获取锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否已锁。</returns>
        public bool Lock(string key, int seconds)
        {
            return _database.LockTake(setKey(key), LockToken, TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 释放锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <returns>是否成功。</returns>
        public bool UnLock(string key)
        {
            return _database.LockRelease(setKey(key), LockToken);
        }

        /// <summary>
        /// 异步获取锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <param name="seconds">过期时间（秒）。</param>
        /// <returns>是否成功。</returns>
        public async Task<bool> LockAsync(string key, int seconds)
        {
            return await _database.LockTakeAsync(setKey(key), LockToken, TimeSpan.FromSeconds(seconds));
        }

        /// <summary>
        /// 异步释放锁。
        /// </summary>
        /// <param name="key">锁名称。</param>
        /// <returns>是否成功。</returns>
        public async Task<bool> UnLockAsync(string key)
        {
            return await _database.LockReleaseAsync(setKey(key), LockToken);
        }

        #endregion

        #region Hash
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<HashKeyValue> HashGetAll(string key)
        {

            //return  _database.HashGetAll(setKey(key));
            var hentry = _database.HashGetAll(setKey(key));
            var result = new List<HashKeyValue>();
            foreach (var h in hentry)
            {
                result.Add(new HashKeyValue()
                {
                    Key = h.Name,
                    Value = h.Value
                });
            }
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public HashEntry[] HashGetAllEntry(string key)
        {
            return _database.HashGetAll(setKey(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<HashEntry[]> HashGetAllEntryAsync(string key)
        {
            return await _database.HashGetAllAsync(setKey(key));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<List<HashKeyValue>> HashGetAllAsync(string key)
        {
            var hentry = await _database.HashGetAllAsync(setKey(key));
            var result = new List<HashKeyValue>();
            foreach (var h in hentry)
            {
                result.Add(new HashKeyValue()
                {
                    Key = h.Name,
                    Value = h.Value
                });
            }
            return result;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        public void HashSet(string key, HashEntry[] vals)
        {
            _database.HashSet(setKey(key), vals);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        /// <param name="isnum"></param>
        /// <returns></returns>
        public async Task HashSetAsync(string key, HashEntry[] vals, bool isnum = false)
        {
            await _database.HashSetAsync(setKey(key), vals);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <param name="isnum"></param>
        /// <returns></returns>
        public async Task HashSetAsync(string key, string field, RedisValue value, bool isnum = false)
        {
            await _database.HashSetAsync(setKey(key), field, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sunkey"></param>
        /// <returns></returns>
        public string HashGet(string key, string sunkey)
        {
            return _database.HashGet(setKey(key), sunkey);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sunkey"></param>
        /// <param name="isnum"></param>
        /// <returns></returns>
        public async Task<string> HashGetAsync(string key, string sunkey, bool isnum = false)
        {
            return await _database.HashGetAsync(setKey(key), sunkey);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <param name="isinnum"></param>
        /// <returns></returns>
        public async Task<RedisValue[]> HashGetAsync(string key, RedisValue[] fields, bool isinnum = false)
        {
            return await _database.HashGetAsync(setKey(key), fields);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, string field)
        {
            return await _database.HashDeleteAsync(setKey(key), field);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public async Task<bool> HashDeleteAsync(string key, RedisValue[] fields)
        {
            await _database.HashDeleteAsync(setKey(key), fields);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool HashDelete(string key, string field)
        {
            return _database.HashDelete(setKey(key), field);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public async Task<long> HashIncrementAsync(string key, string field, long num = 1)
        {
            return await _database.HashIncrementAsync(setKey(key), field, num);
        }

        /// <summary>
        /// HashSet是否有数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public long HashLength(string key)
        {
            return _database.HashLength(setKey(key));
        }

        /// <summary>
        /// HashSet是否有数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<long> HashLengthAsync(string key)
        {
            return await _database.HashLengthAsync(setKey(key));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public long HashIncrement(string key, string field, long num = 1)
        {
            return _database.HashIncrement(setKey(key), field, num);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<long> HashDecrementAsync(string key, string field, long val = 1)
        {
            return await _database.HashDecrementAsync(setKey(key), field, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<long> HashDecrementAsync(string key, string field)
        {
            var val = 1;
            return await _database.HashDecrementAsync(setKey(key), field, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public long HashDecrement(string key, string field, long val = 1)
        {
            return _database.HashDecrement(setKey(key), field, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pattern"></param>
        /// <param name="page"></param>
        /// <param name="size"></param>
        /// <param name="isincom"></param>
        /// <returns></returns>
        public async Task<List<HashEntry>> HashScanAsync(string key, string pattern, int page, int size, bool isincom = true)
        {
            var offset = (page - 1) * size;
            IAsyncEnumerable<HashEntry> result = _database.HashScanAsync(key: setKey(key), pattern: pattern, pageSize: size, pageOffset: offset, cursor: offset);
            var rmlist = new List<HashEntry>();
            var enumerator = result.GetAsyncEnumerator();
            var iindex = 0;
            while (await enumerator.MoveNextAsync())
            {
                iindex++;
                if (iindex <= size)
                {
                    var item = enumerator.Current;
                    rmlist.Add(item);
                }
                else
                {
                    break;
                }
            }
            return rmlist;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<T> HashGetObjectAsync<T>(string key, string field)
        {
            var redisValue = await _database.HashGetAsync(setKey(key), field);
            if (redisValue.HasValue)
            {
                T entity = JsonSerializer.Deserialize<T>(redisValue.ToString(), JsonConfig.Options);// JsonConvert.DeserializeObject<T>(redisValue.ToString());
                return entity;
            }
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="isNum"></param>
        /// <returns></returns>
        public async Task<int> HashGetIntFieldAsync(string key, string field, bool isNum = false)
        {
            var redisValue = await _database.HashGetAsync(setKey(key), field);
            if (redisValue.HasValue)
            {
                int.TryParse(redisValue.ToString(), out var ival);
                return ival;
            }
            return default(int);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="isNum"></param>
        /// <returns></returns>
        public async Task<long> HashGetLongFieldAsync(string key, string field, bool isNum = false)
        {
            var redisValue = await _database.HashGetAsync(setKey(key), field);
            if (redisValue.HasValue)
            {
                long.TryParse(redisValue.ToString(), out var ival);
                return ival;
            }
            return default(long);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task<bool> HashSetObjectAsync<T>(string key, string field, T entity)
        {
            var jsonString = JsonSerializer.Serialize<T>(entity, JsonConfig.Options);// JsonConvert.SerializeObject(entity);
            return await _database.HashSetAsync(setKey(key), field, jsonString);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public bool HashExists(string key, string field)
        {
            return _database.HashExists(setKey(key), field);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<bool> HashExistsAsync(string key, string field)
        {
            return await _database.HashExistsAsync(setKey(key), field);
        }

        #endregion

        #region

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <param name="start"></param>
        ///// <param name="stop"></param>
        ///// <returns></returns>
        //public async Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(string key, int start = 0, int stop = 1)
        //{
        //    return await _database.SortedSetRangeByRankWithScoresAsync(setKey(key), start, stop);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public async Task<RedisValue[]> SortedSetRangeByScoreAsync(string key, long start = 0, long stop = 1)
        {
            return await _database.SortedSetRangeByScoreAsync(setKey(key), start, stop, order: Order.Descending);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> SortedSetIncrementAsync(string key, string field, long value)
        {
            var _read = await _database.SortedSetIncrementAsync(setKey(key), field, (long)value);
            return (long)_read;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<long> SortedSetDecrementAsync(string key, string field, long value)
        {
            var _read = await _database.SortedSetDecrementAsync(setKey(key), field, (long)value);
            return (long)_read;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetRemoveAsync(string key, string field)
        {
            return await _database.SortedSetRemoveAsync(setKey(key), field);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<bool> SortedSetAddAsync(string key, string field, long value)
        {
            return await _database.SortedSetAddAsync(setKey(key), field, value);
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public async Task<long> StringBitPositionAsync(string key, int start, int end, bool bit)
        {
            return await _database.StringBitPositionAsync(setKey(key), bit, start, end);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public async Task<long> StringBitCountAsync(string key, long start = 0, long end = -1)
        {
            return await _database.StringBitCountAsync(setKey(key), start, end);
        }

        /// <summary>
        /// 获取第一次出现的位置
        /// </summary>
        /// <param name="key"></param>
        /// <param name="offset"></param>
        /// <param name="bit"></param>
        /// <returns></returns>
        public async Task<bool> StringSetBitAsync(string key, long offset, bool bit)
        {
            return await _database.StringSetBitAsync(setKey(key), offset, bit);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        public void HashSet(string key, List<HashKeyValue> vals)
        {
            var list = new List<HashEntry>();
            foreach (var item in vals)
            {
                list.Add(new HashEntry(item.Key, item.Value) { });
            }

            _database.HashSet(setKey(key), list.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        /// <param name="isnum"></param>
        /// <returns></returns>
        public async Task HashSetAsync(string key, List<HashKeyValue> vals, bool isnum = false)
        {
            var list = new List<HashEntry>();
            foreach (var item in vals)
            {
                list.Add(new HashEntry(item.Key, item.Value) { });
            }

            await _database.HashSetAsync(setKey(key), list.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public bool SetValue(string key, string val)
        {
            return Set(key, val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            return "is redis!";
        }



        //--------------------------------------------------------------------------------



        #region 同步方法...



        #endregion

        #region 异步方法...


        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public async Task BatchRemoveAsync(string[] keys)
        {
            var batchCommand = _database.CreateBatch();
            var redisKeys = keys.Select(x => new RedisKey(setKey(x))).ToArray();
            await batchCommand.KeyDeleteAsync(redisKeys);
        }



        #endregion

        #region 分布式锁...


        #endregion

        #region Hash

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<HashEntry[]> HashReadAllAsync(string key)
        {
            return await _database.HashGetAllAsync(setKey(key));
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        public async Task HashSetAsync(string key, HashEntry[] vals)
        {
            await _database.HashSetAsync(setKey(key), vals);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public async Task<long> HashIncrementAsync(string key, RedisValue field, long num = 1)
        {
            return await _database.HashIncrementAsync(setKey(key), field, num);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public async Task<long> HashDecrementAsync(string key, RedisValue field, long num = 1)
        {
            return await _database.HashDecrementAsync(setKey(key), field, num);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sunkey"></param>
        /// <returns></returns>
        public async Task<string> HashGetAsync(string key, string sunkey)
        {
            return await _database.HashGetAsync(setKey(key), sunkey);
        }

        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public async Task HashSetAsync(string key, List<HashKeyValue> vals)
        {
            var list = new List<HashEntry>();
            foreach (var item in vals)
            {
                list.Add(new HashEntry(item.Key, item.Value) { });
            }

            await _database.HashSetAsync(setKey(key), list.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public async Task HashIncrementAsync(string key, string field, int num)
        {
            await _database.HashIncrementAsync(setKey(key), field, num);
        }


        /// <summary>
        /// 写入有序排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="score"></param>
        public async Task SortedSetAddAsync(string key, RedisValue field, int score)
        {
            await _database.SortedSetAddAsync(setKey(key), field, score);
        }

        /// <summary>
        /// 批量写入有序排序
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        public async Task SortedSetAddAsync(string key, SortedSetEntry[] fields)
        {
            await _database.SortedSetAddAsync(setKey(key), fields);
        }


        /// <summary>
        /// 从hash中获取随机一个！！！
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<HashEntry> HashRandomFieldsWithValuesAsync(string key)
        {
            var reads = await _database.HashRandomFieldsWithValuesAsync(setKey(key), 1);
            if (reads?.Length > 0)
            {
                return reads.First();
            }
            return default;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pattern"></param>
        /// <param name="size"></param>
        /// <param name="cursor"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public async Task<HashEntry[]> HashScanAsync(string key, string pattern, int size = 1, int cursor = 0, int offset = 0)
        {
            var results = new List<HashEntry>();
            var reads = _database.HashScanAsync(setKey(key), pattern, size, cursor, offset);
            await foreach (var item in reads)
            {
                results.Add(item);
                if (results.Count >= size)
                {
                    break;
                }
            }
            return results.ToArray();
        }


        /// <summary>
        /// 按照分数升序，获取第一个分数 >= 指定分数的数据并删除
        /// </summary>
        /// <param name="key"></param>
        /// <param name="minScore">最低分数</param>
        /// <returns></returns>
        public async Task<SortedSetEntry?> SortedSetStartPopScoreAsync(string key, int minScore)
        {
            var luaScript = @"
                    local members = redis.call('ZRANGEBYSCORE', KEYS[1], ARGV[1], '+inf', 'WITHSCORES', 'LIMIT', 0, 1)
                    if #members == 0 then
                        return nil
                    end
                    redis.call('ZREM', KEYS[1], members[1])
                    return {members[1], members[2]}
                ";
            var result = await _database.ScriptEvaluateAsync(luaScript,
                new RedisKey[] { setKey(key) },
                new RedisValue[] { minScore });
            if (result.IsNull)
            {
                return null;
            }
            var resultArray = (RedisResult[])result;
            if (resultArray.Length >= 2)
            {
                string element = (string)resultArray[0];
                double redisScore = (double)resultArray[1];
                // 将double转换为int（根据您的需求）
                int scoreValue = (int)redisScore;
                return new SortedSetEntry(element, scoreValue);
            }
            return null;
        }

        /// <summary>
        /// 返回分数符合的第一个元素，返回并删除这个元素
        /// </summary>
        public async Task<SortedSetEntry?> SortedSetPopExactScoreAsync(string key, int targetScore)
        {
            var luaScript = @"
                    local members = redis.call('ZRANGEBYSCORE', KEYS[1], ARGV[1], ARGV[1], 'WITHSCORES', 'LIMIT', 0, 1)
                    if #members == 0 then
                        return nil
                    end
                    redis.call('ZREM', KEYS[1], members[1])
                    return {members[1], members[2]}
                ";

            var result = await _database.ScriptEvaluateAsync(luaScript,
                new RedisKey[] { setKey(key) },
                new RedisValue[] { targetScore });

            if (result.IsNull)
            {
                return null;
            }

            var resultArray = (RedisResult[])result;
            if (resultArray.Length >= 2)
            {
                string element = (string)resultArray[0];
                double redisScore = (double)resultArray[1];
                int scoreValue = (int)redisScore;

                return new SortedSetEntry(element, scoreValue);
            }

            return null;
        }


        #region -----------------blog--------------------------------


        #region Hash模块

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<string[]> HashKeysAsync(string key)
        {
            var keys = await _database.HashKeysAsync(setKey(key));
            var list = new List<string>();
            foreach (var item in keys)
            {
                list.Add(item.ToString());
            }
            return list.ToArray();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<string> ReadStringAsync(string key)
        {
            return Task.Run<string>((Func<Task<string>>)(async () =>
            {
                return await this._database.StringGetAsync(setKey(key));
            }));

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<T> ReadObjectAsync<T>(string key)
        {

            string str = await _database.StringGetAsync(setKey(key));
            if (!string.IsNullOrEmpty(str))
            {
                return JsonSerializer.Deserialize<T>(str, JsonConfig.Options);// Newtonsoft.Json.JsonConvert.DeserializeObject<T>(str);
            }
            return default(T);


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<bool> SetAddAsync(string skey, int val)
        {
            return await _database.SetAddAsync(setKey(skey), val);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        public async Task<bool> SetContainsAsync(string skey, int filed)
        {
            return await _database.SetContainsAsync(setKey(skey), filed);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        public async Task<bool> SetRemoveAsync(string skey, int filed)
        {
            return await _database.SetRemoveAsync(setKey(skey), filed);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public async Task<long> SetRandomMemberAsync(string skey)
        {
            (await _database.SetRandomMemberAsync(setKey(skey))).TryParse(out long val);
            return val;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public async Task<long> SetLengthAsync(string skey)
        {
            return (await _database.SetLengthAsync(setKey(skey)));
        }

        /// <summary>
        /// 获取排序中的第一个field并删除
        /// type.action.randcode.id
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public async Task<string> SortedSetRangeByRankAsync(string skey)
        {
            var _read = await _database.SortedSetRangeByRankAsync(setKey(skey), 0, 1);
            if (_read != null && _read.Length > 0)
            {
                var _current = _read[0];
                await _database.SortedSetRemoveAsync(setKey(skey), _current);
                return _current.ToString();
            }
            return String.Empty;
        }

        /// <summary>
        /// 返回最近的一个！并删除
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="_time"></param>
        /// <returns></returns>
        public async Task<string> SortedSetRangeByScoreWithScoresAsync(string skey, long _time)
        {
            var reads = await _database.SortedSetRangeByScoreWithScoresAsync(setKey(skey), 0, _time, take: 1);
            if (reads != null && reads.Length > 0)
            {
                var _field = reads[0].Element.ToString();
                await _database.SortedSetRemoveAsync(setKey(skey), _field);
                return _field;
            }
            return String.Empty;
        }

        /// <summary>
        /// 返回最近的一个！并删除
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="_time">yyyyMMddHHmmss</param>
        /// <returns></returns>
        public async Task<string> SortedSetRangeByScoreAsync(string skey, long _time)
        {
            var reads = await _database.SortedSetRangeByScoreAsync(setKey(skey), 0, _time, take: 1);
            if (reads != null && reads.Length > 0)
            {
                var _field = reads[0].ToString();
                await _database.SortedSetRemoveAsync(setKey(skey), _field);
                return _field;
            }
            return String.Empty;
        }

        /// <summary>
        /// 获取当前排期最长的任务时间戳 如果没有则返回当前时间戳
        /// </summary>
        /// <param name="skey"></param>
        /// <returns>时间戳秒</returns>
        public async Task<long> SortedSetRangeByRankWithScoresAsync(string skey)
        {
            var _read = await _database.SortedSetRangeByRankWithScoresAsync(setKey(skey), 0, 1, Order.Descending);
            if (_read != null && _read.Length > 0)
            {
                var _current = _read[0];
                return (long)_current.Score;
            }
            return DateTime.Now.ToUnixTimeSeconds();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public List<string> ListRange(string skey, int start = 0, int stop = 30)
        {
            var vals = _database.ListRange(setKey(skey), start, stop);
            if (vals != null && vals.Length > 0)
            {
                var strs = new List<string>();
                foreach (var item in vals)
                {
                    if (item.HasValue)
                    {
                        strs.Add(item.ToString());
                    }
                }
                return strs;
            }
            return new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public async Task<List<string>> ListRangeAsync(string skey, int start = 0, int stop = 30)
        {
            var vals = await _database.ListRangeAsync(setKey(skey), start, stop);
            if (vals != null && vals.Length > 0)
            {
                var strs = new List<string>();
                foreach (var item in vals)
                {
                    if (item.HasValue)
                    {
                        strs.Add(item.ToString());
                    }
                }
                return strs;
            }
            return new List<string>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public List<T> ListRange<T>(string skey, int start = 0, int stop = 30)
        {
            var vals = _database.ListRange(setKey(skey), start, stop);
            if (vals != null && vals.Length > 0)
            {
                var strs = new List<T>();
                foreach (var item in vals)
                {
                    if (item.HasValue)
                    {
                        strs.Add(JsonSerializer.Deserialize<T>(item.ToString(), JsonConfig.Options));// Newtonsoft.Json.JsonConvert.DeserializeObject<T>(item.ToString()));
                    }
                }
                return strs;
            }
            return new List<T>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public async Task<List<T>> ListRangeAsync<T>(string skey, int start = 0, int stop = 30)
        {
            var vals = await _database.ListRangeAsync(setKey(skey), start, stop);
            if (vals != null && vals.Length > 0)
            {
                var strs = new List<T>();
                foreach (var item in vals)
                {
                    if (item.HasValue)
                    {
                        strs.Add(JsonSerializer.Deserialize<T>(item.ToString(), JsonConfig.Options));
                    }
                }
                return strs;
            }
            return new List<T>();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="sval"></param>
        /// <returns></returns>
        public async Task<long> ListRightPushAsync(string skey, string sval)
        {
            return await _database.ListRightPushAsync(setKey(skey), sval);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public async Task<long> ListLengthAsync(string skey)
        {
            return await _database.ListLengthAsync(setKey(skey));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public async Task<string> ListLeftPopAsync(string skey)
        {
            return await _database.ListLeftPopAsync(setKey(skey));
        }




        #endregion



        #endregion



    }



}
