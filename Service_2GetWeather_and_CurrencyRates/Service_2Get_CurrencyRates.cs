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
using System.Xml;

namespace Service_2Get_CurrencyRates
{
    public partial class Service_2Get_CurrencyRates : ServiceBase
    {
        private static System.Timers.Timer timerCurrency;

        private static EuropeanCentralBank ecb = new EuropeanCentralBank();
        private static CentralBankofRussia cbr = new CentralBankofRussia();
        private static YahooFinance yf = new YahooFinance();
        private static BLRFinanceInfo blr = new BLRFinanceInfo();
        private static CurrencyData AverageData = new CurrencyData();

        private const string LogName = @"CurrenciesLog.xml";
        private static string LogDir = @"D:\\CurrencyInfoService\\";
        private const string ConfigWay = @"D:\\CurrencyInfoService\\config.conf";

        private static int TimerInterval = 10 * 60 * 1000;//10 min

        private static void ReadConfig()
        {
            if (!Directory.Exists(LogDir))
                Directory.CreateDirectory(LogDir);
            if (!File.Exists(ConfigWay))
            {
                File.WriteAllText(ConfigWay, TimerInterval.ToString() + "|" + LogDir);
            }
            else
            {
                try
                {
                    var data = File.ReadAllText(ConfigWay).Split('|');
                    TimerInterval = Convert.ToInt32(data[0]);
                    LogDir = data[1];
                }
                catch (Exception e)
                {
                    TimerInterval = 10 * 60 * 1000;
                    LogDir = "D:\\CurrencyInfoService\\";
                }
            }
        }

        public static void UpdateCurrencyInfo()
        {
            ecb = new EuropeanCentralBank();
            cbr = new CentralBankofRussia();
            yf = new YahooFinance();
            blr = new BLRFinanceInfo();
            AverageData = new CurrencyData();
            //
            Thread ReadBlr = new Thread(blr.Read);
            ReadBlr.Start(AverageData.abc);
            blr.InReading = true;
            Thread ReadECB = new Thread(ecb.Read);
            ReadECB.Start(AverageData.abc);
            ecb.InReading = true;
            Thread ReadCBR = new Thread(cbr.Read);
            ReadCBR.Start(AverageData.abc);
            cbr.InReading = true;
            Thread ReadYf = new Thread(yf.Read);
            ReadYf.Start(AverageData.abc);
            yf.InReading = true;
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
            ReadConfig();
            UpdateCurrencyInfo();
            CurrencyRates_Writer.CurrencyWrite(LogDir + LogName, ecb, blr, cbr, yf);
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
            UpdateCurrencyInfo();
            CurrencyRates_Writer.CurrencyWrite(LogDir + LogName, ecb, blr, cbr, yf);
        }
    }
}