using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows;

namespace FLauncher.ViewModel
{

    public class InvitationMultiBoolToVisibilityConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is bool isFriend) || !(values[1] is bool isCurrentUser))
                return Visibility.Collapsed;

            string param = parameter as string;
            if (param == "FalseTrue" && !isFriend && isCurrentUser)
                return Visibility.Visible;

            if (param == "TrueFalse" && isFriend && !isCurrentUser)
                return Visibility.Visible;
            if (param == "FalseFalse" && !isFriend && !isCurrentUser)
                return Visibility.Visible;

            return Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }


    }
