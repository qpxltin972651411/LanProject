using LanProject.MainApplication.Model;
using MaterialDesignThemes.Wpf.Transitions;
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

namespace LanProject.MainApplication.View.Transition
{
    /// <summary>
    /// SelectMyUnit.xaml 的互動邏輯
    /// </summary>
    public partial class SelectMyUnit : UserControl
    {
        private Form PT { get; set; }
        private void GetParent(Visual v)
        {
            PT = null;
            while (v != null)
            {
                v = VisualTreeHelper.GetParent(v) as Visual;
                if (v is Form)
                    break;
            }
            PT = v as Form;
        }
        public SelectMyUnit()
        {
            InitializeComponent();
            Loaded += SelectMyUnit_Loaded;
        }
        private void SelectMyUnit_Loaded(object sender, RoutedEventArgs e) => GetParent(this);
        private bool CheckHaveRepeat()
        {
            FormNotificationMessage obj = DataContext as FormNotificationMessage;
            var count = obj.NewForm.SecondPage.TotalList.Count(vpk => (vpk.Name == PT.CreateDialog.NewForm.SecondPage.Detail.Name) && (vpk.Cel == PT.CreateDialog.NewForm.SecondPage.Detail.Cel) && (vpk.Tax == PT.CreateDialog.NewForm.SecondPage.Detail.Tax));
            if (PT.CreateDialog.NewForm.SecondPage.Create)
            {
                if (count == 0) return false;
                PT.CreateDialog.NewForm.SecondPage.VerifyEnable = true;
                PT.CreateDialog.NewForm.SecondPage.Allow = false;
                PT.CreateDialog.NewForm.SecondPage.Message = "* 單位已存在 請試著點選已存在單位選取";
                return true;
            }
            else
            {
                if (count == 1)
                {
                    bool RT = (PT.CreateDialog.NewForm.SecondPage.Original.Name == PT.CreateDialog.NewForm.SecondPage.Detail.Name) &&
                        (PT.CreateDialog.NewForm.SecondPage.Original.Cel == PT.CreateDialog.NewForm.SecondPage.Detail.Cel) &&
                        (PT.CreateDialog.NewForm.SecondPage.Original.Tax == PT.CreateDialog.NewForm.SecondPage.Detail.Tax);
                    if (!RT)
                    {
                        PT.CreateDialog.NewForm.SecondPage.VerifyEnable = true;
                        PT.CreateDialog.NewForm.SecondPage.Allow = false;
                        PT.CreateDialog.NewForm.SecondPage.Message = "* 單位已存在 請試著點選已存在單位選取";
                        return true;
                    }
                }
            }
            return false;
        }
        private bool Verify()
        {
            FormNotificationMessage obj = DataContext as FormNotificationMessage;
            if (obj.CalcMySelfErrorCount() != 0)
            {
                PT.CreateDialog.NewForm.SecondPage.VerifyEnable = true;
                PT.CreateDialog.NewForm.SecondPage.Allow = false;
                PT.CreateDialog.NewForm.SecondPage.Message = "* 欄位檢查有誤";
                return false;
            }
            else
            {
                PT.CreateDialog.NewForm.SecondPage.VerifyEnable = false;
                PT.CreateDialog.NewForm.SecondPage.Allow = true;
                PT.CreateDialog.NewForm.SecondPage.Message = "* 已完成檢查";
                return true;
            }
        }
        private void Existedunit_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window { Title = "選擇自單位", WindowState = WindowState.Maximized };
            window.Content = new MyUnitList();

            if (window.ShowDialog() == true)
            {
                var Receive = window.Content as MyUnitList;
                if (Receive.Get() == null)
                {
                    PT.CreateDialog.NewForm.SecondPage.Create = true;
                }
                else
                {
                    PT.CreateDialog.NewForm.SecondPage.Detail = Receive.Get();
                    PT.CreateDialog.NewForm.SecondPage.City = Method.Function.RefreshCity(PT.CreateDialog.NewForm.SecondPage.Detail.Location.Country);
                    PT.CreateDialog.NewForm.SecondPage.SetOriginal();
                }
            }
            else
            {
                PT.CreateDialog.NewForm.SecondPage.Create = true;
            }
        }
        private void verify_Click(object sender, RoutedEventArgs e)
        {
            if (Verify())
                CheckHaveRepeat();
        }
        private void next_Click(object sender, RoutedEventArgs e)
        {
            if (Verify())
            {
                if (!CheckHaveRepeat())
                {
                    next.Command = Transitioner.MoveNextCommand;
                    return;
                }
            }
            next.Command = null;
        }
    }
}
