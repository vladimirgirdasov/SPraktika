using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace ConsoleServiceTest
{
    internal class Program
    {
        private static System.Timers.Timer timerCurrency;
        private static EuropeanCentralBank ecb = new EuropeanCentralBank();
        private static CentralBankofRussia cbr = new CentralBankofRussia();
        private static YahooFinance yf = new YahooFinance();
        private static BLRFinanceInfo blr = new BLRFinanceInfo();
        private static CurrencyData AverageData = new CurrencyData();

        private const string WayCurrencies = "CurrenciesLog.xml";

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

        public static void CurrencyInfoOut(CurrencyData source, IWebPage page)
        {
            while (true)
            {
                Thread.Sleep(150);//от перегрузки потока
                if (page.InReading == false)
                {
                    foreach (var item in source.CurrencyRates)
                    {
                        Console.WriteLine("{0} = {1}", item.Key, item.Value);
                    }

                    break;
                }
            }
        }

        public static void CurrencyInfoOut(string way, CurrencyData source, IWebPage page)
        {
            while (true)
            {
                Thread.Sleep(150);//от перегрузки потока
                if (page.InReading == false)
                {
                    foreach (var item in source.CurrencyRates)
                    {
                        Console.WriteLine("{0} = {1}", item.Key, item.Value);
                    }

                    break;
                }
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);

            SetTimer(10000);

            Console.ReadLine();
            timerCurrency.Stop();
            timerCurrency.Dispose();

            Console.WriteLine("Terminating the application...");
            Console.ReadLine();
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
            Console.WriteLine("The Elapsed event was raised at {0:dd-MM-yyyy, HH:mm:ss}",
                              e.SignalTime);
            UpdateCurrencyInfo();
            CurrencyInfoOut("1", ecb, ecb);
        }
    }
}