using System;
using System.Collections.Generic;
using System.Text;

namespace Stripe
{

    public class OpenApiMappingAttribute : Attribute
    {

        public string Path { get; set; }
        public Methods Method { get; set; }

    }

    public enum Methods
    {
        Get,
        Post,
        Put,
        Patch,
        Delete
    }

}
