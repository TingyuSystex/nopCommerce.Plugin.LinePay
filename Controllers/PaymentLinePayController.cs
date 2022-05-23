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
using Nop.Services.Orders;
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
        private readonly IOrderService _orderService;
        public LinePayPluginController(ILinePay service,
            ISettingService settingService,
            IStoreContext storeContext,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            INotificationService notificationService,
            IOrderService orderService )
        {
            _service = service;
            _settingService = settingService;
            _storeContext = storeContext;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _notificationService = notificationService;
            _orderService = orderService;
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
        
        [HttpGet]
        public async Task<IActionResult> Confirm ([FromQuery] string orderId, [FromQuery] string transactionId)
        {
            try
            {
                var order = await _orderService.GetOrderByGuidAsync(Guid.Parse(orderId));
                order.AuthorizationTransactionId = transactionId;
                await _orderService.UpdateOrderAsync(order);

                var confirm = await _service.ConfirmLinePayAsync(orderId, transactionId);

                //return View("~/Plugins/Payments.LinePay/Views/Test.cshtml", confirm);
                return RedirectToRoute("CheckoutCompleted", new { orderId = order.Id });
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

    }
}
