using AngleSharp;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace SPraktika
{
    internal class BLRFinanceInfo : CurrencyData, IWebPage
    {
        public string Address
        {
            get { return "http://finance.blr.cc/kurs-valut/ru/"; }
        }

        public async void Read(HashSet<string> ABC)
        {
            try
            {
                //var parser = new HtmlParser();
                //var document = parser.Parse(Address);
                var config = Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync(Address);
                //CSS selector кода валюты и цены
                var cellSelector_Currency = "tbody tr td:nth-child(1) b";
                var cellSelector_Price = "tbody tr td:nth-child(2)";
                var cells_currency = document.QuerySelectorAll(cellSelector_Currency);
                var cells_price = document.QuerySelectorAll(cellSelector_Price);

                var currencies = cells_currency.Select(m => m.TextContent);
                var prices = cells_price.Select(m => m.TextContent.Replace(".", ",").Substring(1));

                for (int i = 0; i < currencies.Count(); i++)
                    CurrencyRates.Add(currencies.ElementAt(i), Convert.ToDouble(prices.ElementAt(i)));
                //add 2 abc
                Add_Currenies_to_Global_Dictionary(ABC);
            }
            catch (Exception e)
            {
                MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Exception в чтении из " + Address);
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