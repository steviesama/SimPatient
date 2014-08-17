using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPatient
{
    /// <summary>
    /// This class contains static functions that handle conversions between various types.
    /// </summary>
    public static class Convert
    {
        /// <summary>
        /// Takes a time in hhmm format and validates it for proper length
        /// and time range, and returns a string in MySQL DATETIME format
        /// if the input is valid.
        /// </summary>
        /// <param name="time">A string holding the time value to validate and return formatted.</param>
        /// <returns>A valid MySQL DATETIME string if time is valid, or an empty string if not.</returns>
        public static string timeStrToMySqlDateStr(string time)
        {
            int _time = 0;

            if (time.Length != 4) return string.Empty;
            else if (int.TryParse(time, out _time) == false) return string.Empty;
            else if (_time < 0 || _time > 2359) return string.Empty;
            
            string hour = time.Substring(0, 2);
            string minute = time.Substring(2, 2);
            
            return DateTime.Now.ToString("yyyy-MM-dd") + string.Format(" {0}:{1}:00", hour, minute);
        }

        /// <summary>
        /// Takes a DateTime object and return a string in HHmm format.
        /// </summary>
        /// <param name="dateTime">The DateTime object to convert to a time string.</param>
        /// <returns>A time string in HHmm format.</returns>
        public static string dateTimeToTimeStr(DateTime dateTime)
        {
            return dateTime.ToString("HHmm");
        }
    } //End class Convert
} //End namespace SimPatient
