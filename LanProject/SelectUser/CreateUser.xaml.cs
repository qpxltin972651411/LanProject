using LanProject.SelectUser.Model;
using LanProject.SelectUser.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Threading;

namespace LanProject.SelectUser
{
    /// <summary>
    /// CreateUser.xaml 的互動邏輯
    /// </summary>
    public partial class CreateUser : Window
    {
        private CreateUserViewModel vm;
        private readonly string programFilesPath = Method.Configuration.ReadSetting("dataPath");
        private readonly string foldername = Method.Configuration.ReadSetting("appfoldername");
        private readonly string userimagesfoldername = Method.Configuration.ReadSetting("userimagesfoldername");
        public CreateUser()
        {
            InitializeComponent();
            vm = new CreateUserViewModel();
            DataContext = vm;
            vm.PropertyChanged += Vm_PropertyChanged;
        }
        private void Vm_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EnablePassword")
            {
                if (!vm.EnablePassword)
                    pwd.Clear();
            }
            if (e.PropertyName == "EnableimagePath")
            {
                if (!vm.EnableimagePath)
                {
                    vm.imagePath = String.Empty;
                    SHOW.ImageSource = null;
                }
            }
            vm.NickName = vm.NickName.Trim();
            if (String.IsNullOrEmpty(vm.NickName))
            {
                vm.EnableSubmit = false;
                return;
            }
            if (vm.EnablePassword)
            {
                if (String.IsNullOrEmpty(Method.SecureConverter.SecureStringToString(vm.Password)))
                {
                    vm.EnableSubmit = false;
                    return;
                }
            }
            if (vm.EnableimagePath)
            {
                if (String.IsNullOrEmpty(vm.imagePath))
                {
                    vm.EnableSubmit = false;
                    return;
                }
            }
            vm.EnableSubmit = true;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            var obj = sender as PasswordBox;
            vm.Password = obj.SecurePassword;
        }
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
            dlg.Filter = "Image files (*.jpg,*.png,*.jpeg)|*.jpg;*.png;*.jpeg";
            dlg.RestoreDirectory = true;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                vm.imagePath = dlg.FileName;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(dlg.FileName);
                bitmap.EndInit();
                SHOW.ImageSource = bitmap;
            }
        }
        private void CloseWindow()
        {
            Hide();
            //var curr = Application.Current.MainWindow;
            MainWindow _next = new MainWindow();
            System.Windows.Application.Current.MainWindow = _next;
            _next.Show();
            Close();
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (vm.HaveRepeat())
            {
                DialogHostModel message = new DialogHostModel
                {
                    Title = "錯誤",
                    Message = "名稱有重複，請重新命名",
                };
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                    await DialogHost.Show(message, "RootDialog2");
                }));
            }
            else
            {
                const string insertIntotbl1TableString = "INSERT INTO userlist (nickname, isLocked, password, imagePath) VALUES (@nickname, @isLocked, @password, @imagePath);";
                Dictionary<string, object> KeyValues = new Dictionary<string, object>();
                List<int> DataTypeList = new List<int>();

                KeyValues.Add("@nickname", vm.NickName);
                DataTypeList.Add(0);

                KeyValues.Add("@isLocked", vm.EnablePassword);
                DataTypeList.Add(1);

                if (vm.EnablePassword)
                {
                    KeyValues.Add("@password", Method.Generate.Base64Encode(Method.SecureConverter.SecureStringToString(vm.Password)));
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@password", DBNull.Value);
                    DataTypeList.Add(3);
                }

                if (vm.EnableimagePath)
                {
                    var newfilename = MainApplication.Method.Function.RandomString(20) + System.IO.Path.GetExtension(vm.imagePath);
                    KeyValues.Add("@imagePath", newfilename);
                    DataTypeList.Add(0);
                    string targetPath = System.IO.Path.Combine(programFilesPath, foldername, userimagesfoldername);
                    Directory.CreateDirectory(targetPath);
                    File.Copy(vm.imagePath, System.IO.Path.Combine(targetPath, newfilename), true);
                }
                else
                {
                    KeyValues.Add("@imagePath", DBNull.Value);
                    DataTypeList.Add(3);
                }
                vm.db.Manipulate(insertIntotbl1TableString, KeyValues, DataTypeList);

                DialogHostModel message = new DialogHostModel
                {
                    Title = "成功",
                    Message = "新增成功",
                };
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                    var reply = await DialogHost.Show(message, "RootDialog2");
                    CloseWindow();
                }));
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            CloseWindow();
        }
    }
}
