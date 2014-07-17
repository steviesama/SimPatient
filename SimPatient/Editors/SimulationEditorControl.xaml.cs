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

using System.Collections.ObjectModel;
using System.Windows.Threading;
using System.Collections;

using MySqlFunctions = MySql.Data.MySqlClient.MySqlHelper;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for SimulationEditorControl.xaml
	/// </summary>
	public partial class SimulationEditorControl : UserControl
	{
        /// <summary>
        /// The UserControl that navigated to this control.
        /// </summary>
        public static UserControl ParentControl { get; set; }

        public static SimulationEditorControl getInstance(UserControl parentControl)
        {
            SimulationEditorControl.ParentControl = parentControl;
            return Instance;
        }

        public static SimulationEditorControl getEmptyInstance(UserControl parentControl)
        {
            SimulationEditorControl.ParentControl = parentControl;
            return EmptyInstance;
        }

        private static SimulationEditorControl _instance;
        public static SimulationEditorControl Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new SimulationEditorControl();

                _instance.simulationNameTextBox.Background = Brushes.White;

                _instance.Dispatcher.BeginInvoke(new Action<UIElement>(x =>
                {
                    if (SimulationPoolControl.Instance.ActionMode == ActionMode.EditMode)
                    {
                        Simulation sim = SimulationPoolControl.SelectedSimulation;
                        if (sim != null)
                        {
                            _instance.simulationNameTextBox.Text = sim.Name;
                            _instance.simulationDescriptionTextBox.Text = sim.Description;
                        }
                    }

                    _instance.simulationNameTextBox.Focus();
                }), DispatcherPriority.ApplicationIdle, _instance);

                if (SimulationPoolControl.SelectedSimulation == null)
                {
                    _instance.addAccountButton.IsEnabled = false;
                    _instance.addPatientButton.IsEnabled = false;
                }
                else
                {
                    _instance.addAccountButton.IsEnabled = true;
                    _instance.addPatientButton.IsEnabled = true;
                }

                return _instance;
            }
        }

        public static SimulationEditorControl EmptyInstance
        {
            get
            {
                emptyControls();   
                return Instance;
            }
        }

		private SimulationEditorControl()
		{
			this.InitializeComponent();

            accountPoolListView.DataContext = UserAccount.UserAccounts;
            UserAccount.refreshUserAccountPool(SimulationPoolControl.SelectedSimulation == null ? 0 : SimulationPoolControl.SelectedSimulation.Id);
                        
            patientPoolListView.DataContext = Patient.PatientPool;
            Patient.refreshPatientPool(SimulationPoolControl.SelectedSimulation.Id);
        }

        private static void emptyControls()
        {
            Instance.simulationNameTextBox.Text = "";
            Instance.simulationDescriptionTextBox.Text = "";

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.loadBottomGrid(ParentControl);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if(simulationNameTextBox.Text.Trim() == string.Empty)
            {
                simulationNameTextBox.Background = Brushes.LightPink;
                MessageBox.Show("No valid simulation name entered.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
                simulationNameTextBox.Focus();
                return;
            }
            else simulationNameTextBox.Background = Brushes.White;

            if (MySqlHelper.connect() == false) return;

            if (SimulationPoolControl.SelectedSimulation == null)
                mySqlAddNewSimulation();
            else mySqlUpdateSimulation();

            MainWindow.Instance.loadBottomGrid(ParentControl);
        }

        private void mySqlAddNewSimulation()
        {
            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery(string.Format("SELECT add_simulation(NULL, '{0}', '{1}', NULL, 0, '{2}')",
                                                                 MySqlFunctions.EscapeString(simulationNameTextBox.Text),
                                                                 MySqlFunctions.EscapeString(simulationDescriptionTextBox.Text),
                                                                 MySqlFunctions.EscapeString(MainWindow.CurrentUser.Username)));

            MySqlHelper.disconnect();

            if ((long)(response[0] as ArrayList)[0] == 0)
                MessageBox.Show("Simulation not saved.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else Simulation.refreshSimulations();
        }

        private void mySqlUpdateSimulation()
        {
            DBConnection dbCon = MySqlHelper.dbCon;
            
            Simulation sim = SimulationPoolControl.SelectedSimulation;
            if (sim == null) return;

            ArrayList response = dbCon.selectQuery(string.Format("SELECT update_simulation({0}, '{1}', '{2}', '{3}', {4}, '{5}')",
                                                                 sim.Id,
                                                                 MySqlFunctions.EscapeString(simulationNameTextBox.Text),
                                                                 MySqlFunctions.EscapeString(simulationDescriptionTextBox.Text),
                                                                 sim.CreationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                 sim.IsDeleted ? 1 : 0,
                                                                 MySqlFunctions.EscapeString(sim.Creator)));

            MySqlHelper.disconnect();

            if ((sbyte)(response[0] as ArrayList)[0] == 0)
                MessageBox.Show("Simulation not Update.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else Simulation.refreshSimulations();
        }

        private void addPatientButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.loadBottomGrid(PatientPoolControl.getInstance(this, "AdminVisualState"));
            Patient.refreshPatients();
        }

        private void addAccountButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.loadBottomGrid(AccountPoolControl.getInstance(this));
        }

        private void removeAccountButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlHelper.removeUserAccountFromSimulation((accountPoolListView.SelectedItem as UserAccount).Id);
            UserAccount.refreshUserAccountPool(SimulationPoolControl.SelectedSimulation.Id);
        }

        private void accountPoolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removeAccountButton.IsEnabled = accountPoolListView.SelectedItem == null ? false : true;
        }

        private void removePatientButton_Click(object sender, RoutedEventArgs e)
        {
            MySqlHelper.removePatientFromSimulation((patientPoolListView.SelectedItem as Patient).Id);
            Patient.refreshPatientPool(SimulationPoolControl.SelectedSimulation.Id);
        }

        private void patientPoolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            removePatientButton.IsEnabled = patientPoolListView.SelectedItem == null ? false : true;
        }
    }
} //End namespace SimPatient