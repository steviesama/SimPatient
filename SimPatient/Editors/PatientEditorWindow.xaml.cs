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

using System.Collections.ObjectModel;
using System.Collections;
using System.Windows.Threading;
using MySqlFunctions = MySql.Data.MySqlClient.MySqlHelper;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for PatientEditorWindow.xaml
	/// </summary>
	public partial class PatientEditorWindow : Window
	{
        private ObservableCollection<MedicationSchedule> medicationSchedules;

		public PatientEditorWindow()
		{
			this.InitializeComponent();

            medicationSchedules = new ObservableCollection<MedicationSchedule>();
            medicationPoolListView.DataContext = medicationSchedules;

            medicationSchedules.Add(new MedicationSchedule
            {
                Name = "Practi-Hydro",
                Strength = "50mg",
                Route = "PO",
                Schedule = "QHS",
                Start = "08/16/2011",
                Stop = "",
                FirstPeriod = "",
                SecondPeriod = "", 
                ThirdPeriod = "2100"
            });

            medicationSchedules.Add(new MedicationSchedule
            {
                Name = "Practi-Empco",
                Strength = "30mg",
                Route = "PO",
                Schedule = "QD",
                Start = "08/16/2011",
                Stop = "",
                FirstPeriod = "",
                SecondPeriod = "0900",
                ThirdPeriod = ""
            });

            this.Closing += PatientEditorWindow_Closing;
		}

        private void PatientEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        /// <summary>
        /// The UserControl that navigated to this control.
        /// </summary>
        public static UserControl ParentControl { get; set; }

        public static PatientEditorWindow getInstance(UserControl parentControl)
        {
            PatientEditorWindow.ParentControl = parentControl;
            return Instance;
        }

        public static PatientEditorWindow getEmptyInstance(UserControl parentControl)
        {
            PatientEditorWindow.ParentControl = parentControl;
            return EmptyInstance;
        }

        private static PatientEditorWindow _instance;
        public static PatientEditorWindow Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PatientEditorWindow();

                //_instance.simulationNameTextBox.Background = Brushes.White;

                //_instance.Dispatcher.BeginInvoke(new Action<UIElement>(x =>
                //{
                //    if (SimulationPoolControl.Instance.ActionMode == ActionMode.EditMode)
                //    {
                //        Simulation sim = SimulationPoolControl.SelectedSimulation;
                //        if (sim != null)
                //        {
                //            _instance.simulationNameTextBox.Text = sim.Name;
                //            _instance.simulationDescriptionTextBox.Text = sim.Description;
                //        }
                //    }

                //    _instance.simulationNameTextBox.Focus();
                //}), DispatcherPriority.ApplicationIdle, _instance);

                //if (SimulationPoolControl.SelectedSimulation == null)
                //{
                //    _instance.addAccountButton.IsEnabled = false;
                //    _instance.addPatientButton.IsEnabled = false;
                //}
                //else
                //{
                //    _instance.addAccountButton.IsEnabled = true;
                //    _instance.addPatientButton.IsEnabled = true;
                //}

                return _instance;
            }
        }
                                                                                            
        public static PatientEditorWindow EmptyInstance
        {
            get
            {
                emptyControls();
                return Instance;
            }
        }

        private static void emptyControls()
        {
            //Instance.simulationNameTextBox.Text = "";
            //Instance.simulationDescriptionTextBox.Text = "";
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            //patientNameTextBox.Text = string.Format("Width: {0} Height: {1}", LayoutRoot.ActualWidth, LayoutRoot.ActualHeight);
        }

        private void mySqlAddNewPatient()
        {
            DBConnection dbCon = MySqlHelper.dbCon;
            ArrayList response = dbCon.selectQuery(
            string.Format("SELECT add_patient(NULL, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}, '{8}', '{9}')",
                          MySqlFunctions.EscapeString(patientNameTextBox.Text),
                          MySqlFunctions.EscapeString(((DateTime)dobDatePicker.SelectedDate).ToString("yyyy-MM-dd HH:mm:ss")),
                          MySqlFunctions.EscapeString(allergiesTextBox.Text),
                          MySqlFunctions.EscapeString(diagnosisTextBox.Text),
                          MySqlFunctions.EscapeString(drNameTextBox.Text),
                          MySqlFunctions.EscapeString(dietTextBox.Text),
                          MySqlFunctions.EscapeString(roomNumberTextBox.Text),
                          MySqlFunctions.EscapeString(weightTextBox.Text),
                          optMale.IsChecked == true ? "MALE" : "FEMALE",
                          MySqlFunctions.EscapeString(notesTextBox.Text)));

            MySqlHelper.disconnect();

            if ((long)(response[0] as ArrayList)[0] == 0)
                MessageBox.Show("Patient not saved.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else Patient.refreshPatients();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            //if (simulationNameTextBox.Text.Trim() == string.Empty)
            //{
            //    simulationNameTextBox.Background = Brushes.LightPink;
            //    MessageBox.Show("No valid simulation name entered.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
            //    simulationNameTextBox.Focus();
            //    return;
            //}
            //else simulationNameTextBox.Background = Brushes.White;

            if (MySqlHelper.connect() == false) return;

            if (PatientPoolControl.SelectedPatient == null)
                mySqlAddNewPatient();
            //else mySqlUpdateSimulation();

            Hide();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
	} //End class PatientEditorWindow

    public class MedicationSchedule
    {
        public string Name { get; set; }
        public string Strength { get; set; }
        public string Route { get; set; }
        public string Schedule { get; set; }
        public string Start { get; set; }
        public string Stop { get; set; }
        public string FirstPeriod { get; set; }
        public string SecondPeriod { get; set; }
        public string ThirdPeriod { get; set; }
    }
} //End namespace SimPatient