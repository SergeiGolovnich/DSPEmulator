using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;

namespace DSPEmulatorUI.ValueConverters
{
    class DoubleValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value != null) 
            {
                return $"{ value }";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                double output = 0;
                double.TryParse(((string)value).Replace('.', ','), out output);
                return output;
            }
            return 0;
        }
    }
}
