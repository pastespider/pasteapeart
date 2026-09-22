//using Microsoft.Extensions.Options;
//using Newtonsoft.Json;
//using StackExchange.Redis;

//namespace ExampleForm.Handler
//{
//    /// <summary>
//    /// 封装 Redis 相关操作的方法类。
//    /// </summary>
//    public class RedisClient : IAppCache
//    {

//        //private readonly RedisConfig _redisconfig;
//        //private IDatabase _database { get { return _mainconnect.Value.GetDatabase(); } }
//        //private Lazy<ConnectionMultiplexer> _mainconnect;
//        //private string PrefixHead = String.Empty;
//        //public RedisClient(IOptions<RedisConfig> options)
//        //{
//        //    _redisconfig = options.Value;
//        //    var oaconfig = ConfigurationOptions.Parse(_redisconfig.MainConnection);
//        //    PrefixHead = _redisconfig.Prefix;
//        //    oaconfig.ClientName = _redisconfig.ClientName;
//        //    _mainconnect = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(oaconfig));
//        //}


//        private readonly RedisConfig _redisconfig;
//        private readonly Lazy<ConnectionMultiplexer>[] _connectionPool;
//        private int _currentConnectionIndex = 0;
//        private readonly object _lock = new object();

//        /// <summary>
//        /// Key的前缀
//        /// </summary>
//        private string PrefixHead = string.Empty;

//        public RedisClient(IOptions<RedisConfig> options)
//        {
//            _redisconfig = options.Value;
//            PrefixHead = _redisconfig.Prefix;

//            // 创建4个连接的池
//            _connectionPool = new Lazy<ConnectionMultiplexer>[4];

//            for (int i = 0; i < _connectionPool.Length; i++)
//            {
//                var config = ConfigurationOptions.Parse(_redisconfig.MainConnection);
//                config.ClientName = $"{_redisconfig.ClientName}_{i}"; // 为每个连接添加序号

//                _connectionPool[i] = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(config));
//            }
//        }

//        // 使用轮询方式获取连接
//        private IDatabase GetDatabase()
//        {
//            lock (_lock)
//            {
//                // 轮询选择连接
//                var connection = _connectionPool[_currentConnectionIndex];
//                _currentConnectionIndex = (_currentConnectionIndex + 1) % _connectionPool.Length;

//                return connection.Value.GetDatabase();
//            }
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        private IDatabase _database { get { return GetDatabase(); } }


//        private string setKey(string key)
//        {
//            return PrefixHead + key;
//        }



//        #region 同步方法...

//        /// <summary>
//        /// 添加一个字符串对象。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <param name="value">值。</param>
//        /// <param name="expiry">过期时间（时间间隔）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public bool Set(string key, string value, TimeSpan? expiry = null)
//        {
//            return _database.StringSet(setKey(key), value, expiry);
//        }

//        /// <summary>
//        /// 设置值
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="val"></param>
//        /// <param name="second"></param>
//        /// <returns></returns>
//        public bool SetValue(string key, string val, int second)
//        {
//            return _database.StringSet(setKey(key), val, TimeSpan.FromSeconds(second));
//        }

//        /// <summary>
//        /// 添加一个字符串对象。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <param name="value">值。</param>
//        /// <param name="seconds">过期时间（秒）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public bool Set(string key, string value, int seconds)
//        {
//            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
//            return _database.StringSet(setKey(key), value, expiry);
//        }

//        /// <summary>
//        /// 添加一个对象。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <typeparam name="T">对象的类型。</typeparam>
//        /// <param name="value">值。</param>
//        /// <param name="expiry">过期时间（时间间隔）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public bool SetObject<T>(string key, T value, TimeSpan? expiry = null)
//        {
//            var data = JsonConvert.SerializeObject(value);
//            return _database.StringSet(setKey(key), data, expiry);
//        }

//        /// <summary>
//        /// 添加一个对象。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <typeparam name="T">对象的类型。</typeparam>
//        /// <param name="value">值。</param>
//        /// <param name="seconds">过期时间（秒）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public bool SetObject<T>(string key, T value, int seconds)
//        {
//            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
//            var data = JsonConvert.SerializeObject(value);
//            return _database.StringSet(setKey(key), data, expiry);
//        }

//        /// <summary>
//        /// 获取一个对象。
//        /// </summary>
//        /// <param name="key">值。</param>
//        /// <returns>返回对象的值。</returns>
//        public T GetObject<T>(string key)
//        {
//            string json = _database.StringGet(setKey(key));
//            if (string.IsNullOrWhiteSpace(json))
//            {
//                return default(T);
//            }
//            T entity = JsonConvert.DeserializeObject<T>(json);
//            return entity;
//        }

