using CommunityToolkit.Mvvm.ComponentModel;
using LanProject.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.MainApplication.Model
{
    public class BasePage : ObservableObject
    {
        private bool allow;
        public bool Allow
        {
            get => allow;
            set => SetProperty(ref allow, value);
        }
        public BasePage()
        {
            Allow = false;
        }
    }
    public class FormTypePage : BasePage
    {
        private int type;
        public int Type
        {
            get => type;
            set => SetProperty(ref type, value);
        }
        public FormTypePage() : base()
        {
            Type = -1;
            PropertyChanged += FormTypePage_PropertyChanged;
        }

        private void FormTypePage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Allow = true;
        }
    }
    public class UnitPage : BasePage
    {
        #region 已存在
        private bool existed;
        public bool Existed
        {
            get => existed;
            set => SetProperty(ref existed, value);
        }
        #endregion
        #region 新增
        private bool create;
        public bool Create
        {
            get => create;
            set => SetProperty(ref create, value);
        }
        #endregion
        public SqliteDatabase db { get; set; }
        #region 讀取
        private void Loading(string execute)
        {
            TotalList = new BindingList<Unit>();
            var res = db.GetDataTable(execute);
            foreach (DataRow row in res.Rows)
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
        #endregion
        public BindingList<Unit> TotalList { get; set; }
        private CreateUnit detail;
        public CreateUnit Detail
        {
            get => detail;
            set => SetProperty(ref detail, value);
        }
        #region Country
        private List<string> country;
        public List<string> Country
        {
            get => country;
            set => SetProperty(ref country, value);
        }
        #endregion
        #region City
        private List<string> city;
        public List<string> City
        {
            get => city;
            set => SetProperty(ref city, value);
        }
        #endregion
        private bool verifyenable;
        public bool VerifyEnable
        {
            get => verifyenable;
            set => SetProperty(ref verifyenable, value);
        }
        private string hint;
        public string Hint
        {
            get => hint;
            set => SetProperty(ref hint, value);
        }
        private string message;
        public string Message
        {
            get => message;
            set => SetProperty(ref message, value);
        }
        public CreateUnit Original;
        public void SetOriginal() => Original = CreateUnit.CloneUnitToCreateUnit(Detail);
        public UnitPage(string execute) : base()
        {
            db = new SqliteDatabase();
            Detail = new CreateUnit();
            VerifyEnable = true;
            Create = true;
            Existed = false;
            Hint = String.Empty;
            Message = String.Empty;
            Original = null;
            Country = Method.Function.CountryList();
            City = Method.Function.RefreshCity(Detail.Location.Country);
            Loading(execute);
            Detail.Location.PropertyChanged += Detail_PropertyChanged;
            Detail.PropertyChanged += Detail_PropertyChanged;
            PropertyChanged += UnitPage_PropertyChanged;
        }
        private void UnitPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Create" && Create)
            {
                Existed = false;
                VerifyEnable = true;
                Allow = false;
                Message = "* 必須先完成檢查才能進行下一步";
                Hint = "新增表單時將會自動新增單位至資料中";
                Detail.Clear();
                Original = null;
            }else if (e.PropertyName == "Existed" && Existed)
            {
                Create = false;
                VerifyEnable = false;
                Allow = true;
                Message = "* 已完成檢查";
                Hint = "若有異動將會同步更新取代原有資料";
                City = Method.Function.RefreshCity(Detail.Location.Country);
                SetOriginal();
            }
        }
        private void Detail_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Country")
                City = Method.Function.RefreshCity(Detail.Location.Country);
            if (Original == null)
            {
                Allow = false;
                VerifyEnable = true;
                Message = "* 必須先完成檢查才能進行下一步";
                Hint = "* 新增表單後連同新增本單位";
            }
            else
            {
                Hint = "* 若有異動將會同步更新取代原有資料";
                if (Detail.IsSame(Original))
                {
                    VerifyEnable = false;
                    Allow = true;
                    Message = "* 已完成檢查";
                    return;
                }
                if (e.PropertyName == "HaveTax" && Detail.HaveTax == Original.HaveTax) Detail.Tax = Original.Tax;
                if (e.PropertyName == "HaveTel" && Detail.HaveTel == Original.HaveTel)
                {
                    Detail.Tel.AreaCode = Original.Tel.AreaCode;
                    Detail.Tel.Number = Original.Tel.Number;
                }
                if (e.PropertyName == "HaveFax" && Detail.HaveFax == Original.HaveFax)
                {
                    Detail.Fax.AreaCode = Original.Fax.AreaCode;
                    Detail.Fax.Number = Original.Fax.Number;
                }
                if (e.PropertyName == "HaveAddress" && Detail.HaveAddress == Original.HaveAddress)
                {
                    Detail.Location.Country = Original.Location.Country;
                    Detail.Location.City = Original.Location.City;
                    Detail.Location.Address = Original.Location.Address;
                }
                VerifyEnable = true;
                Allow = false;
                Message = "* 必須先完成檢查才能進行下一步";
            }
        }
        public bool IsSame()
        {
            if (Original.Name != Detail.Name) return false;
            if (Original.Cel != Detail.Cel) return false;
            if (Original.HaveTax != Detail.HaveTax) return false;
            if (Original.Tax != Detail.Tax) return false;
            if (Original.HaveTel != Detail.HaveTel) return false;
            if (Original.Tel.AreaCode != Detail.Tel.AreaCode) return false;
            if (Original.Tel.Number != Detail.Tel.Number) return false;
            if (Original.HaveFax != Detail.HaveFax) return false;
            if (Original.Fax.AreaCode != Detail.Fax.AreaCode) return false;
            if (Original.Fax.Number != Detail.Fax.Number) return false;
            if (Original.HaveAddress != Detail.HaveAddress) return false;
            if (Original.Location.Country != Detail.Location.Country) return false;
            if (Original.Location.City != Detail.Location.City) return false;
            if (Original.Location.Address != Detail.Location.Address) return false;
            return true;
        }
    }
    public class ProductPage : BasePage
    {
        #region 品名數量
        private int pdlistcount;
        public int PdListCount
        {
            get => pdlistcount;
            set => SetProperty(ref pdlistcount, value);
        }
        #endregion
        private Addr location;
        public Addr Location
        {
            get => location;
            set => SetProperty(ref location, value);
        }
        #region Country
        private List<string> country;
        public List<string> Country
        {
            get => country;
            set => SetProperty(ref country, value);
        }
        #endregion
        #region City
        private List<string> city;
        public List<string> City
        {
            get => city;
            set => SetProperty(ref city, value);
        }
        #endregion

        private BindingList<Product> products;
        public BindingList<Product> ProductList
        {
            get => products;
            set => SetProperty(ref products, value);
        }

        private string error;
        public string Error
        {
            get => error;
            set => SetProperty(ref error, value);
        }

        private double total;
        public double Total
        {
            get => total;
            set
            {
                SetProperty(ref total, value);
                Fivepercent = Math.Round(Total * 0.05, 2);
                Paypal = Total + Fivepercent;
            }
        }

        private double fivepercent;
        public double Fivepercent
        {
            get => fivepercent;
            set => SetProperty(ref fivepercent, value);
        }
        
        private double paypal;
        public double Paypal
        {
            get => paypal;
            set => SetProperty(ref paypal, value);
        }
        public ProductPage() : base()
        {
            Location = new Addr();
            ProductList = new BindingList<Product>();
            PdListCount = 0;
            Error = String.Empty;
            Total = 0.0;
            Country = Method.Function.CountryList();
            City = Method.Function.RefreshCity(Location.Country);
            Location.PropertyChanged += Location_PropertyChanged;
        }

        private void Location_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Country")
                City = Method.Function.RefreshCity(Location.Country);
            if (Location.Country != "無" && Location.City != "無" && Location.Address != String.Empty)
                Allow = true;
            else
                Allow = false;
        }
        
    }
    public class NotePage : BasePage
    {
        public BindingList<string> NoteList { get; set; }
        private string note;
        public string Note
        {
            get => note;
            set => SetProperty(ref note, value);
        }
        public NotePage() : base()
        {
            Note = "無";
            NoteList = Method.Function.ReadingNoteTitleList();
            Allow = true;
        }
    }
    public class FormModel : ObservableObject
    {
        public FormTypePage FirstPage { get; set; }
        public UnitPage SecondPage { get; set; }
        public UnitPage ThirdPage { get; set; }
        public ProductPage FourthPage { get; set; }
        public NotePage FivePage { get; set; }
        public FormModel()
        {
            FirstPage = new FormTypePage();
            SecondPage = new UnitPage("SELECT * FROM myunit;");
            ThirdPage = new UnitPage("SELECT * FROM customunit;");
            FourthPage = new ProductPage();
            FivePage = new NotePage();
        }
    }
    public class ModifyForm : FormModel
    {
        public ModifyForm() : base()
        {
        }
    }
}
