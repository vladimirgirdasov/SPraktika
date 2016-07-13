using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SPraktika
{
    internal class GUI
    {
        public void FillDataGrid_2_SingleResource(DataGrid dg, CurrencyData source, IWebPage page)
        {
            while (true)
            {
                Thread.Sleep(250);//от перегрузки потока
                if (page.InReading == false)
                {
                    dg.ItemsSource = source.CurrencyRates;
                    break;
                }
            }
        }

        public void FillDataGrid_AverageValues(DataGrid dg, CurrencyData source, EuropeanCentralBank ecb, BLRFinanceInfo blr, CentralBankofRussia cbr, YahooFinance yf)
        {
            //source = new CurrencyData();
            while (true)
            {
                Thread.Sleep(250);//от перегрузки потока

                if (!ecb.InReading && !blr.InReading && !cbr.InReading && !yf.InReading)
                {
                    source.CalcAverageCurrencyRate(ecb, blr, cbr, yf);
                    dg.ItemsSource = source.CurrencyRates;
                    break;
                }
            }
        }
    }
}