using System;
using System.Collections.Generic;
using System.IO;
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
            get { return address; }
        }

        public void Read()
        {
            XDocument xdoc = XDocument.Load(new WebClient().OpenRead(this.Address));

            IEnumerable<XElement> elements = xdoc.Descendants("Valute");
            foreach (XElement item in elements)
                CurrencyRates.Add(item.Element("CharCode").Value, Convert.ToDouble(item.Element("Value").Value) / Convert.ToDouble(item.Element("Nominal").Value));
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