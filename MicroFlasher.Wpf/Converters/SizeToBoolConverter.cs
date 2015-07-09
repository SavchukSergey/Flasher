using System;
using System.Globalization;
using System.Windows.Data;

namespace MicroFlasher.Converters {
    public class SizeToBoolConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var val = System.Convert.ToInt32(value);
            return val > 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
