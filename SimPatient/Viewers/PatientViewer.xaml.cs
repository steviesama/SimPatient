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

using PatientGender = SimPatient.Patient.PatientGender;

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for PatientViewer.xaml
	/// </summary>
	public partial class PatientViewer : UserControl
	{
		private long patientId;
		private DateTime date;

		public PatientViewer(long patId, DateTime date)
		{			
			this.InitializeComponent();

			this.patientId = patId;
			this.date = date;

			fillPatientInfo(PatientPoolControl.SelectedPatient);

			MedicationDose.refreshRemainingMedicationDosePool(patientId);
			marPool.DataContext = MedicationAdminstrationRecord.Records;
			MedicationAdminstrationRecord.refreshRecords(patientId, date);

			VisualStateManager.GoToState(medAdminPool, "StationVisualState", false);
			marPool.VisualState = "StationVisualState";
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
			mrIdTextBox.Text = "MR" + pat.Id;
			notesTextBox.Text = pat.Notes;
			if (pat.Gender == PatientGender.Male) optMale.IsChecked = true;
			else optFemale.IsChecked = true;
		}
	} //End class PatientViewer
} //End namespace SimPatient