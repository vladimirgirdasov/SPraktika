using AngleSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace Service_2Get_CurrencyRates
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

        public async System.Threading.Tasks.Task<DataCurrencySet> ReadAsync()
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
                string[] str = { "BLRFinance.Read= Target site: " + e.TargetSite.ToString() + "    Message: " + e.Message + "    Source: " + e.Source };
                File.AppendAllLines("D:\\CurrencyInfoService\\" + "Error.txt", str);
            }
            finally
            {
                InReading = false;
            }
            return new DataCurrencySet(ans, "BLRFinance");
        }

        public void Read(object ABC = null)
        {
            throw new NotImplementedException();
        }

        public DataCurrencySet Read()
        {
            throw new NotImplementedException();
        }
    }
}