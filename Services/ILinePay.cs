using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Payments.LinePay.Models;
using Nop.Services.Payments;

namespace Nop.Plugin.Payments.LinePay.Services
{
    public interface ILinePay
    {
        Task<LinePayResponse> RequestLinePayAsync(PostProcessPaymentRequest postProcessPaymentRequest);

        Task<ConfirmResponse> ConfirmLinePayAsync(string orderId, string transactionId);
    }
}
