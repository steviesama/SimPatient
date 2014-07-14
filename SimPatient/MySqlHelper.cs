using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimPatient.DataModel;
using System.Windows;
using System.Collections;

namespace SimPatient
{
    public static class MySqlHelper
    {
        public static DBConnection dbCon { get; set; }

        public static bool connect()
        {
            PreferencesWindow.loadPreferences();

            if (dbCon != null)
                dbCon.closeConnection();

            dbCon = new DBConnection
            (
                Preferences.HostAddress,
                Preferences.PortAddress,
                Preferences.DatabaseName,
                Preferences.Username,
                Preferences.Password
            );

            return dbCon.openConnection();
        }

        public static void disconnect()
        {
            if (dbCon == null)
                dbCon.closeConnection();
        }

        public static bool loginIsValid(string username, string password)
        {
            if(connect() == false)
                return false;
            ArrayList response = dbCon.selectQuery(string.Format("CALL request_login('{0}', '{1}')", username, password));
            disconnect();

            bool result = response.Count == 0 ? false : true;
            //if result is 1 login is valid, if not 1, all invalid
            return result;
        }

        public static UserAccount requestLogin(string username, string password)
        {
            if (connect() == false)
            {
                MessageBox.Show("Bad MySQL Connection Credentials.", "MySQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return null;
            }

            ArrayList response = dbCon.selectQuery(string.Format("CALL request_login('{0}', '{1}')", username, password));
            disconnect();

            if (response.Count == 0)
                return null;
                        
            return UserAccount.fromArrayList(response[0] as ArrayList);
        }

    }
}