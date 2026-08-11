using Microsoft.Extensions.Caching.Memory;
using StackExchange.Redis;

namespace PasteApeart.Handler
{
    /// <summary>
    /// 本地缓存
    /// </summary>
    public class MemoryClient : IAppCache
    {
        private readonly IMemoryCache _memoryCache;
        public MemoryClient(IMemoryCache memoryCache)
        {
            _memoryCache = memoryCache;
        }

        public bool IsRedis { get => false; }

        public bool Delete(string key)
        {
            _memoryCache.Remove(key);
            return true;
        }

        /// <summary>
        /// error
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Task<bool> DeleteAsync(string key)
        {
            return Task.Run<bool>(() =>
            {
                return Delete(key);
            });
        }

        public bool Exists(string key)
        {
            return _memoryCache.TryGetValue(key, out var val);
            //throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public async Task<bool> ExistsAsync(string key)
        {
            var read = _memoryCache.TryGetValue(key, out var val);
            //throw new NotImplementedException();
            return await Task.FromResult(read);
        }

        public T GetObject<T>(string key)
        {
            if (_memoryCache.TryGetValue(key, out string val))
            {
                //return JsonConvert.DeserializeObject<T>(val);
                return System.Text.Json.JsonSerializer.Deserialize<T>(val, JsonConfig.Options);
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
            // throw new NotImplementedException();
        }

        public Task<T> GetObjectAsync<T>(string key)
        {
            return Task.Run<T>(() =>
            {
                return GetObject<T>(key);
            });
        }

        public Task<string> GetAsync(string key)
        {
            return Task.Run<string>(() =>
            {
                return Get(key);
            });
        }

        public string GetStringValue(string key)
        {
            return Get(key);
        }

        public string HashGet(string key, string sunkey)
        {
            throw new NotImplementedException();
        }

        public HashEntry[] HashGetAll(string key)
        {
            throw new NotImplementedException();
        }

        public Task<HashEntry[]> HashGetAllAsync(string key)
        {
            throw new NotImplementedException();
        }

        public Task<string> HashGetAsync(string key, string sunkey)
        {
            throw new NotImplementedException();
        }

        public void HashSet(string key, HashEntry[] vals)
        {
            throw new NotImplementedException();
        }

        //public void HashSetAsync(string key, HashEntry[] vals)
        //{
        //    throw new NotImplementedException();
        //}

        public bool Lock(string key, int seconds)
        {
            return false;
        }

        public Task<bool> LockAsync(string key, int seconds)
        {
            return Task.Run<bool>(() =>
            {
                return Lock(key, seconds);
            });
        }

        public bool Set(string key, string value, TimeSpan? expiry = null)
        {
            using (var wcache = _memoryCache.CreateEntry(key))
            {
                wcache.Value = value;
                if (expiry == null)
                {
                    wcache.AbsoluteExpiration = System.DateTimeOffset.Now.AddSeconds(30);
                }
                else
                {
                    wcache.AbsoluteExpiration = DateTimeOffset.Now.Add((TimeSpan)expiry);
                }
            }

            return true;
        }

        public bool Set(string key, string value, int seconds)
        {
            using (var wcache = _memoryCache.CreateEntry(key))
            {
                wcache.Value = value;

                wcache.AbsoluteExpiration = System.DateTimeOffset.Now.AddSeconds(seconds);

            }

            return true;
        }

        public bool Set(string key, string value)
        {
            using (var wcache = _memoryCache.CreateEntry(key))
            {
                wcache.Value = value;
            }
            return true;
        }

        public bool SetObject<T>(string key, T value, TimeSpan? expiry = null)
        {
            return Set(key, System.Text.Json.JsonSerializer.Serialize(value, JsonConfig.Options), expiry);
            //return Set(key, JsonConvert.SerializeObject(value), expiry);
        }

        public bool SetObject<T>(string key, T value, int seconds)
        {
            //return Set(key, JsonConvert.SerializeObject(value), seconds);
            return Set(key, System.Text.Json.JsonSerializer.Serialize(value, JsonConfig.Options), seconds);
        }

        public Task<bool> SetAsync(string key, string value, TimeSpan? expiry = null)
        {
            return Task.Run<bool>(() =>
            {
                return Set(key, value, expiry);
            });
        }

        public Task<bool> SetAsync(string key, string value, int seconds)
        {
            return Task.Run<bool>(() =>
            {
                return Set(key, value, seconds);
            });
        }

        public Task<bool> SetObjectAsync<T>(string key, T value, int second)
        {
            return Task.Run<bool>(() =>
            {
                return SetObject(key, value, second);
            });
        }

        public bool SetExpire(string key, TimeSpan? expiry)
        {
            if (_memoryCache.TryGetValue(key, out string sval))
            {

                return Set(key, sval, expiry);
            }
            return false;
        }

        public bool SetExpire(string key, int seconds)
        {
            if (_memoryCache.TryGetValue(key, out string sval))
            {

                return Set(key, sval, seconds);
            }
            return false;
        }

        public Task<bool> SetExpireAsync(string key, int seconds)
        {
            return Task.Run<bool>(() =>
            {
                return SetExpire(key, seconds);
            });
        }

        public Task<bool> SetExpireAsync(string key, TimeSpan? expiry)
        {
            return Task.Run<bool>(() =>
            {
                return SetExpire(key, expiry);
            });
        }

        public bool SetValue(string key, string val, int second)
        {
            return Set(key, val, second);
        }

        public bool SetValue(string key, string val)
        {
            return Set(key, val);
        }

        public bool UnLock(string key)
        {
            return false;
        }

        public Task<bool> UnLockAsync(string key)
        {
            return Task.Run<bool>(() =>
            {
                return UnLock(key);
            });
        }

        public async Task HashSetAsync(string key, HashEntry[] vals)
        {
            await Task.Run(() =>
            {
                throw new NotImplementedException();
            });

        }

        List<HashKeyValue> IAppCache.HashGetAll(string key)
        {
            throw new NotImplementedException();
        }

        Task<List<HashKeyValue>> IAppCache.HashGetAllAsync(string key)
        {
            throw new NotImplementedException();
        }

        public void HashSet(string key, List<HashKeyValue> vals)
        {
            throw new NotImplementedException();
        }

        public Task HashSetAsync(string key, List<HashKeyValue> vals)
        {
            throw new NotImplementedException();
        }
    }
}
