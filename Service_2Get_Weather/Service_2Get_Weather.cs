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
using System.Windows;

namespace Service_2Get_Weather
{
    public partial class Service_2Get_Weather : ServiceBase
    {
        private static System.Timers.Timer timerWeather;
        private static int TimerInterval;

        private const string LogPartName = @"\\WeatherLog__";
        private const string LogExtension = @".xml";
        private static string LogDir = @"";
        private const string ReadmeName = @"\\ReadMe.txt";

        //Файла старее параметра дней удаляются
        private static int SaveFile_UpTo_N_days;

        private static YandexWeather yaWeather;
        private static Gismeteo gisWeather;

        private static void CreateReadMe()
        {
            if (!File.Exists(LogDir + ReadmeName))
            {
                string[] lines = { @"Погода предоставляется сервисами Яндекс.Погода и gismeteo.ru",
                    @"Входные параметры:",
                    @"1) - Интервал обновления в миллисекундах",
                    @"2) - Путь сохранения логов",
                    @"3) - В конфиг Яндекс.Погода пишется форма https://export.yandex.ru/bar/reginfo.xml?region={region}",
                    @"где регион - id нужного региона, который можно взять из https://pogoda.yandex.ru/static/cities.xml",
                    @"4) - В конфиг gismeteo писать html ссылку на gismeteo.Город",
                    @"5) - Сервис хранит данные за последние (N) дней"
                    };
                File.WriteAllLines(LogDir + ReadmeName, lines);
            }
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

            bool args_ok = false;

            if (args == null)
                args_ok = false;
            else
            if (args.Count() != 5)
                args_ok = false;
            else
            if ((int.TryParse(args[0], out TimerInterval) && Directory.Exists(args[1]) && int.TryParse(args[4], out SaveFile_UpTo_N_days)) == false)
                args_ok = false;
            else
            {
                LogDir = args[1];
                yaWeather.href = args[2];
                gisWeather.href = args[3];
                args_ok = true;
            }

            if (args_ok == false)
            {
                EventLog.WriteEntry("Служба остановлена. В обозревателе служб задайте входные параметры через пробел. " +
                    @"1) - Интервал обновления в миллисекундах " +
                    @"2) - Путь сохранения логов " +
                    @"3) - В конфиг Яндекс.Погода пишется форма https://export.yandex.ru/bar/reginfo.xml?region={region} " +
                    @"где регион - id нужного региона, который можно взять из https://pogoda.yandex.ru/static/cities.xml " +
                    @"4) - В конфиг gismeteo писать html ссылку на gismeteo.Город " +
                    @"5) - Сервис хранит данные за последние (N) дней");
                this.Stop();
            }

            EventLog.WriteEntry("Ок, интервал обновления=" + TimerInterval.ToString() + " Путь сохранения=" + LogDir + " Период хранения=" + SaveFile_UpTo_N_days);

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