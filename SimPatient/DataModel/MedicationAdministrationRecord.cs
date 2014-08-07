using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections.ObjectModel;
using System.Collections;

using PatientGender = SimPatient.Patient.PatientGender;

namespace SimPatient
{
	public class MedicationAdminstrationRecord
	{
		public MedicationAdminstrationRecord()
		{
			ReasonCode = 0;
			ReasonNotes = string.Empty;
		}

		private static ObservableCollection<MedicationAdminstrationRecord> _records;
		public static ObservableCollection<MedicationAdminstrationRecord> Records
		{
			get
			{
				if (_records == null)
					_records = new ObservableCollection<MedicationAdminstrationRecord>();

				return _records;
			}
		}

		public static readonly string[] Reasons =
		{
			"On Schedule",
			"Patient Not Available",
			"Nurse Not Available",
			"Medication Not Available",
			"Patient Refused",
			"Other"
		};

		public Patient ForPatient { get; set; }
		public MedicationDose ForDose { get; set; }
		
		public string Initials { get; set; }
		public DateTime AdministrationTime { get; set; }
		public int ReasonCode { get; set; }
		public string AdministrationNotes { get; set; }
		public string ReasonNotes { get; set; }

		public static void refreshRecords(long pat_id, DateTime date)
		{
			if (MySqlHelper.connect() == false) return;

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(string.Format("CALL get_mars({0}, '{1}')", pat_id, date.ToString("yyyy-MM-dd")));

			MySqlHelper.disconnect();

			Records.Clear();

			foreach (ArrayList arrayList in response)
				Records.Add(fromArrayList(arrayList));
		}

		public static MedicationAdminstrationRecord fromArrayList(ArrayList arrayList)
		{
			MedicationAdminstrationRecord mar = new MedicationAdminstrationRecord();
			Medication med = null;
			MedicationDose dose = new MedicationDose();
			Patient pat = new Patient();

			//---keys
			med = Medication.fromMySqlMedication((long)arrayList[0]);
			pat.ParentSimulation = Simulation.fromMySql((long)arrayList[1]);
			pat.Id = (long)arrayList[2];

			//---unique fields
			mar.Initials = (string)arrayList[3];
			//mar.AdministrationTime = null;
			/*if (arrayList[4].GetType() != typeof(DBNull))*/ mar.AdministrationTime = (DateTime)arrayList[4];
			mar.ReasonCode = (sbyte)arrayList[5];
			mar.AdministrationNotes = (string)arrayList[6];
			mar.ReasonNotes = (string)arrayList[7];

			//patient pool field
			pat.AdmissionDate = (DateTime)arrayList[8];

			//---patient duplication
			pat.Name = (string)arrayList[9];
			pat.DateOfBirth = (DateTime)arrayList[10];
			pat.Allergies = (string)arrayList[11];
			pat.Diagnosis = (string)arrayList[12];
			pat.DrName = (string)arrayList[13];
			pat.Diet = (string)arrayList[14];
			pat.RoomNumber = (string)arrayList[15];
			pat.Weight = (short)arrayList[16];
			pat.Gender = ((string)arrayList[17]) == "MALE" ? PatientGender.Male : PatientGender.Female;
			pat.Notes = (string)arrayList[18];

			//---medication dose duplication
			dose.InjectionSite = (sbyte)arrayList[19];
			dose.Schedule = (string)arrayList[20];
			dose.TimePeriod = (DateTime)arrayList[21];
			dose.StartTime = (DateTime)arrayList[22];
			dose.Id = (long)arrayList[23];
			
			dose.ForMedication = med;
			mar.ForDose = dose;
			mar.ForPatient = pat;

			return mar;
		} //End fromArrayList()
	} //End class MedicationAdministrationRecord
} //End namespace SimPatient