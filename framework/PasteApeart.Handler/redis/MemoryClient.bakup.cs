//using Microsoft.Extensions.Caching.Memory;
//using StackExchange.Redis;
//using System.Collections.Concurrent;
//using System.Text;
//using System.Text.Json;

//namespace PasteApeart.Handler.redis
//{
//    /// <summary>
//    /// 本地缓存
//    /// </summary>
//    public class MemoryClient : IAppCache
//    {
//        private readonly IMemoryCache _memoryCache;

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="memoryCache"></param>
//        public MemoryClient(IMemoryCache memoryCache)
//        {
//            _memoryCache = memoryCache;
//            key_hash_set = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
//            key_hash_num = new ConcurrentDictionary<string, ConcurrentDictionary<string, long>>();
//        }

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //private ConcurrentDictionary<string, string> key_val;

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        //private ConcurrentDictionary<string, List<SortScore>> key_sort_set;

//        /// <summary>
//        /// 
//        /// </summary>
//        private ConcurrentDictionary<string, ConcurrentDictionary<string, string>> key_hash_set;

//        /// <summary>
//        /// 
//        /// </summary>
//        private ConcurrentDictionary<string, ConcurrentDictionary<string, long>> key_hash_num;

//        public bool IsRedis { get => false; }

//        public bool Delete(string key)
//        {
//            _memoryCache.Remove(key);
//            return true;
//        }

//        /// <summary>
//        /// error
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public Task<bool> DeleteAsync(string key)
//        {
//            return Task.Run<bool>(() =>
//            {
//                return Delete(key);
//            });
//        }

//        public bool Exists(string key)
//        {
//            return _memoryCache.TryGetValue(key, out var val);
//            //throw new NotImplementedException();
//        }

//        public T GetObject<T>(string key)
//        {
//            if (_memoryCache.TryGetValue(key, out string val))
//            {
//                return JsonSerializer.Deserialize<T>(val, JsonConfig.Options);// JsonConvert.DeserializeObject<T>(val);
//            }
//            return default(T);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public string Get(string key)
//        {
//            if (_memoryCache.TryGetValue(key, out string val))
//            {
//                return val;
//            }
//            return "";
//            // throw new NotImplementedException();
//        }

//        public Task<T> GetObjectAsync<T>(string key)
//        {
//            return Task.Run<T>(() =>
//            {
//                return GetObject<T>(key);
//            });
//        }

//        public Task<string> GetAsync(string key)
//        {
//            return Task.Run<string>(() =>
//            {
//                return Get(key);
//            });
//        }

//        public string GetStringValue(string key)
//        {
//            return Get(key);
//        }

//        public string HashGet(string key, string sunkey)
//        {
//            throw new NotImplementedException();
//        }

//        public HashEntry[] HashGetAll(string key)
//        {
//            throw new NotImplementedException();
//        }

//        //public Task<HashEntry[]> HashGetAllAsync(string key)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="sunkey"></param>
//        /// <returns></returns>
//        public Task<string> HashGetAsync(string key, string sunkey, bool isnum = false)
//        {
//            if (key_hash_set.TryGetValue(key, out var val))
//            {
//                if (val != null && val != default)
//                {
//                    if (val.TryGetValue(sunkey, out var _val))
//                    {
//                        return Task.FromResult(_val);
//                    }
//                }
//            }
//            return Task.FromResult(String.Empty);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public async Task<int> HashGetIntFieldAsync(string key, string field, bool isNum = false)
//        {
//            var read = await this.HashGetAsync(key, field, isNum);
//            if (!string.IsNullOrEmpty(read))
//            {
//                int.TryParse(read, out var ival);
//                return ival;
//            }
//            return default(int);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public async Task<long> HashGetLongFieldAsync(string key, string field, bool isNum = false)
//        {
//            var read = await this.HashGetAsync(key, field, isNum);
//            if (!string.IsNullOrEmpty(read))
//            {
//                long.TryParse(read, out var ival);
//                return ival;
//            }
//            return default(long);
//        }

//        public void HashSet(string key, HashEntry[] vals)
//        {
//            throw new NotImplementedException();
//        }

//        //public void HashSetAsync(string key, HashEntry[] vals)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        public bool Lock(string key, int seconds)
//        {
//            return false;
//        }

//        public Task<bool> LockAsync(string key, int seconds)
//        {
//            //return Task.Run<bool>(() =>
//            //{
//            //    return Lock(key, seconds);
//            //});
//            return Task.FromResult(true);
//        }

//        public bool Set(string key, string value, TimeSpan? expiry = null)
//        {
//            using (var wcache = _memoryCache.CreateEntry(key))
//            {
//                wcache.Value = value;
//                if (expiry == null)
//                {
//                    wcache.AbsoluteExpiration = System.DateTimeOffset.Now.AddSeconds(30);
//                }
//                else
//                {
//                    wcache.AbsoluteExpiration = DateTimeOffset.Now.Add((TimeSpan)expiry);
//                }
//            }

