﻿using System;
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
        private delegate void ReadSourceDlg(HashSet<string> abc);

        public MainWindow()
        {
            InitializeComponent();
            EuropeanCentralBank ecb = new EuropeanCentralBank();
            CentralBankofRussia cbr = new CentralBankofRussia();
            YahooFinance yf = new YahooFinance();
            BLRFinanceInfo blr = new BLRFinanceInfo();
            CurrencyData AverageData = new CurrencyData();

            ReadSourceDlg ReadSources = new ReadSourceDlg(blr.Read);
            ReadSources += cbr.Read;
            ReadSources += ecb.Read;
            ReadSources += yf.Read;
            ReadSources(AverageData.abc);

            tbConsole.Text = ecb.Show();
            tbConsole2.Text = cbr.Show();
            tbConsole3.Text = yf.Show();
            tbConsole5.Text = blr.Show();
            AverageData.CalcAverageCurrencyRate(ecb, cbr, yf, blr);
            tbConsole4.Text = "";
            foreach (var item in AverageData.CurrencyRates)
                tbConsole4.Text += item.Key + " = " + item.Value.ToString() + "руб.\n";
        }
    }
}