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

namespace Service_2GetWeather_and_CurrencyRates
{
    public partial class Service_2GetWeather_and_CurrencyRates : ServiceBase
    {
        private static System.Timers.Timer timerCurrency;

        private static EuropeanCentralBank ecb = new EuropeanCentralBank();
        private static CentralBankofRussia cbr = new CentralBankofRussia();
        private static YahooFinance yf = new YahooFinance();
        private static BLRFinanceInfo blr = new BLRFinanceInfo();
        private static CurrencyData AverageData = new CurrencyData();

        private const string WayCurrencies = "D:\\CurrenciesLog.xml";

        public static void UpdateCurrencyInfo()
        {
            ecb = new EuropeanCentralBank();
            cbr = new CentralBankofRussia();
            yf = new YahooFinance();
            blr = new BLRFinanceInfo();
            AverageData = new CurrencyData();
            // Т.к. при доступе к веб ресурсу возможны задержки И
            //Т.к. AngleSharp работает асинхронно, переключясь на этот процесс, вывод начинался раньше чем AS завершал работу
            // Вытесним его на параллельный поток, и вывод по его готовности
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

            Console.WriteLine("Updating Done");
        }

        public Service_2GetWeather_and_CurrencyRates()
        {
            InitializeComponent();
            CanStop = true;
            CanPauseAndContinue = true;
            AutoLog = true;
            this.ServiceName = "Service_2GetWeather_and_CurrencyRates";
        }

        protected override void OnStart(string[] args)
        {
            UpdateCurrencyInfo();
            CurrencyRates_Writer.CurrencyWrite(WayCurrencies, ecb, blr, cbr, yf);

            SetTimer(10 * 1000);
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
            Console.WriteLine("The Elapsed event was raised at {0:dd-MM-yyyy, HH:mm:ss}", e.SignalTime);
            UpdateCurrencyInfo();
            CurrencyRates_Writer.CurrencyWrite(WayCurrencies, ecb, blr, cbr, yf);
        }
    }
}