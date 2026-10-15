using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;
using System.Collections.Concurrent;
using System.Text;
using System.Text.Json;

namespace PasteApeart.Handler.redis
{
    /// <summary>
    /// 本地缓存
    /// </summary>
    public class MemoryClient : IAppCache
    {
        private readonly IMemoryCache _memoryCache;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="memoryCache"></param>
        public MemoryClient(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
            key_hash_set = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
            key_hash_num = new ConcurrentDictionary<string, ConcurrentDictionary<string, long>>();
            key_set = new ConcurrentDictionary<string, ConcurrentDictionary<int, byte>>();
            key_list = new ConcurrentDictionary<string, List<string>>();
        }

        private ConcurrentDictionary<string, ConcurrentDictionary<string, string>> key_hash_set;
        private ConcurrentDictionary<string, ConcurrentDictionary<string, long>> key_hash_num;

        /// <summary>
        /// Set(整数集合) 支持
        /// </summary>
        private ConcurrentDictionary<string, ConcurrentDictionary<int, byte>> key_set;

        /// <summary>
        /// List 支持
        /// </summary>
        private ConcurrentDictionary<string, List<string>> key_list;

        /// <summary>
        /// List 操作锁
        /// </summary>
        private readonly object _listLock = new object();
        /// <summary>
        /// 
        /// </summary>
        public bool IsRedis { get => false; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Delete(string key)
        {
            _memoryCache.Remove(key);
            key_hash_set.TryRemove(key, out _);
            key_hash_num.TryRemove(key, out _);
            key_set.TryRemove(key, out _);
            key_list.TryRemove(key, out _);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(string key)
        {
            return Task.FromResult(Delete(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool Exists(string key)
        {
            if (_memoryCache.TryGetValue(key, out var _)) return true;
            if (key_hash_set.ContainsKey(key)) return true;
            if (key_hash_num.ContainsKey(key)) return true;
            if (key_set.ContainsKey(key)) return true;
            if (key_list.ContainsKey(key)) return true;
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetObject<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out string val))
            {
                return JsonSerializer.Deserialize<T>(val, JsonConfig.Options);
            }
            return default(T);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string Get(string key)
        {
            if (_memoryCache.TryGetValue(key, out string val))
            {
                return val;
            }
            return "";
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<T> GetObjectAsync<T>(string key)
        {
            return Task.FromResult(GetObject<T>(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<string> GetAsync(string key)
        {
            return Task.FromResult(Get(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string GetStringValue(string key)
        {
            return Get(key);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sunkey"></param>
        /// <returns></returns>
        public string HashGet(string key, string sunkey)
        {
            if (key_hash_set.TryGetValue(key, out var val) && val.TryGetValue(sunkey, out var _val))
            {
                return _val;
            }
            if (key_hash_num.TryGetValue(key, out var numVal) && numVal.TryGetValue(sunkey, out var _numVal))
            {
                return _numVal.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public HashEntry[] HashGetAllEntry(string key)
        {
            if (key_hash_set.TryGetValue(key, out var val))
            {
                return val.Select(x => new HashEntry(x.Key, x.Value)).ToArray();
            }
            if (key_hash_num.TryGetValue(key, out var numVal))
            {
                return numVal.Select(x => new HashEntry(x.Key, x.Value)).ToArray();
            }
            return Array.Empty<HashEntry>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<HashEntry[]> HashGetAllEntryAsync(string key)
        {
            var result = HashGetAllEntry(key);
            return await Task.FromResult(result);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sunkey"></param>
        /// <param name="isnum"></param>
        /// <returns></returns>
        public Task<string> HashGetAsync(string key, string sunkey, bool isnum = false)
        {
            return Task.FromResult(HashGet(key, sunkey));
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
            var read = await this.HashGetAsync(key, field, isNum);
            if (!string.IsNullOrEmpty(read))
            {
                int.TryParse(read, out var ival);
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
            var read = await this.HashGetAsync(key, field, isNum);
            if (!string.IsNullOrEmpty(read))
            {
                long.TryParse(read, out var ival);
                return ival;
            }
            return default(long);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        public void HashSet(string key, HashEntry[] vals)
        {
            var dict = key_hash_set.GetOrAdd(key, _ => new ConcurrentDictionary<string, string>());
            foreach (var entry in vals)
            {
                dict[entry.Name.ToString()] = entry.Value.ToString();
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool Lock(string key, int seconds)
        {
            // 本地缓存模拟锁：若不存在则创建，存在则失败
            var lockKey = "__lock__" + key;
            if (_memoryCache.TryGetValue(lockKey, out _))
            {
                return false;
            }
            Set(lockKey, "1", seconds);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public Task<bool> LockAsync(string key, int seconds)
        {
            return Task.FromResult(Lock(key, seconds));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool Set(string key, string value, TimeSpan? expiry = null)
        {
            using (var wcache = _memoryCache.CreateEntry(key))
            {
                wcache.Value = value;
                if (expiry == null)
                {
                    wcache.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(30);
                }
                else
                {
                    wcache.AbsoluteExpiration = DateTimeOffset.Now.Add((TimeSpan)expiry);
                }
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool Set(string key, string value, int seconds)
        {
            using (var wcache = _memoryCache.CreateEntry(key))
            {
                wcache.Value = value;
                wcache.AbsoluteExpiration = DateTimeOffset.Now.AddSeconds(seconds);
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Set(string key, string value)
        {
            using (var wcache = _memoryCache.CreateEntry(key))
            {
                wcache.Value = value;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool SetObject<T>(string key, T value, TimeSpan? expiry = null)
        {
            return Set(key, JsonSerializer.Serialize<T>(value, JsonConfig.Options), expiry);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool SetObject<T>(string key, T value, int seconds)
        {
            return Set(key, JsonSerializer.Serialize<T>(value, JsonConfig.Options), seconds);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            return Task.FromResult(Set(key, value, expiry));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public Task<bool> SetAsync(string key, string value, int seconds)
        {
            return Task.FromResult(Set(key, value, seconds));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public Task<bool> SetObjectAsync<T>(string key, T value, int second)
        {
            return Task.FromResult(SetObject(key, value, second));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool SetExpire(string key, TimeSpan? expiry)
        {
            if (_memoryCache.TryGetValue(key, out string sval))
            {
                return Set(key, sval, expiry);
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public bool SetExpire(string key, int seconds)
        {
            if (_memoryCache.TryGetValue(key, out string sval))
            {
                return Set(key, sval, seconds);
            }
            return false;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        public Task<bool> SetExpireAsync(string key, int seconds)
        {
            return Task.FromResult(SetExpire(key, seconds));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public Task<bool> SetExpireAsync(string key, TimeSpan? expiry)
        {
            return Task.FromResult(SetExpire(key, expiry));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        public bool SetValue(string key, string val, int second)
        {
            return Set(key, val, second);
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
        /// <param name="key"></param>
        /// <returns></returns>
        public bool UnLock(string key)
        {
            _memoryCache.Remove("__lock__" + key);
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> UnLockAsync(string key)
        {
            return Task.FromResult(UnLock(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        public void HashSet(string key, List<HashKeyValue> vals)
        {
            var dict = key_hash_set.GetOrAdd(key, _ => new ConcurrentDictionary<string, string>());
            foreach (var entry in vals)
            {
                dict[entry.Key] = entry.Value;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Task<long> HashIncrementAsync(string key, string field, long num = 1)
        {
            var dict = key_hash_num.GetOrAdd(key, _ => new ConcurrentDictionary<string, long>());
            var result = dict.AddOrUpdate(field, num, (k, old) => old + num);
            return Task.FromResult(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public Task<RedisValue[]> SortedSetRangeByScoreAsync(string key, long start = 0, long stop = 1)
        {
            if (key_hash_num.TryGetValue(key, out var _hash))
            {
                var list = new List<RedisValue>();
                var hits = _hash
                    .PasteWhereIf(start < stop, x => start <= x.Value && x.Value < stop)
                    .PasteWhereIf(start > stop, x => stop <= x.Value && x.Value < start)
                    .ToList();
                foreach (var item in hits)
                {
                    list.Add(new RedisValue(item.Key));
                }
                return Task.FromResult(list.ToArray());
            }
            return Task.FromResult(new RedisValue[] { });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Task<long> SortedSetIncrementAsync(string key, string field, long num = 1)
        {
            var dict = key_hash_num.GetOrAdd(key, _ => new ConcurrentDictionary<string, long>());
            var result = dict.AddOrUpdate(field, num, (k, old) => old + num);
            return Task.FromResult(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Task<long> SortedSetDecrementAsync(string key, string field, long num)
        {
            var dict = key_hash_num.GetOrAdd(key, _ => new ConcurrentDictionary<string, long>());
            var result = dict.AddOrUpdate(field, -num, (k, old) => old - num);
            return Task.FromResult(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public Task<bool> SortedSetRemoveAsync(string key, string field)
        {
            if (key_hash_num.TryGetValue(key, out var _hash))
            {
                return Task.FromResult(_hash.TryRemove(field, out _));
            }
            return Task.FromResult(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Task<bool> SortedSetAddAsync(string key, string field, long num)
        {
            var dict = key_hash_num.GetOrAdd(key, _ => new ConcurrentDictionary<string, long>());
            dict[field] = num;
            return Task.FromResult(true);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> ExistsAsync(string key)
        {
            return Task.FromResult(Exists(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<List<HashKeyValue>> HashGetAllAsync(string key)
        {
            if (key_hash_set.TryGetValue(key, out var _hash))
            {
                var finds = new List<HashKeyValue>();
                foreach (var item in _hash)
                {
                    finds.Add(new HashKeyValue { Key = item.Key, Value = item.Value });
                }
                return Task.FromResult(finds);
            }
            if (key_hash_num.TryGetValue(key, out var _hash_num))
            {
                var finds = new List<HashKeyValue>();
                foreach (var item in _hash_num)
                {
                    finds.Add(new HashKeyValue { Key = item.Key, Value = item.Value.ToString() });
                }
                return Task.FromResult(finds);
            }
            return Task.FromResult(new List<HashKeyValue> { });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public Task<long> HashDecrementAsync(string key, string field)
        {
            var dict = key_hash_num.GetOrAdd(key, _ => new ConcurrentDictionary<string, long>());
            var result = dict.AddOrUpdate(field, -1, (k, old) => old - 1);
            return Task.FromResult(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        public Task<bool> HashExistsAsync(string key, string filed)
        {
            var _result = false;
            if (key_hash_num.TryGetValue(key, out var _hash) && _hash.ContainsKey(filed))
            {
                _result = true;
            }
            if (!_result && key_hash_set.TryGetValue(key, out var hash_key) && hash_key.ContainsKey(filed))
            {
                _result = true;
            }
            return Task.FromResult(_result);
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
            await Task.Run(() =>
            {
                if (isnum)
                {
                    var dict = key_hash_num.GetOrAdd(key, _ => new ConcurrentDictionary<string, long>());
                    if (long.TryParse(value.ToString(), out var numVal))
                    {
                        dict[field] = numVal;
                    }
                }
                else
                {
                    var dict = key_hash_set.GetOrAdd(key, _ => new ConcurrentDictionary<string, string>());
                    dict[field] = value.ToString();
                }
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public Task<T> HashGetObjectAsync<T>(string key, string field)
        {
            if (key_hash_set.TryGetValue(key, out var hash_key) && hash_key.TryGetValue(field, out var _value))
            {
                if (!String.IsNullOrEmpty(_value))
                {
                    var t = JsonSerializer.Deserialize<T>(_value, JsonConfig.Options);
                    return Task.FromResult(t);
                }
            }
            return Task.FromResult(default(T));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public Task<bool> HashSetObjectAsync<T>(string key, string field, T entity)
        {
            var value = JsonSerializer.Serialize<T>(entity, JsonConfig.Options);
            var dict = key_hash_set.GetOrAdd(key, _ => new ConcurrentDictionary<string, string>());
            dict[field] = value;
            return Task.FromResult(true);
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
        public Task<List<HashEntry>> HashScanAsync(string key, string pattern, int page, int size, bool isincom = false)
        {
            //Func<KeyValuePair<string, long>, bool> numFilter = null;
            //Func<KeyValuePair<string, string>, bool> setFilter = null;

            var position = 3; //1left 2right 3like
            if (pattern.StartsWith("*")) position = 1;
            if (pattern.EndsWith("*"))
            {
                if (position == 1) position = 3;
                else position = 2;
            }
            var realPattern = pattern.Replace("*", "");

            if (isincom)
            {
                if (key_hash_num.TryGetValue(key, out var hash_key))
                {
                    var vals = hash_key
                        .PasteWhereIf(position == 1, x => x.Key.EndsWith(realPattern))
                        .PasteWhereIf(position == 2, x => x.Key.StartsWith(realPattern))
                        .PasteWhereIf(position == 3, x => x.Key.Contains(realPattern))
                        .OrderBy(x => x.Key)
                        .ToList();
                    var start = (page - 1) * size;
                    var end = page * size;
                    var list = new List<HashEntry>();
                    for (var k = 0; k < vals.Count; k++)
                    {
                        if (k >= start && k < end)
                        {
                            var item = vals[k];
                            list.Add(new HashEntry(item.Key, item.Value));
                        }
                        if (k >= end) break;
                    }
                    return Task.FromResult(list);
                }
            }
            else
            {
                if (key_hash_set.TryGetValue(key, out var hash_key))
                {
                    var vals = hash_key
                        .PasteWhereIf(position == 1, x => x.Key.StartsWith(realPattern))
                        .PasteWhereIf(position == 2, x => x.Key.EndsWith(realPattern))
                        .PasteWhereIf(position == 3, x => x.Key.Contains(realPattern))
                        .OrderBy(x => x.Key)
                        .ToList();
                    var start = (page - 1) * size;
                    var end = page * size;
                    var list = new List<HashEntry>();
                    for (var k = 0; k < vals.Count; k++)
                    {
                        if (k >= start && k < end)
                        {
                            var item = vals[k];
                            list.Add(new HashEntry(item.Key, item.Value));
                        }
                        if (k >= end) break;
                    }
                    return Task.FromResult(list);
                }
            }
            return Task.FromResult(new List<HashEntry> { });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <param name="isnum"></param>
        /// <returns></returns>
        public Task<RedisValue[]> HashGetAsync(string key, RedisValue[] fields, bool isnum = false)
        {
            var fis = fields.Select(x => x.ToString()).ToArray();
            if (!isnum)
            {
                if (key_hash_set.TryGetValue(key, out var hash_key))
                {
                    var haves = hash_key.Where(x => fis.Contains(x.Key)).ToList();
                    var reds = new List<RedisValue>();
                    foreach (var kv in haves) reds.Add(kv.Value);
                    return Task.FromResult(reds.ToArray());
                }
            }
            else
            {
                if (key_hash_num.TryGetValue(key, out var hash_key))
                {
                    var haves = hash_key.Where(x => fis.Contains(x.Key)).ToList();
                    var reds = new List<RedisValue>();
                    foreach (var kv in haves) reds.Add(kv.Value);
                    return Task.FromResult(reds.ToArray());
                }
            }
            return Task.FromResult(new RedisValue[] { });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public Task<bool> HashDeleteAsync(string key, string field)
        {
            var _result = false;
            if (key_hash_set.TryGetValue(key, out var hash_key) && hash_key.TryRemove(field, out _))
            {
                _result = true;
            }
            if (key_hash_num.TryGetValue(key, out var _hash) && _hash.TryRemove(field, out _))
            {
                _result = true;
            }
            return Task.FromResult(_result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="fields"></param>
        /// <returns></returns>
        public Task<bool> HashDeleteAsync(string key, RedisValue[] fields)
        {
            var fis = fields.Select(x => x.ToString()).ToArray();
            var _result = false;
            if (key_hash_set.TryGetValue(key, out var hash_key))
            {
                foreach (var kv in fis)
                {
                    if (hash_key.TryRemove(kv, out _)) _result = true;
                }
            }
            if (key_hash_num.TryGetValue(key, out var num_key))
            {
                foreach (var kv in fis)
                {
                    if (num_key.TryRemove(kv, out _)) _result = true;
                }
            }
            return Task.FromResult(_result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<long> HashLengthAsync(string key)
        {
            long count = 0;
            if (key_hash_set.TryGetValue(key, out var hash_key)) count += hash_key.Count;
            if (key_hash_num.TryGetValue(key, out var num_key)) count += num_key.Count;
            return Task.FromResult(count);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public string Print()
        {
            var sb = new StringBuilder();
            if (key_hash_set != null && key_hash_set.Any())
            {
                sb.AppendLine(JsonSerializer.Serialize(key_hash_set, JsonConfig.Options));
            }
            if (key_hash_num != null && key_hash_num.Any())
            {
                sb.AppendLine(JsonSerializer.Serialize(key_hash_num, JsonConfig.Options));
            }
            if (key_set != null && key_set.Any())
            {
                sb.AppendLine(JsonSerializer.Serialize(key_set, JsonConfig.Options));
            }
            if (key_list != null && key_list.Any())
            {
                sb.AppendLine(JsonSerializer.Serialize(key_list, JsonConfig.Options));
            }
            return sb.ToString();
        }

        //------------------------------------------------

        private static readonly Random _random = new();
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public object TryGetValue(string key)
        {
            return _memoryCache.TryGetValue(key, out var val) ? val : null;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory, TimeSpan? expiration = null)
        {
            return _memoryCache.GetOrCreate(key, entry =>
            {
                SetCacheEntryOptions(entry, expiration);
                return factory(entry);
            });
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expiry"></param>
        /// <returns></returns>
        public bool CreateEntry(string key, object value, TimeSpan? expiry = null)
        {
            using var entry = _memoryCache.CreateEntry(key);
            entry.Value = value;
            SetCacheEntryOptions(entry, expiry);
            return true;
        }

        #region 异步方法
        /// <summary>
        /// 
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        public Task BatchRemoveAsync(string[] keys)
        {
            if (keys != null)
            {
                foreach (var key in keys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        Delete(key);
                    }
                }
            }
            return Task.CompletedTask;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="factory"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public Task<T> GetOrCreateAsync<T>(string key, Func<ICacheEntry, Task<T>> factory, TimeSpan? expiration = null)
        {
            return _memoryCache.GetOrCreateAsync(key, async entry =>
            {
                SetCacheEntryOptions(entry, expiration);
                return await factory(entry);
            });
        }

        #endregion

        #region Hash 操作
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="subKey"></param>
        /// <returns></returns>
        public Task<string> HashGetAsync(string key, string subKey)
        {
            return Task.FromResult(HashGet(key, subKey));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public Task HashSetAsync(string key, HashEntry[] vals)
        {
            HashSet(key, vals);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<HashEntry> HashRandomFieldsWithValuesAsync(string key)
        {
            if (key_hash_set.TryGetValue(key, out var dict) && dict.Count > 0)
            {
                var keys = dict.Keys.ToArray();
                var randomKey = keys[_random.Next(keys.Length)];
                return Task.FromResult(new HashEntry(randomKey, dict[randomKey]));
            }
            return Task.FromResult(new HashEntry(string.Empty, string.Empty));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Task<long> HashIncrementAsync(string key, RedisValue field, long num = 1)
        {
            var dict = key_hash_num.GetOrAdd(key, _ => new ConcurrentDictionary<string, long>());
            var result = dict.AddOrUpdate(field.ToString(), num, (k, old) => old + num);
            return Task.FromResult(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="field"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Task<long> HashDecrementAsync(string key, RedisValue field, long num = 1)
        {
            return HashIncrementAsync(key, field, -num);
        }

        #endregion

        #region 未实现的方法（全部补齐）
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="vals"></param>
        /// <returns></returns>
        public Task HashSetAsync(string key, List<HashKeyValue> vals)
        {
            HashSet(key, vals);
            return Task.CompletedTask;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public List<HashKeyValue> HashGetAll(string key)
        {
            var result = new List<HashKeyValue>();
            if (key_hash_set.TryGetValue(key, out var _hash))
            {
                foreach (var item in _hash) result.Add(new HashKeyValue { Key = item.Key, Value = item.Value });
                return result;
            }
            if (key_hash_num.TryGetValue(key, out var _hash_num))
            {
                foreach (var item in _hash_num) result.Add(new HashKeyValue { Key = item.Key, Value = item.Value.ToString() });
                return result;
            }
            return result;
        }
        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="key"></param>
        ///// <returns></returns>
        //Task<List<HashKeyValue>> IAppCache.HashGetAllAsync(string key)
        //{
        //    return Task.FromResult(((IAppCache)this).HashGetAll(key));
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pattern"></param>
        /// <param name="size"></param>
        /// <param name="cursor"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public Task<HashEntry[]> HashScanAsync(string key, string pattern, int size = 1, int cursor = 0, int offset = 0)
        {
            // 简化实现：按 cursor 当 page, size 当每页大小
            var page = cursor <= 0 ? 1 : cursor;
            var task = HashScanAsync(key, pattern, page, size, false);
            return Task.FromResult(task.Result.ToArray());
        }

        #endregion

        #region 私有辅助方法

        private static void SetCacheEntryOptions(ICacheEntry entry, TimeSpan? expiration)
        {
            if (expiration.HasValue)
            {
                entry.AbsoluteExpirationRelativeToNow = expiration;
            }
            else
            {
                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
            }
        }

        #endregion

        #region HashIncrement(int) / ReadString / ReadObject / Set / List / SortedSet 补齐
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="felid"></param>
        /// <param name="num"></param>
        /// <returns></returns>
        public Task HashIncrementAsync(string key, string felid, int num = 1)
        {
            var dict = key_hash_num.GetOrAdd(key, _ => new ConcurrentDictionary<string, long>());
            var result = dict.AddOrUpdate(felid, num, (k, old) => old + num);
            return Task.FromResult(result);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<string> ReadStringAsync(string key)
        {
            return Task.FromResult(Get(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<T> ReadObjectAsync<T>(string key)
        {
            return Task.FromResult(GetObject<T>(key));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public Task<bool> SetAddAsync(string skey, int val)
        {
            var set = key_set.GetOrAdd(skey, _ => new ConcurrentDictionary<int, byte>());
            return Task.FromResult(set.TryAdd(val, 0));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        public Task<bool> SetContainsAsync(string skey, int filed)
        {
            if (key_set.TryGetValue(skey, out var set))
            {
                return Task.FromResult(set.ContainsKey(filed));
            }
            return Task.FromResult(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="filed"></param>
        /// <returns></returns>
        public Task<bool> SetRemoveAsync(string skey, int filed)
        {
            if (key_set.TryGetValue(skey, out var set))
            {
                return Task.FromResult(set.TryRemove(filed, out _));
            }
            return Task.FromResult(false);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public Task<long> SetRandomMemberAsync(string skey)
        {
            if (key_set.TryGetValue(skey, out var set) && set.Count > 0)
            {
                var keys = set.Keys.ToArray();
                return Task.FromResult((long)keys[_random.Next(keys.Length)]);
            }
            return Task.FromResult(0L);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public Task<long> SetLengthAsync(string skey)
        {
            if (key_set.TryGetValue(skey, out var set))
            {
                return Task.FromResult((long)set.Count);
            }
            return Task.FromResult(0L);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="sval"></param>
        /// <returns></returns>
        public Task<long> ListRightPushAsync(string skey, string sval)
        {
            lock (_listLock)
            {
                var list = key_list.GetOrAdd(skey, _ => new List<string>());
                list.Add(sval);
                return Task.FromResult((long)list.Count);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="score"></param>
        /// <returns></returns>
        public Task<string> SortedSetRangeByScoreAsync(string skey, long score)
        {
            if (key_hash_num.TryGetValue(skey, out var _hash))
            {
                var item = _hash.FirstOrDefault(x => x.Value == score);
                if (item.Key != null)
                {
                    return Task.FromResult(item.Key);
                }
            }
            return Task.FromResult(string.Empty);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public Task<long> SortedSetRangeByRankWithScoresAsync(string skey)
        {
            if (key_hash_num.TryGetValue(skey, out var _hash))
            {
                return Task.FromResult((long)_hash.Count);
            }
            return Task.FromResult(0L);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public Task<long> ListLengthAsync(string skey)
        {
            lock (_listLock)
            {
                if (key_list.TryGetValue(skey, out var list))
                {
                    return Task.FromResult((long)list.Count);
                }
                return Task.FromResult(0L);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <returns></returns>
        public Task<string> ListLeftPopAsync(string skey)
        {
            lock (_listLock)
            {
                if (key_list.TryGetValue(skey, out var list) && list.Count > 0)
                {
                    var val = list[0];
                    list.RemoveAt(0);
                    return Task.FromResult(val);
                }
                return Task.FromResult(string.Empty);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="skey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public Task<List<string>> ListRangeAsync(string skey, int start = 0, int stop = 30)
        {
            lock (_listLock)
            {
                if (key_list.TryGetValue(skey, out var list))
                {
                    if (start < 0) start = 0;
                    if (stop >= list.Count) stop = list.Count - 1;
                    if (start > stop) return Task.FromResult(new List<string>());
                    return Task.FromResult(list.GetRange(start, stop - start + 1));
                }
                return Task.FromResult(new List<string>());
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="skey"></param>
        /// <param name="start"></param>
        /// <param name="stop"></param>
        /// <returns></returns>
        public Task<List<T>> ListRangeAsync<T>(string skey, int start = 0, int stop = 30)
        {
            var strList = ListRangeAsync(skey, start, stop).Result;
            var result = new List<T>();
            foreach (var s in strList)
            {
                try
                {
                    result.Add(JsonSerializer.Deserialize<T>(s, JsonConfig.Options));
                }
                catch
                {
                    // 忽略反序列化失败项
                }
            }
            return Task.FromResult(result);
        }

        #endregion
    }
}