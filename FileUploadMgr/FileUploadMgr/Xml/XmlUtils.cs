using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Xml.Linq;

namespace FileUploadMgr.Xml
{
    internal static class XmlUtils
    {
        public static bool TryGetXElements(string xmlPath, string key, out IEnumerable<XElement> elements)
        {
            elements = new List<XElement>();
            try
            {
                elements = XElement.Load(xmlPath).Elements(key);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
        }

        public static bool TryGetElement(string xmlPath, string key, out XElement element)
        {
            element = null;
            try
            {
                element = XElement.Load(xmlPath).Element(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryGetChild(this XElement element, string key, out XElement result)
        {
            result = null;
            try
            {
                result = element.Element(key);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryGetValue(this XElement xElement, string key, out string value)
        {
            value = string.Empty;
            try
            {
                value = xElement.Element(key).Value;
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static bool TryGetValue<T>(this XElement xElement, string key, out T value)
        {
            value = default(T);
            try
            {
                var convertor = TypeDescriptor.GetConverter(typeof(T));
                if (convertor == null) return false;

                var rawData = xElement.Element(key).Value;
                value = (T)convertor.ConvertFromString(rawData);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
