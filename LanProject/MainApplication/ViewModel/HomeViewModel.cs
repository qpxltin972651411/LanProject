using CommunityToolkit.Mvvm.ComponentModel;
using LanProject.Domain;
using LanProject.MainApplication.Model;
using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Media;
using System.Windows.Threading;

namespace LanProject.MainApplication.ViewModel
{
    public class Chart
    {
        public SeriesCollection SeriesCollection { get; set; }
        public Func<double, string> Formatter { get; set; }
        public int Year { get; set; }
        public string[] Labels { get; set; }
        public Chart()
        {
            Year = DateTime.Now.Year;
            Labels = new[] { "1月", "2月", "3月", "4月", "5月", "6月", "7月", "8月", "9月", "10月", "11月", "12月" };
            Formatter = value => value.ToString("C0");
            SeriesCollection = new SeriesCollection();
            /*SeriesCollection = new SeriesCollection
            {
                new LineSeries
                {
                    Title = "Series 1",
                    Values = new ChartValues<double> { 4, 6, 5, 2 ,4 }
                },
                new LineSeries
                {
                    Title = "Series 2",
                    
                    Values = new ChartValues<double> { 6, 7, 3, 4 ,6 }
                },
                new ColumnSeries
                {
                    Values = new ChartValues<decimal> { 5, 6, 2, 7 }
                }
            };*/


        }
    }
    public class HomeViewModel : ObservableObject
    {
        public SqliteDatabase db { get; set; }
        public BindingList<Form> TotalList { get; set; }
        public Chart CT1 { get; set; }
        public Chart CT2 { get; set; }
        public Chart CT3 { get; set; }
        public Chart CT4 { get; set; }
        public List<string> unitlist { get; set; }
        private void LoadUnit()
        {
            unitlist = new List<string>();
            var result = db.GetDataTable("SELECT * FROM myunit;");
            foreach (DataRow row in result.Rows)
                unitlist.Add(row["name"].ToString() + row["cel"].ToString());
        }
        public HomeViewModel()
        {
            db = new SqliteDatabase();
            TotalList = new BindingList<Form>();
            LoadUnit();
            CT1 = new Chart();
            CT2 = new Chart();
            CT3 = new Chart();
            CT4 = new Chart();
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
                var utres = db.GetDataTable("SELECT * FROM myunit WHERE name = @name AND cel = @cel AND tax = @tax;", KeyValues, DataTypeList);
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
            LoadT1();
            LoadT2();
            LoadT3();
            LoadT4();
        }
        private void LoadT1()
        {
            Dictionary<string, Dictionary<DateTime, double>> dct = new Dictionary<string, Dictionary<DateTime, double>>();
            foreach (var item in unitlist)
            {
                var nn = new Dictionary<DateTime, double>();
                for (var i = 0; i < 12; i++)
                    nn.Add(new DateTime(CT1.Year, i + 1, 1), 0);
                dct.Add(item, nn);
            }
            foreach (var item in TotalList)
            {
                if (item.Formtype != 0) continue;
                var result = InRangeAndGetLasestDate(item.EditTimeList,CT1);
                if (result != null)
                    dct[item.Myunit.Name + item.Myunit.Cel][result] += item.Total;
                
            }
            
            foreach (var it in dct)
            {
                var yvals = new ChartValues<double>();
                foreach (var item2 in it.Value)
                    yvals.Add(item2.Value);
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                {
                    CT1.SeriesCollection.Add(
                        new LineSeries
                        {
                            Title = it.Key,
                            Values = yvals,
                        }
                    );
                }));
            }
        }
        private void LoadT2()
        {
            Dictionary<string, Dictionary<DateTime, double>> dct = new Dictionary<string, Dictionary<DateTime, double>>();
            foreach (var item in unitlist)
            {
                var nn = new Dictionary<DateTime, double>();
                for (var i = 0; i < 12; i++)
                    nn.Add(new DateTime(CT2.Year, i + 1, 1), 0);
                dct.Add(item, nn);
            }
            foreach (var item in TotalList)
            {
                if (item.Formtype != 1) continue;
                var result = InRangeAndGetLasestDate(item.EditTimeList, CT2);
                if (result != null)
                    dct[item.Myunit.Name + item.Myunit.Cel][result] += item.Total;

            }

            foreach (var it in dct)
            {
                var yvals = new ChartValues<double>();
                foreach (var item2 in it.Value)
                    yvals.Add(item2.Value);
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                {
                    CT2.SeriesCollection.Add(
                        new LineSeries
                        {
                            Title = it.Key,
                            Values = yvals,
                        }
                    );
                }));
            }
        }
        private void LoadT3()
        {
            Dictionary<string, Dictionary<DateTime, double>> dct = new Dictionary<string, Dictionary<DateTime, double>>();
            foreach (var item in unitlist)
            {
                var nn = new Dictionary<DateTime, double>();
                for (var i = 0; i < 12; i++)
                    nn.Add(new DateTime(CT3.Year, i + 1, 1), 0);
                dct.Add(item, nn);
            }
            foreach (var item in TotalList)
            {
                var result = InRangeAndGetLasestDate(item.EditTimeList, CT3);
                if (result != null)
                    dct[item.Myunit.Name + item.Myunit.Cel][result] += item.Total;

            }

            foreach (var it in dct)
            {
                var yvals = new ChartValues<double>();
                foreach (var item2 in it.Value)
                    yvals.Add(item2.Value);
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                {
                    CT3.SeriesCollection.Add(
                        new ColumnSeries
                        {
                            Title = it.Key,
                            Values = yvals,
                        }
                    );
                }));
            }
        }
        private void LoadT4()
        {
            Dictionary<string, Dictionary<DateTime, int>> dct = new Dictionary<string, Dictionary<DateTime, int>>();
            foreach (var item in unitlist)
            {
                var nn = new Dictionary<DateTime, int>();
                for (var i = 0; i < 12; i++)
                    nn.Add(new DateTime(CT4.Year, i + 1, 1), 0);
                dct.Add(item, nn);
            }
            foreach (var item in TotalList)
            {
                var result = InRangeAndGetLasestDate(item.EditTimeList, CT4);
                if (result != null)
                    dct[item.Myunit.Name + item.Myunit.Cel][result] += 1;

            }

            foreach (var it in dct)
            {
                var yvals = new ChartValues<double>();
                foreach (var item2 in it.Value)
                    yvals.Add(item2.Value);
                System.Windows.Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, new Action(() =>
                {
                    CT4.SeriesCollection.Add(
                        new StackedAreaSeries
                        {
                            Title = it.Key,
                            Values = yvals,
                        }
                    );
                }));
            }
        }
        private DateTime InRangeAndGetLasestDate(List<DateTime> source,Chart t)
        {
            DateTime? lasest = null;
            foreach (var item in source)
            {
                if (t.Year == item.Year)
                {
                    if (lasest == null)
                    {
                        lasest = item.Date;
                    }
                    else
                    {
                        if (item.Date > lasest)
                            lasest = item.Date;

                    }
                }
            }
            return new DateTime(lasest.Value.Year,lasest.Value.Month,1);
        }
        public void Loading()
        {
            Reading();
        }
    }
}
