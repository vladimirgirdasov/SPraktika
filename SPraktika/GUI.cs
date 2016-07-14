using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

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

        public void Show_YandexWeather(WeatherInfo_ wi, string city, Label City, Label TimeOfDay, Image iWeather, Label lTemperature, Label lWindSpeed,
            Label lWindDirection, Label lPressure, Label lDampness, Label lTemperatureTomorrow)// wi - YandexWeather.weatherInfo
        {
            City.Content = city;
            TimeOfDay.Content = wi.TimeOfDay;
            //image Add
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(wi.ImageOfCurrentWeather, UriKind.Absolute);
            bitmap.EndInit();
            iWeather.Source = bitmap;
            //
            lTemperature.Content = wi.Temperature;
            lWindSpeed.Content = wi.WindSpeed + " м/с";
            lWindDirection.Content = wi.WindDirection;
            lPressure.Content = wi.pressure + " мм рт. ст.";
            lDampness.Content = wi.dampness + "%";
            lTemperatureTomorrow.Content = wi.TemperatureTomorrow;
        }
    }
}