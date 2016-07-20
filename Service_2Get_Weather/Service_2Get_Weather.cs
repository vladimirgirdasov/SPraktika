using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Service_2Get_Weather
{
    public partial class Service_2Get_Weather : ServiceBase
    {
        private static System.Timers.Timer timerWeather;
        private static int TimerInterval = 30 * 60 * 1000;//30 min

        private const string LogName = @"WeatherLog.xml";
        private static string LogDir = @"D:\\WeatherInfoService\\";
        private const string ConfigWay = @"D:\\WeatherInfoService\\config.conf";
        private const string ReadmeWay = @"D:\\WeatherInfoService\\ReadMe.txt";

        private static YandexWeather yaWeather;
        private static Gismeteo gisWeather;

        private static void ReadConfig()
        {
            if (!Directory.Exists(LogDir))
                Directory.CreateDirectory(LogDir);
            if (!File.Exists(ReadmeWay))
            {
                string[] lines = { @"Погода предоставляется сервисами Яндекс.Погода и gismeteo.ru",
                    @"В конфиг gismeteo писать html ссылку на gismeteo.Город",
                    @"В конфиг Яндекс.Погода пишется форма https://export.yandex.ru/bar/reginfo.xml?region={region}",
                    @"где регион - id нужного региона, который можно взять из https://pogoda.yandex.ru/static/cities.xml"
                    };
                File.WriteAllLines(ReadmeWay, lines);
            }
            if (!File.Exists(ConfigWay))
            {
                File.WriteAllText(ConfigWay, TimerInterval.ToString() + "|" + LogDir + "|" + YandexWeather.DefaultHref + "|" + Gismeteo.DefaultHref);
            }
            else
            {
                try
                {
                    var data = File.ReadAllText(ConfigWay).Split('|');
                    TimerInterval = Convert.ToInt32(data[0]);
                    LogDir = data[1];
                    yaWeather.href = data[2];
                    gisWeather.href = data[3];
                }
                catch (Exception e)
                {
                    TimerInterval = 30 * 60 * 1000;
                    LogDir = @"D:\\WeatherInfoService\\";
                    yaWeather.href = YandexWeather.DefaultHref;
                    gisWeather.href = Gismeteo.DefaultHref;

                    string[] str = { "Target site: " + e.TargetSite.ToString() + "    Message: " + e.Message + "    Source: " + e.Source };
                    File.AppendAllLines("D:\\WeatherInfoService\\" + "Error.txt", str);
                }
            }
            if (yaWeather.href == "")
                yaWeather.href = YandexWeather.DefaultHref;
            if (gisWeather.href == "")
                gisWeather.href = Gismeteo.DefaultHref;
        }

        public Service_2Get_Weather()
        {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
            this.ServiceName = "Service_2Get_Weather";
        }

        protected override void OnStart(string[] args)
        {
            yaWeather = new YandexWeather();
            gisWeather = new Gismeteo();
            ReadConfig();
            UpdateWeatherInfo();
            Weather_Writer.WeatherWrite(LogDir + LogName, yaWeather, gisWeather);
            SetTimer(TimerInterval);
        }

        protected override void OnStop()
        {
            timerWeather.Stop();
            timerWeather.Dispose();
        }

        private static void SetTimer(int intervalCurrency)
        {
            timerWeather = new System.Timers.Timer(intervalCurrency);
            timerWeather.Elapsed += TimerCurrencyElapsedAction;
            timerWeather.AutoReset = true;
            timerWeather.Enabled = true;
        }

        private static void TimerCurrencyElapsedAction(Object source, ElapsedEventArgs e)
        {
            yaWeather = new YandexWeather();
            gisWeather = new Gismeteo();
            ReadConfig();
            UpdateWeatherInfo();
            Weather_Writer.WeatherWrite(LogDir + LogName, yaWeather, gisWeather);
        }

        public static void UpdateWeatherInfo()
        {
            yaWeather.Read();
            gisWeather.InReading = true;
            Thread ReadGis = new Thread(gisWeather.Read);
            ReadGis.Start();
        }
    }
}