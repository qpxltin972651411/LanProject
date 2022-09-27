using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LanProject.MainApplication.Converter
{
    internal class FormTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int buttonId = System.Convert.ToInt32(parameter);
            int groupValue = System.Convert.ToInt32(value);
            return buttonId == groupValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int buttonId = System.Convert.ToInt32(parameter);
            bool isChecked = System.Convert.ToBoolean(value);
            return isChecked ? buttonId : Binding.DoNothing;
        }
    }
}
