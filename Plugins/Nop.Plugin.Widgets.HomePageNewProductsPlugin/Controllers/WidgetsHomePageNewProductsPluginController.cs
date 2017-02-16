using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Directory;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Tax;
using Nop.Plugin.Widgets.HomePageNewProductsPlugin.Models;
using Nop.Web.Extensions;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models.Catalog;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin.Controllers
{
    public class WidgetsHomePageNewProductsPluginController : BasePluginController
    {
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly IStoreService _storeService;
        private readonly ISettingService _settingService;
        private readonly IProductService _productsService;
        private readonly ILocalizationService _localizationService;
        private readonly ICategoryService _categoryService;
        private readonly ISpecificationAttributeService _specificationAttributeService;
        private readonly IPriceCalculationService _priceCalculationService;
        private readonly IPriceFormatter _priceFormatter;
        private readonly IPermissionService _permissionService;
        private readonly ITaxService _taxService;
        private readonly ICurrencyService _currencyService;
        private readonly IPictureService _pictureService;
        private readonly IMeasureService _measureService;
        private readonly IWebHelper _webHelper;
        private readonly ICacheManager _cacheManager;

        public WidgetsHomePageNewProductsPluginController(IWorkContext workContext,
            IStoreContext storeContext,
            IStoreService storeService,
            ISettingService settingService,
            IProductService productsService,
            ILocalizationService localizationService,
            ICategoryService categoryService,
            ISpecificationAttributeService specificationAttributeService,
            IPriceCalculationService priceCalculationService,
            IPriceFormatter priceFormatter,
            IPermissionService permissionService,
            ITaxService taxService,
            ICurrencyService currencyService,
            IPictureService pictureService,
            IMeasureService measureService,
            IWebHelper webHelper,
            ICacheManager cacheManager)
        {
            this._workContext = workContext;
            this._storeContext = storeContext;
            this._storeService = storeService;
            this._settingService = settingService;
            this._productsService = productsService;
            this._localizationService = localizationService;
            this._categoryService = categoryService;
            this._specificationAttributeService = specificationAttributeService;
            this._priceCalculationService = priceCalculationService;
            this._priceFormatter = priceFormatter;
            this._permissionService = permissionService;
            this._taxService = taxService;
            this._currencyService = currencyService;
            this._pictureService = pictureService;
            this._measureService = measureService;
            this._webHelper = webHelper;
            this._cacheManager = cacheManager;
        }

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var homePageNewProductsSettings = _settingService.LoadSetting<HomePageNewProductsSettings>(storeScope);
            var model = new ConfigurationModel();
            model.ItemsCount = homePageNewProductsSettings.ItemsCount;
            model.ItemsCountRangeHint = string.Format(_localizationService.GetResource("plugin.widgets.homepagenewproductsplugin.itemscountrange.hint"), ConfigurationModel.MinCountOfItemsToDisplay, ConfigurationModel.MaxCountOfItemsToDisplay);
            model.ZoneId = homePageNewProductsSettings.WidgetZone;
            model.AvailableZones.Add(new SelectListItem() { Text = "Before body end html tag", Value = "body_end_html_tag_before" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Head html tag", Value = "head_html_tag" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Home page top", Value = "home_page_top" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Home page before categories", Value = "home_page_before_categories" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Home page before products", Value = "home_page_before_products" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Home page before best sellers", Value = "home_page_before_best_sellers" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Home page before news", Value = "home_page_before_news" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Home page before poll", Value = "home_page_before_poll" });
            model.AvailableZones.Add(new SelectListItem() { Text = "Home page bottom", Value = "home_page_bottom" });

            model.ActiveStoreScopeConfiguration = storeScope;
            if (storeScope > 0)
            {
                model.ItemsCount_OverrideForStore = _settingService.SettingExists(homePageNewProductsSettings, x => x.ItemsCount, storeScope);
                model.ZoneId_OverrideForStore = _settingService.SettingExists(homePageNewProductsSettings, x => x.WidgetZone, storeScope);
            }

            return View("~/Plugins/Widgets.HomePageNewProductsPlugin/Views/WidgetsHomePageNewProductsPlugin/Configure.cshtml", model);
        }

        [HttpPost]
        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure(ConfigurationModel model)
        {
            if (ModelState.IsValid)
            {
                var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
                var homePageNewProductsSettings = _settingService.LoadSetting<HomePageNewProductsSettings>(storeScope);
                homePageNewProductsSettings.ItemsCount = model.ItemsCount;
                homePageNewProductsSettings.WidgetZone = model.ZoneId;

                _settingService.SaveSettingOverridablePerStore(homePageNewProductsSettings, x => x.ItemsCount, model.ItemsCount_OverrideForStore, storeScope, false);
                _settingService.SaveSettingOverridablePerStore(homePageNewProductsSettings, x => x.WidgetZone, model.ZoneId_OverrideForStore, storeScope, false);

                _settingService.ClearCache();

                SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            }

            return this.Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone, object additionalData = null)
        {
            var storeScope = this.GetActiveStoreScopeConfiguration(_storeService, _workContext);
            var homePageNewProductsSettings = _settingService.LoadSetting<HomePageNewProductsSettings>(storeScope);

            var products = this._productsService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,                
                orderBy: ProductSortingEnum.CreatedOn,
                pageSize: homePageNewProductsSettings.ItemsCount);

            var model = new List<ProductOverviewModel>();
            model.AddRange(this.PrepareProductOverviewModels(products : products, storeScope : storeScope));

            return View("~/Plugins/Widgets.HomePageNewProductsPlugin/Views/WidgetsHomePageNewProductsPlugin/PublicInfo.cshtml", model);
        }

        [NonAction]
        protected virtual IEnumerable<ProductOverviewModel> PrepareProductOverviewModels(IEnumerable<Product> products, int storeScope = 0,
            bool preparePriceModel = true, bool preparePictureModel = true,
            int? productThumbPictureSize = null, bool prepareSpecificationAttributes = false,
            bool forceRedirectionAfterAddingToCart = false)
        {
            var catalogSettings = this._settingService.LoadSetting<CatalogSettings>(storeScope);
            var mediaSettings = this._settingService.LoadSetting<MediaSettings>(storeScope);

            return this.PrepareProductOverviewModels(this._workContext,
            this._storeContext,
            this._categoryService,
            this._productsService,
            this._specificationAttributeService,
            this._priceCalculationService,
            this._priceFormatter,
            this._permissionService,
            this._localizationService,
            this._taxService,
            this._currencyService,
            this._pictureService,
            this._measureService,
            this._webHelper,
            this._cacheManager,
            catalogSettings,
            mediaSettings,
            products,
            preparePriceModel, preparePictureModel,
            productThumbPictureSize, prepareSpecificationAttributes,
            forceRedirectionAfterAddingToCart);
        }
    }
}
