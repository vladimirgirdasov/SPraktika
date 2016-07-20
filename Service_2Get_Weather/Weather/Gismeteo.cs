using AngleSharp;
using AngleSharp.Dom.Html;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Service_2Get_Weather
{
    internal class Gismeteo : WeatherInfo_, IWebPage
    {
        public const string DefaultHref = @"https://www.gismeteo.ru/city/daily/4508/";
        public string href;

        public string City;

        public Gismeteo()
        {
            inReading = true;
            City = @"";
            href = "";
            TimeOfDay = "";//время суток города
            WindSpeed = "";
            WindDirection = "";
            dampness = "";//Влажность [%]
            Temperature = "";
            pressure = "";// Атмосферное давление [мм рт. стлб.]
            TemperatureTomorrow = "";
            ImageOfCurrentWeather = ""; //адрес изображения текущей погоды\осадок
            cloudness = ""; //обачлность, текстовое описание (пассмурно, гроза) (гисметео)
        }

        public bool InReading
        {
            get { return inReading; }

            set { inReading = value; }
        }

        public string NameOfResource
        {
            get
            {
                return "Gismeteo";
            }
        }

        public string Address
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        private bool inReading;

        public async void Read()
        {
            InReading = true;
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync(href);

                var main_dir = document.QuerySelector("[class='section higher']");

                City = main_dir.QuerySelector("[class='typeM']").TextContent;

                var cell_cloudness = main_dir.QuerySelector("[class='cloudness'] dd table tbody tr td");
                cloudness = cell_cloudness.TextContent;

                var cell_Temperature = main_dir.QuerySelector("[class='temp'] [class='value m_temp c']");
                Temperature = cell_Temperature.TextContent;

                var cell_wind_section = main_dir.QuerySelector("[class='wicon wind']");
                var cell_wind_direction = cell_wind_section.QuerySelector("dl");
                WindDirection = cell_wind_direction.GetAttribute("title");

                var cell_wind_speed = cell_wind_section.QuerySelector("dl [class='value m_wind ms']");
                WindSpeed = cell_wind_speed.TextContent;

                var cell_pressure = main_dir.QuerySelector("[class='wicon barp'] [class='value m_press torr']");
                pressure = cell_pressure.TextContent;

                var cell_dampness = main_dir.QuerySelector("[class='wicon hum']");
                dampness = cell_dampness.TextContent;
                dampness = dampness.Substring(0, dampness.IndexOf('%') + 1);
            }
            catch (Exception e)
            {
                string[] str = { "Gismeteo.Read= Target site: " + e.TargetSite.ToString() + "    Message: " + e.Message + "    Source: " + e.Source };
                File.AppendAllLines("D:\\WeatherInfoService\\" + "Error.txt", str);
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