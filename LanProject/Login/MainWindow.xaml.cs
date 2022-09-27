using LanProject.Domain;
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
using System.Windows.Shapes;

namespace LanProject.Login
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        private void submitBtn_Click(object sender, RoutedEventArgs e)
        {
            var verify = Method.StringExtensions.OnlyAcceptLetterAndNumber(userNameBox);
            if (verify != String.Empty)
            {
                warningLabel.Content = verify;
                return;
            }
            SqliteDatabase _db = new SqliteDatabase();
            try
            {
                string query = "SELECT * FROM user WHERE acc=@acc AND pwd=@pwd;";
                Dictionary<string, object> KeyValues = new Dictionary<string, object>();
                List<int> DataTypeList = new List<int>();

                KeyValues.Add("@acc", userNameBox.Text);
                DataTypeList.Add(0);

                KeyValues.Add("@pwd", Method.Generate.Base64Encode(passwordBox.Password));
                DataTypeList.Add(0);
                var result = _db.GetDataTable(query, KeyValues, DataTypeList);

                if (result.Rows.Count == 1)
                {
                    SelectUser.MainWindow dashboard = new SelectUser.MainWindow();
                    Application.Current.MainWindow = dashboard;
                    dashboard.Show();
                    this.Close();
                }
                else
                {
                    warningLabel.Content = "錯誤";
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "error");
            }
        }

    }
}
