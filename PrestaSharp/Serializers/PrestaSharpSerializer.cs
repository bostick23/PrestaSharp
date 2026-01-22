using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using RestSharp;
using RestSharp.Extensions;
using RestSharp.Serializers;
using Bukimedia.PrestaSharp.Helpers;

namespace Bukimedia.PrestaSharp.Serializers
{
    public class PrestaSharpSerializer : ISerializer, IRestSerializer
    {
        public string Namespace { get; set; }
        public string RootElement { get; set; }
        public string DateFormatString { get; set; }  // Custom date format string
        public RestSharp.ContentType ContentType { get; set; } = RestSharp.ContentType.Xml;

        // ISerializer implementation
        public string Serialize(object obj) => PrestaSharpSerialize(obj);

        // IRestSerializer implementation
        public ISerializer Serializer => this;
        public IDeserializer Deserializer => new Deserializers.PrestaSharpDeserializer();
        public string[] AcceptedContentTypes => new[] { "application/xml", "text/xml" };
        public SupportsContentType SupportsContentType => contentType => 
        {
            var contentTypeString = contentType.ToString();
            return AcceptedContentTypes.Any(ct => contentTypeString.IndexOf(ct, StringComparison.OrdinalIgnoreCase) >= 0);
        };
        public DataFormat DataFormat => DataFormat.Xml;
        
        public string Serialize(Parameter parameter) => parameter.Value == null ? string.Empty : PrestaSharpSerialize(parameter.Value);

        public PrestaSharpSerializer()
        {
        }

        public PrestaSharpSerializer(string @namespace)
        {
            Namespace = @namespace;
        }

        /// <summary>
        /// Serialize the object as XML
        /// </summary>
        /// <param name="obj">Object to serialize</param>
        /// <returns>XML as string</returns>
        public string PrestaSharpSerialize(object obj)
        {
            var doc = new XDocument();

            var t = obj.GetType();
            var name = t.Name;

            var root = new XElement(name.AsNamespaced(Namespace));

            if (obj is IList)
            {
                var itemTypeName = "";
                foreach (var item in (IList)obj)
                {
                    var type = item.GetType();
                    if (itemTypeName == "")
                    {
                        itemTypeName = type.Name;
                    }
                    var instance = new XElement(itemTypeName);
                    Map(instance, item);
                    root.Add(instance);
                }
            }
            else
                Map(root, obj);

            if (RootElement.HasValue())
            {
                var wrapper = new XElement(RootElement.AsNamespaced(Namespace), root);
                doc.Add(wrapper);
            }
            else
            {
                doc.Add(root);
            }

            return doc.ToString();
        }

        private void Map(XElement root, object obj)
        {
            var objType = obj.GetType();

            var props = from p in objType.GetProperties()
                        where p.CanRead && p.CanWrite
                        select p;

            foreach (var prop in props)
            {
                var name = prop.Name;
                var rawValue = prop.GetValue(obj, null);

                //Hack to serialize Bukimedia.PrestaSharp.Entities.AuxEntities.language
                if (obj.GetType().FullName.Equals("Bukimedia.PrestaSharp.Entities.AuxEntities.language") && root.Name.LocalName.Equals("language") && name.Equals("id"))
                {

                    root.Add(new XAttribute(XName.Get("id"), rawValue));
                    continue;
                }
                else if (obj.GetType().FullName.Equals("Bukimedia.PrestaSharp.Entities.AuxEntities.language") && root.Name.LocalName.Equals("language") && name.Equals("Value"))
                {
                    XText xtext = new XText(rawValue == null ? "" : rawValue.ToString());
                    root.Add(xtext);
                    continue;

                }

                if (rawValue == null)
                {
                    continue;
                }

                var value = GetSerializedValue(rawValue);
                var propType = prop.PropertyType;

                var useAttribute = false;
                // Check for XmlElement attribute to get custom element name
                var xmlElementAttr = prop.GetCustomAttribute<System.Xml.Serialization.XmlElementAttribute>();
                if (xmlElementAttr != null && !string.IsNullOrEmpty(xmlElementAttr.ElementName))
                {
                    name = xmlElementAttr.ElementName;
                }

                var nsName = name.AsNamespaced(Namespace);
                var element = new XElement(nsName);

                if (propType.IsPrimitive || propType.IsValueType || propType == typeof(string))
                {
                    if (useAttribute)
                    {
                        root.Add(new XAttribute(name, value));
                        continue;
                    }

                    element.Value = value;
                }
                else if (rawValue is IList)
                {
                    var itemTypeName = "";
                    foreach (var item in (IList)rawValue)
                    {
                        if (itemTypeName == "")
                        {
                            var type = item.GetType();
                            itemTypeName = type.Name;
                        }
                        var instance = new XElement(itemTypeName);
                        Map(instance, item);
                        element.Add(instance);
                    }
                }
                else
                {
                    Map(element, rawValue);
                }

                root.Add(element);
            }
        }

        private string GetSerializedValue(object obj)
        {
            var output = obj;

            if (obj is DateTime && !string.IsNullOrEmpty(DateFormatString))
            {
                output = ((DateTime)obj).ToString(DateFormatString);
            }
            else if (obj is bool)
            {
                output = obj.ToString().ToLowerInvariant();
            }
            else if (obj is decimal)
            {
                output = obj.ToString().Replace(",", ".");
            }

            return output.ToString();
        }
    }
}
