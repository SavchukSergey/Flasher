using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace MicroFlasher.Converters {
    public class GridLengthConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var val = (int)value;
            if (val > 0) return new GridLength(double.Parse(parameter.ToString()), GridUnitType.Star);
            return new GridLength(0, GridUnitType.Pixel);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
