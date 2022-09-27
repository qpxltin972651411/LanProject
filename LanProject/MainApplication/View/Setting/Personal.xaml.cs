using LanProject.MainApplication.ViewModel;
using LanProject.SelectUser.Model;
using MaterialDesignThemes.Wpf;
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
    /// Personal.xaml 的互動邏輯
    /// </summary>
    public partial class Personal : UserControl
    {
        private PersonalViewModel vm { get; set; }
        public Personal()
        {
            InitializeComponent();
            vm = new PersonalViewModel(Application.Current.Properties);
            DataContext = vm;
        }
        private bool CanSubmit()
        {
            if (vm.NewNickname == String.Empty)
            {
                if (vm.NoEdit)
                {
                    if (mainpwd.Password == String.Empty)
                    {
                        return false;
                    }
                }
                else if (vm.EnablePassword)
                {
                    if (pwd.Password == String.Empty)
                    {
                        if (mainpwd.Password == String.Empty)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogHostModel message;
            if (!CanSubmit())
            {
                message = new DialogHostModel
                {
                    Title = "無法送出",
                    Message = "未填寫修改資訊",
                };
                DialogHost.Show(message, "RootDialog");
                return;
            }
            message = new DialogHostModel
            {
                Title = "無法送出",
                Message = "可以",
            };
            DialogHost.Show(message, "RootDialog");
        }
    }
}
