using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPatient
{
	public class MedicationAdminstrationRecord
	{
		public Patient ForPatient { get; set; }
		public Medication ForMedication { get; set; }
		
		public string Initials { get; set; }
		public DateTime AdministrationTime { get; set; }
		public int ReasonCode { get; set; }
		public string ReasonNotes { get; set; }
		public string AdministrationNotes { get; set; }
	}
}