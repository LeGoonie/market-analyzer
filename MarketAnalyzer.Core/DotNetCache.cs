using System;
using System.Collections.Generic;
using System.Runtime.Caching;

namespace MarketAnalyzer.Core
{
    public class DotNetCache : ICache
    {
        //TODO: change cache time to very long time!!!
        public const int DefaultCacheTimeInMinutes = 5;

        private ObjectCache Cache { get { return MemoryCache.Default; } }

        public T Get<T>(string key)
        {
            return (T)Cache[key];
        }

        public object Get(string key)
        {
            return Cache[key];
        }

        public void Set(string key, object data)
        {
            Set(key, data, TimeSpan.FromMinutes(DefaultCacheTimeInMinutes));
        }

        public void Set(string key, object data, TimeSpan duration)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + duration;

            Cache.Add(new CacheItem(key, data), policy);
        }

        public bool IsSet(string key)
        {
            return (Cache[key] != null);
        }

        public void Invalidate(string key)
        {
            Cache.Remove(key);
        }

        public List<string> GetAllKeys()
        {
            var keys = new List<string>();

            foreach (var key in Cache)
            {
                keys.Add(key.Key);
            }

            return keys;
        }

        public void Update(string key, object data, TimeSpan duration)
        {
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration = DateTime.Now + duration;

            Cache[key] = data;
            Cache.Add(new CacheItem(key, data), policy);
        }
    }
}