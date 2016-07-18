using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Windows;
using System.Xml.Linq;

namespace ConsoleServiceTest
{
    internal class CentralBankofRussia : CurrencyData, IWebPage
    {
        public string Address
        {
            get { return "http://www.cbr.ru/scripts/XML_daily.asp"; }
        }

        public bool InReading
        {
            get { return inReading; }

            set { inReading = value; }
        }

        public string NameOfResource
        {
            get { return "CentralBankOfRussia"; }
        }

        private bool inReading;

        public CentralBankofRussia()
        {
            InReading = true;// При старте данные не получены
        }

        public void Read(object ABC)
        {
            InReading = true;
            try
            {
                XDocument xdoc = XDocument.Load(new WebClient().OpenRead(this.Address));

                IEnumerable<XElement> elements = xdoc.Descendants("Valute");
                foreach (XElement item in elements)
                    CurrencyRates.Add(item.Element("CharCode").Value, Convert.ToDouble(item.Element("Value").Value) / Convert.ToDouble(item.Element("Nominal").Value));
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