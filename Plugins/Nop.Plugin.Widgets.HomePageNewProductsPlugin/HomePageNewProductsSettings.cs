using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin
{
    public class HomePageNewProductsSettings : ISettings
    {
        public int ItemsCount { get; set; }

        public string WidgetZone { get; set; }
    }
}
