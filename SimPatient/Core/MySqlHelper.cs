using SimPatient.DataModel;
using System.Windows;
using System.Collections;

using MySql.Data.MySqlClient;

namespace SimPatient
{
    /// <summary>
    /// Contains MySQL Connector/DBConnection static helper functions.
    /// </summary>
    public static class MySqlHelper
    {
        /// <summary>
        /// The database connection reference for use after MySqlHelper.connect() is called.
        /// </summary>
        public static DBConnection dbCon { get; set; }

        /// <summary>
        /// Attempts a connection to the MySQL server specified in the PreferencesWindow
        /// using the credentials also specified there and returns true or false depending
        /// on if the connection was successful.
        /// </summary>
        /// <returns>A boolean value reflecting whether or not the connection to the
        /// MySQL database was successful or not.</returns>
        public static bool connect()
        {
            //This is tightly coupled with DBConnection and preferably shouldn't be.
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

        /// <summary>
        /// Disconnects from the MySQL database that was previously connected
        /// to if a connection object is allocated.
        /// </summary>
        public static void disconnect()
        {
            if (dbCon == null)
                dbCon.closeConnection();
        }

        /// <summary>
        /// Used to validate the passed login information, returning true or
        /// false depending on if the user credentials are valid.
        /// </summary>
        /// <param name="username">A string holding the username.</param>
        /// <param name="password">A string holding the password.</param>
        /// <returns>A boolean value reflecting whether or not the user
        /// credentials are valid or not.</returns>
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

        /// <summary>
        /// Used to request a login from the MySQL back end.
        /// </summary>
        /// <param name="username">A string holding the username.</param>
        /// <param name="password">A string holding the password.</param>
        /// <returns>A valid UserAccount object if the login was successful
        /// or null if not.</returns>
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

        /// <summary>
        /// Used to determine if a username exist in the database or not.
        /// </summary>
        /// <param name="username">A string holding the username.</param>
        /// <returns>A boolean value reflecting whether or not the
        /// supplied username exists in the database or not.</returns>
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

        /// <summary>
        /// Used to bind a user account to a simulation by adding them to
        /// the Account Pool in the back end.
        /// </summary>
        /// <param name="simId">A long value holding a simulation id.</param>
        /// <param name="userId">A long value holding a user id.</param>
        /// <returns>A boolean value reflecting whether or not the user account
        /// was successfully associated with the simulation.</returns>
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
            catch(MySqlException)
            {
                MessageBox.Show("Already in User Account Pool.", "Insert Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }

            disconnect();

            return true;
        }

        /// <summary>
        /// Used to unbind a user account for any simulations it may be
        /// bound to in the Account Pool.
        /// </summary>
        /// <param name="userId">A long value holding a user id.</param>
        /// <returns>A boolean value reflecting whether or not the user
        /// account was successfully unbound from simulations.</returns>
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

        /// <summary>
        /// Used to bind a patient to a simulation by adding them to
        /// the Patient Pool in the back end.
        /// </summary>
        /// <param name="simId">A long value holding a simulation id.</param>
        /// <param name="patId">A long value holding a patient id.</param>
        /// <returns>A boolean value reflecting whether or not the
        /// patient was successfully added to the Patient Pool.</returns>
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
            catch (MySqlException)
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

        /// <summary>
        /// Used to unbind a patient for any simulations it may be
        /// bound to in the Patient Pool.
        /// </summary>
        /// <param name="patId">A long value holding a patient id.</param>
        /// <returns>A boolean value reflecting whether or not the
        /// patient was successfully unbound from any simulations.</returns>
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

        /// <summary>
        /// Used to remove a medication dose from the Medication Pool (tblMedicationDose).
        /// </summary>
        /// <param name="doseId">A long value holding a medication dose id./param>
        /// <returns>A boolean value reflecting whether or not the
        /// removal was successful.</returns>
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