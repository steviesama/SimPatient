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

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for MedicationAdministrationControl.xaml
	/// </summary>
	public partial class MedicationAdministrationControl : UserControl
	{
		public static MedicationDose SelectedDose { get; set; }

		public MedicationAdministrationControl()
		{
			this.InitializeComponent();
			medicationPoolListView.DataContext = MedicationDose.MedicationDosePool;
		}

		private string _visualState;
		public string VisualState
		{
			get { return _visualState; }
			set
			{
				VisualStateManager.GoToState(this, value, false);
				_visualState = value;
			}
		}

		private void addButton_Click(object sender, RoutedEventArgs e)
		{
			SelectedDose = null;
			Medication.refreshMedicationPool();
			MedicationPoolWindow.Instance.ShowDialog();
		}

		private void editButton_Click(object sender, RoutedEventArgs e)
		{
			MedicationDoseEditorWindow.fillDoseInfo(SelectedDose);
			MedicationDoseEditorWindow.Instance.ShowDialog();
		}

		private void selectButton_Click(object sender, RoutedEventArgs e)
		{
			if (ScanVerifyWindow.verifyMedication(SelectedDose.ForMedication) == true)
				MedicationReconciliationWindow.reconcile(SelectedDose);
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
			MainWindow.Instance.loadBottomGrid(PatientPoolControl.Instance);
		}

		private void medicationPoolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((sender as ListView).SelectedItem != null)
			{
				SelectedDose = (sender as ListView).SelectedItem as MedicationDose;
				editButton.IsEnabled = removeButton.IsEnabled = selectButton.IsEnabled = true;

			}
			else editButton.IsEnabled = removeButton.IsEnabled = selectButton.IsEnabled = false;
		}

		private void removeButton_Click(object sender, RoutedEventArgs e)
		{
			MySqlHelper.removeMedicationDose(SelectedDose.Id);
			MedicationDose.refreshMedicationDosePool(SelectedDose.ForPatient.Id);
		}
	} //End class MedicationAdministrationControl
} //End namespace SimPatient