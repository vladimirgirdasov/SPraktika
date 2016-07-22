using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace SPraktika
{
    internal class GUI
    {
        public void Fill_DataGrid_Currencies(DataGrid dg, List<CurrencyRating> data)
        {
            dg.ItemsSource = data.ToDictionary(d => d.cur, d => d.val);
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

        public void Show_GismeteoWeather(WeatherInfo_ wi, string city, Label City, Label lCloudness, Image iWeather, Label lTemperature, Label lWindSpeed,
            Label lWindDirection, Label lPressure, Label lDampness)
        {
            City.Content = city;
            lCloudness.Content = wi.cloudness;
            //image Add
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.UriSource = new Uri(wi.ImageOfCurrentWeather, UriKind.Absolute);
            bitmap.EndInit();
            iWeather.Source = bitmap;
            //
            lTemperature.Content = wi.Temperature;
            lWindSpeed.Content = wi.WindSpeed;
            lWindDirection.Content = wi.WindDirection;
            lPressure.Content = wi.pressure;
            lDampness.Content = wi.dampness;
        }

        public void Fill_Gismeteo_ComboBox_from(Dictionary<string, string> data, ComboBox cbGismeteo)
        {
            cbGismeteo.Items.Clear();
            foreach (var item in data.Reverse())//reverse по алфавиту
                cbGismeteo.Items.Add(item.Key);
        }

        public bool cbGismeteoRegion_InEditing;

        public GUI()
        {
            cbGismeteoRegion_InEditing = false;
        }

        public void Gismeteo_button_back__Turn(Button button, bool state)//true - on, false - off
        {
            button.IsEnabled = state;
        }

        public void Gismeteo_Change_CurrentDir_Label(Label label, string dir)
        {
            label.Content = dir;
        }
    }
}