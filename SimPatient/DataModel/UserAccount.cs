using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Collections;
using System.Windows;

namespace SimPatient
{
	public class UserAccount
	{
	    public enum UserAccountType { Administrator, Station }

		public long Id { get; set; }
		public string Username { get; set; }
		public string Password { get; set; }
		public UserAccountType Type { get; set; }
		public string Fullname { get; set; }
		public string Notes { get; set; }
		public bool IsLocked { get; set; }
        public string LockedBy { get; set; }
        public string Creator { get; set; }
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
            ua.LockedBy = (string)arrayList[7];
            ua.Creator = (string)arrayList[8];
            ua.ParentSimulation = Simulation.fromMySqlUserAccountPool(ua.Id);

            return ua;
        }
	}
}