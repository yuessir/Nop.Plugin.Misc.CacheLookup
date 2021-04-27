
using Microsoft.AspNetCore.Mvc;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.CacheLookup.Components
{
    [ViewComponent(Name = "CacheLookup")]
    public class CacheLookupViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
           
            return View("~/Plugins/Misc.CacheLookup/Areas/Admin/Views/CacheLookup/PublicInfo.cshtml");
        }
        

       
    }
}