//            return true;
//        }

//        public bool Set(string key, string value, int seconds)
//        {
//            using (var wcache = _memoryCache.CreateEntry(key))
//            {
//                wcache.Value = value;

//                wcache.AbsoluteExpiration = System.DateTimeOffset.Now.AddSeconds(seconds);

//            }

//            return true;
//        }

//        public bool Set(string key, string value)
//        {
//            using (var wcache = _memoryCache.CreateEntry(key))
//            {
//                wcache.Value = value;
//            }
//            return true;
//        }

//        public bool SetObject<T>(string key, T value, TimeSpan? expiry = null)
//        {

//            return Set(key, JsonSerializer.Serialize<T>(value, JsonConfig.Options), expiry);
//        }

//        public bool SetObject<T>(string key, T value, int seconds)
//        {
//            return Set(key, JsonSerializer.Serialize<T>(value, JsonConfig.Options), seconds);
//        }

//        public Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
//        {
//            return Task.Run<bool>(() =>
//            {
//                return Set(key, value, expiry);
//            });
//        }

//        public Task<bool> SetAsync(string key, string value, int seconds)
//        {
//            return Task.Run<bool>(() =>
//            {
//                return Set(key, value, seconds);
//            });
//        }

//        public Task<bool> SetObjectAsync<T>(string key, T value, int second)
//        {
//            return Task.Run<bool>(() =>
//            {
//                return SetObject(key, value, second);
//            });
//        }

//        public bool SetExpire(string key, TimeSpan? expiry)
//        {
//            if (_memoryCache.TryGetValue(key, out string sval))
//            {

//                return Set(key, sval, expiry);
//            }
//            return false;
//        }

//        public bool SetExpire(string key, int seconds)
//        {
//            if (_memoryCache.TryGetValue(key, out string sval))
//            {
//                return Set(key, sval, seconds);
//            }
//            return false;
//        }

//        public Task<bool> SetExpireAsync(string key, int seconds)
//        {
//            return Task.Run<bool>(() =>
//            {
//                return SetExpire(key, seconds);
//            });
//        }

//        public Task<bool> SetExpireAsync(string key, TimeSpan? expiry)
//        {
//            return Task.Run<bool>(() =>
//            {
//                return SetExpire(key, expiry);
//            });
//        }

//        public bool SetValue(string key, string val, int second)
//        {
//            return Set(key, val, second);
//        }

//        public bool SetValue(string key, string val)
//        {
//            return Set(key, val);
//        }

//        public bool UnLock(string key)
//        {
//            return false;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        public Task<bool> UnLockAsync(string key)
//        {
//            //return Task.Run<bool>(() =>
//            //{
//            //    return UnLock(key);
//            //});
//            return Task.FromResult(true);
//        }

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        ///// <param name="key"></param>
//        ///// <param name="vals"></param>
//        ///// <returns></returns>
//        //public async Task HashSetAsync(string key, HashEntry[] vals, bool isnum = false)
//        //{
//        //    await Task.Run(() =>
//        //    {
//        //        if (isnum)
//        //        {
//        //            if(key_hash_num.TryGetValue(key, out var _hash))
//        //            {
//        //                foreach (var entry in vals)
//        //                {
//        //                    _hash.AddOrUpdate(entry.Name.ToString(), long.Parse(entry.Value.ToString()), (k, oldValue) => long.Parse(entry.Value.ToString()));
//        //                }
//        //            }
//        //            else
//        //            {
//        //                _hash = new ConcurrentDictionary<string, long>();

//        //                foreach (var entry in vals)
//        //                {
//        //                    _hash.AddOrUpdate(entry.Name.ToString(), long.Parse(entry.Value.ToString()), (k, oldValue) => long.Parse(entry.Value.ToString()));
//        //                }
//        //                key_hash_num.TryAdd(key, _hash);
//        //            }
//        //        }
//        //        else
//        //        {
//        //            key_hash_set.TryGetValue(key, out var _hash);
//        //            if (_hash == null || _hash == default)
//        //            {
//        //                _hash = new ConcurrentDictionary<string, string>();
//        //            }
//        //            foreach (var entry in vals)
//        //            {
//        //                _hash.AddOrUpdate(entry.Name.ToString(), entry.Value.ToString(), (k, oldValue) => entry.Value.ToString());
//        //            }
//        //            key_hash_set.AddOrUpdate(key, _hash, (k, oldvalue) => _hash);
//        //        }


//        //    });
//        //}

//        //public List<HashKeyValue> HashGetAll(string key)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        //public Task<List<HashKeyValue>> HashGetAllAsync(string key)
//        //{
//        //    throw new NotImplementedException();
//        //}

