using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPatient
{
    public static class Convert
    {
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

        public static string dateTimeToTimeStr(DateTime dateTime)
        {
            return dateTime.ToString("HHmm");
        }
    } //End class Convert
} //End namespace SimPatient
