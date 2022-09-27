using LanProject.Domain;
using LanProject.MainApplication.Model;
using LanProject.MainApplication.ViewModel;
using LanProject.Method;
using LanProject.SelectUser.Model;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace LanProject.MainApplication
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly string programFilesPath = Configuration.ReadSetting("dataPath");
        private readonly string foldername = Configuration.ReadSetting("appfoldername");
        private readonly string userimagesfoldername = Configuration.ReadSetting("userimagesfoldername");
        private MainViewModel vm { get; }
        private SqliteDatabase db { get; set; }
        public BitmapImage ConvertBMP(Bitmap src)
        {
            MemoryStream ms = new MemoryStream();
            ((System.Drawing.Bitmap)src).Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
            BitmapImage image = new BitmapImage();
            image.BeginInit();
            ms.Seek(0, SeekOrigin.Begin);
            image.StreamSource = ms;
            image.EndInit();
            return image;
        }
        private void updateUserIcon()
        {
            if (vm.CurrentUser.Hasimage)
            {
                BitmapImage image = new BitmapImage(new Uri(vm.CurrentUser.Imagepath, UriKind.Absolute));
                pfile.Source = image;
            }
            else
            {
                Bitmap Bmp = new Bitmap(100, 100);
                using (Graphics gfx = Graphics.FromImage(Bmp))
                {
                    System.Drawing.Brush b = new SolidBrush((System.Drawing.Color)new System.Drawing.ColorConverter().ConvertFromString(new BrushConverter().ConvertToString(vm.CurrentUser.Background)));
                    gfx.FillRectangle(b, 0, 0, 100, 100);
                }
                pfile.Source = ConvertBMP(Bmp);
            }
        }
        public MainWindow()
        {
            db = new SqliteDatabase();
            InitializeComponent();
            Closing += MainWindow_Closing;
            vm = new MainViewModel(Application.Current.Properties);
            DataContext = vm;
            updateUserIcon();
        }
        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e) => vm.TimeTickThread.Abort();
        public void PopupNews(string message)
        {
            Task.Factory.StartNew(() => Thread.Sleep(200)).ContinueWith(t =>
            {
                //note you can use the message queue from any thread, but just for the demo here we 
                //need to get the message queue from the snackbar, so need to be on the dispatcher
                MainSnackbar.MessageQueue?.Enqueue(message, "關閉", null);
            }, TaskScheduler.FromCurrentSynchronizationContext());
        }
        private void DemoItemsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //until we had a StaysOpen glag to Drawer, this will help with scroll bars
            var dependencyObject = Mouse.Captured as DependencyObject;

            while (dependencyObject != null)
            {
                if (dependencyObject is ScrollBar) return;
                dependencyObject = VisualTreeHelper.GetParent(dependencyObject);
            }

            Model.Page currentPage = (DemoItemsListBox.SelectedItem as LanProject.MainApplication.Model.Page);
            if ((vm.SelectedIndex < 0) || (currentPage == null))
            {
                //PopupNews("錯誤");
                return;
            }
            //PopupNews(String.Format("已切換到 {0}",vm.SelectedItem.DisplayName));
        }
        private void OnSelectedItemChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            DialogHost.CloseDialogCommand.Execute(null, null);
            MainScrollViewer.ScrollToHome();
        }

        private void logout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                YesNoDialogHostModel confirm = new YesNoDialogHostModel
                {
                    Title = "問題",
                    Message = String.Format("{0} 確定要登出嗎?", Application.Current.Properties["identity"].ToString()),
                };
                bool AgainResult = Convert.ToBoolean((await DialogHost.Show(confirm, "RootDialog")).ToString());
                if (AgainResult)
                    Logout();
            }));
        }
        private void Logout()
        {
            vm.TimeTickThread.Abort();
            ClearPropertys();
            Login.MainWindow _next = new Login.MainWindow();
            Application.Current.MainWindow = _next;
            _next.Show();
            Close();
        }
        private void ClearPropertys()
        {
            Application.Current.Properties["identity"] = null;
            Application.Current.Properties["imagepath"] = null;
            Application.Current.Properties["Hasimage"] = null;
            Application.Current.Properties["Background"] = null;
            Application.Current.Properties["email"] = null;
        }

        private void pfile_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            dlg.Filter = "Image files (*.jpg,*.png,*.jpeg)|*.jpg;*.png;*.jpeg";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                const string updateIntotbl1TableString = "UPDATE userlist SET imagePath = @imagePath WHERE nickname = @nickname;";
                Dictionary<string, object> KeyValues = new Dictionary<string, object>();
                List<int> DataTypeList = new List<int>();
                var newfilename = Method.Function.RandomString(20) + Path.GetExtension(dlg.FileName);
                KeyValues.Add("@imagePath", newfilename);
                DataTypeList.Add(0);
                KeyValues.Add("@nickname", vm.CurrentUser.Nickname);
                DataTypeList.Add(0);
                db.Manipulate(updateIntotbl1TableString, KeyValues, DataTypeList);

                string targetPath = Path.Combine(programFilesPath, foldername, userimagesfoldername);
                Directory.CreateDirectory(targetPath);
                File.Copy(dlg.FileName, Path.Combine(targetPath, newfilename), true);

                vm.CurrentUser.Hasimage = true;
                vm.CurrentUser.Imagepath = Path.Combine(targetPath, newfilename);
                updateUserIcon();
                DialogHostModel message = new DialogHostModel
                {
                    Title = "成功",
                    Message = "編輯成功",
                };
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                    await DialogHost.Show(message, "RootDialog");
                }));
            }
        }
    }
}
