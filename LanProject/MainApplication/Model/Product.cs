using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanProject.MainApplication.Model
{
    public class Product : ModelBase
    {
        private int id;

        private string name;
        private double length;
        private double width;
        private double area;

        private double ironmold;
        private double powercoating;
        private double ironslips;
        private double nut;

        private double total;

        public Product(int i, string n, double l, double w, double im, double pc, double islips, double nt)
        {
            ID = i;
            Name = n;
            Length = l;
            Width = w;
            Ironmold = im;
            Powercoating = pc;
            Ironslips = islips;
            Nut = nt;
        }
        public int ID
        {
            get => id;
            set => SetProperty(ref id, value);
        }
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }
        public double Length
        {
            get => length;
            set
            {
                SetProperty(ref length, value);
                CalcArea();
                CalcTotal();
            }
        }
        public double Width
        {
            get => width;
            set
            {
                SetProperty(ref width, value);
                CalcArea();
                CalcTotal();
            }
        }
        public double Area
        {
            get => area;
            set => SetProperty(ref area, value);
        }
        public double Ironmold
        {
            get => ironmold;
            set
            {
                SetProperty(ref ironmold, value);
                CalcTotal();
            }
        }
        public double Powercoating
        {
            get => powercoating;
            set
            {
                SetProperty(ref powercoating, value);
                CalcTotal();
            }
        }
        public double Ironslips
        {
            get => ironslips;
            set
            {
                SetProperty(ref ironslips, value);
                CalcTotal();
            }
        }
        public double Nut
        {
            get => nut;
            set
            {
                SetProperty(ref nut, value);
                CalcTotal();
            }
        }
        public double Total
        {
            get => total;
            set => SetProperty(ref total, value);
        }
        private void CalcArea()
        {
            Area = 0.0;
            if (Math.Abs(Length) < 0.001) return;
            if (Math.Abs(Width) < 0.001) return;
            Area = Math.Round(Length * Width, 2);
        }
        private void CalcTotal()
        {
            Total = 0.0;
            if (Math.Abs(Length) < 0.001) return;
            if (Math.Abs(Width) < 0.001) return;
            if (Math.Abs(Ironmold) < 0.001) return;
            Total = Math.Round(Length * Width, 2) * (Ironmold + Powercoating + Ironslips + Nut);
        }
    }
}
