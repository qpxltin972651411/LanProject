using LanProject.Method;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Shapes;

namespace LanProject.InitSetup.Initialization
{
    /// <summary>
    /// Fix.xaml 的互動邏輯
    /// </summary>
    public partial class Fix : Window
    {
        public event Action<bool> resetSignal;
        public Fix()
        {
            InitializeComponent();
            if (resetSignal != null)
                resetSignal(false);
        }
        private void reSet_Click(object sender, RoutedEventArgs e)
        {
            if (resetSignal != null)
                resetSignal(true);
            Close();
        }
        private void reSelectFolderPath_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.Description = "請選擇路徑";
            path.ShowDialog();
            if (path.SelectedPath != String.Empty)
                Configuration.AddUpdateAppSettings("dataPath", path.SelectedPath);
            Close();
        }

        private void reTry_Click(object sender, RoutedEventArgs e) => Close();

        private void close_Click(object sender, RoutedEventArgs e) => Process.GetCurrentProcess().Kill();
    }
}
