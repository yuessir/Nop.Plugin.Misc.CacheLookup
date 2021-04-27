using System;
using System.Collections.Generic;
using Nop.Core;
using Nop.Services.Cms;
using Nop.Services.Plugins;

namespace Nop.Plugin.Misc.CacheLookup
{
    public class CacheLookupPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly IWebHelper _webHelper;

        public CacheLookupPlugin( IWebHelper webHelper)
        {
            _webHelper = webHelper;
          
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;

        public IList<string> GetWidgetZones()
        {
            return new List<string> { "CacheLookup" };
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "CacheLookup";
        }

        //public void ManageSiteMap(SiteMapNode rootNode)
        //{
        //    var pluginNode = new SiteMapNode()
        //    {
        //        SystemName = "CacheLookup",
        //        Title = "Cache Lookup",
        //        Visible = true
        //    };

        //    pluginNode.ChildNodes.Add(new SiteMapNode()
        //    {
        //        SystemName = "CacheLookup",
        //        Title = "MemoryCache",
        //        ControllerName = "CacheLookup",
        //        ActionName = "List",
        //        IconClass = "fa-dot-circle-o",
        //        Visible = true,
        //        RouteValues = new RouteValueDictionary() { { "area", AreaNames.Admin } },
        //    });

        //    rootNode.ChildNodes.Add(pluginNode);
        //}

        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation((bool?)null)}Admin/CacheLookup/Configure";
        }


        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            base.Uninstall();
        }
    }
}
