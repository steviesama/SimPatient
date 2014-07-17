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

using System.Windows.Threading;
using System.Collections;

using UserAccountType = SimPatient.UserAccount.UserAccountType;
using MySqlFunctions = MySql.Data.MySqlClient.MySqlHelper;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for AccountEditorWindow.xaml
	/// </summary>
	public partial class AccountEditorWindow : Window
	{
        /// <summary>
        /// The UserControl that navigated to this control.
        /// </summary>
        public static UserControl ParentControl { get; set; }

        public static AccountEditorWindow getInstance(UserControl parentControl)
        {
            AccountEditorWindow.ParentControl = parentControl;
            return Instance;
        }

        public static AccountEditorWindow getEmptyInstance(UserControl parentControl)
        {
            AccountEditorWindow.ParentControl = parentControl;
            return EmptyInstance;
        }

        private static AccountEditorWindow _instance;
        public static AccountEditorWindow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AccountEditorWindow();

                if (AccountPoolControl.SelectedUserAccount != null)
                    fillControls(AccountPoolControl.SelectedUserAccount);

                return _instance;
            }
        }

        public static AccountEditorWindow EmptyInstance
        {
            get
            {
                emptyControls();
                return Instance;
            }
        }

        public static void fillControls(UserAccount ua)
        {
            AccountEditorWindow act = AccountEditorWindow.Instance;

            act.usernameTextBox.Text = ua.Username;
            act.passwordBox.Password = ua.Password;
            act.confirmPasswordBox.Password = ua.Password;
            act.notesTextBox.Text = ua.Notes;
            act.fullnameTextBox.Text = ua.Fullname;
            if (ua.Type == UserAccountType.Administrator)
                act.optAdministrator.IsChecked = true;
            else act.optStation.IsChecked = true;

        }

        public static void emptyControls()
        {
            AccountEditorWindow act = AccountEditorWindow.Instance;

            act.usernameTextBox.Text =
            act.passwordBox.Password =
            act.confirmPasswordBox.Password =
            act.notesTextBox.Text =
            act.fullnameTextBox.Text = string.Empty;
            act.optStation.IsChecked = true;
        }

		public AccountEditorWindow()
		{
			this.InitializeComponent();
		}

        private void mySqlAddNewUserAccount()
        {
            DBConnection dbCon = MySqlHelper.dbCon;

            ArrayList response = dbCon.selectQuery(string.Format("SELECT add_user_account(NULL, '{0}', '{1}', {2}, '{3}', '{4}', {5}, NULL, '{6}')",
                                                                 MySqlFunctions.EscapeString(usernameTextBox.Text),
                                                                 MySqlFunctions.EscapeString(passwordBox.SecurePassword.convertToUnsecureString()),
                                                                 optAdministrator.IsChecked == true ? 0 : 1,
                                                                 MySqlFunctions.EscapeString(fullnameTextBox.Text),
                                                                 MySqlFunctions.EscapeString(notesTextBox.Text),
                                                                 0,
                                                                 MainWindow.CurrentUser.Username));

            MySqlHelper.disconnect();

            if ((long)(response[0] as ArrayList)[0] == 0)
                MessageBox.Show("UserAccount not saved.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else Simulation.refreshSimulations();
        }

        private void mySqlUpdateUserAccount()
        {
            DBConnection dbCon = MySqlHelper.dbCon;

            UserAccount ua = AccountPoolControl.SelectedUserAccount;
            if (ua == null) return;

            ArrayList response = dbCon.selectQuery(string.Format("SELECT add_user_account(NULL, '{0}', '{1}', {2}, '{3}', '{4}', {5}, '{6}', '{7}')",
                                                                 MySqlFunctions.EscapeString(usernameTextBox.Text),
                                                                 MySqlFunctions.EscapeString(passwordBox.SecurePassword.convertToUnsecureString()),
                                                                 optAdministrator.IsChecked == true ? 0 : 1,
                                                                 MySqlFunctions.EscapeString(fullnameTextBox.Text),
                                                                 MySqlFunctions.EscapeString(notesTextBox.Text),
                                                                 0,
                                                                 string.Empty,
                                                                 MainWindow.CurrentUser.Username));

            MySqlHelper.disconnect();

            if ((sbyte)(response[0] as ArrayList)[0] == 0)
                MessageBox.Show("UserAccount not Update.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else Simulation.refreshSimulations();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MySqlHelper.connect() == false) return;

            if (AccountPoolControl.SelectedUserAccount == null)
                mySqlAddNewUserAccount();
            else mySqlUpdateUserAccount();
            
            UserAccount.refreshUserAccounts();

            Hide();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
	} //End class AccountEditorWindow
} //End namespace SimPatient