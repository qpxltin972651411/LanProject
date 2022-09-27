using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;
using LanProject.MainApplication.Model;
using LanProject.MainApplication.ViewModel;
using LiveCharts;
using LiveCharts.Wpf;
using MaterialDesignThemes.Wpf;

namespace LanProject.MainApplication.View
{
    /// <summary>
    /// Home.xaml 的互動邏輯
    /// </summary>
    public partial class Home : UserControl
    {
        private HomeViewModel vm { get; set; }
        private LoadingInfoNotificationMessage LoadingDialog { get; set; }
        public Home()
        {
            Loaded += Home_Loaded;
            InitializeComponent();
            vm = new HomeViewModel();
            DataContext = vm;
        }

        private void Home_Loaded(object sender, RoutedEventArgs e)
        {
            Load();
        }
        #region 讀取表單
        private void LoadingResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
        }
        private void Loading(object sender, DoWorkEventArgs e) => vm.Loading();
        public void Load()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                LoadingDialog = new LoadingInfoNotificationMessage();
                await DialogHost.Show(LoadingDialog, "RootDialog");
            }));
            var bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(Loading);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadingResult);
            bw.RunWorkerAsync();
        }
        #endregion
    }
}
