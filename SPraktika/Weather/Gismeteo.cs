using AngleSharp;
using AngleSharp.Dom.Html;
using System;
using System.Collections.Generic;
using System.IO;
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

        public string BackPageHref;

        public string CurrentHref;

        public string BackPagedRegionName;

        public const string ConfigDirectoryDefault = "Gismeteo.conf";

        public void SaveLastCity(string way = ConfigDirectoryDefault)
        {
            File.WriteAllText(way, CurrentHref + "|" + CitySelected);
        }

        public Gismeteo()
        {
            RegionHrefs = new Dictionary<string, string>();
            inReading = true;
            CitySelected = "";
            BackPageHref = "";
            CurrentHref = Address;
        }

        public Gismeteo(string WayConfig)
        {
            try
            {
                var data = File.ReadAllText(Gismeteo.ConfigDirectoryDefault).Split('|');

                RegionHrefs = new Dictionary<string, string>();
                inReading = true;
                CitySelected = data[1];
                BackPageHref = "";
                CurrentHref = data[0];
            }
            catch (Exception e)
            {
                MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Ошибка в чтении Gismeteo.conf");
            }
        }

        public bool IsInRootDir()
        {
            if (CurrentHref == Address)
                return true;
            else
                return false;
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
                //
                CurrentHref = (string)Addr;
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

        public async void ExtractBackPageHref()
        {
            if (IsInRootDir())
                MessageBox.Show("Error going back. It's already root dir", "Error");
            else
            {
                if (CurrentHref.Contains("daily"))//если в листе
                {
                    //вернемся на страницу с таблицами (не daily), затем оттуда тоже самое, что для табличной
                    InReading = true;
                    try
                    {
                        var config = Configuration.Default.WithDefaultLoader();
                        var document = await BrowsingContext.New(config).OpenAsync(CurrentHref);
                        var cell_Page_with_Tables_href = document.QuerySelector("[class='section higher'] [class='scity'] span a");
                        string BackPage_with_Tables_href = ((IHtmlAnchorElement)cell_Page_with_Tables_href).Href;
                        //Получена табличная страница
                        var document2 = await BrowsingContext.New(config).OpenAsync(BackPage_with_Tables_href);
                        var cell_prev_hrefs = document2.QuerySelectorAll("[class='section'] [class='h3'] a");
                        BackPageHref = ((IHtmlAnchorElement)cell_prev_hrefs.Last()).Href;
                        BackPagedRegionName = cell_prev_hrefs.Last().TextContent;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Ошибка в чтении адреса предыдущей страницы " + Address);
                    }
                    finally
                    {
                        InReading = false;
                    }
                }
                else//иначе - Промежуточные страницы выбора региона
                {
                    InReading = true;
                    try
                    {
                        var config = Configuration.Default.WithDefaultLoader();
                        var document = await BrowsingContext.New(config).OpenAsync(CurrentHref);
                        var cell_prev_hrefs = document.QuerySelectorAll("[class='section'] [class='h3'] a");
                        BackPageHref = ((IHtmlAnchorElement)cell_prev_hrefs.Last()).Href;
                        BackPagedRegionName = cell_prev_hrefs.Last().TextContent;
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Ошибка в чтении адреса предыдущей страницы " + Address);
                    }
                    finally
                    {
                        InReading = false;
                    }
                }
            }
        }
    }
}