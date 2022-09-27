using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.SelectUser.Model
{
    public class DialogHostModel : ObservableObject
    {
        private string _message;
        private string _title;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
    public class enterPasswordDialogHost : DialogHostModel
    {
        #region Password
        private SecureString _pass;
        public SecureString Password
        {
            get => _pass;
            set => SetProperty(ref _pass, value);
        }
        #endregion
        #region Enabled Password 
        private bool _isPasswordValid;
        public bool IsPasswordValid
        {
            get => _isPasswordValid;
            set => SetProperty(ref _isPasswordValid, value);
        }
        #endregion
        public enterPasswordDialogHost()
        {
            PropertyChanged += EnterPasswordDialogHost_PropertyChanged;
            Title = "請輸入驗證碼";
            Message = String.Empty;
            Password = new SecureString();
            IsPasswordValid = false;
        }
        public enterPasswordDialogHost(string name)
        {
            PropertyChanged += EnterPasswordDialogHost_PropertyChanged;
            Title = String.Format("請輸入 {0} 設定之驗證碼", name);
            Message = String.Empty;
            Password = new SecureString();
            IsPasswordValid = false;
        }
        private void EnterPasswordDialogHost_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Password")
            {
                if (Password.Length == 0)
                    IsPasswordValid = false;
                else
                    IsPasswordValid = true;
            }
        }
    }
}
