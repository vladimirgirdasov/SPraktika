using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
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
        private List<CurrencyRating> cbrList = new List<CurrencyRating>();
        private List<CurrencyRating> ecbList = new List<CurrencyRating>();
        private List<CurrencyRating> yfList = new List<CurrencyRating>();
        private List<CurrencyRating> blrList = new List<CurrencyRating>();
        private List<CurrencyRating> avgList = new List<CurrencyRating>();//Среднее

        private EuropeanCentralBank ecbReader = new EuropeanCentralBank();
        private CentralBankofRussia cbrReader = new CentralBankofRussia();
        private YahooFinance yfReader = new YahooFinance();
        private BLRFinanceInfo blrReader = new BLRFinanceInfo();
        //Погода
        private YandexCities yaCity = new YandexCities();
        private YandexWeather yaWeather = new YandexWeather();
        private Gismeteo gisWeather = new Gismeteo();
        private Thread ThreadReadRegion;
        private Thread ThreadReadGismeteoWeather;
        private Thread ThreadExtractBackPageHref;

        private GUI gui = new GUI();

        public bool done;

        public async void UpdateCurrencyInfo()
        {
            blrList = await blrReader.ReadAsync();
            cbrList = cbrReader.Read();
            ecbList = ecbReader.Read();
            yfList = yfReader.Read();
            avgList = AverageCurrencyData.CalcAverageRates(blrList, cbrList, ecbList, yfList);
            //MessageBox.Show("Done", "UpdateCurrencyInfo");
            done = true;
        }

        public MainWindow()
        {
            InitializeComponent();
            blrReader.InReading = true;
            Thread a = new Thread(UpdateCurrencyInfo);
            a.Start();
            while (true)
            {
                if (done)
                {
                    gui.Fill_DataGrid_Currencies(dgSingleSource, cbrList);
                    gui.Fill_DataGrid_Currencies(dgAverageValues, avgList);
                    break;
                }
                Thread.Sleep(250);
            }

            /*
            if (File.Exists(YandexCities.ConfigDirectoryDefault))
            {
                yaCity = new YandexCities(YandexCities.ConfigDirectoryDefault);
                yaWeather = new YandexWeather();
                yaWeather.Read(yaCity.City_id);
                gui.Show_YandexWeather(yaWeather, yaCity.City, lCity, lTimeOfDay, iWeather, lTemperature, lWindSpeed, lWindDirection, lPressure, lDampness, lTemperatureTomorrow);
            }

            if (File.Exists(Gismeteo.ConfigDirectoryDefault))
            {
                gisWeather = new Gismeteo(Gismeteo.ConfigDirectoryDefault);

                ThreadReadGismeteoWeather = new Thread(gisWeather.Read);
                ThreadReadGismeteoWeather.Start(gisWeather.CurrentHref);
                gisWeather.InReading = true;
                //
                while (true)//ожидаем прочтения
                {
                    if (gisWeather.InReading == false)
                    {
                        gui.Show_GismeteoWeather(gisWeather, gisWeather.CitySelected, lCity1, lCloudness1, iWeather1, lTemperature1, lWindSpeed1, lWindDirection1, lPressure1, lDampness1);
                        break;
                    }
                    Thread.Sleep(150);//от перегрузки потока
                }
            }
            */

            //TEMPORARY
            bGismeteo.Opacity = 0.25;
            bYandexWeather.Opacity = 1;
            GridWeatherYandex.Visibility = Visibility.Hidden;
            GridWeatherGismeteo.Visibility = Visibility.Visible;
        }

        private void cbSelectCurrencyResource_Loaded(object sender, RoutedEventArgs e)
        {
            cbSelectCurrencyResource.Items.Add("ЦБ Европы");
            cbSelectCurrencyResource.Items.Add("ЦБ РФ");
            cbSelectCurrencyResource.Items.Add("Yahoo Finance");
            cbSelectCurrencyResource.Items.Add("BLR Finance");
            cbSelectCurrencyResource.SelectedIndex = 1;
        }

        private void cbSelectCurrencyResource_SelectionChanged(object sender, SelectionChangedEventArgs e)//!!

        {
            string name = cbSelectCurrencyResource.SelectedValue.ToString();
            switch (name)
            {
                case "ЦБ Европы":
                    gui.Fill_DataGrid_Currencies(dgSingleSource, ecbList);
                    break;
                case "ЦБ РФ":
                    gui.Fill_DataGrid_Currencies(dgSingleSource, cbrList);
                    break;
                case "Yahoo Finance":
                    gui.Fill_DataGrid_Currencies(dgSingleSource, yfList);
                    break;
                case "BLR Finance":
                    gui.Fill_DataGrid_Currencies(dgSingleSource, blrList);
                    break;
                default:
                    MessageBox.Show("cbSelectCurrencyResource_SelectionChanged : Unexpected value", "Error");
                    break;
            }
        }

        private void bUpdateCurrencyInfo_Click(object sender, RoutedEventArgs e)//!!
        {
            //UpdateCurrencyInfo();
            ////Перерисовка таблицы по одному источнику
            //string name;
            //if (cbSelectCurrencyResource.SelectedValue != null)
            //    name = cbSelectCurrencyResource.SelectedValue.ToString();
            //else
            //    name = "ЦБ РФ";
            //switch (name)
            //{
            //    case "ЦБ Европы":
            //        gui.FillDataGrid_2_SingleResource(dgSingleSource, ecb, ecb);
            //        break;
            //    case "ЦБ РФ":
            //        gui.FillDataGrid_2_SingleResource(dgSingleSource, cbr, cbr);
            //        break;
            //    case "Yahoo Finance":
            //        gui.FillDataGrid_2_SingleResource(dgSingleSource, yf, yf);
            //        break;
            //    case "BLR Finance":
            //        gui.FillDataGrid_2_SingleResource(dgSingleSource, blr, blr);
            //        break;
            //    default:
            //        MessageBox.Show("cbSelectCurrencyResource_SelectionChanged : Unexpected value", "Error");
            //        break;
            //}
            ////Перерисовка таблцы со средними показателями
            //gui.FillDataGrid_AverageValues(dgAverageValues, AverageData, ecb, blr, cbr, yf);
        }

        private void bUpdateCurrencyInfo_MouseEnter(object sender, MouseEventArgs e)
        {
            lUpdateCurrencyInfo.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
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
            if (yaCity.City != "")
            {
                yaWeather = new YandexWeather();
                yaWeather.Read(yaCity.City_id);
                gui.Show_YandexWeather(yaWeather, yaCity.City, lCity, lTimeOfDay, iWeather, lTemperature, lWindSpeed, lWindDirection, lPressure, lDampness, lTemperatureTomorrow);
                //Запишем последний корректной введеный город (id) в conf, как город по умолчанию
                yaCity.SaveLastCity();
            }
        }

        private void bFindCity_MouseEnter(object sender, MouseEventArgs e)
        {
            lFindCity.Foreground = System.Windows.Media.Brushes.DeepSkyBlue;
        }

        private void bFindCity_MouseLeave(object sender, MouseEventArgs e)
        {
            lFindCity.Foreground = System.Windows.Media.Brushes.Gray;
        }

        private void bGismeteo_Click(object sender, RoutedEventArgs e)
        {
            bGismeteo.Opacity = 0.25;
            bYandexWeather.Opacity = 1;
            GridWeatherYandex.Visibility = Visibility.Hidden;
            GridWeatherGismeteo.Visibility = Visibility.Visible;
        }

        private void bYandexWeather_Click(object sender, RoutedEventArgs e)
        {
            bGismeteo.Opacity = 1;
            bYandexWeather.Opacity = 0.25;
            GridWeatherYandex.Visibility = Visibility.Visible;
            GridWeatherGismeteo.Visibility = Visibility.Hidden;
        }

        private void cbGismeteoRegion_Loaded(object sender, RoutedEventArgs e)
        {
            gisWeather = new Gismeteo();
            ThreadReadRegion = new Thread(gisWeather.ReadRegions_from);
            ThreadReadRegion.Start(gisWeather.Address);
            gisWeather.InReading = true;
            //
            while (true)
            {
                if (gisWeather.InReading == false)
                {
                    gui.Fill_Gismeteo_ComboBox_from(gisWeather.RegionHrefs, cbGismeteoRegion);
                    break;
                }
                Thread.Sleep(150);//от перегрузки потока
            }
            gui.Gismeteo_button_back__Turn(bGismeteo_back, !gisWeather.IsInRootDir());//Если в корне, блокируем кнопку Back
        }

        private void cbGismeteoRegion_SelectionChanged(object sender, SelectionChangedEventArgs e)//!!
        {
            //if (gui.cbGismeteoRegion_InEditing == false)
            //{
            //    gui.cbGismeteoRegion_InEditing = true;
            //    var region = cbGismeteoRegion.SelectedItem;
            //    gisWeather.CurrentHref = gisWeather.RegionHrefs[(string)region];
            //    if ((gisWeather.RegionHrefs[(string)region]).Contains("daily") == true)
            //    {
            //        gisWeather.CitySelected = (string)region;
            //        ThreadReadGismeteoWeather = new Thread(gisWeather.Read);
            //        ThreadReadGismeteoWeather.Start(gisWeather.RegionHrefs[(string)region]);
            //        gisWeather.InReading = true;
            //        //
            //        while (true)//ожидаем прочтения
            //        {
            //            if (gisWeather.InReading == false)
            //            {
            //                gui.Show_GismeteoWeather(gisWeather, gisWeather.CitySelected, lCity1, lCloudness1, iWeather1, lTemperature1, lWindSpeed1, lWindDirection1, lPressure1, lDampness1);
            //                gisWeather.SaveLastCity();//Сохраним последний корректно выбранный регион, как регион по умолчанию
            //                break;
            //            }
            //            Thread.Sleep(150);//от перегрузки потока
            //        }
            //    }
            //    else
            //    {
            //        ThreadReadRegion = new Thread(gisWeather.ReadRegions_from);
            //        ThreadReadRegion.Start(gisWeather.RegionHrefs[(string)region]);
            //        gisWeather.InReading = true;
            //        //
            //        while (true)
            //        {
            //            if (gisWeather.InReading == false)
            //            {
            //                gui.Fill_Gismeteo_ComboBox_from(gisWeather.RegionHrefs, cbGismeteoRegion);
            //                break;
            //            }
            //            Thread.Sleep(150);//от перегрузки потока
            //        }
            //    }
            //    gui.Gismeteo_button_back__Turn(bGismeteo_back, !gisWeather.IsInRootDir());//Если в корне, блокируем кнопку Back
            //    gui.Gismeteo_Change_CurrentDir_Label(lCurrentDir, (string)region);
            //}
            //gui.cbGismeteoRegion_InEditing = false;
        }

        private void bGismeteo_back_Click(object sender, RoutedEventArgs e)
        {
            ThreadExtractBackPageHref = new Thread(gisWeather.ExtractBackPageHref);
            ThreadExtractBackPageHref.Start();
            gisWeather.InReading = true;
            while (true)
            {
                if (gisWeather.InReading == false)
                {
                    ThreadReadRegion = new Thread(gisWeather.ReadRegions_from);
                    ThreadReadRegion.Start(gisWeather.BackPageHref);
                    gisWeather.InReading = true;
                    //
                    while (true)
                    {
                        if (gisWeather.InReading == false)
                        {
                            gui.cbGismeteoRegion_InEditing = true;
                            gui.Fill_Gismeteo_ComboBox_from(gisWeather.RegionHrefs, cbGismeteoRegion);
                            gui.cbGismeteoRegion_InEditing = false;
                            break;
                        }
                        Thread.Sleep(150);//от перегрузки потока
                    }
                    break;
                }
                Thread.Sleep(150);//от перегрузки потока
            }
            gui.Gismeteo_button_back__Turn(bGismeteo_back, !gisWeather.IsInRootDir());//Если в корне, блокируем кнопку Back
            gui.Gismeteo_Change_CurrentDir_Label(lCurrentDir, gisWeather.BackPagedRegionName);
        }
    }
}