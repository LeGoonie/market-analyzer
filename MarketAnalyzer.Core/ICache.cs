using System;
using System.Collections.Generic;

namespace MarketAnalyzer.Core
{
    public interface ICache
    {
        object Get(string key);

        T Get<T>(string key);

        List<string> GetAllKeys();

        void Invalidate(string key);

        bool IsSet(string key);

        void Set(string key, object data);

        void Set(string key, object data, TimeSpan duration);

        void Update(string key, object data, TimeSpan duration);
    }
}