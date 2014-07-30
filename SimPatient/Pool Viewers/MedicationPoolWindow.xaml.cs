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

namespace SimPatient
{
	/// <summary>
	/// Interaction logic for MedicationPoolWindow.xaml
	/// </summary>
	public partial class MedicationPoolWindow : Window
	{
		public static Medication SelectedMedication { get; set; }

		public MedicationPoolWindow()
		{
			this.InitializeComponent();
			Owner = MainWindow.Instance;
			medicationPoolListView.DataContext = Medication.MedicationPool;
			Medication.refreshMedicationPool();            

			this.Closing += MedicationPoolWindow_Closing;
		}

		private void MedicationPoolWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
		{
            e.Cancel = true;
            MedicationDose.refreshMedicationDosePool(PatientPoolControl.SelectedPatient.Id);
			Hide();
		}

		/// <summary>
		/// The UserControl that navigated to this control.
		/// </summary>
		public static UserControl ParentControl { get; set; }

		public static MedicationPoolWindow getInstance(UserControl parentControl)
		{
			PatientEditorWindow.ParentControl = parentControl;
			return Instance;
		}

		public static MedicationPoolWindow getEmptyInstance(UserControl parentControl)
		{
			MedicationPoolWindow.ParentControl = parentControl;
			//emptyControls();
			return Instance;
		}

		private static MedicationPoolWindow _instance;
		public static MedicationPoolWindow Instance
		{
			get
			{
				if (_instance == null)
					_instance = new MedicationPoolWindow();

				return _instance;
			}
		}

		private void cancelButton_Click(object sender, RoutedEventArgs e)
		{
            //close is intercepted and the refresh of the dose pool is called
            Close();
		}

		private void newButton_Click(object sender, RoutedEventArgs e)
		{
			Medication.refreshMedicationPool();
            //implicitly sets new mode
			MedicationEditorWindow.EmptyInstance.ShowDialog();
		}

		private void medicationPoolListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if ((sender as ListView).SelectedItem != null)
			{
				SelectedMedication = (sender as ListView).SelectedItem as Medication;
				selectButton.IsEnabled = editButton.IsEnabled = true;
			}
			else selectButton.IsEnabled = editButton.IsEnabled = false;
		}

		private void selectButton_Click(object sender, RoutedEventArgs e)
		{
			Hide();
			MedicationDoseEditorWindow.EmptyInstance.ShowDialog();
		}

		private void editButton_Click(object sender, RoutedEventArgs e)
		{
            //implicitly sets edit mode
            MedicationEditorWindow.fillMedicationInfo(SelectedMedication);
            MedicationEditorWindow.Instance.ShowDialog();
		}
	} //End class MedicationPoolWindow
} //End namespace SimPatient