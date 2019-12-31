using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Masterpiece.Code.Cache
{

    public delegate TValue ValueProvider<TKey, TValue>(TKey key);

    public delegate TValue ValueProviderType<TKey, TValue>(TKey key, int type);

    public sealed class MemoryCacheObject<TKey>
    {
        #region Fields

        private readonly IDictionary<Type, IDictionary<TKey, object>> cache = new Dictionary<Type, IDictionary<TKey, object>>();
        #endregion

        #region Constructor (s) / Destructor


        #endregion

        #region Properties

        #endregion

        #region Public Methods

        public IDictionary<TKey, T> GetAll<T>()
        {
            IDictionary<TKey, T> returnedCollection = new Dictionary<TKey, T>();
            if (cache.ContainsKey(typeof(T)))
            {
                foreach (KeyValuePair<TKey, object> kvp in cache[typeof(T)])
                {
                    returnedCollection.Add(kvp.Key, (T)kvp.Value);
                }
            }
            return returnedCollection;
        }

        public T Get<T>(TKey key)
        {
            return Get<T>(key, null);
        }

        public T Get<T>(TKey key, ValueProvider<TKey, T> valueProvider)
        {
            if (Contains<T>(key))
            {
                return (T)cache[typeof(T)][key];
            }

            if (valueProvider != null)
            {
                T value = valueProvider(key);
                Set(key, value);
                return value;
            }
            return default(T);
        }

        public T Get<T>(TKey key, ValueProviderType<TKey, T> valueProvider, int type)
        {
            if (Contains<T>(key))
            {
                return (T)cache[typeof(T)][key];
            }

            if (valueProvider != null)
            {
                T value = valueProvider(key, type);
                Set(key, value);
                return value;
            }
            return default(T);
        }

        public object Get(Type type, TKey key, ValueProvider<TKey, object> valueProvider = null)
        {
            if (Contains(type, key))
            {
                return cache[type][key];
            }

            if (valueProvider != null)
            {
                var value = valueProvider(key);
                Set(key, value);
                return value;
            }
            return null;
        }

        public void Set<T>(TKey key, T value)
        {
            Set(typeof(T), key, value);
        }

        public void Set(Type type, TKey key, object value)
        {
            if (!cache.ContainsKey(type))
            {
                cache[type] = new Dictionary<TKey, object>();
            }

            cache[type][key] = value;
        }

        public bool Contains<T>(TKey key)
        {
            return Contains(typeof(T), key);
        }

        public bool Contains(Type type, TKey key)
        {
            bool result = false;
            if (cache.ContainsKey(type))
            {
                if (cache[type].ContainsKey(key))
                {
                    result = true;
                }
            }
            return result;
        }

        public int GetItemCount<T>()
        {
            int count = 0;
            if (cache.ContainsKey(typeof(T)))
            {
                count = cache[typeof(T)].Count;
            }
            return count;
        }

        public void Clear<T>()
        {
            if (cache.ContainsKey(typeof(T)))
            {
                cache[typeof(T)].Clear();
            }
        }

        public void ClearAll()
        {
            cache.Clear();
        }
        #endregion

        #region Private Methods

        #endregion
    }
}