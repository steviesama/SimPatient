using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace SimPatient
{
    /// <summary>
    /// Interaction logic for PatientPoolControl.xaml
    /// </summary>
    public partial class PatientPoolControl : UserControl
    {
        public static Patient SelectedPatient { get; set; }

        private static string _visualState;
        public static string VisualState
        {
            get { return _visualState; }
            set
            {
                VisualStateManager.GoToState(PatientPoolControl.Instance, value, false);
                _visualState = value;
                refresh();
            }
        }

        public static void refresh()
        {
            if (_visualState == "AdminVisualState")
            {
                PatientPoolControl.Instance.DataContext = Patient.Patients;
                Patient.refreshPatients();
            }
            else if (MainWindow.CurrentUser.ParentSimulation == null)
            {
                PatientPoolControl.Instance.DataContext = Patient.PatientPool;
                Patient.PatientPool.Clear();
            }
            else
            {
                PatientPoolControl.Instance.DataContext = Patient.PatientPool;
                Patient.refreshPatientPool(MainWindow.CurrentUser.ParentSimulation.Id);
            }
        }

        private static PatientPoolControl _instance;
        public static PatientPoolControl Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new PatientPoolControl();
                
                return _instance;
            }
        }

        /// <summary>
        /// The UserControl that navigated to this control.
        /// </summary>
        public static UserControl ParentControl { get; set; }

        public static PatientPoolControl getInstance(UserControl parentControl, string visualState)
        {
            Instance.basedOnCheckBox.IsChecked = false;
            PatientPoolControl.ParentControl = parentControl;
            PatientPoolControl.VisualState = visualState;
            return Instance;
        }

        private PatientPoolControl()
        {
            InitializeComponent();

            //default to select mode
            ActionMode = ActionMode.SelectMode;
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
                        selectButton.Content = "Select";
                        break;
                    case ActionMode.EditMode:
                        selectButton.Content = "Edit";
                        break;
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow.Instance.loadBottomGrid(ParentControl);
        }

        private void newButton_Click(object sender, RoutedEventArgs e)
        {
            PatientEditorWindow.Instance.ActionMode = ActionMode.NewMode;
            PatientEditorWindow.Instance.ShowDialog();
        }

        private void selectButton_Click(object sender, RoutedEventArgs e)
        {
            switch (PatientPoolControl.VisualState)
            {
                case "AdminVisualState":
                    MySqlHelper.addPatientToSimulation(SimulationPoolControl.SelectedSimulation.Id, SelectedPatient.Id);
                    Patient.refreshPatientPool(SimulationPoolControl.SelectedSimulation.Id);
                    UserAccount.refreshUserAccountPool(SimulationPoolControl.SelectedSimulation.Id);
                    MainWindow.Instance.loadBottomGrid(ParentControl);
                    break;
                case "StationVisualState":
                    if (ScanVerifyWindow.verifyPatient(PatientPoolControl.SelectedPatient) == true)
                        MainWindow.Instance.loadBottomGrid(new PatientViewer(PatientPoolControl.SelectedPatient.Id, DateTime.Today));
                    break;
            }
        }

        private void patientPoolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ListView).SelectedItem != null)
            {
                SelectedPatient = (sender as ListView).SelectedItem as Patient;
                selectButton.IsEnabled = editButton.IsEnabled = deleteButton.IsEnabled = true;
            }
            else selectButton.IsEnabled = editButton.IsEnabled = deleteButton.IsEnabled = false;
        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            PatientEditorWindow.Instance.ActionMode = ActionMode.EditMode;
            MedicationDose.refreshMedicationDosePool(PatientPoolControl.SelectedPatient.Id);
            PatientEditorWindow.Instance.ShowDialog();
        }

        private void deleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBoxResult.No == MessageBox.Show("Are you sure you want to delete this patient?",
                                                       "Confirmation", MessageBoxButton.YesNo,
                                                       MessageBoxImage.Warning)) return;
            if (MySqlHelper.connect() == false) return;

            if (MySqlHelper.dbCon.deleteQuery("DELETE FROM tblPatient WHERE id=" +
                                              (patientPoolListView.SelectedItem as Patient).Id) == false)
                MessageBox.Show("Delete is not possible.\nPatient is referenced by other records in the database.",
                                "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
            else Patient.refreshPatients();

            MySqlHelper.disconnect();
        }
    } //End class PatientPoolControl
} //End namespace SimPatient