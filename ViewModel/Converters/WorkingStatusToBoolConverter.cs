using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows.Data;
using static FileSpy.Model.Enums;

namespace FileSpy.ViewModel.Converters
{
    public class WorkingStatusToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var status = value != null ? (WorkStatus)value : WorkStatus.Idle;

            return status == WorkStatus.Working;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
