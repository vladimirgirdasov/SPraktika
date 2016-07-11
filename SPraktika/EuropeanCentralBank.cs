using System.IO;
using System.Linq;
using System.Net;
using System.Xml;
using System.Xml.Linq;

namespace SPraktika
{
    internal class EuropeanCentralBank : CurrencyData, IXmlPage
    {
        private string address = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";//xmlns data page URL

        private XNamespace ns_gesmes = "http://www.gesmes.org/xml/2002-08-01";//xmlns namespaces
        private XNamespace ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

        public string Address
        {
            get
            {
                return address;
            }
        }

        public string Read()
        {
            string ans = "";

            WebClient client = new WebClient();
            Stream stream = client.OpenRead(this.Address);
            XDocument xdoc = XDocument.Load(stream);

            var items = from xe in xdoc.Element(ns_gesmes + "Envelope").Element(ns + "Cube").Element(ns + "Cube").Elements(ns + "Cube")
                        select new Rating
                        {
                            Name = xe.Attribute("currency").Value,
                            Price = xe.Attribute("rate").Value
                        };
            foreach (var item in items)
                ans += item.Name + " " + item.Price + "\n";

            return ans;
        }
    }
}