//        public void HashSet(string key, List<HashKeyValue> vals)
//        {
//            throw new NotImplementedException();
//        }

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        ///// <param name="key"></param>
//        ///// <param name="vals"></param>
//        ///// <returns></returns>
//        //public async Task HashSetAsync(string key, List<HashKeyValue> vals, bool isnum = false)
//        //{
//        //    await Task.Run(() =>
//        //    {
//        //        if (isnum)
//        //        {
//        //            if(key_hash_num.TryGetValue(key, out var _hash))
//        //            {
//        //                foreach (var entry in vals)
//        //                {
//        //                    _hash.AddOrUpdate(entry.Key, long.Parse(entry.Value), (k, oldValue) => long.Parse(entry.Value));
//        //                }
//        //            }
//        //            else
//        //            {
//        //                _hash = new ConcurrentDictionary<string, long>();
//        //                foreach (var entry in vals)
//        //                {
//        //                    _hash.AddOrUpdate(entry.Key, long.Parse(entry.Value), (k, oldValue) => long.Parse(entry.Value));
//        //                }
//        //                key_hash_num.TryAdd(key, _hash);
//        //            }
//        //        }
//        //        else
//        //        {
//        //            key_hash_set.TryGetValue(key, out var _hash);
//        //            if (_hash == null || _hash == default)
//        //            {
//        //                _hash = new ConcurrentDictionary<string, string>();
//        //            }
//        //            foreach (var entry in vals)
//        //            {
//        //                _hash.AddOrUpdate(entry.Key, entry.Value, (k, oldValue) => entry.Value);
//        //            }
//        //            key_hash_set.AddOrUpdate(key, _hash, (k, oldvalue) => _hash);
//        //        }
//        //    });
//        //}

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <param name="num"></param>
//        /// <returns></returns>
//        public Task<long> HashIncrementAsync(string key, string field, long num = 1)
//        {
//            var _result = num;
//            key_hash_num.TryGetValue(key, out var _hash);
//            if (_hash == null || _hash == default)
//            {
//                _hash = new ConcurrentDictionary<string, long>() { };
//                _hash.TryAdd(field, num);
//                key_hash_num.TryAdd(key, _hash);
//            }
//            else
//            {
//                if (_hash.ContainsKey(field))
//                {
//                    _hash[field] = _hash[field] + num;
//                    _result = _hash[field];
//                }
//                else
//                {
//                    _hash.TryAdd(field, num);
//                }
//            }
//            return Task.FromResult(_result);
//        }

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        ///// <param name="key"></param>
//        ///// <param name="start"></param>
//        ///// <param name="stop"></param>
//        ///// <returns></returns>
//        //public Task<SortedSetEntry[]> SortedSetRangeByRankWithScoresAsync(string key, int start = 0, int stop = 1)
//        //{
//        //    if (key_hash_num.TryGetValue(key, out var _hash))
//        //    {
//        //        var list = new List<SortedSetEntry>();
//        //        var hits = _hash.Where(x => x.Value >= start && x.Value < stop).ToList();
//        //        foreach (var item in hits)
//        //        {
//        //            list.Add(new SortedSetEntry(item.Key, item.Value));
//        //        }
//        //        return Task.FromResult(list.ToArray());
//        //    }
//        //    return Task.FromResult(new SortedSetEntry[] { });
//        //}

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="start"></param>
//        /// <param name="stop"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<RedisValue[]> SortedSetRangeByScoreAsync(string key, long start = 0, long stop = 1)
//        {
//            if (key_hash_num.TryGetValue(key, out var _hash))
//            {
//                var list = new List<RedisValue>();
//                var hits = _hash
//                    .PasteWhereIf(start < stop, x => start <= x.Value && x.Value < stop)
//                    .PasteWhereIf(start > stop, x => stop <= x.Value && x.Value < start)
//                    .ToList();
//                foreach (var item in hits)
//                {
//                    list.Add(new RedisValue(item.Key));
//                }
//                return Task.FromResult(list.ToArray());
//            }
//            return Task.FromResult(new RedisValue[] { });
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <param name="num"></param>
//        /// <returns></returns>
//        public Task<long> SortedSetIncrementAsync(string key, string field, long num = 1)
//        {
//            var _result = num;
//            key_hash_num.TryGetValue(key, out var _hash);
//            if (_hash == null || _hash == default)
//            {
//                _hash = new ConcurrentDictionary<string, long>() { };
//                _hash.TryAdd(field, num);
//                key_hash_num.TryAdd(key, _hash);
//            }
//            else
//            {
//                if (_hash.ContainsKey(field))
//                {
//                    _hash[field] = _hash[field] + num;
//                    _result = _hash[field];
//                }
//                else
//                {
//                    _hash.TryAdd(field, num);
//                }
//            }
//            return Task.FromResult(_result);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <param name="num"></param>
//        /// <returns></returns>
//        public Task<long> SortedSetDecrementAsync(string key, string field, long num)
//        {
//            var _result = num;
//            key_hash_num.TryGetValue(key, out var _hash);
//            if (_hash == null || _hash == default)
//            {
//                _hash = new ConcurrentDictionary<string, long>() { };
//                _hash.TryAdd(field, num);
//                key_hash_num.TryAdd(key, _hash);
//            }
//            else
//            {
//                if (_hash.ContainsKey(field))
//                {
//                    _hash[field] = _hash[field] - num;
//                    _result = _hash[field];
//                }
//                else
//                {
//                    _hash.TryAdd(field, num);
//                }
//            }
//            return Task.FromResult(_result);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public Task<bool> SortedSetRemoveAsync(string key, string field)
//        {
//            var _result = false;
//            if (key_hash_num.TryGetValue(key, out var _hash))
//            {
//                if (_hash.ContainsKey(field))
//                {
//                    _hash.Remove(field, out _);
//                    _result = true;
//                }
//            }
//            return Task.FromResult(_result);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <param name="num"></param>
//        /// <returns></returns>
//        public Task<bool> SortedSetAddAsync(string key, string field, long num)
//        {
//            if (key_hash_num.TryGetValue(key, out var _hash))
//            {
//                if (_hash.ContainsKey(field))
//                {
//                    _hash[field] = num;
//                }
//                else
//                {
//                    _hash.TryAdd(field, num);
//                }
//            }
//            else
//            {
//                _hash = new ConcurrentDictionary<string, long>() { };
//                _hash.TryAdd(field, num);
//                key_hash_num.TryAdd(key, _hash);
//            }
//            return Task.FromResult(true);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<bool> ExistsAsync(string key)
//        {
//            var _result = _memoryCache.TryGetValue(key, out var val);
//            return Task.FromResult(_result);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<List<HashKeyValue>> HashGetAllAsync(string key)
//        {
//            if (key_hash_set.TryGetValue(key, out var _hash))
//            {
//                var finds = new List<HashKeyValue>();
//                foreach (var item in _hash)
//                {
//                    finds.Add(new HashKeyValue { Key = item.Key, Value = item.Value });
//                }
//                return Task.FromResult(finds);
//            }
//            if (key_hash_num.TryGetValue(key, out var _hash_num))
//            {
//                var finds = new List<HashKeyValue>();
//                foreach (var item in _hash_num)
//                {
//                    finds.Add(new HashKeyValue { Key = item.Key, Value = item.ToString() });
//                }
//                return Task.FromResult(finds);
//            }
//            return Task.FromResult(new List<HashKeyValue> { });
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public Task<long> HashDecrementAsync(string key, string field)
//        {
//            var num = 1;
//            long _result = num;
//            key_hash_num.TryGetValue(key, out var _hash);
//            if (_hash == null || _hash == default)
//            {
//                _hash = new ConcurrentDictionary<string, long>() { };
//                _hash.TryAdd(field, 0 - num);
//                key_hash_num.TryAdd(key, _hash);
//            }
//            else
//            {
//                if (_hash.ContainsKey(field))
//                {
//                    _hash[field] = _hash[field] - num;
//                    _result = _hash[field];
//                }
//                else
//                {
//                    _hash.TryAdd(field, num);
//                }
//            }
//            return Task.FromResult(_result);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="filed"></param>
//        /// <returns></returns>
//        public Task<bool> HashExistsAsync(string key, string filed)
//        {
//            var _result = false;

