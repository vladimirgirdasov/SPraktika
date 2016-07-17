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

        public Gismeteo()
        {
            RegionHrefs = new Dictionary<string, string>();
            inReading = true;
            CitySelected = "";
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

                var cell_cloudness = document.QuerySelectorAll("[class='section higher'] [class='cloudness'] dd table tbody tr td");
                cloudness = cell_cloudness.First().TextContent;
                var a = true;
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