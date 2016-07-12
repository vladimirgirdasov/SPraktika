using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Xml.Linq;

namespace SPraktika
{
    internal class CentralBankofRussia : CurrencyData, IWebPage
    {
        public string Address
        {
            get { return "http://www.cbr.ru/scripts/XML_daily.asp"; }
        }

        public void Read(HashSet<string> ABC)
        {
            XDocument xdoc = XDocument.Load(new WebClient().OpenRead(this.Address));

            IEnumerable<XElement> elements = xdoc.Descendants("Valute");
            foreach (XElement item in elements)
                CurrencyRates.Add(item.Element("CharCode").Value, Convert.ToDouble(item.Element("Value").Value) / Convert.ToDouble(item.Element("Nominal").Value));
            //add 2 abc
            foreach (var item in CurrencyRates)
                ABC.Add(item.Key);
        }

        public string Show()
        {
            string ans = "";
            foreach (var item in CurrencyRates)
            {
                ans += item.Key + " = " + item.Value.ToString() + "руб.\n";
            }
            return ans;
        }
    }
}