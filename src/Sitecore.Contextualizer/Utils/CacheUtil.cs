using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Caching;
using log4net;

namespace Sitecore.SharedSource.Contextualizer.Utils
{
	public static class CacheUtil
	{
		/// <summary>
		/// The cache namespace this util class will use to avoid conflict with other cache keys.
		/// </summary>
		private const string CacheNamespace = "Velir.SitecorePlugins.ContextualMenu";

		/// <summary>
		/// The lock object used when writing to the cache.
		/// </summary>
		private static readonly Object LockObject;

		/// <summary>
		/// The list of cached keys that this cache util object added to the cache.
		/// Gives us the ability to remove only the cache that this util created.
		/// </summary>
		private static List<string> CachedKeys;

		/// <summary>
		/// Our http context cache object.
		/// </summary>
		private static Cache Cache
		{
			get
			{
				//if we don't have a current http context then return null.
				if (HttpContext.Current == null)
				{
					return null;
				}

				//return our cache object.
				return HttpContext.Current.Cache;
			}
		}

		/// <summary>
		/// Our callback delegate for calling a method to populate the cache if it doesn't exist.
		/// </summary>
		/// <typeparam name="T">Generic return type.</typeparam>
		/// <returns>Results to add to the cache.</returns>
		public delegate T CacheCallBack<T>();

		/// <summary>
		/// Our callback delegate for calling a method to populate the cache if it doesn't exist.
		/// </summary>
		/// <typeparam name="T">Generic return type.</typeparam>
		/// <param name="args">Callback method arguments.</param>
		/// <returns>Results to add to the cache.</returns>
		public delegate T CacheCallBackWithParameters<T>(Dictionary<string, object> args);

		/// <summary>
		/// Out static constructor for initializing our static properties.
		/// </summary>
		static CacheUtil()
		{
			LockObject = new object();
			CachedKeys = new List<string>();
		}

		/// <summary>
		/// Retrieves the desired object from the cache. If the object is null, executes the callback
		/// method to set it up and store it in the cache.
		/// </summary>
		/// <typeparam name="T">A reference type</typeparam>
		/// <param name="key">The cache key, must be unique for each object</param>
		/// <param name="callback">A callback method to retrieve the object if it is not in cache.</param>
		/// <returns></returns>
		public static T GetFromCache<T>(string key, CacheCallBack<T> callback) where T : class
		{
			return GetFromCache<T>(key, TimeSpan.Zero, callback);
		}

		/// <summary>
		/// Retrieves the desired object from the cache. If the object is null, executes the callback
		/// method to set it up and store it in the cache.
		/// </summary>
		/// <typeparam name="T">A reference type</typeparam>
		/// <param name="key">The cache key, must be unique for each object</param>
		/// <param name="absoluteExpiration">A TimeSpan after which the item will expire from the cache.</param>
		/// <param name="callback">A callback method to retrieve the object if it is not in cache.</param>
		/// <returns></returns>
		public static T GetFromCache<T>(string key, TimeSpan absoluteExpiration, CacheCallBack<T> callback) where T : class
		{
			//if our cache is null then just return our results without storing them in the cache.
			//we may not be able to use the cache, but the user can still get their results.
			if (Cache == null)
			{
				//log this as a warning.  We want to use the cache whenever possible
				Logger.Warn("HttpContext cache could not be retrieved.  CacheUtil will return the results without using the cache.");

				//get our results from our callback method
				T results = null;
				if (callback != null)
				{
					results = callback();
				}

				//return our results
				return results;
			}

			//get our namespace key
			string namespaceKey = CacheNamespace + "." + key;

			//try to get our results from the cache.  No need to look them up if we have them cached.
			Object resultsCache = Cache[namespaceKey];
			if (resultsCache != null)
			{
				//return our results
				return (T)resultsCache;
			}

			//we don't have any results in the cache so get our results from the callback method passed
			//and add them to the cache.
			T newResults = null;
			if (callback != null)
			{
				newResults = callback();

				//determine the expiration for our cache
				DateTime expiration = absoluteExpiration != TimeSpan.Zero
										? DateTime.Now.Add(absoluteExpiration)
										: Cache.NoAbsoluteExpiration;

				//add our results to our cache
				AddToCache(newResults, namespaceKey, expiration);
			}

			//return our results
			return newResults;
		}

