using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Security;

namespace SimPatient
{
	public class UserAccount
	{
	    public enum UserAccountType { Administrator, Station }

		public int Id { get; set; }
		public string Username { get; set; }
		public SecureString Password { get; set; }
		public UserAccountType Type { get; set; }
		public string Fullname { get; set; }
		public string Notes { get; set; }
		public bool IsLocked { get; set; }
		//the simulation this object is associated with
        public Simulation ParentSimulation { get; set; }
	}
}