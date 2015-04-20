# Aspect-Oriented-Cache
Implementation of Aspect oriented cache using Post Sharp 4.5.1

This is based on Adam Bells earlier release of PostSharp.Cache
This caching supports aspect oriented caching using Post Sharp.
I have used PostSharp 4.1.10 for implementing this cache.
There are number of different cache providers supported by this.
Based on needs, You can decide to store it In Process, In Memory or in Disk.

You can set the cache type in your application or web configuration file.
Example: 
<add key="CacheAspect.CacheType" value="CacheAspect.SystemMemoryCache"/>
Other Cache Types include: CacheAspect.InProcessMemoryCache, CacheAspect.BTreeCache  

You can also specify how much time we should keep object in cache in configu file.
<add key="CacheAspect.TimeToLive" value="7:0:0:0"/>

For Btree Cache, Following key will specify directory path for Disk Caching
<add key="CacheAspect.DiskPath" value=""/> 
