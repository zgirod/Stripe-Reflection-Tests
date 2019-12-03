namespace Stripe
{
    using System;
    using Newtonsoft.Json;

    [OpenApiMapping(Path = "/v1/invoices", Method = Methods.Get)]
    public class InvoiceListOptions : ListOptions
    {
        /// <summary>
        /// Either <c>charge_automatically</c>, or <c>send_invoice</c>.
        /// </summary>
        [JsonProperty("collection_method")]
        public string CollectionMethod { get; set; }

        /// <summary>
        /// A filter on the list based on the object <c>created</c> field. The value can be a
        /// <see cref="DateTime"/> or a <see cref="DateRangeOptions"/>.
        /// </summary>
        [JsonProperty("created")]
        public DateTime Created { get; set; }

        /// <summary>
        /// Only return invoices for the customer specified by this customer ID.
        /// </summary>
        [JsonProperty("customer")]
        public string Customer { get; set; }

        /// <summary>
        /// A filter on the list based on the object <c>due_date</c> field. The value can be a
        /// <see cref="DateTime"/> or a <see cref="DateRangeOptions"/>.
        /// </summary>
        [JsonProperty("due_date")]
        public DateTime DueDate { get; set; }

        /// <summary>
        /// A filter on the list based on the object paid field.
        /// </summary>
        [JsonProperty("paid")]
        public bool? Paid { get; set; }

        /// <summary>
        /// Only return invoices for the subscription specified by this subscription ID.
        /// </summary>
        [JsonProperty("subscription")]
        public string Subscription { get; set; }
    }
}
