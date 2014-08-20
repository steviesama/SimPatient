using System;

using System.Collections;
using System.Collections.ObjectModel;

namespace SimPatient
{
    /// <summary>
    /// Class that models the tblSimulation MySQL database data and provides
    /// methods that marshal such data from the database.
    /// </summary>
    public class Simulation
	{
        //---member property reflecting the fields in the database
		public long Id { get; set; }
		public string Name { get; set; }
		public string Description { get; set; }
		public DateTime CreationDate { get; set; }
		public bool IsDeleted { get; set; }
		public string Creator { get; set; }

		private static ObservableCollection<Simulation> _simulations;
        /// <summary>
        /// A static collection of Simulation objects that leverages the fact
        /// that ObservableCollection from the System.Collection.ObjectModel namespace
        /// implements INotifyPropertyChanged so that when the collection is modified
        /// any UI components that have this collection set as its DataContext will be
        /// updated each time it is modified.
        /// </summary>
        public static ObservableCollection<Simulation> Simulations
		{
			get
			{
				if (_simulations == null)
					_simulations = new ObservableCollection<Simulation>();

				return _simulations;
			}
		}

        /// <summary>
        /// Refreshes the Simulations static property with all simulations in the database
        /// that aren't marked as deleted.
        /// </summary>
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

        /// <summary>
        /// Creates a Simulation object from the MySQL database that has the id
        /// referenced by the id argument.
        /// </summary>
        /// <param name="id">A long value indicating the id of the simulation to marshal.</param>
        /// <returns>A Simulation object constructed from the simulation id provided, or
        /// null if no such simulation exists in the database.</returns>
		public static Simulation fromMySql(long id)
		{
			Simulation sim = new Simulation();

			if (MySqlHelper.connect() == false) return null;

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(string.Format("SELECT * FROM tblSimulation WHERE id={0}", id));

			MySqlHelper.disconnect();

			if (response.Count == 0) return null;

			ArrayList arrayList = response[0] as ArrayList;

			sim.Id           = (long)arrayList[0];
			sim.Name         = (string)arrayList[1];
			sim.Description  = (string)arrayList[2];
			sim.CreationDate = (DateTime)arrayList[3];
			sim.IsDeleted    = ((sbyte)arrayList[4]) == 1 ? true : false;
			sim.Creator      = (string)arrayList[5];
			
			return sim;
		}

        /// <summary>
        /// Creates a Simulation object by identifying the simulation id
        /// that may be associated with a provided patient id in the database.
        /// </summary>
        /// <param name="patId">A long value containing the patient id to check for.</param>
        /// <returns>A Simulation object constructed from a simulation id associated
        /// with the provided by patId or null if no such combination exists.</returns>
		public static Simulation fromMySqlPatientPool(long patId)
		{
			MySqlHelper.connect();

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(string.Format("CALL simulation_from_pat_id({0})", patId));

			MySqlHelper.disconnect();

			if (response.Count == 0) return null;

			return Simulation.fromArrayList(response[0] as ArrayList);
		}

        /// <summary>
        /// Creates a Simulation object by identifying the simulation id
        /// that may be associated with a provided user account id in the database.
        /// </summary>
        /// <param name="userId">A long value containing the user account id to check for.</param>
        /// <returns>A Simulation object constructed from a simulation id associated
        /// with the provided by userId or null if no such combination exists.</returns>
        public static Simulation fromMySqlUserAccountPool(long userId)
		{
			MySqlHelper.connect();

			DBConnection dbCon = MySqlHelper.dbCon;
			ArrayList response = dbCon.selectQuery(string.Format("CALL simulation_from_user_id({0})", userId));

			MySqlHelper.disconnect();

			if (response.Count == 0) return null;
						
			return Simulation.fromArrayList(response[0] as ArrayList);
		}

        /// <summary>
        /// Creates a Simulation object from the supplied ArrayList.
        /// </summary>
        /// <param name="arrayList">An ArrayList holding the simulation fields.</param>
        /// <returns>A Simulation object constructed from the supplied ArrayList.</returns>
		public static Simulation fromArrayList(ArrayList arrayList)
		{
			Simulation sim = new Simulation();

			sim.Id           = (long)arrayList[0];
			sim.Name         = (string)arrayList[1];
			sim.Description  = (string)arrayList[2];
			sim.CreationDate = (DateTime)arrayList[3];
			sim.IsDeleted    = ((sbyte)arrayList[4]) == 1 ? true : false;
			sim.Creator      = (string)arrayList[5];

			return sim;
		}
	} //End class Simulation
} //End namespace SimPatient