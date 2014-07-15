using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using System.Security;
using System.IO;
using System.Runtime.InteropServices;
using SimPatient.DataModel;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for PreferencesWindow.xaml
	/// </summary>
	public partial class PreferencesWindow : Window
	{

        private static PreferencesWindow _preferencesWindow;
        public static PreferencesWindow Instance
        {
            get
            {
                if (_preferencesWindow == null)
                    _preferencesWindow = new PreferencesWindow();

                return _preferencesWindow;
            }
        }

		public PreferencesWindow()
		{
			this.InitializeComponent();
			
            this.Loaded += PreferencesWindow_Loaded;
		}

        private void PreferencesWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            //e.Cancel = true;
            //Hide();
        }

        private void PreferencesWindow_Loaded(object sender, RoutedEventArgs e)
        {
            PreferencesWindow.loadPreferences();

            mySqlHostTextBox.Text = Preferences.HostAddress;
            mySqlPortTextBox.Text = Preferences.PortAddress;
            mySqlDatabaseTextBox.Text = Preferences.DatabaseName;
            mySqlUsernameTextBox.Text = Preferences.Username;
            mySqlPasswordBox.Password = Preferences.Password;
        }

        public static void loadPreferences()
        {
            if (File.Exists("preferences.config") == false) return;

            StreamReader sReader = new StreamReader("preferences.config");

            if (sReader == null)
            {
                MessageBox.Show("preferences.config could not be read!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Preferences.HostAddress = sReader.ReadLine();
            Preferences.PortAddress = sReader.ReadLine();
            Preferences.DatabaseName = sReader.ReadLine();
            Preferences.Username = sReader.ReadLine();
            Preferences.Password = sReader.ReadLine();

            sReader.Close();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            StreamWriter sWriter = new StreamWriter("preferences.config", false);

            if (sWriter == null)
            {
                MessageBox.Show("preferences.config could not be written!", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            sWriter.WriteLine(mySqlHostTextBox.Text);
            sWriter.WriteLine(mySqlPortTextBox.Text);
            sWriter.WriteLine(mySqlDatabaseTextBox.Text);
            sWriter.WriteLine(mySqlUsernameTextBox.Text);
            sWriter.WriteLine(mySqlPasswordBox.SecurePassword.convertToUnsecureString());

            sWriter.Close();

            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
	}

    public static class MyExtensionsMethods
    {
        public static string convertToUnsecureString(this SecureString securePassword)
        {
            if (securePassword == null)
                throw new ArgumentNullException("securePassword");

            IntPtr unmanagedString = IntPtr.Zero;
            try
            {
                unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
                return Marshal.PtrToStringUni(unmanagedString);
            }
            finally
            {
                Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
            }
        }
    }
} //End namespace SimPatient