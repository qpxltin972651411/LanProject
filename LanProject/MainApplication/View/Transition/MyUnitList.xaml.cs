using LanProject.MainApplication.Map;
using LanProject.MainApplication.Model;
using LanProject.MainApplication.ViewModel;
using MaterialDesignThemes.Wpf;
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

namespace LanProject.MainApplication.View.Transition
{
    /// <summary>
    /// MyUnitList.xaml 的互動邏輯
    /// </summary>
    public partial class MyUnitList : UserControl
    {
        public CreateUnit Get()
        {
            return CreateUnit.CloneUnitToCreateUnit(CurrentSelectedUnit);
        }
        private SelfCompanyViewModel vm { get; set; }
        public SearchNotificationMessage SearchDialog { get; set; }
        public LoadingInfoNotificationMessage LoadingDialog { get; set; }
        private Unit currentselectedunit;
        public Unit CurrentSelectedUnit
        {
            get => currentselectedunit;
            set
            {
                currentselectedunit = value;
                if (Finish == null) return;
                if (currentselectedunit == null)
                    Finish.IsEnabled = false;
                else
                    Finish.IsEnabled = true;
            }
        }
        #region 讀取單位
        private void LoadingResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("SecondDialog"))
                DialogHost.Close("SecondDialog");
        }
        private void Loading(object sender, DoWorkEventArgs e) => vm.Loading("SELECT * FROM myunit;");
        private void Load()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                LoadingDialog = new LoadingInfoNotificationMessage();
                await DialogHost.Show(LoadingDialog, "SecondDialog");
            }));
            var bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(Loading);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(LoadingResult);
            bw.RunWorkerAsync();
        }
        #endregion
        #region 搜尋單位
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchDialog == null)
            {
                SearchDialog = new SearchNotificationMessage();
                SearchDialog.Search.PropertyChanged += Search_PropertyChanged;
            }
            DialogHost.Show(SearchDialog, "SecondDialog");
        }
        private void Search_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(Searching);
            bw.RunWorkerAsync();
        }
        private void Searching(object sender, DoWorkEventArgs e)
        {
            if (vm.CloneList == null)
                vm.CloneList = new BindingList<Unit>(vm.TotalList);
            vm.TotalList = new BindingList<Unit>(vm.CloneList);
            if (!String.IsNullOrEmpty(SearchDialog.Search.Name))
                vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Name.ToLower().Contains(SearchDialog.Search.Name.ToLower())).ToList());
            if (!String.IsNullOrEmpty(SearchDialog.Search.Cel))
                vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Cel.Contains(SearchDialog.Search.Cel)).ToList());
            if (SearchDialog.Search.Notax)
            {
                vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Tax == String.Empty).ToList());
            }
            else
            {
                if (SearchDialog.Search.HaveTax)
                {
                    if (SearchDialog.Search.Tax == String.Empty)
                        vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Tax != String.Empty).ToList());
                    else
                        vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Tax.Contains(SearchDialog.Search.Tax)).ToList());
                }
            }

            if (SearchDialog.Search.Notel)
            {
                vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => (x.Tel.AreaCode == String.Empty && x.Tel.Number == String.Empty)).ToList());
            }
            else
            {
                if (SearchDialog.Search.HaveTel)
                {
                    if (SearchDialog.Search.Tel.AreaCode == String.Empty && SearchDialog.Search.Tel.Number == String.Empty)
                        vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => (x.Tel.AreaCode != String.Empty || x.Tel.Number != String.Empty)).ToList());
                    else
                        vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Tel.AreaCode.Contains(SearchDialog.Search.Tel.AreaCode) && x.Tel.Number.Contains(SearchDialog.Search.Tel.Number)).ToList());
                }
            }

            if (SearchDialog.Search.Nofax)
            {
                vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => (x.Fax.AreaCode == String.Empty && x.Fax.Number == String.Empty)).ToList());
            }
            else
            {
                if (SearchDialog.Search.HaveFax)
                {
                    if (SearchDialog.Search.Fax.AreaCode == String.Empty && SearchDialog.Search.Fax.Number == String.Empty)
                        vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => (x.Fax.AreaCode != String.Empty || x.Fax.Number != String.Empty)).ToList());
                    else
                        vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Fax.AreaCode.Contains(SearchDialog.Search.Fax.AreaCode) && x.Fax.Number.Contains(SearchDialog.Search.Fax.Number)).ToList());
                }
            }

            if (SearchDialog.Search.Noaddress)
            {
                vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => (x.Location.Country == String.Empty && x.Location.City == String.Empty && x.Location.Address == String.Empty)).ToList());
            }
            else
            {
                if (SearchDialog.Search.HaveAddress)
                {
                    if (SearchDialog.Search.Location.Country == "無" && SearchDialog.Search.Location.City == "無" && SearchDialog.Search.Location.Address == String.Empty)
                    {
                        vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => (x.Location.Country != String.Empty || x.Location.City != String.Empty || x.Location.Address != String.Empty)).ToList());
                    }
                    else
                    {
                        if (SearchDialog.Search.Location.Country != "無")
                        {
                            //vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Country.Contains(SearchDialog.Search.Country) && x.City.Contains(SearchDialog.Search.City) && x.Address.Contains(SearchDialog.Search.Address)).ToList());
                            vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Location.Country.Contains(SearchDialog.Search.Location.Country)).ToList());
                        }
                        if (SearchDialog.Search.Location.City != "無")
                        {
                            //vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Country.Contains(SearchDialog.Search.Country) && x.City.Contains(SearchDialog.Search.City) && x.Address.Contains(SearchDialog.Search.Address)).ToList());
                            vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Location.City.Contains(SearchDialog.Search.Location.City)).ToList());
                        }
                        if (SearchDialog.Search.Location.Address != String.Empty)
                        {
                            //vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Country.Contains(SearchDialog.Search.Country) && x.City.Contains(SearchDialog.Search.City) && x.Address.Contains(SearchDialog.Search.Address)).ToList());
                            vm.TotalList = new BindingList<Unit>(vm.TotalList.Where(x => x.Location.Address.Contains(SearchDialog.Search.Location.Address)).ToList());
                        }
                    }
                }
            }
            vm.Filter();
        }
        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            if (SearchDialog == null) return;
            SearchDialog.Clear();
            SearchDialog.Search.PropertyChanged += Search_PropertyChanged;
            if (vm.CloneList != null)
                vm.TotalList = new BindingList<Unit>(vm.CloneList);
            vm.CloneList = null;
            vm.Filter();
        }
        #endregion
        public MyUnitList()
        {
            InitializeComponent();
            CurrentSelectedUnit = null;
            Loaded += MyUnitList_Loaded;
            vm = new SelfCompanyViewModel();
            DataContext = vm;
        }
        private void MyUnitList_Loaded(object sender, RoutedEventArgs e) => Load();
        private void PrevButton_Click(object sender, RoutedEventArgs e) => vm.PrevPage();
        private void NextButton_Click(object sender, RoutedEventArgs e) => vm.NextPage();
        private void SelectButton_Click(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as Unit;
            CurrentSelectedUnit = obj;
            helperText.Foreground = Brushes.Green;
            helperText.Text = String.Format("你已選擇 {0}", CurrentSelectedUnit.Name);
            Table.SelectedItem = CurrentSelectedUnit;
        }
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var source = ((FrameworkElement)sender).DataContext as Unit;
            var infoDialog = new UnitDetailNotificationMessage(source, 0);
            DialogHost.Show(infoDialog, "SecondDialog");
        }
        private void Finish_Click(object sender, RoutedEventArgs e)
        {
            Window parentWindow = (Window)Parent;
            parentWindow.DialogResult = true;
            parentWindow.Close();
        }
    }
}

