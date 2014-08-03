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
using System.Windows.Navigation;
using System.Windows.Shapes;

using UserAccountType = SimPatient.UserAccount.UserAccountType;
using System.Windows.Threading;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for LoginControl.xaml
	/// </summary>
	public partial class LoginControl : UserControl
	{
        private static LoginControl _loginControl;
        public static LoginControl Instance
        {
            get
            {
                if (_loginControl == null)
                    _loginControl = new LoginControl();

                _loginControl.Dispatcher.BeginInvoke(new Action(() =>
                {
                    _loginControl.usernameTextBox.Text = "";
                    _loginControl.passwordBox.Password = "";
                    _loginControl.usernameTextBox.Focus();
                }));

                MainWindow.Instance.mnuLogout.IsEnabled = false;
                PreferencesWindow.Instance.userAccountTabItem.IsEnabled = false;

                return _loginControl;
            }
        }

		private LoginControl()
		{
			this.InitializeComponent();
		}

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
                    PatientPoolControl.VisualState = "AdminVisualState";
                }
                else
                {
                    userControl = PatientPoolControl.Instance;
                    PatientPoolControl.VisualState = "StationVisualState";
                }

                MainWindow.Instance.loadBottomGrid(userControl);
            }
        }
	}
}