//            if (key_hash_num.TryGetValue(key, out var _hash))
//            {
//                if (_hash.ContainsKey(filed))
//                {
//                    _result = true;
//                }
//            }
//            if (!_result)
//            {
//                if (key_hash_set.TryGetValue(key, out var hash_key))
//                {
//                    if (hash_key.ContainsKey(filed))
//                    {
//                        _result = true;
//                    }
//                }
//            }
//            return Task.FromResult(true);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <param name="value"></param>
//        /// <returns></returns>
//        public async Task HashSetAsync(string key, string field, RedisValue value, bool isnum = false)
//        {
//            await Task.Run(() =>
//            {
//                if (isnum)
//                {
//                    if (key_hash_num.TryGetValue(key, out var hash_key))
//                    {
//                        hash_key.AddOrUpdate(field, long.Parse(value.ToString()), (n, o) => long.Parse(value.ToString()));
//                    }
//                    else
//                    {
//                        var find = new ConcurrentDictionary<string, long>();
//                        find.TryAdd(field, long.Parse(value.ToString()));
//                        key_hash_num.TryAdd(key, find);
//                    }
//                }
//                else
//                {
//                    if (key_hash_set.TryGetValue(key, out var hash_key))
//                    {
//                        hash_key.AddOrUpdate(field, value.ToString(), (n, o) => value.ToString());
//                    }
//                    else
//                    {
//                        var find = new ConcurrentDictionary<string, string>();
//                        find.TryAdd(field, value.ToString());
//                        key_hash_set.TryAdd(key, find);
//                    }
//                }
//            });
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public Task<T> HashGetObjectAsync<T>(string key, string field)
//        {
//            if (key_hash_set.TryGetValue(key, out var hash_key))
//            {
//                if (hash_key.TryGetValue(field, out var _value))
//                {
//                    if (!String.IsNullOrEmpty(_value))
//                    {
//                        var t = JsonSerializer.Deserialize<T>(_value, JsonConfig.Options);// Newtonsoft.Json.JsonConvert.DeserializeObject<T>(_value);
//                        return Task.FromResult(t);
//                    }
//                }
//            }
//            return Task.FromResult(default(T));
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <param name="entity"></param>
//        /// <returns></returns>
//        public Task<bool> HashSetObjectAsync<T>(string key, string field, T entity)
//        {
//            var value = JsonSerializer.Serialize<T>(entity, JsonConfig.Options);
//            //Newtonsoft.Json.JsonConvert.SerializeObject(entity);
//            if (key_hash_set.TryGetValue(key, out var hash_key))
//            {

