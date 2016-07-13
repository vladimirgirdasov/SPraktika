using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;

namespace SPraktika
{
    internal class EuropeanCentralBank : CurrencyData, IWebPage
    {
        private XNamespace ns_gesmes = "http://www.gesmes.org/xml/2002-08-01";//xmlns namespaces
        private XNamespace ns = "http://www.ecb.int/vocabulary/2002-08-01/eurofxref";

        public string Address
        {
            get { return "http://www.ecb.europa.eu/stats/eurofxref/eurofxref-daily.xml"; }
        }

        public bool InReading
        {
            get { return inReading; }

            set { inReading = value; }
        }

        private bool inReading;

        public EuropeanCentralBank()
        {
            InReading = true;// При старте данные не получены
        }

        public void Read(object ABC)
        {
            InReading = true;
            try
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
                    CurrencyRates.Add(item.Currency, Convert.ToDouble(koef.First().Rate) / (Convert.ToDouble(item.Rate)));
                }
                CurrencyRates.Remove("RUB");
                CurrencyRates.Add("EUR", Convert.ToDouble(koef.First().Rate));
                //add 2 abc
                Add_Currenies_to_Global_Dictionary((HashSet<string>)ABC);
            }
            catch (Exception e)
            {
                MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Exception в чтении из " + Address);
            }
            finally
            {
                InReading = false;
            }
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