using System;

using System.Collections;
using System.Collections.ObjectModel;

namespace SimPatient
{
    /// <summary>
    /// Class that models the tblPatient MySQL database data and provides
    /// methods that marshal such data from the database.
    /// </summary>
	public class Patient
	{
        /// <summary>
        /// An enumeration used to determine the gender of a patient.
        /// </summary>
		public enum PatientGender { Male, Female }

		private static ObservableCollection<Patient> _patients;
        /// <summary>
        /// A static collection of Patient objects that leverages the fact
        /// that ObservableCollection from the System.Collection.ObjectModel namespace
        /// implements INotifyPropertyChanged so that when the collection is modified
        /// any UI components that have this collection set as its DataContext will be
        /// updated each time it is modified.  This collection is used as a list of
        /// patients available to be associated with simulations.
        /// </summary>
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
        /// <summary>
        /// Like the Patients ObservableCollection above, except it is used as
        /// a list of patients associated with the currently selected simulation.
        /// Sometimes the 2 lists can be accessed simultaneously, so 2 collections
        /// were created.
        /// </summary>
		public static ObservableCollection<Patient> PatientPool
		{
			get
			{
				if (_patientPool == null)
					_patientPool = new ObservableCollection<Patient>();

				return _patientPool;
			}
		}

        //---member property reflecting the fields in the database
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

        /// <summary>
        /// Refreshes the Patients static property with all patients in the database
        /// that aren't currently associated with a simulation.
        /// </summary>
		public static void refreshPatients()
		{
			if (MySqlHelper.connect() == false) return;

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery("SELECT * from tblPatient AS p WHERE 0 = (SELECT COUNT(pp.pat_id) FROM tblPatientPool AS pp WHERE pp.pat_id=p.id) ORDER BY name");

			MySqlHelper.disconnect();

			Patients.Clear();
			
			foreach (ArrayList arrayList in response)
				Patients.Add(fromArrayList(arrayList));
		}

        /// <summary>
        /// Refreshes the PatientPool static property with all patients in the database
        /// that are associated with the simulation identified by the passed simId.
        /// </summary>
        /// <param name="simId">A long value holding the simulation id to get
        /// patients associated with.</param>
		public static void refreshPatientPool(long simId)
		{
			if (MySqlHelper.connect() == false) return;

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(
				"SELECT * FROM tblPatient AS p WHERE p.id = (SELECT pp.pat_id FROM tblPatientPool AS pp WHERE pp.pat_id=p.id AND pp.sim_id=" + simId + ") ORDER BY name");

			MySqlHelper.disconnect();

			PatientPool.Clear();

			foreach (ArrayList arrayList in response)
				PatientPool.Add(fromArrayList(arrayList));
		}

        /// <summary>
        /// Refreshes the PatientPool static property with all patients who have
        /// medication administration records logged from the simulation whose id
        /// is passed and on the date specified by date.
        /// </summary>
        /// <param name="simId">A long value containing the simulation id to scan patients for.</param>
        /// <param name="date">A DateTime object holding the date to check MAR records for.</param>
		public static void refreshPatientPoolFromMars(long simId, DateTime date)
		{
			if (MySqlHelper.connect() == false) return;

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(string.Format("CALL patients_from_mars_by_sim({0}, '{1}')", simId, date.ToString("yyyy-MM-dd")));

			MySqlHelper.disconnect();

			PatientPool.Clear();

			foreach (ArrayList arrayList in response)
				PatientPool.Add(fromArrayList(arrayList));
		}

        /// <summary>
        /// Creates a Patient object from the supplied ArrayList.
        /// </summary>
        /// <param name="arrayList">An ArrayList holding the patient fields.</param>
        /// <returns>A Patient object constructed from the supplied ArrayList.</returns>
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
			pat.AdmissionDate = getPatientAdmissionDate(pat.Id);
			pat.ParentSimulation = Simulation.fromMySqlPatientPool(pat.Id);

			return pat;
		}

        /// <summary>
        /// Creates a Patient object from data marshaled from the MySQL database
        /// identified by the passed patId.
        /// </summary>
        /// <param name="patId">A long value holding the id of the patient to marshal.</param>
        /// <returns>A Patient object constructed from the MySQL record marshaled.</returns>
		public static Patient fromMySqlPatient(long patId)
		{
			MySqlHelper.connect();

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(string.Format("SELECT * FROM tblPatient WHERE id={0}", patId));

			MySqlHelper.disconnect();

			if (response.Count == 0) return null;

			return Patient.fromArrayList(response[0] as ArrayList);
		}

        /// <summary>
        /// Fetches the patient admission date from the tblPatientPool. Admission date is
        /// meta-data that exists in a separate table and requires a separate fetch
        /// operation.
        /// </summary>
        /// <param name="patId">A long value holding the id of the patient to check for.</param>
        /// <returns>A DateTime object holding the patient admission date, or DateTime.Now
        /// if no such PatientPool simulation/patient combination exists.</returns>
		public static DateTime getPatientAdmissionDate(long patId)
		{
			if (MySqlHelper.connect() == false) return DateTime.Now;
			ArrayList response = MySqlHelper.dbCon.selectQuery("SELECT admit_date FROM tblPatientPool WHERE pat_id=" + patId);
			MySqlHelper.disconnect();

			if (response.Count == 0) return DateTime.Now;

			return ((DateTime)((ArrayList)response[0])[0]).Subtract(new TimeSpan(3, 0, 0, 0)); ;
		}
	} //End class Patient
} //End namespace SimPatient