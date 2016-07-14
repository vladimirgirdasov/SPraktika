using System;
using System.Collections.Generic;
using System.Data;
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
        //Валюты
        private EuropeanCentralBank ecb = new EuropeanCentralBank();
        private CentralBankofRussia cbr = new CentralBankofRussia();
        private YahooFinance yf = new YahooFinance();
        private BLRFinanceInfo blr = new BLRFinanceInfo();
        private CurrencyData AverageData = new CurrencyData();
        //Города(Погода)
        private YandexCities yaCity = new YandexCities();

        private GUI gui = new GUI();

        public void UpdateCurrencyInfo()
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
            Thread ReadECB = new Thread(ecb.Read);
            ReadECB.Start(AverageData.abc);
            Thread ReadCBR = new Thread(cbr.Read);
            ReadCBR.Start(AverageData.abc);
            Thread ReadYf = new Thread(yf.Read);
            ReadYf.Start(AverageData.abc);

            //MessageBox.Show("Done", "UpdateCurrencyInfo");
        }

        public MainWindow()
        {
            InitializeComponent();
            UpdateCurrencyInfo();
            gui.FillDataGrid_2_SingleResource(dgSingleSource, blr, blr);
            gui.FillDataGrid_AverageValues(dgAverageValues, AverageData, ecb, blr, cbr, yf);
        }

        private void cbSelectCurrencyResource_Loaded(object sender, RoutedEventArgs e)
        {
            cbSelectCurrencyResource.Items.Add("ЦБ Европы");
            cbSelectCurrencyResource.Items.Add("ЦБ РФ");
            cbSelectCurrencyResource.Items.Add("Yahoo Finance");
            cbSelectCurrencyResource.Items.Add("BLR Finance");
        }

        private void cbSelectCurrencyResource_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string name = cbSelectCurrencyResource.SelectedValue.ToString();
            switch (name)
            {
                case "ЦБ Европы":
                    gui.FillDataGrid_2_SingleResource(dgSingleSource, ecb, ecb);
                    break;
                case "ЦБ РФ":
                    gui.FillDataGrid_2_SingleResource(dgSingleSource, cbr, cbr);
                    break;
                case "Yahoo Finance":
                    gui.FillDataGrid_2_SingleResource(dgSingleSource, yf, yf);
                    break;
                case "BLR Finance":
                    gui.FillDataGrid_2_SingleResource(dgSingleSource, blr, blr);
                    break;
                default:
                    MessageBox.Show("cbSelectCurrencyResource_SelectionChanged : Unexpected value", "Error");
                    break;
            }
        }

        private void bUpdateCurrencyInfo_Click(object sender, RoutedEventArgs e)
        {
            UpdateCurrencyInfo();
            //Перерисовка таблицы по одному источнику
            string name;
            if (cbSelectCurrencyResource.SelectedValue != null)
                name = cbSelectCurrencyResource.SelectedValue.ToString();
            else
                name = "ЦБ РФ";
            switch (name)
            {
                case "ЦБ Европы":
                    gui.FillDataGrid_2_SingleResource(dgSingleSource, ecb, ecb);
                    break;
                case "ЦБ РФ":
                    gui.FillDataGrid_2_SingleResource(dgSingleSource, cbr, cbr);
                    break;
                case "Yahoo Finance":
                    gui.FillDataGrid_2_SingleResource(dgSingleSource, yf, yf);
                    break;
                case "BLR Finance":
                    gui.FillDataGrid_2_SingleResource(dgSingleSource, blr, blr);
                    break;
                default:
                    MessageBox.Show("cbSelectCurrencyResource_SelectionChanged : Unexpected value", "Error");
                    break;
            }
            //Перерисовка таблцы со средними показателями
            gui.FillDataGrid_AverageValues(dgAverageValues, AverageData, ecb, blr, cbr, yf);
        }

        private void bUpdateCurrencyInfo_MouseEnter(object sender, MouseEventArgs e)
        {
            lUpdateCurrencyInfo.Foreground = System.Windows.Media.Brushes.LightSkyBlue;
            lUpdateCurrencyInfo.FontWeight = FontWeights.Light;
        }

        private void bUpdateCurrencyInfo_MouseLeave(object sender, MouseEventArgs e)
        {
            lUpdateCurrencyInfo.Foreground = System.Windows.Media.Brushes.Gray;
            lUpdateCurrencyInfo.FontWeight = FontWeights.ExtraLight;
        }

        private void bFindCity_Click(object sender, RoutedEventArgs e)
        {
            yaCity = new YandexCities();
            yaCity.Read(tbFindCity.Text);
        }
    }
}