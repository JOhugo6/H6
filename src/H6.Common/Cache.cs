using H6DF.Common;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Text;

namespace H6.Common
{
  public class Cache
  {
    private static object _wasCheck = false;
    private static IMemoryCache _memoryCache;

    private Threading.LockerManager _locker = new Threading.LockerManager();

    private const int DefaultCacheTimeout = 60;

    public Cache (IMemoryCache memoryCache)
    {
      _memoryCache = memoryCache;
    }

    private void CheckKey(string key)
    {
      if (string.IsNullOrWhiteSpace(key)) throw new ArgumentException("Key is empty");
    }

    /// <summary>
    /// set object into cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <param name="expiration"></param>
    public void Set<T>(string key, T value = default(T), long expiration = DefaultCacheTimeout)
    {
      CheckKey(key);
      if (EqualityComparer<T>.Default.Equals(value, default(T))) _memoryCache.Remove(key);
      else
      {
        var options = new MemoryCacheEntryOptions().SetAbsoluteExpiration(TimeSpan.FromSeconds(expiration));
        _memoryCache.Set(key, value, options);
      }
    }

    /// <summary>
    /// remove cache object from cache
    /// </summary>
    /// <param name="key"></param>
    public void Remove(string key)
    {
      CheckKey(key);
      _memoryCache.Remove(key);
    }

    /// <summary>
    /// returns object from cache
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key"></param>
    /// <param name="clone"></param>
    /// <returns></returns>
    public T Get<T>(string key, bool clone = true)
    {
      CheckKey(key);
      var value = _memoryCache.Get<T>(key);

      if (EqualityComparer<T>.Default.Equals(value, default(T))) return value;
      if (!clone) return value;

      var clonedValue = (T)ReflectionHelper.CloneObject(value);
      return clonedValue;
    }
  }
}
