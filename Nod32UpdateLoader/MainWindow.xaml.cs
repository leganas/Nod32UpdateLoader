using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Nod32UpdateLoader
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindowDataContainer MainWindowDataContainer;

        public MainWindow()
        {
            InitializeComponent();
            MainWindowDataContainer = (MainWindowDataContainer)DataContext;
            if (Setting.current.HidenStart) this.Hide();
        }

        private void NotifyIconExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void MenuItem_Setting_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog browserDialog = new FolderBrowserDialog();
            browserDialog.Description = @"Укажите папку распаковки";
            if (browserDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                Setting.current.ExtractPath = browserDialog.SelectedPath;
                Setting.SaveSetting();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           MainWindowDataContainer.getNOD32Update("http://progzona.ru/bezopasnost/bazae/8-bazy.html");  
        }
    }
}
