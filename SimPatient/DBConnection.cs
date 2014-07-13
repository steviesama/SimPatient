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
        public bool OpenConnection()
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
        public bool CloseConnection()
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

        //public bool InsertNewService(CSXService record)
        //{
        //    string query = string.Format("INSERT INTO tblServices (department_id, service_name, service_description) VALUES({0}, '{1}', '{2}')", record.Parent.Id, record.Name, record.Description);

        //    return InsertQuery(query);
        //}

        //public ArrayList SelectServices(CSXDepartment fromTemplate)
        //{
        //    return SelectQuery("SELECT service_id, service_name, service_description FROM tblServices WHERE department_id=" + fromTemplate.Id);
        //}

        //public bool InsertNewDepartment(CSXDepartment record)
        //{
        //    string query = string.Format("INSERT INTO tblDepartments (csx_template_id, department_name, department_description) VALUES({0}, '{1}', '{2}')", record.Parent.Id, record.Name, record.Description);

        //    return InsertQuery(query);
        //}

        //public ArrayList SelectDepartments(CSXTemplate fromTemplate)
        //{
        //    return SelectQuery("SELECT department_id, department_name, department_description FROM tblDepartments WHERE csx_template_id=" + fromTemplate.Id);
        //}

        //public bool InsertTemplate(CSXTemplate record)
        //{            
        //    string query = string.Format("INSERT INTO tblCSXTemplate (csx_template_name, csx_template_description) VALUES('{0}', '{1}')", record.Name, record.Description);

        //    return InsertQuery(query);
        //}

        //public bool UpdateTemplate(CSXTemplate record)
        //{
        //    string query = string.Format("UPDATE tblCSXTemplate SET csx_template_name='{0}', csx_template_description='{1}' WHERE csx_template_id='{2}'",
        //                                 record.Name, record.Description, record.Id);

        //    return UpdateQuery(query);
        //}

        //public bool UpdateDepartment(CSXDepartment record)
        //{
        //    string query = string.Format("UPDATE tblDepartments SET department_name='{0}', department_description='{1}' WHERE department_id='{2}'",
        //                                 record.Name, record.Description, record.Id);

        //    return UpdateQuery(query);
        //}

        //public bool UpdateService(CSXService record)
        //{
        //    string query = string.Format("UPDATE tblServices SET service_name='{0}', service_description='{1}' WHERE service_id='{2}'",
        //                                 record.Name, record.Description, record.Id);

        //    return UpdateQuery(query);
        //}

        //public bool Update(Record record)
        //{
        //    if (record is CSXTemplate)
        //        return UpdateTemplate(record as CSXTemplate);
        //    else if (record is CSXDepartment)
        //        return UpdateDepartment(record as CSXDepartment);
        //    else if (record is CSXService)
        //        return UpdateService(record as CSXService);

        //    return false;
        //}

        public ArrayList SelectTemplates()
        {
            return SelectQuery("SELECT csx_template_id, csx_template_name, csx_template_description FROM tblCSXTemplate");
        }

        //Insert statement
        public bool InsertQuery(string query)
        {
            bool success = true;

            //open connection
            //if (this.OpenConnection() == true)
            //{

            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);

            try
            {
                //Execute command
                cmd.ExecuteNonQuery();
            }
            catch(MySqlException ex)
            {
                MessageBox.Show(ex.Message, "MySQL Insert Error", MessageBoxButton.OK, MessageBoxImage.Error);
                success = false;                    
            }

                //close connection
            //    this.CloseConnection();                
            //}

            return success;
        }

        //Update statement
        public bool UpdateQuery(string query)
        {
            bool success = true;

            //open connection
            //if (this.OpenConnection() == true)
            //{

            //create command and assign the query and connection from the constructor
            MySqlCommand cmd = new MySqlCommand(query, connection);

            try
            {
                //Execute command
                cmd.ExecuteNonQuery();
            }
            catch (MySqlException ex)
            {
                MessageBox.Show(ex.Message, "MySQL Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
                success = false;
            }

                //close connection
            //    this.CloseConnection();
            //}

            return success;
        }

        //public bool DeleteTemplate(CSXTemplate record)
        //{
        //    string query = string.Format("DELETE FROM tblCSXTemplate WHERE csx_template_id='{0}'", record.Id);
        //    return DeleteQuery(query);
        //}

        //public bool DeleteDepartment(CSXDepartment record)
        //{
        //    string query = string.Format("DELETE FROM tblDepartments WHERE department_id='{0}'", record.Id);
        //    return DeleteQuery(query);
        //}

        //public bool DeleteService(CSXService record)
        //{
        //    string query = string.Format("DELETE FROM tblServices WHERE service_id='{0}'", record.Id);
        //    return DeleteQuery(query);
        //}

        public bool DeleteQuery(string query)
        {
            bool success = true;

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);

                try
                {
                    cmd.ExecuteNonQuery();
                }
                catch (MySqlException ex)
                {
                    MessageBox.Show(ex.Message, "MySQL Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    success = false;
                }

                this.CloseConnection();
            }

            return success;
        }

        //Delete statement
        public void Delete()
        {
            string query = "DELETE FROM tableinfo WHERE name='John Smith'";

            if (this.OpenConnection() == true)
            {
                MySqlCommand cmd = new MySqlCommand(query, connection);
                cmd.ExecuteNonQuery();
                this.CloseConnection();
            }
        }

        public ArrayList SelectQuery(string query)
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

                //close Connection
            //    this.CloseConnection();
            //}
            
            //if list is empty, no records
            return list;
            
        }

        //Select statement
        public ArrayList Select()
        {
            string query = "SHOW TABLES";

            //Create a list to store the result
            ArrayList list = new ArrayList();

            //Open connection
            if (this.OpenConnection() == true)
            {
                //Create Command
                MySqlCommand cmd = new MySqlCommand(query, connection);
                //Create a data reader and Execute the command
                MySqlDataReader dataReader = cmd.ExecuteReader();

                //Read the data and store them in the list
                while (dataReader.Read())
                {
                    for (int i = 0; i < dataReader.FieldCount; i++)
                        list.Add(dataReader[i]);
                }

                string msg = "";

                foreach (string field in list)
                    msg += field + "\n";

                MessageBox.Show(msg, "SHOW csx_core.TABLES;");

                //close Data Reader
                dataReader.Close();

                //close Connection
                this.CloseConnection();

                //return list to be displayed
                return list;
            }
            else
            {
                return list;
            }
        }

        //Count statement
        public int Count()
        {
            string query = "SELECT Count(*) FROM tableinfo";
            int Count = -1;

            //Open Connection
            if (this.OpenConnection() == true)
            {
                //Create Mysql Command
                MySqlCommand cmd = new MySqlCommand(query, connection);

                //ExecuteScalar will return one value
                Count = int.Parse(cmd.ExecuteScalar() + "");

                //close Connection
                this.CloseConnection();

                return Count;
            }
            else
            {
                return Count;
            }
        }

        //Backup
        public void Backup()
        {
            try
            {
                DateTime Time = DateTime.Now;
                int year = Time.Year;
                int month = Time.Month;
                int day = Time.Day;
                int hour = Time.Hour;
                int minute = Time.Minute;
                int second = Time.Second;
                int millisecond = Time.Millisecond;

                //Save file to C:\ with the current date as a filename
                string path;
                path = "C:\\MySqlBackup" + year + "-" + month + "-" + day +
            "-" + hour + "-" + minute + "-" + second + "-" + millisecond + ".sql";
                StreamWriter file = new StreamWriter(path);


                ProcessStartInfo psi = new ProcessStartInfo();
                psi.FileName = "mysqldump";
                psi.RedirectStandardInput = false;
                psi.RedirectStandardOutput = true;
                psi.Arguments = string.Format(@"-u{0} -p{1} -h{2} {3}",
                    uid, password, server, database);
                psi.UseShellExecute = false;

                Process process = Process.Start(psi);

                string output;
                output = process.StandardOutput.ReadToEnd();
                file.WriteLine(output);
                process.WaitForExit();
                file.Close();
                process.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("Error , unable to backup!\n" + ex.Message);
            }
        }

        //Restore
        public void Restore()
        {
        }
    }
} //End namespace SimPatient