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

        private const string LogPartName = @"WeatherLog__";
        private const string LogExtension = @".xml";
        private static string LogDir = @"D:\\WeatherInfoService\\";
        private const string ConfigWay = @"D:\\WeatherInfoService\\config.conf";
        private const string ReadmeWay = @"D:\\WeatherInfoService\\ReadMe.txt";

        //Файла старее параметра дней удаляются
        private static int SaveFile_UpTo_N_days = 5;

        private static YandexWeather yaWeather;
        private static Gismeteo gisWeather;

        private static void CreateReadMe()
        {
            if (!File.Exists(ReadmeWay))
            {
                string[] lines = { @"Погода предоставляется сервисами Яндекс.Погода и gismeteo.ru",
                    @"Параметры конфига:",
                    @"1) - Интервал обновления в миллисекундах",
                    @"2) - В конфиг Яндекс.Погода пишется форма https://export.yandex.ru/bar/reginfo.xml?region={region}",
                    @"где регион - id нужного региона, который можно взять из https://pogoda.yandex.ru/static/cities.xml",
                    @"3) - В конфиг gismeteo писать html ссылку на gismeteo.Город",
                    @"4) - Сервис хранит данные за последние (N) дней"
                    };
                File.WriteAllLines(ReadmeWay, lines);
            }
        }

        private static void ReadConfig()
        {
            if (!Directory.Exists(LogDir))
                Directory.CreateDirectory(LogDir);
            if (!File.Exists(ConfigWay))
            {
                File.WriteAllText(ConfigWay, TimerInterval.ToString() + "|" + LogDir + "|" + YandexWeather.DefaultHref + "|" + Gismeteo.DefaultHref + "|" + SaveFile_UpTo_N_days.ToString());
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
                    SaveFile_UpTo_N_days = int.Parse(data[4]);
                }
                catch (Exception e)
                {
                    TimerInterval = 30 * 60 * 1000;
                    LogDir = @"D:\\WeatherInfoService\\";
                    yaWeather.href = YandexWeather.DefaultHref;
                    gisWeather.href = Gismeteo.DefaultHref;
                    SaveFile_UpTo_N_days = 5;

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
            CreateReadMe();
            UpdateWeatherInfo();
            Weather_Writer.WeatherWrite(LogDir + LogPartName + DateTime.Today.ToString("dd'.'MM'.'yyyy") + LogExtension, yaWeather, gisWeather);
            Check_Old_Logs_To_Delete();
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
            Weather_Writer.WeatherWrite(LogDir + LogPartName + DateTime.Today.ToString("dd'.'MM'.'yyyy") + LogExtension, yaWeather, gisWeather);
            Check_Old_Logs_To_Delete();
        }

        public static void UpdateWeatherInfo()
        {
            yaWeather.Read();
            gisWeather.InReading = true;
            Thread ReadGis = new Thread(gisWeather.Read);
            ReadGis.Start();
        }

        private static void Check_Old_Logs_To_Delete()
        {
            if (SaveFile_UpTo_N_days < 1)
                return;
            //Сегодняшний день
            DateTime CurrentDay = DateTime.Today;
            //Найдем Все файлы логов
            string[] all_logs = Directory.GetFiles(LogDir, "WeatherLog__??.??.????.xml", SearchOption.TopDirectoryOnly);
            //Достанем из их заголовков строку с датой и попытаемся пропарсить на дату
            DateTime[] all_dates = new DateTime[all_logs.Count()];
            for (int i = 0; i < all_logs.Count(); i++)
            {
                int id = all_logs[i].IndexOf("WeatherLog__");
                string date_Line = all_logs[i].Substring(id + "WeatherLog__".Length, 10);
                //Если не парсится - ставим минимальную дату и удалим)))
                if (!DateTime.TryParseExact(date_Line, "dd'.'MM'.'yyyy", null, System.Globalization.DateTimeStyles.None, out all_dates[i]))
                    all_dates[i] = DateTime.MinValue;
            }
            //Удаляем старые файлы:
            //Если (Дата файла + SaveFile_UpTo_N_days) < CurrentDay => Удаление
            for (int i = 0; i < all_logs.Count(); i++)
            {
                if (all_dates[i].AddDays(SaveFile_UpTo_N_days) < CurrentDay)
                    File.Delete(all_logs[i]);
            }
        }
    }
}