//                hash_key.AddOrUpdate(field, value, (n, o) => value);

//            }
//            else
//            {
//                var find = new ConcurrentDictionary<string, string>();
//                find.TryAdd(field, value);
//                key_hash_set.TryAdd(key, find);
//            }
//            return Task.FromResult(true);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="pattern"></param>
//        /// <param name="page"></param>
//        /// <param name="size"></param>
//        /// <returns></returns>
//        public Task<List<HashEntry>> HashScanAsync(string key, string pattern, int page, int size, bool isincom = false)
//        {
//            if (isincom)
//            {
//                if (key_hash_num.TryGetValue(key, out var hash_key))
//                {
//                    var position = 3;//1left 2right 3like
//                    if (pattern.StartsWith("*"))
//                    {
//                        position = 1;
//                    }
//                    if (pattern.EndsWith("*"))
//                    {
//                        if (position == 1)
//                        {
//                            position = 3;
//                        }
//                        else
//                        {
//                            position = 2;
//                        }
//                    }
//                    pattern = pattern.Replace("*", "");
//                    var vals = hash_key
//                        .PasteWhereIf(position == 1, x => x.Key.EndsWith(pattern))
//                        .PasteWhereIf(position == 2, x => x.Key.StartsWith(pattern))
//                        .PasteWhereIf(position == 3, x => x.Key.Contains(pattern))
//                        .OrderBy(x => x.Key)
//                        .ToList();
//                    var start = (page - 1) * size;
//                    var end = page * size;
//                    var list = new List<HashEntry>();
//                    for (var k = 0; k < vals.Count; k++)
//                    {
//                        if (k >= start && k < end)
//                        {
//                            var item = vals[k];
//                            list.Add(new HashEntry(item.Key, item.Value));
//                        }
//                        if (k > end)
//                        {
//                            break;
//                        }
//                    }
//                    return Task.FromResult(list);
//                }
//            }
//            else
//            {
//                if (key_hash_set.TryGetValue(key, out var hash_key))
//                {
//                    var position = 3;//1left 2right 3like
//                    if (pattern.StartsWith("*"))
//                    {
//                        position = 1;
//                    }
//                    if (pattern.EndsWith("*"))
//                    {
//                        if (position == 1)
//                        {
//                            position = 3;
//                        }
//                        else
//                        {
//                            position = 2;
//                        }
//                    }
//                    pattern = pattern.Replace("*", "");
//                    var vals = hash_key
//                        .PasteWhereIf(position == 1, x => x.Key.StartsWith(pattern))
//                        .PasteWhereIf(position == 2, x => x.Key.EndsWith(pattern))
//                        .PasteWhereIf(position == 3, x => x.Key.Contains(pattern))
//                        .OrderBy(x => x.Key)
//                        .ToList();
//                    var start = (page - 1) * size;
//                    var end = page * size;
//                    var list = new List<HashEntry>();
//                    for (var k = 0; k < vals.Count; k++)
//                    {
//                        if (k >= start && k < end)
//                        {
//                            var item = vals[k];
//                            list.Add(new HashEntry(item.Key, item.Value));
//                        }
//                        if (k > end)
//                        {
//                            break;
//                        }
//                    }
//                    return Task.FromResult(list);
//                }
//            }
//            return Task.FromResult(new List<HashEntry> { });
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="fields"></param>
//        /// <returns></returns>
//        public Task<RedisValue[]> HashGetAsync(string key, RedisValue[] fields, bool isnum = false)
//        {
//            var fis = fields.Select(x => x.ToString()).ToArray();
//            if (!isnum)
//            {
//                if (key_hash_set.TryGetValue(key, out var hash_key))
//                {
//                    var haves = hash_key.Where(x => fis.Contains(x.Key)).ToList();
//                    var reds = new List<RedisValue>();
//                    foreach (var kv in haves)
//                    {
//                        reds.Add(kv.Value);
//                    }
//                    return Task.FromResult(reds.ToArray());
//                }
//            }
//            else
//            {
//                if (key_hash_num.TryGetValue(key, out var hash_key))
//                {
//                    var haves = hash_key.Where(x => fis.Contains(x.Key)).ToList();
//                    var reds = new List<RedisValue>();
//                    foreach (var kv in haves)
//                    {
//                        reds.Add(kv.Value);
//                    }
//                    return Task.FromResult(reds.ToArray());
//                }
//            }
//            return Task.FromResult(new RedisValue[] { });
//        }


