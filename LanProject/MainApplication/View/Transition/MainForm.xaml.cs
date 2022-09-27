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
    /// MainForm.xaml 的互動邏輯
    /// </summary>
    public partial class MainForm : UserControl
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
        private void MainForm_Loaded(object sender, RoutedEventArgs e) => GetParent(this);
        public MainForm()
        {
            InitializeComponent();
            Loaded += MainForm_Loaded;
        }
        private void deleteRow_Click(object sender, RoutedEventArgs e)
        {
            Product target = ((FrameworkElement)sender).DataContext as Product;
            FormNotificationMessage obj = DataContext as FormNotificationMessage;
            obj.DeleteOneRowToProductList(target);

        }

        private void createRow_Click(object sender, RoutedEventArgs e)
        {
            FormNotificationMessage obj = DataContext as FormNotificationMessage;
            obj.InsertNewRowToProductList();
        }

        private void cloneRow_Click(object sender, RoutedEventArgs e)
        {
            Product _clone = ((FrameworkElement)sender).DataContext as Product;
            FormNotificationMessage obj = DataContext as FormNotificationMessage;
            obj.CloneOneRowToProductList(_clone);
        }
        private void next_Click(object sender, RoutedEventArgs e)
        {
            next.Command = null;
            FormNotificationMessage obj = DataContext as FormNotificationMessage;
            if (obj.CalcFormErrorCount() != 0)
            {
                obj.NewForm.FourthPage.Error = "* 欄位格式有誤";
                //obj.NewForm.Step4 = false;
                return;
            }
            if (obj.NewForm.FourthPage.ProductList.Count == 0)
            {
                obj.NewForm.FourthPage.Error = "* 品項需至少1項以上";
                //obj.NewForm.Step4 = false;
                return;
            }
            obj.NewForm.FourthPage.Error = String.Empty;
            obj.NewForm.FourthPage.Allow = true;
            next.Command = Transitioner.MoveNextCommand;
            return;
        }
    }
}
