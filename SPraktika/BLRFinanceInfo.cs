using AngleSharp.Dom;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SPraktika
{
    internal class BLRFinanceInfo : CurrencyData, IWebPage
    {
        public string Address
        {
            get
            { return "http://finance.blr.cc/kurs-valut/ru/"; }
        }

        public void Read(HashSet<string> ABC)
        {
        }

        public string Show()
        {
            throw new NotImplementedException();
        }
    }
}