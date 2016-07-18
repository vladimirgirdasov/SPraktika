using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Xml;

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

        public static void CurrencyWrite(string way = WayCurrencies)
        {
            DateTime localDate = DateTime.Now;

            if (!File.Exists(way))//Если файла нету, создаем
            {
                XmlTextWriter textWritter = new XmlTextWriter(way, Encoding.UTF8);
                textWritter.WriteStartDocument();
                textWritter.WriteStartElement("head");
                textWritter.WriteEndElement();
                textWritter.Close();
            }

            XmlDocument document = new XmlDocument();
            document.Load(way);

            XmlNode element_update = document.CreateElement("update");
            XmlAttribute attribute_date = document.CreateAttribute("date"); // создаём атрибут
            attribute_date.Value = localDate.ToString(); // устанавливаем значение атрибута
            element_update.Attributes.Append(attribute_date);
            document.DocumentElement.AppendChild(element_update);

            CurrencyInfoOut(document, element_update, ecb, ecb);
            CurrencyInfoOut(document, element_update, blr, blr);
            CurrencyInfoOut(document, element_update, cbr, cbr);
            CurrencyInfoOut(document, element_update, yf, yf);

            document.Save(way);
            Console.WriteLine("Writing done");
        }

        public static void CurrencyInfoOut(XmlDocument document, XmlNode element_update, CurrencyData source, IWebPage page)
        {
            while (true)
            {
                Thread.Sleep(100);//от перегрузки потока
                if (page.InReading == false)
                {
                    XmlNode element_rates_block = document.CreateElement("currency_data");
                    element_update.AppendChild(element_rates_block); // указываем родителя

                    XmlAttribute attribute_resource = document.CreateAttribute("resource"); // создаём атрибут
                    attribute_resource.Value = page.NameOfResource; // устанавливаем значение атрибута
                    element_rates_block.Attributes.Append(attribute_resource); // добавляем атрибут

                    foreach (var item in source.CurrencyRates)
                    {
                        XmlNode element_currency = document.CreateElement("currency"); // даём имя
                        element_currency.InnerText = item.Value.ToString(); // и значение
                        element_rates_block.AppendChild(element_currency); // и указываем кому принадлежит

                        XmlAttribute attribute_curr_name = document.CreateAttribute("name"); // создаём атрибут
                        attribute_curr_name.Value = item.Key; // устанавливаем значение атрибута
                        element_currency.Attributes.Append(attribute_curr_name); // добавляем атрибут
                    }
                    break;
                }
            }
        }

        private static void Main(string[] args)
        {
            Console.WriteLine("The application started at {0:HH:mm:ss.fff}", DateTime.Now);

            SetTimer(30000);

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
            CurrencyWrite();
        }
    }
}