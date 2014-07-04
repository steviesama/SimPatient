using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Globalization;
using System.Windows.Data;

namespace SimPatient
{
    [ValueConversion(typeof(object), typeof(string))]
    public class RouteToStringConverter : BaseConverter, IValueConverter
    {
        public RouteToStringConverter() { /*shut-up compiler*/ }
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int routeIndex = (int)value;
            if (routeIndex >= 0 && routeIndex < Medication.Routes.Length)
                return Medication.Routes[routeIndex];
            return "NONE";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
} //End namespace SimPatient
