using CommunityToolkit.Mvvm.ComponentModel;
using LanProject.Domain;
using LanProject.MainApplication.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.MainApplication.ViewModel
{
    public class BaseViewModel : ObservableObject
    {
        public SqliteDatabase db { get; set; }
        public BindingList<Unit> TotalList { get; set; }
        public BindingList<Unit> CloneList { get; set; }
        #region 目前頁面顯示資料集

        private BindingList<Unit> _currentpagelist;
        public BindingList<Unit> currentPageList
        {
            get => _currentpagelist;
            set => SetProperty(ref _currentpagelist, value);
        }
        #endregion
        #region 一頁顯示的筆數

        private int _perpagedisplaypfcount;
        public int PerPageDisplayOfCount
        {
            get => _perpagedisplaypfcount;
            set => SetProperty(ref _perpagedisplaypfcount, value);
        }
        #endregion
        #region 總筆數
        private int _totalcount;
        public int TotalCount
        {
            get => _totalcount;
            set => SetProperty(ref _totalcount, value);
        }
        #endregion
        #region 總頁數
        private int _totalpage;
        public int TotalPage
        {
            get => _totalpage;
            set => SetProperty(ref _totalpage, value);
        }
        #endregion
        #region 目前為第幾頁
        private int _currentpage;
        public int CurrentPage
        {
            get => _currentpage;
            set => SetProperty(ref _currentpage, value);
        }
        #endregion
        public BaseViewModel()
        {
            db = new SqliteDatabase();
            TotalList = new BindingList<Unit>();
            CloneList = null;
            currentPageList = new BindingList<Unit>();
            PerPageDisplayOfCount = 15;
            TotalCount = 0;
            TotalPage = 1;
            CurrentPage = 1;
        }
        private void Reading(string executeString)
        {
            TotalList = new BindingList<Unit>();
            var result = db.GetDataTable(executeString);
            foreach (DataRow row in result.Rows)
            {
                var unit = new Unit
                {
                    Name = row["name"].ToString(),
                    Tax = row["tax"].ToString(),
                    Cel = row["cel"].ToString(),
                    Tel = new Contact(row["telareacode"].ToString(), row["telnumber"].ToString()),
                    Fax = new Contact(row["faxareacode"].ToString(), row["faxnumber"].ToString()),
                    Location = new Addr(row["country"].ToString(), row["city"].ToString(), row["address"].ToString())
                };
                TotalList.Add(unit);
            }
        }
        public void Loading(string executeString)
        {
            Reading(executeString);
            reLoadingInfo();
            UpdateCurrentDisplayList();
        }
        public void Filter()
        {
            CurrentPage = 1;
            reLoadingInfo();
            UpdateCurrentDisplayList();
        }
        public void reLoadingInfo()
        {
            TotalCount = TotalList.Count;
            TotalPage = TotalCount / PerPageDisplayOfCount;
            if ((TotalCount % PerPageDisplayOfCount) != 0)
                TotalPage += 1;
            if (TotalPage == 0)
                TotalPage += 1;
        }
        public void UpdateCurrentDisplayList()
        {
            currentPageList = null;
            if (CurrentPage == 1)
                currentPageList = new BindingList<Unit>(TotalList.Skip(0).Take(PerPageDisplayOfCount).ToList());
            else
                currentPageList = new BindingList<Unit>(TotalList.Skip((CurrentPage - 1) * PerPageDisplayOfCount).Take(PerPageDisplayOfCount).ToList());
        }
        public void PrevPage()
        {
            CurrentPage -= 1;
            UpdateCurrentDisplayList();
        }
        public bool NextPage()
        {
            if (CurrentPage == TotalPage)
                return false;
            CurrentPage += 1;
            UpdateCurrentDisplayList();
            return true;
        }
    }
}
