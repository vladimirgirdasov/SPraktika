using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace SPraktika
{
    internal class CentralBankofRussia : CurrencyData, IXmlPage
    {
        private string address = "http://www.cbr.ru/scripts/XML_daily.asp";//xml data page URL

        public CentralBankofRussia()
        {
            CurrencyRates = new System.Collections.Generic.Dictionary<string, double>();
        }

        public string Address
        {
            get
            {
                return address;
            }
        }

        public void Read()
        {
            WebClient client = new WebClient();
            Stream stream = client.OpenRead(this.Address);
            XDocument xdoc = XDocument.Load(stream);

            IEnumerable<XElement> elements = xdoc.Descendants("Valute");
            foreach (XElement item in elements)
            {
                CurrencyRates.Add(item.Element("CharCode").Value, Convert.ToDouble(item.Element("Value").Value));
            }
        }

        public string Show()
        {
            throw new NotImplementedException();
        }
    }
}