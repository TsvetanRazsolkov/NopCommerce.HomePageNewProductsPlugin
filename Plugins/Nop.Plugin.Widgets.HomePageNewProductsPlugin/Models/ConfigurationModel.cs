using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using FluentValidation.Attributes;
using Nop.Plugin.Widgets.HomePageNewProductsPlugin.Validation;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin.Models
{
    [Validator(typeof(ConfigurationValidator))]
    public class ConfigurationModel : BaseNopModel
    {
        public const int MinCountOfItemsToDisplay = 1;
        public const int MaxCountOfItemsToDisplay = 100;
        
        public ConfigurationModel()
        {
            AvailableZones = new List<SelectListItem>();
        }

        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Admin.ContentManagement.Widgets.ChooseZone")]
        public string ZoneId { get; set; }
        public IList<SelectListItem> AvailableZones { get; set; }
        public bool ZoneId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugin.Widgets.HomePageNewProductsPlugin.ItemsCount")]
        public int ItemsCount { get; set; }
        public bool ItemsCount_OverrideForStore { get; set; }

        public string ItemsCountRangeHint { get; set; }
    }
}
