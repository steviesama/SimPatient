using System;
using System.Windows;
using System.Windows.Controls;

using UserAccountType = SimPatient.UserAccount.UserAccountType;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for LoginControl.xaml
	/// </summary>
	public partial class LoginControl : UserControl
	{
		private LoginControl()
		{
			this.InitializeComponent();
		}

		private static LoginControl _loginControl;
        /// <summary>
        /// Instance property used to maintain a single instance of the current control or window
        /// and create one if an instance doesn't yet exist.  Some controls may be reset as a part
        /// of access.
        /// </summary>
		public static LoginControl Instance
		{
			get
			{
				if (_loginControl == null)
					_loginControl = new LoginControl();

                /*BeginInvoke is used so that this property, and other operations already
                  in progress on the UI thread can finish executing in order for the 
                  focus function to work properly.*/
				_loginControl.Dispatcher.BeginInvoke(new Action(() =>
				{
					_loginControl.usernameTextBox.Text = "";
					_loginControl.passwordBox.Password = "";
					_loginControl.usernameTextBox.Focus();
				}));

                //---disable the controls disallowed while logged out
				MainWindow.Instance.mnuLogout.IsEnabled = false;
				PreferencesWindow.Instance.userAccountTabItem.IsEnabled = false;

				return _loginControl;
			}
		}

        /// <summary>
        /// The click even handler for the Login button.
        /// </summary>
		private void Login_Click(object sender, RoutedEventArgs e)
		{
			UserAccount ua = MySqlHelper.requestLogin(usernameTextBox.Text, passwordBox.SecurePassword.convertToUnsecureString());

			MainWindow.CurrentUser = ua;

			if (ua == null)
				MessageBox.Show("Invalid Username or Password.", "Login Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else
			{
				MainWindow.Instance.mnuLogout.IsEnabled = true;
				PreferencesWindow.Instance.userAccountTabItem.IsEnabled = true;
				
                UserControl userControl = null;

				if(ua.Type == UserAccountType.Administrator)
				{
					userControl = SimulationPoolControl.Instance;
					MainWindow.Instance.mnuMarArchiverViewer.IsEnabled = true;
					PatientPoolControl.VisualState = "AdminVisualState";
				}
				else
				{
					userControl = PatientPoolControl.Instance;
					MainWindow.Instance.mnuMarArchiverViewer.IsEnabled = false;
					PatientPoolControl.VisualState = "StationVisualState";
				}

				MainWindow.Instance.loadBottomGrid(userControl);
			}
		}
	} //End class LoginControl
} //End namespace SimPatient