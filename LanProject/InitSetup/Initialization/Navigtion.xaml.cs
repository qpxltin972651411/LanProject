using LanProject.InitSetup.Initialization.Pages;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LanProject.InitSetup.Initialization
{
    /// <summary>
    /// Navigtion.xaml 的互動邏輯
    /// </summary>
    public partial class Navigtion : NavigationWindow
    {
        private Finish initpage { get; set; }
        public bool LockClosing = false;
        public Navigtion()
        {
            InitializeComponent();
        }

        private void NavigationWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (initpage == null)
            {
                e.Cancel = false;
                return;
            }
            else
            {
                if (initpage.mandatory)
                    return;
            }
            LockClosing = true;
            if (MessageBox.Show("確定要終止程式運行嗎", "離開", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
            {
                e.Cancel = true;
                LockClosing = false;
                if (initpage.mandatory)
                    Close();
                return;
            }
            Process.GetCurrentProcess().Kill();
        }

        private void NavigationWindow_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            var ta = new ThicknessAnimation();
            ta.Duration = TimeSpan.FromSeconds(0.3);
            ta.DecelerationRatio = 0.7;
            ta.To = new Thickness(0, 0, 0, 0);
            if (e.NavigationMode == NavigationMode.New)
            {
                ta.From = new Thickness(500, 0, 0, 0);
            }
            else if (e.NavigationMode == NavigationMode.Back)
            {
                ta.From = new Thickness(0, 0, 500, 0);
            }
            if (e.Content != null)
                (e.Content as Page).BeginAnimation(MarginProperty, ta);
        }

        private void NavigationWindow_Navigated(object sender, NavigationEventArgs e)
        {
            Page obj = (Page)this.Content;
            if (obj.Name == "finishpage")
            {
                Finish _p = (Finish)Content;
                initpage = _p;
            }
        }
    }
}
