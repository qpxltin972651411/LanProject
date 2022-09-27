using LanProject.Domain;
using LanProject.MainApplication.Model;
using MaterialDesignThemes.Wpf;
using MaterialDesignThemes.Wpf.Transitions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace LanProject.MainApplication.View.Transition
{
    /// <summary>
    /// EditFinish.xaml 的互動邏輯
    /// </summary>
    public partial class EditFinish : UserControl
    {
        private LoadingInfoNotificationMessage LoadingDialog { get; set; }
        private EditFormNotificationMessage m { get; set; }
        private readonly string programFilesPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);//LanProject.Method.Configuration.ReadSetting("dataPath")
        private readonly string foldername = LanProject.Method.Configuration.ReadSetting("appfoldername");
        private readonly string notefolder = LanProject.Method.Configuration.ReadSetting("notes");
        private SqliteDatabase db { get; set; }
        private ModifyForm source { get; set; }
        public Transitioner GetTransitioner(UserControl source)
        {
            Transitioner main = null;
            var obj = LogicalTreeHelper.GetParent(source);
            while (obj.GetType() != typeof(Transitioner))
                obj = LogicalTreeHelper.GetParent(obj);
            main = obj as Transitioner;
            return main;
        }
        private Form Container { get; set; }
        public Form GetParent(UserControl source)
        {
            Form main = null;
            var obj = VisualTreeHelper.GetParent(source);
            while (obj.GetType() != typeof(Form))
                obj = VisualTreeHelper.GetParent(obj);
            main = obj as Form;
            return main;
        }
        public EditFinish()
        {
            db = new SqliteDatabase();
            Loaded += EditFinish_Loaded;
            InitializeComponent();
        }
        private void EditFinish_Loaded(object sender, RoutedEventArgs e) => Container = GetParent(this);
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FormNotificationMessage obj = DataContext as FormNotificationMessage;
            if (obj == null) return;
            string keyword = obj.NewForm.FivePage.Note;
            if (keyword == "無")
            {
                viewer.Document.Blocks.Clear();
                return;
            }

            if (File.Exists(System.IO.Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", keyword))))
            {
                var txtRange = new TextRange(viewer.Document.ContentStart, viewer.Document.ContentEnd);
                FileStream fs = File.OpenRead(System.IO.Path.Combine(programFilesPath, foldername, notefolder, String.Format("{0}.xaml", keyword)));
                txtRange.Load(fs, DataFormats.Xaml);
                fs.Close();
            }
        }
        private void FinishButton_Click(object sender, RoutedEventArgs e)
        {
            m = DataContext as EditFormNotificationMessage;
            if (m.IsSame())
            {
                Container.MainPage.PopupNews("表單未更改");
                return;
            }
            source = m.NewForm;
            Update();
        }
        private bool CreateUnit(CreateUnit s, string table1, string table2)
        {
            try
            {
                string executeString = String.Format("INSERT INTO {0}(name,tax,cel,telareacode,telnumber,faxareacode,faxnumber,country,city,address) VALUES(@name,@tax,@cel,@telareacode,@telnumber,@faxareacode,@faxnumber,@country,@city,@address);", table1);
                Dictionary<string, object> KeyValues = new Dictionary<string, object>();
                List<int> DataTypeList = new List<int>();
                KeyValues.Add("@name", s.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@tax", s.HaveTax ? s.Tax : String.Empty);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", s.Cel);
                DataTypeList.Add(0);

                if (s.HaveTel)
                {
                    KeyValues.Add("@telareacode", s.Tel.AreaCode);
                    DataTypeList.Add(0);
                    KeyValues.Add("@telnumber", s.Tel.Number);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@telareacode", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@telnumber", DBNull.Value);
                    DataTypeList.Add(3);
                }

                if (s.HaveFax)
                {
                    KeyValues.Add("@faxareacode", s.Fax.AreaCode);
                    DataTypeList.Add(0);
                    KeyValues.Add("@faxnumber", s.Fax.Number);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@faxareacode", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@faxnumber", DBNull.Value);
                    DataTypeList.Add(3);
                }
                if (s.HaveAddress)
                {
                    KeyValues.Add("@country", s.Location.Country);
                    DataTypeList.Add(0);
                    KeyValues.Add("@city", s.Location.City);
                    DataTypeList.Add(0);
                    KeyValues.Add("@address", s.Location.Address);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@country", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@city", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@address", DBNull.Value);
                    DataTypeList.Add(3);
                }
                db.Manipulate(executeString, KeyValues, DataTypeList);

                string newp = s.GetMergeProperty();

                executeString = String.Format("INSERT INTO {0}(action,newproperty,user,name,tax,cel) VALUES(@action,@newproperty,@user,@name,@tax,@cel);", table2);
                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();

                KeyValues.Add("@action", "新增單位");
                DataTypeList.Add(0);
                KeyValues.Add("@newproperty", newp);
                DataTypeList.Add(0);
                KeyValues.Add("@user", Application.Current.Properties["identity"].ToString());
                DataTypeList.Add(0);
                KeyValues.Add("@name", s.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@tax", s.HaveTax ? s.Tax : String.Empty);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", s.Cel);
                DataTypeList.Add(0);
                db.Manipulate(executeString, KeyValues, DataTypeList);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
        private bool EditUnit(CreateUnit s, CreateUnit sc, string table1, string table2)
        {
            try
            {
                string execute = String.Format("UPDATE {0} SET name = @name , tax = @tax , cel = @cel , telareacode = @telareacode , telnumber = @telnumber , faxareacode = @faxareacode , faxnumber = @faxnumber , country = @country , city = @city , address = @address WHERE name = @keyname AND tax = @keytax AND cel = @keycel ;", table1);
                Dictionary<string, object> KeyValues = new Dictionary<string, object>();
                List<int> DataTypeList = new List<int>();

                KeyValues.Add("@name", s.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@tax", s.HaveTax ? s.Tax : String.Empty);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", s.Cel);
                DataTypeList.Add(0);

                if (s.HaveTel)
                {
                    KeyValues.Add("@telareacode", s.Tel.AreaCode);
                    DataTypeList.Add(0);
                    KeyValues.Add("@telnumber", s.Tel.Number);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@telareacode", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@telnumber", DBNull.Value);
                    DataTypeList.Add(3);
                }

                if (s.HaveFax)
                {
                    KeyValues.Add("@faxareacode", s.Fax.AreaCode);
                    DataTypeList.Add(0);
                    KeyValues.Add("@faxnumber", s.Fax.Number);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@faxareacode", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@faxnumber", DBNull.Value);
                    DataTypeList.Add(3);
                }
                if (s.HaveAddress)
                {
                    KeyValues.Add("@country", s.Location.Country);
                    DataTypeList.Add(0);
                    KeyValues.Add("@city", s.Location.City);
                    DataTypeList.Add(0);
                    KeyValues.Add("@address", s.Location.Address);
                    DataTypeList.Add(0);
                }
                else
                {
                    KeyValues.Add("@country", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@city", DBNull.Value);
                    DataTypeList.Add(3);
                    KeyValues.Add("@address", DBNull.Value);
                    DataTypeList.Add(3);
                }
                KeyValues.Add("@keyname", sc.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@keytax", sc.HaveTax ? sc.Tax : String.Empty);
                DataTypeList.Add(0);
                KeyValues.Add("@keycel", sc.Cel);
                DataTypeList.Add(0);

                db.Manipulate(execute, KeyValues, DataTypeList);

                string oldp = sc.GetMergeProperty();
                string newp = s.GetMergeProperty();

                execute = String.Format("INSERT INTO {0}(action,newproperty,oldproperty,user,name,tax,cel) VALUES(@action,@newproperty,@oldproperty,@user,@name,@tax,@cel);", table2);
                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();

                KeyValues.Add("@action", "編輯單位");
                DataTypeList.Add(0);
                KeyValues.Add("@newproperty", newp);
                DataTypeList.Add(0);
                KeyValues.Add("@oldproperty", oldp);
                DataTypeList.Add(0);
                KeyValues.Add("@user", Application.Current.Properties["identity"].ToString());
                DataTypeList.Add(0);
                KeyValues.Add("@name", s.Name);
                DataTypeList.Add(0);
                KeyValues.Add("@tax", s.HaveTax ? s.Tax : String.Empty);
                DataTypeList.Add(0);
                KeyValues.Add("@cel", s.Cel);
                DataTypeList.Add(0);
                db.Manipulate(execute, KeyValues, DataTypeList);
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

        }
        #region 編輯表單
        public void Update()
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(async () => {
                LoadingDialog = new LoadingInfoNotificationMessage { Title = "編輯表單中", Message = "請稍後" };
                await DialogHost.Show(LoadingDialog, "RootDialog");
            }));
            var bw = new BackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(Edit);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(EditResult);
            bw.RunWorkerAsync();
        }
        private void Edit(object sender, DoWorkEventArgs e)
        {
            if (source.SecondPage.Create)
            {
                CreateUnit(source.SecondPage.Detail, "myunit", "myuniteditrecord");
            }
            else
            {
                if (!source.SecondPage.IsSame())
                    EditUnit(source.SecondPage.Detail, source.SecondPage.Original, "myunit", "myuniteditrecord");
            }
            if (source.ThirdPage.Create)
            {
                CreateUnit(source.ThirdPage.Detail, "customunit", "customuniteditrecord");
            }
            else
            {
                if (!source.ThirdPage.IsSame())
                    EditUnit(source.ThirdPage.Detail, source.ThirdPage.Original, "customunit", "customuniteditrecord");
            }
            EditMainTable();
            EditProductTable();
            InsertFormEditRecord();
        }
        private void EditResult(object sender, RunWorkerCompletedEventArgs e)
        {
            if (DialogHost.IsDialogOpen("RootDialog"))
                DialogHost.Close("RootDialog");
            if (DialogHost.IsDialogOpen("MainDialog"))
                DialogHost.Close("MainDialog");
            GetTransitioner(this).SelectedIndex = 0;
        }
        #endregion
        private  void EditMainTable()
        {
            string execute3 = "UPDATE formtable SET type = @type, mname = @mname, mtax = @mtax, mcel = @mcel, oname = @oname, otax = @otax, ocel = @ocel, note = @note, country = @country, city = @city, address = @address, total = @total WHERE id = @id;";
            var KeyValues = new Dictionary<string, object>();
            var DataTypeList = new List<int>();

            KeyValues.Add("@type", source.FirstPage.Type);
            DataTypeList.Add(1);

            KeyValues.Add("@mname", source.SecondPage.Detail.Name);
            DataTypeList.Add(0);
            KeyValues.Add("@mtax", source.SecondPage.Detail.HaveTax ? source.SecondPage.Detail.Tax : String.Empty);
            DataTypeList.Add(0);
            KeyValues.Add("@mcel", source.SecondPage.Detail.Cel);
            DataTypeList.Add(0);

            KeyValues.Add("@oname", source.ThirdPage.Detail.Name);
            DataTypeList.Add(0);
            KeyValues.Add("@otax", source.ThirdPage.Detail.HaveTax ? source.ThirdPage.Detail.Tax : String.Empty);
            DataTypeList.Add(0);
            KeyValues.Add("@ocel", source.ThirdPage.Detail.Cel);
            DataTypeList.Add(0);

            KeyValues.Add("@note", source.FivePage.Note);
            DataTypeList.Add(0);

            KeyValues.Add("@country", source.FourthPage.Location.Country);
            DataTypeList.Add(0);
            KeyValues.Add("@city", source.FourthPage.Location.City);
            DataTypeList.Add(0);
            KeyValues.Add("@address", source.FourthPage.Location.Address);
            DataTypeList.Add(0);

            KeyValues.Add("@total", source.FourthPage.Total);
            DataTypeList.Add(2);

            KeyValues.Add("@id", m.id);
            DataTypeList.Add(0);
            db.Manipulate(execute3, KeyValues, DataTypeList);
        }
        private void EditProductTable()
        {
            string executeString = "DELETE FROM formdetails WHERE id = @id;";
            Dictionary<string, object> KeyValues = new Dictionary<string, object>();
            List<int> DataTypeList = new List<int>();

            KeyValues.Add("@id", m.id);
            DataTypeList.Add(0);
            db.Manipulate(executeString, KeyValues, DataTypeList);
            foreach (var item in source.FourthPage.ProductList)
            {
                string execute3 = "INSERT INTO formdetails(id,pname,length,width,ironmold,powercoating,ironslips,nut) VALUES(@id,@pname,@length,@width,@ironmold,@powercoating,@ironslips,@nut);";
                KeyValues = new Dictionary<string, object>();
                DataTypeList = new List<int>();

                KeyValues.Add("@id", m.id);
                DataTypeList.Add(0);
                KeyValues.Add("@pname", item.Name);
                DataTypeList.Add(0);

                KeyValues.Add("@length", item.Length);
                DataTypeList.Add(2);
                KeyValues.Add("@width", item.Width);
                DataTypeList.Add(2);
                KeyValues.Add("@ironmold", item.Ironmold);
                DataTypeList.Add(2);
                KeyValues.Add("@powercoating", item.Powercoating);
                DataTypeList.Add(2);
                KeyValues.Add("@ironslips", item.Ironslips);
                DataTypeList.Add(2);
                KeyValues.Add("@nut", item.Nut);
                DataTypeList.Add(2);
                db.Manipulate(execute3, KeyValues, DataTypeList);
            }
        }
        private void InsertFormEditRecord()
        {
            string id2 = String.Empty;
            DataTable res2 = null;
            do
            {
                id2 = String.Format("{0:000}", new Random().Next(1000));
                var execute = "Select * FROM formeditrecord WHERE id = @id AND editid = @editid;";
                var KeyValuesc = new Dictionary<string, object>();
                var DataTypeListc = new List<int>();
                KeyValuesc.Add("@id", m.id);
                DataTypeListc.Add(0);
                KeyValuesc.Add("@editid", String.Format("{0}{1}", m.id, id2));
                DataTypeListc.Add(0);
                res2 = db.GetDataTable(execute, KeyValuesc, DataTypeListc);
            } while (res2.Rows.Count > 0);


            string execute3 = "INSERT INTO formeditrecord(id,title,user,editid) VALUES(@id,@title,@user,@editid);";
            var KeyValues = new Dictionary<string, object>();
            var DataTypeList = new List<int>();

            KeyValues.Add("@id", m.id);
            DataTypeList.Add(0);
            KeyValues.Add("@title", "編輯表單");
            DataTypeList.Add(0);
            KeyValues.Add("@user", Application.Current.Properties["identity"].ToString());
            DataTypeList.Add(0);
            KeyValues.Add("@editid", String.Format("{0}{1}", m.id, id2));
            DataTypeList.Add(0);
            db.Manipulate(execute3, KeyValues, DataTypeList);
            InsertProductDetails(String.Format("{0}{1}", m.id, id2));
            InsertFormEditInfo(String.Format("{0}{1}", m.id, id2));
        }
        private void InsertProductDetails(string id)
        {
            List<int> vs = new List<int>();
            for (int i = 0; i < source.FourthPage.ProductList.Count; i++)
            {
                bool repeat = false;
                for (int j = 0; j < m.Original.FourthPage.ProductList.Count; j++)
                {
                    if ((m.Original.FourthPage.ProductList[j].Name == source.FourthPage.ProductList[i].Name) &&
                        (m.Original.FourthPage.ProductList[j].Length.CompareTo(source.FourthPage.ProductList[i].Length) == 0) &&
                        (m.Original.FourthPage.ProductList[j].Width.CompareTo(source.FourthPage.ProductList[i].Width) == 0) &&
                        (m.Original.FourthPage.ProductList[j].Ironmold.CompareTo(source.FourthPage.ProductList[i].Ironmold) == 0) &&
                        (m.Original.FourthPage.ProductList[j].Powercoating.CompareTo(source.FourthPage.ProductList[i].Powercoating) == 0) && 
                        (m.Original.FourthPage.ProductList[j].Ironslips.CompareTo(source.FourthPage.ProductList[i].Ironslips) == 0) &&
                        (m.Original.FourthPage.ProductList[j].Nut.CompareTo(source.FourthPage.ProductList[i].Nut) == 0))
                    {
                        if (!vs.Contains(j))
                        {
                            repeat = true;
                            vs.Add(j);
                            break;
                        }
                    }
                }
                if (!repeat)
                {
                    var item = source.FourthPage.ProductList[i];
                    var mergestring = String.Format("{0}:::{1}:::{2}:::{3}:::{4}:::{5}", item.Length, item.Width, item.Ironmold, item.Powercoating, item.Ironslips, item.Nut);
                    Istr("producteditinfo", id, "新增", item.Name, String.Empty, mergestring);
                }
            }
            vs = new List<int>();
            for (int i = 0; i < m.Original.FourthPage.ProductList.Count; i++)
            {
                bool repeat = false;
                for (int j = 0; j < source.FourthPage.ProductList.Count; j++)
                {
                    if ((source.FourthPage.ProductList[j].Name == m.Original.FourthPage.ProductList[i].Name) &&
                        (source.FourthPage.ProductList[j].Length.CompareTo(m.Original.FourthPage.ProductList[i].Length) == 0) && 
                        (source.FourthPage.ProductList[j].Width.CompareTo(m.Original.FourthPage.ProductList[i].Width) == 0) && 
                        (source.FourthPage.ProductList[j].Ironmold.CompareTo(m.Original.FourthPage.ProductList[i].Ironmold) == 0) && 
                        (source.FourthPage.ProductList[j].Powercoating.CompareTo(m.Original.FourthPage.ProductList[i].Powercoating) == 0) && 
                        (source.FourthPage.ProductList[j].Ironslips.CompareTo(m.Original.FourthPage.ProductList[i].Ironslips) == 0) && 
                        (source.FourthPage.ProductList[j].Nut.CompareTo(m.Original.FourthPage.ProductList[i].Nut) == 0))
                    {
                        if (!vs.Contains(j))
                        {
                            repeat = true;
                            vs.Add(j);
                            break;
                        }
                    }
                }
                if (!repeat)
                {
                    var item = m.Original.FourthPage.ProductList[i];
                    var mergestring = String.Format("{0}:::{1}:::{2}:::{3}:::{4}:::{5}", item.Length, item.Width, item.Ironmold, item.Powercoating, item.Ironslips, item.Nut);
                    Istr("producteditinfo", id, "移除", item.Name, String.Empty, mergestring);
                }
            }
        }
        private void Istr(string table, string id, string act, string pro, string old_, string new_)
        {
            string execute3 = String.Format("INSERT INTO {0}(editid,action,property,oldvalue,newvalue) VALUES(@editid,@action,@property,@oldvalue,@newvalue);", table);
            var KeyValues = new Dictionary<string, object>();
            var DataTypeList = new List<int>();

            KeyValues.Add("@editid", id);
            DataTypeList.Add(0);
            KeyValues.Add("@action", act);
            DataTypeList.Add(0);
            KeyValues.Add("@property", pro);
            DataTypeList.Add(0);
            KeyValues.Add("@oldvalue", old_);
            DataTypeList.Add(0);
            KeyValues.Add("@newvalue", new_);
            DataTypeList.Add(0);
            db.Manipulate(execute3, KeyValues, DataTypeList);
        }
        private void InsertFormEditInfo(string id)
        {
            if (source.FirstPage.Type != m.Original.FirstPage.Type)
                Istr("formeditinfo", id, "編輯", "表單種類", m.Original.FirstPage.Type == 0 ? "報價單" : "請款單", source.FirstPage.Type == 0 ? "報價單" : "請款單");
            
            if (source.SecondPage.Detail.Name != m.Original.SecondPage.Detail.Name)
                Istr("formeditinfo", id, "編輯", "本單位名稱", m.Original.SecondPage.Detail.Name, source.SecondPage.Detail.Name);
            if (source.SecondPage.Detail.Tax != m.Original.SecondPage.Detail.Tax)
                Istr("formeditinfo", id, "編輯", "本單位統編", m.Original.SecondPage.Detail.Tax, source.SecondPage.Detail.HaveTax ? source.SecondPage.Detail.Tax : String.Empty);
            if (source.SecondPage.Detail.Cel != m.Original.SecondPage.Detail.Cel)
                Istr("formeditinfo", id, "編輯", "本單位Cel", m.Original.SecondPage.Detail.Cel, source.SecondPage.Detail.Cel);


            if (source.ThirdPage.Detail.Name != m.Original.ThirdPage.Detail.Name)
                Istr("formeditinfo", id, "編輯", "客戶單位名稱", m.Original.ThirdPage.Detail.Name, source.ThirdPage.Detail.Name);
            if (source.ThirdPage.Detail.Tax != m.Original.ThirdPage.Detail.Tax)
                Istr("formeditinfo", id, "編輯", "客戶單位統編", m.Original.ThirdPage.Detail.Tax, source.ThirdPage.Detail.HaveTax ? source.ThirdPage.Detail.Tax : String.Empty);
            if (source.ThirdPage.Detail.Cel != m.Original.ThirdPage.Detail.Cel)
                Istr("formeditinfo", id, "編輯", "客戶單位Cel", m.Original.ThirdPage.Detail.Cel, source.ThirdPage.Detail.Cel);

            if (source.FivePage.Note != m.Original.FivePage.Note)
                Istr("formeditinfo", id, "編輯", "備註名稱", m.Original.FivePage.Note, source.FivePage.Note);

            if (source.FourthPage.Location.Country != m.Original.FourthPage.Location.Country)
                Istr("formeditinfo", id, "編輯", "工程縣市", m.Original.FourthPage.Location.Country, source.FourthPage.Location.Country);
            if (source.FourthPage.Location.City != m.Original.FourthPage.Location.City)
                Istr("formeditinfo", id, "編輯", "工程地區", m.Original.FourthPage.Location.City, source.FourthPage.Location.City);
            if (source.FourthPage.Location.Address != m.Original.FourthPage.Location.Address)
                Istr("formeditinfo", id, "編輯", "工程詳細地址", m.Original.FourthPage.Location.Address, source.FourthPage.Location.Address);
            if (source.FourthPage.Total != m.Original.FourthPage.Total)
                Istr("formeditinfo", id, "編輯", "總額", m.Original.FourthPage.Total.ToString("C0"), source.FourthPage.Total.ToString("C0"));
        }
    }
}
