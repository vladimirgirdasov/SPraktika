using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyServicesDataReader
{
    public class WeatherDataUnit
    {
        public string date { get; set; }//дата измерения
        public string time { get; set; }//время измерения

        public string city { get; set; }

        public string timeOfDay { get; set; }//время суток города
        public string windSpeed { get; set; }
        public string windDirection { get; set; }
        public string dampness { get; set; }//Влажность [%]
        public string temp { get; set; }
        public string pressure { get; set; }// Атмосферное давление [мм рт. стлб.]
        public string tempTomorrow { get; set; }
        public string cloudness { get; set; } //обачлность, текстовое описание (пассмурно, гроза) (гисметео)

        public string source { get; set; }//сайт источник

        public WeatherDataUnit(string date, string time, string city, string timeOfDay, string windSpeed
            , string windDirection, string dampness, string temperature, string pressure, string temperatureTomorrow
            , string cloudness, string source)
        {
            this.date = date;
            this.time = time;
            this.city = city;
            this.timeOfDay = timeOfDay;
            this.windSpeed = windSpeed;
            this.windDirection = windDirection;
            this.dampness = dampness;
            this.temp = temperature;
            this.pressure = pressure;
            this.tempTomorrow = temperatureTomorrow;
            this.cloudness = cloudness;
            this.source = source;
        }
    }
}