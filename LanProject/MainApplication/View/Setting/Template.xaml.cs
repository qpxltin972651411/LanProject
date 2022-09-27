using FontAwesome.WPF;
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
    /// Template.xaml 的互動邏輯
    /// </summary>
    public partial class Template : UserControl
    {
        public static readonly RoutedEvent TemplateClickEvent =
                  EventManager.RegisterRoutedEvent("TemplateClick", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(Template));

        public event RoutedEventHandler ElementClick
        {
            add { AddHandler(TemplateClickEvent, value); }
            remove { RemoveHandler(TemplateClickEvent, value); }
        }
        void Button_Click(object sender, RoutedEventArgs e) => RaiseEvent(new RoutedEventArgs(TemplateClickEvent));

        public FontAwesomeIcon Icon
        {
            get { return (FontAwesomeIcon)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }
        public static readonly DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(FontAwesomeIcon), typeof(Template));
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("Title", typeof(string), typeof(Template));
        public bool IsActive
        {
            get { return (bool)GetValue(IsActiveProperty); }
            set { SetValue(IsActiveProperty, value); }
        }
        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register("IsActive", typeof(bool), typeof(Template));

        public Template()
        {
            InitializeComponent();
        }
    }
}
