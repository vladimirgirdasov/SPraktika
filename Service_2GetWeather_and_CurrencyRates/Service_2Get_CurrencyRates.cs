﻿using System;
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
using System.Xml;

namespace Service_2Get_CurrencyRates
{
    public partial class Service_2Get_CurrencyRates : ServiceBase
    {
        //Timer
        private static System.Timers.Timer timerCurrency;
        private static int TimerInterval = 10 * 60 * 1000;//10 min
        //Валюты
        private static DataCurrencySet cbrData = new DataCurrencySet();
        private static DataCurrencySet ecbData = new DataCurrencySet();
        private static DataCurrencySet yfData = new DataCurrencySet();
        private static DataCurrencySet blrData = new DataCurrencySet();
        private static DataCurrencySet avgData = new DataCurrencySet();//Среднее

        private static EuropeanCentralBank ecbReader = new EuropeanCentralBank();
        private static CentralBankofRussia cbrReader = new CentralBankofRussia();
        private static YahooFinance yfReader = new YahooFinance();
        private static BLRFinanceInfo blrReader = new BLRFinanceInfo();
        //Updating data
        public static bool update_done;

        public static async void UpdateCurrencyInfo()
        {
            blrData = await blrReader.ReadAsync();
            cbrData = cbrReader.Read();
            ecbData = ecbReader.Read();
            yfData = yfReader.Read();
            avgData = AverageCurrencyData.CalcAverageRates(blrData.data, cbrData.data, ecbData.data, yfData.data);
            update_done = true;
        }

        //Directories/Config
        private const string LogPartName = @"CurrenciesLog__";
        private const string LogExtension = @".xml";
        private static string LogDir = @"D:\\CurrencyInfoService\\";
        private const string ConfigWay = @"D:\\CurrencyInfoService\\config.conf";
        private const string ReadMeWay = @"D:\\CurrencyInfoService\\ReadMe.txt";

        private static int SaveFile_UpTo_N_days = 5;

        public static void Update_And_Write_Data()
        {
            update_done = false;
            new Thread(UpdateCurrencyInfo).Start();
            while (true)
            {
                if (update_done)
                {
                    CurrencyRates_Writer.CurrencyWrite(LogDir + LogPartName + DateTime.Today.ToString("d'.'MM'.'yyyy") + LogExtension, cbrData, blrData, ecbData, yfData, avgData);
                    break;
                }
                Thread.Sleep(250);
            }
        }

        private static void Read_or_Create_Config()
        {
            if (!Directory.Exists(LogDir))
                Directory.CreateDirectory(LogDir);
            if (!File.Exists(ConfigWay))
            {
                File.WriteAllText(ConfigWay, TimerInterval.ToString() + "|" + LogDir + "|" + SaveFile_UpTo_N_days.ToString());
            }
            else
            {
                try
                {
                    var data = File.ReadAllText(ConfigWay).Split('|');
                    TimerInterval = Convert.ToInt32(data[0]);
                    LogDir = data[1];
                    SaveFile_UpTo_N_days = int.Parse(data[2]);
                }
                catch (Exception e)
                {
                    TimerInterval = 10 * 60 * 1000;
                    LogDir = "D:\\CurrencyInfoService\\";
                    SaveFile_UpTo_N_days = 5;
                }
            }
        }

        private static void Check_Old_Logs_To_Delete()
        {
            if (SaveFile_UpTo_N_days < 1)
                return;
            //Сегодняшний день
            DateTime CurrentDay = DateTime.Today;
            //Найдем Все файлы логов
            string[] all_logs = Directory.GetFiles(LogDir, "CurrenciesLog__??.??.????.xml", SearchOption.TopDirectoryOnly);
            //Достанем из их заголовков строку с датой и попытаемся пропарсить на дату
            DateTime[] all_dates = new DateTime[all_logs.Count()];
            for (int i = 0; i < all_logs.Count(); i++)
            {
                int id = all_logs[i].IndexOf("CurrenciesLog__");
                string date_Line = all_logs[i].Substring(id + "CurrenciesLog__".Length, 10);
                //Если не парсится - ставим минимальную дату и удалим)))
                if (!DateTime.TryParseExact(date_Line, "d'.'MM'.'yyyy", null, System.Globalization.DateTimeStyles.None, out all_dates[i]))
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

        private static void Create_ReadMe()
        {
            string[] lines = { "Параметры конфига разделяется символом (|)",
                "1) - Интервал обновления данных в миллисекундах",
                "2) - Путь для сохранения логов",
                "3 - Сервис хранит данные за последние (N) дней",
                "Чтобы сервис не удалял старые логи, поставьте в третьем параметре число x<=(0)" };
            File.WriteAllLines(ReadMeWay, lines);
        }

        public Service_2Get_CurrencyRates()
        {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
            this.ServiceName = "Service_2Get_CurrencyRates";
        }

        protected override void OnStart(string[] args)
        {
            Read_or_Create_Config();
            Create_ReadMe();
            Update_And_Write_Data();
            Check_Old_Logs_To_Delete();
            SetTimer(TimerInterval);
        }

        protected override void OnStop()
        {
            timerCurrency.Stop();
            timerCurrency.Dispose();
        }

        private static void SetTimer(int intervalCurrency)
        {
            timerCurrency = new System.Timers.Timer(intervalCurrency);
            timerCurrency.Elapsed += TimerCurrencyElapsedAction;
            timerCurrency.AutoReset = true;
            timerCurrency.Enabled = true;
        }

        private static void TimerCurrencyElapsedAction(Object source, ElapsedEventArgs e)
        {
            Update_And_Write_Data();
            Check_Old_Logs_To_Delete();
        }
    }
}