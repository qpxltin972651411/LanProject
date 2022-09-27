using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.MainApplication.Model
{
    public class Contact : ObservableObject
    {
        #region 區域號碼
        private string areacode;
        public string AreaCode
        {
            get => areacode;
            set => SetProperty(ref areacode, value);
        }
        #endregion
        #region 區域號碼
        private string number;
        public string Number
        {
            get => number;
            set => SetProperty(ref number, value);
        }
        #endregion
        public Contact()
        {
            AreaCode = String.Empty;
            Number = String.Empty;
        }
        public Contact(string a,string n)
        {
            AreaCode = a;
            Number = n;
        }
        public void Clear()
        {
            AreaCode = String.Empty;
            Number = String.Empty;
        }
        public string DisplayContact
        {
            get
            {
                if (AreaCode == String.Empty) return "無";
                if (Number == String.Empty) return "無";
                return String.Format("{0}-{1}", AreaCode, Number);
            }
        }
    }
    public class Addr : ObservableObject
    {
        #region 縣市
        private string country;
        public string Country
        {
            get => country;
            set
            {
                SetProperty(ref country, value);
                City = "無";
            }
        }
        #endregion
        #region 地區
        private string city;
        public string City
        {
            get => city;
            set => SetProperty(ref city, value);
        }
        #endregion
        #region 詳細地址
        private string address;
        public string Address
        {
            get => address;
            set => SetProperty(ref address, value);
        }
        #endregion
        public Addr()
        {
            Country = "無";
            City = "無";
            Address = String.Empty;
        }
        public Addr(string ct, string cy,string adr)
        {
            Country = ct;
            City = cy;
            Address = adr;
        }
        public void Clear()
        {
            Country = "無";
            City = "無";
            Address = String.Empty;
        }
        public string DisplayAddress
        {
            get
            {
                if (Country == "無") return "無";
                if (City == "無") return "無";
                if (Address == String.Empty) return "無";
                return String.Format("{0}{1}{2}", Country, City, Address);
            }
        }
    }
    public class Unit : ModelBase
    {
        private string _name;
        private string _tax;
        private string _cel;
        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }
        public string Tax
        {
            get => _tax;
            set => SetProperty(ref _tax, value);
        }
        public string DisplayTax
        {
            get
            {
                if (Tax == String.Empty)
                    return "無";
                return Tax;
            }
        }
        public string Cel
        {
            get => _cel;
            set => SetProperty(ref _cel, value);
        }
        public Contact Tel { get; set; }
        public Contact Fax { get; set; }
        public Addr Location { get; set; }
        public Unit()
        {
            Name = String.Empty;
            Cel = String.Empty;
            Tax = String.Empty;
            Tel = new Contact();
            Fax = new Contact();
            Location = new Addr();
        }
        public void Clear()
        {
            Name = String.Empty;
            Cel = String.Empty;
            Tax = String.Empty;
            Tel.Clear();
            Fax.Clear();
            Location.Clear();
        }
    }
    public class CreateUnit : Unit
    {
        private bool _havetax;
        private bool _havetel;
        private bool _havefax;
        private bool _haveaddress;
        public bool HaveTax
        {
            get => _havetax;
            set
            {
                SetProperty(ref _havetax, value);
                if (!value) Tax = String.Empty;
            }
        }
        public bool HaveTel
        {
            get => _havetel;
            set
            {
                SetProperty(ref _havetel, value);
                if (!value) Tel.Clear();
            }
        }
        public bool HaveFax
        {
            get => _havefax;
            set
            {
                SetProperty(ref _havefax, value);
                if (!value) Fax.Clear();
            }
        }
        public bool HaveAddress
        {
            get => _haveaddress;
            set
            {
                SetProperty(ref _haveaddress, value);
                if (!value) Location.Clear();
            }
        }
        public CreateUnit() : base()
        {
            HaveTax = false;
            HaveTel = false;
            HaveFax = false;
            HaveAddress = false;
        }
        public new void Clear()
        {
            Name = String.Empty;
            Cel = String.Empty;
            HaveTax = false;
            HaveTel = false;
            HaveFax = false;
            HaveAddress = false;
        }
        public static void CopyValueUnitToCreateUnit(Unit source,CreateUnit target)
        {
            target.Name = source.Name;
            target.Cel = source.Cel;
            target.Tax = source.Tax;
            if (String.IsNullOrWhiteSpace(target.Tax))
                target.HaveTax = false;
            else
                target.HaveTax = true;

            target.Fax.AreaCode = source.Fax.AreaCode;
            target.Fax.Number = source.Fax.Number;
            if (String.IsNullOrWhiteSpace(target.Fax.AreaCode) || String.IsNullOrWhiteSpace(target.Fax.Number))
                target.HaveFax = false;
            else
                target.HaveFax = true;

            target.Tel.AreaCode = source.Tel.AreaCode;
            target.Tel.Number = source.Tel.Number;
            if (String.IsNullOrWhiteSpace(target.Tel.AreaCode) || String.IsNullOrWhiteSpace(target.Tel.Number))
                target.HaveTel = false;
            else
                target.HaveTel = true;

            target.Location.Country = source.Location.Country;
            target.Location.City = source.Location.City;
            target.Location.Address = source.Location.Address;
            if (String.IsNullOrWhiteSpace(target.Location.Address) || target.Location.Country == "無" || target.Location.City == "無")
                target.HaveAddress = false;
            else
                target.HaveAddress = true;
        }
        public static CreateUnit CloneUnitToCreateUnit(Unit source)
        {
            CreateUnit NewValue = new CreateUnit();
            NewValue.Name = source.Name;
            NewValue.Cel = source.Cel;
            NewValue.Tax = source.Tax;
            if (String.IsNullOrWhiteSpace(NewValue.Tax))
                NewValue.HaveTax = false;
            else
                NewValue.HaveTax = true;

            NewValue.Fax.AreaCode = source.Fax.AreaCode;
            NewValue.Fax.Number = source.Fax.Number;
            if (String.IsNullOrWhiteSpace(NewValue.Fax.AreaCode) || String.IsNullOrWhiteSpace(NewValue.Fax.Number))
                NewValue.HaveFax = false;
            else
                NewValue.HaveFax = true;

            NewValue.Tel.AreaCode = source.Tel.AreaCode;
            NewValue.Tel.Number = source.Tel.Number;
            if (String.IsNullOrWhiteSpace(NewValue.Tel.AreaCode) || String.IsNullOrWhiteSpace(NewValue.Tel.Number))
                NewValue.HaveTel = false;
            else
                NewValue.HaveTel = true;

            NewValue.Location.Country = source.Location.Country;
            NewValue.Location.City = source.Location.City;
            NewValue.Location.Address = source.Location.Address;
            if (String.IsNullOrWhiteSpace(NewValue.Location.Address) || NewValue.Location.Country == "無" || NewValue.Location.City == "無")
                NewValue.HaveAddress = false;
            else
                NewValue.HaveAddress = true;
            return NewValue;
        }
        public bool IsSame(CreateUnit source)
        {
            if (Name != source.Name)
                return false;
            if (HaveTax != source.HaveTax)
                return false;
            if (Tax != source.Tax)
                return false;
            if (Cel != source.Cel)
                return false;
            if (HaveTel != source.HaveTel)
                return false;
            if (Tel.AreaCode != source.Tel.AreaCode || Tel.Number != source.Tel.Number)
                return false;
            if (HaveFax != source.HaveFax)
                return false;
            if (Fax.AreaCode != source.Fax.AreaCode || Fax.Number != source.Fax.Number)
                return false;
            if (HaveAddress != source.HaveAddress)
                return false;
            if (Location.Country != source.Location.Country)
                return false;
            if (Location.City != source.Location.City)
                return false;
            if (Location.Address != source.Location.Address)
                return false;
            return true;
        }
        public string GetMergeProperty() => String.Format("{0}:::{1}:::{2}:::{3}:::{4}:::{5}:::{6}:::{7}:::{8}:::{9}",
            Name, Tax, Cel,
            HaveTel ? Tel.AreaCode : "null",
            HaveTel ? Tel.Number : "null",
            HaveFax ? Fax.AreaCode : "null",
            HaveFax ? Fax.Number : "null",
            HaveAddress ? Location.Country : "null",
            HaveAddress ? Location.City : "null",
            HaveAddress ? Location.Address : "null");
    }
    public class SearchUnit : CreateUnit
    {
        #region no tax
        private bool _notax;
        public bool Notax
        {
            get => _notax;
            set
            {
                SetProperty(ref _notax, value);
                if (value)
                {
                    Tax = String.Empty;
                    HaveTax = false;
                }
            }
        }
        #endregion
        #region no tel
        private bool _notel;
        public bool Notel
        {
            get => _notel;
            set
            {
                SetProperty(ref _notel, value);
                if (value)
                {
                    Tel.Clear();
                    HaveTel = false;
                }
            }
        }
        #endregion
        #region no fax
        private bool _nofax;
        public bool Nofax
        {
            get => _nofax;
            set
            {
                SetProperty(ref _nofax, value);
                if (value)
                {
                    Fax.Clear();
                    HaveFax = false;
                }
            }
        }
        #endregion
        #region no address
        private bool _noaddress;
        public bool Noaddress
        {
            get => _noaddress;
            set
            {
                SetProperty(ref _noaddress, value);
                if (value)
                {
                    Location.Clear();
                    HaveAddress = false;
                }
            }
        }
        #endregion
        public SearchUnit() : base()
        {
            Notax = false;
            Notel = false;
            Nofax = false;
            Noaddress = false;
        }
    }
}
