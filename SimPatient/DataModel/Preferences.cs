using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPatient.DataModel
{
    class Preferences
    {
        public static string HostAddress { get; set; }
        public static string PortAddress { get; set; }
        public static string DatabaseName { get; set; }
        public static string Username { get; set; }
        public static string Password { get; set; }
    }
}
