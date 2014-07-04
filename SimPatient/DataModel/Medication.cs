using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPatient
{
    public class Medication
    {
        public static readonly string[] Routes =
        {
            "IM", "PO", "MORE_ROUTE_CODES"
        };
        public string Name { get; set; }
        public int Strength { get; set; }
        public int Route { get; set; }
    }
} //End namespace SimPatient