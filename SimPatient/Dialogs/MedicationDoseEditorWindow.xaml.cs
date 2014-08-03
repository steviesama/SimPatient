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

using System.Collections;
using MySqlFunctions = MySql.Data.MySqlClient.MySqlHelper;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for MedicationDoseEditorWindow.xaml
	/// </summary>
	public partial class MedicationDoseEditorWindow : Window
	{
		public MedicationDoseEditorWindow()
		{
			this.InitializeComponent();
			Owner = MainWindow.Instance;
			ActionMode = ActionMode.NewMode;
			this.Closing += MedicationDoseEditorWindow_Closing;
		}

		private void MedicationDoseEditorWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
			e.Cancel = true;
            Hide();

		}

		private static MedicationDoseEditorWindow _instance;
		public static MedicationDoseEditorWindow Instance
		{
			get
			{
				if (_instance == null)
					_instance = new MedicationDoseEditorWindow();

				return _instance;
			}
		}

		public static MedicationDoseEditorWindow EmptyInstance
		{
			get
			{
				emptyControls();
                Instance.ActionMode = ActionMode.NewMode;
				return Instance;
			}
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

		internal static void emptyControls()
		{
			Instance.scheduleTextBox.Text = string.Empty;
			Instance.timePeriodTextBox.Text = string.Empty;
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

			Hide();
			switch(ActionMode)
			{
				case ActionMode.NewMode:
					mySqlAddNewMedicationDose();
					break;
				case ActionMode.EditMode:
					mySqlUpdateMedicationDose();
					break;
			}

		}

		public bool isInputValid()
		{
			bool isValid = true;

            //ensure schedule code is uppercase
            scheduleTextBox.Text = scheduleTextBox.Text.ToUpper().Trim();

			if (Util.validateStringTextBox(scheduleTextBox) == false) isValid = false;
			if (Util.validateTimeTextBox(timePeriodTextBox) == false) isValid = false;

			return isValid;
		}

		public static void fillDoseInfo(MedicationDose dose)
		{
			if (dose == null) return;
			Instance.scheduleTextBox.Text = dose.Schedule;
			Instance.timePeriodTextBox.Text = dose.TimePeriod.ToString("HHmm");
			Instance.ActionMode = ActionMode.EditMode;
		}        

		private long mySqlAddNewMedicationDose()
		{
			DBConnection dbCon = MySqlHelper.dbCon;

			Patient pat = PatientPoolControl.SelectedPatient;
			Medication med = MedicationPoolWindow.SelectedMedication;

			ArrayList response = dbCon.selectQuery(
			string.Format("SELECT add_dose({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7})",
						  "NULL",
						  med.Id,
						  pat.Id,
						  0,
						  scheduleTextBox.Text,
						  Convert.timeStrToMySqlDateStr(timePeriodTextBox.Text),
						  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						  "NULL"));

			MySqlHelper.disconnect();

			if ((long)(response[0] as ArrayList)[0] == 0)
				MessageBox.Show("Medication dose not saved.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else MedicationDose.refreshMedicationDosePool(PatientPoolControl.SelectedPatient.Id);

			return (long)(response[0] as ArrayList)[0];
		}

		private sbyte mySqlUpdateMedicationDose()
		{
			DBConnection dbCon = MySqlHelper.dbCon;

			MedicationDose dose = MedicationAdministrationControl.SelectedDose;
			Patient pat = dose.ForPatient;
			Medication med = dose.ForMedication;

			ArrayList response = dbCon.selectQuery(
			string.Format("SELECT update_dose({0}, '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', {7})",
						  dose.Id,
						  med.Id,
						  pat.Id,
						  0,
						  scheduleTextBox.Text,
						  Convert.timeStrToMySqlDateStr(timePeriodTextBox.Text),
						  DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
						  "NULL"));

			MySqlHelper.disconnect();

			if ((sbyte)(response[0] as ArrayList)[0] == 0)
				MessageBox.Show("Medication dose not updated.", "Update Error", MessageBoxButton.OK, MessageBoxImage.Error);
			else MedicationDose.refreshMedicationDosePool(PatientPoolControl.SelectedPatient.Id);

			return (sbyte)(response[0] as ArrayList)[0];
		}

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Hide();
        }
	} //End class MedicationDoseEditorWindow
} //End namespace SimPatient