//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="field"></param>
//        /// <returns></returns>
//        public async Task<bool> HashDeleteAsync(string key, string field)
//        {
//            await Task.Run(() =>
//            {
//                var _result = false;
//                if (key_hash_set.TryGetValue(key, out var hash_key))
//                {
//                    if (hash_key.ContainsKey(field))
//                    {
//                        _result = true;
//                    }
//                    hash_key.Remove(field, out var abc);
//                }
//                if (!_result)
//                {
//                    if (key_hash_num.TryGetValue(key, out var _hash))
//                    {
//                        if (_hash.ContainsKey(field))
//                        {
//                            _result = true;
//                        }
//                        _hash.Remove(field, out var abc);
//                    }
//                }
//                return _result;
//            });
//            return false;
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="fields"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<bool> HashDeleteAsync(string key, RedisValue[] fields)
//        {
//            var fis = fields.Select(x => x.ToString()).ToArray();
//            if (key_hash_set.TryGetValue(key, out var hash_key))
//            {
//                foreach (var kv in fis)
//                {
//                    hash_key.Remove(kv, out var va);
//                }
//                var haves = hash_key.Where(x => fis.Contains(x.Key)).ToList();
//                var reds = new List<RedisValue>();
//                foreach (var kv in haves)
//                {
//                    reds.Add(kv.Value);
//                }
//                return Task.FromResult(true);
//            }
//            return Task.FromResult(false);
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<long> HashLengthAsync(string key)
//        {
//            if (key_hash_set.TryGetValue(key, out var hash_key))
//            {
//                return Task.FromResult((long)hash_key.Keys.Count);
//            }
//            return Task.FromResult((long)0);
//        }

//        /// <summary>
//        /// 打印
//        /// </summary>
//        /// <returns></returns>
//        public string Print()
//        {
//            var sb = new StringBuilder();

//            if (key_hash_set != null && key_hash_set.Any())
//            {
//                //sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(key_hash_set));
//                sb.AppendLine(JsonSerializer.Serialize(key_hash_set, JsonConfig.Options));
//            }

//            if (key_hash_num != null && key_hash_num.Any())
//            {
//                //sb.AppendLine(Newtonsoft.Json.JsonConvert.SerializeObject(key_hash_num));
//                sb.AppendLine(JsonSerializer.Serialize(key_hash_set, JsonConfig.Options));
//            }

//            return sb.ToString();
//        }


//        //------------------------------------------------


//        //private readonly IMemoryCache _memoryCache;
//        private static readonly Random _random = new();

//        //public MemoryClient(IMemoryCache memoryCache)
//        //{
//        //    _memoryCache = memoryCache;
//        //}

//        //public bool IsRedis => false;

//        #region 同步方法

//        //public bool Delete(string key)
//        //{
//        //    _memoryCache.Remove(key);
//        //    return true;
//        //}

//        //public bool Exists(string key)
//        //{
//        //    return _memoryCache.TryGetValue(key, out _);
//        //}

//        //public string Get(string key)
//        //{
//        //    return _memoryCache.TryGetValue(key, out string? val) ? val ?? string.Empty : string.Empty;
//        //}

//        //public T GetObject<T>(string key)
//        //{
//        //    if (_memoryCache.TryGetValue(key, out string? val) && !string.IsNullOrEmpty(val))
//        //    {
//        //        return JsonConvert.DeserializeObject<T>(val);
//        //    }
//        //    return default;
//        //}

//        //public string GetStringValue(string key)
//        //{
//        //    return Get(key);
//        //}

//        public object TryGetValue(string key)
//        {
//            return _memoryCache.TryGetValue(key, out var val) ? val : null;
//        }

//        public T GetOrCreate<T>(string key, Func<ICacheEntry, T> factory, TimeSpan? expiration = null)
//        {
//            return _memoryCache.GetOrCreate(key, entry =>
//            {
//                SetCacheEntryOptions(entry, expiration);
//                return factory(entry);
//            });
//        }

//        //public bool Set(string key, string value, TimeSpan? expiry = null)
//        //{
//        //    using var entry = _memoryCache.CreateEntry(key);
//        //    entry.Value = value;
//        //    SetCacheEntryOptions(entry, expiry);
//        //    return true;
//        //}

//        //public bool Set(string key, string value, int seconds)
//        //{
//        //    return Set(key, value, TimeSpan.FromSeconds(seconds));
//        //}

//        //public bool Set(string key, string value)
//        //{
//        //    using var entry = _memoryCache.CreateEntry(key);
//        //    entry.Value = value;
//        //    return true;
//        //}

//        //public bool SetObject<T>(string key, T value, TimeSpan? expiry = null)
//        //{
//        //    return Set(key, JsonConvert.SerializeObject(value), expiry);
//        //}

//        //public bool SetObject<T>(string key, T value, int seconds)
//        //{
//        //    return Set(key, JsonConvert.SerializeObject(value), TimeSpan.FromSeconds(seconds));
//        //}

