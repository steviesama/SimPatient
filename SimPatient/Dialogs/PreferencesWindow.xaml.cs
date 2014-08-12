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

using System.Threading;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for PreferencesWindow.xaml
	/// </summary>
	public partial class PreferencesWindow : Window
	{

		private static PreferencesWindow _instance;
		public static PreferencesWindow Instance
		{
			get
			{
				if (_instance == null)
					_instance = new PreferencesWindow();

				_instance.tabControl.SelectedIndex = 0;
				_instance.changePasswordCheckBox.IsChecked = false;
				_instance.currentPasswordBox.Password = string.Empty;
				_instance.newPasswordBox.Password = string.Empty;
				_instance.confirmNewPasswordBox.Password = string.Empty;

				return _instance;
			}
		}

		public PreferencesWindow()
		{
			this.InitializeComponent();

			this.Loaded += PreferencesWindow_Loaded;
			this.Closing +=PreferencesWindow_Closing;
		}

		private void PreferencesWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
			Hide();
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

			if (changePasswordCheckBox.IsChecked == true)
				if (changePassword() == false) return;

			Hide();
		}

		private bool changePassword()
		{
			if (isInputValid() == false) return false;

			if (MySqlHelper.connect() == false) return false;

			MySqlHelper.dbCon.selectQuery(string.Format("UPDATE tblUserAccount SET password='{0}' WHERE id={1}",
										  newPasswordBox.SecurePassword.convertToUnsecureString(), MainWindow.CurrentUser.Id));

			MySqlHelper.disconnect();

			//store new password
			MainWindow.CurrentUser.Password = newPasswordBox.SecurePassword.convertToUnsecureString();

			return true;
		}

		public bool isInputValid()
		{
			bool isValid = true;

			if(currentPasswordBox.SecurePassword.convertToUnsecureString() != MainWindow.CurrentUser.Password)
			{
				currentPasswordBox.Background = Brushes.LightPink;
				isValid = false;
			}
			else currentPasswordBox.Background = Brushes.White;

			if (newPasswordBox.Password.Length < 4 || newPasswordBox.Password.Length > 40)
			{
				newPasswordBox.Background = Brushes.LightPink;
				isValid = false;
			}
			else newPasswordBox.Background = Brushes.White;

			if (newPasswordBox.Password != confirmNewPasswordBox.Password)
			{
				confirmNewPasswordBox.Background = Brushes.LightPink;
				isValid = false;
			}
			else confirmNewPasswordBox.Background = Brushes.White;

			return isValid;
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}

		private void changePasswordCheckBox_Checked(object sender, RoutedEventArgs e)
		{
			currentPasswordBox.IsEnabled = newPasswordBox.IsEnabled = confirmNewPasswordBox.IsEnabled = true;
		}

		private void changePasswordCheckBox_Unchecked(object sender, RoutedEventArgs e)
		{
			currentPasswordBox.IsEnabled = newPasswordBox.IsEnabled = confirmNewPasswordBox.IsEnabled = false;
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