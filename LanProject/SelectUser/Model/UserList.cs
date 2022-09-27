using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LanProject.SelectUser.Model
{
    public class UserList : ObservableObject
    {
        #region nickname
        private string _nickname;
        public string Nickname
        {
            get => _nickname;
            set => SetProperty(ref _nickname, value);
        }
        #endregion
        #region IsLocked
        private bool _islocked;
        public bool IsLocked
        {
            get => _islocked;
            set => SetProperty(ref _islocked, value);
        }
        #endregion
        #region password
        private string _password;
        public string Password
        {
            get => _password;
            set => SetProperty(ref _password, value);
        }
        #endregion
        #region imagePath
        private string _imagepath;
        public string Imagepath
        {
            get => _imagepath;
            set => SetProperty(ref _imagepath, value);
        }
        #endregion
        #region Have Image
        private bool _hasimage;
        public bool Hasimage
        {
            get => _hasimage;
            set => SetProperty(ref _hasimage, value);
        }
        #endregion
        #region Background
        private Brush _color;
        public Brush Background
        {
            get => _color;
            set => SetProperty(ref _color, value);
        }
        #endregion
        #region email
        private string _email;
        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }
        #endregion
    }
}
