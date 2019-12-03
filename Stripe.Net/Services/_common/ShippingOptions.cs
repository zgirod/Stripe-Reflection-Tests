namespace Stripe
{
    using Newtonsoft.Json;

    public class ShippingOptions
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("phone")]
        public string Phone { get; set; }

        [JsonProperty("address")]
        public AddressOptions Address { get; set; }
    }
}
