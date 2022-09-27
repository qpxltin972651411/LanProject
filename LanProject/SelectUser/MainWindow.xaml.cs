using LanProject.MainApplication.Model;
using LanProject.SelectUser.Model;
using LanProject.SelectUser.ViewModel;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LanProject.SelectUser
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private MainWindowViewModel vm;
        private enterPasswordDialogHost pop = null;
        private PasswordBox TargetPassword = null;
        public MainWindow()
        {
            InitializeComponent();
            vm = new MainWindowViewModel();
            DataContext = vm;
        }
        private void Successful(UserList obj)
        {
            Application.Current.Properties["identity"] = obj.Nickname;
            Application.Current.Properties["imagepath"] = obj.Imagepath;
            Application.Current.Properties["Hasimage"] = obj.Hasimage;
            Application.Current.Properties["Background"] = obj.Background;
            Application.Current.Properties["email"] = obj.Email;
            Application.Current.Properties["password"] = obj.IsLocked ? obj.Password : String.Empty;
            if (DialogHost.IsDialogOpen("RootDialog2")) DialogHost.Close("RootDialog2");

            MainApplication.MainWindow _next = new MainApplication.MainWindow();
            Application.Current.MainWindow = _next;
            _next.Show();
            Close();
        }
        private void ToggleButton_Click(object sender, RoutedEventArgs e)
        {
            UserList obj = ((FrameworkElement)sender).DataContext as UserList;
            if (obj.IsLocked)
            {
                pop = new enterPasswordDialogHost(obj.Nickname);
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                    bool result = Convert.ToBoolean((await DialogHost.Show(pop, "RootDialog2")).ToString());
                    if (result)
                    {
                        if (Method.Generate.Base64Decode(obj.Password) == Method.SecureConverter.SecureStringToString(pop.Password))
                        {
                            YesNoDialogHostModel confirm = new YesNoDialogHostModel
                            {
                                Title = "問題",
                                Message = String.Format("確定選擇 {0} 嗎?", obj.Nickname),
                            };
                            var answer = DialogHost.Show(confirm, "RootDialog2");
                            await answer;
                            bool AgainResult = Convert.ToBoolean(answer.Result.ToString());
                            if (AgainResult)
                            {
                                DialogHostModel message = new DialogHostModel
                                {
                                    Title = "成功",
                                    Message = "歡迎",
                                };
                                var reply = await DialogHost.Show(message, "RootDialog2");
                                Successful(obj);
                            }
                            else
                            {
                                DialogHostModel message = new DialogHostModel
                                {
                                    Title = "取消",
                                    Message = "已取消",
                                };
                                await DialogHost.Show(message, "RootDialog2");
                            }
                        }
                        else
                        {
                            DialogHostModel message = new DialogHostModel
                            {
                                Title = "錯誤",
                                Message = "驗證碼有誤",
                            };
                            await DialogHost.Show(message, "RootDialog2");
                        }
                    }
                    ToggleButton target = sender as ToggleButton;
                    target.IsChecked = !target.IsChecked;
                    if (TargetPassword != null)
                        TargetPassword.Clear();
                }));
            }
            else
            {
                YesNoDialogHostModel confirm = new YesNoDialogHostModel
                {
                    Title = "問題",
                    Message = String.Format("確定選擇 {0} 嗎?", obj.Nickname),
                };
                Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                    var answer = DialogHost.Show(confirm, "RootDialog2");
                    await answer;
                    bool AgainResult = Convert.ToBoolean(answer.Result.ToString());
                    if (AgainResult)
                    {
                        DialogHostModel message = new DialogHostModel
                        {
                            Title = "成功",
                            Message = "歡迎",
                        };
                        var reply = await DialogHost.Show(message, "RootDialog2");
                        Successful(obj);
                    }
                    else
                    {
                        DialogHostModel message = new DialogHostModel
                        {
                            Title = "取消",
                            Message = "已取消",
                        };
                        await DialogHost.Show(message, "RootDialog2");
                    }

                    ToggleButton target = sender as ToggleButton;
                    target.IsChecked = !target.IsChecked;
                }));
            }
        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            TargetPassword = sender as PasswordBox;
            if (pop != null)
                pop.Password = ((PasswordBox)sender).SecurePassword;
        }

        private void CreateUser_Click(object sender, RoutedEventArgs e)
        {
            Hide();
            //var curr = Application.Current.MainWindow;
            CreateUser _next = new CreateUser();
            Application.Current.MainWindow = _next;
            _next.Show();
            Close();
        }
    }
}
