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

namespace LanProject.InitSetup.Initialization.Pages
{
    /// <summary>
    /// SelectPath.xaml 的互動邏輯
    /// </summary>
    public partial class SelectPath : Page
    {
        public SelectPath()
        {
            InitializeComponent();
            Path.Text = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
        }
        private void Select()
        {
            FolderBrowserDialog path = new FolderBrowserDialog();
            path.Description = "請選擇路徑";
            path.ShowDialog();
            if (path.SelectedPath != String.Empty)
                Path.Text = path.SelectedPath;
        }
        private void SelectPathBtn_Click(object sender, RoutedEventArgs e) => Select();
        private void Path_PreviewMouseDown(object sender, MouseButtonEventArgs e) => Select();
        private void Next_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(new Signup(this));
    }
}
