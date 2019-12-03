using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using NUnit.Framework;
using Stripe;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    public class Tests
    {

        private static readonly HttpClient _client = new HttpClient();
        private SwaggerDocument _swaggerDocument = null;

        //https://stackoverflow.com/questions/22537233/json-net-how-to-deserialize-interface-property-based-on-parent-holder-object/22539730#22539730
        public class ParameterConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(Swashbuckle.AspNetCore.Swagger.IParameter));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                return jo.ToObject<NonBodyParameter>(serializer);
            }

            public override bool CanWrite
            {
                get { return false; }
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }

        private readonly JsonSerializerSettings _settings = new JsonSerializerSettings()
        {
	        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
	        NullValueHandling = NullValueHandling.Ignore,
            Converters = new List<JsonConverter>() { new ParameterConverter() }
        };

        [SetUp]
        public async Task SetupAsync()
        {

            //TODO: make a config setting
            HttpResponseMessage response = await _client.GetAsync("https://raw.githubusercontent.com/stripe/openapi/master/openapi/spec3.json");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            _swaggerDocument = JsonConvert.DeserializeObject<SwaggerDocument>(responseBody, _settings);

        }

        [Test]
        public void OptionsClassesHaveAttributes()
        {

            var excludedOptionsClasses = new List<string>()
            {
                "BaseOptions",
                "ListOptions",
                "AddressJapanOptions",
                "AddressOptions",
                "DateRangeOptions",
                "RequestOptions",
                "ShippingOptions",
                "InvoiceItemPeriodOptions"
            };

            var assembly = Assembly.Load("Stripe.Net");
            var optionsMissingAttributes = assembly.GetTypes()
                .Where(x => x.GetCustomAttributes(typeof(OpenApiMappingAttribute), true).Any() == false
                && x.Name.EndsWith("Options")
                && excludedOptionsClasses.Contains(x.Name) == false)
                .Select(x => x).ToList();

            var sb = new StringBuilder();
            foreach (var optionsMissingAttribute in optionsMissingAttributes)
            {
                sb.AppendLine(optionsMissingAttribute.Name);
            }

            var classesWithMissingAttributes = sb.ToString();
            if (string.IsNullOrWhiteSpace(classesWithMissingAttributes) == false)
            {
                Assert.Fail($"The following classes are missing the OpenApiMappingAttribute: {Environment.NewLine}{classesWithMissingAttributes}");
            }

        }

        [Test]
        public void PropertyCrossReference()
        {

            var assembly = Assembly.Load("Stripe.Net");
            var options = assembly.GetTypes().Where(x => x.GetCustomAttributes(typeof(OpenApiMappingAttribute), true).Any())
                .Select(x =>
                new {
                    type = x,
                    attribute = (OpenApiMappingAttribute)x.GetCustomAttributes(typeof(OpenApiMappingAttribute), true).SingleOrDefault()
                }).ToList();

            var hasErrors = false;
            var errorMessage = new StringBuilder();
            foreach (var option in options)
            {

                var path = _swaggerDocument.Paths.SingleOrDefault(x => x.Key == option.attribute.Path);
                var operation = GetOperation(path.Value, option.attribute.Method);
                if (operation == null)
                {
                    hasErrors = true;
                    errorMessage.AppendLine($"Could not find operation for:{Environment.NewLine}Class: {option.type.Name}{Environment.NewLine}Path: {option.attribute.Path}{Environment.NewLine}Method: {option.attribute.Method}{Environment.NewLine}{Environment.NewLine}");
                    continue;
                }

                var sdkClassPropertyNames = GetProperties(option.type).OrderBy(x => x).ToList();
                var swaggerPropertyNames = GetProperties(option.attribute.Method, operation).OrderBy(x => x).ToList();

                if (CompareLists(sdkClassPropertyNames, swaggerPropertyNames) == false)
                {
                    hasErrors = true;
                    errorMessage.AppendLine($"Property mismatch for:{Environment.NewLine}Class: {option.type.Name}{Environment.NewLine}Path: {option.attribute.Path}{Environment.NewLine}Method: {option.attribute.Method}{Environment.NewLine}Sdk has:{Environment.NewLine}{string.Join(",", sdkClassPropertyNames)}{Environment.NewLine}Swagger has:{Environment.NewLine}{string.Join(",", swaggerPropertyNames)}{Environment.NewLine}{Environment.NewLine}");
                }

            }
            
            if (hasErrors)
            {
                Assert.Fail(errorMessage.ToString());
            }

        }

        private bool CompareLists(List<string> list1, List<string> list2)
        {
            if (list1.Count != list2.Count)
                return false;

            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i].Equals(list2[i], StringComparison.InvariantCultureIgnoreCase) == false)
                    return false;
            }

            return true;
        }

        private List<string> GetProperties(Type type)
        {

            var excludedPropertyNames = new List<string>()
            {
                "ExtraParams"
            };

            var propertyNames = new List<string>();
            var properties = type.GetProperties().Where(x => excludedPropertyNames.Contains(x.Name) == false).ToList();
            foreach (var property in properties)
            {

                var jsonPropertyAttribute = (JsonPropertyAttribute)property.GetCustomAttributes(typeof(JsonPropertyAttribute), true).SingleOrDefault();
                propertyNames.Add(jsonPropertyAttribute == null ? property.Name : jsonPropertyAttribute.PropertyName);

            }

            return propertyNames;

        }

        private List<string> GetProperties(Methods method, Operation operation)
        {

            if (method == Methods.Get)
            {
                return operation.Parameters.Select(x => x.Name).OrderBy(x => x).ToList();
            }
            else
            {

                Dictionary<string, dynamic> bodyParameters = JsonConvert.DeserializeObject<Dictionary<string, dynamic>>(((JObject)((JObject)((JObject)((JObject)operation.Extensions["requestBody"])["content"])["application/x-www-form-urlencoded"])["schema"])["properties"].ToString());
                return bodyParameters.Keys.Select(x => x).ToList();

            }

        }

        private Operation GetOperation(PathItem path, Methods method)
        {

            if (path == null) return null;

            if (method == Methods.Get)
                return path.Get;
            else if (method == Methods.Post)
                return path.Post;

            return null;


        }

    }

}