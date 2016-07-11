using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Xml.Linq;

namespace SPraktika
{
    internal class YahooFinance : CurrencyData, IXmlPage
    {
        private string address = "http://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote?format=xml";//xml data page URL

        public string Address
        {
            get { return address; }
        }

        public YahooFinance()
        {
            CurrencyRates = new Dictionary<string, double>();
        }

        public void Read()
        {
            XDocument xdoc = XDocument.Load(new WebClient().OpenRead(this.Address));

            IEnumerable<XElement> elements = xdoc.Descendants("resource");
            foreach (XElement item in elements)
            {
                var sym = from xe in item.Elements("field")
                          where xe.Attribute("name").Value == "symbol"
                          select xe.Value;
                var prc = from xe in item.Elements("field")
                          where xe.Attribute("name").Value == "price"
                          select xe.Value;
                var symbol = sym.First().Substring(0, 3);
                var price = Convert.ToDouble(prc.First().Replace(".", ","));
                CurrencyRates.Add(symbol, price);
            }

            //т.к.yahoo предоставляет данные относительно USD, пересчитаем курс на рубли
            var koef = CurrencyRates["RUB"];
            CurrencyRates.Remove("RUB");
            var tmp = new string[CurrencyRates.Count];
            CurrencyRates.Keys.CopyTo(tmp, 0);
            foreach (var item in tmp)
                CurrencyRates[item] = koef / CurrencyRates[item];
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