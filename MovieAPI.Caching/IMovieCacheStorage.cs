using System;

namespace MovieAPI.Caching
{
    public interface IMovieCacheStorage
    {
        void Remove(string key);
        void Store(string key, object data);
        void Store(string key, object data, DateTime absoluteExpiration, TimeSpan slidingExpiration);
        T Retrieve<T>(string key);
    }
}
