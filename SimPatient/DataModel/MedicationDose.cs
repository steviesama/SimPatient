using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPatient
{
    class MedicationDose
    {
        public int Id { get; set; }
        public int MedId { get; set; }
        public Medication Medication { get; set; }
        public int InjectionSite { get; set; }
        public string Schedule { get; set; }
        public DateTime TimePeriod { get; set; }
        public DateTime AdministrationTime { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime StopTime { get; set; }
    }
}