//        /// <summary>
//        /// 获取一个字符串对象。
//        /// </summary>
//        /// <param name="key">值。</param>
//        /// <returns>返回对象的值。</returns>
//        public string Get(string key)
//        {
//            return _database.StringGet(setKey(key));
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public string GetStringValue(string key)
//        {
//            return _database.StringGet(setKey(key));
//        }

//        /// <summary>
//        /// 删除一个对象。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public bool Delete(string key)
//        {
//            return _database.KeyDelete(setKey(key));
//        }

//        /// <summary>
//        /// 返回键是否存在。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <returns>返回键是否存在。</returns>
//        public bool Exists(string key)
//        {
//            return _database.KeyExists(setKey(key));
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public async Task<bool> ExistsAsync(string key)
//        {
//            return await _database.KeyExistsAsync(setKey(key));
//        }

//        /// <summary>
//        /// 设置一个键的过期时间。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <param name="expiry">过期时间（时间间隔）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public bool SetExpire(string key, TimeSpan? expiry)
//        {
//            return _database.KeyExpire(setKey(key), expiry);
//        }

//        /// <summary>
//        /// 设置一个键的过期时间。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <param name="seconds">过期时间（秒）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public bool SetExpire(string key, int seconds)
//        {

//            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
//            return _database.KeyExpire(setKey(key), expiry);
//        }

//        #endregion

//        #region 异步方法...

//        /// <summary>
//        /// 异步添加一个字符串对象。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <param name="value">值。</param>
//        /// <param name="expiry">过期时间（时间间隔）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public async Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
//        {
//            return await _database.StringSetAsync(setKey(key), value, expiry);
//        }

//        /// <summary>
//        /// 异步添加一个字符串对象。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <param name="value">值。</param>
//        /// <param name="seconds">过期时间（秒）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public async Task<bool> SetAsync(string key, string value, int seconds)
//        {
//            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
//            return await _database.StringSetAsync(setKey(key), value, expiry);
//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="key"></param>
//        /// <param name="value"></param>
//        /// <param name="second"></param>
//        /// <returns></returns>
//        public async Task<bool> SetObjectAsync<T>(string key, T value, int second)
//        {
//            var data = JsonConvert.SerializeObject(value);
//            return await _database.StringSetAsync(setKey(key), data, TimeSpan.FromSeconds(second));
//        }

//        /// <summary>
//        /// 异步获取一个对象。
//        /// </summary>
//        /// <typeparam name="T">对象的类型。</typeparam>
//        /// <param name="key">值。</param>
//        /// <returns>返回对象的值。</returns>
//        public async Task<T> GetObjectAsync<T>(string key)
//        {
//            string json = await _database.StringGetAsync(setKey(key));
//            if (string.IsNullOrWhiteSpace(json))
//            {
//                return default(T);
//            }
//            T entity = JsonConvert.DeserializeObject<T>(json);
//            return entity;
//        }

//        /// <summary>
//        /// 异步获取一个字符串对象。
//        /// </summary>
//        /// <param name="key">值。</param>
//        /// <returns>返回对象的值。</returns>
//        public async Task<string> GetAsync(string key)
//        {
//            return await _database.StringGetAsync(setKey(key));
//        }

//        /// <summary>
//        /// 异步删除一个对象。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public async Task<bool> DeleteAsync(string key)
//        {
//            return await _database.KeyDeleteAsync(setKey(key));
//        }

//        /// <summary>
//        /// 异步设置一个键的过期时间。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <param name="seconds">过期时间（秒）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public async Task<bool> SetExpireAsync(string key, int seconds)
//        {
//            TimeSpan expiry = TimeSpan.FromSeconds(seconds);
//            return await _database.KeyExpireAsync(setKey(key), expiry);
//        }

//        /// <summary>
//        /// 异步设置一个键的过期时间。
//        /// </summary>
//        /// <param name="key">键。</param>
//        /// <param name="expiry">过期时间（时间间隔）。</param>
//        /// <returns>返回是否执行成功。</returns>
//        public async Task<bool> SetExpireAsync(string key, TimeSpan? expiry)
//        {
//            return await _database.KeyExpireAsync(setKey(key), expiry);
//        }

//        #endregion

//        #region 分布式锁...

//        /// <summary>
//        /// 分布式锁 Token。
//        /// </summary>
//        private static readonly RedisValue LockToken = Environment.MachineName;

