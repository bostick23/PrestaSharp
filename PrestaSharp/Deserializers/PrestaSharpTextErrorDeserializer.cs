using RestSharp;
using RestSharp.Serializers;
using System;

namespace Bukimedia.PrestaSharp.Deserializers
{
    public class PrestaSharpTextErrorDeserializer : IDeserializer
    {
        public T Deserialize<T>(RestResponse response)
        {
            throw new Exception("Prestashop failed to serve XML response instead got text: " + response.Content);
        }

        public string RootElement { get; set; }
        public string Namespace { get; set; }
        public string DateFormat { get; set; }
    }
}