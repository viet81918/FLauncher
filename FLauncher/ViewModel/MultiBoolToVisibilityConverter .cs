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
            // Check if we have the right number of values and they are all booleans
            if (values == null || values.Length == 0)
                return Visibility.Collapsed;

            // Check if all the values are boolean and store them
            bool[] boolValues = values.OfType<bool>().ToArray();

            // Example logic: If all conditions are true, return Visible
            if (boolValues.Length > 0)
            {
                // You can adjust this logic as needed based on the number of booleans and the conditions
                if (boolValues.All(b => b))  // If all booleans are true
                    return Visibility.Visible;
            }

            // Return Collapsed if any of the conditions is false
            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException("MultiBoolToVisibilityConverter does not support ConvertBack.");
        }
    }


}
