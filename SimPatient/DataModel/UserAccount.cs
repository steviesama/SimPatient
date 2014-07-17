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
	public class UserAccount
	{
	    public enum UserAccountType { Administrator, Station }

        private static ObservableCollection<UserAccount> _userAccounts;
        public static ObservableCollection<UserAccount> UserAccounts
        {
            get
            {
                if (_userAccounts == null)
                    _userAccounts = new ObservableCollection<UserAccount>();

                return _userAccounts;
            }
        }

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

        public static void refreshUserAccounts()
        {
            if (MySqlHelper.connect() == false) return;

            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery(
                "SELECT * from tblUserAccount AS ua WHERE 0 = (SELECT COUNT(uap.user_id) FROM tblUserAccountPool AS uap WHERE uap.user_id=ua.id) AND ua.type=1");

            MySqlHelper.disconnect();

            UserAccounts.Clear();

            foreach (ArrayList arrayList in response)
                UserAccounts.Add(fromArrayList(arrayList));
        }

        public static void refreshUserAccountPool(long simId)
        {
            if (MySqlHelper.connect() == false) return;

            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery(
                "SELECT * from tblUserAccount AS ua WHERE ua.id = (SELECT uap.user_id FROM tblUserAccountPool AS uap WHERE uap.user_id=ua.id AND uap.sim_id=" + simId + ")");

            MySqlHelper.disconnect();

            UserAccounts.Clear();

            foreach (ArrayList arrayList in response)
                UserAccounts.Add(fromArrayList(arrayList));
        }
	} //End class UserAccount
} //End namespace SimPatient