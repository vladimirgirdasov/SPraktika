﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPraktika
{
    internal class WeatherInfo_
    {
        public string TimeOfDay;//время суток города
        public string WindSpeed;
        public string WindDirection;
        public string dampness;//Влажность [%]
        public string Temperature;
        public string pressure;// Атмосферное давление [мм рт. стлб.]
        public string TemperatureTomorrow;
        public string ImageOfCurrentWeather; //адрес изображения текущей погоды\осадок
    }
}