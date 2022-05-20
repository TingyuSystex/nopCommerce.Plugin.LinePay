using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Payments.LinePay.Models;
using Nop.Plugin.Payments.LinePay.Services;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Models.DataTables;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.LinePay.Controllers
{
    [AutoValidateAntiforgeryToken]
    [AuthorizeAdmin] //confirms access to the admin panel
    [Area(AreaNames.Admin)] //specifies the area containing a controller or action
    public class LinePayPluginController : BasePaymentController
    {
        private readonly ILinePay _service;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService; 
        private readonly INotificationService _notificationService;
        public LinePayPluginController(ILinePay service,
            ISettingService settingService,
            IStoreContext storeContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            INotificationService notificationService)
        {
            _service = service;
            _settingService = settingService;
            _storeContext = storeContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
        }
        
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var linePaySettings = await _settingService.LoadSettingAsync<LinePaySettings>(storeScope);

            var model = new ConfigurationModel
            {
                UseSandbox = linePaySettings.UseSandbox,
                ChannelId = linePaySettings.ChannelId,
                ChannelSecretKey = linePaySettings.ChannelSecretKey,
                ActiveStoreScopeConfiguration = storeScope
            };


            if (storeScope <= 0)
                return View("~/Plugins/Payments.LinePay/Views/Configure.cshtml", model);

            model.UseSandbox_OverrideForStore = await _settingService.SettingExistsAsync(linePaySettings, x => x.UseSandbox, storeScope);
            model.ChannelId_OverrideForStore = await _settingService.SettingExistsAsync(linePaySettings, x => x.ChannelId, storeScope);
            model.ChannelSecretKey_OverrideForStore = await _settingService.SettingExistsAsync(linePaySettings, x => x.ChannelSecretKey, storeScope);
            
            return View("~/Plugins/Payments.LinePay/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var linePaySettings = await _settingService.LoadSettingAsync<LinePaySettings>(storeScope);

            //save settings
            linePaySettings.UseSandbox = model.UseSandbox;
            linePaySettings.ChannelId = model.ChannelId;
            linePaySettings.ChannelSecretKey = model.ChannelSecretKey;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(linePaySettings, x => x.UseSandbox, model.UseSandbox_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(linePaySettings, x => x.ChannelId, model.ChannelId_OverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(linePaySettings, x => x.ChannelSecretKey, model.ChannelSecretKey_OverrideForStore, storeScope, false);
            
            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        [HttpPost]
        public async Task<IActionResult> RequestLinePay()
        {
            try
            {
                return Ok(new DataTablesModel { Data = await _service.RequestLinePayAsync() });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }
        
    }
}
