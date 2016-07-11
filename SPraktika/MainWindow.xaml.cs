﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
        public MainWindow()
        {
            InitializeComponent();
            EuropeanCentralBank ecb = new EuropeanCentralBank();
            ecb.Read();
            tbConsole.Text = ecb.Show();
            tbConsole.Text += "==============\n";
            CentralBankofRussia cbr = new CentralBankofRussia();
            cbr.Read();
            tbConsole.Text += cbr.Show();
            tbConsole.Text += "==============\n";
            YahooFinance yf = new YahooFinance();
            yf.Read();
            tbConsole.Text += yf.Show();
            tbConsole.Text += "==============\n";
        }
    }
}