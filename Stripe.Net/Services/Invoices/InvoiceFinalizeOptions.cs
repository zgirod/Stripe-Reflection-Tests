namespace Stripe
{
    using System;
    using System.Collections.Generic;
    using Newtonsoft.Json;

    [OpenApiMapping(Path = @"/v1/invoices/{invoice}/finalize", Method = Methods.Post)]
    public class InvoiceFinalizeOptions : BaseOptions
    {
        [JsonProperty("auto_advance")]
        public bool? AutoAdvance { get; set; }
    }
}
