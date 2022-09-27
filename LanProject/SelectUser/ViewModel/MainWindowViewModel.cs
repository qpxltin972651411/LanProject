using LanProject.Domain;
using LanProject.SelectUser.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LanProject.SelectUser.ViewModel
{
    internal class MainWindowViewModel
    {
        public ObservableCollection<UserList> userList { get; set; }
        private SqliteDatabase db;
        private readonly string programFilesPath = Method.Configuration.ReadSetting("dataPath");
        private readonly string foldername = Method.Configuration.ReadSetting("appfoldername");
        private readonly string userimagesfoldername = Method.Configuration.ReadSetting("userimagesfoldername");
        public MainWindowViewModel()
        {
            db = new SqliteDatabase();
            Loading();
        }
        private void Loading()
        {
            userList = new ObservableCollection<UserList>();
            var result = db.GetDataTable("SELECT * FROM userlist;");
            var converter = new BrushConverter();
            foreach (DataRow item in result.Rows)
            {
                userList.Add(
                    new UserList
                    {
                        Nickname = item["nickname"].ToString(),
                        IsLocked = Convert.ToBoolean(Convert.ToInt32(item["isLocked"].ToString())),
                        Password = Convert.ToBoolean(Convert.ToInt32(item["isLocked"].ToString())) ? item["password"].ToString() : null,
                        Imagepath = item["imagePath"] == DBNull.Value ? item["nickname"].ToString().Substring(0, 1) : System.IO.Path.Combine(programFilesPath, foldername, userimagesfoldername, item["imagePath"].ToString()),
                        Hasimage = item["imagePath"] != DBNull.Value,
                        Background = (Brush)converter.ConvertFromString(Method.ColorExtensions.ToHexColor()),
                        Email = item["email"].ToString(),
                    }
                );
            }
        }
    }
}
