using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimPatient
{
	public class Patient
    {
        public enum PatientGender { Male, Female }

		public int Id { get; set; }
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Allergies { get; set; }
		public string Diagnosis { get; set; }
		public string DrName { get; set; }
		public string Diet { get; set; }
		public string RoomNumber { get; set; }
		public int Weight { get; set; }
		public PatientGender Gender { get; set; }
		public string Notes { get; set; }
		//the simulation this object is associated with
		public Simulation ParentSimulation { get; set; }
        public DateTime AdmissionDate { get; set; }
        public string Creator { get; set; }
	}
}