using LanProject.MainApplication.Map;
using LanProject.MainApplication.Model;
using LanProject.MainApplication.ViewModel;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LanProject.MainApplication.View
{
    /// <summary>
    /// Form.xaml 的互動邏輯
    /// </summary>
    public partial class Form : UserControl
    {
        private FormViewModel vm { get; set; }
        public MainWindow MainPage { get; set; }
        public LoadingInfoNotificationMessage LoadingDialog { get; set; }
        public CreateFormNotificationMessage CreateDialog { get; set; }
        public FormSearchNotificationMessage SearchDialog { get; set; }
        private Model.Form storeForm { get; set; }
        public Form()
        {
            InitializeComponent();
            Loaded += Form_Loaded;
            vm = new FormViewModel();
            DataContext = vm;
        }
        private void PrevButton_Click(object sender, RoutedEventArgs e) => vm.PrevPage();
        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (!vm.NextPage()) MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_FORM_PAGE_NEXT_PAGE_FAILED));
        }
        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            MainPage = Method.Function.GetParent(this);
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
        #region 刪除表單
        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            storeForm = ((FrameworkElement)sender).DataContext as Model.Form;
            var delDialog = new DeleteInfoNotificationMessage { Title = "問題", Message = String.Format("確定要刪除表單 {0} 嗎？", storeForm.ID) };
            DialogHost.Show(delDialog, "MainDialog");
        }
        private void Delete_Click(object sender, RoutedEventArgs e) => Delete();
        public void Delete()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                LoadingDialog = new LoadingInfoNotificationMessage { Title = "刪除表單中", Message = "請稍後" };
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
            string executeString = "DELETE FROM formtable WHERE id = @id;";
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            List<int> DataTypeList = new List<int>();
            KeyValues.Add("@id", storeForm.ID);
            DataTypeList.Add(0);
            e.Result = vm.db.Manipulate(executeString, KeyValues, DataTypeList);
        }
        private void DeleteResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            if (e.Error != null)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_FORM_PAGE_DELETE_UNIT_UNKNOWN_FAILED));
            }
            else if (e.Cancelled)
            {
                MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_FORM_PAGE_DELETE_UNIT_UNKNOWN_FAILED));
            }
            else
            {
                if (Convert.ToBoolean(e.Result))
                    MainPage.PopupNews(Method.Function.GetDescription(CorrectCodeList.CORRECT_FORM_PAGE_DELETE_FORM_SUCCESS));
                else
                    MainPage.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_FORM_PAGE_DELETE_UNIT_WRITING_FAILED));
            }
            Load();
        }
        #endregion
        #region 新增表單
        private void OpenCreateDialog_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                LoadingDialog = new LoadingInfoNotificationMessage();
                await DialogHost.Show(LoadingDialog, "RootDialog");
            }));
            var bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(OpenCreateDialog);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OpenCreateDialogResult);
            bw.RunWorkerAsync();
        }
        private void OpenCreateDialog(object sender, DoWorkEventArgs e) => CreateDialog = new CreateFormNotificationMessage();
        private void OpenCreateDialogResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                await DialogHost.Show(CreateDialog, "MainDialog");
                Load();
            }));
        }
        #endregion
        #region 編輯表單
        private void EditButton_Click(object sender, RoutedEventArgs e)
        {
            storeForm = ((FrameworkElement)sender).DataContext as Model.Form;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                LoadingDialog = new LoadingInfoNotificationMessage();
                await DialogHost.Show(LoadingDialog, "RootDialog");
            }));
            var bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(OpenEditDialog);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(OpenEditDialogResult);
            bw.RunWorkerAsync();
        }
        private void OpenEditDialog(object sender, DoWorkEventArgs e) => CreateDialog = new EditFormNotificationMessage(storeForm);
        private void OpenEditDialogResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                await DialogHost.Show(CreateDialog, "MainDialog");
                Load();
            }));
        }
        #endregion


        #region 搜尋表單
        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            if (SearchDialog == null )SearchDialog = new FormSearchNotificationMessage(vm);
            DialogHost.Show(SearchDialog, "MainDialog");
        }
        #endregion
        
        private void InfoButton_Click(object sender, RoutedEventArgs e)
        {
            var infoform = ((FrameworkElement)sender).DataContext as Model.Form;
            var infoDialog = new FormDetailNotificationMessage(infoform, vm.db);
            DialogHost.Show(infoDialog, "MainDialog");
        }
        public Task<bool> GetPDF(string filename)
        {
            string targetpdf = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename),
                String.Format("{0}.pdf", System.IO.Path.GetFileNameWithoutExtension(filename)));
            try
            {
                Spire.Xls.Workbook wb = new Spire.Xls.Workbook();
                wb.LoadFromFile(filename);
                wb.SaveToFile(targetpdf, Spire.Xls.FileFormat.PDF);
                return Task.FromResult(true);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return Task.FromResult(false);
            }
        }
        private void GenerateXLSX(object sender, DoWorkEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                LoadingDialog = new LoadingInfoNotificationMessage();
                await DialogHost.Show(LoadingDialog, "RootDialog");
            }));
            string filepath = Method.Output.Excel(storeForm, fillpage, producter ? Application.Current.Properties["identity"].ToString() : String.Empty);
            if (filepath == null) return;
            GetPDF(String.Format("{0}.xlsx", filepath));
            e.Result = String.Format("{0}.pdf", filepath);
        }
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (DialogHost.IsDialogOpen("MainDialog")) DialogHost.Close("MainDialog");
        }
        bool producter, fillpage;
        private void previewButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                YesNoCancelDialogHostModel confirm = new YesNoCancelDialogHostModel
                {
                    Title = "問題",
                    Message = "是否印出製表人字樣？"
                };
                var Result = await DialogHost.Show(confirm, "RootDialog");
                if (Result == null)
                    return;
                producter = Convert.ToBoolean(Result);

                confirm = new YesNoCancelDialogHostModel
                {
                    Title = "問題",
                    Message = "版面印出當不足時是否填滿？"
                };
                Result = await DialogHost.Show(confirm, "RootDialog");
                if (Result == null)
                    return;
                fillpage = Convert.ToBoolean(Result);


                storeForm = ((FrameworkElement)sender).DataContext as Model.Form;
                var bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(GenerateXLSX);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(GenerateXLSXResult);
                bw.RunWorkerAsync();
            }));
        }
        private void GenerateXLSXResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            var showup = new PDFViewerNotificationMessage(String.Format("file:///{0}", e.Result.ToString()));
            DialogHost.Show(showup, "MainDialog");
        }
        private void downloadButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                YesNoCancelDialogHostModel confirm = new YesNoCancelDialogHostModel
                {
                    Title = "問題",
                    Message = "是否印出製表人字樣？"
                };
                var Result = await DialogHost.Show(confirm, "RootDialog");
                if (Result == null)
                    return;
                producter = Convert.ToBoolean(Result);

                confirm = new YesNoCancelDialogHostModel
                {
                    Title = "問題",
                    Message = "版面印出當不足時是否填滿？"
                };
                Result = await DialogHost.Show(confirm, "RootDialog");
                if (Result == null)
                    return;
                fillpage = Convert.ToBoolean(Result);

                storeForm = ((FrameworkElement)sender).DataContext as Model.Form;
                var bw = new BackgroundWorker();
                bw.WorkerSupportsCancellation = true;
                bw.DoWork += new DoWorkEventHandler(GenerateXLSX);
                bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(DownloadResult);
                bw.RunWorkerAsync();
            }));
        }
        private void DownloadResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PDF File|*.pdf";
            try
            {
                if (saveFileDialog.ShowDialog() == true)
                {
                    if (File.Exists(saveFileDialog.FileName))
                        File.Delete(saveFileDialog.FileName);
                    File.Move(e.Result.ToString(), saveFileDialog.FileName);

                    MainPage.PopupNews("已下載檔案");
                    Process.Start(System.IO.Path.GetDirectoryName(saveFileDialog.FileName));
                }
                else
                {
                    MainPage.PopupNews("已取消");
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ClearSearch_Click(object sender, RoutedEventArgs e)
        {
            SearchDialog.Clear();
            //var noti = new NotificationMessage { Title = "訊息", Message = "已清除" };
            //DialogHost.Show(noti, "RootDialog");
            /*if (!SearchDialog.DateFilter.Before && !SearchDialog.DateFilter.Range && !SearchDialog.DateFilter.After && !SearchDialog.TypeFilter.T1 && !SearchDialog.TypeFilter.T2 && 
                !SearchDialog.LocationFilter.Have && !SearchDialog.My.Have && !SearchDialog.Custom.Have && !SearchDialog.UserFilter.Have)
            { } else { }*/
        }

        private void DatePicker_Loaded(object sender, RoutedEventArgs e)
        {
            var dp = sender as DatePicker;
            dp.Language = XmlLanguage.GetLanguage("zh-TW");
        }
    }
}
