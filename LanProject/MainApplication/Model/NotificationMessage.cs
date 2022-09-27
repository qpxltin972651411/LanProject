using CommunityToolkit.Mvvm.ComponentModel;
using LanProject.Domain;
using LanProject.MainApplication.View.Transition;
using LanProject.MainApplication.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Windows;

namespace LanProject.MainApplication.Model
{
    public class NotificationMessage : ObservableObject
    {
        private string _message = String.Empty;
        private string _title = String.Empty;
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
    public class LoadingInfoNotificationMessage : NotificationMessage
    {
        public LoadingInfoNotificationMessage()
        {
            Title = "載入中";
            Message = "請稍後";
        }
    }
    public class YesNoDialogHostModel : NotificationMessage { }
    public class YesNoCancelDialogHostModel : NotificationMessage { }

    public class EnterNoteNameInfoNotificationMessage : NotificationMessage
    {
        #region Note Name
        private string _notename;
        public string NoteName
        {
            get => _notename;
            set => SetProperty(ref _notename, value);
        }
        #endregion
        private bool allow;
        public bool Allow
        {
            get => allow;
            set => SetProperty(ref allow, value);
        }
        private string error;
        public string Error
        {
            get => error;
            set => SetProperty(ref error, value);
        }
        public EnterNoteNameInfoNotificationMessage()
        {
            Title = "請輸入備註名稱";
            Message = "備註名稱";
            NoteName = String.Empty;
            Error = String.Empty;
            Allow = false;
            PropertyChanged += EnterNoteNameInfoNotificationMessage_PropertyChanged;
        }
        private void EnterNoteNameInfoNotificationMessage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (NoteName == String.Empty)
                Allow = false;
            else
                Allow = true;
        }
    }

