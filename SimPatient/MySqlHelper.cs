using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SimPatient.DataModel;
using System.Windows;
using System.Collections;

using MySql.Data.MySqlClient;
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

        public static bool usernameExists(string username)
        {
            if (connect() == false)
            {
                MessageBox.Show("Bad MySQL Connection Credentials.", "MySQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            ArrayList response = dbCon.selectQuery(string.Format("SELECT * FROM tblUserAccount WHERE username='{0}'", username));
            disconnect();

            return (response.Count == 0) ? false : true;
        }

        public static bool addUserAccountToSimulation(long simId, long userId)
        {
            if (connect() == false)
            {
                MessageBox.Show("Bad MySQL Connection Credentials.", "MySQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                ArrayList response = dbCon.selectQuery(string.Format("INSERT INTO tblUserAccountPool VALUES({0}, {1})", userId, simId));
            }
            catch(MySqlException ex)
            {
                MessageBox.Show("Already in User Account Pool.", "Insert Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            disconnect();

            //if (response.Count == 0)
            //    return false;

            return true;
        }

        public static bool removeUserAccountFromSimulation(long userId)
        {
            if (connect() == false)
            {
                MessageBox.Show("Bad MySQL Connection Credentials.", "MySQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            ArrayList response = dbCon.selectQuery(string.Format("DELETE FROM tblUserAccountPool WHERE user_id={0}", userId));
            disconnect();

            return true;
        }

        public static bool addPatientToSimulation(long simId, long patId)
        {
            if (connect() == false)
            {
                MessageBox.Show("Bad MySQL Connection Credentials.", "MySQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            try
            {
                //add admit data parameter and argument to this call
                ArrayList response = dbCon.selectQuery(string.Format("INSERT INTO tblPatientPool VALUES({0}, {1}, CURRENT_DATE())", simId, patId));
            }
            catch (MySqlException ex)
            {
                //MessageBox.Show(ex.Message, ex.Number.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                MessageBox.Show("Already in Patient Pool.", "Insert Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            disconnect();

            //if (response.Count == 0)
            //    return false;

            return true;
        }

        public static bool removePatientFromSimulation(long patId)
        {
            if (connect() == false)
            {
                MessageBox.Show("Bad MySQL Connection Credentials.", "MySQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            ArrayList response = dbCon.selectQuery(string.Format("DELETE FROM tblPatientPool WHERE pat_id={0}", patId));
            disconnect();

            return true;
        }

        public static bool removeMedicationDose(long doseId)
        {
            if (connect() == false)
            {
                MessageBox.Show("Bad MySQL Connection Credentials.", "MySQL Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            ArrayList response = dbCon.selectQuery(string.Format("DELETE FROM tblMedicationDose WHERE id={0}", doseId));
            disconnect();

            return true;
        }
    } //End class MySqlHelper
} //End namespace SimPatient