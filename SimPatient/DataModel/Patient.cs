using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.ObjectModel;

namespace SimPatient
{
	public class Patient
	{
		public enum PatientGender { Male, Female }

		private static ObservableCollection<Patient> _patients;
		public static ObservableCollection<Patient> Patients
		{
			get
			{
				if (_patients == null)
					_patients = new ObservableCollection<Patient>();

				return _patients;
			}
		}

		private static ObservableCollection<Patient> _patientPool;
		public static ObservableCollection<Patient> PatientPool
		{
			get
			{
				if (_patientPool == null)
					_patientPool = new ObservableCollection<Patient>();

				return _patientPool;
			}
		}

		public long Id { get; set; }
		public string Name { get; set; }
		public DateTime DateOfBirth { get; set; }
		public string Allergies { get; set; } 
		public string Diagnosis { get; set; }
		public string DrName { get; set; }
		public string Diet { get; set; }
		public string RoomNumber { get; set; }
		public short Weight { get; set; }
		public PatientGender Gender { get; set; }
		public string Notes { get; set; }
		//the simulation this object is associated with
		public Simulation ParentSimulation { get; set; }
		public DateTime AdmissionDate { get; set; }
		public string Creator { get; set; }

		public static void refreshPatients()
		{
			if (MySqlHelper.connect() == false) return;

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery("SELECT * from tblPatient AS p WHERE 0 = (SELECT COUNT(pp.pat_id) FROM tblPatientPool AS pp WHERE pp.pat_id=p.id)");

			MySqlHelper.disconnect();

			Patients.Clear();
			
			foreach (ArrayList arrayList in response)
				Patients.Add(fromArrayList(arrayList));
		}

		public static void refreshPatientPool(long simId)
		{
			if (MySqlHelper.connect() == false) return;

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(
				"SELECT * from tblPatient AS p WHERE p.id = (SELECT pp.pat_id FROM tblPatientPool AS pp WHERE pp.pat_id=p.id AND pp.sim_id=" + simId + ")");

			MySqlHelper.disconnect();

			PatientPool.Clear();

			foreach (ArrayList arrayList in response)
				PatientPool.Add(fromArrayList(arrayList));
		}

		public static Patient fromArrayList(ArrayList arrayList)
		{
			Patient pat = new Patient();

			pat.Id               = (long)arrayList[0];
			pat.Name             = (string)arrayList[1];
			pat.DateOfBirth      = (DateTime)arrayList[2];
			pat.Allergies        = (string)arrayList[3];
			pat.Diagnosis        = (string)arrayList[4];
			pat.DrName           = (string)arrayList[5];
			pat.Diet             = (string)arrayList[6];
			pat.RoomNumber       = (string)arrayList[7];
			pat.Weight           = (short)arrayList[8];
			pat.Gender           = ((string)arrayList[9]) == "MALE" ? PatientGender.Male : PatientGender.Female;
			pat.Notes            = (string)arrayList[10];
			pat.ParentSimulation = Simulation.fromMySqlUserAccountPool(pat.Id);

			return pat;
		}

		//public static void refreshUserAccounts()
		//{
		//    if (MySqlHelper.connect() == false) return;

		//    DBConnection dbCon = MySqlHelper.dbCon;
		//    ArrayList response = dbCon.selectQuery(
		//        "SELECT * from tblUserAccount AS ua WHERE 0 = (SELECT COUNT(uap.user_id) FROM tblUserAccountPool AS uap WHERE uap.user_id=ua.id) AND ua.type=1");

		//    MySqlHelper.disconnect();

		//    UserAccounts.Clear();

		//    foreach (ArrayList arrayList in response)
		//        UserAccounts.Add(fromArrayList(arrayList));
		//}

		//public static void refreshUserAccountPool(long simId)
		//{
		//    if (MySqlHelper.connect() == false) return;

		//    DBConnection dbCon = MySqlHelper.dbCon;
		//    ArrayList response = dbCon.selectQuery(
		//        "SELECT * from tblUserAccount AS ua WHERE ua.id = (SELECT uap.user_id FROM tblUserAccountPool AS uap WHERE uap.user_id=ua.id AND uap.sim_id=" + simId + ")");

		//    MySqlHelper.disconnect();

		//    UserAccounts.Clear();

		//    foreach (ArrayList arrayList in response)
		//        UserAccounts.Add(fromArrayList(arrayList));
		//}

	} //End class Patient
} //End namespace SimPatient