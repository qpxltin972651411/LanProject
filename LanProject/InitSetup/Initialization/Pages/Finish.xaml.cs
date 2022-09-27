using LanProject.Domain;
using LanProject.Method;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

namespace LanProject.InitSetup.Initialization.Pages
{
    /// <summary>
    /// Finish.xaml 的互動邏輯
    /// </summary>
    public partial class Finish : Page
    {
        private string createInfoWord { get; set; }
        public bool mandatory { get; set; }

        private string[] strs = { "", ".", "..", "...", "...." };
        private bool working { get; set; }
        private Signup _page { get; set; }
        private Thread AnimationThread { get; set; }
        private Thread CreateThread { get; set; }
        public Finish() => InitializeComponent();
        public Finish(Signup p)
        {
            _page = p;

            InitializeComponent();
            Creating();
        }
        private void Creating()
        {
            working = true;
            mandatory = false;
            createInfoWord = "建置環境中";
            AnimationThread = new Thread(new ThreadStart(CreatingAnimation));
            AnimationThread.Start();
            CreateThread = new Thread(new ThreadStart(CreatingEnvironment));
            CreateThread.Start();
        }
        void CreatingEnvironment()
        {
            try
            {
                Thread.Sleep(2000);
                Application.Current.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Configuration.AddUpdateAppSettings("dataPath", _page._firstpage.Path.Text);
                    string foldername = Configuration.ReadSetting("appfolderName");
                    if (String.IsNullOrEmpty(foldername))
                        throw new Exception("找不到位置");

                    Directory.CreateDirectory(System.IO.Path.Combine(_page._firstpage.Path.Text, foldername));
                    SqliteDatabase _db = new SqliteDatabase();
                    _db.InitDatabaseSchemas();
                    const string insertIntotbl1TableString = "INSERT INTO user (acc, pwd) VALUES (@acc, @pwd)";
                    Dictionary<string, object> KeyValues = new Dictionary<string, object>();
                    List<int> DataTypeList = new List<int>();

                    KeyValues.Add("@acc", _page.acc);
                    DataTypeList.Add(0);

                    KeyValues.Add("@pwd", Generate.Base64Encode(_page.passwd));
                    DataTypeList.Add(0);
                    _db.Manipulate(insertIntotbl1TableString, KeyValues, DataTypeList);
                }));
                working = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }

        }
        void CreatingAnimation()
        {
            try
            {
                Int64 tick = 0;
                while (working)
                {
                    Thread.Sleep(200);
                    this.Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        this.creatingInfo.Content = createInfoWord + strs[tick % strs.Length] + tick.ToString();
                    });
                    tick = (tick + 1) % Int64.MaxValue;
                }
                this.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    this.creatingInfo.Content = "環境建置完成";
                    this.toggleIcon.IsChecked = true;
                });
                this.Dispatcher.BeginInvoke((Action)delegate ()
                {
                    mandatory = true;
                    Navigtion win = (Navigtion)Parent;
                    if (!win.LockClosing)
                        win.Close();
                });

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Message", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }
        }
    }
}
