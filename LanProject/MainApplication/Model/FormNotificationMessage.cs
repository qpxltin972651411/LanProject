using LanProject.Domain;
using LanProject.MainApplication.ValidationExceptionBehavior;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace LanProject.MainApplication.Model
{
    public class FormNotificationMessage : NotificationMessage, IMySelfValidationException, ICustomValidationException, IFormValidationException
    {
        public int MySelfErrorCount { get; set; }
        public int CustomErrorCount { get; set; }
        public int FormErrorCount { get; set; }
        public Dictionary<int, int> RowErrorCount { get; set; }
        public SqliteDatabase db { get; set; }
        #region New Form
        private ModifyForm newform;
        public ModifyForm NewForm
        {
            get => newform;
            set => SetProperty(ref newform, value);
        }
        #endregion
        public int CalcMySelfErrorCount()
        {
            int result = MySelfErrorCount;
            if (!NewForm.SecondPage.Detail.HaveTax) result -= 1;
            if (!NewForm.SecondPage.Detail.HaveTel) result -= 2;
            if (!NewForm.SecondPage.Detail.HaveFax) result -= 2;
            if (!NewForm.SecondPage.Detail.HaveAddress) result -= 3;
            return result;
        }
        public int CalcCustomErrorCount()
        {
            int result = CustomErrorCount;
            if (!NewForm.ThirdPage.Detail.HaveTax) result -= 1;
            if (!NewForm.ThirdPage.Detail.HaveTel) result -= 2;
            if (!NewForm.ThirdPage.Detail.HaveFax) result -= 2;
            if (!NewForm.ThirdPage.Detail.HaveAddress) result -= 3;
            return result;
        }
        public int CalcFormErrorCount() => FormErrorCount;
        public FormNotificationMessage()
        {
            Title = "新增表單";
            Message = "填寫資訊";
            db = new SqliteDatabase();
            NewForm = new ModifyForm();
            if (RowErrorCount == null)
                RowErrorCount = new Dictionary<int, int>();
        }
        public void InsertNewRowToProductList()
        {
            var initrow = new Product(NewForm.FourthPage.PdListCount, String.Empty, 0, 0, 0, 0, 0, 0);
            NewForm.FourthPage.PdListCount += 1;
            initrow.PropertyChanged += Initrow_PropertyChanged;
            NewForm.FourthPage.ProductList.Add(initrow);
            Initrow_PropertyChanged(null, null);
        }
        public void InsertNewRowToProductList(ModifyForm source)
        {
            var initrow = new Product(source.FourthPage.PdListCount, String.Empty, 0, 0, 0, 0, 0, 0);
            source.FourthPage.PdListCount += 1;
            source.FourthPage.ProductList.Add(initrow);
        }

        public void CloneOneRowToProductList(Product s)
        {
            var idx = NewForm.FourthPage.ProductList.IndexOf(s);
            var _new_clone = new Product(NewForm.FourthPage.PdListCount, s.Name, s.Length, s.Width, s.Ironmold, s.Powercoating, s.Ironslips, s.Nut);
            NewForm.FourthPage.PdListCount += 1;
            _new_clone.PropertyChanged += Initrow_PropertyChanged;
            NewForm.FourthPage.ProductList.Insert(idx, _new_clone);
            Initrow_PropertyChanged(null, null);
        }
        public void DeleteOneRowToProductList(Product s)
        {
            if (RowErrorCount.ContainsKey(s.ID))
            {
                FormErrorCount -= RowErrorCount[s.ID];
                RowErrorCount.Remove(s.ID);
            }
            NewForm.FourthPage.ProductList.Remove(s);
            Initrow_PropertyChanged(null, null);
        }
        private void Initrow_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (FormErrorCount == 0)
                NewForm.FourthPage.Allow = true;
            else
                NewForm.FourthPage.Allow = false;
            double sum = 0;
            foreach (var item in NewForm.FourthPage.ProductList)
            {
                if (item.Name == String.Empty) return;
                sum = sum + item.Total;
            }
            NewForm.FourthPage.Total = sum;
        }
    }
    public class CreateFormNotificationMessage : FormNotificationMessage
    {
        public CreateFormNotificationMessage() : base()
        {

        }
    }
    public class EditFormNotificationMessage : CreateFormNotificationMessage
    {
        public ModifyForm Original { get; set; }
        public string id { get; set; }
        private void init(ModifyForm target,Form source,bool productinsert)
        {
            target.FirstPage.Type = source.Formtype;
            target.FirstPage.Allow = true;

            target.SecondPage.Existed = true;
            CreateUnit.CopyValueUnitToCreateUnit(source.Myunit, target.SecondPage.Detail);
            target.SecondPage.SetOriginal();
            target.SecondPage.City = Method.Function.RefreshCity(target.SecondPage.Detail.Location.Country);
            target.SecondPage.Allow = true;
            target.SecondPage.VerifyEnable = false;
            target.SecondPage.Message = "* 已完成檢查";

            target.ThirdPage.Existed = true;
            CreateUnit.CopyValueUnitToCreateUnit(source.Customunit, target.ThirdPage.Detail);
            target.ThirdPage.SetOriginal();
            target.ThirdPage.City = Method.Function.RefreshCity(target.ThirdPage.Detail.Location.Country);
            target.ThirdPage.Allow = true;
            target.ThirdPage.VerifyEnable = false;
            target.ThirdPage.Message = "* 已完成檢查";

            target.FourthPage.Location.Country = source.Location.Country;
            target.FourthPage.Location.City = source.Location.City;
            target.FourthPage.Location.Address = source.Location.Address;
            for (var i = 0; i < source.Products.Count; i++)
            {
                if (productinsert)
                    InsertNewRowToProductList(target);
                else
                    InsertNewRowToProductList();
                target.FourthPage.ProductList[i].Name = source.Products[i].Name;
                target.FourthPage.ProductList[i].Width = source.Products[i].Width;
                target.FourthPage.ProductList[i].Length = source.Products[i].Length;
                target.FourthPage.ProductList[i].Ironmold = source.Products[i].Ironmold;
                target.FourthPage.ProductList[i].Powercoating = source.Products[i].Powercoating;
                target.FourthPage.ProductList[i].Ironslips = source.Products[i].Ironslips;
                target.FourthPage.ProductList[i].Nut = source.Products[i].Nut;
            }
            if (productinsert)
            {
                double sum = 0;
                foreach (var item in target.FourthPage.ProductList)
                    sum = sum + item.Total;
                target.FourthPage.Total = sum;
            }
            target.FivePage.Note = source.Note;
        }
        public EditFormNotificationMessage(Form source) : base()
        {
            id = source.ID;
            Title = "編輯表單";
            Message = "編輯資訊";
            init(NewForm, source,false);
            Original = new ModifyForm();
            init(Original, source,true);
        }
        public bool IsSame()
        {
            if (Original.FirstPage.Type != NewForm.FirstPage.Type) return false;

            if (NewForm.SecondPage.Create) return false;
            if (NewForm.SecondPage.Detail.Name != Original.SecondPage.Detail.Name) return false;
            if (NewForm.SecondPage.Detail.Cel != Original.SecondPage.Detail.Cel) return false;
            if (NewForm.SecondPage.Detail.Tax != Original.SecondPage.Detail.Tax) return false;
            if (NewForm.SecondPage.Detail.Tel.AreaCode != Original.SecondPage.Detail.Tel.AreaCode) return false;
            if (NewForm.SecondPage.Detail.Tel.Number != Original.SecondPage.Detail.Tel.Number) return false;
            if (NewForm.SecondPage.Detail.Fax.AreaCode != Original.SecondPage.Detail.Fax.AreaCode) return false;
            if (NewForm.SecondPage.Detail.Fax.Number != Original.SecondPage.Detail.Fax.Number) return false;
            if (NewForm.SecondPage.Detail.Location.Country != Original.SecondPage.Detail.Location.Country) return false;
            if (NewForm.SecondPage.Detail.Location.City != Original.SecondPage.Detail.Location.City) return false;
            if (NewForm.SecondPage.Detail.Location.Address != Original.SecondPage.Detail.Location.Address) return false;

            if (NewForm.ThirdPage.Create) return false;
            if (NewForm.ThirdPage.Detail.Name != Original.ThirdPage.Detail.Name) return false;
            if (NewForm.ThirdPage.Detail.Cel != Original.ThirdPage.Detail.Cel) return false;
            if (NewForm.ThirdPage.Detail.Tax != Original.ThirdPage.Detail.Tax) return false;
            if (NewForm.ThirdPage.Detail.Tel.AreaCode != Original.ThirdPage.Detail.Tel.AreaCode) return false;
            if (NewForm.ThirdPage.Detail.Tel.Number != Original.ThirdPage.Detail.Tel.Number) return false;
            if (NewForm.ThirdPage.Detail.Fax.AreaCode != Original.ThirdPage.Detail.Fax.AreaCode) return false;
            if (NewForm.ThirdPage.Detail.Fax.Number != Original.ThirdPage.Detail.Fax.Number) return false;
            if (NewForm.ThirdPage.Detail.Location.Country != Original.ThirdPage.Detail.Location.Country) return false;
            if (NewForm.ThirdPage.Detail.Location.City != Original.ThirdPage.Detail.Location.City) return false;
            if (NewForm.ThirdPage.Detail.Location.Address != Original.ThirdPage.Detail.Location.Address) return false;


            if (Original.FourthPage.Location.Country != NewForm.FourthPage.Location.Country) return false;
            if (Original.FourthPage.Location.City != NewForm.FourthPage.Location.City) return false;
            if (Original.FourthPage.Location.Address != NewForm.FourthPage.Location.Address) return false;

            if (Original.FivePage.Note != NewForm.FivePage.Note) return false;

            if (Original.FourthPage.ProductList.Count != NewForm.FourthPage.ProductList.Count) return false;

            List<int> vs = new List<int>();
            for (int i = 0; i < NewForm.FourthPage.ProductList.Count; i++)
            {
                for (int j = 0; j < Original.FourthPage.ProductList.Count; j++)
                {
                    if (vs.Contains(j)) continue;
                    if (!(Original.FourthPage.ProductList[j].Name == NewForm.FourthPage.ProductList[i].Name)) continue;
                    if (Original.FourthPage.ProductList[j].Length.CompareTo(NewForm.FourthPage.ProductList[i].Length) != 0) continue;
                    if (Original.FourthPage.ProductList[j].Width.CompareTo(NewForm.FourthPage.ProductList[i].Width) != 0) continue;
                    if (Original.FourthPage.ProductList[j].Ironmold.CompareTo(NewForm.FourthPage.ProductList[i].Ironmold) != 0) continue;
                    if (Original.FourthPage.ProductList[j].Powercoating.CompareTo(NewForm.FourthPage.ProductList[i].Powercoating) != 0) continue;
                    if (Original.FourthPage.ProductList[j].Ironslips.CompareTo(NewForm.FourthPage.ProductList[i].Ironslips) != 0) continue;
                    if (Original.FourthPage.ProductList[j].Nut.CompareTo(NewForm.FourthPage.ProductList[i].Nut) != 0) continue;
                    vs.Add(j);
                    break;
                }
            }
            if (vs.Count != NewForm.FourthPage.ProductList.Count) return false;
            return true;
        }
    }
    public class FormDetailNotificationMessage : NotificationMessage
    {
        private Form source;
        public Form Source
        {
            get => source;
            set => SetProperty(ref source, value);
        }
        private BindingList<UnitEditRecord> _displayitemrecord;
        public BindingList<UnitEditRecord> DisplayItemRecord
        {
            get => _displayitemrecord;
            set => SetProperty(ref _displayitemrecord, value);
        }
        public FormDetailNotificationMessage(Form s, SqliteDatabase db)
        {
            Source = s;
            Load(db);
        }
        public void Load(SqliteDatabase db)
        {
            DisplayItemRecord = new BindingList<UnitEditRecord>();
            string executeString = "Select title,user,editid,Timestamp FROM formeditrecord WHERE id = @id;";
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            List<int> DataTypeList = new List<int>();
            KeyValues.Add("@id", Source.ID);
            DataTypeList.Add(0);
            var result = db.GetDataTable(executeString, KeyValues, DataTypeList);

            foreach (DataRow dataRow in result.Rows)
            {
                string Title = dataRow["title"].ToString();
                string user = dataRow["user"].ToString();
                string editid = dataRow["editid"].ToString();
                DateTime t = Convert.ToDateTime(dataRow["Timestamp"].ToString()).AddHours(8);
                var temp = new UnitEditRecord(Title, user, t);

                executeString = "Select action,property,oldvalue,newvalue FROM formeditinfo WHERE editid = @editid;";
                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();
                KeyValues.Add("@editid", editid);
                DataTypeList.Add(0);
                var result2 = db.GetDataTable(executeString, KeyValues, DataTypeList);
                foreach (DataRow dr2 in result2.Rows)
                {
                    string property = dr2["property"].ToString();
                    string oldvalue = dr2["oldvalue"].ToString();
                    string newvalue = dr2["newvalue"].ToString();
                    temp.Details.Add(new Detail(oldvalue, newvalue, property));
                }

                executeString = "Select action,property,oldvalue,newvalue FROM producteditinfo WHERE editid = @editid;";
                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();
                KeyValues.Add("@editid", editid);
                DataTypeList.Add(0);
                result2 = db.GetDataTable(executeString, KeyValues, DataTypeList);
                foreach (DataRow dr2 in result2.Rows)
                {
                    string act = dr2["action"].ToString();
                    string property = dr2["property"].ToString();
                    string oldvalue = dr2["oldvalue"].ToString();
                    string newvalue = dr2["newvalue"].ToString();
                    var spt = newvalue.Split(new string[] { ":::" }, StringSplitOptions.None);
                    temp.ProductLists.Add(new ProductDetail(act, -1, property, Convert.ToDouble(spt[0].ToString()),
                        Convert.ToDouble(spt[1].ToString()), Convert.ToDouble(spt[2].ToString()), Convert.ToDouble(spt[3].ToString()),
                        Convert.ToDouble(spt[4].ToString()), Convert.ToDouble(spt[5].ToString())
                        ));
                }
                if (temp.Details.Count > 0 || temp.ProductLists.Count > 0)
                    DisplayItemRecord.Add(temp);
            }
        }
    }
}
