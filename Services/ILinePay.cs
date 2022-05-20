using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Plugin.Payments.LinePay.Models;

namespace Nop.Plugin.Payments.LinePay.Services
{
    public interface ILinePay
    {
        Task<List<LinePayResponse>> RequestLinePayAsync();
    }
}
