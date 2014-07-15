using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Windows;
using System.Collections.ObjectModel;

namespace SimPatient
{
	public class Simulation
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreationDate { get; set; }
		public bool IsDeleted { get; set; }
        public string Creator { get; set; }

        private static ObservableCollection<Simulation> _simulations;
        public static ObservableCollection<Simulation> Simulations
        {
            get
            {
                if (_simulations == null)
                    _simulations = new ObservableCollection<Simulation>();

                return _simulations;
            }
        }

        public static void refreshSimulations()
        {
            if (MySqlHelper.connect() == false) return;

            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery("SELECT * FROM tblSimulation WHERE deleted != 1");
            
            MySqlHelper.disconnect();

            Simulations.Clear();

            foreach (ArrayList arrayList in response)
                Simulations.Add(fromArrayList(arrayList));
        }

        public static Simulation fromMySql(long id)
        {
            Simulation sim = new Simulation();

            if (MySqlHelper.connect() == false) return null;

            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery(string.Format("SELECT * FROM tblSimulation WHERE id={0}", id));

            MySqlHelper.disconnect();

            if (response.Count == 0) return null;

            ArrayList arrayList = response[0] as ArrayList;

            sim.Id = (long)arrayList[0];
            sim.Name = (string)arrayList[1];
            sim.Description = (string)arrayList[2];
            sim.CreationDate = (DateTime)arrayList[3];
            sim.IsDeleted = ((sbyte)arrayList[4]) == 1 ? true : false;
            sim.Creator = (string)arrayList[5];
            
            return sim;
        }

        public static Simulation fromMySqlUserAccountPool(long id)
        {
            MySqlHelper.connect();

            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery(string.Format("CALL simulation_from_user_id({0})", id));

            MySqlHelper.disconnect();

            if (response.Count == 0) return null;
                        
            return Simulation.fromArrayList(response[0] as ArrayList);
        }

        public static Simulation fromArrayList(ArrayList arrayList)
        {
            Simulation sim = new Simulation();

            sim.Id = (long)arrayList[0];
            sim.Name = (string)arrayList[1];
            sim.Description = (string)arrayList[2];
            sim.CreationDate = (DateTime)arrayList[3];
            sim.IsDeleted = ((sbyte)arrayList[4]) == 1 ? true : false;
            sim.Creator = (string)arrayList[5];

            return sim;
        }

	}
}