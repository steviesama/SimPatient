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
	/// Interaction logic for MedicationReconciliationWindow.xaml
	/// </summary>
	public partial class MedicationReconciliationWindow : Window
	{
		private static MedicationAdminstrationRecord Mar { get; set; }

		public MedicationReconciliationWindow()
		{
			this.InitializeComponent();
			Mar = new MedicationAdminstrationRecord();
			this.IsVisibleChanged += MedicationReconciliationWindow_IsVisibleChanged;
		}

		private void MedicationReconciliationWindow_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			if (IsVisible == true)
			{
				if(Mar.ForDose.Schedule != "PRN")
					if (Util.isOnTime(DateTime.Now.TimeOfDay, Mar.ForDose.TimePeriod.TimeOfDay, 15) == false)
						MedicationOffScheduleWindow.getReason(Mar);
			}
		}

		public static void reconcile(MedicationDose dose)
		{			
			MedicationReconciliationWindow recon = new MedicationReconciliationWindow();
			Mar.ForDose = dose;
			Mar.ForPatient = dose.ForPatient;
			Mar.AdministrationTime = DateTime.Now;
			recon.ShowDialog();
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void saveButton_Click(object sender, RoutedEventArgs e)
		{
			if(isInputValid() == false) return;

			Mar.Initials = initialsTextBox.Text;
			Mar.ForDose.InjectionSite = injectionSiteComboBox.SelectedIndex;
			Mar.AdministrationNotes = notesTextBox.Text;

			if (MySqlHelper.connect() == false) return;

			mySqlAddNewMar();
			MedicationDose.refreshRemainingMedicationDosePool(Mar.ForPatient.Id);
			Close();
		}

		private bool isInputValid()
		{
			bool isValid = true;
			Medication med = Mar.ForDose.ForMedication;

			if (Util.validateStringTextBox(initialsTextBox) == false) isValid = false;
			if(med.Route >= 0 && med.Route <= 2)
			{
				if (injectionSiteComboBox.SelectedIndex == 0)
				{
					MessageBox.Show("This dose is an injection.\nAn injection site is required.", "Save Error",
									MessageBoxButton.OK, MessageBoxImage.Error);
					isValid = false;
				}
			}

			return isValid;
		}

		private void mySqlAddNewMar()
		{
			DBConnection dbCon = MySqlHelper.dbCon;

			ArrayList response = dbCon.selectQuery(
			string.Format("SELECT add_mar({0}, {1}, {2}, '{3}', '{4}', {5}, '{6}', '{7}', {8})",
						  Mar.ForDose.Id,
						  Mar.ForPatient.ParentSimulation.Id,
						  Mar.ForPatient.Id,
						  MySqlFunctions.EscapeString(Mar.Initials),
						  Mar.AdministrationTime.ToString("yyyy-MM-dd HH:mm:ss"),
						  Mar.ReasonCode,
						  MySqlFunctions.EscapeString(Mar.AdministrationNotes),
						  MySqlFunctions.EscapeString(Mar.ReasonNotes),
						  Mar.ForDose.InjectionSite));

			MySqlHelper.disconnect();

			MedicationAdminstrationRecord.refreshRecords(Mar.ForPatient.Id, DateTime.Today);
		}
	} //End class MedicationReconciliationWindow
} //End namespace SimPatient