		/// <summary>
		/// Retrieves the desired object from the cache. If the object is null, executes the callback
		/// method to set it up and store it in the cache.
		/// </summary>
		/// <typeparam name="T">A reference type</typeparam>
		/// <param name="key">The cache key, must be unique for each object</param>
		/// <param name="absoluteExpiration">A TimeSpan after which the item will expire from the cache.</param>
		/// <param name="callback">A callback method to retrieve the object if it is not in cache.</param>
		/// <param name="args">The arguments to pass to the callback method.</param>
		/// <returns></returns>
		public static T GetFromCacheWithParams<T>(string key, TimeSpan absoluteExpiration, CacheCallBackWithParameters<T> callback, Dictionary<string, object> args) where T : class
		{
			//if our cache is null then just return our results without storing them in the cache.
			//we may not be able to use the cache, but the user can still get their results.
			if (Cache == null)
			{
				//log this as a warning.  We want to use the cache whenever possible
				Logger.Warn("HttpContext cache could not be retrieved.  CacheUtil will return the results without using the cache.");

				//get our results from our callback method
				T results = null;
				if (callback != null)
				{
					results = callback.DynamicInvoke(args) as T;
				}

				//return our results
				return results;
			}

			//get our namespace key
			string namespaceKey = CacheNamespace + "." + key;

			//try to get our results from the cache.  No need to look them up if we have them cached.
			Object resultsCache = Cache[namespaceKey];
			if (resultsCache != null)
			{
				//return our results
				return (T)resultsCache;
			}

			//we don't have any results in the cache so get our results from the callback method passed
			//and add them to the cache.
			T newResults = null;
			if (callback != null)
			{
				newResults = callback.DynamicInvoke(args) as T;

				//determine the expiration for our cache
				DateTime expiration = absoluteExpiration != TimeSpan.Zero
																? DateTime.Now.Add(absoluteExpiration)
																: Cache.NoAbsoluteExpiration;

				//add our results to our cache
				AddToCache(newResults, namespaceKey, expiration);
			}

			//return our results
			return newResults;
		}

		/// <summary>
		/// Will clear a specific cache key from our cache.
		/// </summary>
		/// <param name="key">The cache key to clear.</param>
		public static void Clear(string key)
		{
			//if we don't have a cache object then just return
			if (Cache == null)
			{
				return;
			}

			//auto detect if our key already has the namespace
			string namespaceKey = key;
			if (!namespaceKey.StartsWith(CacheNamespace + "."))
			{
				namespaceKey = CacheNamespace + "." + namespaceKey;
			}

			//clear our key
			Cache.Remove(namespaceKey);
		}

		/// <summary>
		/// Will clear all cache keys added through this utility class.
		/// </summary>
		public static void ClearAll()
		{
			//add all our cached keys to a new list
			List<string> cachedKeys = new List<string>(CachedKeys);

			//loop through all our keys and remove them
			foreach (string namespaceKey in cachedKeys)
			{
				Clear(namespaceKey);
			}
		}

		private static void RemoveCachedKey(string namespaceKey)
		{
			//remove our key from our stored list if we have it
			if (CachedKeys.Contains(namespaceKey))
			{
				CachedKeys.Remove(namespaceKey);
			}
		}

		private static void AddCachedKey(string namespaceKey)
		{
			//if we don't have the key then add it
			if (!CachedKeys.Contains(namespaceKey))
			{
				CachedKeys.Add(namespaceKey);
			}
		}

		/// <summary>
		/// Will add our object to the cache using the provided key.
		/// </summary>
		/// <param name="obj">The object to cache.</param>
		/// <param name="namespaceKey">The key to store the object under.</param>
		/// <param name="expiration">When the cache should expire.</param>
		private static void AddToCache(Object obj, string namespaceKey, DateTime expiration)
		{
			//if our cache is null then just return.  Can't cache into null
			if (Cache == null)
			{
				return;
			}

			//if the object we are trying to cache is null then just return.  No need to cache null
			if (obj == null)
			{
				return;
			}

			//if our cache entry is still null then add our results
			if (Cache[namespaceKey] == null)
			{
				//lock our object to prevent multiple threads from writing to it at the same time
				lock (LockObject)
				{
					//double check to make sure our cache entry is still null
					if (Cache[namespaceKey] == null)
					{
						//add our object to our cache.
						Cache.Add(namespaceKey, obj, null, expiration, Cache.NoSlidingExpiration,
															CacheItemPriority.Normal, CacheKeyRemoved);

						//add our key to our stored list
						AddCachedKey(namespaceKey);
					}
				}
			}
		}

		/// <summary>
		/// Callback method that will remove our cached key from our list when the cache is removed.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="reason"></param>
		private static void CacheKeyRemoved(string key, object value, CacheItemRemovedReason reason)
		{
			//remove our cached key from our list
			RemoveCachedKey(key);
		}

		private static ILog _logger;
		private static ILog Logger
		{
			get
			{
				if (_logger == null)
				{
					_logger = LogManager.GetLogger(typeof(CacheUtil));
				}
				return _logger;
			}
		}
	}
}
