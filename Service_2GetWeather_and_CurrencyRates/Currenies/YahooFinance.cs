using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Xml.Linq;

namespace Service_2Get_CurrencyRates
{
    internal class YahooFinance : CurrencyData, IWebPage
    {
        public string Address
        {
            get { return "http://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote?format=xml"; }
        }

        public bool InReading
        {
            get { return inReading; }

            set { inReading = value; }
        }

        private bool inReading;

        public string NameOfResource
        {
            get { return "YahooFinance"; }
        }

        public YahooFinance()
        {
            InReading = true;// При старте данные не получены
        }

        public void Read(object ABC)
        {
            InReading = true;
            try
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
                //add 2 abc
                Add_Currenies_to_Global_Dictionary((HashSet<string>)ABC);
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Message: {1}", NameOfResource, e.Message.ToString());//!!
            }
            finally
            {
                InReading = false;
            }
        }

        public string Show()
        {
            throw new NotImplementedException();
        }
    }
}