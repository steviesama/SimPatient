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

using System.Windows.Threading;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for AccountPoolControl.xaml
	/// </summary>
	public partial class AccountPoolControl : UserControl
    {
        public static UserAccount SelectedUserAccount { get; set; }

        private static AccountPoolControl _instance;
        public static AccountPoolControl Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new AccountPoolControl();

                _instance.ActionMode = ActionMode.SelectMode;

                return _instance;
            }
        }

        /// <summary>
        /// The UserControl that navigated to this control.
        /// </summary>
        public static UserControl ParentControl { get; set; }

        public static AccountPoolControl getInstance(UserControl parentControl)
        {
            AccountPoolControl.ParentControl = parentControl;
            UserAccount.refreshUserAccounts();
            return Instance;
        }
        
		public AccountPoolControl()
		{
			this.InitializeComponent();

            accountListView.DataContext = UserAccount.UserAccounts;
            UserAccount.refreshUserAccounts();
		}

        private ActionMode _actionMode;
        public ActionMode ActionMode
        {
            get { return _actionMode; }
            set
            {
                _actionMode = value;

                switch (value)
                {
                    case ActionMode.SelectMode:
                        actionButton.Content = "Select";
                        break;
                    case ActionMode.EditMode:
                        actionButton.Content = "Edit";
                        break;
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            UserAccount.refreshUserAccountPool(SimulationPoolControl.SelectedSimulation.Id);
            MainWindow.Instance.loadBottomGrid(ParentControl);
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            //indicates that new button was clicked from other processes
            SelectedUserAccount = null;
            AccountEditorWindow.getEmptyInstance(this).ShowDialog();

        }

        private void accountListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                SelectedUserAccount = (sender as ListView).SelectedItem as UserAccount;
                actionButton.IsEnabled = true;
            }
            else actionButton.IsEnabled = false;
        }

        private void actionButton_Click(object sender, RoutedEventArgs e)
        {
            switch(ActionMode)
            {
                case ActionMode.EditMode:
                    break;
                case ActionMode.SelectMode:
                    MySqlHelper.addUserAccountToSimulation(SimulationPoolControl.SelectedSimulation.Id, SelectedUserAccount.Id);
                    break;
            }

            UserAccount.refreshUserAccountPool(SimulationPoolControl.SelectedSimulation.Id);
            MainWindow.Instance.loadBottomGrid(ParentControl);
        }
	} //End class AccountPoolControl
} //End namespace SimPatient