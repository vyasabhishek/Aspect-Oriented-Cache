# Aspect-Oriented-Cache
* 1. What is this library?

This is based on Adam Bells earlier release of PostSharp.Cache
This caching supports aspect oriented caching using Post Sharp.
I have used PostSharp 4.1.10 for implementing this cache.
There are number of different cache providers supported by this.
Based on needs, You can decide to store it In Process, In Memory or in Disk.


* 2. Simple Example
#+BEGIN_SRC CSHARP
[Cache.Cacheable] //this method now cached, will only be called once per guid
public SomeExpensiveObject GetExpensiveObject(Guid userId)
{
..
}
#+END_SRC 
* 3. Cache with invalidation:
#+BEGIN_SRC CSHARP
[Cache.Cacheable("UniqueKeyForThisMethod")] //cache using this key plus parameter(s)
public SomeObject GetObjectById(Int32 Id)
{
...
}

[Cache.TriggerInvalidation("UniqueKeyForThisMethod")] //delete from cache using this key and passed parameter(s)
public void RemoveObjectById(Int32 Id)
{
..
} 
#+END_SRC 

* 4. How to configure this library? 
You can set the cache type in your application or web configuration file.
Example: 
<add key="CacheAspect.CacheType" value="CacheAspect.SystemMemoryCache"/>
Other Cache Types include: CacheAspect.InProcessMemoryCache, CacheAspect.BTreeCache  

You can also specify how much time we should keep object in cache in configu file.
<add key="CacheAspect.TimeToLive" value="7:0:0:0"/>

For Btree Cache, Following key will specify directory path for Disk Caching
<add key="CacheAspect.DiskPath" value=""/> 
