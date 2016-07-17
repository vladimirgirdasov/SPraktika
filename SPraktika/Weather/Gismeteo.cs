using AngleSharp;
using AngleSharp.Dom.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace SPraktika
{
    internal class Gismeteo : WeatherInfo_, IWebPage
    {
        public string Address
        {
            get { return "https://www.gismeteo.ru/citysearch/"; }
        }

        public string CitySelected;

        public Dictionary<string, string> RegionHrefs;//ключ-Регион, значение- ссылка после клика на регион

        public string prevPageHref;

        public Gismeteo()
        {
            RegionHrefs = new Dictionary<string, string>();
            inReading = true;
            CitySelected = "";
            prevPageHref = "";
        }

        public bool InReading
        {
            get { return inReading; }

            set { inReading = value; }
        }

        private bool inReading;

        public async void Read(object Addr = null)
        {
            InReading = true;
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync((string)Addr);

                var main_dir = document.QuerySelector("[class='section higher']");

                var cell_cloudness = main_dir.QuerySelector("[class='cloudness'] dd table tbody tr td");
                cloudness = cell_cloudness.TextContent;

                var cell_image = main_dir.QuerySelector("[class='cloudness'] [class='png']");
                //example of image attribute value:
                //background-image: url(//st5.gismeteo.ru/static/images/icons/new/d.sun.c3.png)
                ImageOfCurrentWeather = cell_image.GetAttribute("style").Substring(24);
                ImageOfCurrentWeather = "https://" + ImageOfCurrentWeather.Substring(0, ImageOfCurrentWeather.Length - 1);

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
                MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Exception в чтении из " + Address);
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

        public async void ReadRegions_from(object Addr)
        {
            InReading = true;
            try
            {
                var config = Configuration.Default.WithDefaultLoader();
                var document = await BrowsingContext.New(config).OpenAsync((string)Addr);

                var cells_regions = document.QuerySelectorAll("[class='group first'] li a");
                var cells_regions2 = document.QuerySelectorAll("[class='group'] li a");

                var regions = cells_regions.Select(m => m.TextContent).ToList();
                var links = cells_regions.Select(m => ((IHtmlAnchorElement)m).Href).ToList();

                if (cells_regions2.Count() != 0)
                {
                    regions.AddRange(cells_regions2.Select(m => m.TextContent).ToList());
                    links.AddRange(cells_regions2.Select(m => ((IHtmlAnchorElement)m).Href).ToList());
                }
                //Add 2 map [region_name : href]
                RegionHrefs = new Dictionary<string, string>();
                for (int i = regions.Count() - 1; i >= 0; i--)
                {
                    if (RegionHrefs.ContainsKey(regions[i]) == false)//Если 2 города с одним названием, то верхний - аэропорт
                        RegionHrefs.Add(regions[i], links[i]);
                    else
                        RegionHrefs.Add(regions[i] + " (аэропорт)", links[i]);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Ошибка в чтении регионов " + Address);
            }
            finally
            {
                InReading = false;
            }
        }
    }
}