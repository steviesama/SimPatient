using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPatient
{
    class MedicationDose
    {
        //{Binding Path=Medication.Name...etc}
        public Medication Medication { get; set; }
        public DateTime TimePeriod { get; set; }
    }
}