    public class CreateUnitNotificationMessage : NotificationMessage
    {
        #region Create Unit
        private CreateUnit _newunit;
        public CreateUnit NewUnit
        {
            get => _newunit;
            set => SetProperty(ref _newunit, value);
        }
        #endregion
        #region Submit Is not Enable
        private bool _allow;
        public bool Allow
        {
            get => _allow;
            set
            {
                SetProperty(ref _allow, value);
                OnPropertyChanged();
            }
        }
        #endregion
        #region Country
        private List<string> _country;
        public List<string> Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }
        #endregion
        #region City
        private List<string> _city;
        public List<string> City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
        #endregion
        public CreateUnitNotificationMessage()
        {
            Title = "新增單位";
            Message = "填寫資訊";
            Allow = false;
            NewUnit = new CreateUnit();
            Country = Method.Function.CountryList();
            City = Method.Function.RefreshCity(NewUnit.Location.Country);
            NewUnit.PropertyChanged += NewUnit_PropertyChanged;
            NewUnit.Location.PropertyChanged += NewUnit_PropertyChanged;
            NewUnit.Tel.PropertyChanged += NewUnit_PropertyChanged;
            NewUnit.Fax.PropertyChanged += NewUnit_PropertyChanged;
        }
        private void NewUnit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Country")
                City = Method.Function.RefreshCity(NewUnit.Location.Country);
            Allow = Method.Function.VerifyInput(NewUnit);
        }
    }
    public class DeleteInfoNotificationMessage : NotificationMessage
    {
        public DeleteInfoNotificationMessage()
        {
            Title = "刪除";
        }
    }
    public class SearchNotificationMessage : NotificationMessage
    {
        #region Search Unit
        private SearchUnit _searchunit;
        public SearchUnit Search
        {
            get => _searchunit;
            set => SetProperty(ref _searchunit, value);
        }
        #endregion
        #region Current state is not in searching

        private bool _state;
        public bool State
        {
            get => _state;
            set
            {
                SetProperty(ref _state, value);
                OnPropertyChanged();
            }
        }
        #endregion
        #region Country
        private List<string> _country;
        public List<string> Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }
        #endregion
        #region City
        private List<string> _city;
        public List<string> City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
        #endregion
        private void Search_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Country")
                City = Method.Function.RefreshCity(Search.Location.Country);
            Status(e.PropertyName);
            State = Enable();
        }
        private void Status(string n)
        {
            if (n == "HaveTax")
            {
                if (Search.HaveTax && Search.Notax)
                    Search.Notax = false;
            }
            if (n == "HaveTel")
            {
                if (Search.HaveTel && Search.Notel)
                    Search.Notel = false;
            }
            if (n == "HaveFax")
            {
                if (Search.HaveFax && Search.Nofax)
                    Search.Nofax = false;
            }
            if (n == "HaveAddress")
            {
                if (Search.HaveAddress && Search.Noaddress)
                    Search.Noaddress = false;
            }
        }
        private bool Enable()
        {
            if (!String.IsNullOrWhiteSpace(Search.Name)) return true;
            if (!String.IsNullOrWhiteSpace(Search.Cel)) return true;

            if (Search.HaveTax) return true;
            if (Search.HaveTel) return true;
            if (Search.HaveFax) return true;
            if (Search.HaveAddress) return true;

            if (Search.Notax) return true;
            if (Search.Notel) return true;
            if (Search.Nofax) return true;
            if (Search.Noaddress) return true;
            return false;
        }
        public void Clear()
        {
            Search = new SearchUnit();
            Search.PropertyChanged += Search_PropertyChanged;
            Search.Location.PropertyChanged += Search_PropertyChanged;
            Search.Tel.PropertyChanged += Search_PropertyChanged;
            Search.Fax.PropertyChanged += Search_PropertyChanged;
            State = false;
        }
        public SearchNotificationMessage()
        {
            Title = "搜尋單位";
            Message = "填寫欲搜尋的相關資訊";
            Clear();
            Country = Method.Function.CountryList();
            City = Method.Function.RefreshCity(Search.Location.Country);
        }
    }
    public class EditNotificationMessage : NotificationMessage
    {
        #region Edit Unit
        private CreateUnit _editunit;
        public CreateUnit Edit
        {
            get => _editunit;
            set => SetProperty(ref _editunit, value);
        }
        #endregion
        #region Submit Is not Enable
        private bool _allow;
        public bool Allow
        {
            get => _allow;
            set
            {
                SetProperty(ref _allow, value);
                OnPropertyChanged();
            }
        }
        #endregion
        #region Country
        private List<string> _country;
        public List<string> Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }
        #endregion
        #region City
        private List<string> _city;
        public List<string> City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
        #endregion
        private CreateUnit Original;
        public CreateUnit GetOriginal() => Original;
        public EditNotificationMessage(Unit source)
        {
            Title = "編輯單位";
            Message = "修改資訊";
            Allow = false;

            Edit = CreateUnit.CloneUnitToCreateUnit(source);
            Original = CreateUnit.CloneUnitToCreateUnit(source);
            Edit.PropertyChanged += Edit_PropertyChanged;
            Edit.Location.PropertyChanged += Edit_PropertyChanged;
            Edit.Tel.PropertyChanged += Edit_PropertyChanged;
            Edit.Fax.PropertyChanged += Edit_PropertyChanged;

            Country = Method.Function.CountryList();
            City = Method.Function.RefreshCity(Edit.Location.Country);
        }
        private void Edit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Country")
                City = Method.Function.RefreshCity(Edit.Location.Country);
            if (e.PropertyName == "HaveTax" && Edit.HaveTax == Original.HaveTax) Edit.Tax = Original.Tax;
            if (e.PropertyName == "HaveTel" && Edit.HaveTel == Original.HaveTel)
            {
                Edit.Tel.AreaCode = Original.Tel.AreaCode;
                Edit.Tel.Number = Original.Tel.Number;
            }
            if (e.PropertyName == "HaveFax" && Edit.HaveFax == Original.HaveFax)
            {
                Edit.Fax.AreaCode = Original.Fax.AreaCode;
                Edit.Fax.Number = Original.Fax.Number;
            }
            if (e.PropertyName == "HaveAddress" && Edit.HaveAddress == Original.HaveAddress)
            {
                Edit.Location.Country = Original.Location.Country;
                Edit.Location.City = Original.Location.City;
                Edit.Location.Address = Original.Location.Address;
            }
            Allow = Method.Function.VerifyInput(Edit);
            Allow = !Edit.IsSame(Original);
        }
    }
    public class UnitDetailNotificationMessage : NotificationMessage
    {
        private SqliteDatabase db { get; }

        private Unit source;
        public Unit Source
        {
            get => source;
            set => SetProperty(ref source, value);
        }

        private BindingList<UnitEditRecord> editrecord;
        public BindingList<UnitEditRecord> EditRecord
        {
            get => editrecord;
            set => SetProperty(ref editrecord, value);
        }

        private BindingList<UnitEditRecord> quotationrecord;
        public BindingList<UnitEditRecord> QuotationRecord
        {
            get => quotationrecord;
            set => SetProperty(ref quotationrecord, value);
        }
        public string QuotationDollar { get; set; }

        private BindingList<UnitEditRecord> invoicerecord;
        public BindingList<UnitEditRecord> InvoiceRecord
        {
            get => invoicerecord;
            set => SetProperty(ref invoicerecord, value);
        }
        public string InvoiceDollar { get; set; }
        public UnitDetailNotificationMessage(Unit s,int mode)
        {
            Source = s;
            db = new SqliteDatabase();
            string tbname = "myuniteditrecord";
            string executeQuotation = "Select id,country,city,address,total FROM formtable WHERE mname = @name AND mtax = @tax AND mcel = @cel AND type = @type;";
            if (mode == 1)
            {
                tbname = "customuniteditrecord";
                executeQuotation = "Select id,country,city,address,total FROM formtable WHERE oname = @name AND otax = @tax AND ocel = @cel AND type = @type;";
            }
            LoadEditRecord(tbname);
            LoadQuotationRecord(executeQuotation);
            LoadInvoiceRecord(executeQuotation);
        }
        public void LoadEditRecord(string tb)
        {
            EditRecord = new BindingList<UnitEditRecord>();
            string executeString = String.Format("Select action,newproperty,oldproperty,user,name,tax,cel,datetime(Timestamp,'localtime') FROM {0} WHERE name = @name AND tax = @tax AND cel = @cel;",tb);
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            List<int> DataTypeList = new List<int>();
            KeyValues.Add("@name", Source.Name);
            DataTypeList.Add(0);
            KeyValues.Add("@tax", Source.Tax);
            DataTypeList.Add(0);
            KeyValues.Add("@cel", Source.Cel);
            DataTypeList.Add(0);
            var result = db.GetDataTable(executeString, KeyValues, DataTypeList);
            foreach (DataRow dataRow in result.Rows)
            {
                string Title = dataRow["action"].ToString();
                string newproperty = dataRow["newproperty"].ToString();
                string oldproperty = dataRow["oldproperty"].ToString();
                string user = dataRow["user"].ToString();
                DateTime t = Convert.ToDateTime(dataRow.ItemArray[7].ToString());
                if (Title == "新增單位")
                {
                    var tmp = newproperty.Split(new string[] { ":::" }, StringSplitOptions.None);
                    EditRecord.Add(
                        new UnitEditRecord(Title, user, t,
                        new Detail("新增", tmp[0], "名稱"),
                        new Detail("新增", tmp[2], "Cel"),
                        new Detail("新增", (tmp[1] == String.Empty) ? "無" : tmp[1], "統編"),
                        new Detail("新增", (tmp[3] == "null") ? "無" : tmp[3], "Tel區碼"),
                        new Detail("新增", (tmp[4] == "null") ? "無" : tmp[4], "Tel號碼"),
                        new Detail("新增", (tmp[5] == "null") ? "無" : tmp[5], "Fax區碼"),
                        new Detail("新增", (tmp[6] == "null") ? "無" : tmp[6], "Fax號碼"),
                        new Detail("新增", (tmp[7] == "null") ? "無" : tmp[7], "縣市"),
                        new Detail("新增", (tmp[8] == "null") ? "無" : tmp[8], "地區"),
                        new Detail("新增", (tmp[9] == "null") ? "無" : tmp[9], "詳細地址"))
                    );
                }
                else if (Title == "編輯單位")
                {
                    var tmp = newproperty.Split(new string[] { ":::" }, StringSplitOptions.None);
                    var oldtmp = oldproperty.Split(new string[] { ":::" }, StringSplitOptions.None);
                    EditRecord.Add(
                        new UnitEditRecord(Title, user, t,
                        tmp[0] != oldtmp[0] ? new Detail(oldtmp[0], tmp[0], "名稱") : null,
                        tmp[1] != oldtmp[1] ? new Detail(oldtmp[1] == String.Empty ? "無" : oldtmp[1], (tmp[1] == String.Empty) ? "無" : tmp[1], "統編") : null,
                        tmp[2] != oldtmp[2] ? new Detail(oldtmp[2], tmp[2], "Cel") : null,
                        tmp[3] != oldtmp[3] ? new Detail((oldtmp[3] == "null") ? "無" : oldtmp[3],(tmp[3] == "null") ? "無" : tmp[3], "Tel區碼") : null,
                        tmp[4] != oldtmp[4] ? new Detail((oldtmp[4] == "null") ? "無" : oldtmp[4],(tmp[4] == "null") ? "無" : tmp[4], "Tel號碼") : null,
                        tmp[5] != oldtmp[5] ? new Detail((oldtmp[5] == "null") ? "無" : oldtmp[5], (tmp[5] == "null") ? "無" : tmp[5], "Fax區碼") : null,
                        tmp[6] != oldtmp[6] ? new Detail((oldtmp[6] == "null") ? "無" : oldtmp[6], (tmp[6] == "null") ? "無" : tmp[6], "Fax號碼") : null,
                        tmp[7] != oldtmp[7] ? new Detail((oldtmp[7] == "null") ? "無" : oldtmp[7], (tmp[7] == "null") ? "無" : tmp[7], "縣市") : null,
                        tmp[8] != oldtmp[8] ? new Detail((oldtmp[8] == "null") ? "無" : oldtmp[8], (tmp[8] == "null") ? "無" : tmp[8], "地區") : null,
                        tmp[9] != oldtmp[9] ? new Detail((oldtmp[9] == "null") ? "無" : oldtmp[9], (tmp[9] == "null") ? "無" : tmp[9], "詳細地址") : null
                        )
                    );
                }
            }
        }
        public void LoadQuotationRecord(string execute)
        {
            QuotationRecord = new BindingList<UnitEditRecord>();
            string executeString = execute;
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            List<int> DataTypeList = new List<int>();
            KeyValues.Add("@name", Source.Name);
            DataTypeList.Add(0);
            KeyValues.Add("@tax", Source.Tax);
            DataTypeList.Add(0);
            KeyValues.Add("@cel", Source.Cel);
            DataTypeList.Add(0);
            KeyValues.Add("@type", 0);
            DataTypeList.Add(1);
            var result = db.GetDataTable(executeString, KeyValues, DataTypeList);
            var temp = 0.0;
            foreach (DataRow dataRow in result.Rows)
            {
                string id = dataRow["id"].ToString();
                string country = dataRow["country"].ToString();
                string city = dataRow["city"].ToString();
                string addr = dataRow["address"].ToString();
                string total = Convert.ToDouble(dataRow["total"].ToString()).ToString("C0");

                temp += Convert.ToDouble(dataRow["total"].ToString());

                executeString = "Select user,Timestamp FROM formeditrecord s1 WHERE id = @id AND Timestamp = (SELECT MAX(Timestamp) FROM formeditrecord s2 WHERE s1.id = s2.id) ORDER BY id, Timestamp;";
                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();
                KeyValues.Add("@id", id);
                DataTypeList.Add(0);
                var result2 = db.GetDataTable(executeString, KeyValues, DataTypeList);
                DataRow dr2 = result2.Rows[0];
                string user = dr2["user"].ToString();
                DateTime dt = Convert.ToDateTime(dr2["Timestamp"].ToString()).AddHours(8);
                QuotationRecord.Add(
                    new UnitEditRecord(
                        String.Format("表單ID {0}", id), user, dt,
                        new Detail("", country, "工程縣市"),
                        new Detail("", city, "工程地區"),
                        new Detail("", addr, "工程地址"),
                        new Detail("", total, "總金額"),
                        new Detail("", user, "最後編輯人"),
                        new Detail("", dt.ToString(), "最後編輯時間")
                    )
                );
            }
            QuotationDollar = temp.ToString("C0");
        }
        public void LoadInvoiceRecord(string execute)
        {
            InvoiceRecord = new BindingList<UnitEditRecord>();
            string executeString = execute;
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            List<int> DataTypeList = new List<int>();
            KeyValues.Add("@name", Source.Name);
            DataTypeList.Add(0);
            KeyValues.Add("@tax", Source.Tax);
            DataTypeList.Add(0);
            KeyValues.Add("@cel", Source.Cel);
            DataTypeList.Add(0);
            KeyValues.Add("@type", 1);
            DataTypeList.Add(1);
            var result = db.GetDataTable(executeString, KeyValues, DataTypeList);
            var temp = 0.0;
            foreach (DataRow dataRow in result.Rows)
            {
                string id = dataRow["id"].ToString();
                string country = dataRow["country"].ToString();
                string city = dataRow["city"].ToString();
                string addr = dataRow["address"].ToString();
                string total = Convert.ToDouble(dataRow["total"].ToString()).ToString("C0");

                temp += Convert.ToDouble(dataRow["total"].ToString());

                executeString = "Select user,Timestamp FROM formeditrecord s1 WHERE id = @id AND Timestamp = (SELECT MAX(Timestamp) FROM formeditrecord s2 WHERE s1.id = s2.id) ORDER BY id, Timestamp;";
                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();
                KeyValues.Add("@id", id);
                DataTypeList.Add(0);
                var result2 = db.GetDataTable(executeString, KeyValues, DataTypeList);
                DataRow dr2 = result2.Rows[0];
                string user = dr2["user"].ToString();
                DateTime dt = Convert.ToDateTime(dr2["Timestamp"].ToString()).AddHours(8);
                InvoiceRecord.Add(
                    new UnitEditRecord(
                        String.Format("表單ID {0}", id), user, dt,
                        new Detail("", country, "工程縣市"),
                        new Detail("", city, "工程地區"),
                        new Detail("", addr, "工程地址"),
                        new Detail("", total, "總金額"),
                        new Detail("", user, "最後編輯人"),
                        new Detail("", dt.ToString(), "最後編輯時間")
                    )
                );
            }
            InvoiceDollar = temp.ToString("C0");
        }
    }


    public class CreateNoteNotificationMessage : NotificationMessage
    {
        private bool allow;
        public bool Allow
        {
            get => allow;
            set => SetProperty(ref allow, value);
        }
        public CreateNoteNotificationMessage()
        {
            Title = "新增備註";
            Allow = false;
        }
    }
    public class EditNoteNotificationMessage : NotificationMessage
    {
        private bool allow;
        public bool Allow
        {
            get => allow;
            set => SetProperty(ref allow, value);
        }
        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public EditNoteNotificationMessage(string n)
        {
            Title = "編輯備註";
            Allow = false;
            Name = n;
        }
    }

    public class PDFViewerNotificationMessage : NotificationMessage
    {
        private string url;
        public string Url
        {
            get => url;
            set => SetProperty(ref url, value);
        }
        public PDFViewerNotificationMessage(string u)
        {
            Url = u;
        }
    }
    public class BaseSearchModel : ObservableObject
    {
        private bool nix;
        public bool Nix
        {
            get => nix;
            set => SetProperty(ref nix, value);
        }
        public BaseSearchModel() => Nix = true;
        public bool IsNix() => Nix;
    }
    public class SearchByDatePick : BaseSearchModel
    {
        private DateTime begintime;
        public DateTime BeginTime
        {
            get => begintime;
            set
            {
                if ((value > EndTime) && Range)
                {
                    MessageBox.Show("日期不應於後者之後", "Date pick error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                SetProperty(ref begintime, value);
            }
        }
        private DateTime endtime;
        public DateTime EndTime
        {
            get => endtime;
            set
            {
                if ((BeginTime > value) && Range)
                {
                    MessageBox.Show("日期不應於前者之前", "Date pick error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                SetProperty(ref endtime, value);
            }
        }
        private bool before;
        public bool Before
        {
            get => before;
            set
            {
                SetProperty(ref before, value);
                if (value)
                {
                    Nix = false;
                    EndTime = DateTime.MaxValue.AddYears(-1);
                    BeginTime = DateTime.Now;
                    EndTime = DateTime.Now;
                }
            }
        }
        private bool range;
        public bool Range
        {
            get => range;
            set
            {
                SetProperty(ref range, value);
                if (value)
                {
                    Nix = false;
                    EndTime = DateTime.MaxValue.AddYears(-1);
                    BeginTime = DateTime.Now;
                    EndTime = DateTime.Now;
                }
            }
        }
        private bool after;
        public bool After
        {
            get => after;
            set
            {
                SetProperty(ref after, value);
                if (value)
                {
                    Nix = false;
                    EndTime = DateTime.MaxValue.AddYears(-1);
                    BeginTime = DateTime.Now;
                    EndTime = DateTime.Now;
                }
            }
        }
        public SearchByDatePick() : base()
        {
            Before = false;
            Range = false;
            After = false;
            EndTime = DateTime.MaxValue;
            BeginTime = DateTime.Now;
            EndTime = DateTime.Now;
        }
        public void Clear()
        {
            Before = false;
            Range = false;
            After = false;
            EndTime = DateTime.MaxValue;
            BeginTime = DateTime.Now;
            EndTime = DateTime.Now;
            Nix = true;
        }
    }
    public class SearchByFormType : BaseSearchModel
    {
        private bool t1;
        public bool T1
        {
            get => t1;
            set
            {
                SetProperty(ref t1, value);
                if (value) Nix = false;
            }
        }
        private bool t2;
        public bool T2
        {
            get => t2;
            set
            {
                SetProperty(ref t2, value);
                if (value) Nix = false;
            }
        }
        public SearchByFormType() : base()
        {
            T1 = false;
            T2 = false;
        }
        public void Clear()
        {
            T1 = false;
            T2 = false;
            Nix = true;
        }
    }
    public class SearchByLocation : BaseSearchModel
    {
        private bool have;
        public bool Have
        {
            get => have;
            set
            {
                SetProperty(ref have, value);
                if (value)
                {
                    Location.Clear();
                    Nix = false;
                }
            }
        }
        private Addr location;
        public Addr Location
        {
            get => location;
            set => SetProperty(ref location, value);
        }
        #region Country
        private List<string> _country;
        public List<string> Country
        {
            get => _country;
            set => SetProperty(ref _country, value);
        }
        #endregion
        #region City
        private List<string> _city;
        public List<string> City
        {
            get => _city;
            set => SetProperty(ref _city, value);
        }
        #endregion
        public SearchByLocation() : base()
        {
            Have = false;
            Location = new Addr();
            Country = Method.Function.CountryList();
            City = Method.Function.RefreshCity(Location.Country);
        }
        public void Clear()
        {
            Have = false;
            Location.Clear();
            Nix = true;
        }
    }   //E
    public class SearchByUnit : BaseSearchModel
    {
        private Unit unit;
        public Unit Unit
        {
            get => unit;
            set => SetProperty(ref unit, value);
        }
        private bool nix;
        public new bool Nix
        {
            get => nix;
            set
            {
                if (value)
                {
                    Unit = null;
                    Have = false;
                }
                SetProperty(ref nix, value);
            }
        }
        private bool have;
        public bool Have
        {
            get => have;
            set
            {
                if (value) Nix = false;
                SetProperty(ref have, value);
            }
        }
        private int mode;
        public SearchByUnit(int idx) : base()
        {
            Nix = true;
            mode = idx;
            Have = false;
            Unit = null;
            PropertyChanged += SearchByUnit_PropertyChanged;
        }
        public void Clear()
        {
            Nix = true;
            Have = false;
            Unit = null;
        }
        private void SearchByUnit_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (mode == 0)
            {
                if (e.PropertyName == "Have" && Have)
                {
                    Window window = new Window { Title = "選擇自單位", WindowState = WindowState.Maximized };
                    window.Content = new MyUnitList();

                    if (window.ShowDialog() == true)
                    {
                        var Receive = window.Content as MyUnitList;
                        if (Receive.Get() == null)
                            Nix = true;
                        else
                            Unit = Receive.Get();
                    }
                    else
                    {
                        Nix = true;
                    }
                }
            }
            else
            {
                if (e.PropertyName == "Have" && Have)
                {
                    Window window = new Window { Title = "選擇他單位", WindowState = WindowState.Maximized };
                    window.Content = new CustomUnitList();

                    if (window.ShowDialog() == true)
                    {
                        var Receive = window.Content as CustomUnitList;
                        if (Receive.Get() == null)
                            Nix = true;
                        else
                            Unit = Receive.Get();
                    }
                    else
                    {
                        Nix = true;
                    }
                }
            }
        }
    }
    public class SearchByUser : BaseSearchModel
    {
        private bool have;
        public bool Have
        {
            get => have;
            set
            {
                if (value) Nix = false;
                SetProperty(ref have, value);
            }
        }
        public List<string> Users { get; set; }
        private SqliteDatabase db { get; set; }
        public ObservableCollection<string> SelectedUsers { get; set; }
        public SearchByUser() : base()
        {
            db = new SqliteDatabase();
            Have = false;
            SelectedUsers = new ObservableCollection<string>();
            Users = new List<string>();
            var result = db.GetDataTable("SELECT * FROM userlist;");
            foreach (DataRow item in result.Rows)
                Users.Add(item["nickname"].ToString());
        }
        public void Clear()
        {
            Nix = true;
            Have = false;
            SelectedUsers.Clear();
        }
    }       //E
    public class SearchByMoney : BaseSearchModel
    {
        private int begin;
        public int Begin
        {
            get => begin;
            set
            {
                if ((value > End) && Range)
                {
                    MessageBox.Show("前者不應超過後者", String.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                SetProperty(ref begin, value);
            }
        }
        private int end;
        public int End
        {
            get => end;
            set
            {
                if ((Begin > value) && Range)
                {
                    MessageBox.Show("後者不應小於前者", String.Empty, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                SetProperty(ref end, value);
            }
        }
        private bool before;
        public bool Before
        {
            get => before;
            set
            {
                SetProperty(ref before, value);
                if (value)
                {
                    Nix = false;
                    Begin = 0;
                }
            }
        }
        private bool range;
        public bool Range
        {
            get => range;
            set
            {
                SetProperty(ref range, value);
                if (value)
                {
                    Nix = false;
                    Begin = 0;
                    End = 0;
                }
            }
        }
        private bool after;
        public bool After
        {
            get => after;
            set
            {
                SetProperty(ref after, value);
                if (value)
                {
                    Nix = false;
                    Begin = 0;
                }
            }
        }
        public SearchByMoney() : base()
        {
            Before = false;
            Range = false;
            After = false;
            Begin = 0;
            End = 0;
        }
        public void Clear()
        {
            Before = false;
            Range = false;
            After = false;
            Begin = 0;
            End = 0;
            Nix = true;
        }
    }
    public class FormSearchNotificationMessage : NotificationMessage
    {
        #region Current state is not in searching

        private bool _state;
        public bool State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }
        #endregion
        private FormViewModel Source { get; set; }
        public SearchByDatePick DateFilter { get; set; }
        public SearchByFormType TypeFilter { get; set; }
        public SearchByLocation LocationFilter { get; set; }
        public SearchByUnit My { get; set; }
        public SearchByUnit Custom { get; set; }
        public SearchByUser UserFilter { get; set; }
        public SearchByMoney MoneyFilter { get; set; }
        public FormSearchNotificationMessage(FormViewModel s)
        {
            Title = "搜尋表單";
            Message = "填寫欲搜尋的相關資訊";
            Source = s;

            DateFilter = new SearchByDatePick();
            DateFilter.PropertyChanged += Filter_PropertyChanged;

            TypeFilter = new SearchByFormType();
            TypeFilter.PropertyChanged += Filter_PropertyChanged;

            LocationFilter = new SearchByLocation();
            LocationFilter.PropertyChanged += Filter_PropertyChanged;
            LocationFilter.Location.PropertyChanged += Location_PropertyChanged;

            My = new SearchByUnit(0);
            My.PropertyChanged += Filter_PropertyChanged;
            Custom = new SearchByUnit(1);
            Custom.PropertyChanged += Filter_PropertyChanged;

            UserFilter = new SearchByUser();
            UserFilter.PropertyChanged += Filter_PropertyChanged;
            UserFilter.SelectedUsers.CollectionChanged += SelectedUsers_CollectionChanged;

            MoneyFilter = new SearchByMoney();
            MoneyFilter.PropertyChanged += Filter_PropertyChanged;
        }

        private void SelectedUsers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Search_PropertyChanged(sender, null);
        }

        private void Location_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Country")
                LocationFilter.City = Method.Function.RefreshCity(LocationFilter.Location.Country);
            Search_PropertyChanged(sender, e);
        }

        private void Search_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(Searching);
            bw.RunWorkerAsync();
        }
        private void Searching(object sender, DoWorkEventArgs e)
        {
            if (Source.CloneList == null)
                Source.CloneList = new BindingList<Form>(Source.TotalList);
            Source.TotalList = new BindingList<Form>(Source.CloneList);

            if (DateFilter.Before)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.EditTimeList.Any(y => DateFilter.BeginTime.Date >= y.Date)).ToList());
            else if (DateFilter.Range)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.EditTimeList.Any(y => (DateFilter.BeginTime.Date <= y.Date && DateFilter.EndTime.Date >= y.Date))).ToList());
            else if (DateFilter.After)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.EditTimeList.Any(y => DateFilter.BeginTime.Date <= y.Date)).ToList());

            if (TypeFilter.T1)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.Formtype == 0).ToList());
            else if (TypeFilter.T2)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.Formtype == 1).ToList());

            if (LocationFilter.Have)
            {
                if (LocationFilter.Location.Country != "無")
                    Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.Location.Country == LocationFilter.Location.Country).ToList());
                if (LocationFilter.Location.City != "無")
                    Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.Location.City == LocationFilter.Location.City).ToList());
                if (LocationFilter.Location.Address != String.Empty)
                    Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.Location.Address.Contains(LocationFilter.Location.Address)).ToList());
            }

            if (My.Have && My.Unit != null)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => (x.Myunit.Name == My.Unit.Name && x.Myunit.Cel == My.Unit.Cel && x.Myunit.Tax == My.Unit.Tax)).ToList());
            if (Custom.Have && Custom.Unit != null)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => (x.Customunit.Name == Custom.Unit.Name && x.Customunit.Cel == Custom.Unit.Cel && x.Customunit.Tax == Custom.Unit.Tax)).ToList());

            if (UserFilter.Have && UserFilter.SelectedUsers.Count > 0)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.Users.Any(y => UserFilter.SelectedUsers.Contains(y))).ToList());

            if (MoneyFilter.Before)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.Total <= MoneyFilter.Begin).ToList());
            else if (MoneyFilter.Range)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => (x.Total >= MoneyFilter.Begin && x.Total <= MoneyFilter.End)).ToList());
            else if (MoneyFilter.After)
                Source.TotalList = new BindingList<Form>(Source.TotalList.Where(x => x.Total >= MoneyFilter.Begin).ToList());
            Source.Filter();
        }
        private void Filter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (DateFilter.Nix && TypeFilter.Nix && LocationFilter.Nix && My.Nix && Custom.Nix && UserFilter.Nix && MoneyFilter.Nix)
                State = false;
            else
                State = true;
            Search_PropertyChanged(sender, e);  
        }
        public void Clear()
        {
            DateFilter.Clear();
            TypeFilter.Clear();
            LocationFilter.Clear();
            My.Clear();
            Custom.Clear();
            UserFilter.Clear();
            MoneyFilter.Clear();
        }
    }
}
