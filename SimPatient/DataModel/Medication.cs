using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Collections.ObjectModel;

namespace SimPatient
{
	public class Medication
	{
		private static ObservableCollection<Medication> _medicationPool;
		public static ObservableCollection<Medication> MedicationPool
		{
			get
			{
				if (_medicationPool == null)
					_medicationPool = new ObservableCollection<Medication>();

				return _medicationPool;
			}
		}

		public static readonly string[] Routes =
		{
			"IM", //intramuscular (injection)
			"ID", //intradermal (injection)
			"SQ", //subcutaneous (injection)
			"IV", //intraveinous (no-site needed)
			"PO", //per os (by mouth)
			"SL", //sublingual
			"TP" //topical
		};
		
		public long Id { get; set; }
		public string Name { get; set; }
		public string Strength { get; set; }
		public int Route { get; set; }

		public static Medication fromArrayList(ArrayList arrayList)
		{
			Medication med = new Medication();

			med.Id = (long)arrayList[0];
			med.Name = (string)arrayList[1];
			med.Strength = (string)arrayList[2];
			med.Route = (int)arrayList[3];

			return med;
		}

		public static void refreshMedicationPool()
		{
			if (MySqlHelper.connect() == false) return;

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery("SELECT * FROM tblMedication ORDER BY name");

			MySqlHelper.disconnect();

			MedicationPool.Clear();

			foreach (ArrayList arrayList in response)
				MedicationPool.Add(fromArrayList(arrayList));
		}

		public static Medication fromMySqlMedication(long med_id)
		{
			MySqlHelper.connect();

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(string.Format("SELECT * FROM tblMedication WHERE id={0}", med_id));

			MySqlHelper.disconnect();

			if (response.Count == 0) return null;

			return Medication.fromArrayList(response[0] as ArrayList);
		}
	} //End class Medication
} //End namespace SimPatient