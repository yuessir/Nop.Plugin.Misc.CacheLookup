using System;
using System.Collections.Generic;
using System.Text;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Misc.CacheLookup.Areas.Admin.Model
{
    public class CacheItemModel : BaseNopModel
    {
        public string Key { get; set; }

        public string Type { get; set; }

        public int? Count { get; set; }

        public string Value { get; set; }

        public int Size { get; set; }

        public DateTime ExpiryDate { get; set; }
    }
    public class CacheItemListDetails : BasePagedListModel<CacheItemModel>
    {

    }
}
