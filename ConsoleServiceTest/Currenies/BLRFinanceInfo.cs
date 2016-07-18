using AngleSharp;
using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;

namespace ConsoleServiceTest
{
    internal class BLRFinanceInfo : CurrencyData, IWebPage
    {
        public string Address
        {
            get { return "http://finance.blr.cc/kurs-valut/ru/"; }
        }

        public bool InReading
        {
            get { return inReading; }

            set { inReading = value; }
        }

        public string NameOfResource
        {
            get { return "BLR_Finance"; }
        }

        public BLRFinanceInfo()
        {
            InReading = true;// При старте данные не получены
        }

        private bool inReading;

        public async void Read(object ABC)
        {
            InReading = true;
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
                    CurrencyRates.Add(currencies.ElementAt(i), Convert.ToDouble(prices.ElementAt(i)));
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