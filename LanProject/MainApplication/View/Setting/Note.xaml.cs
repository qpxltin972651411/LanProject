using LanProject.Domain;
using LanProject.MainApplication.Map;
using LanProject.MainApplication.Model;
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

namespace LanProject.MainApplication.View.Setting
{
    /// <summary>
    /// Note.xaml 的互動邏輯
    /// </summary>
    public partial class Note : UserControl
    {
        public CreateNoteNotificationMessage notemodel { get; set; }
        public EditNoteNotificationMessage notemodel2 { get; set; }
        public NoteViewModel vm { get; set; }
        private readonly string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);//LanProject.Method.Configuration.ReadSetting("dataPath")
        private readonly string foldername = LanProject.Method.Configuration.ReadSetting("appfoldername");
        private readonly string notefolder = LanProject.Method.Configuration.ReadSetting("notes");
        public Note()
        {
            InitializeComponent();
            vm = new NoteViewModel();
            DataContext = vm;
        }
        private void Create_Click(object sender, RoutedEventArgs e)
        {
            if (notemodel == null) notemodel = new CreateNoteNotificationMessage();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                await DialogHost.Show(notemodel, "MainDialog");
            }));

        }

        private void noteedit_Click(object sender, RoutedEventArgs e)
        {
            string obj = ((FrameworkElement)sender).DataContext as string;
            notemodel2 = new EditNoteNotificationMessage(obj);
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                await DialogHost.Show(notemodel2, "MainDialog");
                vm = new NoteViewModel();
                DataContext = vm;
            }));
        }

        private void notedelete_Click(object sender, RoutedEventArgs e)
        {
            string obj = ((FrameworkElement)sender).DataContext as string;
            YesNoDialogHostModel confirm = new YesNoDialogHostModel
            {
                Title = "問題",
                Message = String.Format("確定刪除備註 {0} 嗎?", obj),
            };
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                bool AgainResult = Convert.ToBoolean((await DialogHost.Show(confirm, "RootDialog")).ToString());
                if (AgainResult)
                {
                    File.Delete(System.IO.Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", obj)));
                    string execute3 = "UPDATE formtable SET  note = @note WHERE note = @keynote;";
                    var KeyValues = new Dictionary<string, object>();
                    var DataTypeList = new List<int>();

                    KeyValues.Add("@note", "無");
                    DataTypeList.Add(0);
                    KeyValues.Add("@keynote", obj);
                    DataTypeList.Add(0);
                    var db = new SqliteDatabase();
                    db.Manipulate(execute3, KeyValues, DataTypeList);
                    vm = new NoteViewModel();
                    DataContext = vm;
                }
            }));
        }

        private void noterename_Click(object sender, RoutedEventArgs e)
        {
            string obj = ((FrameworkElement)sender).DataContext as string;
            var _enternameInfo = new EnterNoteNameInfoNotificationMessage();
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                MessageBoxResult reply = MessageBoxResult.Yes;
                do
                {
                    bool result = bool.Parse((await DialogHost.Show(_enternameInfo, "RootDialog")).ToString());
                    if (!result) return;
                    reply = MessageBox.Show(String.Format("以 {0} 取代 {1} 命名", _enternameInfo.NoteName, obj), "確定嗎?", MessageBoxButton.YesNoCancel, MessageBoxImage.Question);
                    if (reply == MessageBoxResult.Cancel) return;
                    if (obj == _enternameInfo.NoteName)
                    {
                        _enternameInfo.Error = Method.Function.GetDescription(ErrorCodeList.ERROR_SETTING_PAGE_CREATE_NOTE_FAILED_NAME_NOT_CHANGE);
                        reply = MessageBoxResult.No;
                    }
                    if ((reply == MessageBoxResult.Yes) && (Method.Function.IsFilenameValid(_enternameInfo.NoteName) != null))
                    {
                        _enternameInfo.Error = Method.Function.IsFilenameValid(_enternameInfo.NoteName);
                        reply = MessageBoxResult.No;
                    }
                    if ((reply == MessageBoxResult.Yes) && (vm.NoteList.Contains(_enternameInfo.NoteName)))
                    {
                        _enternameInfo.Error = Method.Function.GetDescription(ErrorCodeList.ERROR_SETTING_PAGE_CREATE_NOTE_FAILED_NAME_REPEAT);
                        reply = MessageBoxResult.No;
                    }
                } while (reply == MessageBoxResult.No);
                var _old = System.IO.Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", obj));
                var _new = System.IO.Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", _enternameInfo.NoteName));
                File.Move(_old, _new);
                string execute3 = "UPDATE formtable SET note = @note WHERE note = @keynote;";
                var KeyValues = new Dictionary<string, object>();
                var DataTypeList = new List<int>();

                KeyValues.Add("@note", _enternameInfo.NoteName);
                DataTypeList.Add(0);
                KeyValues.Add("@keynote", obj);
                DataTypeList.Add(0);
                var db = new SqliteDatabase();
                db.Manipulate(execute3, KeyValues, DataTypeList);
                vm = new NoteViewModel();
                DataContext = vm;
            }));
        }
    }
}
