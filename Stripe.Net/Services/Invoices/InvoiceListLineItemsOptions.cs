namespace Stripe
{
    using System.Collections.Generic;
    using Newtonsoft.Json;


    [OpenApiMapping(Path = "/v1/invoices/{invoice}/lines", Method = Methods.Get)]
    public class InvoiceListLineItemsOptions : ListOptions
    {
        [JsonProperty("coupon")]
        public string Coupon { get; set; }

        [JsonProperty("customer")]
        public string Customer { get; set; }

        [JsonProperty("subscription")]
        public string Subscription { get; set; }

        [JsonProperty("subscription_items")]
        public List<string> SubscriptionItems { get; set; }

        [JsonProperty("subscription_plan")]
        public string SubscriptionPlan { get; set; }
    }
}
