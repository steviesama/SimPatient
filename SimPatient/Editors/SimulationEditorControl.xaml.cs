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
        ObservableCollection<Patient> patients;

        public static Simulation ToEdit { get; set; }

        private static SimulationEditorControl _simulationEditorControl;
        public static SimulationEditorControl Instance
        {
            get
            {
                if (_simulationEditorControl == null)
                    _simulationEditorControl = new SimulationEditorControl();

                _simulationEditorControl.Dispatcher.BeginInvoke(new Action<UIElement>(x =>
                {
                    _simulationEditorControl.simulationNameTextBox.Focus();
                }), DispatcherPriority.ApplicationIdle, _simulationEditorControl);

                return _simulationEditorControl;
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
            patients = new ObservableCollection<Patient>();
            patientPoolListView.DataContext = patients;

            patients.Add(new Patient { Name = "Twitter, Dora", DateOfBirth = new DateTime(1950, 1, 9), DrName = "Harko", Id = 789987 });
            patients.Add(new Patient { Name = "Watson, Josh", DateOfBirth = new DateTime(1986, 12, 30), DrName = "Ioda", Id = 123456 });
            patients.Add(new Patient { Name = "Taylor, Casi", DateOfBirth = new DateTime(1986, 6, 13), DrName = "Jeffreys", Id = 654321 });
        }

        private static void emptyControls()
        {
            Instance.simulationNameTextBox.Text = "";
            Instance.simulationDescriptionTextBox.Text = "";

        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.loadBottomGrid(SimulationPoolControl.Instance);
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (MySqlHelper.connect() == false) return;
            
            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery(string.Format("SELECT add_simulation(NULL, '{0}', '{1}', NULL, 0, '{2}')",
                                                                 MySqlFunctions.EscapeString(simulationNameTextBox.Text),
                                                                 MySqlFunctions.EscapeString(simulationDescriptionTextBox.Text),
                                                                 MySqlFunctions.EscapeString(MainWindow.CurrentUser.Username)));

            MySqlHelper.disconnect();

            if ((long)(response[0] as ArrayList)[0] == 0)
                MessageBox.Show("Simulation not saved.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else Simulation.refreshSimulations();
            
            MainWindow.Instance.loadBottomGrid(SimulationPoolControl.Instance);
        }
    }
} //End namespace SimPatient