//        public bool CreateEntry(string key, object value, TimeSpan? expiry = null)
//        {
//            using var entry = _memoryCache.CreateEntry(key);
//            entry.Value = value;
//            SetCacheEntryOptions(entry, expiry);
//            return true;
//        }

//        //public bool SetValue(string key, string val, int second)
//        //{
//        //    return Set(key, val, second);
//        //}

//        //public bool SetValue(string key, string val)
//        //{
//        //    return Set(key, val);
//        //}

//        //public bool SetExpire(string key, TimeSpan? expiry)
//        //{
//        //    if (_memoryCache.TryGetValue(key, out string? val))
//        //    {
//        //        return Set(key, val ?? string.Empty, expiry);
//        //    }
//        //    return false;
//        //}

//        //public bool SetExpire(string key, int seconds)
//        //{
//        //    return SetExpire(key, TimeSpan.FromSeconds(seconds));
//        //}

//        //public bool Lock(string key, int seconds)
//        //{
//        //    // MemoryCache 不支持分布式锁，直接返回 true
//        //    return true;
//        //}

//        //public bool UnLock(string key)
//        //{
//        //    return true;
//        //}

//        #endregion

//        #region 异步方法（同步包装，返回 Task）

//        //public Task<bool> DeleteAsync(string key)
//        //{
//        //    Delete(key);
//        //    return Task.FromResult(true);
//        //}

//        public Task BatchRemoveAsync(string[] keys)
//        {
//            if (keys != null)
//            {
//                foreach (var key in keys)
//                {
//                    if (!string.IsNullOrEmpty(key))
//                    {
//                        _memoryCache.Remove(key);
//                    }
//                }
//            }
//            return Task.CompletedTask;
//        }

//        //public Task<bool> ExistsAsync(string key)
//        //{
//        //    return Task.FromResult(Exists(key));
//        //}

//        //public Task<string> GetAsync(string key)
//        //{
//        //    return Task.FromResult(Get(key));
//        //}

//        //public Task<T?> GetObjectAsync<T>(string key)
//        //{
//        //    return Task.FromResult(GetObject<T>(key));
//        //}

//        public Task<T> GetOrCreateAsync<T>(string key, Func<ICacheEntry, Task<T>> factory, TimeSpan? expiration = null)
//        {
//            return _memoryCache.GetOrCreateAsync(key, async entry =>
//            {
//                SetCacheEntryOptions(entry, expiration);
//                return await factory(entry);
//            });
//        }

//        //public Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
//        //{
//        //    return Task.FromResult(Set(key, value, expiry));
//        //}

//        //public Task<bool> SetAsync(string key, string value, int seconds)
//        //{
//        //    return Task.FromResult(Set(key, value, seconds));
//        //}

//        //public Task<bool> SetObjectAsync<T>(string key, T value, int second)
//        //{
//        //    return Task.FromResult(SetObject(key, value, second));
//        //}

//        //public Task<bool> SetExpireAsync(string key, int seconds)
//        //{
//        //    return Task.FromResult(SetExpire(key, seconds));
//        //}

//        //public Task<bool> SetExpireAsync(string key, TimeSpan? expiry)
//        //{
//        //    return Task.FromResult(SetExpire(key, expiry));
//        //}

//        //public Task<bool> LockAsync(string key, int seconds)
//        //{
//        //    return Task.FromResult(Lock(key, seconds));
//        //}

//        //public Task<bool> UnLockAsync(string key)
//        //{
//        //    return Task.FromResult(UnLock(key));
//        //}

//        #endregion

//        #region Hash 操作

//        //public string HashGet(string key, string subKey)
//        //{
//        //    if (_memoryCache.TryGetValue(key, out var obj) && obj is Dictionary<string, string> dict)
//        //    {
//        //        return dict.TryGetValue(subKey, out var val) ? val : string.Empty;
//        //    }
//        //    return string.Empty;
//        //}

//        //public HashEntry[] HashGetAll(string key)
//        //{
//        //    if (_memoryCache.TryGetValue(key, out var obj) && obj is Dictionary<string, string> dict)
//        //    {
//        //        return dict.Select(x => new HashEntry(x.Key, x.Value)).ToArray();
//        //    }
//        //    return Array.Empty<HashEntry>();
//        //}

//        //public void HashSet(string key, HashEntry[] vals)
//        //{
//        //    var dict = _memoryCache.TryGetValue(key, out var obj) && obj is Dictionary<string, string> existing
//        //        ? existing
//        //        : new Dictionary<string, string>();

//        //    foreach (var entry in vals)
//        //    {
//        //        dict[entry.Name.ToString()] = entry.Value.ToString();
//        //    }

//        //    _memoryCache.Set(key, dict);
//        //}

//        public Task<string> HashGetAsync(string key, string subKey)
//        {
//            return Task.FromResult(HashGet(key, subKey));
//        }

