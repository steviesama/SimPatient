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
            "IM", //intramuscular (injection)
            "PO", //per os (by mouth)
            "MORE_ROUTE_CODES"
        };
        
		public int Id { get; set; }
		public string Name { get; set; }
		public string Strength { get; set; }
		public int Route { get; set; }
	}
}