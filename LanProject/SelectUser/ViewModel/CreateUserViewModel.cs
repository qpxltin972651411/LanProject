using CommunityToolkit.Mvvm.ComponentModel;
using LanProject.Domain;
using LanProject.SelectUser.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.SelectUser.ViewModel
{
    internal class CreateUserViewModel : ObservableObject
    {
        public SqliteDatabase db;
        public ObservableCollection<UserList> userList { get; set; }
        #region nickname
        private string _nickname;
        public string NickName
        {
            get => _nickname;
            set => SetProperty(ref _nickname, value);
        }
        #endregion
        #region password
        private SecureString _password;
        public SecureString Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        #endregion
        #region imagePath
        private string _imagepath;
        public string imagePath
        {
            get => _imagepath;
            set
            {
                SetProperty(ref _imagepath, value);
                if (String.IsNullOrEmpty(value))
                    DisplayArea = false;
                else
                    DisplayArea = true;
            }
        }
        #endregion
        #region DisplayArea
        private bool _displayarea;
        public bool DisplayArea
        {
            get => _displayarea;
            set => SetProperty(ref _displayarea, value);
        }
        #endregion
        #region Enablepassword
        private bool _enablepassword;
        public bool EnablePassword
        {
            get => _enablepassword;
            set => SetProperty(ref _enablepassword, value);
        }
        #endregion
        #region EnableimagePath
        private bool _enableimagepath;
        public bool EnableimagePath
        {
            get => _enableimagepath;
            set => SetProperty(ref _enableimagepath, value);
        }
        #endregion
        #region EnableSubmit
        private bool _enablesubmit;
        public bool EnableSubmit
        {
            get => _enablesubmit;
            set => SetProperty(ref _enablesubmit, value);
        }
        #endregion
        private void Loading()
        {
            userList = new ObservableCollection<UserList>();
            var result = db.GetDataTable("SELECT * FROM userlist;");
            foreach (DataRow item in result.Rows)
            {
                userList.Add(
                    new UserList
                    {
                        Nickname = item["nickname"].ToString(),
                        IsLocked = Convert.ToBoolean(Convert.ToInt32(item["isLocked"].ToString())),
                        Password = Convert.ToBoolean(Convert.ToInt32(item["isLocked"].ToString())) ? item["password"].ToString() : null,
                        Imagepath = item["imagePath"] == DBNull.Value ? item["nickname"].ToString().Substring(0, 1) : item["imagePath"].ToString(),
                        Hasimage = item["imagePath"] != DBNull.Value,
                        Background = null,
                        Email = item["email"].ToString(),
                    }
                ) ;
            }
        }
        public CreateUserViewModel()
        {
            db = new SqliteDatabase();
            NickName = String.Empty;
            Password = new SecureString();
            imagePath = String.Empty;
            DisplayArea = false;
            EnableimagePath = false;
            EnablePassword = false;
            EnableSubmit = false;
        }
        public bool HaveRepeat()
        {
            Loading();
            foreach (var item in userList)
            {
                if (item.Nickname == NickName)
                    return true;
            }
            return false;
        }
    }
}
