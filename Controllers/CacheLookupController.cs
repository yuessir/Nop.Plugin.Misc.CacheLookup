
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Plugin.Misc.CacheLookup.Areas.Admin.Model;
using Nop.Services.Helpers;
using Nop.Services.Security;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.Extensions;
using Nop.Web.Framework.Mvc;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.CacheLookup.Controller
{
    [AuthorizeAdmin]
    [Area("Admin")]
    [AutoValidateAntiforgeryToken]
    public class CacheLookupController : BasePluginController
    {
        private readonly IDateTimeHelper _dateTimeHelper;
        private readonly IPermissionService _permissionService;
        private readonly IMemoryCache _memoryCache;
        public CacheLookupController(IPermissionService permissionService, IDateTimeHelper dateTimeHelper, IMemoryCache memoryCache)
        {
            _permissionService = permissionService;
            _dateTimeHelper = dateTimeHelper;
            _memoryCache = memoryCache;
        }
        private IPagedList<CacheItemModel> GetCacheItems()
        {
            var list = new List<CacheItemModel>();

            var cache = _memoryCache;

            var storesField = typeof(MemoryCache).GetProperty("EntriesCollection", BindingFlags.NonPublic | BindingFlags.Instance);

            if (storesField != null)
            {
                if (storesField.GetValue(cache) is ICollection stores)
                {
                    // MemoryCacheStore
                    foreach (var store in stores)
                    {
                        var entriesField = store.GetType().GetProperty("Key");
                        var entriesFieldVal = store.GetType().GetProperty("Value");

                        if (entriesField != null)
                        {
                            var entries = (ICacheEntry)entriesFieldVal.GetValue(store);

                            if (entries != null)
                            {

                                var cacheItem = new CacheItemModel();

                                // MemoryCacheKey
                                cacheItem.Key = entries.Key.ToString();

                                // MemoryCacheEntry

                                if (entries.Value != null)
                                {
                                    var entryValue = entries.Value;

                                    if (entryValue != null)
                                    {
                                        // type
                                        if (entryValue is IDictionary)
                                        {
                                            cacheItem.Type = "Dictionary";
                                            cacheItem.Count = ((IDictionary)entryValue).Count;
                                        }
                                        else if (entryValue is IList)
                                        {
                                            cacheItem.Type = "List";
                                            cacheItem.Count = ((IList)entryValue).Count;
                                        }
                                        else
                                        {
                                            cacheItem.Type = entryValue.GetType().Name;
                                        }

                                        // value
                                        cacheItem.Value = JsonConvert.SerializeObject(entryValue, Formatting.Indented);

                                        // size
                                        var size = 0;

                                        if (entryValue is IEnumerable)
                                        {
                                            foreach (var item in ((IEnumerable)entryValue))
                                            {
                                                size += GetObjectSize(item);
                                            }
                                        }
                                        else
                                        {
                                            size = GetObjectSize(entryValue);
                                        }

                                        cacheItem.Size = size;
                                    }
                                }

                                var utcAbsExpProp = entries.AbsoluteExpiration;


                                var entryExpiry = utcAbsExpProp;

                                if (entryExpiry != null)
                                {
                                    var tz = _dateTimeHelper.CurrentTimeZone;
                                    var date = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(entryExpiry.Value.UtcDateTime,
                                        tz.Id);
                                    cacheItem.ExpiryDate = date;
                                }


                                if (cacheItem.Key != null && cacheItem.Value != null)
                                {
                                    list.Add(cacheItem);
                                }

                            }
                        }
                    }
                }
            }
            return new PagedList<CacheItemModel>(list, 0, 15, list.Count);
   
        }
       
        private int GetObjectSize(object obj)
        {
            return (int)typeof(BareMetal)
                                .GetMethod("SizeOf")
                                .MakeGenericMethod(obj.GetType())
                                .Invoke(obj, null);
        }
  
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            return ViewComponent("CacheLookup");
        }

        public IActionResult CacheItems()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();
            return Json(GetCacheItemListDetails());
        }

        private CacheItemListDetails GetCacheItemListDetails()
        {
            var result = GetCacheItems();
            var model = new CacheItemListDetails().PrepareToGrid(null, result, () =>
            {
                return result.Select(rate => rate);
            });
            return model;
        }
        public ActionResult DeleteCacheItem(string key)
        {
            RemoveCacheItem(key);
            return Json(GetCacheItemListDetails());
        }
      
        private void RemoveCacheItem(string key)
        {
            var cache = _memoryCache;

            if (cache.TryGetValue(key, out var val))
            {
                cache.Remove(key);
            }
        }



    }
    public static class BareMetal
    {
        public static unsafe int SizeOf<T>()
        {
            return Unsafe.SizeOf<T>();
        }
    }
}
