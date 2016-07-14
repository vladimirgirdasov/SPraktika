using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace SPraktika
{
    class YandexWeather : IWebPage
    {
        public WeatherInfo_ weatherInfo;

        public YandexWeather()
        {
            weatherInfo = new WeatherInfo_();
        }

        public string Address
        {
            get
            {
                return "https://export.yandex.ru/bar/reginfo.xml?region=";//В конце добавить YandexCities.city_id
            }
        }

        public bool InReading
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public void Read(object city_id)//city_id - YandexCities.city_id (string)
        {
            weatherInfo = new WeatherInfo_();
            try
            {
                XDocument xdoc = XDocument.Load(new WebClient().OpenRead(this.Address + city_id));
                IEnumerable<XElement> elements = xdoc.Descendants("day_part");

                var cur = elements.First();//Информация на текущее время суток (утро/день/вечер/ночь)
                weatherInfo.TimeOfDay = cur.Attribute("type").Value;
                weatherInfo.ImageOfCurrentWeather = ((IEnumerable<XElement>)cur.Descendants("image-v3")).First().Value;
                weatherInfo.WindSpeed = ((IEnumerable<XElement>)cur.Descendants("wind_speed")).First().Value;
                weatherInfo.WindDirection = ((IEnumerable<XElement>)cur.Descendants("wind_direction")).First().Value; ;
                weatherInfo.dampness = ((IEnumerable<XElement>)cur.Descendants("dampness")).First().Value;
                weatherInfo.Temperature = ((IEnumerable<XElement>)cur.Descendants("temperature")).First().Value;
                weatherInfo.pressure = ((IEnumerable<XElement>)cur.Descendants("pressure")).First().Value;
                IEnumerable<XElement> element_tomorrow = xdoc.Descendants("tomorrow");
                weatherInfo.TemperatureTomorrow = ((IEnumerable<XElement>)element_tomorrow.Descendants("temperature")).Last().Value;
            }
            catch (Exception e)
            {
                MessageBox.Show("Target site: " + e.TargetSite.ToString() + "\nMessage: " + e.Message + "\nSource: " + e.Source, "Exception в чтении из " + Address);
            }
        }

        public string Show()
        {
            throw new NotImplementedException();
        }
    }
}