using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace SPraktika
{
    internal class EuropeanCentralBank : CurrencyData, IXmlPage
    {
        private string address = "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml";//xmlns data page URL

        private XNamespace ns_gesmes = "http://www.gesmes.org/xml/2002-08-01";//xmlns namespaces
        private XNamespace ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

        public EuropeanCentralBank()
        {
            CurrencyRates = new System.Collections.Generic.Dictionary<string, double>();
        }

        public string Address
        {
            get { return address; }
        }

        public void Read()
        {
            XDocument xdoc = XDocument.Load(new WebClient().OpenRead(this.Address));

            var items = from xe in xdoc.Element(ns_gesmes + "Envelope").Element(ns + "Cube").Element(ns + "Cube").Elements(ns + "Cube")
                        select new Rating
                        {
                            Currency = xe.Attribute("currency").Value,
                            Rate = xe.Attribute("rate").Value.Replace(".", ",")
                        };

            //т.к.ecb предоставляет данные относительно ЕВРО, пересчитаем курс на рубли
            var koef = from xe in xdoc.Element(ns_gesmes + "Envelope").Element(ns + "Cube").Element(ns + "Cube").Elements(ns + "Cube")
                       where xe.Attribute("currency").Value == "RUB"
                       select new Rating
                       {
                           Currency = xe.Attribute("currency").Value,
                           Rate = xe.Attribute("rate").Value.Replace(".", ",")
                       };
            foreach (var item in items)
            {
                double tmp = Convert.ToDouble(item.Rate) / Convert.ToDouble(koef.First().Rate);
                CurrencyRates.Add(item.Currency, Convert.ToDouble(koef.First().Rate) / (Convert.ToDouble(item.Rate)));
            }
            CurrencyRates.Remove("RUB");
            CurrencyRates.Add("EUR", Convert.ToDouble(koef.First().Rate));
        }

        public string Show()
        {
            string ans = "";
            foreach (var item in CurrencyRates)
            {
                ans += item.Key + " = " + item.Value.ToString() + "\n";
            }
            return ans;
        }
    }
}