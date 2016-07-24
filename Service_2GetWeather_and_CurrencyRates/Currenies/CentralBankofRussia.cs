using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Service_2Get_CurrencyRates
{
    internal class CentralBankofRussia : IWebPage
    {
        public string Address
        {
            get { return "http://www.cbr.ru/scripts/XML_daily.asp"; }
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

                IEnumerable<XElement> elements = xdoc.Descendants("Valute");
                foreach (XElement item in elements)
                {
                    var tmp = new CurrencyRating(item.Element("CharCode").Value, (Convert.ToDouble(item.Element("Value").Value) / Convert.ToDouble(item.Element("Nominal").Value)).ToString());
                    ans.Add(tmp);
                }
            }
            catch (Exception e)
            {
                string[] str = { "CentralBankOfRussia.Read= Target site: " + e.TargetSite.ToString() + "    Message: " + e.Message + "    Source: " + e.Source };
                File.AppendAllLines("D:\\CurrencyInfoService\\" + "Error.txt", str);
            }
            return new DataCurrencySet(ans, "CentrlBankofRussia");
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