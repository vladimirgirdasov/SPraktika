using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Xml.Linq;

namespace MyServicesDataReader
{
    public static class WeatherLogsReader
    {
        public static List<WeatherDataUnit> Read(string[] log_paths)
        {
            List<WeatherDataUnit> ans = new List<WeatherDataUnit>();

            //Проход по логам
            foreach (var path in log_paths)
            {
                //Чтение лога path
                try
                {
                    XDocument xdoc = XDocument.Load(path);
                    IEnumerable<XElement> updates = xdoc.Descendants("update");

                    //Проход по апдейтам
                    foreach (XElement update in updates)
                    {
                        //Дата апдейта
                        var datetime = update.Attribute("date").Value.Split(' ');
                        var cur_date = datetime[0];
                        var cur_time = datetime[1];

                        //Проход по ресурсам
                        IEnumerable<XElement> resources = update.Descendants("weather_data");
                        foreach (var resource in resources)
                        {
                            var cur_resource = resource.Attribute("resource").Value;
                            var cur_city = resource.Attribute("city").Value;

                            var cur_timeOfDay = resource.Element("TimeOfDay").Value;
                            var cur_windSpeed = resource.Element("WindSpeed").Value;
                            var cur_WindDirection = resource.Element("WindDirection").Value;
                            var cur_dampness = resource.Element("dampness").Value;
                            var cur_Temperature = resource.Element("Temperature").Value;
                            var cur_pressure = resource.Element("pressure").Value;
                            var cur_TemperatureTomorrow = resource.Element("TemperatureTomorrow").Value;
                            var cur_cloudness = resource.Element("cloudness").Value;

                            //Добавим еденицы измерения где нет
                            if (!cur_windSpeed.Contains("м/с"))
                                cur_windSpeed += "м/с";
                            if (!cur_dampness.Contains("%"))
                                cur_dampness += "%";
                            if (!cur_Temperature.Contains("°C"))
                                cur_Temperature += "°C";
                            if (!cur_pressure.Contains("мм рт. ст."))
                                cur_pressure += "мм рт. ст.";

                            //null на прочерк
                            if (cur_cloudness == "NULL")
                                cur_cloudness = "-";
                            if (cur_TemperatureTomorrow == "NULL")
                                cur_TemperatureTomorrow = "-";
                            if (cur_timeOfDay == "NULL")
                                cur_timeOfDay = "-";

                            ans.Add(new WeatherDataUnit(cur_date, cur_time, cur_city, cur_timeOfDay, cur_windSpeed,
                                cur_WindDirection, cur_dampness, cur_Temperature, cur_pressure, cur_TemperatureTomorrow,
                                cur_cloudness, cur_resource));
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(path + "  " + e.Message, "Ошибка чтения лога");
                }
            }
            return ans;
        }
    }
}