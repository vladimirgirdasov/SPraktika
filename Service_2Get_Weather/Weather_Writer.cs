using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace Service_2Get_Weather
{
    internal class Weather_Writer
    {
        public static void WeatherWrite(string way, YandexWeather yaWeather, Gismeteo gisWeather)
        {
            DateTime localDate = DateTime.Now;

            if (!File.Exists(way))//Если файла нету, создаем
            {
                XmlTextWriter textWritter = new XmlTextWriter(way, Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("head");
                textWritter.WriteEndElement();
                textWritter.Close();
            }

            XmlDocument document = new XmlDocument();
            document.Load(way);

            XmlNode element_update = document.CreateElement("update");
            XmlAttribute attribute_date = document.CreateAttribute("date"); // создаём атрибут
            attribute_date.Value = localDate.ToString(); // устанавливаем значение атрибута
            element_update.Attributes.Append(attribute_date);
            document.DocumentElement.AppendChild(element_update);

            WeatherInfoOut(document, element_update, yaWeather, yaWeather);
            WeatherInfoOut(document, element_update, gisWeather, gisWeather);

            document.Save(way);
        }

        public static void WeatherInfoOut(XmlDocument document, XmlNode element_update, WeatherInfo_ source, IWebPage page)
        {
            while (true)
            {
                Thread.Sleep(100);//от перегрузки потока
                if (page.InReading == false)
                {
                    XmlNode element_weather_data_block = document.CreateElement("weather_data");
                    element_update.AppendChild(element_weather_data_block); // указываем родителя

                    XmlAttribute attribute_resource = document.CreateAttribute("resource"); // создаём атрибут
                    attribute_resource.Value = page.NameOfResource; // устанавливаем значение атрибута
                    element_weather_data_block.Attributes.Append(attribute_resource); // добавляем атрибут
                    //Далее параметры погоды
                    if (source.TimeOfDay.Length == 0)
                        source.TimeOfDay = "NULL";
                    XmlNode element_TimeOfDay = document.CreateElement("TimeOfDay"); // даём имя
                    element_TimeOfDay.InnerText = source.TimeOfDay; // и значение
                    element_weather_data_block.AppendChild(element_TimeOfDay); // и указываем кому принадлежит

                    if (source.WindSpeed.Length == 0)
                        source.WindSpeed = "NULL";
                    XmlNode element_WindSpeed = document.CreateElement("WindSpeed");
                    element_WindSpeed.InnerText = source.WindSpeed;
                    element_weather_data_block.AppendChild(element_WindSpeed);

                    if (source.WindDirection.Length == 0)
                        source.WindDirection = "NULL";
                    XmlNode element_WindDirection = document.CreateElement("WindDirection");
                    element_WindDirection.InnerText = source.WindDirection;
                    element_weather_data_block.AppendChild(element_WindDirection);

                    if (source.dampness.Length == 0)
                        source.dampness = "NULL";
                    XmlNode element_dampness = document.CreateElement("dampness");
                    element_dampness.InnerText = source.dampness;
                    element_weather_data_block.AppendChild(element_dampness);

                    if (source.Temperature.Length == 0)
                        source.Temperature = "NULL";
                    XmlNode element_Temperature = document.CreateElement("Temperature");
                    element_Temperature.InnerText = source.Temperature;
                    element_weather_data_block.AppendChild(element_Temperature);

                    if (source.pressure.Length == 0)
                        source.pressure = "NULL";
                    XmlNode element_pressure = document.CreateElement("pressure");
                    element_pressure.InnerText = source.pressure;
                    element_weather_data_block.AppendChild(element_pressure);

                    if (source.TemperatureTomorrow.Length == 0)
                        source.TemperatureTomorrow = "NULL";
                    XmlNode element_TemperatureTomorrow = document.CreateElement("TemperatureTomorrow");
                    element_TemperatureTomorrow.InnerText = source.TemperatureTomorrow;
                    element_weather_data_block.AppendChild(element_TemperatureTomorrow);

                    if (source.cloudness.Length == 0)
                        source.cloudness = "NULL";
                    XmlNode element_cloudness = document.CreateElement("cloudness");
                    element_cloudness.InnerText = source.cloudness;
                    element_weather_data_block.AppendChild(element_cloudness);

                    break;
                }
            }
        }
    }
}