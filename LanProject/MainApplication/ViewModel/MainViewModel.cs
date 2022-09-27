using CommunityToolkit.Mvvm.ComponentModel;
using LanProject.MainApplication.Model;
using LanProject.SelectUser.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;

namespace LanProject.MainApplication.ViewModel
{
    internal class MainViewModel : ObservableObject
    {
        public Thread TimeTickThread { get; set; }
        #region 目前選擇的頁面索引值
        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set
            {
                selectedIndex = value;
                OnPropertyChanged("SelectedIndex");
            }
        }
        #endregion
        #region 目前選擇的頁面
        private Page _selectedItem;
        public Page SelectedItem
        {
            get => _selectedItem;
            set => SetProperty(ref _selectedItem, value);
        }
        #endregion
        #region 目前時間
        private string now;
        public string Now
        {
            get => now;
            set => SetProperty(ref now, value);
        }
        #endregion
        #region 頁面總覽
        private ObservableCollection<Page> dataList;
        public ObservableCollection<Page> DataList
        {
            get => dataList;
            set => SetProperty(ref dataList, value);
        }
        #endregion
        private UserList _currentuser;
        public UserList CurrentUser
        {
            get => _currentuser;
            set => SetProperty(ref _currentuser, value);
        }
        private void DateTimeTick()
        {
            while (true)
            {
                Now = DateTime.UtcNow.AddHours(08).ToString("HH : mm : ss");
                Thread.Sleep(1000);
            }
        }
        private readonly IDictionary Collection = null;
        public MainViewModel()
        {
            TimeTickThread = new Thread(new ThreadStart(DateTimeTick));
            TimeTickThread.Start();
            SelectedIndex = 0;              //預設選擇頁面
            DataList = GetPageList();
        }
        public MainViewModel(IDictionary _collection)
        {
            TimeTickThread = new Thread(new ThreadStart(DateTimeTick));
            TimeTickThread.Start();
            SelectedIndex = 0;              //預設選擇頁面
            DataList = GetPageList();
            Collection = _collection;
            InitCurrentUser();
        }
        private void InitCurrentUser()
        {
            CurrentUser = new UserList();
            CurrentUser.Nickname = Collection["identity"].ToString();
            CurrentUser.Hasimage = Convert.ToBoolean(Collection["Hasimage"]);
            CurrentUser.Imagepath = Collection["imagepath"].ToString();
            CurrentUser.Background = (Brush)Collection["Background"];
            CurrentUser.Email = String.IsNullOrWhiteSpace(Collection["email"].ToString()) ? "無信箱" : Collection["email"].ToString();
        }
        private ObservableCollection<Page> GetPageList()
        {
            //string name = Properties.Lang.ResourceManager.GetString("Button");
            return new ObservableCollection<Page>
            {
                new Page{ ImgPath = "Home", DisplayName = "首頁",Name = "Home",type = typeof(View.Home)},
                new Page{ ImgPath = "AddressBookOutline", DisplayName = "自單位",Name = "SelfCompany",type = typeof(View.SelfCompany)},
                new Page{ ImgPath = "AddressCardOutline", DisplayName = "他單位",Name = "OtherCompany",type = typeof(View.OtherCompany)},
                new Page{ ImgPath = "Wpforms", DisplayName = "表單",Name = "Form",type = typeof(View.Form)},
                new Page{ ImgPath = "Gears", DisplayName = "設定",Name = "Setting",type = typeof(View.Settings)},
            };
        }
    }
}