//        /// <summary>
//        /// 是redis缓存
//        /// </summary>
//        public bool IsRedis => true;

//        /// <summary>
//        /// 获取锁。
//        /// </summary>
//        /// <param name="key">锁名称。</param>
//        /// <param name="seconds">过期时间（秒）。</param>
//        /// <returns>是否已锁。</returns>
//        public bool Lock(string key, int seconds)
//        {
//            return _database.LockTake(setKey(key), LockToken, TimeSpan.FromSeconds(seconds));
//        }

//        /// <summary>
//        /// 释放锁。
//        /// </summary>
//        /// <param name="key">锁名称。</param>
//        /// <returns>是否成功。</returns>
//        public bool UnLock(string key)
//        {
//            return _database.LockRelease(setKey(key), LockToken);
//        }

//        /// <summary>
//        /// 异步获取锁。
//        /// </summary>
//        /// <param name="key">锁名称。</param>
//        /// <param name="seconds">过期时间（秒）。</param>
//        /// <returns>是否成功。</returns>
//        public async Task<bool> LockAsync(string key, int seconds)
//        {
//            return await _database.LockTakeAsync(setKey(key), LockToken, TimeSpan.FromSeconds(seconds));
//        }

//        /// <summary>
//        /// 异步释放锁。
//        /// </summary>
//        /// <param name="key">锁名称。</param>
//        /// <returns>是否成功。</returns>
//        public async Task<bool> UnLockAsync(string key)
//        {
//            return await _database.LockReleaseAsync(setKey(key), LockToken);
//        }

//        #endregion

//        #region Hash
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public List<HashKeyValue> HashGetAll(string key)
//        {

//            //return  _database.HashGetAll(setKey(key));

//            var hentry = _database.HashGetAll(setKey(key));
//            var result = new List<HashKeyValue>();
//            foreach (var h in hentry)
//            {
//                result.Add(new HashKeyValue()
//                {
//                    Key = h.Name,
//                    Value = h.Value
//                });
//            }
//            return result;

//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public async Task<List<HashKeyValue>> HashGetAllAsync(string key)
//        {
//            var hentry = await _database.HashGetAllAsync(setKey(key));
//            var result = new List<HashKeyValue>();
//            foreach (var h in hentry)
//            {
//                result.Add(new HashKeyValue()
//                {
//                    Key = h.Name,
//                    Value = h.Value
//                });
//            }
//            return result;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public async Task<HashEntry[]> HashReadAllAsync(string key)
//        {
//            return await _database.HashGetAllAsync(setKey(key));
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="vals"></param>
//        public void HashSet(string key, HashEntry[] vals)
//        {
//            _database.HashSet(setKey(key), vals);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="vals"></param>
//        public async Task HashSetAsync(string key, HashEntry[] vals)
//        {
//            await _database.HashSetAsync(setKey(key), vals);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <param name="num"></param>
//        /// <returns></returns>
//        public async Task HashIncrementAsync(string key, RedisValue field, int num = 1)
//        {
//            await _database.HashIncrementAsync(setKey(key), field, num);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <param name="num"></param>
//        /// <returns></returns>
//        public async Task HashDecrementAsync(string key, RedisValue field, int num = 1)
//        {
//            await _database.HashDecrementAsync(setKey(key), field, num);
//        }

//        public string HashGet(string key, string sunkey)
//        {
//            return _database.HashGet(setKey(key), sunkey);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="sunkey"></param>
//        /// <returns></returns>
//        public async Task<string> HashGetAsync(string key, string sunkey)
//        {
//            return await _database.HashGetAsync(setKey(key), sunkey);
//        }

//        #endregion

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <param name="bit"></param>   
//        /// <returns></returns>
//        public long StringBitPosition(string key, int start, int end, bool bit)
//        {
//            return _database.StringBitPosition(setKey(key), bit, start, end);
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <param name="bit"></param>
//        /// <returns></returns>
//        public async Task<long> StringBitPositionAsync(string key, int start, int end, bool bit)
//        {
//            return await _database.StringBitPositionAsync(setKey(key), bit, start, end);
//        }


//        /// <summary>
//        /// 获取个数?
//        /// </summary>
//        /// <param name="deskey"></param>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public long StringBitOperation(string deskey, string key)
//        {
//            return _database.StringBitOperation(Bitwise.And, setKey(deskey), setKey(key));
//        }


