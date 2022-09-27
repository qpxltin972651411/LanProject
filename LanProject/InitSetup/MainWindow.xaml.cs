using LanProject.Domain;
using LanProject.InitSetup.Initialization;
using LanProject.Method;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace LanProject.InitSetup
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private string InfoWord { get; set; }

        private string[] animateStrings = { "", ".", "..", "...", "...." };
        private bool SucceedSignal { get; set; }
        private bool WorkingSignal { get; set; }
        private Thread AnimationThread { get; set; }
        private Thread LoadThread { get; set; }
        private bool FinishSignal { get; set; }
        private void animationLoadingGif()
        {
            SucceedSignal = false;
            WorkingSignal = true;
            InfoWord = "檢查環境中";
            loadingGif.Foreground = Method.ColorExtensions.linearGradientForeground();
            var dbAnimation = new DoubleAnimation(360, 0, new Duration(TimeSpan.FromSeconds(1)));
            var rt = new RotateTransform();
            loadingGif.RenderTransform = rt;
            loadingGif.RenderTransformOrigin = new Point(0.5, 0.5);
            dbAnimation.RepeatBehavior = RepeatBehavior.Forever;
            rt.BeginAnimation(RotateTransform.AngleProperty, dbAnimation);
        }
        public MainWindow()
        {
            Process thisProc = Process.GetCurrentProcess();
            if (Process.GetProcessesByName(thisProc.ProcessName).Length > 1)
            {
                MessageBox.Show("程序已在運行中", "無法開啟", MessageBoxButton.OK, MessageBoxImage.Error);
                FinishSignal = true;
                Application.Current.Shutdown();
                return;
            }
            InitializeComponent();
            animationLoadingGif();
            LoadingEnvironment();
        }
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (FinishSignal)
            {
                e.Cancel = false;
            }
            else
            {
                if (MessageBox.Show("確定要終止程式運行嗎", "離開", MessageBoxButton.OKCancel, MessageBoxImage.Question) == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                    return;
                }
                if (LoadThread == null || AnimationThread == null)
                    return;
                Process.GetCurrentProcess().Kill();
            }
        }
        private void LoadingEnvironment()
        {
            AnimationThread = new Thread(new ThreadStart(LoadingAnimation));
            AnimationThread.Start();
            LoadThread = new Thread(new ThreadStart(LoadingData));
            LoadThread.Start();
        }
        private void Successful()
        {
            WorkingSignal = false;
            SucceedSignal = true;
            FinishSignal = true;
            InfoWord = "載入成功";
            Thread.Sleep(3000);
            Dispatcher.Invoke((Action)(() =>
            {
                Hide();
                Authorize.MainWindow _next = new Authorize.MainWindow();
                Application.Current.MainWindow = _next;
                _next.Show();
                Close();
            }));
        }
        void LoadingData()
        {
            try
            {
                string dataPath = Configuration.ReadSetting("dataPath");
                Thread.Sleep(3000);
                if (String.IsNullOrEmpty(dataPath))
                {
                    //first to run it
                    enterFirstSettingPage();
                }
                else
                {
                    //check folder is not exists and data file is not exists
                    string folderName = Configuration.ReadSetting("appfolderName");
                    string databasenme = Configuration.ReadSetting("databasename");
                    if (String.IsNullOrEmpty(folderName))
                    {
                        //MessageBox.Show("[ appfolderName not exists ] - 系統設定檔錯誤", "訊息", MessageBoxButton.OK, MessageBoxImage.Error);
                        Process.GetCurrentProcess().Kill();
                        return;
                    }
                    if (!Directory.Exists(System.IO.Path.Combine(dataPath, folderName)))
                    {
                        //folder not exists
                        //MessageBox.Show("指定環境中找不到目錄", "選擇", MessageBoxButton.OK, MessageBoxImage.Error);
                        showFixPage();
                    }
                    else
                    {
                        //folder exists
                        List<string> filelist = Directory.GetFiles(System.IO.Path.Combine(dataPath, folderName)).ToList();
                        foreach (var item in filelist)
                        {
                            string ext = System.IO.Path.GetExtension(item);
                            string fn = System.IO.Path.GetFileNameWithoutExtension(item);
                            if (ext.Equals(".sqlite") && fn.Equals(databasenme))
                            {
                                SqliteDatabase _db = new SqliteDatabase();
                                _db.CreateDatabase();
                                if (_db.CheckDatabaseSchema())
                                {
                                    Successful();
                                    return;
                                }
                                else
                                {
                                    showFixPage();
                                    return;
                                }
                            }
                        }
                        //MessageBox.Show("找不到檔案", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                        showFixPage();
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }
        }
        void LoadingAnimation()
        {
            try
            {
                Int64 countTime = 0;
                while (WorkingSignal)
                {
                    Thread.Sleep(200);
                    Dispatcher.BeginInvoke((Action)delegate ()
                    {
                        loadingInfo.Content = InfoWord + animateStrings[countTime % animateStrings.Length];
                        toggleIcon();
                    });

                    countTime = (countTime + 1) % Int64.MaxValue;
                }
                Dispatcher.BeginInvoke((Action)delegate ()
                {
                    loadingInfo.Content = InfoWord;
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "訊息", MessageBoxButton.OK, MessageBoxImage.Error);
                Process.GetCurrentProcess().Kill();
            }
        }
        private void enterFirstSettingPage()
        {
            WorkingSignal = false;
            SucceedSignal = false;
            InfoWord = "首次使用須設定";
            Dispatcher.Invoke((Action)(() =>
            {
                Navigtion _new = new Navigtion();
                _new.ShowDialog();
            }));
            WorkingSignal = true;
            InfoWord = "檢查環境中";
            LoadingEnvironment();
        }
        private void showFixPage()
        {
            bool signal = false;
            Application.Current.Dispatcher.BeginInvoke(new Action(() =>
            {
                WorkingSignal = false;
                SucceedSignal = false;
                InfoWord = "找不到資料";

                Fix _fix = new Fix();
                _fix.resetSignal += value => signal = value;
                _fix.ShowDialog();
                if (signal == false)
                {
                    WorkingSignal = true;
                    InfoWord = "檢查環境中";
                    LoadingEnvironment();
                }
                else
                {
                    enterFirstSettingPage();
                }
            }));
        }
        private void toggleIcon()
        {
            if (SucceedSignal)
            {
                successIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.CheckCircle;
                successIcon.Foreground = Brushes.Green;
            }
            else
            {
                successIcon.Icon = FontAwesome.WPF.FontAwesomeIcon.Remove;
                successIcon.Foreground = Brushes.Red;
            }
            if (WorkingSignal)
                toggleSuccessIcon.IsChecked = false;
            else
                toggleSuccessIcon.IsChecked = true;
        }
    }
}
