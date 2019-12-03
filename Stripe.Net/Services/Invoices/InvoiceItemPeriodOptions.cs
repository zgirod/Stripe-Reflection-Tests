namespace Stripe
{
    using System;
    using Newtonsoft.Json;

    public class InvoiceItemPeriodOptions
    {
        /// <summary>
        /// The end of the period, which must be greater than or equal to the start.
        /// </summary>
        [JsonProperty("end")]
        public DateTime? End { get; set; }

        /// <summary>
        /// he start of the period.
        /// </summary>
        [JsonProperty("start")]
        public DateTime? Start { get; set; }
    }
}
