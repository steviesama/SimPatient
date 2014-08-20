using System;

using System.Globalization;
using System.Windows.Data;

namespace SimPatient
{
    /// <summary>
    /// Converts an associated object of type int interpreted as an index to
    /// Medication Route Codes into the appropriate Route String display
    /// contains in the static Routes string array attached to the
    /// Medication class.
    /// </summary>
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
