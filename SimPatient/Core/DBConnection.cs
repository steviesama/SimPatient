using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//my references
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows;
using System.Collections;

using System.Security.Cryptography;

namespace SimPatient
{
    public class DBConnection
    {
        private MySqlConnection connection;
        private string server;
        private string port;
        private string database;
        private string uid;
        private string password;

        //Constructor
        public DBConnection()
        {
            Initialize(server, port, database, uid, password);
            RijndaelManaged crypto = new RijndaelManaged();

        }

        public DBConnection(string server, string port, string database, string uid, string password)
        {
            Initialize(server, port, database, uid, password);
        }

        //Initialize values
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

        //open connection to database
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

        //Close connection
        public bool closeConnection()
        {
            try
            {
                connection.Close();
                return true;
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }
        
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

        public ArrayList selectQuery(string query)
        {

            //Create a list to store the result
            ArrayList list = new ArrayList();
            ArrayList fields;

            //Open connection
            //if (this.OpenConnection() == true)
            //{

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

            //  close Connection
            //  this.CloseConnection();
            //}
            
            //if list is empty, no records
            return list;
            
        }
    } //End class DBConnection
} //End namespace SimPatient