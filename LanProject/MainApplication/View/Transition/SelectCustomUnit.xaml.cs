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
    /// SelectCustomUnit.xaml 的互動邏輯
    /// </summary>
    public partial class SelectCustomUnit : UserControl
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
        public SelectCustomUnit()
        {
            InitializeComponent();
            Loaded += SelectCustomUnit_Loaded;
        }
        private void SelectCustomUnit_Loaded(object sender, RoutedEventArgs e) => GetParent(this);
        private bool CheckHaveRepeat()
        {
            FormNotificationMessage obj = DataContext as FormNotificationMessage;
            var count = obj.NewForm.ThirdPage.TotalList.Count(vpk => (vpk.Name == PT.CreateDialog.NewForm.ThirdPage.Detail.Name) && (vpk.Cel == PT.CreateDialog.NewForm.ThirdPage.Detail.Cel) && (vpk.Tax == PT.CreateDialog.NewForm.ThirdPage.Detail.Tax));
            if (PT.CreateDialog.NewForm.ThirdPage.Create)
            {
                if (count == 0) return false;
                PT.CreateDialog.NewForm.ThirdPage.VerifyEnable = true;
                PT.CreateDialog.NewForm.ThirdPage.Allow = false;
                PT.CreateDialog.NewForm.ThirdPage.Message = "* 單位已存在 請試著點選已存在單位選取";
                return true;
            }
            else
            {
                if (count == 1)
                {
                    bool RT = (PT.CreateDialog.NewForm.ThirdPage.Original.Name == PT.CreateDialog.NewForm.ThirdPage.Detail.Name) &&
                        (PT.CreateDialog.NewForm.ThirdPage.Original.Cel == PT.CreateDialog.NewForm.ThirdPage.Detail.Cel) &&
                        (PT.CreateDialog.NewForm.ThirdPage.Original.Tax == PT.CreateDialog.NewForm.ThirdPage.Detail.Tax);
                    if (!RT)
                    {
                        PT.CreateDialog.NewForm.ThirdPage.VerifyEnable = true;
                        PT.CreateDialog.NewForm.ThirdPage.Allow = false;
                        PT.CreateDialog.NewForm.ThirdPage.Message = "* 單位已存在 請試著點選已存在單位選取";
                        return true;
                    }
                }
            }
            return false;
        }
        private bool Verify()
        {
            FormNotificationMessage obj = DataContext as FormNotificationMessage;
            if (obj.CalcCustomErrorCount() != 0)
            {
                PT.CreateDialog.NewForm.ThirdPage.VerifyEnable = true;
                PT.CreateDialog.NewForm.ThirdPage.Allow = false;
                PT.CreateDialog.NewForm.ThirdPage.Message = "* 欄位檢查有誤";
                return false;
            }
            else
            {
                PT.CreateDialog.NewForm.ThirdPage.VerifyEnable = false;
                PT.CreateDialog.NewForm.ThirdPage.Allow = true;
                PT.CreateDialog.NewForm.ThirdPage.Message = "* 已完成檢查";
                return true;
            }
        }
        private void Existedunit_Click(object sender, RoutedEventArgs e)
        {
            Window window = new Window { Title = "選擇他單位", WindowState = WindowState.Maximized };
            window.Content = new CustomUnitList();

            if (window.ShowDialog() == true)
            {
                var Receive = window.Content as CustomUnitList;
                if (Receive.Get() == null)
                {
                    PT.CreateDialog.NewForm.ThirdPage.Create = true;
                }
                else
                {
                    PT.CreateDialog.NewForm.ThirdPage.Detail = Receive.Get();
                    PT.CreateDialog.NewForm.ThirdPage.City = Method.Function.RefreshCity(PT.CreateDialog.NewForm.ThirdPage.Detail.Location.Country);
                    PT.CreateDialog.NewForm.ThirdPage.SetOriginal();
                }
            }
            else
            {
                PT.CreateDialog.NewForm.ThirdPage.Create = true;
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
