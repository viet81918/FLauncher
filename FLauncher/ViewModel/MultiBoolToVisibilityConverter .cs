using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace FLauncher.ViewModel
{
    public class MultiBoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (values == null || values.Length < 2)
                return Visibility.Collapsed;

            if (values[0] is bool isBuy && values[1] is bool isPublisher)
            {
                // Only show if IsBuy is true and IsPublisher is false
                return isBuy && !isPublisher ? Visibility.Visible : Visibility.Collapsed;
            }

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("MultiBoolToVisibilityConverter does not support ConvertBack.");
        }
    }

}
