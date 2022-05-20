using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;

namespace Nop.Plugin.Payments.LinePay
{
    public class LinePayPlugin : BasePlugin
    {
        private readonly IWebHelper _webHelper;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;

        public LinePayPlugin(IWebHelper webHelper, 
            ILocalizationService localizationService,
            ISettingService settingService
            )
        {
            _webHelper = webHelper;
            _localizationService = localizationService;
            _settingService = settingService;
        }
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/LinePayPlugin/Configure";
        }

        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new LinePaySettings
            {
                UseSandbox = true
            });

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payments.LinePay.Fields.ChannelId"] = "Channel ID",
                ["Plugins.Payments.LinePay.Fields.ChannelId.Hint"] = "輸入Channel ID",
                ["Plugins.Payments.LinePay.Fields.ChannelSecretKey	"] = "Channel Secret Key",
                ["Plugins.Payments.LinePay.Fields.ChannelSecretKey.Hint"] = "輸入Channel Secret Key",
                ["Plugins.Payments.LinePay.Fields.UseSandbox"] = "Use Sandbox",
                ["Plugins.Payments.LinePay.Fields.UseSandbox.Hint"] = "使用測試環境",
                //["Plugins.Payments.LinePay.Fields.RedirectionTip"] = "You will be redirected to PayPal site to complete the order.",
                ["Plugins.Payments.LinePay.Instructions"] = @"
                    <p>
	                    <b>LinePay</b>
	                    <br />
	                    <br />註冊為LINE Pay的商家，可以吸引全球的LINE用戶作為自己的客戶。此外，通過LINE擴展商家的銷售管道，可以預見銷售額的迅速成長。
                        <br />只有註冊為LINE Pay的商家，才能為LINE Pay用戶提供線上付款服務。<br />
                        <br />進入申請頁面（<a href=""https://pay.line.me"" target=""_blank"">https://pay.line.me</a>）<br />
	                    <br />完成LINE Pay商家註冊後，將會提供用於Sandbox和Production環境的“Channel Id”和“Channel SecretKey”。<br />
	                    <br />商店後台：<a href=""https://pay.line.me/tw/center/test/main?locale=zh_TW"" target=""_blank"">https://pay.line.me/tw/center/test/main?locale=zh_TW</a> <br />
	                    <br />
                    </p>",
                ["Plugins.Payments.LinePay.PaymentMethodDescription"] = "前往LinePay頁面完成付款。",
                //["Plugins.Payments.LinePay.RoundingWarning"] = "It looks like you have \"ShoppingCartSettings.RoundPricesDuringCalculation\" setting disabled. Keep in mind that this can lead to a discrepancy of the order total amount, as PayPal only rounds to two decimals.",

            });

            await base.InstallAsync();
        }

        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<LinePaySettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.LinePay");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task<string> GetPaymentMethodDescriptionAsync()
        {
            return await _localizationService.GetResourceAsync("Plugins.Payments.LinePay.PaymentMethodDescription");
        }

    }
}