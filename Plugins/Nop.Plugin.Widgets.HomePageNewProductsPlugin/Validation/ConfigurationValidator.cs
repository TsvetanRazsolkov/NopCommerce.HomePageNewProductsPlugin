using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FluentValidation;
using Nop.Plugin.Widgets.HomePageNewProductsPlugin.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin.Validation
{
    public class ConfigurationValidator : BaseNopValidator<ConfigurationModel>
    {
        public ConfigurationValidator(ILocalizationService localizationService)
        {
            RuleFor(x => x.ItemsCount)
                .InclusiveBetween(ConfigurationModel.MinCountOfItemsToDisplay, ConfigurationModel.MaxCountOfItemsToDisplay)
                .WithMessage(string.Format(localizationService.GetResource("plugin.widgets.homepagenewproductsplugin.itemscountrange.errormessage"), ConfigurationModel.MinCountOfItemsToDisplay, ConfigurationModel.MaxCountOfItemsToDisplay));
        }
    }
}
