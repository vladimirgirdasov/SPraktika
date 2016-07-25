using System;
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

namespace MyServicesDataReader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Сюда читается вся инфа. Из неё делаются выборки
        public static List<CurrencyDataUnit> CurrencyData = new List<CurrencyDataUnit>();
        public static List<WeatherDataUnit> WeatherData = new List<WeatherDataUnit>();

        public MainWindow()
        {
            InitializeComponent();
            LogsDirectories.Read_and_Check_Config();
            tbFolderCurrencyLogs.Text = LogsDirectories.DirectoryCurrency;
            tbFolderWeatherLogs.Text = LogsDirectories.DirectoryWeather;
            //Чтение и заполнение фильтров и вывод полной таблицы
            Update_CurrencyData_and_Filters(cbCurrencyFilterDate, cbCurrencyFilterName, cbCurrencyFilterSource);
            Show_Currencies_With_Filters(dgCurrency, cbCurrencyFilterDate.SelectedValue.ToString(), cbCurrencyFilterName.SelectedValue.ToString(), cbCurrencyFilterSource.SelectedValue.ToString());
            Update_WeatherData_and_Filters(cbWeatherFilterDate, cbWeatherFilterCity, cbWeatherFilterTimeOfDay, cbWeatherFilterSource);
            Show_Weather_With_Filters(dgWeather, cbWeatherFilterDate.SelectedValue.ToString(), cbWeatherFilterCity.SelectedValue.ToString(),
                cbWeatherFilterTimeOfDay.SelectedValue.ToString(), cbWeatherFilterSource.SelectedValue.ToString());
        }

        private void bFolderCurrencyLogs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                if (LogsDirectories.IsCurrencyDirectory(dialog.SelectedPath))
                {
                    tbFolderCurrencyLogs.Text = dialog.SelectedPath;
                    LogsDirectories.Set_and_Write_Config(dialog.SelectedPath, null);
                    MessageBox.Show("Логи курсов валют в директории найдены, Конфиг обновлен", "OK");
                }
                else
                {
                    MessageBox.Show("Тут файлы логов не найдены", "Ошибка определения директории");
                }
            }
        }

        private void bFolderWeatherLogs_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                if (LogsDirectories.IsWeatherDirectory(dialog.SelectedPath))
                {
                    tbFolderWeatherLogs.Text = dialog.SelectedPath;
                    LogsDirectories.Set_and_Write_Config(null, dialog.SelectedPath);
                    MessageBox.Show("Логи погоды в директории найдены, Конфиг обновлен", "OK");
                }
                else
                {
                    MessageBox.Show("Тут файлы логов не найдены", "Ошибка определения директории");
                }
            }
        }

        private void bShowCurrency_Click(object sender, RoutedEventArgs e)
        {
            Show_Currencies_With_Filters(dgCurrency, cbCurrencyFilterDate.SelectedValue.ToString(), cbCurrencyFilterName.SelectedValue.ToString(), cbCurrencyFilterSource.SelectedValue.ToString());
        }

        private void bShowWeather_Click(object sender, RoutedEventArgs e)
        {
            Show_Weather_With_Filters(dgWeather, cbWeatherFilterDate.SelectedValue.ToString(), cbWeatherFilterCity.SelectedValue.ToString(),
                cbWeatherFilterTimeOfDay.SelectedValue.ToString(), cbWeatherFilterSource.SelectedValue.ToString());
        }

        private static void Show_Currencies_With_Filters(DataGrid TableCurrency, string dateFilter, string nameFilter, string sourceFilter)
        {
            List<CurrencyDataUnit> ans = new List<CurrencyDataUnit>();
            //Проводим выборки по фильтрам
            if (dateFilter != "Все")
            {
                ans = (from x in CurrencyData
                       where x.date == dateFilter
                       select x)
                          .ToList();
            }
            else
            {
                ans = (from x in CurrencyData
                       select x)
                       .ToList();
            }
            if (nameFilter != "Все")
            {
                ans = (from x in ans
                       where x.name == nameFilter
                       select x)
                       .ToList();
            }
            if (sourceFilter != "Все")
            {
                ans = (from x in ans
                       where x.source == sourceFilter
                       select x)
                       .ToList();
            }
            TableCurrency.ItemsSource = ans;
        }

        private static void Show_Weather_With_Filters(DataGrid TableCurrency, string dateFilter, string cityFilter, string timeOfDayFilter, string sourceFilter)
        {
            List<WeatherDataUnit> ans = new List<WeatherDataUnit>();
            //Проводим выборки по фильтрам
            if (dateFilter != "Все")
            {
                ans = (from x in WeatherData
                       where x.date == dateFilter
                       select x)
                          .ToList();
            }
            else
            {
                ans = (from x in WeatherData
                       select x)
                       .ToList();
            }
            if (cityFilter != "Все")
            {
                ans = (from x in ans
                       where x.city == cityFilter
                       select x)
                       .ToList();
            }
            if (timeOfDayFilter != "Все")
            {
                ans = (from x in ans
                       where x.timeOfDay == timeOfDayFilter
                       select x)
                       .ToList();
            }
            if (sourceFilter != "Все")
            {
                ans = (from x in ans
                       where x.source == sourceFilter
                       select x)
                       .ToList();
            }
            TableCurrency.ItemsSource = ans;
        }

        public static void Update_CurrencyData_and_Filters(ComboBox cbDateFilter, ComboBox cbNameFilter, ComboBox cbSourceFilter)
        {
            CurrencyData = CurrencyLogsReader.Read(LogsDirectories.Get_CurrencyLogs_Paths());
            //Обновление фильтров выборки:
            //Обнуление
            cbDateFilter.Items.Clear();
            cbNameFilter.Items.Clear();
            cbSourceFilter.Items.Clear();
            //Fill Dates
            cbDateFilter.Items.Add("Все");
            cbDateFilter.SelectedIndex = 0;
            var dates = CurrencyData.Select(x => x.date).Distinct().ToList();
            dates.Sort();
            foreach (var date in dates)
            {
                cbDateFilter.Items.Add(date);
            }
            //Fill Names
            cbNameFilter.Items.Add("Все");
            cbNameFilter.SelectedIndex = 0;
            var names = CurrencyData.Select(x => x.name).Distinct().ToList();
            names.Sort();
            foreach (var name in names)
            {
                cbNameFilter.Items.Add(name);
            }
            //Fill Sources
            cbSourceFilter.Items.Add("Все");
            cbSourceFilter.SelectedIndex = 0;
            var sources = CurrencyData.Select(x => x.source).Distinct().ToList();
            sources.Sort();
            foreach (var source in sources)
            {
                cbSourceFilter.Items.Add(source);
            }
        }

        public static void Update_WeatherData_and_Filters(ComboBox cbDateFilter, ComboBox cbCityFilter, ComboBox cbTimeOfDayFilter, ComboBox cbSourceFilter)
        {
            WeatherData = WeatherLogsReader.Read(LogsDirectories.Get_WeatherLogs_Paths());
            //Обновление фильтров выборки:
            //Обнуление
            cbDateFilter.Items.Clear();
            cbCityFilter.Items.Clear();
            cbTimeOfDayFilter.Items.Clear();
            cbSourceFilter.Items.Clear();
            //Fill Dates
            cbDateFilter.Items.Add("Все");
            cbDateFilter.SelectedIndex = 0;
            var dates = WeatherData.Select(x => x.date).Distinct().ToList();
            dates.Sort();
            foreach (var date in dates)
            {
                cbDateFilter.Items.Add(date);
            }
            //Fill City
            cbCityFilter.Items.Add("Все");
            cbCityFilter.SelectedIndex = 0;
            var cities = WeatherData.Select(x => x.city).Distinct().ToList();
            cities.Sort();
            foreach (var city in cities)
            {
                cbCityFilter.Items.Add(city);
            }
            //Fill TimeOfDay
            cbTimeOfDayFilter.Items.Add("Все");
            cbTimeOfDayFilter.SelectedIndex = 0;
            var times = WeatherData.Select(x => x.timeOfDay).Distinct().ToList();
            times.Sort();
            foreach (var time in times)
            {
                cbTimeOfDayFilter.Items.Add(time);
            }
            //Fill Sources
            cbSourceFilter.Items.Add("Все");
            cbSourceFilter.SelectedIndex = 0;
            var sources = WeatherData.Select(x => x.source).Distinct().ToList();
            sources.Sort();
            foreach (var source in sources)
            {
                cbSourceFilter.Items.Add(source);
            }
        }

        private void bUpdateCurrency_Click(object sender, RoutedEventArgs e)
        {
            Update_CurrencyData_and_Filters(cbCurrencyFilterDate, cbCurrencyFilterName, cbCurrencyFilterSource);
            Show_Currencies_With_Filters(dgCurrency, cbCurrencyFilterDate.SelectedValue.ToString(), cbCurrencyFilterName.SelectedValue.ToString(), cbCurrencyFilterSource.SelectedValue.ToString());
        }

        private void bUpdateWeather_Click(object sender, RoutedEventArgs e)
        {
            Update_WeatherData_and_Filters(cbWeatherFilterDate, cbWeatherFilterCity, cbWeatherFilterTimeOfDay, cbWeatherFilterSource);
            Show_Weather_With_Filters(dgWeather, cbWeatherFilterDate.SelectedValue.ToString(), cbWeatherFilterCity.SelectedValue.ToString(),
                cbWeatherFilterTimeOfDay.SelectedValue.ToString(), cbWeatherFilterSource.SelectedValue.ToString());
        }
    }
}