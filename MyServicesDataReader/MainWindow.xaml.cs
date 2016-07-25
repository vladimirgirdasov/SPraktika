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
        public MainWindow()
        {
            InitializeComponent();
            LogsDirectories.Read_and_Check_Config();
            tbFolderCurrencyLogs.Text = LogsDirectories.DirectoryCurrency;
            tbFolderWeatherLogs.Text = LogsDirectories.DirectoryWeather;
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

        private void dgCurrency_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void bShowCurrency_Click(object sender, RoutedEventArgs e)
        {
            dgCurrency.ItemsSource = CurrencyLogsReader.Read(LogsDirectories.Get_CurrencyLogs_Paths());
        }
    }
}