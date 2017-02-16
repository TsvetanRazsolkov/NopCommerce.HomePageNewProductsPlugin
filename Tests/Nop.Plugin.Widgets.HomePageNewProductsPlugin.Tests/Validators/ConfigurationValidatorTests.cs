using System;

using FluentValidation.TestHelper;
using Nop.Plugin.Widgets.HomePageNewProductsPlugin.Models;
using Nop.Plugin.Widgets.HomePageNewProductsPlugin.Validation;
using Nop.Web.MVC.Tests.Public.Validators;
using NUnit.Framework;

namespace Nop.Plugin.Widgets.HomePageNewProductsPlugin.Tests.Validators
{
    [TestFixture]
    public class ConfigurationValidatorTests : BaseValidatorTests
    {
        private static readonly Random randomGenerator = new Random();

        private ConfigurationValidator _validator;

        [SetUp]
        public new void Setup()
        {
            _validator = new ConfigurationValidator(this._localizationService);
        }

        [Test]
        public void Should_have_error_when_itemscount_is_outside_range()
        {
            var model = new ConfigurationModel();
            model.ItemsCount = ConfigurationValidatorTests.randomGenerator.Next(int.MinValue, ConfigurationModel.MinCountOfItemsToDisplay);

            _validator.ShouldHaveValidationErrorFor(x => x.ItemsCount, model);

            model.ItemsCount = ConfigurationValidatorTests.randomGenerator.Next(ConfigurationModel.MaxCountOfItemsToDisplay + 1, int.MaxValue);
            _validator.ShouldHaveValidationErrorFor(x => x.ItemsCount, model);
        }

        [Test]
        public void Should_not_have_error_when_itemscount_is_not_outside_range()
        {
            var model = new ConfigurationModel();
            model.ItemsCount = ConfigurationValidatorTests.randomGenerator.Next(ConfigurationModel.MinCountOfItemsToDisplay, ConfigurationModel.MaxCountOfItemsToDisplay);
            _validator.ShouldNotHaveValidationErrorFor(x => x.ItemsCount, model);

            model.ItemsCount = ConfigurationModel.MinCountOfItemsToDisplay;
            _validator.ShouldNotHaveValidationErrorFor(x => x.ItemsCount, model);

            model.ItemsCount = ConfigurationModel.MaxCountOfItemsToDisplay;
            _validator.ShouldNotHaveValidationErrorFor(x => x.ItemsCount, model);
        }
    }
}
