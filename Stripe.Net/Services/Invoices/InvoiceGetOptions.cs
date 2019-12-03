namespace Stripe
{

    [OpenApiMapping(Path = @"/v1/invoices/{invoice}", Method = Methods.Get)]
    public class InvoiceGetOptions : BaseOptions
    {
    }
}
