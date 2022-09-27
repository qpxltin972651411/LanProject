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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LanProject.InitSetup.Initialization.Pages
{
    /// <summary>
    /// Signup.xaml 的互動邏輯
    /// </summary>
    public partial class Signup : Page
    {
        static IDictionary<string, Brush> _boxesBrush = new Dictionary<string, Brush>();
        public string acc { get; set; }
        public string passwd { get; set; }
        public SelectPath _firstpage;
        public Signup()
        {
            InitializeComponent();
        }
        public Signup(SelectPath p)
        {
            InitializeComponent();
            _firstpage = p;
            if (account.BorderBrush == null)
                return;
            _boxesBrush[account.Name] = account.BorderBrush.Clone();
            _boxesBrush[pwd.Name] = pwd.BorderBrush.Clone();
            _boxesBrush[pwdconf.Name] = pwd.BorderBrush.Clone();
        }
        private void Prev_Click(object sender, RoutedEventArgs e) => NavigationService.Navigate(_firstpage);
        private void FinishBtn_Click(object sender, RoutedEventArgs e)
        {
            var result = Method.StringExtensions.OnlyAcceptLetterAndNumber(account);
            if (result != String.Empty)
            {
                message.Text = result;
                message.Foreground = new SolidColorBrush(Colors.Red);
                account.BorderBrush = new SolidColorBrush(Colors.Red);
                if (!(popup1.working))
                    popup1.IsOpen = true;
                Storyboard myStoryboard = (Storyboard)account.Resources["TestStoryboard1"];
                Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as DoubleAnimationUsingKeyFrames, account);
                myStoryboard.Begin();
                return;
            }
            account.BorderBrush = _boxesBrush[account.Name];

            if (pwd.Password.ToString() == String.Empty)
            {
                message.Text = " * 密碼欄位不得為空";
                message.Foreground = new SolidColorBrush(Colors.Red);
                pwd.BorderBrush = new SolidColorBrush(Colors.Red);
                if (!(popup1.working))
                    popup1.IsOpen = true;
                Storyboard myStoryboard = (Storyboard)pwd.Resources["TestStoryboard2"];
                Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as DoubleAnimationUsingKeyFrames, pwd);
                myStoryboard.Begin();
                return;
            }
            pwd.BorderBrush = _boxesBrush[pwd.Name];

            if (pwdconf.Password.ToString() != pwd.Password.ToString())
            {
                message.Text = " * 重複密碼與前不同";
                message.Foreground = new SolidColorBrush(Colors.Red);
                pwdconf.BorderBrush = new SolidColorBrush(Colors.Red);
                if (!(popup1.working))
                    popup1.IsOpen = true;
                Storyboard myStoryboard = (Storyboard)pwdconf.Resources["TestStoryboard3"];
                Storyboard.SetTarget(myStoryboard.Children.ElementAt(0) as DoubleAnimationUsingKeyFrames, pwdconf);
                myStoryboard.Begin();
                return;
            }
            pwdconf.BorderBrush = _boxesBrush[pwdconf.Name];
            acc = account.Text;
            passwd = pwd.Password.ToString();
            NavigationService.Navigate(new Finish(this));
        }
    }
}
