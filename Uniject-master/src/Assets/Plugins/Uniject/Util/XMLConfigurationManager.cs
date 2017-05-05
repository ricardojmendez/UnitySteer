using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Uniject.Configuration {

    /// <summary>
    /// Applied to primitive types including string, float and double to have them read automatically
    /// from a specified XML file at a specified XPath.
    /// </summary>
    public class XMLConfigValue : System.Attribute {
        public string file { get; private set; }
        public string xpath { get; private set; }
        public XMLConfigValue(string file, string xpath) {
            this.file = file;
            this.xpath = xpath;
        }
    }

    /// <summary>
    /// Caches our XML files.
    /// </summary>
    public class XMLConfigManager {

        private Dictionary<string, XDocument> documentCache = new Dictionary<string, XDocument>();
        private IResourceLoader loader;
        public XMLConfigManager(IResourceLoader loader) {
            this.loader = loader;
        }

        public T getValue<T>(string xmlFile, string xpath) {
            XDocument doc = null;
            if (documentCache.ContainsKey(xmlFile)) {
                doc = documentCache[xmlFile];
            }

            if (null == doc) {
                doc = loader.loadDoc(xmlFile);
                documentCache.Add(xmlFile, doc);
            }

            XElement element = doc.XPathSelectElement(xpath);

            if (null == element) {
                throw new ArgumentException("Xpath element not found:" + xpath);
            }
            return (T)(Convert.ChangeType(element.Value, typeof(T), CultureInfo.InvariantCulture));
        }
    }
}
