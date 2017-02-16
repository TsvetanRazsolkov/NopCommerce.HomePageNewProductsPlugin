using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Routing;

using Nop.Core.Plugins;
using Nop.Services.Cms;
using Nop.Services.Configuration;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin
{
    public class HomePageNewProductsPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ISettingService _settingService;
        private readonly HomePageNewProductsSettings _newProductsSettings;

        public HomePageNewProductsPlugin(ISettingService settingService, HomePageNewProductsSettings newProductsSettings)
        {
            this._settingService = settingService;
            this._newProductsSettings = newProductsSettings;
        }

        public void GetConfigurationRoute(out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "WidgetsHomePageNewProductsPlugin";
            routeValues = new RouteValueDictionary { { "Namespaces", "Nop.Plugin.Widgets.HomePageNewProductsPlugin.Controllers" }, { "area", null } };
        }

        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out System.Web.Routing.RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "WidgetsHomePageNewProductsPlugin";
            routeValues = new RouteValueDictionary
            {
                {"Namespaces", "Nop.Plugin.Widgets.HomePageNewProductsPlugin.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        public IList<string> GetWidgetZones()
        {
            return !string.IsNullOrWhiteSpace(this._newProductsSettings.WidgetZone)
                       ? new List<string>() { this._newProductsSettings.WidgetZone }
                       : new List<string>() { "home_page_before_categories" };
        }
    }
}
