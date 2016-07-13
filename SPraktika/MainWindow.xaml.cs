using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SPraktika
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private void Go(object upperCase)
        {
            bool upper = (bool)upperCase;
            MessageBox.Show((upper ? "HELLO!" : "hello!"));
        }

        public void fun(object a)
        {
        }

        public MainWindow()
        {
            InitializeComponent();
            EuropeanCentralBank ecb = new EuropeanCentralBank();
            CentralBankofRussia cbr = new CentralBankofRussia();
            YahooFinance yf = new YahooFinance();
            BLRFinanceInfo blr = new BLRFinanceInfo();
            CurrencyData AverageData = new CurrencyData();

            // Т.к. при доступе к веб ресурсу возможны задержки И
            //Т.к. AngleSharp работает асинхронно, переключясь на этот процесс, вывод начинался раньше чем AS завершал работу
            // Вытесним его на параллельный поток, и вывод по его готовности
            Thread ReadBlr = new Thread(blr.Read);
            ReadBlr.Start(AverageData.abc);
            Thread ReadECB = new Thread(ecb.Read);
            ReadECB.Start(AverageData.abc);
            Thread ReadCBR = new Thread(cbr.Read);
            ReadCBR.Start(AverageData.abc);
            Thread ReadYf = new Thread(yf.Read);
            ReadYf.Start(AverageData.abc);

            while (true)
            {
                Thread.Sleep(250);//от перегрузки потока
                if (!blr.InReading && !cbr.InReading && !ecb.InReading && !yf.InReading)
                {
                    tbConsole.Text = ecb.Show();
                    tbConsole2.Text = cbr.Show();
                    tbConsole3.Text = yf.Show();
                    tbConsole5.Text = blr.Show();
                    AverageData.CalcAverageCurrencyRate(ecb, cbr, yf, blr);
                    tbConsole4.Text = "";
                    foreach (var item in AverageData.CurrencyRates)
                        tbConsole4.Text += item.Key + " = " + item.Value.ToString() + "руб.\n";
                    break;
                }
            }
        }
    }
}