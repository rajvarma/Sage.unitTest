##Sage Core Cache usage and Sample

Sage Core caching service is an extension to Windows Azure Cache, with transient fault handling. The initial implementation provides APIs for co-located cache. Following are the steps involved to use the caching service.

1. Install Sage.Core.Cache NuGet package from Sage NuGet gallery.
2. Update the Web.Config or App.Config with the Cache Identifier name, this will be typically the name of the Web or the worker project
3. Configure the web/worker to use Cache and choose Co-located.
4. Set the cache size % to desired value (Recommended 30%)
5. Set the expiration type as sliding
6. You can add more named cache if needed
7. You can DI AzureCache or manually instantiate the object and use it.

####Usage

To instantiate default cache

    ICache cache = new AzureCache();
or To instantiate a name cache, passing the name of the cache

    ICache cache = new AzureCache("NamedCache");

To Set Items from Cache

    cache.SetCacheItem("CacheKey", CacheValue);

To Get Items from Cache

    var cacheValue = cache.GetCacheItem("CacheKey");
To remove Items from Cache

    cache.RemoveCacheItem("CacheKey");
