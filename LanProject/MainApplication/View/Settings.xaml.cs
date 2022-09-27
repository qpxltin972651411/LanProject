using LanProject.MainApplication.Map;
using LanProject.MainApplication.Model;
using LanProject.MainApplication.View.Setting;
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
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LanProject.MainApplication.View
{
    /// <summary>
    /// Settings.xaml 的互動邏輯
    /// </summary>
    /// 
    public enum SettingPages
    {
        Personal, Note
    }
    public partial class Settings : UserControl
    {
        private Personal personalPage = new Personal();
        private Note notePage = new Note();
        private Container containerPage = new Container();
        private RichTextBox Editor = null, Editor2 = null;
        TextPointer StartSelect = null, EndSelect = null, StartSelect2 = null, EndSelect2 = null;
        private MainWindow _Parent { get; set; }
        private readonly string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);//LanProject.Method.Configuration.ReadSetting("dataPath")
        private readonly string foldername = LanProject.Method.Configuration.ReadSetting("appfoldername");
        private readonly string notefolder = LanProject.Method.Configuration.ReadSetting("notes");
        public Settings()
        {
            Loaded += Settings_Loaded;
            InitializeComponent();
        }
        private void Settings_Loaded(object sender, RoutedEventArgs e) => _Parent = Method.Function.GetParent(this);
        public void ExecutePage(SettingPages page)
        {
            backButton.Visibility = Visibility.Visible;

            switch (page)
            {
                case SettingPages.Note:
                    container.Content = notePage;
                    titleText.Text = "備註設定";
                    break;
                /*case SettingPages.Personal:
                    container.Content = personalPage;
                    titleText.Text = "個人設定";
                    break;*/
            }
        }
        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            container.Content = containerPage;
            backButton.Visibility = Visibility.Collapsed;
            titleText.Text = "設定";
        }
        private bool IsRichTextBoxEmpty(RichTextBox source)
        {
            if (source.Document.Blocks.Count == 0) return true;
            TextPointer startPointer = source.Document.ContentStart.GetNextInsertionPosition(LogicalDirection.Forward);
            TextPointer endPointer = source.Document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward);
            return startPointer.CompareTo(endPointer) == 0;
        }
        #region 新增備註
        private void writespace_TextChanged(object sender, TextChangedEventArgs e)
        {
            Editor = sender as RichTextBox;
            if (IsRichTextBoxEmpty(Editor))
                notePage.notemodel.Allow = false;
            else
                notePage.notemodel.Allow = true;
        }
        private void writespace_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            StartSelect = rtb.Selection.Start;
            EndSelect = rtb.Selection.End;
        }
        private void ClrPcker_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (StartSelect == null || EndSelect == null) return;
            if (!e.NewValue.HasValue) return;
            var ctr = e.NewValue.Value;
            if (ctr.ToString() == "#00FFFFFF")
                ctr = Brushes.Black.Color;
            var brush = new SolidColorBrush(ctr);
            var tx = new TextRange(StartSelect, EndSelect);
            tx.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            StartSelect = null;
            EndSelect = null;
            Xceed.Wpf.Toolkit.ColorPicker colorPicker = sender as Xceed.Wpf.Toolkit.ColorPicker;
            colorPicker.SelectedColor = null;
        }
        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            var _enternameInfo = new EnterNoteNameInfoNotificationMessage();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                MessageBoxResult reply = MessageBoxResult.Yes;
                do
                {
                    bool result = bool.Parse((await DialogHost.Show(_enternameInfo, "RootDialog")).ToString());
                    if (!result) return;
                    reply = MessageBox.Show(String.Format("以 {0} 命名", _enternameInfo.NoteName), "確定嗎?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (reply == MessageBoxResult.Cancel) return;
                    if ((reply == MessageBoxResult.Yes) && (Method.Function.IsFilenameValid(_enternameInfo.NoteName) != null))
                    {
                        _enternameInfo.Error = Method.Function.IsFilenameValid(_enternameInfo.NoteName);
                        reply = MessageBoxResult.No;
                    }

                } while (reply == MessageBoxResult.No);
                Directory.CreateDirectory(System.IO.Path.Combine(programFilesPath, foldername, notefolder));
                var fileLists = Directory.GetFiles(System.IO.Path.Combine(programFilesPath, foldername, notefolder));
                int count = fileLists.Count(x => System.IO.Path.GetFileName(x) == String.Format("{0}.xaml", _enternameInfo.NoteName));
                if (count > 0)
                {
                    _Parent.PopupNews(Method.Function.GetDescription(ErrorCodeList.ERROR_SETTING_PAGE_CREATE_NOTE_FAILED_NAME_REPEAT));
                    return;
                }
                TextRange textRange = new TextRange(Editor.Document.ContentStart, Editor.Document.ContentEnd);
                FileStream fs = File.OpenWrite(System.IO.Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", _enternameInfo.NoteName)));
                textRange.Save(fs, DataFormats.Xaml);
                fs.Close();
                _Parent.PopupNews(Method.Function.GetDescription(CorrectCodeList.CORRECT_SETTING_PAGE_CREATE_NOTE_SUCCESS));
                Editor.Document.Blocks.Clear();
                notePage.vm = new NoteViewModel();
                notePage.DataContext = notePage.vm;
            }));
        }
        #endregion
        private void writespace2_Loaded(object sender, RoutedEventArgs e)
        {
            Editor2 = sender as RichTextBox;
            string keyword = notePage.notemodel2.Name;
            if (File.Exists(System.IO.Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", keyword))))
            {
                var txtRange = new TextRange(Editor2.Document.ContentStart, Editor2.Document.ContentEnd);
                FileStream fs = File.OpenRead(System.IO.Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", keyword)));
                txtRange.Load(fs, DataFormats.Xaml);
                fs.Close();
                _Parent.PopupNews(String.Format("載入備註 {0} 成功！", keyword));
            }
        }
        private void EditNote_Click(object sender, RoutedEventArgs e)
        {
            YesNoDialogHostModel confirm = new YesNoDialogHostModel
            {
                Title = "問題",
                Message = String.Format("確定修改備註嗎?"),
            };
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                bool AgainResult = Convert.ToBoolean((await DialogHost.Show(confirm, "RootDialog")).ToString());
                if (AgainResult)
                {
                    TextRange textRange = new TextRange(Editor2.Document.ContentStart, Editor2.Document.ContentEnd);
                    FileStream fs = File.OpenWrite(System.IO.Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", notePage.notemodel2.Name)));
                    textRange.Save(fs, DataFormats.Xaml);
                    fs.Close();
                    _Parent.PopupNews(Method.Function.GetDescription(CorrectCodeList.CORRECT_SETTING_PAGE_EDIT_NOTE_SUCCESS));
                    Editor2.Document.Blocks.Clear();
                }
            }));
        }
        private void ClrPcker2_Background_SelectedColorChanged(object sender, RoutedPropertyChangedEventArgs<Color?> e)
        {
            if (StartSelect2 == null || EndSelect2 == null) return;
            if (!e.NewValue.HasValue) return;
            var ctr = e.NewValue.Value;
            if (ctr.ToString() == "#00FFFFFF")
                ctr = Brushes.Black.Color;
            var brush = new SolidColorBrush(ctr);
            var tx = new TextRange(StartSelect2, EndSelect2);
            tx.ApplyPropertyValue(TextElement.ForegroundProperty, brush);
            StartSelect2 = null;
            EndSelect2 = null;
            Xceed.Wpf.Toolkit.ColorPicker colorPicker = sender as Xceed.Wpf.Toolkit.ColorPicker;
            colorPicker.SelectedColor = null;

        }
        private void writespace2_TextChanged(object sender, TextChangedEventArgs e)
        {
            Editor2 = sender as RichTextBox;
            if (IsRichTextBoxEmpty(Editor2))
                notePage.notemodel2.Allow = false;
            else
                notePage.notemodel2.Allow = true;
        }
        private void writespace2_SelectionChanged(object sender, RoutedEventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox;
            StartSelect2 = rtb.Selection.Start;
            EndSelect2 = rtb.Selection.End;
        }
    }
}
