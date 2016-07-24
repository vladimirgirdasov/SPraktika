using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Service_2Get_CurrencyRates
{
    internal class EuropeanCentralBank : IWebPage
    {
        private XNamespace ns_gesmes = "http://www.gesmes.org/xml/2002-08-01";//xmlns namespaces
        private XNamespace ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";
        public string Address
        {
            get { return "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"; }
        }

        private bool inReading;
        public bool InReading
        {
            get { return inReading; }

            set { inReading = value; }
        }

        public DataCurrencySet Read()
        {
            var ans = new List<CurrencyRating>();
            try
            {
                XDocument xdoc = XDocument.Load(new WebClient().OpenRead(this.Address));

                //т.к.ecb предоставляет данные относительно ЕВРО, надо будет пересчитать курс на рубли
                var koef = from xe in xdoc.Element(ns_gesmes + "Envelope").Element(ns + "Cube").Element(ns + "Cube").Elements(ns + "Cube")
                           where xe.Attribute("currency").Value == "RUB"
                           select xe.Attribute("rate").Value.Replace(".", ",");

                //Соберём и перекоментируем валюты. Валюту RUB игнорируем. 1рубль=1рубль
                var items = from xe in xdoc.Element(ns_gesmes + "Envelope").Element(ns + "Cube").Element(ns + "Cube").Elements(ns + "Cube")
                            where xe.Attribute("currency").Value != "RUB"
                            select new CurrencyRating
                            {
                                cur = xe.Attribute("currency").Value,
                                val = (Convert.ToDouble(koef.First()) / Convert.ToDouble(xe.Attribute("rate").Value.Replace(".", ","))).ToString()
                            };

                foreach (var item in items)
                {
                    ans.Add(item);
                }

                //Добавим EUR, им является коэффициент
                ans.Add(new CurrencyRating("EUR", koef.First()));
            }
            catch (Exception e)
            {
                string[] str = { "EuropeanCentralBank= Target site: " + e.TargetSite.ToString() + "    Message: " + e.Message + "    Source: " + e.Source };
                File.AppendAllLines("D:\\CurrencyInfoService\\" + "Error.txt", str);
            }
            return new DataCurrencySet(ans, "EuropeanCentralBank");
        }

        public void Read(object ABC = null)
        {
            throw new NotImplementedException();
        }

        public Task<DataCurrencySet> ReadAsync()
        {
            throw new NotImplementedException();
        }
    }
}