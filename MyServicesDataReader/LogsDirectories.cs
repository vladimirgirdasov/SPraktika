using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MyServicesDataReader
{
    public static class LogsDirectories
    {
        public static string DirectoryCurrency = @"c:\";
        public static string DirectoryWeather = @"C:\";

        private static string ConfigName = @"AppConfig.conf";

        public static void Read_and_Check_Config()
        {
            if (File.Exists(ConfigName))
            {
                try
                {
                    var data = File.ReadAllLines(ConfigName);
                    DirectoryCurrency = data[0];
                    DirectoryWeather = data[1];

                    if (!IsCurrencyDirectory(DirectoryCurrency))
                        throw new Exception("В вашей последней директории валют не найдено логов. Директория сброшена");

                    if (!IsWeatherDirectory(DirectoryWeather))
                        throw new Exception("В вашей последней директории погоды не найдено логов. Директория сброшена");
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Ошибка чтения конфига директорий");
                    SetDefaultDirectories();
                }
            }
        }

        public static void Set_and_Write_Config(string dirCurr, string dirWeather)
        {
            if (dirCurr != null)
                DirectoryCurrency = dirCurr;
            if (dirWeather != null)
                DirectoryWeather = dirWeather;
            try
            {
                if (File.Exists(ConfigName))
                    File.Delete(ConfigName);

                string[] data = { DirectoryCurrency, DirectoryWeather };
                File.WriteAllLines(ConfigName, data);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка записи конфига директорий");
                SetDefaultDirectories();
            }
        }

        public static void SetDefaultDirectories()
        {
            DirectoryCurrency = @"";
            DirectoryWeather = @"";
        }

        public static bool IsCurrencyDirectory(string dirCurr)
        {
            string[] all_logs = Directory.GetFiles(dirCurr, "CurrenciesLog__??.??.????.xml", SearchOption.TopDirectoryOnly);
            if (all_logs.Count() > 0)
                return true;
            else
                return false;
        }

        public static bool IsWeatherDirectory(string dirWeather)
        {
            string[] all_logs = Directory.GetFiles(dirWeather, "WeatherLog__??.??.????.xml", SearchOption.TopDirectoryOnly);
            if (all_logs.Count() > 0)
                return true;
            else
                return false;
        }

        public static string[] Get_CurrencyLogs_Paths()
        {
            string[] all_logs = Directory.GetFiles(DirectoryCurrency, "CurrenciesLog__??.??.????.xml", SearchOption.TopDirectoryOnly);
            return all_logs;
        }

        public static string[] Get_WeatherLogs_Paths()
        {
            string[] all_logs = Directory.GetFiles(DirectoryWeather, "WeatherLog__??.??.????.xml", SearchOption.TopDirectoryOnly);
            return all_logs;
        }
    }
}