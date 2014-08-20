using System;

using System.Collections;
using System.Collections.ObjectModel;

namespace SimPatient
{
    /// <summary>
    /// Class that models the tblUserAccount MySQL database data and provides
    /// methods that marshal such data from the database.
    /// </summary>
	public class UserAccount
	{
        /// <summary>
        /// Enumeration used to determine the account type of a given UserAccount object.
        /// </summary>
	    public enum UserAccountType { Administrator, Station }

        private static ObservableCollection<UserAccount> _userAccounts;
        /// <summary>
        /// A static collection of UserAccount objects that leverages the fact
        /// that ObservableCollection from the System.Collection.ObjectModel namespace
        /// implements INotifyPropertyChanged so that when the collection is modified
        /// any UI components that have this collection set as its DataContext will be
        /// updated each time it is modified.
        /// </summary>
        public static ObservableCollection<UserAccount> UserAccounts
        {
            get
            {
                if (_userAccounts == null)
                    _userAccounts = new ObservableCollection<UserAccount>();

                return _userAccounts;
            }
        }

        //---member property reflecting the fields in the database
		public long Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public UserAccountType Type { get; set; }
		public string Fullname { get; set; }
		public string Notes { get; set; }
		public bool IsLocked { get; set; }
        public string Creator { get; set; }
        public string LockedBy { get; set; }
		//the simulation this object is associated with
        public Simulation ParentSimulation { get; set; }

        /// <summary>
        /// Static method used to create a UserAccount objects from
        /// the supplied ArrayList.
        /// </summary>
        /// <param name="arrayList">An ArrayList containing the UserAccount fields.</param>
        /// <returns>A UserAccount object constructed from the supplied ArrayList.</returns>
        public static UserAccount fromArrayList(ArrayList arrayList)
        {
            UserAccount ua = new UserAccount();

            ua.Id = (long)arrayList[0];
            ua.Username = (string)arrayList[1];
            ua.Password = (string)arrayList[2];
            ua.Type = (sbyte)arrayList[3] == 0 ? UserAccountType.Administrator : UserAccountType.Station;
            ua.Fullname = (string)arrayList[4];
            ua.Notes = (string)arrayList[5];
            ua.IsLocked = (sbyte)arrayList[6] == 0 ? false : true;
            ua.Creator = (string)arrayList[7];
            ua.LockedBy = arrayList[8] is DBNull ? string.Empty : (string)arrayList[8];
            ua.ParentSimulation = Simulation.fromMySqlUserAccountPool(ua.Id);

            return ua;
        }

        /// <summary>
        /// Refreshes the UserAccounts static property with the all UserAccounts that are Stations
        /// and not already bound to a simulation.
        /// </summary>
        public static void refreshUserAccounts()
        {
            if (MySqlHelper.connect() == false) return;

            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery(
                "SELECT * FROM tblUserAccount AS ua WHERE 0 = (SELECT COUNT(uap.user_id) FROM tblUserAccountPool AS uap WHERE uap.user_id=ua.id) AND ua.type=1");

            MySqlHelper.disconnect();

            UserAccounts.Clear();

            foreach (ArrayList arrayList in response)
                UserAccounts.Add(fromArrayList(arrayList));
        }

        /// <summary>
        /// Refreshes the UserAccounts static property with the UserAccounts that are associated with
        /// the simulation identified by the supplied simId.
        /// </summary>
        /// <param name="simId">A long value containing the simulation id to compare user accounts against.</param>
        public static void refreshUserAccountPool(long simId)
        {
            if (MySqlHelper.connect() == false) return;

            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery(
                "SELECT * FROM tblUserAccount AS ua WHERE ua.id = (SELECT uap.user_id FROM tblUserAccountPool AS uap WHERE uap.user_id=ua.id AND uap.sim_id=" + simId + ")");

            MySqlHelper.disconnect();

            UserAccounts.Clear();

            foreach (ArrayList arrayList in response)
                UserAccounts.Add(fromArrayList(arrayList));
        }
	} //End class UserAccount
} //End namespace SimPatient