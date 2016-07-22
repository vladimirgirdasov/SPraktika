using AngleSharp;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace SPraktika
{
    internal class BLRFinanceInfo : IWebPage
    {
        public string Address
        {
            get { return "http://finance.blr.cc/kurs-valut/ru/"; }
        }

        private bool inReading;
        public bool InReading
        {
            get { return inReading; }

            set { inReading = value; }
        }

        public async System.Threading.Tasks.Task<List<CurrencyRating>> ReadAsync()
        {
            InReading = true;
            var ans = new List<CurrencyRating>();
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync(Address);
                var cellSelector_Currency = "tbody tr td:nth-child(1) b";//CSS selector кода валюты
                var cellSelector_Price = "tbody tr td:nth-child(2)";//цены
                var cells_currency = document.QuerySelectorAll(cellSelector_Currency);
                var cells_price = document.QuerySelectorAll(cellSelector_Price);

                var currencies = cells_currency.Select(m => m.TextContent);
                var prices = cells_price.Select(m => m.TextContent.Replace(".", ",").Substring(1));

                for (int i = 0; i < currencies.Count(); i++)
                {
                    ans.Add(new CurrencyRating(currencies.ElementAt(i), prices.ElementAt(i)));
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Exception в чтении из " + Address);
            }
            finally
            {
                InReading = false;
            }
            return ans;
        }

        public void Read(object ABC = null)
        {
            throw new NotImplementedException();
        }

        public List<CurrencyRating> Read()
        {
            throw new NotImplementedException();
        }
    }
}