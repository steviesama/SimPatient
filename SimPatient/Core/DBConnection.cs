using MySql.Data.MySqlClient;
using System.Windows;
using System.Collections;

namespace SimPatient
{
    /// <summary>
    /// A lightweight class to handle the MySQL connection queries.
    /// </summary>
    public class DBConnection
    {
        private MySqlConnection connection;
        private string server;
        private string port;
        private string database;
        private string uid;
        private string password;

        /// <summary>
        /// The parameterized constructor that takes all the connection credentials required
        /// to establish a connection to the MySQL server.
        /// </summary>
        /// <param name="server">The MySQL server address.</param>
        /// <param name="port">The MySQL port address.</param>
        /// <param name="database">The database name to connect to within the server.</param>
        /// <param name="uid">The MySQL username.</param>
        /// <param name="password">The MySQL password.</param>
        public DBConnection(string server, string port, string database, string uid, string password)
        {
            Initialize(server, port, database, uid, password);
        }

        /// <summary>
        /// The parameterized initialization function used across any constructors that takes
        /// all the connection credentials required to establish a connection to the MySQL server.
        /// </summary>
        /// <param name="server">The MySQL server address.</param>
        /// <param name="port">The MySQL port address.</param>
        /// <param name="database">The database name to connect to within the server.</param>
        /// <param name="uid">The MySQL username.</param>
        /// <param name="password">The MySQL password.</param>
        private void Initialize(string server, string port, string database, string uid, string password)
        {
            this.server = server;
            this.port = port;
            this.database = database;
            this.uid = uid;
            this.password = password;
            string connectionString = "SERVER=" + server + ";" +
                                      "PORT=" + port + ";" +
                                      "DATABASE=" + database + ";" +
                                      "UID=" + uid + ";" +
                                      "PASSWORD=" + password + ";";

            connection = new MySqlConnection(connectionString);
        }

        /// <summary>
        /// Attempts to open a connection to the MySQL database and returns true or
        /// false depending on if it was successful.
        /// </summary>
        /// <returns>A boolean value reflecting whether or not the connection to
        /// the database was successful.</returns>
        public bool openConnection()
        {
            try
            {
                connection.Open();
                return true;
            }
            catch (MySqlException ex)
            {
                //When handling errors, you can your application's response based 
                //on the error number.
                //The two most common error numbers when connecting are as follows:
                //0: Cannot connect to server.
                //1045: Invalid user name and/or password.
                switch (ex.Number)
                {
                    case 0:
                        MessageBox.Show("Cannot connect to server.  Contact administrator");
                        break;

                    case 1045:
                        MessageBox.Show("Invalid username/password, please try again");
                        break;
                    default:
                        MessageBox.Show(ex.Message, "MySQL Connection Error", MessageBoxButton.OK, MessageBoxImage.Error);
                        break;
                }
                return false;
            }
        }

        /// <summary>
        /// Attempts to close the connection to the MySQL database and returns true or
        /// false depending on if it was successful.
        /// </summary>
        /// <returns>A boolean value reflecting whether or not the closing of the
        /// connection to the database was successful.</returns>
        public bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException)
            {
                return false;
            }
        }
        
        /// <summary>
        /// Takes a query that can be executed as any non query (doesn't return records)
        /// and return true or false depending on whether or not an exception was thrown.
        /// However it was intended to be used with delete queries.
        /// </summary>
        /// <param name="query">A string containing the query to execute.</param>
        /// <returns>A boolean value reflecting whether or not an exception was thrown.</returns>
        public bool deleteQuery(string query)
        {
            bool success = true;

            MySqlCommand cmd = new MySqlCommand(query, connection);

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException)
            {
                //MessageBox.Show(ex.Message, "MySQL Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                success = false;
            }

            return success;
        }

        /// <summary>
        /// Takes a query that can be executed with a MySQL data reader in order to
        /// return a series of matches records.
        /// </summary>
        /// <param name="query">A string containing the query to execute.</param>
        /// <returns>An ArrayList containing all the resulting set of records.</returns>
        public ArrayList selectQuery(string query)
        {

            //Create a list to store the result
            ArrayList list = new ArrayList();
            ArrayList fields;

            //Create Command
            MySqlCommand cmd = new MySqlCommand(query, connection);
            //Create a data reader and Execute the command
            MySqlDataReader dataReader = cmd.ExecuteReader();

            //Read the data and store them in the list
            while (dataReader.Read())
            {
                //new list of fields
                fields = new ArrayList();
                //iterate over all fields
                for (int i = 0; i < dataReader.FieldCount; i++)
                    //storing each field
                    fields.Add(dataReader[i]);
                //add fields to list as a row
                list.Add(fields);
            }

            //close Data Reader
            dataReader.Close();
            
            //if list is empty, no records
            return list;
            
        }
    } //End class DBConnection
} //End namespace SimPatient