//        /// <summary>
//        /// 设置值
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="offset"></param>
//        /// <param name="bit"></param>
//        /// <returns></returns>
//        public bool StringSetBit(string key, long offset, bool bit)
//        {
//            return _database.StringSetBit(setKey(key), offset, bit);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        public long StringBitCount(string key, long start = 0, long end = -1)
//        {
//            return _database.StringBitCount(setKey(key), start, end);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="start"></param>
//        /// <param name="end"></param>
//        /// <returns></returns>
//        public async Task<long> StringBitCountAsync(string key, long start = 0, long end = -1)
//        {
//            return await _database.StringBitCountAsync(setKey(key), start, end);
//        }

//        /// <summary>
//        /// 获取第一次出现的位置
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="offset"></param>
//        /// <param name="bit"></param>
//        /// <returns></returns>
//        public async Task<bool> StringSetBitAsync(string key, long offset, bool bit)
//        {
//            return await _database.StringSetBitAsync(setKey(key), offset, bit);
//        }

//        public void HashSet(string key, List<HashKeyValue> vals)
//        {
//            var list = new List<HashEntry>();
//            foreach (var item in vals)
//            {
//                list.Add(new HashEntry(item.Key, item.Value) { });
//            }

//            _database.HashSet(setKey(key), list.ToArray());
//        }

//        public async Task HashSetAsync(string key, List<HashKeyValue> vals)
//        {
//            var list = new List<HashEntry>();
//            foreach (var item in vals)
//            {
//                list.Add(new HashEntry(item.Key, item.Value) { });
//            }

//            await _database.HashSetAsync(setKey(key), list.ToArray());
//        }

//        public bool SetValue(string key, string val)
//        {
//            return Set(key, val);
//        }

//        /// <summary>
//        /// 写入有序排序
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <param name="score"></param>
//        public async Task SortedSetAddAsync(string key, RedisValue field, int score)
//        {
//            await _database.SortedSetAddAsync(setKey(key), field, score);
//        }

//        /// <summary>
//        /// 批量写入有序排序
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="fields"></param>
//        public async Task SortedSetAddAsync(string key, SortedSetEntry[] fields)
//        {
//            await _database.SortedSetAddAsync(setKey(key), fields);
//        }


//        /// <summary>
//        /// 按照分数升序，获取第一个分数 >= 指定分数的数据并删除
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="minScore">最低分数</param>
//        /// <returns></returns>
//        public async Task<SortedSetEntry?> SortedSetStartPopScoreAsync(string key, int minScore)
//        {
//            var luaScript = @"
//                    local members = redis.call('ZRANGEBYSCORE', KEYS[1], ARGV[1], '+inf', 'WITHSCORES', 'LIMIT', 0, 1)
//                    if #members == 0 then
//                        return nil
//                    end
//                    redis.call('ZREM', KEYS[1], members[1])
//                    return {members[1], members[2]}
//                ";

//            var result =await _database.ScriptEvaluateAsync(luaScript,
//                new RedisKey[] { setKey(key) },
//                new RedisValue[] { minScore });

//            if (result.IsNull)
//            {
//                return null;
//            }

//            var resultArray = (RedisResult[])result;
//            if (resultArray.Length >= 2)
//            {
//                string element = (string)resultArray[0];
//                double redisScore = (double)resultArray[1];

//                // 将double转换为int（根据您的需求）
//                int scoreValue = (int)redisScore;

//                return new SortedSetEntry(element, scoreValue);
//            }

//            return null;
//        }

//        /// <summary>
//        /// 返回分数符合的第一个元素，返回并删除这个元素
//        /// </summary>
//        public async Task<SortedSetEntry?> SortedSetPopExactScoreAsync(string key, int targetScore)
//        {
//            var luaScript = @"
//                    local members = redis.call('ZRANGEBYSCORE', KEYS[1], ARGV[1], ARGV[1], 'WITHSCORES', 'LIMIT', 0, 1)
//                    if #members == 0 then
//                        return nil
//                    end
//                    redis.call('ZREM', KEYS[1], members[1])
//                    return {members[1], members[2]}
//                ";

//            var result = await _database.ScriptEvaluateAsync(luaScript,
//                new RedisKey[] { setKey(key) },
//                new RedisValue[] { targetScore });

//            if (result.IsNull)
//            {
//                return null;
//            }

//            var resultArray = (RedisResult[])result;
//            if (resultArray.Length >= 2)
//            {
//                string element = (string)resultArray[0];
//                double redisScore = (double)resultArray[1];
//                int scoreValue = (int)redisScore;

//                return new SortedSetEntry(element, scoreValue);
//            }

//            return null;
//        }


//    }



//}
