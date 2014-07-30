using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.ObjectModel;

namespace SimPatient
{
	public class MedicationDose
    {
        private static ObservableCollection<MedicationDose> _medicationDosePool;
        public static ObservableCollection<MedicationDose> MedicationDosePool
        {
            get
            {
                if (_medicationDosePool == null)
                    _medicationDosePool = new ObservableCollection<MedicationDose>();

                return _medicationDosePool;
            }
        }

        public long Id { get; set; }
		public Medication ForMedication { get; set; }
		public Patient ForPatient { get; set; }
		public int InjectionSite { get; set; }
		public string Schedule { get; set; }
		public DateTime TimePeriod { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime? StopTime { get; set; }

        public static MedicationDose fromArrayList(ArrayList arrayList)
        {
            MedicationDose dose = new MedicationDose();

            dose.Id = (long)arrayList[0];
            //---switch these 2 lines to a fromMySql(long id) call for respective objects
            dose.ForMedication = Medication.fromMySqlMedication((long)arrayList[1]);
            dose.ForPatient = Patient.fromMySqlPatient((long)arrayList[2]);
            dose.InjectionSite = (sbyte)arrayList[3];
            dose.Schedule = (string)arrayList[4];
            dose.TimePeriod = (DateTime)arrayList[5];
            dose.StartTime = (DateTime)arrayList[6];
            dose.StopTime = null;
            if(arrayList[7].GetType() != typeof(DBNull)) dose.StopTime = (DateTime)arrayList[6];

            return dose;
        }

        public static void refreshMedicationDosePool(long pat_id)
        {
            if (MySqlHelper.connect() == false) return;

            DBConnection dbCon = MySqlHelper.dbCon;            
            ArrayList response = dbCon.selectQuery(string.Format("CALL get_patient_doses({0})", pat_id));

            MySqlHelper.disconnect();

            MedicationDosePool.Clear();

            foreach (ArrayList arrayList in response)
                MedicationDosePool.Add(fromArrayList(arrayList));
        }
	} //End class MedicationDose
}//End namespace SimPatient