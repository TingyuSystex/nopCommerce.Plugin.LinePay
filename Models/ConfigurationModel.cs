using System.ComponentModel.DataAnnotations;
using Nop.Web.Framework.Mvc.ModelBinding;
using Nop.Web.Framework.Models;

namespace Nop.Plugin.Payments.LinePay.Models
{
    public record ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }

        [NopResourceDisplayName("Plugins.Payments.LinePay.Fields.UseSandbox")]
        public bool UseSandbox { get; set; }
        public bool UseSandbox_OverrideForStore { get; set; }

        [DataType(DataType.EmailAddress)]
        [NopResourceDisplayName("Plugins.Payments.LinePay.Fields.ChannelId")]
        public string ChannelId { get; set; }
        public bool ChannelId_OverrideForStore { get; set; }

        [NopResourceDisplayName("Plugins.Payments.LinePay.Fields.ChannelSecretKey")]
        public string ChannelSecretKey { get; set; }
        public bool ChannelSecretKey_OverrideForStore { get; set; }

        
    }
}