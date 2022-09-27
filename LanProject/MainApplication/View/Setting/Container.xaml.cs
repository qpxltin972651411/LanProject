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

namespace LanProject.MainApplication.View.Setting
{
    /// <summary>
    /// Container.xaml 的互動邏輯
    /// </summary>
    public partial class Container : UserControl
    {
        private Settings window;
        private void LoadWindow()
        {
            Settings main = null;
            var obj = VisualTreeHelper.GetParent(this);
            while (obj.GetType() != typeof(Settings))
                obj = VisualTreeHelper.GetParent(obj);
            main = obj as Settings;
            window = main;
        }
        public Container()
        {
            InitializeComponent();
        }
        private void Note_ElementClick(object sender, RoutedEventArgs e)
        {

            LoadWindow();
            if (window == null) return;
            window.ExecutePage(SettingPages.Note);
        }

        private void Personal_ElementClick(object sender, RoutedEventArgs e)
        {
            LoadWindow();
            if (window == null) return;
            window.ExecutePage(SettingPages.Personal);
        }
    }
}
