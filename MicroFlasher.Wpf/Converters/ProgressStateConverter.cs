using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Shell;

namespace MicroFlasher.Converters {
    public class ProgressStateConverter : IValueConverter {

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            var val = value as DeviceOperationStatus? ?? DeviceOperationStatus.Normal;
            switch (val) {
                case DeviceOperationStatus.Normal:
                    return TaskbarItemProgressState.Normal;
                case DeviceOperationStatus.Error:
                    return TaskbarItemProgressState.Error;
                default:
                    return TaskbarItemProgressState.Normal;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}
