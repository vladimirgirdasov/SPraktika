using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace Service_2Get_Weather
{
    internal class YandexWeather : WeatherInfo_, IWebPage
    {
        public const string DefaultHref = @"https://export.yandex.ru/bar/reginfo.xml?region=11148";
        public string href;

        public YandexWeather()
        {
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

        private bool inReading;

        public string NameOfResource
        {
            get
            {
                return "Yandex.Weather";
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

        public void Read()
        {
            try
            {
                XDocument xdoc = XDocument.Load(new WebClient().OpenRead(href));
                IEnumerable<XElement> elements = xdoc.Descendants("day_part");

                City = xdoc.Descendants("weather").First().Descendants("day").First().Descendants("title").First().Value;

                var cur = elements.First();//Информация на текущее время суток (утро/день/вечер/ночь)
                TimeOfDay = cur.Attribute("type").Value;
                WindSpeed = ((IEnumerable<XElement>)cur.Descendants("wind_speed")).First().Value;
                WindDirection = ((IEnumerable<XElement>)cur.Descendants("wind_direction")).First().Value; ;
                dampness = ((IEnumerable<XElement>)cur.Descendants("dampness")).First().Value;
                Temperature = ((IEnumerable<XElement>)cur.Descendants("temperature")).First().Value;
                pressure = ((IEnumerable<XElement>)cur.Descendants("pressure")).First().Value;
                IEnumerable<XElement> element_tomorrow = xdoc.Descendants("tomorrow");
                TemperatureTomorrow = ((IEnumerable<XElement>)element_tomorrow.Descendants("temperature")).Last().Value;
            }
            catch (Exception e)
            {
            }
        }

        public string Show()
        {
            throw new NotImplementedException();
        }
    }
}