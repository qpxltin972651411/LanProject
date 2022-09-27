using LanProject.Authorize.Model;
using LanProject.Method;
using MongoDB.Driver;
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
using System.Windows.Shapes;

namespace LanProject.Authorize
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        private string tablename = Configuration.ReadSetting("table");
        private readonly Database db = new Database();
        private List<MongoDBModel> authTableList = new List<MongoDBModel>();
        private readonly string cpuid = PCInfo.GetCpuID();
        private readonly string macaddress = PCInfo.GetEthernetMacaddress();
        private readonly string motherboard = PCInfo.GetID();
        private readonly string programFilesPath = Configuration.ReadSetting("dataPath");
        private readonly string foldername = Configuration.ReadSetting("appfoldername");
        public MainWindow()
        {
            InitializeComponent();
        }
        private void WindowRendered(object sender, EventArgs e)
        {
            if (!Directory.Exists(System.IO.Path.Combine(programFilesPath, foldername)))
            {
                MessageBox.Show("找不到認證紀錄", "認證失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show("請致電聯絡或信箱聯繫", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            var files = Directory.GetFiles(System.IO.Path.Combine(programFilesPath, foldername));
            if (files.Length == 0)
            {
                MessageBox.Show("找不到認證紀錄", "認證失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show("請致電聯絡或信箱聯繫", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (var item in files)
            {
                if (item.EndsWith(".lic"))
                {
                    var read = File.ReadAllLines(item)[0];
                    var enco = Generate.MD5Decrypt(read.Substring(8, read.Length - 8), read.Substring(0, 8));
                    if (enco == null)
                    {
                        MessageBox.Show("讀取註冊資料失敗", "認證失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                        MessageBox.Show("請重新輸入原序號", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    var encosplit = enco.Split(new[] { "___" }, StringSplitOptions.None);
                    if (!encosplit[0].Equals(cpuid))
                    {
                        MessageBox.Show("CPU識別與原資料不符", "認證失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                        MessageBox.Show("請致電聯絡或信箱聯繫，請求序號清除原紀錄", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    if (!encosplit[1].Equals(macaddress))
                    {
                        MessageBox.Show("網卡與原資料不符", "認證失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                        MessageBox.Show("請致電聯絡或信箱聯繫，請求序號清除原紀錄", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    if (!encosplit[2].Equals(motherboard))
                    {
                        MessageBox.Show("主機板與原資料不符", "認證失敗", MessageBoxButton.OK, MessageBoxImage.Error);
                        MessageBox.Show("請致電聯絡或信箱聯繫，請求序號清除原紀錄", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
                        return;
                    }
                    Successful();
                    return;
                }
            }
            MessageBox.Show("找不到認證紀錄", "認證失敗", MessageBoxButton.OK, MessageBoxImage.Error);
            MessageBox.Show("請致電聯絡或信箱聯繫", "訊息", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        private void Successful()
        {
            Login.MainWindow _next = new Login.MainWindow();
            Application.Current.MainWindow = _next;
            _next.Show();
            Close();
        }
        private void submit_Click(object sender, RoutedEventArgs e)
        {
            char[] separators = new char[] { ' ', '_', '-' };
            var input = FindName("productkeyinput") as Xceed.Wpf.Toolkit.MaskedTextBox;

            var value = input.Text.Trim().Replace(separators, "");
            if (value.Length < 20)
            {
                MessageBox.Show("序號不足20碼", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //MessageBox.Show(value, "輸入的值", MessageBoxButton.OK, MessageBoxImage.Information);
            var authtable = db.conn.GetCollection<MongoDBModel>(tablename);
            authTableList = authtable.Find(a => a.licenseKey.Equals(value)).ToList();
            if (authTableList.Count == 0)
            {
                MessageBox.Show("序號不存在，請確認序號後重新輸入", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                if (authTableList[0].pcID == null)
                {
                    var enmd5 = Generate.MD5Encrypt(String.Format("{0}___{1}___{2}", cpuid, macaddress, motherboard), value.Substring(0, 8));
                    var _mongoCollection = db.conn.GetCollection<MongoDBModel>(tablename);
                    var filter = Builders<MongoDBModel>.Filter.Eq(a => a.licenseKey, value);
                    // 設定新的值
                    var update = Builders<MongoDBModel>.Update.Set(a => a.pcID, enmd5);
                    //將過濾條件與設定值傳入 collection 進行更新
                    _mongoCollection.UpdateOne(filter, update);

                    //

                    string path = System.IO.Path.Combine(programFilesPath, foldername);
                    path = System.IO.Path.Combine(path, String.Format("{0}.lic", Generate.RandomString(10)));
                    using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                    {
                        Byte[] info = new UTF8Encoding(true).GetBytes(String.Format("{0}{1}", value.Substring(0, 8), enmd5));
                        // Add some information to the file.
                        fs.Write(info, 0, info.Length);
                    }
                    MessageBox.Show("成功啟用序號", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                    Successful();
                }
                else
                {
                    var enmd5 = Generate.MD5Encrypt(String.Format("{0}___{1}___{2}", cpuid, macaddress, motherboard), value.Substring(0, 8));
                    MessageBox.Show("序號曾使用過");
                    if (authTableList[0].pcID.Equals(enmd5))
                    {

                        //

                        string path = System.IO.Path.Combine(programFilesPath, foldername);
                        path = System.IO.Path.Combine(path, String.Format("{0}.lic", Generate.RandomString(10)));
                        using (FileStream fs = File.Open(path, FileMode.Create, FileAccess.Write, FileShare.None))
                        {
                            Byte[] info = new UTF8Encoding(true).GetBytes(String.Format("{0}{1}", value.Substring(0, 8), enmd5));
                            // Add some information to the file.
                            fs.Write(info, 0, info.Length);
                        }
                        MessageBox.Show("成功啟用序號", "成功", MessageBoxButton.OK, MessageBoxImage.Information);
                        Successful();
                        return;
                    }
                    MessageBox.Show("序號已被註冊", "錯誤", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
        }
        private void Hyperlink_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var email = Configuration.ReadSetting("email");
            Clipboard.SetText(email);
            MessageBox.Show("已複製信箱", "消息", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
