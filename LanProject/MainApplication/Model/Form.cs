using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.MainApplication.Model
{
    public class Form : ModelBase
    {
        private string id;
        private Unit myunit;
        private Unit customunit;
        private int formtype;
        private BindingList<Product> products;
        private double total;
        private string note;
        private Addr location;
        public string ID
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        public int Formtype
        {
            get => formtype;
            set => SetProperty(ref formtype, value);
        }
        public string DisplayType
        {
            get
            {
                if (Formtype == 0)
                    return "報價單";
                if (Formtype == 1)
                    return "請款單";
                return "已完成";
            }
        }
        public double Total
        {
            get => total;
            set => SetProperty(ref total, value);
        }
        public string Note
        {
            get => note;
            set => SetProperty(ref note, value);
        }
        public Unit Myunit
        {
            get => myunit;
            set => SetProperty(ref myunit, value);
        }
        public Unit Customunit
        {
            get => customunit;
            set => SetProperty(ref customunit, value);
        }
        public Addr Location
        {
            get => location;
            set =>SetProperty(ref location, value);
        }
        public BindingList<Product> Products
        {
            get => products;
            set => SetProperty(ref products, value);
        }
        private List<string> users;
        public List<string> Users
        {
            get => users;
            set => SetProperty(ref users, value);
        }
        private List<DateTime> edittimelist;
        public List<DateTime> EditTimeList
        {
            get => edittimelist;
            set => SetProperty(ref edittimelist, value);
        }
    }
}
