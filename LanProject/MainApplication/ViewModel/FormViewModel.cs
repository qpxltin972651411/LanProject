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
    public class FormViewModel : ObservableObject
    {
        public SqliteDatabase db { get; set; }
        public BindingList<Form> TotalList { get; set; }
        public BindingList<Form> CloneList { get; set; }
        #region 目前頁面顯示資料集

        private BindingList<Form> _currentpagelist;
        public BindingList<Form> currentPageList
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
        public FormViewModel()
        {
            db = new SqliteDatabase();
            TotalList = new BindingList<Form>();
            CloneList = null;
            currentPageList = new BindingList<Form>();
            PerPageDisplayOfCount = 15;
            TotalCount = 0;
            CurrentPage = 1;
        }
        private void Reading()
        {
            TotalList = new BindingList<Form>();
            var result = db.GetDataTable("SELECT * FROM formtable;");
            foreach (DataRow row in result.Rows)
            {
                var ft = new Form
                {
                    ID = row["id"].ToString(),
                    Formtype = Convert.ToInt32(row["type"].ToString()),
                    Myunit = new Unit { Name = row["mname"].ToString(), Tax = row["mtax"].ToString(), Cel = row["mcel"].ToString() },
                    Customunit = new Unit { Name = row["oname"].ToString(), Tax = row["otax"].ToString(), Cel = row["ocel"].ToString() },
                    Note = row["note"].ToString(),
                    Location = new Addr(
                        String.IsNullOrWhiteSpace(row["country"].ToString()) ? String.Empty : row["country"].ToString(),
                        String.IsNullOrWhiteSpace(row["city"].ToString()) ? String.Empty : row["city"].ToString(),
                        String.IsNullOrWhiteSpace(row["address"].ToString()) ? String.Empty : row["address"].ToString()),
                    Total = Convert.ToDouble(row["total"].ToString()),
                    Users = new List<string>(),
                    EditTimeList = new List<DateTime>(),
                };


                Dictionary<string, object> KeyValues = new Dictionary<string, object>();
                List<int> DataTypeList = new List<int>();
                KeyValues.Add("@name", ft.Myunit.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", ft.Myunit.Cel);
                DataTypeList.Add(0); 
                KeyValues.Add("@tax", ft.Myunit.Tax);
                DataTypeList.Add(0);
                var utres = db.GetDataTable("SELECT * FROM myunit WHERE name = @name AND cel = @cel AND tax = @tax;",KeyValues,DataTypeList);
                foreach (DataRow i in utres.Rows)
                {
                    ft.Myunit.Cel = i["cel"].ToString();
                    ft.Myunit.Tel = new Contact(i["telareacode"].ToString(), i["telnumber"].ToString());
                    ft.Myunit.Fax = new Contact(i["faxareacode"].ToString(), i["faxnumber"].ToString());
                    ft.Myunit.Location = new Addr(i["country"].ToString(), i["city"].ToString(), i["address"].ToString());
                    break;
                }

                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();
                KeyValues.Add("@name", ft.Customunit.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", ft.Customunit.Cel);
                DataTypeList.Add(0);
                KeyValues.Add("@tax", ft.Customunit.Tax);
                DataTypeList.Add(0);
                utres = db.GetDataTable("SELECT * FROM customunit WHERE name = @name AND cel = @cel AND tax = @tax;", KeyValues, DataTypeList);
                foreach (DataRow i in utres.Rows)
                {
                    ft.Customunit.Cel = i["cel"].ToString();
                    ft.Customunit.Tel = new Contact(i["telareacode"].ToString(), i["telnumber"].ToString());
                    ft.Customunit.Fax = new Contact(i["faxareacode"].ToString(), i["faxnumber"].ToString());
                    ft.Customunit.Location = new Addr(i["country"].ToString(), i["city"].ToString(), i["address"].ToString());
                    break;
                }


                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();
                ft.Products = new BindingList<Product>();
                KeyValues.Add("@id", ft.ID);
                DataTypeList.Add(0);
                var pdlist = db.GetDataTable("SELECT * FROM formdetails WHERE id = @id;", KeyValues, DataTypeList);
                for (var i = 0; i < pdlist.Rows.Count; i++)
                {
                    DataRow dr = pdlist.Rows[i];
                    var pd = new Product(i,
                        dr["pname"].ToString(),
                        Convert.ToDouble(dr["length"].ToString()),
                        Convert.ToDouble(dr["width"].ToString()),
                        Convert.ToDouble(dr["ironmold"].ToString()),
                        Convert.ToDouble(dr["powercoating"].ToString()),
                        Convert.ToDouble(dr["ironslips"].ToString()),
                        Convert.ToDouble(dr["nut"].ToString()));
                    ft.Products.Add(pd);
                }

                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();
                KeyValues.Add("@id", ft.ID);
                DataTypeList.Add(0);
                utres = db.GetDataTable("SELECT * FROM formeditrecord WHERE id = @id;", KeyValues, DataTypeList);
                foreach (DataRow i in utres.Rows)
                {
                    ft.EditTimeList.Add(Convert.ToDateTime(i["Timestamp"].ToString()));
                    var usr = i["user"].ToString();
                    if (ft.Users.Contains(usr))
                        continue;
                    ft.Users.Add(usr);
                }
                    
                
                TotalList.Add(ft);
            }
        }
        public void Loading()
        {
            Reading();
            reLoadingInfo();
            UpdateCurrentDisplayList();
        }
        public void Filter()
        {
            CurrentPage = 1;
            reLoadingInfo();
            UpdateCurrentDisplayList();
        }
        private void reLoadingInfo()
        {
            TotalCount = TotalList.Count;
            TotalPage = TotalCount / PerPageDisplayOfCount;
            if ((TotalCount % PerPageDisplayOfCount) != 0)
                TotalPage += 1;
            if (TotalPage == 0)
                TotalPage += 1;
        }
        private void UpdateCurrentDisplayList()
        {
            currentPageList = null;
            if (CurrentPage == 1)
                currentPageList = new BindingList<Form>(TotalList.Skip(0).Take(PerPageDisplayOfCount).ToList());
            else
                currentPageList = new BindingList<Form>(TotalList.Skip((CurrentPage - 1) * PerPageDisplayOfCount).Take(PerPageDisplayOfCount).ToList());
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
