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

namespace LanProject.MainApplication.View
{
    /// <summary>
    /// SelfCompany.xaml 的互動邏輯
    /// </summary>
    public partial class SelfCompany : UserControl
    {
        private SelfCompanyViewModel vm { get; }
        private MainWindow MainPage { get; set; }
        private LoadingInfoNotificationMessage LoadingDialog { get; set; }
        private CreateUnitNotificationMessage CreateDialog { get; set; }
        private SearchNotificationMessage SearchDialog { get; set; }
        private EditNotificationMessage EditDialog { get; set; }
        private Unit storeUnit { get; set; }
        public SelfCompany()
        {
            InitializeComponent();
            Loaded += SelfCompany_Loaded;
            vm = new SelfCompanyViewModel();
            DataContext = vm;
        }
        private void SelfCompany_Loaded(object sender, RoutedEventArgs e)
        {
            MainPage = Method.Function.GetParent(this);
            Load();
        }
        private void PrevButton_Click(object sender, RoutedEventArgs e) => vm.PrevPage();
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!vm.NextPage()) MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_NEXT_PAGE_FAILED));
        }
        #region 讀取單位
        private void LoadingResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            if (e.Error != null)
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_LOAD_UNIT_UNKNOWN_FAILED));
            else if (e.Cancelled)
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_LOAD_UNIT_UNKNOWN_FAILED));
        }
        private void Loading(object sender, DoWorkEventArgs e) => vm.Loading("SELECT * FROM myunit;");
        private void Load()
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
        #region 新增單位
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            int errorcount = vm.CalcErrorCount(CreateDialog.NewUnit);
            if (errorcount != 0)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_CREATE_UNIT_VERIFY_INPUT_FAILED));
                return;
            }
            else
            {
                DialogHost.CloseDialogCommand.Execute(null, null);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                    LoadingDialog = new LoadingInfoNotificationMessage { Title = "新增中", Message = "請稍後" };
                    await DialogHost.Show(LoadingDialog, "RootDialog");
                }));
                var bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(Creating);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(CreateResult);
                bw.RunWorkerAsync();
            }
        }
        private void Creating(object sender, DoWorkEventArgs e)
        {
            if (Method.Function.UnitHaveRepeat(vm.TotalList, CreateDialog.NewUnit))
            {
                e.Cancel = true;
                return;
            }
            try
            {
                string executeString = "INSERT INTO myunit(name,tax,cel,telareacode,telnumber,faxareacode,faxnumber,country,city,address) VALUES(@name,@tax,@cel,@telareacode,@telnumber,@faxareacode,@faxnumber,@country,@city,@address);";
                Dictionary<string, object> KeyValues = new Dictionary<string, object>();
                List<int> DataTypeList = new List<int>();
                KeyValues.Add("@name", CreateDialog.NewUnit.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@tax", CreateDialog.NewUnit.HaveTax ? CreateDialog.NewUnit.Tax : String.Empty);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", CreateDialog.NewUnit.Cel);
                DataTypeList.Add(0);

                if (CreateDialog.NewUnit.HaveTel)
                {
                    KeyValues.Add("@telareacode", CreateDialog.NewUnit.Tel.AreaCode);
                    DataTypeList.Add(0);
                    KeyValues.Add("@telnumber", CreateDialog.NewUnit.Tel.Number);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@telareacode", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@telnumber", DBNull.Value);
                    DataTypeList.Add(3);
                }

                if (CreateDialog.NewUnit.HaveFax)
                {
                    KeyValues.Add("@faxareacode", CreateDialog.NewUnit.Fax.AreaCode);
                    DataTypeList.Add(0);
                    KeyValues.Add("@faxnumber", CreateDialog.NewUnit.Fax.Number);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@faxareacode", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@faxnumber", DBNull.Value);
                    DataTypeList.Add(3);
                }
                if (CreateDialog.NewUnit.HaveAddress)
                {
                    KeyValues.Add("@country", CreateDialog.NewUnit.Location.Country);
                    DataTypeList.Add(0);
                    KeyValues.Add("@city", CreateDialog.NewUnit.Location.City);
                    DataTypeList.Add(0);
                    KeyValues.Add("@address", CreateDialog.NewUnit.Location.Address);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@country", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@city", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@address", DBNull.Value);
                    DataTypeList.Add(3);
                }
                vm.db.Manipulate(executeString, KeyValues, DataTypeList);

                string newp = CreateDialog.NewUnit.GetMergeProperty();

                executeString = "INSERT INTO myuniteditrecord(action,newproperty,user,name,tax,cel) VALUES(@action,@newproperty,@user,@name,@tax,@cel);";
                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();

                KeyValues.Add("@action", "新增單位");
                DataTypeList.Add(0);
                KeyValues.Add("@newproperty", newp);
                DataTypeList.Add(0);
                KeyValues.Add("@user", Application.Current.Properties["identity"].ToString());
                DataTypeList.Add(0);
                KeyValues.Add("@name", CreateDialog.NewUnit.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@tax", CreateDialog.NewUnit.HaveTax ? CreateDialog.NewUnit.Tax : String.Empty);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", CreateDialog.NewUnit.Cel);
                DataTypeList.Add(0);
                vm.db.Manipulate(executeString, KeyValues, DataTypeList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                e.Cancel = true;
            }
        }
        private void CreateResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            if (e.Error != null)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_CREATE_UNIT_UNKNOWN_FAILED));
            }
            else if (e.Cancelled)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_CREATE_UNIT_REPEAT_FAILED));
            }
            else
            {
                MainPage.PopupNews(Method.Function.GetDescription(CorrectCodeList.CORRECT_SELFCOMPANY_PAGE_CREATE_UNIT_SUCCESS));
            }
            ClearSearch_Click(null, null);
            Load();
        }
        private void OpenCreateDialog_Click(object sender, RoutedEventArgs e)
        {
            CreateDialog = new CreateUnitNotificationMessage();
            DialogHost.Show(CreateDialog, "MainDialog");
        }
        #endregion
        #region 刪除單位
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            storeUnit = ((FrameworkElement)sender).DataContext as Unit;
            var delDialog = new DeleteInfoNotificationMessage { Title = "刪除詢問", Message = String.Format("確定要刪除 {0} 嗎？", storeUnit.Name) };
            DialogHost.Show(delDialog, "MainDialog");
        }
        private void Delete_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                LoadingDialog = new LoadingInfoNotificationMessage { Title = "刪除中", Message = "請稍後" };
                await DialogHost.Show(LoadingDialog, "RootDialog");
            }));
            var bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(Deleting);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DeleteResult);
            bw.RunWorkerAsync();
        }
        private void Deleting(object sender, DoWorkEventArgs e)
        {
            string executeString = "DELETE FROM myunit WHERE name = @name AND tax = @tax AND cel = @cel;";
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            List<int> DataTypeList = new List<int>();

            KeyValues.Add("@name", storeUnit.Name);
            DataTypeList.Add(0);
            KeyValues.Add("@tax", storeUnit.Tax);
            DataTypeList.Add(0);
            KeyValues.Add("@cel", storeUnit.Cel);
            DataTypeList.Add(0);
            e.Result = vm.db.Manipulate(executeString, KeyValues, DataTypeList);
        }
        private void DeleteResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            if (e.Error != null)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_DELETE_UNIT_UNKNOWN_FAILED));
            }
            else if (e.Cancelled)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_DELETE_UNIT_UNKNOWN_FAILED));
            }
            else
            {
                if (Convert.ToBoolean(e.Result))
                    MainPage.PopupNews(Method.Function.GetDescription(CorrectCodeList.CORRECT_SELFCOMPANY_PAGE_DELETE_UNIT_SUCCESS));
                else
                    MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_DELETE_UNIT_WRITING_FAILED));
            }
            ClearSearch_Click(null, null);
            Load();
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
            DialogHost.Show(SearchDialog, "MainDialog");
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
        #region 編輯單位
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            var obj = ((FrameworkElement)sender).DataContext as Unit;
            EditDialog = new EditNotificationMessage(obj);
            DialogHost.Show(EditDialog, "MainDialog");
        }
        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            int errorcount = vm.CalcErrorCount(EditDialog.Edit);
            if (errorcount != 0)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_EDIT_UNIT_VERIFY_INPUT_FAILED));
                return;
            }
            else
            {
                DialogHost.CloseDialogCommand.Execute(null, null);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                    LoadingDialog = new LoadingInfoNotificationMessage { Title = "編輯中", Message = "請稍後" };
                    await DialogHost.Show(LoadingDialog, "RootDialog");
                }));
                var bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(Editing);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EditResult);
                bw.RunWorkerAsync();
            }
        }
        private void Editing(object sender, DoWorkEventArgs e)
        {
            if (Method.Function.EditUnitHaveRepeat(vm.TotalList, EditDialog.Edit, EditDialog.GetOriginal()))
            {
                e.Cancel = true;
                return;
            }
            try
            {
                string execute = "UPDATE myunit SET name = @name , tax = @tax , cel = @cel , telareacode = @telareacode , telnumber = @telnumber , faxareacode = @faxareacode , faxnumber = @faxnumber , country = @country , city = @city , address = @address WHERE name = @keyname AND tax = @keytax AND cel = @keycel ;";
                Dictionary<string, object> KeyValues = new Dictionary<string, object>();
                List<int> DataTypeList = new List<int>();

                KeyValues.Add("@name", EditDialog.Edit.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@tax", EditDialog.Edit.HaveTax ? EditDialog.Edit.Tax : String.Empty);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", EditDialog.Edit.Cel);
                DataTypeList.Add(0);

                if (EditDialog.Edit.HaveTel)
                {
                    KeyValues.Add("@telareacode", EditDialog.Edit.Tel.AreaCode);
                    DataTypeList.Add(0);
                    KeyValues.Add("@telnumber", EditDialog.Edit.Tel.Number);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@telareacode", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@telnumber", DBNull.Value);
                    DataTypeList.Add(3);
                }

                if (EditDialog.Edit.HaveFax)
                {
                    KeyValues.Add("@faxareacode", EditDialog.Edit.Fax.AreaCode);
                    DataTypeList.Add(0);
                    KeyValues.Add("@faxnumber", EditDialog.Edit.Fax.Number);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@faxareacode", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@faxnumber", DBNull.Value);
                    DataTypeList.Add(3);
                }
                if (EditDialog.Edit.HaveAddress)
                {
                    KeyValues.Add("@country", EditDialog.Edit.Location.Country);
                    DataTypeList.Add(0);
                    KeyValues.Add("@city", EditDialog.Edit.Location.City);
                    DataTypeList.Add(0);
                    KeyValues.Add("@address", EditDialog.Edit.Location.Address);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@country", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@city", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@address", DBNull.Value);
                    DataTypeList.Add(3);
                }
                CreateUnit Original = EditDialog.GetOriginal();
                KeyValues.Add("@keyname", Original.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@keytax", Original.Tax);
                DataTypeList.Add(0);
                KeyValues.Add("@keycel", Original.Cel);
                DataTypeList.Add(0);
                vm.db.Manipulate(execute, KeyValues, DataTypeList);

                string oldp = Original.GetMergeProperty();
                string newp = EditDialog.Edit.GetMergeProperty();

                execute = "INSERT INTO myuniteditrecord(action,newproperty,oldproperty,user,name,tax,cel) VALUES(@action,@newproperty,@oldproperty,@user,@name,@tax,@cel);";
                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();

                KeyValues.Add("@action", "編輯單位");
                DataTypeList.Add(0);
                KeyValues.Add("@newproperty", newp);
                DataTypeList.Add(0);
                KeyValues.Add("@oldproperty", oldp);
                DataTypeList.Add(0);
                KeyValues.Add("@user", Application.Current.Properties["identity"].ToString());
                DataTypeList.Add(0);
                KeyValues.Add("@name", EditDialog.Edit.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@tax", EditDialog.Edit.Tax);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", EditDialog.Edit.Cel);
                DataTypeList.Add(0);
                vm.db.Manipulate(execute, KeyValues, DataTypeList);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                e.Cancel = true;
            }
        }
        private void EditResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            if (e.Error != null)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_EDIT_UNIT_UNKNOWN_FAILED));
            }
            else if (e.Cancelled)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SELFCOMPANY_PAGE_EDIT_UNIT_REPEAT_FAILED));
            }
            else
            {
                MainPage.PopupNews(Method.Function.GetDescription(CorrectCodeList.CORRECT_SELFCOMPANY_PAGE_EDIT_UNIT_SUCCESS));
            }
            ClearSearch_Click(null, null);
            Load();
        }
        #endregion
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var source = ((FrameworkElement)sender).DataContext as Unit;
            var infoDialog = new UnitDetailNotificationMessage(source,0);
            DialogHost.Show(infoDialog, "MainDialog");
        }
    }
}
