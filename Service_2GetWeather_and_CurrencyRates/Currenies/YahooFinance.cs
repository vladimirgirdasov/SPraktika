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
    internal class YahooFinance : IWebPage
    {
        public string Address
        {
            get { return "http://finance.yahoo.com/webservice/v1/symbols/allcurrencies/quote?format=xml"; }
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
                    ans.Add(new CurrencyRating(symbol, price.ToString()));
                }

                //т.к.yahoo предоставляет данные относительно USD, пересчитаем курс на рубли
                var koef = ans.Where(x => x.cur == "RUB").ToList().First().val;
                //тут удялаем рубли, т.к. 1Руб=1Руб
                int id = ans.FindIndex(x => x.cur == "RUB");
                ans.RemoveAt(id);
                //Конвертация USD2RUB
                for (int i = 0; i < ans.Count(); i++)
                {
                    ans[i].val = (Convert.ToDouble(koef) / Convert.ToDouble(ans[i].val)).ToString();
                }
            }
            catch (Exception e)
            {
                string[] str = { "YahooFinance.Read= Target site: " + e.TargetSite.ToString() + "    Message: " + e.Message + "    Source: " + e.Source };
                File.AppendAllLines("D:\\CurrencyInfoService\\" + "Error.txt", str);
            }
            return new DataCurrencySet(ans, "YahooFinance");
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