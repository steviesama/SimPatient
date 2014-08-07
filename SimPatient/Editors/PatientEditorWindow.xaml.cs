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
using System.Text.RegularExpressions;
using System.IO;
using System.Printing;
using PatientGender = SimPatient.Patient.PatientGender;

using SUT.PrintEngine;
using SUT.PrintEngine.Utils;

using System.Diagnostics;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for PatientEditorWindow.xaml
	/// </summary>
	public partial class PatientEditorWindow : Window
	{
		public PatientEditorWindow()
		{
			this.InitializeComponent();

			Owner = MainWindow.Instance;
			printBarcodeButton.DataContext = this;

			VisualStateManager.GoToState(medAdminPool, "AdminVisualState", false);

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
			Instance.resetUserControls(true);
			return Instance;
		}

		private static PatientEditorWindow _instance;
		public static PatientEditorWindow Instance
		{
			get
			{
				if (_instance == null)
					_instance = new PatientEditorWindow();


				if (_instance.ActionMode == ActionMode.NewMode && PatientPoolControl.Instance.basedOnCheckBox.IsChecked == true ||
					_instance.ActionMode == ActionMode.EditMode)
				{
					_instance.resetUserControls(false);
					_instance.fillPatientInfo(PatientPoolControl.SelectedPatient);
				}
				else _instance.resetUserControls(true);

				switch(_instance.ActionMode)
				{
					case ActionMode.NewMode:
						_instance.medAdminPool.addButton.IsEnabled = false;
						MedicationDose.MedicationDosePool.Clear();
						break;
					default:
						_instance.medAdminPool.addButton.IsEnabled = true;
						MedicationDose.refreshMedicationDosePool(PatientPoolControl.SelectedPatient.Id);
						break;
				}

				return _instance;
			}
		}
		
		public void fillPatientInfo(Patient pat)
		{
			if (pat == null) return;
			patientNameTextBox.Text = pat.Name;
			dobDatePicker.SelectedDate = pat.DateOfBirth;
			allergiesTextBox.Text = pat.Allergies;
			diagnosisTextBox.Text = pat.Diagnosis;
			drNameTextBox.Text = pat.DrName;
			dietTextBox.Text = pat.Diet;
			roomNumberTextBox.Text = pat.RoomNumber;
			weightTextBox.Text = pat.Weight.ToString();

			if(ActionMode == ActionMode.EditMode) mrIdTextBox.Text = "MR" + pat.Id;
			notesTextBox.Text = pat.Notes;
			if (pat.Gender == PatientGender.Male) optMale.IsChecked = true;
			else optFemale.IsChecked = true;
		}        

		private Patient getPatientInfo()
		{
			Patient pat = new Patient();

			if (isInputValid() == false && Util.validateMRTextBox(mrIdTextBox)) return null;

			pat.Name = patientNameTextBox.Text;
			pat.DateOfBirth = (DateTime)dobDatePicker.SelectedDate;
			pat.Allergies = allergiesTextBox.Text;
			pat.Diagnosis = diagnosisTextBox.Text;
			pat.DrName = drNameTextBox.Text;
			pat.Diet = dietTextBox.Text;
			pat.RoomNumber = roomNumberTextBox.Text;
			//---these next 2 should be ok because of the isInputValid() call up top
			pat.Weight = short.Parse(weightTextBox.Text);
			pat.Id = long.Parse(mrIdTextBox.Text.Substring(2));
			pat.Notes = notesTextBox.Text;
			pat.Gender = (optMale.IsChecked == true ? PatientGender.Male : PatientGender.Female);

			return pat;
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
						//actionButton.Content = "Select";
						break;
					case ActionMode.EditMode:
						//actionButton.Content = "Edit";
						break;
				}
			}
		}

		public void resetUserControls(bool emptyText)
		{
			patientNameTextBox.Background = Brushes.White;
			dobDatePicker.Background = Brushes.White;
			drNameTextBox.Background = Brushes.White;
			roomNumberTextBox.Background = Brushes.White;
			weightTextBox.Background = Brushes.White;
			mrIdTextBox.Background = Brushes.White;
			if (emptyText == true) emptyControls();

			if (ActionMode == ActionMode.EditMode)
			{
				autoIdCheckBox.IsEnabled = false;
				printBarcodeButton.IsEnabled = true;
			}
			else
			{
				autoIdCheckBox.IsEnabled = true;
				printBarcodeButton.IsEnabled = false;
			}

			closeOnSaveCheckBox.IsChecked = true;
			autoIdCheckBox.IsChecked = true;
		}

		internal static void emptyControls()
		{
			_instance.patientNameTextBox.Text = string.Empty;
			_instance.dobDatePicker.SelectedDate = null;
			_instance.drNameTextBox.Text = string.Empty;
			_instance.roomNumberTextBox.Text = string.Empty;
			_instance.weightTextBox.Text = string.Empty;
			_instance.mrIdTextBox.Text = string.Empty;
			_instance.notesTextBox.Text = string.Empty;
			_instance.allergiesTextBox.Text = string.Empty;
			_instance.diagnosisTextBox.Text = string.Empty;
		}

		private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
		{
			//patientNameTextBox.Text = string.Format("Width: {0} Height: {1}", LayoutRoot.ActualWidth, LayoutRoot.ActualHeight);
		}

		private long mySqlAddNewPatient()
		{
			DBConnection dbCon = MySqlHelper.dbCon;

			ArrayList response = dbCon.selectQuery(
			string.Format("SELECT add_patient({10}, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}, '{8}', '{9}')",
						  MySqlFunctions.EscapeString(patientNameTextBox.Text),
						  MySqlFunctions.EscapeString(((DateTime)dobDatePicker.SelectedDate).ToString("yyyy-MM-dd HH:mm:ss")),
						  MySqlFunctions.EscapeString(allergiesTextBox.Text),
						  MySqlFunctions.EscapeString(diagnosisTextBox.Text),
						  MySqlFunctions.EscapeString(drNameTextBox.Text),
						  MySqlFunctions.EscapeString(dietTextBox.Text),
						  MySqlFunctions.EscapeString(roomNumberTextBox.Text),
						  MySqlFunctions.EscapeString(weightTextBox.Text),
						  optMale.IsChecked == true ? "MALE" : "FEMALE",
						  MySqlFunctions.EscapeString(notesTextBox.Text),
						  autoIdCheckBox.IsChecked == true ? "NULL" : mrIdTextBox.Text.Substring(2)));

			MySqlHelper.disconnect();

			if ((long)(response[0] as ArrayList)[0] == -1)
				MessageBox.Show(string.Format("The Medical Record Number {0} is not unique.\nPlease use a unique number.\nPatient not saved.", 
											  mrIdTextBox.Text),
								"Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
			if ((long)(response[0] as ArrayList)[0] == 0)
				MessageBox.Show("Patient not saved.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else Patient.refreshPatients();
			
			return (long)(response[0] as ArrayList)[0];
		}

		private sbyte mySqlUpdatePatient()
		{
			DBConnection dbCon = MySqlHelper.dbCon;

			Patient pat = PatientPoolControl.SelectedPatient;
			if (pat == null) return 0;

			ArrayList response = dbCon.selectQuery(
				string.Format("SELECT update_patient({10}, '{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7}, '{8}', '{9}')",
				MySqlFunctions.EscapeString(patientNameTextBox.Text),
				pat.DateOfBirth.ToString("yyyy-MM-dd HH:mm:ss"),
				MySqlFunctions.EscapeString(allergiesTextBox.Text),
				MySqlFunctions.EscapeString(diagnosisTextBox.Text),
				MySqlFunctions.EscapeString(drNameTextBox.Text),
				MySqlFunctions.EscapeString(dietTextBox.Text),
				MySqlFunctions.EscapeString(roomNumberTextBox.Text),
				MySqlFunctions.EscapeString(weightTextBox.Text),
				optMale.IsChecked == true ? "MALE" : "FEMALE",
				MySqlFunctions.EscapeString(notesTextBox.Text),
				pat.Id)
			);

			MySqlHelper.disconnect();
			
			sbyte result = (sbyte)(response[0] as ArrayList)[0];

			if (result == 0)
				MessageBox.Show("Patient not updated.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else Patient.refreshPatients();

			return result;
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			if (isInputValid() == false)
			{
				MessageBox.Show("Invalid input entered.\nPlease correct errors marked by red fields.",
								"Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			if (MySqlHelper.connect() == false) return;

			long newId = -1;

			if (ActionMode == ActionMode.NewMode)
			{
				newId = mySqlAddNewPatient();
				if (newId > 0) mrIdTextBox.Text = "MR" + newId;
				PatientPoolControl.SelectedPatient = Patient.fromMySqlPatient(newId);
				fillPatientInfo(PatientPoolControl.SelectedPatient);
			}
			else newId = mySqlUpdatePatient();

			//if close on save is checked and a good newId is set, hide the window
			if (closeOnSaveCheckBox.IsChecked == true && newId > 0) Hide();
			//else it's still showing, so change to edit mode in case save is clicked again
			else
			{
				//change to edit mode in case save is clicked again
				ActionMode = ActionMode.EditMode;
				//reset user controls...access Instance to update med admin pool
				Instance.resetUserControls(false);
			}
		} //End saveButton_Click()

		public bool isInputValid()
		{
			bool isValid = true;

			if (Util.validateStringTextBox(patientNameTextBox) == false) isValid = false;
			if (Util.validateDatePicker(dobDatePicker) == false) isValid = false;
			if (Util.validateStringTextBox(drNameTextBox) == false) isValid = false;
			if (Util.validateStringTextBox(roomNumberTextBox) == false) isValid = false;
			if (Util.validateNumberTextBox(weightTextBox) == false) isValid = false;
			if (autoIdCheckBox.IsChecked == false)
			{
				if (Util.validateMRTextBox(mrIdTextBox) == false) isValid = false;
			}
			else mrIdTextBox.Background = Brushes.White;

			return isValid;
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Hide();
		}

		/// <summary>
		/// Be sure to add code to verify that the essential data fields are filled out before allowing
		/// a print to occur.
		/// </summary>
		private void printBarcodeButton_Click(object sender, RoutedEventArgs e)
		{
			printBarcode();
		}

		private void printBarcode()
		{
			if (getPatientInfo() == null)
			{
				MessageBox.Show("Invalid input entered.\nPlease correct errors marked by red fields.",
								"Input Error", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}

			DrawingVisual visual = Util.renderPatientBarcode(getPatientInfo());

			var visualSize = new Size(visual.ContentBounds.Width, visual.ContentBounds.Height);
			var printControl = PrintControlFactory.Create(visualSize, visual);
	
			printControl.ShowPrintPreview();
		}
	} //End class PatientEditorWindow
} //End namespace SimPatient