using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.LinePay.Infrastructure
{
    /// <summary>
    /// Represents plugin route provider
    /// </summary>
    public class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //add route for the access token callback
            //endpointRouteBuilder.MapControllerRoute("LinePayRequest", "Plugins/Payments/LinePay/",
            //    new { controller = "PaymentLinePay", action = "RequestLinePayAsync" });
            //endpointRouteBuilder.MapControllerRoute("LinePayRequest", "Plugins.Payments/LinePay/",
            //    new { controller = "PaymentLinePay", action = "RequestLinePayAsync" });
            //endpointRouteBuilder.MapControllerRoute("LinePayRequest", "Plugins/Payment/LinePay/",
            //    new { controller = "PaymentLinePay", action = "RequestLinePayAsync" });
            //endpointRouteBuilder.MapControllerRoute("LinePayRequest", "Plugins.Payment/LinePay/",
            //    new { controller = "PaymentLinePay", action = "RequestLinePayAsync" });
            //endpointRouteBuilder.MapControllerRoute("LinePayRequest", "Admin/LinePayPlugin/RequestLinePayAsync/",
            //    new { controller = "PaymentLinePay", action = "RequestLinePayAsync" });
            //endpointRouteBuilder.MapControllerRoute("LinePayRequest", "LinePayPlugin/RequestLinePayAsync/",
            //    new { controller = "PaymentLinePay", action = "RequestLinePayAsync" });
            //endpointRouteBuilder.MapControllerRoute("CustomersDistributionByCountry", "Plugins/Payments/CustomerDistByCountry/",
            //    new { controller = "PaymentLinePay", action = "RequestLinePayAsync" });
            //endpointRouteBuilder.MapControllerRoute("CustomersDistributionByCountry", "Plugins/Payment/CustomerDistByCountry/",
            //    new { controller = "PaymentLinePay", action = "RequestLinePayAsync" });
            //endpointRouteBuilder.MapControllerRoute("CustomersDistributionByCountry", "Plugins/Tutorial/CustomerDistByCountry/",
            //    new { controller = "PaymentLinePay", action = "RequestLinePayAsync" });
        }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
        public int Priority => 0;
    }
}
