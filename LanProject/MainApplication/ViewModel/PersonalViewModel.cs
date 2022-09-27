using CommunityToolkit.Mvvm.ComponentModel;
using LanProject.SelectUser.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LanProject.MainApplication.ViewModel
{
    public class PersonalViewModel : ObservableObject
    {
        private readonly IDictionary Collection = null;
        private UserList _currentuser;
        public UserList CurrentUser
        {
            get => _currentuser;
            set => SetProperty(ref _currentuser, value);
        }
        public bool enableclear
        {
            get ;
            set ;
        }
        private bool enp;
        public bool EnablePassword
        {
            get => enp;
            set => SetProperty(ref enp, value);
        }
        private bool notedit;
        public bool NoEdit
        {
            get => notedit;
            set => SetProperty(ref notedit, value);
        }
        private bool clrpwd;
        public bool ClrPwd
        {
            get => clrpwd;
            set => SetProperty(ref clrpwd, value);
        }
        private string newnickname;
        public string NewNickname
        {
            get => newnickname;
            set => SetProperty(ref newnickname, value);
        }
        private void InitCurrentUser()
        {
            CurrentUser = new UserList();
            CurrentUser.Nickname = Collection["identity"].ToString();
            CurrentUser.Hasimage = Convert.ToBoolean(Collection["Hasimage"]);
            CurrentUser.Imagepath = Collection["imagepath"].ToString();
            CurrentUser.Background = (Brush)Collection["Background"];
            CurrentUser.Email = String.IsNullOrWhiteSpace(Collection["email"].ToString()) ? "無信箱" : Collection["email"].ToString();
            CurrentUser.Password = Collection["password"].ToString();
        }
        public PersonalViewModel(IDictionary _collection)
        {
            Collection = _collection;
            InitCurrentUser();
            ClrPwd = false;
            EnablePassword = false;
            NoEdit = true;
            enableclear = !(CurrentUser.Password == String.Empty);
            NewNickname = String.Empty;
        }
    }
}