//        //public Task<HashEntry[]> HashGetAllAsync(string key)
//        //{
//        //    return Task.FromResult(HashGetAll(key));
//        //}

//        public Task HashSetAsync(string key, HashEntry[] vals)
//        {
//            HashSet(key, vals);
//            return Task.CompletedTask;
//        }

//        ///// <summary>
//        ///// 
//        ///// </summary>
//        ///// <param name="key"></param>
//        ///// <param name="field"></param>
//        ///// <returns></returns>
//        //public Task<bool> HashDeleteAsync(string key, string field)
//        //{
//        //    if (_memoryCache.TryGetValue(key, out var obj) && obj is Dictionary<string, string> dict)
//        //    {
//        //        if (dict.ContainsKey(field))
//        //        {
//        //            dict.Remove(field);
//        //        }
//        //    }
//        //    return Task.FromResult(true);
//        //}

//        public Task<HashEntry> HashRandomFieldsWithValuesAsync(string key)
//        {
//            if (_memoryCache.TryGetValue(key, out var obj) && obj is Dictionary<string, string> dict && dict.Count > 0)
//            {
//                var keys = dict.Keys.ToArray();
//                var randomKey = keys[_random.Next(keys.Length)];
//                return Task.FromResult(new HashEntry(randomKey, dict[randomKey]));
//            }
//            return Task.FromResult(new HashEntry(string.Empty, string.Empty));
//        }

//        public Task<long> HashIncrementAsync(string key, RedisValue field, long num = 1)
//        {
//            // MemoryCache 不支持原子增减，简单实现
//            var current = HashGet(key, field.ToString());
//            if (long.TryParse(current, out var value))
//            {
//                var newValue = value + num;
//                HashSet(key, new[] { new HashEntry(field, newValue.ToString()) });
//                return Task.FromResult(newValue);
//            }
//            HashSet(key, new[] { new HashEntry(field, num.ToString()) });
//            return Task.FromResult(num);
//        }

//        public Task<long> HashDecrementAsync(string key, RedisValue field, long num = 1)
//        {
//            return HashIncrementAsync(key, field, -num);
//        }

//        #endregion

//        #region 未实现的方法（抛出明确异常）

//        //public void HashSet(string key, List<HashKeyValue> vals)
//        //{
//        //    throw new NotImplementedException("MemoryCache 不支持此操作");
//        //}

//        public Task HashSetAsync(string key, List<HashKeyValue> vals)
//        {
//            throw new NotImplementedException("MemoryCache 不支持此操作");
//        }

//        List<HashKeyValue> IAppCache.HashGetAll(string key)
//        {
//            throw new NotImplementedException("MemoryCache 不支持此操作");
//        }

//        Task<List<HashKeyValue>> IAppCache.HashGetAllAsync(string key)
//        {
//            throw new NotImplementedException("MemoryCache 不支持此操作");
//        }

//        public Task<HashEntry[]> HashScanAsync(string key, string pattern, int size = 1, int cursor = 0, int offset = 0)
//        {
//            throw new NotImplementedException("MemoryCache 不支持 HashScan");
//        }

//        #endregion

//        #region 私有辅助方法

//        private static void SetCacheEntryOptions(ICacheEntry entry, TimeSpan? expiration)
//        {
//            if (expiration.HasValue)
//            {
//                entry.AbsoluteExpirationRelativeToNow = expiration;
//            }
//            else
//            {
//                entry.SlidingExpiration = TimeSpan.FromMinutes(5);
//            }
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <param name="felid"></param>
//        /// <param name="num"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task HashIncrementAsync(string key, string felid, int num = 1)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<string> ReadStringAsync(string key)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="key"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<T> ReadObjectAsync<T>(string key)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <param name="val"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<bool> SetAddAsync(string skey, int val)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <param name="filed"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<bool> SetContainsAsync(string skey, int filed)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <param name="filed"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<bool> SetRemoveAsync(string skey, int filed)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<long> SetRandomMemberAsync(string skey)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<long> SetLengthAsync(string skey)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <param name="sval"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<long> ListRightPushAsync(string skey, string sval)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <param name="score"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<string> SortedSetRangeByScoreAsync(string skey, long score)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<long> SortedSetRangeByRankWithScoresAsync(string skey)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<long> ListLengthAsync(string skey)
//        {
//            throw new NotImplementedException();
//        }
//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<string> ListLeftPopAsync(string skey)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <param name="skey"></param>
//        /// <param name="start"></param>
//        /// <param name="stop"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<List<string>> ListRangeAsync(string skey, int start = 0, int stop = 30)
//        {
//            throw new NotImplementedException();
//        }

//        /// <summary>
//        /// 
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <param name="skey"></param>
//        /// <param name="start"></param>
//        /// <param name="stop"></param>
//        /// <returns></returns>
//        /// <exception cref="NotImplementedException"></exception>
//        public Task<List<T>> ListRangeAsync<T>(string skey, int start = 0, int stop = 30)
//        {
//            throw new NotImplementedException();
//        }

//        #endregion


//    }

//}
