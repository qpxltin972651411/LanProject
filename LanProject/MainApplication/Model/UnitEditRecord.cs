using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.MainApplication.Model
{
    public sealed class ProductDetail : Product
    {
        public string Action { get; set; }
        public ProductDetail(string act, int i, string n, double l, double w, double im, double p, double ir, double nt) : base(i, n, l, w, im, p, ir, nt)
        {
            Action = act;
        }
    }
    public sealed class Detail
    {
        public Detail(string oldvalue, string newvalue, string property)
        {
            Oldvalue = oldvalue;
            Newvalue = newvalue;
            Property = property;
        }
        public string Property { get; set; }
        public string Oldvalue { get; set; }
        public string Newvalue { get; set; }
    }
    public sealed class UnitEditRecord
    {
        public UnitEditRecord(string action, string user, DateTime dt, params Detail[] details)
        {
            Action = action;
            User = user;
            ActionTime = dt;

            if (details.Length > 0)
            {
                List<Detail> _filter = new List<Detail>();
                foreach (Detail detail in details)
                {
                    if (detail != null)
                    {
                        if (detail.Property == "縣市" || detail.Property == "地區" || detail.Property == "詳細地址")
                        {
                            if (!(detail.Oldvalue == detail.Newvalue))
                                _filter.Add(detail);
                        }
                        else
                        {
                            _filter.Add(detail);
                        }

                    }

                }
                Details = new BindingList<Detail>(_filter);
            }
            else
            {
                Details = new BindingList<Detail>();
            }
            ProductLists = new BindingList<ProductDetail>();
        }
        public string Action { get; set; }
        public string User { get; set; }
        public DateTime ActionTime { get; set; }
        public BindingList<Detail> Details { get; set; }
        public BindingList<ProductDetail> ProductLists { get; set; }
    }
}
