using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPatient
{
	public class MedicationDose
	{
		public int Id { get; set; }
		public Medication ForMedication { get; set; }
		public Patient ForPatient { get; set; }
		public int InjectionSite { get; set; }
		public string Schedule { get; set; }
		public DateTime TimePeriod { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime StopTime { get; set; }
	}
}